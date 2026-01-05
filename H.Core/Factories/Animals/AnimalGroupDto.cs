using H.Core.Enumerations;
using H.Core.Models.Animals;
using System.Collections.ObjectModel;

namespace H.Core.Factories.Animals;

public class AnimalGroupDto : DtoBase, IAnimalGroupDto
{
    #region Fields

    private AnimalType? _groupType;

    #endregion

    #region Properties

    public ObservableCollection<AnimalType> ValidAnimalTypes { get; set; } = new ObservableCollection<AnimalType>();

    public AnimalType? GroupType
    {
        get => _groupType;
        set => SetProperty(ref _groupType, value);
    } 

	#endregion
}