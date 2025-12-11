using AutoMapper;
using H.Core.Mappers;
using Prism.Ioc;

namespace H.Avalonia.Infrastructure.MapperServices;

/// <summary>
/// Service responsible for configuring and registering AutoMapper mappers with the dependency injection container.
/// </summary>
public class MapperRegistrationService
{
    /// <summary>
    /// Configures and registers all AutoMapper mappers with the container.
    /// </summary>
    /// <param name="containerRegistry">The Prism container registry for dependency injection.</param>
    public void RegisterMappers(IContainerRegistry containerRegistry)
    {
        // Crop mappers
        RegisterCropMappers(containerRegistry);

        // Field mappers
        RegisterFieldMappers(containerRegistry);

        // Feed ingredient mappers
        RegisterFeedIngredientMappers(containerRegistry);

        // Animal component mappers
        RegisterAnimalComponentMappers(containerRegistry);

        // Management period mappers
        RegisterManagementPeriodMappers(containerRegistry);

        // Climate mappers
        RegisterClimateMappers(containerRegistry);
    }

    private void RegisterCropMappers(IContainerRegistry containerRegistry)
    {
        var cropDtoToCropDtoConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<CropDtoToCropDtoMapper>();
        });

        var cropDtoToCropViewItemConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<CropDtoToCropViewItemMapper>();
        });

        var cropViewItemToCropDtoConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<CropViewItemToCropDtoMapper>();
        });

        containerRegistry.RegisterInstance(cropDtoToCropDtoConfiguration.CreateMapper(), nameof(CropDtoToCropDtoMapper));
        containerRegistry.RegisterInstance(cropDtoToCropViewItemConfiguration.CreateMapper(), nameof(CropDtoToCropViewItemMapper));
        containerRegistry.RegisterInstance(cropViewItemToCropDtoConfiguration.CreateMapper(), nameof(CropViewItemToCropDtoMapper));
    }

    private void RegisterFieldMappers(IContainerRegistry containerRegistry)
    {
        var fieldComponentToFieldDtoConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<FieldComponentToDtoMapper>();
        });

        var fieldDtoToFieldComponentConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<FieldDtoToFieldComponentMapper>();
        });

        var fieldDtoToFieldDtoConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<FieldDtoToFieldDtoMapper>();
        });

        containerRegistry.RegisterInstance(fieldComponentToFieldDtoConfiguration.CreateMapper(), nameof(FieldComponentToDtoMapper));
        containerRegistry.RegisterInstance(fieldDtoToFieldComponentConfiguration.CreateMapper(), nameof(FieldDtoToFieldComponentMapper));
        containerRegistry.RegisterInstance(fieldDtoToFieldDtoConfiguration.CreateMapper(), nameof(FieldDtoToFieldDtoMapper));
    }

    private void RegisterFeedIngredientMappers(IContainerRegistry containerRegistry)
    {
        var feedIngredientToFeedIngredientConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<FeedIngredientToFeedIngredientMapper>();
        });

        containerRegistry.RegisterInstance(feedIngredientToFeedIngredientConfiguration.CreateMapper(), nameof(FeedIngredientToFeedIngredientMapper));
    }

    private void RegisterAnimalComponentMappers(IContainerRegistry containerRegistry)
    {
        var animalComponentDtoToAnimalComponentConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<AnimalComponentDtoToAnimalComponentMapper>();
        });

        var animalComponentDtoToAnimalComponentDtoConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<AnimalComponentDtoToAnimalComponentDtoMapper>();
        });

        var animalComponentToAnimalComponentDtoConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<AnimalComponentBaseToAnimalComponentDtoMapper>();
        });

        containerRegistry.RegisterInstance(animalComponentDtoToAnimalComponentConfiguration.CreateMapper(), nameof(AnimalComponentDtoToAnimalComponentMapper));
        containerRegistry.RegisterInstance(animalComponentDtoToAnimalComponentDtoConfiguration.CreateMapper(), nameof(AnimalComponentDtoToAnimalComponentDtoMapper));
        containerRegistry.RegisterInstance(animalComponentToAnimalComponentDtoConfiguration.CreateMapper(), nameof(AnimalComponentBaseToAnimalComponentDtoMapper));
    }

    private void RegisterManagementPeriodMappers(IContainerRegistry containerRegistry)
    {
        var managementPeriodDtoToManagementPeriodDtoConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<ManagementPeriodDtoToManagementPeriodDtoMapper>();
        });

        var managementPeriodToManagementPeriodDtoConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<ManagementPeriodToManagementPeriodDtoMapper>();
        });

        var managementPeriodDtoToManagementPeriodConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<ManagementPeriodDtoToManagementPeriodMapper>();
        });

        containerRegistry.RegisterInstance(managementPeriodDtoToManagementPeriodDtoConfiguration.CreateMapper(), nameof(ManagementPeriodDtoToManagementPeriodDtoMapper));
        containerRegistry.RegisterInstance(managementPeriodToManagementPeriodDtoConfiguration.CreateMapper(), nameof(ManagementPeriodToManagementPeriodDtoMapper));
        containerRegistry.RegisterInstance(managementPeriodDtoToManagementPeriodConfiguration.CreateMapper(), nameof(ManagementPeriodDtoToManagementPeriodMapper));
    }

    private void RegisterClimateMappers(IContainerRegistry containerRegistry)
    {
        var dailyClimateDataToDtoConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<DailyClimateDataToDailyClimateDtoMapper>();
        });

        var dailyClimateDtoToDataConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<DailyClimateDtoToDailyClimateDataMapper>();
        });

        var dailyClimateDtoToDtoConfiguration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<DailyClimateDtoToDailyClimateDtoMapper>();
        });

        containerRegistry.RegisterInstance(dailyClimateDataToDtoConfiguration.CreateMapper(), nameof(DailyClimateDataToDailyClimateDtoMapper));
        containerRegistry.RegisterInstance(dailyClimateDtoToDataConfiguration.CreateMapper(), nameof(DailyClimateDtoToDailyClimateDataMapper));
        containerRegistry.RegisterInstance(dailyClimateDtoToDtoConfiguration.CreateMapper(), nameof(DailyClimateDtoToDailyClimateDtoMapper));
    }
}