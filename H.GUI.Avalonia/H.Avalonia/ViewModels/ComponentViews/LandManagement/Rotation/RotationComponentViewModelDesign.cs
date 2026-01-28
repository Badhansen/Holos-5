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
            Name = "Western Canadian Prairie Rotation",
            StartYear = 2020,
            EndYear = 2025,
            FieldArea = 100.0,
            NumberOfFields = 3,
        };

        // Set up sample rotation parameters
        base.ShiftRotationEnabled = true;

        // Create sample crops for the CropDtos collection
        base.CropDtos = new ObservableCollection<ICropDto>
        {
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
                CropType = CropType.Canola,
                WetYield = 2200,
                AmountOfIrrigation = 0,
                IsSelected = false
            },
            new CropDto
            {
                Year = 2022,
                CropType = CropType.Peas,
                WetYield = 2800,
                AmountOfIrrigation = 0,
                IsSelected = false
            },
            new CropDto
            {
                Year = 2023,
                CropType = CropType.Barley,
                WetYield = 4000,
                AmountOfIrrigation = 0,
                IsSelected = true
            },
            new CropDto
            {
                Year = 2024,
                CropType = CropType.AlfalfaMedicagoSativaL,
                WetYield = 8000,
                AmountOfIrrigation = 100,
                IsSelected = false
            },
            new CropDto
            {
                Year = 2025,
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