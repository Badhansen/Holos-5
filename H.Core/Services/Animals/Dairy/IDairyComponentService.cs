using H.Core.Factories.Animals.Dairy;
using H.Core.Models;
using H.Core.Models.Animals.Dairy;

namespace H.Core.Services.Animals.Dairy;

/// <summary>
/// Service interface for managing dairy component operations and data transfer.
/// Provides dairy-specific methods for component initialization and DTO conversion.
/// </summary>
public interface IDairyComponentService
{
    /// <summary>
    /// Initializes a new dairy component with default values appropriate for a dairy operation.
    /// </summary>
    /// <param name="farm">The farm to which this dairy component belongs</param>
    /// <param name="dairyComponent">The dairy component to initialize</param>
    void InitializeComponent(Farm farm, DairyComponent dairyComponent);
    
    /// <summary>
    /// Creates a dairy component DTO from a domain model for UI binding.
    /// </summary>
    /// <param name="dairyComponent">The source dairy component domain model</param>
    /// <returns>A DTO suitable for binding in the view</returns>
    IDairyComponentDto TransferToDairyComponentDto(DairyComponent dairyComponent);
    
    /// <summary>
    /// Transfers data from DTO back to the domain model after validation.
    /// </summary>
    /// <param name="dairyDto">The source DTO containing user input</param>
    /// <param name="dairyComponent">The target domain model to update</param>
    /// <returns>The updated dairy component</returns>
    DairyComponent TransferDairyDtoToSystem(DairyComponentDto dairyDto, DairyComponent dairyComponent);
    
    /// <summary>
    /// Auto-generates the four lifecycle-based animal groups for a dairy herd based on calculated herd composition.
    /// Creates: Calf group, Heifer group, Lactating cow group, and Dry cow group.
    /// 
    /// This method bridges the gap between simplified herd overview inputs and detailed
    /// animal group structure required for emissions calculations.
    /// 
    /// IMPORTANT - DATA PRESERVATION:
    /// This method ONLY generates groups if:
    /// 1. forceRegeneration = true (user explicitly clicked "Regenerate Groups" button), OR
    /// 2. The component has NO existing groups (first-time setup)
    /// 
    /// This prevents accidental deletion of user-configured animal groups when loading saved components.
    /// </summary>
    /// <param name="dairyDto">The DTO containing herd overview parameters and calculated animal counts</param>
    /// <param name="dairyComponent">The dairy component to populate with animal groups</param>
    /// <param name="forceRegeneration">If true, clears existing groups and regenerates. If false, only generates if component has no groups.</param>
    void GenerateAnimalGroups(DairyComponentDto dairyDto, DairyComponent dairyComponent, bool forceRegeneration = false);
}
