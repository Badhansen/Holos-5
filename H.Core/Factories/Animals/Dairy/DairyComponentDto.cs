using System.ComponentModel;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Core.Factories.Animals;

namespace H.Core.Factories.Animals.Dairy;

/// <summary>
/// A class used to validate input as it relates to a <see cref="H.Core.Models.Animals.Dairy.DairyComponent"/>. 
/// This class is used to validate input before any input is transferred to the <see cref="H.Core.Models.Animals.Dairy.DairyComponent"/>
/// 
/// This DTO contains the herd composition input parameters that drive the lifecycle-based dairy herd calculations.
/// </summary>
public class DairyComponentDto : AnimalComponentDto, IDairyComponentDto
{
    #region Fields

    // Herd Overview - Input Parameters
    private int _totalMilkingCows = 100;
    private double _replacementRate = 30.0;
    private int _calvingIntervalMonths = 14;
    private int _dryPeriodDays = 60;
    private double _calfMortalityRate = 5.0;
    private double _femaleCalfRatio = 50.0;
    
    // Calculated Herd Composition - Read-only outputs
    private int _calculatedCalves;
    private int _calculatedHeifers;
    private int _calculatedLactating;
    private int _calculatedDry;
    
    #endregion

    #region Constructors

    public DairyComponentDto()
    {
        this.PropertyChanged += OnPropertyChanged;
        
        // Calculate initial values
        CalculateHerdComposition();
    }

    #endregion

    #region Properties - Input Parameters

    /// <summary>
    /// The total number of cows in the milking herd
    /// 
    /// (number of animals)
    /// </summary>
    public int TotalMilkingCows
    {
        get => _totalMilkingCows;
        set
        {
            if (SetProperty(ref _totalMilkingCows, value))
            {
                CalculateHerdComposition();
            }
        }
    }

    /// <summary>
    /// The percentage of the herd that is replaced annually
    /// 
    /// (%)
    /// </summary>
    [Units(MetricUnitsOfMeasurement.Percentage)]
    public double ReplacementRate
    {
        get => _replacementRate;
        set
        {
            if (SetProperty(ref _replacementRate, value))
            {
                CalculateHerdComposition();
            }
        }
    }

    /// <summary>
    /// The average number of months between calvings
    /// 
    /// (months)
    /// </summary>
    [Units(MetricUnitsOfMeasurement.Months)]
    public int CalvingIntervalMonths
    {
        get => _calvingIntervalMonths;
        set
        {
            if (SetProperty(ref _calvingIntervalMonths, value))
            {
                CalculateHerdComposition();
            }
        }
    }

    /// <summary>
    /// The number of days before calving that a cow is not milked (dry period)
    /// 
    /// (days)
    /// </summary>
    [Units(MetricUnitsOfMeasurement.Days)]
    public int DryPeriodDays
    {
        get => _dryPeriodDays;
        set
        {
            if (SetProperty(ref _dryPeriodDays, value))
            {
                CalculateHerdComposition();
            }
        }
    }

    /// <summary>
    /// The percentage of calves that die before reaching 4 months of age
    /// 
    /// (%)
    /// </summary>
    [Units(MetricUnitsOfMeasurement.Percentage)]
    public double CalfMortalityRate
    {
        get => _calfMortalityRate;
        set
        {
            if (SetProperty(ref _calfMortalityRate, value))
            {
                CalculateHerdComposition();
            }
        }
    }

    /// <summary>
    /// The expected percentage of female calves born
    /// 
    /// (%)
    /// </summary>
    [Units(MetricUnitsOfMeasurement.Percentage)]
    public double FemaleCalfRatio
    {
        get => _femaleCalfRatio;
        set
        {
            if (SetProperty(ref _femaleCalfRatio, value))
            {
                CalculateHerdComposition();
            }
        }
    }

    #endregion

    #region Properties - Calculated Outputs

    /// <summary>
    /// The calculated number of calves in the herd (birth to 4 months)
    /// 
    /// (number of animals)
    /// </summary>
    public int CalculatedCalves
    {
        get => _calculatedCalves;
        private set => SetProperty(ref _calculatedCalves, value);
    }

    /// <summary>
    /// The calculated number of heifers (replacement stock)
    /// 
    /// (number of animals)
    /// </summary>
    public int CalculatedHeifers
    {
        get => _calculatedHeifers;
        private set => SetProperty(ref _calculatedHeifers, value);
    }

    /// <summary>
    /// The calculated number of lactating cows (producing milk)
    /// 
    /// (number of animals)
    /// </summary>
    public int CalculatedLactating
    {
        get => _calculatedLactating;
        private set => SetProperty(ref _calculatedLactating, value);
    }

    /// <summary>
    /// The calculated number of dry cows (not producing milk, pre-calving)
    /// 
    /// (number of animals)
    /// </summary>
    public int CalculatedDry
    {
        get => _calculatedDry;
        private set => SetProperty(ref _calculatedDry, value);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Calculates the herd composition based on the input parameters
    /// </summary>
    private void CalculateHerdComposition()
    {
        // Calculate based on replacement rate and herd size
        var totalReplacements = (int)Math.Ceiling(TotalMilkingCows * (ReplacementRate / 100.0));
        
        // Calculate dry cows based on dry period
        // Dry period fraction = days dry / days in year
        var dryPeriodFraction = DryPeriodDays / 365.0;
        CalculatedDry = (int)Math.Ceiling(TotalMilkingCows * dryPeriodFraction);
        
        // Lactating = Total milking cows - Dry cows
        CalculatedLactating = TotalMilkingCows - CalculatedDry;
        
        // Heifers = replacement stock
        CalculatedHeifers = totalReplacements;
        
        // Calculate calves (accounting for mortality and female ratio)
        // Assume one calf per cow per year
        var expectedCalves = TotalMilkingCows;
        var survivingCalves = expectedCalves * (1 - CalfMortalityRate / 100.0);
        var femaleCalves = survivingCalves * (FemaleCalfRatio / 100.0);
        CalculatedCalves = (int)Math.Ceiling(femaleCalves);
    }

    #endregion

    #region Validation Methods

    /// <summary>
    /// Validates that the total milking cows is a positive number
    /// </summary>
    private void ValidateTotalMilkingCows()
    {
        var key = nameof(TotalMilkingCows);
        
        if (TotalMilkingCows <= 0)
        {
            AddError(key, "Total milking cows must be greater than zero");
        }
        else if (TotalMilkingCows > 10000)
        {
            AddError(key, "Total milking cows cannot exceed 10,000");
        }
        else
        {
            RemoveError(key);
        }
    }

    /// <summary>
    /// Validates that the replacement rate is within a reasonable range
    /// </summary>
    private void ValidateReplacementRate()
    {
        var key = nameof(ReplacementRate);
        
        if (ReplacementRate < 0)
        {
            AddError(key, "Replacement rate cannot be negative");
        }
        else if (ReplacementRate > 100)
        {
            AddError(key, "Replacement rate cannot exceed 100%");
        }
        else
        {
            RemoveError(key);
        }
    }

    /// <summary>
    /// Validates that the calving interval is within a reasonable range
    /// </summary>
    private void ValidateCalvingIntervalMonths()
    {
        var key = nameof(CalvingIntervalMonths);
        
        if (CalvingIntervalMonths < 10)
        {
            AddError(key, "Calving interval must be at least 10 months");
        }
        else if (CalvingIntervalMonths > 24)
        {
            AddError(key, "Calving interval cannot exceed 24 months");
        }
        else
        {
            RemoveError(key);
        }
    }

    /// <summary>
    /// Validates that the dry period is within a reasonable range
    /// </summary>
    private void ValidateDryPeriodDays()
    {
        var key = nameof(DryPeriodDays);
        
        if (DryPeriodDays < 0)
        {
            AddError(key, "Dry period cannot be negative");
        }
        else if (DryPeriodDays > 120)
        {
            AddError(key, "Dry period cannot exceed 120 days");
        }
        else
        {
            RemoveError(key);
        }
    }

    /// <summary>
    /// Validates that the calf mortality rate is within a reasonable range
    /// </summary>
    private void ValidateCalfMortalityRate()
    {
        var key = nameof(CalfMortalityRate);
        
        if (CalfMortalityRate < 0)
        {
            AddError(key, "Calf mortality rate cannot be negative");
        }
        else if (CalfMortalityRate > 50)
        {
            AddError(key, "Calf mortality rate cannot exceed 50%");
        }
        else
        {
            RemoveError(key);
        }
    }

    /// <summary>
    /// Validates that the female calf ratio is within a reasonable range
    /// </summary>
    private void ValidateFemaleCalfRatio()
    {
        var key = nameof(FemaleCalfRatio);
        
        if (FemaleCalfRatio < 0)
        {
            AddError(key, "Female calf ratio cannot be negative");
        }
        else if (FemaleCalfRatio > 100)
        {
            AddError(key, "Female calf ratio cannot exceed 100%");
        }
        else
        {
            RemoveError(key);
        }
    }

    #endregion

    #region Event Handlers

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == null)
            return;

        // Validate properties when they change
        switch (e.PropertyName)
        {
            case nameof(TotalMilkingCows):
                ValidateTotalMilkingCows();
                break;
                
            case nameof(ReplacementRate):
                ValidateReplacementRate();
                break;
                
            case nameof(CalvingIntervalMonths):
                ValidateCalvingIntervalMonths();
                break;
                
            case nameof(DryPeriodDays):
                ValidateDryPeriodDays();
                break;
                
            case nameof(CalfMortalityRate):
                ValidateCalfMortalityRate();
                break;
                
            case nameof(FemaleCalfRatio):
                ValidateFemaleCalfRatio();
                break;
        }
    }

    #endregion
}
