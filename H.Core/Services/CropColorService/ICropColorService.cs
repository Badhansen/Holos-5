using H.Core.Enumerations;

namespace H.Core.Services.CropColorService;

/// <summary>
/// Service for providing color information for crop types.
/// Colors are categorized by crop type (cereals, oilseeds, pulses, forages, etc.)
/// </summary>
public interface ICropColorService
{
    /// <summary>
    /// Gets the hexadecimal color code for a given crop type.
    /// Returns color based on crop category:
    /// - Cereals: Orange (#FFF3E0)
    /// - Oilseeds: Green (#E8F5E9)
    /// - Pulses: Blue (#E3F2FD)
    /// - Forages: Purple (#F3E5F5)
    /// - Fallow: Gray (#FAFAFA)
    /// - Default: Light Gray (#F5F5F5)
    /// </summary>
    /// <param name="cropType">The crop type to get the color for</param>
    /// <returns>Hexadecimal color string (e.g., "#FFF3E0")</returns>
    string GetCropColorHex(CropType cropType);

    /// <summary>
    /// Gets the display name for a crop type.
    /// Returns a clean, human-readable name without icons or emojis.
    /// The colored cell backgrounds provide visual distinction between crop categories.
    /// </summary>
    /// <param name="cropType">The crop type to get the display name for</param>
    /// <returns>Display name (e.g., "Wheat", "Canola", "Alfalfa")</returns>
    string GetCropDisplayName(CropType cropType);
}
