using H.Infrastructure;

namespace H.Core.Models.Animals.Dairy;

/// <summary>
/// Represents a named population group within a dairy lifecycle stage.
/// This allows users to organize animals into pens, barns, cohorts, or other groupings
/// for more detailed herd composition tracking.
/// </summary>
public class DairyPopulationGroup : ModelBase
{
    #region Fields

    private string _groupName = string.Empty;
    private int _numberOfAnimals;

    #endregion

    #region Properties

    /// <summary>
    /// User-defined name for this population group (e.g., "Pen 1", "Barn A", "Breeding Group")
    /// </summary>
    public string GroupName
    {
        get => _groupName;
        set => SetProperty(ref _groupName, value);
    }

    /// <summary>
    /// Number of animals in this population group
    /// </summary>
    public int NumberOfAnimals
    {
        get => _numberOfAnimals;
        set => SetProperty(ref _numberOfAnimals, value);
    }

    #endregion

    #region Constructors

    public DairyPopulationGroup()
    {
    }

    public DairyPopulationGroup(string groupName, int numberOfAnimals)
    {
        GroupName = groupName;
        NumberOfAnimals = numberOfAnimals;
    }

    #endregion
}
