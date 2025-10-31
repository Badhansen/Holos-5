using H.Core.CustomAttributes;
using H.Core.Enumerations;

namespace H.Core.Factories.Crops;

public partial class CropDto
{
    #region Fields

    private double _biomassCoefficientProduct;
    private double _biomassCoefficientStraw;
    private double _biomassCoefficientRoots;
    private double _biomassCoefficientExtraroot;

    #endregion

    #region Properties

    #endregion

    #region Properties

    /// <summary>
    /// R_p
    ///
    /// (unitless)
    /// </summary>
    [Units(MetricUnitsOfMeasurement.None)]
    public double BiomassCoefficientProduct
    {
        get => _biomassCoefficientProduct;
        set => SetProperty(ref _biomassCoefficientProduct, value);
    }

    /// <summary>
    /// R_s
    ///
    /// (unitless)
    /// </summary>
    [Units(MetricUnitsOfMeasurement.None)]
    public double BiomassCoefficientStraw
    {
        get => _biomassCoefficientStraw;
        set => SetProperty(ref _biomassCoefficientStraw, value);
    }

    /// <summary>
    /// R_r
    ///
    /// (unitless)
    /// </summary>
    [Units(MetricUnitsOfMeasurement.None)]
    public double BiomassCoefficientRoots
    {
        get => _biomassCoefficientRoots;
        set => SetProperty(ref _biomassCoefficientRoots, value);
    }

    /// <summary>
    /// R_e
    ///
    /// (unitless)
    /// </summary>
    [Units(MetricUnitsOfMeasurement.None)]
    public double BiomassCoefficientExtraroot
    {
        get => _biomassCoefficientExtraroot;
        set => SetProperty(ref _biomassCoefficientExtraroot, value);
    } 

    #endregion
}