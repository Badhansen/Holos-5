using System.Collections.ObjectModel;

namespace H.Core.Factories.Animals;

public class AnimalComponentDto : DtoBase, IAnimalComponentDto
{
    #region Fields
    
    private ObservableCollection<AnimalGroupDto> _animalGroupDtos;
    
    #endregion
    
    #region Constructors
    
    public AnimalComponentDto()
    {
        _animalGroupDtos = new ObservableCollection<AnimalGroupDto>();
    }
    
    #endregion
    
    #region Properties
    
    /// <summary>
    /// Collection of animal group DTOs that represent the groups in this component.
    /// These DTOs are bound to the view and include validation logic.
    /// </summary>
    public ObservableCollection<AnimalGroupDto> AnimalGroupDtos
    {
        get => _animalGroupDtos;
        set => SetProperty(ref _animalGroupDtos, value);
    }
    
    #endregion
}