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
}
