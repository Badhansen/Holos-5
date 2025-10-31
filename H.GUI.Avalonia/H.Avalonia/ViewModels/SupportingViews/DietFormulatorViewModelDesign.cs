using System.Collections.ObjectModel;
using H.Core.Providers.Feed;
using H.Core.Enumerations;
using H.Core.Services.DietService;
using H.Core.Services.StorageService;
using Prism.Regions;

namespace H.Avalonia.ViewModels.SupportingViews
{
    /// <summary>
    /// Design-time view model for the Diet Formulator view
    /// </summary>
    public class DietFormulatorViewModelDesign : DietFormulatorViewModel
    {
        public DietFormulatorViewModelDesign()
        {
            // Create sample data for design-time
            Diets = new ObservableCollection<IDietDto>
            {
                new DietDto
                {
                    Name = "High Energy Beef Diet",
                    AnimalType = AnimalType.Beef,
                    DietType = DietType.HighEnergy,
                    CrudeProtein = 12.5,
                    TotalDigestibleNutrient = 85.0,
                    Forage = 15.0,
                    DailyDryMatterFeedIntakeOfFeed = 12.0,
                    MethaneConversionFactor = 0.065,
                    Comments = "High energy diet for beef cattle"
                },
                new DietDto
                {
                    Name = "Medium Energy Dairy Diet",
                    AnimalType = AnimalType.Dairy,
                    DietType = DietType.MediumEnergy,
                    CrudeProtein = 16.2,
                    TotalDigestibleNutrient = 75.0,
                    Forage = 45.0,
                    DailyDryMatterFeedIntakeOfFeed = 22.0,
                    MethaneConversionFactor = 0.061,
                    Comments = "Standard dairy cow diet"
                },
                new DietDto
                {
                    Name = "Swine Gestation Diet",
                    AnimalType = AnimalType.Swine,
                    DietType = DietType.Gestation,
                    CrudeProtein = 14.3,
                    TotalDigestibleNutrient = 80.0,
                    Forage = 0.0,
                    DailyDryMatterFeedIntakeOfFeed = 2.49,
                    MethaneConversionFactor = 0.001,
                    Comments = "Diet for gestating sows"
                },
                new DietDto
                {
                    Name = "Sheep High Protein Diet",
                    AnimalType = AnimalType.Sheep,
                    DietType = DietType.HighEnergyAndProtein,
                    CrudeProtein = 18.5,
                    TotalDigestibleNutrient = 78.0,
                    Forage = 60.0,
                    DailyDryMatterFeedIntakeOfFeed = 1.8,
                    MethaneConversionFactor = 0.065,
                    Comments = "High protein diet for sheep"
                },
                new DietDto
                {
                    Name = "Poultry Starter Diet",
                    AnimalType = AnimalType.Poultry,
                    DietType = DietType.HighEnergyAndProtein,
                    CrudeProtein = 22.0,
                    TotalDigestibleNutrient = 82.0,
                    Forage = 0.0,
                    DailyDryMatterFeedIntakeOfFeed = 0.12,
                    MethaneConversionFactor = 0.001,
                    Comments = "Starter diet for young poultry"
                }
            };

            SearchText = "";
        }

        public DietFormulatorViewModelDesign(IRegionManager regionManager, IStorageService storageService,
            IDietService dietService) : base(regionManager, storageService, dietService)
        {
        }
    }
}