using H.Core.Enumerations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SharpKml.Dom.Xal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace H.Avalonia.Services
{
    public class NominatimGeocoderService : IDefaultGeocoderService
    {
        #region Fields

        private const int Timeout = 60;
        private ILogger _logger;

        #endregion

        #region Properties

        #endregion

        #region Constructors

        public NominatimGeocoderService(ILogger logger)
        {
            if (logger != null)
            {
                _logger = logger;
            }
            else
            {
                throw new ArgumentNullException(nameof(logger));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if the address is already cached.
        /// </summary>
        /// <param name="address">The address to check if a cache exists.</param>
        /// <returns></returns>
        public bool IsCached(string address)
        {
            var path = this.GetCachePath(address);
            return File.Exists(path);
        }

        /// <summary>
        /// Returns the latitude and longitude for the given address.
        /// </summary>
        /// <param name="address">The address to geocode and get coordinates for.</param>
        /// <returns>Longitude and latitude coordinates.</returns>
        public async Task<(double latitude, double longitude)> GetCoordinates(string street, string municipality, Province province, string country, string postalCode)
        {
            street = PrepareStreetStringForAPI(street);
            // If no cached data, get data from Nominatim API and cache it.
            string content = this.GetCachedData(street + municipality + province.ToString() + country + postalCode);
            if (string.IsNullOrWhiteSpace(content))
            {
                content = await GetAndCacheNominatimData(street, municipality, province, country, postalCode);
            }
            // If cached data or API data was available, parse it and return coordinates.
            if (!string.IsNullOrWhiteSpace(content))
            {
                return ParseNominatimApiContentForCoordinates(content);
            }
            // Hit if there was an error downloading data from the API and no cached data available.
            else
            {
                _logger.LogError($"{nameof(NominatimGeocoderService)}: there was an error while trying to download Nominatim coordinates data,");
                return (0, 0);
            }
        }

        /// <summary>
        /// Returns the province for the given address.
        /// </summary>
        /// <param name="address">The address to geocode and get the province for.</param>
        /// <returns>Province enum of where the address is located.</returns>
        public async Task<Province?> GetProvince(string street, string municipality, Province province, string country, string postalCode)
        {
            street = PrepareStreetStringForAPI(street);
            string content = this.GetCachedData(street + municipality + province.ToString() + country + postalCode);
            // If no cached data, get data from Nominatim API and cache it.
            if (string.IsNullOrWhiteSpace(content))
            {
               content = await GetAndCacheNominatimData(street, municipality, province, country, postalCode);
            }
            // If cached data or API data was available, parse it and return coordinates.
            if (!string.IsNullOrWhiteSpace(content))
            {
                return ParseNominatimApiContentForProvince(content);
            }
            // Hit if there was an error downloading data from the API and no cached data available.
            else
            {
                _logger.LogError($"{nameof(NominatimGeocoderService)}: there was an error while trying to download Nominatim coordinates data,");
                return (null);
            }
        }

        #endregion

        #region Private Methods

        private string PrepareStreetStringForAPI(string address)
        {
            // Dictionary containing directions that can be converted to their abbreviated counterpart
            // Nominatim requires street name direction suffix be abbreviated
            // Ex. "South Parkside Drive South" returns invalid, "South Parkside Drive S" or "S Parkside Drive S" returns valid coordinates
            var directions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"northwest", "NW"},
                {"northeast", "NE"},
                {"southwest", "SW"},
                {"southeast", "SE"},
                {"north", "N"},
                {"south", "S"},
                {"east", "E"},
                {"west", "W"}
            };
            // Only match pattern when word is isolated by space or punctuation. 
            // Ex. "StreetName South, Alberta" becomes "Streetname S, Alberta". "Southstreet Avenue, Alberta" remains the same.
            var pattern = @"\b(" + string.Join("|", directions.Keys.OrderByDescending(k => k.Length)) + @")\b";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            string convertedAddress = regex.Replace(address, match =>
            {
                var key = match.Value.ToLower();
                return directions.ContainsKey(key) ? directions[key] : match.Value;
            });
            return convertedAddress;
        }

        /// <summary>
        /// Gets the correct Nominatim API URL for the given address.
        /// </summary>
        /// <param name="address">The address to format for a Nominatim API call.</param>
        /// <returns>Returns the url needed to access the API for the given address.</returns>
        private string GetCorrectApiUrl(string address)
        {
            address = Uri.EscapeDataString(address);
            var format = "json";
            var Url = $"https://nominatim.openstreetmap.org/search?q={address}&format={format}&addressdetails=1&limit=1";
            return Url;
        }

        /// <summary>
        /// Gets the correct Nominatim API URL for the given address.
        /// </summary>
        /// <param name="address">The address to format for a Nominatim API call.</param>
        /// <returns>Returns the url needed to access the API for the given address.</returns>
        private string GetCorrectApiUrl(string street, string municipality, Province province, string country, string postalCode)
        {
            street = Uri.EscapeDataString(street);
            municipality = Uri.EscapeDataString(municipality);
            string provinceString = Uri.EscapeDataString(province.ToString());
            country = Uri.EscapeDataString(country);
            postalCode = Uri.EscapeDataString(postalCode);
            var format = "json";
            var Url = $"https://nominatim.openstreetmap.org/search?street={street}&city{municipality}&state={provinceString}&postalcode={postalCode}&format={format}&addressdetails=1&limit=1&countrycodes=ca";
            return Url;
        }

        /// <summary>
        /// Returns geocoding data for the specified address from the Nominatim API and caches the result.
        /// </summary>
        /// <param name="address">The address to geocode. Must be a non-empty string representing a valid location.</param>
        /// <returns>A JSON string containing the geocoding data for the specified address if the API call is successful;
        /// otherwise, returns null.</returns>
        private async Task<string> GetAndCacheNominatimData(string street, string municipality, Province province, string country, string postalCode)
        {
            string apiUrl = GetCorrectApiUrl(street, municipality, province, country, postalCode);
            string content = null;
            try 
            {
                    // Run a task that forces the Nominatim API to timeout if the timeout property isn't able to gracefully time out the API call. If the API
                    // does not respond within this time (slow internet connection or API issues), we return null.
                    var getNominatimApi = Task.Run(() => DownloadNominatimApiData(apiUrl));
                    if (getNominatimApi.Wait(TimeSpan.FromSeconds(Timeout)))
                    {
                        _logger.LogInformation($"{nameof(NominatimGeocoderService)}.{nameof(GetCoordinates)}, Nominatim API Task Status: {getNominatimApi.Status}");
                        content = getNominatimApi.Result;
                        // Check if content returned by API is empty, if so do not return
                        if (content == "[]")
                        {
                            _logger.LogError($"{nameof(NominatimGeocoderService)}.{nameof(DownloadNominatimApiData)}: API content empty. Address not valid.");
                            return null;
                        }
                        CacheData(street + municipality + province + country + postalCode, content);
                    }
                    else
                    {
                        _logger.LogError($"{nameof(NominatimGeocoderService)}.{nameof(GetCoordinates)}, Nominatim API Task Status: {getNominatimApi.Status}");
                        throw new Exception("Nominatim API couldn't be reached or connection timed out.");
                    }
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Could not load data from Nominatim API. Exception thrown.");
                _logger.LogError($"Exception occurred in {nameof(NominatimGeocoderService)}.{nameof(GetCoordinates)}. Exception message: {e.Message}");
                _logger.LogError($"Inner Exception message: {e.InnerException}");
                _logger.LogInformation($"Returning null Data.");
            }
            return content;
        }

        /// <summary>
        /// Returns geocoding data for the specified address from the Nominatim API and caches the result.
        /// </summary>
        /// <param name="address">The address to geocode. Must be a non-empty string representing a valid location.</param>
        /// <returns>A JSON string containing the geocoding data for the specified address if the API call is successful;
        /// otherwise, returns null.</returns>
        private async Task<string> GetAndCacheNominatimData(string address)
        {
            string apiUrl = GetCorrectApiUrl(address);
            string content = null;
            try
            {
                // Run a task that forces the Nominatim API to timeout if the timeout property isn't able to gracefully time out the API call. If the API
                // does not respond within this time (slow internet connection or API issues), we return null.
                var getNominatimApi = Task.Run(() => DownloadNominatimApiData(apiUrl));
                if (getNominatimApi.Wait(TimeSpan.FromSeconds(Timeout)))
                {
                    _logger.LogInformation($"{nameof(NominatimGeocoderService)}.{nameof(GetCoordinates)}, Nominatim API Task Status: {getNominatimApi.Status}");
                    content = getNominatimApi.Result;
                    // Check if content returned by API is empty, if so do not return
                    if (content == "[]")
                    {
                        _logger.LogError($"{nameof(NominatimGeocoderService)}.{nameof(DownloadNominatimApiData)}: API content empty. Address not valid.");
                        return null;
                    }
                    CacheData(address, content);
                }
                else
                {
                    _logger.LogError($"{nameof(NominatimGeocoderService)}.{nameof(GetCoordinates)}, Nominatim API Task Status: {getNominatimApi.Status}");
                    throw new Exception("Nominatim API couldn't be reached or connection timed out.");
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Could not load data from Nominatim API. Exception thrown.");
                _logger.LogError($"Exception occurred in {nameof(NominatimGeocoderService)}.{nameof(GetCoordinates)}. Exception message: {e.Message}");
                _logger.LogError($"Inner Exception message: {e.InnerException}");
                _logger.LogInformation($"Returning null Data.");
            }
            return content;
        }

        /// <summary>
        /// Given a Nominatim API URL, downloads the data from the API.
        /// </summary>
        /// <param name="url">The URL used to access the Nominatim API.</param>
        /// <returns>Returns JSON array in string format from Nominatim's API.</returns>
        private async Task<string> DownloadNominatimApiData(string url)
        {
            // using HttpClient over WebClient as more features supported in HttpClient.
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("holos-aafc-v5 (Holos@agr.gc.ca)");
            // Download the string content from the Nominatim API
            var content = await _httpClient.GetStringAsync(url);

            if (content != string.Empty)
            {
                _logger.LogInformation($"{nameof(NominatimGeocoderService)}.{nameof(DownloadNominatimApiData)} : API content downloaded successfully.");
            }
            else
            {
                _logger.LogError($"{nameof(NominatimGeocoderService)}.{nameof(DownloadNominatimApiData)}, API content could be not downloaded.");
                _logger.LogError($"{nameof(NominatimGeocoderService)}.{nameof(DownloadNominatimApiData)}, API url: {url}");
            }
            return content;
        }

        /// <summary>
        /// Based off the address, returns the cache path for the address.
        /// </summary>
        /// <param name="address">The address the path will be based off of.</param>
        /// <returns>The path based off of the given address.</returns>
        private string GetCachePath(string address)
        {
            // Sanitize address for file name, replace common address characters with underscores.
            var invalidCharacters = Path.GetInvalidFileNameChars();
            var cleanedFileName = invalidCharacters.Aggregate(address, (current, c) => current.Replace(c, '_')).Replace(" ", "_").Replace(",", "");
            var filename = $"nominatim_geocoder_data_address_{cleanedFileName}";

            var path = Path.GetTempPath();
            return Path.Combine(path, filename);
        }

        /// <summary>
        /// Retrieves cached data for the given address if it exists.
        /// </summary>
        /// <param name="address">The address tied to the cached file.</param>
        /// <returns>Returns JSON array in string format from a previous Nominatim API call.</returns>
        private string GetCachedData(string address)
        {
            var path = GetCachePath(address);
            if (File.Exists(path))
            {
                _logger.LogInformation($"Loading cached Nominatim Geocoder data for address: {address}");
                return File.ReadAllText(path);
            }
            return null;
        }

        /// <summary>
        /// Caches the Nominatim API data for the given address.
        /// </summary>
        /// <param name="address">The address tied to the cached file.</param>
        /// <param name="content">The content returned from the API call tied to the address.</param>
        private void CacheData(string address, string content)
        {
            _logger.LogInformation($"Caching Nominatim Geocoder data for address: {address}");
            var path = GetCachePath(address);
            File.WriteAllText(path, content);
        }

        /// <summary>
        /// Parses the Nominatim API content for latitude and longitude coordinates.
        /// </summary>
        /// <param name="content">The content from the API call to be parsed for longitude and latitude data.</param>
        /// <returns>Two doubles representing the longitude and latitude.</returns>
        private (double latitude, double longitude) ParseNominatimApiContentForCoordinates(string content)
        {
            // Initially read as JArray since Nominatim returns an array of one JSON object.
            JObject jObject = JArray.Parse(content).FirstOrDefault() as JObject;
            // Access properties from the JObject
            var lat = double.Parse(jObject["lat"]?.ToString());
            var lon = double.Parse(jObject["lon"]?.ToString());
            return (latitude: lat, longitude: lon);
        }

        /// <summary>
        /// Parses the Nominatim API content for province information.
        /// </summary>
        /// <param name="content">The content from the API call to be parsed for provincial data.</param>
        /// <returns>Province enum based off of the given API data.</returns>
        private Province? ParseNominatimApiContentForProvince(string content)
        {
            // Initially read as JArray since Nominatim returns an array of one JSON object.
            JObject jObject = JArray.Parse(content).FirstOrDefault() as JObject;

            // Access province from the JObject
            var provinceString = jObject["address"]?["province"]?.ToString().Replace(" ", ""); // Ontario returns as province type from Nominatim/OpenStreetMap
            if (provinceString == null)
            {
                provinceString = jObject["address"]?["state"]?.ToString().Replace(" ", ""); // All other provinces/territories return as state type
            }

            // Correct province string to match Province enum name
            if (provinceString == "NewfoundlandandLabrador")
                provinceString = "Newfoundland";

            // Convert province string to Province enum
            if (Enum.IsDefined(typeof(Province), provinceString))
                return (Province)Enum.Parse(typeof(Province), provinceString);
            else
                return null;
        }

        #endregion
    }
}