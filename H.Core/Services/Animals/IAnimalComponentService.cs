using CsvHelper;
using H.Core.Factories;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services.Animals;

public interface IAnimalComponentService : IComponentService
{
    public IAnimalComponentDto TransferToAnimalComponentDto(AnimalComponentBase animalComponent);
    public AnimalComponentBase TransferAnimalComponentDtoToSystem(AnimalComponentDto animalComponentDto,
        AnimalComponentBase animalComponent);
}