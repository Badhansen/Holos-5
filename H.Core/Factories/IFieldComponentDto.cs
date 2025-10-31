using System.Collections.ObjectModel;
using System.ComponentModel;
using H.Core.Factories.Crops;

namespace H.Core.Factories;

public interface IFieldComponentDto : IDto
{
    ObservableCollection<ICropDto> CropDtos { get; set; }

    /// <summary>
    /// The total size of the field
    /// </summary>
    double FieldArea { get; set; }
}