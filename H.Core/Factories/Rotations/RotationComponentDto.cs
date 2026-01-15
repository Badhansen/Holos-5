using H.Core.Models.LandManagement.Fields;
using H.Core.Models.LandManagement.Rotation;

namespace H.Core.Factories.Rotations;

/// <summary>
/// A class used to validate input as it relates to a <see cref="RotationComponent"/>. This class is used to valid input before any input
/// is transferred to the <see cref="RotationComponent"/>
/// </summary>
public class RotationComponentDto : DtoBase, IRotationComponentDto
{
    #region Fields

    private double _fieldArea;

    #endregion

    #region MyRegion

    /// <summary>
    /// All fields that belong to this rotation must have the same area. This property defines that area.
    /// </summary>
    public double FieldArea
    {
        get => _fieldArea;
        set => SetProperty(ref _fieldArea, value);
    } 

    #endregion
}