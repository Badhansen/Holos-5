using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Avalonia.Services
{
    public interface IDefaultGeocoderService
    {
        /// <summary>
        /// Returns the province for the given address.
        /// </summary>
        /// <param name="street">The street address to geocode and get the province for</param>
        /// <returns>Province enum of where the address is located</returns>
        //
        /// <summary>
        /// Checks if the address is already cached.
        /// </summary>
        /// <param name="address">The address to check if a cache exists</param>
        /// <returns>True if cached file exists for this address, false otherwise</returns>
        bool IsCached(string address);
        /// <summary>
        /// Returns the latitude and longitude for the given address.
        /// </summary>
        /// <param name="street">The street address to geocode and get coordinates for</param>
        /// <param name="municipality">The municipality of the address to geocode and get coordinates for</param>
        /// <param name="province">The province of the address to geocode and get coordinates for</param>
        /// <param name="country">The country of the address to geocode and get coordinates for</param>
        /// <param name="postalCode">The postal code of the address to geocode and get coordinates for</param>
        /// <returns>Longitude and latitude coordinates</returns>
        Task<(double latitude, double longitude)> GetCoordinates(string street, string municipality, Province province, string country, string postalCode);

        /// <summary>
        /// Returns the province for the given address.
        /// </summary>
        /// <param name="street">The street address to geocode and get the province </param>
        /// <param name="municipality"></param>
        /// <param name="province"></param>
        /// <param name="country"></param>
        /// <param name="postalCode"></param>
        /// <returns></returns>
        Task<Province?> GetProvince(string street, string municipality, Province province, string country, string postalCode);
    }
}
