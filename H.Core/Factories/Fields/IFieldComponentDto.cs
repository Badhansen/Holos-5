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

    #endregion
}