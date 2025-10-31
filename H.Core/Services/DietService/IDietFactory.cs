using H.Core.Enumerations;
using H.Core.Providers.Feed;

namespace H.Core.Services.DietService
{
    public interface IDietFactory
    {
        IDietDto Create();
        IDietDto Create(DietType dietType, AnimalType animalType);
        IReadOnlyList<Tuple<AnimalType, DietType>> GetValidDietKeys();
        bool IsValidDietType(AnimalType animalType, DietType dietType);
    }
}