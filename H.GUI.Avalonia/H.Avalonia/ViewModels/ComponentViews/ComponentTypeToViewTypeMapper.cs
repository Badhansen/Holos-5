using H.Avalonia.Views.ComponentViews.LandManagement.Field;
using H.Core.Models;

namespace H.Avalonia.ViewModels.ComponentViews;

public class ComponentTypeToViewTypeMapper
{
    public static string GetViewName(ComponentBase component)
    {
        /*
         * Land Management
         */

        if (component.ComponentType == ComponentType.Field)
        {
            return nameof(FieldComponentView);
        }

        if (component.ComponentType == ComponentType.Rotation)
        {
            return nameof(Views.ComponentViews.LandManagement.RotationComponentView);
        }
        if (component.ComponentType == ComponentType.Shelterbelt)
        {
            return nameof(Views.ComponentViews.LandManagement.ShelterbeltComponentView);
        }
        
        /*
         * Beef Production
         */

        if (component.ComponentType == ComponentType.CowCalf)
        {
            return nameof(Views.ComponentViews.Beef.CowCalfComponentView);
        }
        if (component.ComponentType == ComponentType.Backgrounding)
        {
            return nameof(Views.ComponentViews.Beef.BackgroundingComponentView);
        }
        if (component.ComponentType == ComponentType.Finishing)
        {
            return nameof(Views.ComponentViews.Beef.FinishingComponentView);
        }

        /*
         * Dairy
         */

         if (component.ComponentType == ComponentType.Dairy) {
            return nameof(Views.ComponentViews.Dairy.DairyComponentView);
         }

         /*
          * Swine
          */

        if (component.ComponentType == ComponentType.SwineGrowers)
        {
            return nameof(Views.ComponentViews.Swine.GrowerToFinishComponentView);
        }

        if (component.ComponentType == ComponentType.FarrowToWean)
        {
            return nameof(Views.ComponentViews.Swine.FarrowToWeanComponentView);
        }

        if (component.ComponentType == ComponentType.IsoWean)
        {
            return nameof(Views.ComponentViews.Swine.IsoWeanComponentView);
        }

        if (component.ComponentType == ComponentType.FarrowToFinish)
        {
            return nameof(Views.ComponentViews.Swine.FarrowToFinishComponentView);
        }

        /*
         * Sheep
         */

        if (component.ComponentType == ComponentType.Sheep)
        {
            return nameof(Views.ComponentViews.LandManagement.SheepComponentView);
        }

        if (component.ComponentType == ComponentType.SheepFeedlot)
        {
            return nameof(Views.ComponentViews.Sheep.SheepFeedlotComponentView);
        }

        if(component.ComponentType == ComponentType.Rams)
        {
            return nameof(Views.ComponentViews.Sheep.RamsComponentView);
        }
        
        if(component.ComponentType == ComponentType.LambsAndEwes)
        {
            return nameof(Views.ComponentViews.Sheep.LambsAndEwesComponentView);
        }

        /*
         * Other Animals
         */

        if(component.ComponentType == ComponentType.Goats)
        {
            return nameof(Views.ComponentViews.OtherAnimals.GoatsComponentView);
        }

        if (component.ComponentType == ComponentType.Deer)
        {
            return nameof(Views.ComponentViews.OtherAnimals.DeerComponentView);
        }
        
        if (component.ComponentType == ComponentType.Horses)
        {
            return nameof(Views.ComponentViews.OtherAnimals.HorsesComponentView);
        }
        
        if (component.ComponentType == ComponentType.Mules)
        {
            return nameof(Views.ComponentViews.OtherAnimals.MulesComponentView);
        }
        
        if (component.ComponentType == ComponentType.Bison)
        {
            return nameof(Views.ComponentViews.OtherAnimals.BisonComponentView);
        }

        if (component.ComponentType == ComponentType.Llamas)
        {
            return nameof(Views.ComponentViews.OtherAnimals.LlamaComponentView);
        }

        /*
         * Poultry
         */

        if (component.ComponentType == ComponentType.ChickenPulletFarm)
        {
            return nameof(Views.ComponentViews.Poultry.ChickenPulletsComponentView);
        }

        if (component.ComponentType == ComponentType.ChickenMultiplierBreeder)
        {
            return nameof(Views.ComponentViews.Poultry.ChickenMultiplierBreederComponentView);
        }

        if (component.ComponentType == ComponentType.ChickenMeatProduction)
        {
            return nameof(Views.ComponentViews.Poultry.ChickenMeatProductionComponentView);
        }

        if (component.ComponentType == ComponentType.TurkeyMultiplierBreeder)
        {
            return nameof(Views.ComponentViews.Poultry.TurkeyMultiplierBreederComponentView);
        }

        if (component.ComponentType == ComponentType.TurkeyMeatProduction)
        {
            return nameof(Views.ComponentViews.Poultry.TurkeyMeatProductionComponentView);
        }

        if (component.ComponentType == ComponentType.ChickenEggProduction)
        {
            return nameof(Views.ComponentViews.Poultry.ChickenEggProductionComponentView);
        }

        if (component.ComponentType == ComponentType.ChickenMultiplierHatchery)
        {
            return nameof(Views.ComponentViews.Poultry.ChickenMultiplierHatcheryComponentView);
        }

        /*
         * Infrastructure
         */

        if (component.ComponentType == ComponentType.AnaerobicDigestion)
        {
            return nameof(Views.ComponentViews.Infrastructure.AnaerobicDigestionComponentView);
        }

        return string.Empty;
    }
}