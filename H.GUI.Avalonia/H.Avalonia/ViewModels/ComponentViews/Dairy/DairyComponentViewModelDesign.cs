using H.Core.Factories.Animals.Dairy;
using H.Core.Services.Animals.Dairy;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Prism.Events;
using Prism.Regions;
using System.Linq;
using System;
using H.Core.Enumerations;

namespace H.Avalonia.ViewModels.ComponentViews.Dairy;

/// <summary>
/// Design-time ViewModel for the Dairy Component view, providing sample data for the Avalonia designer.
/// This allows designers to see realistic data while building the UI without running the full application.
/// </summary>
public class DairyComponentViewModelDesign : DairyComponentViewModel
{
    /// <summary>
    /// Parameterless constructor for design-time support.
    /// Initializes the ViewModel with sample dairy herd data for visualization in the designer.
    /// </summary>
    public DairyComponentViewModelDesign()
    {
        // Initialize ManureStateTypes collection (filtering out obsolete values)
        ManureStateTypes = Enum.GetValues<ManureStateType>()
            .Where(x => !x.GetType().GetMember(x.ToString())[0]
                .GetCustomAttributes(typeof(ObsoleteAttribute), false).Any())
            .ToList();

        // Initialize HousingTypes collection (filtering out obsolete values)
        HousingTypes = Enum.GetValues<HousingType>()
            .Where(x => !x.GetType().GetMember(x.ToString())[0]
                .GetCustomAttributes(typeof(ObsoleteAttribute), false).Any())
            .ToList();

        // Create a sample DairyComponentDto with realistic values
        var dairyDto = new DairyComponentDto
        {
            Name = "Sample Dairy Herd",
            
            // Input parameters with typical dairy operation values
            TotalMilkingCows = 150,
            ReplacementRate = 28.5,
            CalvingIntervalMonths = 13,
            DryPeriodDays = 60,
            CalfMortalityRate = 4.5,
            FemaleCalfRatio = 48.0,
            
            // Production defaults - typical Holstein values
            DefaultMilkProduction = 28.0,
            DefaultMilkFatContent = 3.7,
            DefaultMilkProteinContent = 3.1,
            
            // Manure handling system defaults for heifer phases
            HeiferPhase1ManureHandlingSystem = ManureStateType.LiquidNoCrust,
            HeiferPhase2ManureHandlingSystem = ManureStateType.LiquidNoCrust,
            
            // Calf phases - typically solid storage for individual pens
            CalfPhase1ManureHandlingSystem = ManureStateType.SolidStorage,
            CalfPhase2ManureHandlingSystem = ManureStateType.SolidStorage,
            
            // Lactating phases - typically liquid slurry systems
            LactatingPhase1ManureHandlingSystem = ManureStateType.LiquidNoCrust,
            LactatingPhase2ManureHandlingSystem = ManureStateType.LiquidNoCrust,
            LactatingPhase3ManureHandlingSystem = ManureStateType.LiquidNoCrust,
            LactatingPhase4ManureHandlingSystem = ManureStateType.LiquidNoCrust,
            
            // Dry phases - typically deep bedding
            DryPhase1ManureHandlingSystem = ManureStateType.DeepBedding,
            DryPhase2ManureHandlingSystem = ManureStateType.DeepBedding,
            
            // Housing type defaults for heifer phases
            HeiferPhase1HousingType = HousingType.FreeStallBarnSlurryScraping,
            HeiferPhase2HousingType = HousingType.FreeStallBarnSlurryScraping,
            
            // Calf phases - typically housed in barn with solid litter
            CalfPhase1HousingType = HousingType.HousedInBarnSolid,
            CalfPhase2HousingType = HousingType.HousedInBarnSolid,
            
            // Lactating phases - typically free stall barn
            LactatingPhase1HousingType = HousingType.FreeStallBarnSlurryScraping,
            LactatingPhase2HousingType = HousingType.FreeStallBarnSlurryScraping,
            LactatingPhase3HousingType = HousingType.FreeStallBarnSlurryScraping,
            LactatingPhase4HousingType = HousingType.FreeStallBarnSlurryScraping,
            
            // Dry phases - typically free stall with solid litter
            DryPhase1HousingType = HousingType.FreeStallBarnSolidLitter,
            DryPhase2HousingType = HousingType.FreeStallBarnSolidLitter
        };

        // Assign the sample DTO to the bound property
        base.SelectedDairyComponentDto = dairyDto;
    }

    /// <summary>
    /// Constructor with dependency injection for runtime use.
    /// This constructor is never called in design mode but is required for the inheritance hierarchy.
    /// </summary>
    /// <param name="regionManager">Prism region manager for navigation</param>
    /// <param name="eventAggregator">Event aggregator for pub/sub messaging</param>
    /// <param name="storageService">Storage service for farm data</param>
    /// <param name="dairyComponentService">Service for dairy component operations</param>
    /// <param name="logger">Logger instance</param>
    public DairyComponentViewModelDesign(
        IRegionManager regionManager,
        IEventAggregator eventAggregator,
        IStorageService storageService,
        IDairyComponentService dairyComponentService,
        ILogger logger)
        : base(regionManager, eventAggregator, storageService, dairyComponentService, logger)
    {
    }
}
