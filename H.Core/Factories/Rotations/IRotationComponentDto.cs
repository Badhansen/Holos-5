using CsvHelper.TypeConversion;

namespace H.Core.Factories.Rotations;

public interface IRotationComponentDto : IDto
{
    #region Properties

    /// <summary>
    /// All fields that belong to this rotation must have the same area. This property defines that area.
    /// </summary>
    double FieldArea { get; set; }

    #endregion
}