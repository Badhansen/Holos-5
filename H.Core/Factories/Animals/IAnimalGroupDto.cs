using H.Core.Enumerations;
using H.Core.Models.Animals;

namespace H.Core.Factories.Animals;

public interface IAnimalGroupDto : IDto
{
    AnimalType? GroupType { get; set; }
}