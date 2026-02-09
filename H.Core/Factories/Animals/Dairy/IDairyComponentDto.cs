using H.Core.Factories.Animals;
using H.Core.Enumerations;

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
    
    // Herd Production Defaults - Used to populate management periods
    // NOTE: See DairyComponentDto for detailed explanation of two-level architecture
    double DefaultMilkProduction { get; set; }
    double DefaultMilkFatContent { get; set; }
    double DefaultMilkProteinContent { get; set; }
    
    // Staggered Progression - Flow Rate Inputs
    // These represent the number of animals entering each lifecycle stage per year
    int CalvesEnteringPerYear { get; set; }
    int HeifersEnteringPerYear { get; set; }
    int LactatingCowsEnteringPerYear { get; set; }
    int DryCowsEnteringPerYear { get; set; }
    
    // Staggered Progression - Calculated Steady-State Populations
    // These are read-only calculated properties showing the steady-state population in each stage
    int SteadyStateCalves { get; }
    int SteadyStateHeifers { get; }
    int SteadyStateLactating { get; }
    int SteadyStateDry { get; }
    int TotalSteadyStateHerdSize { get; }
    
    // Manure Handling Systems - Phase-specific configurations
    ManureStateType HeiferPhase1ManureHandlingSystem { get; set; }
    ManureStateType HeiferPhase2ManureHandlingSystem { get; set; }
}
