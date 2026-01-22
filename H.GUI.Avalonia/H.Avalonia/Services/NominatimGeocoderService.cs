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
using Avalonia.Controls.Notifications;
using Path = System.IO.Path;

namespace H.Avalonia.Services
{
    public class NominatimGeocoderService : IDefaultGeocoderService
    {
        #region Fields

        private ILogger _logger;
        private INotificationManagerService _notificationManagerService;

        private const int ApiTimeout = 60;
        private const int ApiLockoutSeconds = 5;

        private DateTime _lastApiRequestTime = DateTime.MinValue;

        #endregion

        #region Properties

        #endregion

        #region Constructors

        public NominatimGeocoderService(ILogger logger, INotificationManagerService notificationManagerService)
        {
            if (logger != null)
            {
                _logger = logger;
            }
            else
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (notificationManagerService != null)
            {
                _notificationManagerService = notificationManagerService;
            }
            else
            {
                throw new ArgumentNullException(nameof(notificationManagerService));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if the address is already cached.
        /// </summary>
        /// <param name="street">The street address to check if geocode data is cached</param>
        /// <param name="municipality">The municipality of the address to check if geocode data is cached</param>
        /// <param name="province">The province of the address to check if geocode data is cached</param>
        /// <param name="country">The country of the address to check if geocode data is cached</param>
        /// <param name="postalCode">The postal code of the address to check if geocode data is cached</param>
        /// <returns>True if cached file exists for this address, false otherwise</returns>
        public bool IsCached(string street, string municipality, Province province, string postalCode, string country = "Canada")
        {
            var path = this.GetCachePath(street, municipality, province, country, postalCode);
            return File.Exists(path);
        }

        /// <summary>
        /// Returns the latitude and longitude for the given address.
        /// </summary>
        /// <param name="street">The street address to geocode and get coordinates for</param>
        /// <param name="municipality">The municipality of the address to geocode and get coordinates for</param>
        /// <param name="province">The province of the address to geocode and get coordinates for</param>
        /// <param name="country">The country of the address to geocode and get coordinates for</param>
        /// <param name="postalCode">The postal code of the address to geocode and get coordinates for</param>
        /// <returns>Longitude and latitude coordinates</returns>
        public async Task<(double latitude, double longitude)> GetCoordinates(string street, string municipality, Province province, string postalCode, string country = "Canada")
        {
            street = PrepareStreetStringForAPI(street);
            // If no cached data, get data from Nominatim API and cache it.
            string content = this.GetCachedData(street, municipality, province, country, postalCode);
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
        /// Returns a JObject of all the data returned from the Nominatim API for the given address
        /// </summary>
        /// <param name="street">The street address to geocode and get coordinates for</param>
        /// <param name="municipality">The municipality of the address to geocode and get coordinates for</param>
        /// <param name="province">The province of the address to geocode and get coordinates for</param>
        /// <param name="country">The country of the address to geocode and get coordinates for</param>
        /// <param name="postalCode">The postal code of the address to geocode and get coordinates for</param>
        /// <returns>JObject containing all the data returned from the Nominatim API for the given address</returns>
        public async Task <JObject> GetApiContent(string street, string municipality, Province province, string postalCode, string country = "Canada")
        {
            street = PrepareStreetStringForAPI(street);
            // If no cached data, get data from Nominatim API and cache it.
            string content = this.GetCachedData(street, municipality, province, country, postalCode);
            if (string.IsNullOrWhiteSpace(content))
            {
                content = await GetAndCacheNominatimData(street, municipality, province, country, postalCode);
            }
            return JArray.Parse(content).FirstOrDefault() as JObject;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Prepares the street address string for styling that works better with Nominatim 
        /// </summary>
        /// <param name="street">The street address string to reformat to a preferable format</param>
        /// <returns></returns>
        private string PrepareStreetStringForAPI(string street)
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

            string convertedAddress = regex.Replace(street, match =>
            {
                var key = match.Value.ToLower();
                return directions.ContainsKey(key) ? directions[key] : match.Value;
            });
            return convertedAddress;
        }

        /// <summary>
        /// Gets the correct Nominatim API URL for the given address.
        /// </summary>
        /// <param name="street">The street address used in the api call</param>
        /// <param name="municipality">The municipality used in the api call</param>
        /// <param name="province">The province used in the api call</param>
        /// <param name="country">The country used in the api call</param>
        /// <param name="postalCode">The postal code used in the api call</param>
        /// <returns>Returns the url needed to access the API for the given address.</returns>
        private string GetCorrectApiUrl(string street, string municipality, Province province, string country, string postalCode)
        {
            street = Uri.EscapeDataString(street);
            municipality = Uri.EscapeDataString(municipality);
            string provinceString = Uri.EscapeDataString(province.ToString());
            country = Uri.EscapeDataString(country);
            postalCode = Uri.EscapeDataString(postalCode);
            var format = "json";
            var Url = $"https://nominatim.openstreetmap.org/search?street={street}&city={municipality}&state={provinceString}&country={country}&postalcode={postalCode}&format={format}&addressdetails=1&limit=1";
            return Url;
        }

        /// <summary>
        /// Returns geocoding data for the specified address from the Nominatim API and caches the result.
        /// </summary>
        /// <param name="street">The street address used in the api call</param>
        /// <param name="municipality">The municipality used in the api call</param>
        /// <param name="province">The province used in the api call</param>
        /// <param name="country">The country used in the api call</param>
        /// <param name="postalCode">The postal code used in the api call</param>
        /// <returns>A JSON string containing the geocoding data for the specified address if the API call is successful;
        /// otherwise, returns null.</returns>
        private async Task<string> GetAndCacheNominatimData(string street, string municipality, Province province, string country, string postalCode)
        {
            // Check if request is locked out due to previous request being made too recently
            if (IsRequestLocked())
            {
                _notificationManagerService.ShowToast("Too many requests made.", $"Please wait {ApiLockoutSeconds - (DateTime.Now - _lastApiRequestTime).TotalSeconds:F0} seconds before looking up another address.");
                _logger.LogWarning($"{nameof(NominatimGeocoderService)}: API request blocked due to lockout timer.");
                return null;
            }
            _lastApiRequestTime = DateTime.Now;

            string apiUrl = GetCorrectApiUrl(street, municipality, province, country, postalCode);
            string content = null;
            try 
            {
                    // Run a task that forces the Nominatim API to timeout if the timeout property isn't able to gracefully time out the API call. If the API
                    // does not respond within this time (slow internet connection or API issues), we return null.
                    var getNominatimApi = Task.Run(() => DownloadNominatimApiData(apiUrl));
                    if (getNominatimApi.Wait(TimeSpan.FromSeconds(ApiTimeout)))
                    {
                        _logger.LogInformation($"{nameof(NominatimGeocoderService)}.{nameof(GetCoordinates)}, Nominatim API Task Status: {getNominatimApi.Status}");
                        content = getNominatimApi.Result;
                        // Check if content returned by API is empty, if so do not return
                        if (content == "[]")
                        {
                            _logger.LogError($"{nameof(NominatimGeocoderService)}.{nameof(DownloadNominatimApiData)}: API content empty. Address not valid.");
                            _notificationManagerService.ShowToast(H.Core.Properties.Resources.CoordinateError, H.Core.Properties.Resources.CantFindCoordinate, NotificationType.Error);
                            return null;
                        }
                        CacheData(street, municipality, province, country, postalCode, content);
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
        /// <param name="street">The street address used in the naming of the cache file</param>
        /// <param name="municipality">The municipality used in the naming of the cache file</param>
        /// <param name="province">The province used in the naming of the cache file</param>
        /// <param name="country">The country used in the naming of the cache file</param>
        /// <param name="postalCode">The postal code used in the naming of the cache file</param>
        /// <returns>The path based off of the given address.</returns>
        private string GetCachePath(string street, string municipality, Province province, string country, string postalCode)
        {
            var joinedAddress = street+"_"+municipality+"_"+province+"_"+country+"_"+postalCode;
            // Sanitize address for file name, replace common address characters with underscores.
            var invalidCharacters = Path.GetInvalidFileNameChars();
            var cleanedFileName = invalidCharacters.Aggregate(joinedAddress, (current, c) => current.Replace(c, '_')).Replace(" ", "_").Replace(",", "");
            var filename = $"nominatim_geocoder_data_address_{cleanedFileName}";

            var path = Path.GetTempPath();
            return Path.Combine(path, filename);
        }

        /// <summary>
        /// Retrieves cached data for the given address if it exists.
        /// </summary>
        /// <param name="street">The street address used in the naming of the cache file to be retrieved</param>
        /// <param name="municipality">The municipality used in the naming of the cache file to be retrieved</param>
        /// <param name="province">The province used in the naming of the cache file to be retrieved</param>
        /// <param name="country">The country used in the naming of the cache file to be retrieved</param>
        /// <param name="postalCode">The postal code used in the naming of the cache file to be retrieved</param>
        /// <returns>Returns JSON array in string format from a previous Nominatim API call.</returns>
        private string GetCachedData(string street, string municipality, Province province, string country, string postalCode)
        {
            var path = GetCachePath(street, municipality, province, country, postalCode);
            if (File.Exists(path))
            {
                _logger.LogInformation($"Loading cached Nominatim Geocoder data for address: {street} {municipality}, {province.ToString()}, {country}, {postalCode}");
                return File.ReadAllText(path);
            }
            return null;
        }

        /// <summary>
        /// Caches the Nominatim API data for the given address.
        /// </summary>
        /// <param name="street">The street address used in the naming of the cache file to be retrieved</param>
        /// <param name="municipality">The municipality used in the naming of the cache file to be retrieved</param>
        /// <param name="province">The province used in the naming of the cache file to be retrieved</param>
        /// <param name="country">The country used in the naming of the cache file to be retrieved</param>
        /// <param name="postalCode">The postal code used in the naming of the cache file to be retrieved</param>
        private void CacheData(string street, string municipality, Province province, string country, string postalCode, string content)
        {
            _logger.LogInformation($"Caching Nominatim Geocoder data for address: {street} {municipality}, {province}, {country}, {postalCode}");
            var path = GetCachePath(street, municipality, province, country, postalCode);
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
        /// Returns boolean on if the API request is locked out due to previous request being made too recently.
        /// </summary>
        /// <returns>False if an API call can be made, true if an API call can be made.</returns>
        private bool IsRequestLocked()
        {
            return (DateTime.Now - _lastApiRequestTime).TotalSeconds < ApiLockoutSeconds;
        }
        #endregion
    }
}