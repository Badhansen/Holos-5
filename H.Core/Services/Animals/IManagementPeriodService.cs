using H.Core.Enumerations;
using H.Core.Factories;
using H.Core.Models.Animals;

namespace H.Core.Services.Animals;

public interface IManagementPeriodService
{
    IReadOnlyCollection<BeddingMaterialType> GetValidBeddingMaterialTypes(AnimalType animalType);
    IManagementPeriodDto TransferToManagementPeriodDto(ManagementPeriod managementPeriod);
    IManagementPeriodDto TransferManagementPeriodDtoToSystem(IManagementPeriodDto managementPeriodDto, ManagementPeriod managementPeriod);
}