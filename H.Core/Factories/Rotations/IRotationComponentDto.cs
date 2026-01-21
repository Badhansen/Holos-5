using CsvHelper.TypeConversion;

namespace H.Core.Factories.Rotations;

public interface IRotationComponentDto : IDto
{
    #region Properties

    /// <summary>
    /// All fields that belong to this rotation must have the same area. This property defines that area.
    /// </summary>
    double FieldArea { get; set; }

    /// <summary>
    /// The start year for the rotation component that defines when the first crop is planted
    /// </summary>
    int StartYear { get; set; }

    /// <summary>
    /// The end year for the rotation component that defines when the last crop is planted
    /// </summary>
    int EndYear { get; set; }

    #endregion
}