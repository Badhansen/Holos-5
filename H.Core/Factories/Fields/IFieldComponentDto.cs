using System.Collections.ObjectModel;
using H.Core.Factories.Crops;

namespace H.Core.Factories.Fields;

public interface IFieldComponentDto : IDto
{
    #region Properties

    ObservableCollection<ICropDto>? CropDtos { get; set; }

    /// <summary>
    /// The total size of the field
    /// </summary>
    double FieldArea { get; set; }

    /// <summary>
    /// The start year for the field component that defines when the first crop in the <see cref="CropDtos"/> collection is planted
    /// </summary>
    int StartYear { get; set; }

    /// <summary>
    /// The end year for the field component that defines when the last crop in the <see cref="CropDtos"/> collection is planted
    /// </summary>
    int EndYear { get; set; }

    #endregion
}