using System.Collections.ObjectModel;

namespace H.Core.Factories.Animals;

public interface IAnimalComponentDto : IDto
{
    /// <summary>
    /// Collection of animal group DTOs that represent the groups in this component.
    /// These DTOs are bound to the view and include validation logic.
    /// </summary>
    ObservableCollection<AnimalGroupDto> AnimalGroupDtos { get; set; }
}