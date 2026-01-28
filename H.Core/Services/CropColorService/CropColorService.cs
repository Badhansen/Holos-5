using H.Core.Enumerations;

namespace H.Core.Services.CropColorService;

/// <summary>
/// Default implementation of <see cref="ICropColorService"/>.
/// Provides consistent color coding and display names for crop types across the application.
/// </summary>
public class CropColorService : ICropColorService
{
    #region Public Methods

    /// <summary>
    /// Gets the hexadecimal color code for a given crop type based on its category.
    /// </summary>
    /// <param name="cropType">The crop type to get the color for</param>
    /// <returns>Hexadecimal color string (e.g., "#FFF3E0")</returns>
    public string GetCropColorHex(CropType cropType)
    {
        // Cereals - Orange
        if (IsCereal(cropType))
        {
            return "#FFF3E0";
        }

        // Oilseeds - Green
        if (IsOilseed(cropType))
        {
            return "#E8F5E9";
        }

        // Pulses - Blue
        if (IsPulse(cropType))
        {
            return "#E3F2FD";
        }

        // Forages - Purple
        if (IsForage(cropType))
        {
            return "#F3E5F5";
        }

        // Fallow - Gray
        if (IsFallow(cropType))
        {
            return "#FAFAFA";
        }

        // Default - Light gray
        return "#F5F5F5";
    }

    /// <summary>
    /// Gets the display name for a crop type.
    /// The colored background of cells provides visual distinction between crop categories,
    /// so icons/emojis are not necessary.
    /// </summary>
    /// <param name="cropType">The crop type to get the display name for</param>
    /// <returns>Display name (e.g., "Wheat", "Canola")</returns>
    public string GetCropDisplayName(CropType cropType)
    {
        return cropType switch
        {
            // Cereals
            CropType.Wheat => "Wheat",
            CropType.Barley => "Barley",
            CropType.Oats => "Oats",
            CropType.Rye => "Rye",
            CropType.Triticale => "Triticale",
            CropType.Durum => "Durum",
            CropType.SpringWheat => "Spring Wheat",
            CropType.WinterWheat => "Winter Wheat",
            CropType.Corn => "Corn",
            CropType.GrainCorn => "Grain Corn",
            CropType.SilageCorn => "Silage Corn",
            
            // Oilseeds
            CropType.Canola => "Canola",
            CropType.Flax => "Flax",
            CropType.FlaxSeed => "Flax Seed",
            CropType.Sunflower => "Sunflower",
            CropType.SunflowerSeed => "Sunflower Seed",
            CropType.Soybeans => "Soybeans",
            CropType.Mustard => "Mustard",
            CropType.MustardSeed => "Mustard Seed",
            
            // Pulses
            CropType.Peas => "Peas",
            CropType.DryPeas => "Dry Peas",
            CropType.FieldPeas => "Field Peas",
            CropType.Lentils => "Lentils",
            CropType.Beans => "Beans",
            CropType.DryBean => "Dry Bean",
            CropType.Chickpeas => "Chickpeas",
            CropType.FabaBeans => "Faba Beans",
            CropType.ColouredWhiteFabaBeans => "Coloured White Faba Beans",
            
            // Forages
            CropType.AlfalfaMedicagoSativaL => "Alfalfa",
            CropType.AlfalfaHay => "Alfalfa Hay",
            CropType.TameLegume => "Tame Legume",
            CropType.TameGrass => "Tame Grass",
            CropType.TameMixed => "Tame Mixed",
            CropType.Forage => "Forage",
            CropType.GrassHay => "Grass Hay",
            CropType.PerennialForages => "Perennial Forages",
            
            // Fallow
            CropType.Fallow => "Fallow",
            CropType.SummerFallow => "Summer Fallow",
            
            // Default fallback
            _ => cropType.ToString()
        };
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Determines if a crop type is a cereal.
    /// </summary>
    private bool IsCereal(CropType cropType)
    {
        return cropType == CropType.Wheat ||
               cropType == CropType.Barley ||
               cropType == CropType.Oats ||
               cropType == CropType.Rye ||
               cropType == CropType.Triticale ||
               cropType == CropType.GrainCorn ||
               cropType == CropType.SpringWheat ||
               cropType == CropType.WinterWheat ||
               cropType == CropType.Durum ||
               cropType == CropType.Corn ||
               cropType == CropType.FodderCorn ||
               cropType == CropType.SilageCorn ||
               cropType == CropType.CornSilage ||
               cropType == CropType.MaltBarley ||
               cropType == CropType.BarleySilage ||
               cropType == CropType.OatSilage ||
               cropType == CropType.FallRye ||
               cropType == CropType.SpringRye ||
               cropType == CropType.SmallGrainCereals ||
               cropType == CropType.MixedGrains ||
               cropType == CropType.Millet;
    }

    /// <summary>
    /// Determines if a crop type is an oilseed.
    /// </summary>
    private bool IsOilseed(CropType cropType)
    {
        return cropType == CropType.Canola ||
               cropType == CropType.Flax ||
               cropType == CropType.FlaxSeed ||
               cropType == CropType.Sunflower ||
               cropType == CropType.SunflowerSeed ||
               cropType == CropType.Soybeans ||
               cropType == CropType.Mustard ||
               cropType == CropType.MustardSeed ||
               cropType == CropType.Safflower ||
               cropType == CropType.CanarySeed ||
               cropType == CropType.Oilseeds ||
               cropType == CropType.Linola ||
               cropType == CropType.Hyola;
    }

    /// <summary>
    /// Determines if a crop type is a pulse.
    /// </summary>
    private bool IsPulse(CropType cropType)
    {
        return cropType == CropType.Peas ||
               cropType == CropType.DryPeas ||
               cropType == CropType.FieldPeas ||
               cropType == CropType.Lentils ||
               cropType == CropType.Beans ||
               cropType == CropType.DryBean ||
               cropType == CropType.BeansDryField ||
               cropType == CropType.Chickpeas ||
               cropType == CropType.FabaBeans ||
               cropType == CropType.ColouredWhiteFabaBeans ||
               cropType == CropType.PulseCrops;
    }

    /// <summary>
    /// Determines if a crop type is a forage.
    /// </summary>
    private bool IsForage(CropType cropType)
    {
        return cropType == CropType.AlfalfaMedicagoSativaL ||
               cropType == CropType.AlfalfaHay ||
               cropType == CropType.AlfalfaSeed ||
               cropType == CropType.TameLegume ||
               cropType == CropType.TameGrass ||
               cropType == CropType.TameMixed ||
               cropType == CropType.Forage ||
               cropType == CropType.ForageForSeed ||
               cropType == CropType.GrassHay ||
               cropType == CropType.PerennialForages ||
               cropType == CropType.SeededGrassland ||
               cropType == CropType.RangelandNative ||
               cropType == CropType.TamePasture ||
               cropType == CropType.NativePasture ||
               cropType == CropType.BromeHay ||
               cropType == CropType.TimothyHay ||
               cropType == CropType.GrassSeed ||
               cropType == CropType.HayAndForageSeed;
    }

    /// <summary>
    /// Determines if a crop type is fallow.
    /// </summary>
    private bool IsFallow(CropType cropType)
    {
        return cropType == CropType.Fallow ||
               cropType == CropType.SummerFallow;
    }

    #endregion
}
