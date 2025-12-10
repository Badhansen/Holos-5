using Avalonia.Controls.Shapes;
using BruTile.Wmts.Generated;
using H.Core.Enumerations;
using H.Core.Providers.Climate;
using ImTools;
using Mapsui;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Utilities;
using Newtonsoft.Json.Linq;
using SharpKml.Dom.Atom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using Path = System.IO.Path;

namespace H.Avalonia.Services
{
    public class NominatimGeocoderService : INominatimGeocoderService
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

        public bool IsCached(string address)
        {
            var path = this.GetCachePath(address);
            return File.Exists(path);
        }

        public async Task<(double latitude, double longitude)> GetCoordinates(string address)
        {
            // If no cached data, get data from Nominatim API and cache it.
            string content = this.GetCachedData(address);
            if (string.IsNullOrWhiteSpace(content))
            {
                content = await GetAndCacheNominatimData(address);
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

        public async Task<Province?> GetProvince(string address)
        {
            string content = this.GetCachedData(address);
            // If no cached data, get data from Nominatim API and cache it.
            if (string.IsNullOrWhiteSpace(content))
            {
               content = await GetAndCacheNominatimData(address);
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

        private string GetCorrectApiUrl(string address)
        {
            address = Uri.EscapeDataString(address);
            var format = "json";
            var Url = $"https://nominatim.openstreetmap.org/search?q={address}&format={format}&addressdetails=1&limit=1";
            return Url;
        }

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
                            _logger.LogWarning($"{nameof(NominatimGeocoderService)}.{nameof(DownloadNominatimApiData)}: API content empty.");
                            return String.Empty;
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
        private string GetCachePath(string address)
        {
            // Sanitize address for file name, replace common address characters with underscores.
            var invalidCharacters = Path.GetInvalidFileNameChars();
            var cleanedFileName = invalidCharacters.Aggregate(address, (current, c) => current.Replace(c, '_')).Replace(" ", "_").Replace(",", "");
            var filename = $"nominatim_geocoder_data_address_{cleanedFileName}";

            var path = Path.GetTempPath();
            return Path.Combine(path, filename);
        }

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

        private void CacheData(string address, string content)
        {
            _logger.LogInformation($"Caching Nominatim Geocoder data for address: {address}");
            var path = GetCachePath(address);
            File.WriteAllText(path, content);
        }

        private (double latitude, double longitude) ParseNominatimApiContentForCoordinates(string content)
        {
            // Initially read as JArray since Nominatim returns an array of one JSON object.
            JObject jObject = JArray.Parse(content).FirstOrDefault() as JObject;
            // Access properties from the JObject
            var lat = double.Parse(jObject["lat"]?.ToString());
            var lon = double.Parse(jObject["lon"]?.ToString());
            return (latitude: lat, longitude: lon);
        }

        private Province ParseNominatimApiContentForProvince(string content)
        {
            // Initially read as JArray since Nominatim returns an array of one JSON object.
            JObject jObject = JArray.Parse(content).FirstOrDefault() as JObject;
            // Access province from the JObject
            var provinceString = jObject["address"]?["state"]?.ToString().Replace(" ", ""); // Remove spaces for enum parsing.
            // Convert province string to Province enum
            Province provinceEnum = (Province)Enum.Parse(typeof(Province), provinceString);
            return provinceEnum;
        }

        #endregion
    }
}
