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

    /// <summary>
    /// Total number of fields in this rotation component
    /// </summary>
    int NumberOfFields { get; set; }

    /// <summary>
    /// The length of the rotation in years (EndYear - StartYear).
    /// </summary>
    int RotationLength { get; }

    /// <summary>
    /// The total area of all fields in the rotation (FieldArea * NumberOfFields).
    /// (ha)
    /// </summary>
    double TotalRotationArea { get; }

    /// <summary>
    /// The total number of crop-years in the rotation (NumberOfFields * RotationLength).
    /// This represents the total number of individual crop instances across all fields and all years.
    /// </summary>
    int TotalCropYears { get; }

    #endregion
}