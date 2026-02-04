using H.Core.Factories.Animals;

namespace H.Core.Factories.Animals.Dairy;

/// <summary>
/// Interface for dairy component data transfer object
/// </summary>
public interface IDairyComponentDto : IAnimalComponentDto
{
    // Herd Overview - Input Parameters
    int TotalMilkingCows { get; set; }
    double ReplacementRate { get; set; }
    int CalvingIntervalMonths { get; set; }
    int DryPeriodDays { get; set; }
    double CalfMortalityRate { get; set; }
    double FemaleCalfRatio { get; set; }
    
    // Calculated Herd Composition - Read-only outputs
    int CalculatedCalves { get; }
    int CalculatedHeifers { get; }
    int CalculatedLactating { get; }
    int CalculatedDry { get; }
}
