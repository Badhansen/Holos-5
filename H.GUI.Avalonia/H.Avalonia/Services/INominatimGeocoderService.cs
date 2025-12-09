using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Avalonia.Services
{
    public interface INominatimGeocoderService
    {
        /// <summary>
        /// Checks if the address is already cached.
        /// </summary>
        /// <param name="address">The address to check if a cache exists</param>
        /// <returns></returns>
        bool IsCached(string address);
        /// <summary>
        /// Returns the latitude and longitude for the given address.
        /// </summary>
        /// <param name="address">The address to geocode and get coordinates for</param>
        /// <returns></returns>
        Task<(double latitude, double longitude)> GetCoordinates(string address);
        /// <summary>
        /// Returns the province for the given address.
        /// </summary>
        /// <param name="address">The address to geocode and get the province for</param>
        /// <returns></returns>
        Task<Province?> GetProvince(string address);
    }
}
