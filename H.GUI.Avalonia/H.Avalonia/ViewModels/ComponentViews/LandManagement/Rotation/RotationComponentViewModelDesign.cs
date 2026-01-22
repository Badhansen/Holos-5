using System;
using System.Collections.ObjectModel;
using Avalonia.Media;
using H.Core.Enumerations;
using H.Core.Factories.Crops;
using H.Core.Factories.Rotations;

namespace H.Avalonia.ViewModels.ComponentViews.LandManagement.Rotation;

public class RotationComponentViewModelDesign : RotationComponentViewModel
{
    public RotationComponentViewModelDesign()
    {
        // Initialize the DTO with sample data
        base.SelectedRotationComponentDto = new RotationComponentDto
        {
            Name = "Western Canadian Prairie Rotation",
            StartYear = 2020,
            EndYear = 2025,
            FieldArea = 100.0
        };

        // Set up sample rotation parameters
        NumberOfFields = 3;
        ShiftRotationEnabled = true;

        // Create sample crops for the timeline (6-year rotation: Wheat → Canola → Peas → Barley → Alfalfa → Fallow)
        CropsInRotation = new ObservableCollection<CropTimelineItemDesign>
        {
            new CropTimelineItemDesign
            {
                Year = 2020,
                YearLabel = "2020",
                CropType = CropType.Wheat,
                CropTypeName = "Wheat",
                CropCategory = "Cereal",
                CropIcon = "🌾",
                CropColorBrush = Brush.Parse("#FFF3E0"), // Orange for cereals
                IsSelected = false
            },
            new CropTimelineItemDesign
            {
                Year = 2021,
                YearLabel = "2021",
                CropType = CropType.Canola,
                CropTypeName = "Canola",
                CropCategory = "Oilseed",
                CropIcon = "🌻",
                CropColorBrush = Brush.Parse("#E8F5E9"), // Green for oilseeds
                IsSelected = false
            },
            new CropTimelineItemDesign
            {
                Year = 2022,
                YearLabel = "2022",
                CropType = CropType.Peas,
                CropTypeName = "Field Peas",
                CropCategory = "Pulse",
                CropIcon = "🫘",
                CropColorBrush = Brush.Parse("#E3F2FD"), // Blue for pulses
                IsSelected = false
            },
            new CropTimelineItemDesign
            {
                Year = 2023,
                YearLabel = "2023",
                CropType = CropType.Barley,
                CropTypeName = "Barley",
                CropCategory = "Cereal",
                CropIcon = "🌾",
                CropColorBrush = Brush.Parse("#FFF3E0"), // Orange for cereals
                IsSelected = true // Selected for demonstration
            },
            new CropTimelineItemDesign
            {
                Year = 2024,
                YearLabel = "2024",
                CropType = CropType.AlfalfaMedicagoSativaL,
                CropTypeName = "Alfalfa",
                CropCategory = "Forage",
                CropIcon = "🍀",
                CropColorBrush = Brush.Parse("#F3E5F5"), // Purple for forages
                IsSelected = false
            },
            new CropTimelineItemDesign
            {
                Year = 2025,
                YearLabel = "2025",
                CropType = CropType.Fallow,
                CropTypeName = "Fallow",
                CropCategory = "Fallow",
                CropIcon = "⬜",
                CropColorBrush = Brush.Parse("#FAFAFA"), // Gray for fallow
                IsSelected = false
            }
        };

        // Create sample field assignment preview data
        FieldAssignmentPreview = new ObservableCollection<FieldAssignmentRowDesign>
        {
            new FieldAssignmentRowDesign
            {
                FieldName = "Field 1 (100 ha)",
                Year2020 = "Wheat 🌾",
                Year2020Background = Brush.Parse("#FFF3E0"), // Orange for cereals
                Year2021 = "Canola 🌻",
                Year2021Background = Brush.Parse("#E8F5E9"), // Green for oilseeds
                Year2022 = "Peas 🫘",
                Year2022Background = Brush.Parse("#E3F2FD"), // Blue for pulses
                Year2023 = "Barley 🌾",
                Year2023Background = Brush.Parse("#FFF3E0"), // Orange for cereals
                Year2024 = "Alfalfa 🍀",
                Year2024Background = Brush.Parse("#F3E5F5"), // Purple for forages
                Year2025 = "Fallow ⬜",
                Year2025Background = Brush.Parse("#FAFAFA") // Gray for fallow
            },
            new FieldAssignmentRowDesign
            {
                FieldName = "Field 2 (100 ha)",
                Year2020 = "Fallow ⬜",
                Year2020Background = Brush.Parse("#FAFAFA"), // Gray for fallow
                Year2021 = "Wheat 🌾",
                Year2021Background = Brush.Parse("#FFF3E0"), // Orange for cereals
                Year2022 = "Canola 🌻",
                Year2022Background = Brush.Parse("#E8F5E9"), // Green for oilseeds
                Year2023 = "Peas 🫘",
                Year2023Background = Brush.Parse("#E3F2FD"), // Blue for pulses
                Year2024 = "Barley 🌾",
                Year2024Background = Brush.Parse("#FFF3E0"), // Orange for cereals
                Year2025 = "Alfalfa 🍀",
                Year2025Background = Brush.Parse("#F3E5F5") // Purple for forages
            },
            new FieldAssignmentRowDesign
            {
                FieldName = "Field 3 (100 ha)",
                Year2020 = "Alfalfa 🍀",
                Year2020Background = Brush.Parse("#F3E5F5"), // Purple for forages
                Year2021 = "Fallow ⬜",
                Year2021Background = Brush.Parse("#FAFAFA"), // Gray for fallow
                Year2022 = "Wheat 🌾",
                Year2022Background = Brush.Parse("#FFF3E0"), // Orange for cereals
                Year2023 = "Canola 🌻",
                Year2023Background = Brush.Parse("#E8F5E9"), // Green for oilseeds
                Year2024 = "Peas 🫘",
                Year2024Background = Brush.Parse("#E3F2FD"), // Blue for pulses
                Year2025 = "Barley 🌾",
                Year2025Background = Brush.Parse("#FFF3E0") // Orange for cereals
            }
        };

        // Set validation message
        ShowValidationMessage = true;
        ValidationMessage = "✓ All parameters are valid. Ready to create rotation with 3 fields over 6 years.";
    }

    // Design-time properties
    public int NumberOfFields { get; set; }
    public bool ShiftRotationEnabled { get; set; }
    public ObservableCollection<CropTimelineItemDesign> CropsInRotation { get; set; }
    public ObservableCollection<FieldAssignmentRowDesign> FieldAssignmentPreview { get; set; }
    public bool ShowValidationMessage { get; set; }
    public string ValidationMessage { get; set; }

    public int RotationLength => SelectedRotationComponentDto != null 
        ? (SelectedRotationComponentDto.EndYear - SelectedRotationComponentDto.StartYear + 1) 
        : 0;

    public double TotalRotationArea => SelectedRotationComponentDto != null 
        ? (SelectedRotationComponentDto.FieldArea * NumberOfFields) 
        : 0;

    public int TotalCropYears => NumberOfFields * RotationLength;

    public bool HasNoCrops => CropsInRotation == null || CropsInRotation.Count == 0;
}

/// <summary>
/// Design-time model for a crop timeline item
/// </summary>
public class CropTimelineItemDesign
{
    public int Year { get; set; }
    public string YearLabel { get; set; }
    public CropType CropType { get; set; }
    public string CropTypeName { get; set; }
    public string CropCategory { get; set; }
    public string CropIcon { get; set; }
    public IBrush CropColorBrush { get; set; }
    public bool IsSelected { get; set; }
}

/// <summary>
/// Design-time model for field assignment preview row
/// </summary>
public class FieldAssignmentRowDesign
{
    public string FieldName { get; set; }
    public string Year2020 { get; set; }
    public IBrush Year2020Background { get; set; }
    public string Year2021 { get; set; }
    public IBrush Year2021Background { get; set; }
    public string Year2022 { get; set; }
    public IBrush Year2022Background { get; set; }
    public string Year2023 { get; set; }
    public IBrush Year2023Background { get; set; }
    public string Year2024 { get; set; }
    public IBrush Year2024Background { get; set; }
    public string Year2025 { get; set; }
    public IBrush Year2025Background { get; set; }
}