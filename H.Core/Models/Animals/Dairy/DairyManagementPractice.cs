using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.Animals.Dairy;

/// <summary>
/// Represents a single management practice (phase) within a dairy lifecycle stage.
/// Each practice defines a named period with specific manure handling and housing configurations.
///
/// Users can dynamically add and remove management practices for each lifecycle stage
/// (Calf, Heifer, Lactating, Dry) to model their specific dairy operation.
/// </summary>
public class DairyManagementPractice : ModelBase
{
    #region Fields

    private string _practiceName = string.Empty;
    private ManureStateType _manureHandlingSystem = ManureStateType.LiquidNoCrust;
    private HousingType _housingType = HousingType.FreeStallBarnSlurryScraping;

    #endregion

    #region Constructors

    public DairyManagementPractice()
    {
    }

    public DairyManagementPractice(string practiceName)
    {
        PracticeName = practiceName;
    }

    public DairyManagementPractice(string practiceName, ManureStateType manureHandlingSystem, HousingType housingType)
    {
        PracticeName = practiceName;
        ManureHandlingSystem = manureHandlingSystem;
        HousingType = housingType;
    }

    #endregion

    #region Properties

    /// <summary>
    /// User-defined name for this management practice (e.g., "Early Lactation", "Milk-Fed Period", "Far-off Dry")
    /// </summary>
    public string PracticeName
    {
        get => _practiceName;
        set => SetProperty(ref _practiceName, value);
    }

    /// <summary>
    /// The manure handling system used during this management practice.
    /// Determines how manure is stored, processed, and handled.
    /// </summary>
    public ManureStateType ManureHandlingSystem
    {
        get => _manureHandlingSystem;
        set => SetProperty(ref _manureHandlingSystem, value);
    }

    /// <summary>
    /// The type of housing facility used during this management practice.
    /// Determines the barn/facility type and its associated emissions factors.
    /// </summary>
    public HousingType HousingType
    {
        get => _housingType;
        set => SetProperty(ref _housingType, value);
    }

    #endregion
}
