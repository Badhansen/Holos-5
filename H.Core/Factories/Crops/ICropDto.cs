using System.Collections.ObjectModel;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Factories.Crops;

/// <summary>
/// A data transfer object used to validate and collection information about a <see cref="CropViewItem"/>
/// </summary>
public interface ICropDto : IDto
{
    /// <summary>
    /// The crop type being grown
    /// </summary>
    CropType CropType { get; set; }

    /// <summary>
    /// The list of valid crop types available for selection
    /// </summary>
    ObservableCollection<CropType> ValidCropTypes { get; set; }

    /// <summary>
    /// The year in which the crop was grown
    /// </summary>
    int Year { get; set; }

    /// <summary>
    /// The total amount of annual irrigation
    ///
    /// (mm)
    /// </summary>
    double AmountOfIrrigation { get; set; }

    /// <summary>
    /// Total wet weight yield of the crop
    ///
    /// (kg ha^-1)
    /// </summary>
    double WetYield { get; set; }
}