using System.Collections.ObjectModel;
using H.Core.Enumerations;
using H.Core.Factories;
using H.Core.Factories.Animals;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using H.Core.Services.Animals;

namespace H.Avalonia.ViewModels.ComponentViews;

public class AnimalComponentViewModelDesign : AnimalComponentViewModelBase
{
    public AnimalComponentViewModelDesign()
    {
        base.SelectedAnimalComponentDto = new AnimalComponentDto();
        base.SelectedAnimalComponentDto.Name = "Bison #2";

        ViewName = "Bison";

        var validAnimalTypes = new ObservableCollection<AnimalType>([
                AnimalType.NotSelected,
                AnimalType.Bison,
                AnimalType.Goats,
                AnimalType.Alpacas,
                AnimalType.Deer,
                AnimalType.Elk,
                AnimalType.Llamas,
                AnimalType.Horses,
                AnimalType.Mules
            ]);

        base.ManagementPeriodDtos?.Add(new ManagementPeriodDto() { Name = "Bison Management Period" });
        base.AnimalGroupDtos?.Add(new AnimalGroupDto() { ValidAnimalTypes = validAnimalTypes, GroupType =  AnimalType.NotSelected});
        base.AnimalGroupDtos?.Add(new AnimalGroupDto() {ValidAnimalTypes = validAnimalTypes, GroupType = AnimalType.Bison });
        base.AnimalGroupDtos?.Add(new AnimalGroupDto() {ValidAnimalTypes = validAnimalTypes, GroupType = AnimalType.Alpacas });
    }

    protected AnimalComponentViewModelDesign(IAnimalComponentService animalComponentService, ILogger logger, IStorageService storageService, IManagementPeriodService managementPeriodService) : base(animalComponentService, logger, storageService, managementPeriodService)
    {
    }
}