using H.Core.Enumerations;
using H.Core.Models.Animals;

namespace H.Core.Factories.Animals;

public class AnimalGroupDto : DtoBase, IAnimalGroupDto
{
    public AnimalType? GroupType { get; set; }
}