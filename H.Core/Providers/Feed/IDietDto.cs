using H.Core.Enumerations;

namespace H.Core.Providers.Feed;

public interface IDietDto
{
    bool IsDefaultDiet { get; set; }
    string Name { get; set; }
    DietType DietType { get; set; }
    AnimalType AnimalType { get; set; }
    double MethaneConversionFactor { get; set; }
    double DietaryNetEnergyConcentration { get; set; }
    IReadOnlyCollection<IFeedIngredient> Ingredients { get; set; }
}