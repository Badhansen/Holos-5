using System;
using System.Collections.ObjectModel;
using Avalonia.Media;
using H.Core.Enumerations;
using H.Core.Factories.Crops;
using H.Core.Factories.Rotations;
using H.Core.Services.CropColorService;
using System.Linq;

namespace H.Avalonia.ViewModels.ComponentViews.LandManagement.Rotation;

public class RotationComponentViewModelDesign : RotationComponentViewModel
{
    public RotationComponentViewModelDesign() : base()
    {
        // Create an instance of the crop color service for design-time use
        var cropColorService = new CropColorService();

        // Initialize the DTO with sample data
        base.SelectedRotationComponentDto = new RotationComponentDto
        {
            Name = "Diverse Crop Rotation Showcase",
            StartYear = 2020,
            EndYear = 2035,
            FieldArea = 100.0,
            NumberOfFields = 4,
        };

        // Set up sample rotation parameters
        base.ShiftRotationEnabled = true;

        // Create sample crops for the CropDtos collection showcasing all color categories
        base.CropDtos = new ObservableCollection<ICropDto>
        {
            // Cereals - Orange (#FFF3E0)
            new CropDto
            {
                Year = 2020,
                CropType = CropType.Wheat,
                WetYield = 3500,
                AmountOfIrrigation = 0,
                IsSelected = false
            },
            new CropDto
            {
                Year = 2021,
                CropType = CropType.Barley,
                WetYield = 4000,
                AmountOfIrrigation = 0,
                IsSelected = false
            },
            new CropDto
            {
                Year = 2022,
                CropType = CropType.Oats,
                WetYield = 3800,
                AmountOfIrrigation = 0,
                IsSelected = false
            },
            new CropDto
            {
                Year = 2023,
                CropType = CropType.Corn,
                WetYield = 9500,
                AmountOfIrrigation = 50,
                IsSelected = false
            },
            // Oilseeds - Green (#E8F5E9)
            new CropDto
            {
                Year = 2024,
                CropType = CropType.Canola,
                WetYield = 2200,
                AmountOfIrrigation = 0,
                IsSelected = false
            },
            new CropDto
            {
                Year = 2025,
                CropType = CropType.Flax,
                WetYield = 1800,
                AmountOfIrrigation = 0,
                IsSelected = false
            },
            new CropDto
            {
                Year = 2026,
                CropType = CropType.Sunflower,
                WetYield = 2400,
                AmountOfIrrigation = 0,
                IsSelected = false
            },
            new CropDto
            {
                Year = 2027,
                CropType = CropType.Soybeans,
                WetYield = 3200,
                AmountOfIrrigation = 0,
                IsSelected = true
            },
            // Pulses - Blue (#E3F2FD)
            new CropDto
            {
                Year = 2028,
                CropType = CropType.Peas,
                WetYield = 2800,
                AmountOfIrrigation = 0,
                IsSelected = false
            },
            new CropDto
            {
                Year = 2029,
                CropType = CropType.Lentils,
                WetYield = 2100,
                AmountOfIrrigation = 0,
                IsSelected = false
            },
            new CropDto
            {
                Year = 2030,
                CropType = CropType.Chickpeas,
                WetYield = 2400,
                AmountOfIrrigation = 0,
                IsSelected = false
            },
            new CropDto
            {
                Year = 2031,
                CropType = CropType.FabaBeans,
                WetYield = 3500,
                AmountOfIrrigation = 0,
                IsSelected = false
            },
            // Forages - Purple (#F3E5F5)
            new CropDto
            {
                Year = 2032,
                CropType = CropType.AlfalfaMedicagoSativaL,
                WetYield = 8000,
                AmountOfIrrigation = 100,
                IsSelected = false
            },
            new CropDto
            {
                Year = 2033,
                CropType = CropType.TameGrass,
                WetYield = 6000,
                AmountOfIrrigation = 50,
                IsSelected = false
            },
            new CropDto
            {
                Year = 2034,
                CropType = CropType.GrassHay,
                WetYield = 5500,
                AmountOfIrrigation = 30,
                IsSelected = false
            },
            // Fallow - Gray (#FAFAFA)
            new CropDto
            {
                Year = 2035,
                CropType = CropType.Fallow,
                WetYield = 0,
                AmountOfIrrigation = 0,
                IsSelected = false
            }
        };

        // Field assignment rows will be generated automatically by the base class
        // when CropDtos is set

        // Set validation message
        ShowValidationMessage = true;
        ValidationMessage = "✓ All parameters are valid. Ready to create rotation with 3 fields over 6 years.";
    }

    // Design-time properties
    public bool ShowValidationMessage { get; set; }
    public string ValidationMessage { get; set; }

    public int RotationLength => SelectedRotationComponentDto != null 
        ? (SelectedRotationComponentDto.EndYear - SelectedRotationComponentDto.StartYear + 1) 
        : 0;

    public double TotalRotationArea => SelectedRotationComponentDto != null 
        ? (SelectedRotationComponentDto.FieldArea * SelectedRotationComponentDto.NumberOfFields) 
        : 0;

    public int TotalCropYears => SelectedRotationComponentDto != null
        ? (SelectedRotationComponentDto.NumberOfFields * RotationLength)
        : 0;
}