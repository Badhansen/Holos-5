using H.Core.Enumerations;
using H.Core.Providers.Feed;

namespace H.Core.Services.DietService;

public interface IDietService
{
    IReadOnlyList<AnimalType> GetValidAnimalDietTypes(AnimalType animalType);
    IReadOnlyList<IDietDto> GetDiets();
    IDietDto GetNoDiet();
    IDietDto GetDiet(AnimalType animalType, DietType dietType);
}