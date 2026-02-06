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
    
    // Herd Production Defaults - Used to populate management periods
    // NOTE: These are separate from ManagementPeriodDto properties
    // WHY DUPLICATE PROPERTIES?
    // - Component level: Herd-level defaults for simplified user input
    // - ManagementPeriod level: Actual values used in emissions calculations
    // - When auto-generating animal groups, these defaults populate the management period values
    // - Advanced users can then override individual management period values if needed
    private double _defaultMilkProduction = 25.0;
    private double _defaultMilkFatContent = 3.9;
    private double _defaultMilkProteinContent = 3.2;
    
    // Staggered Progression - Flow Rate Inputs
    private int _calvesEnteringPerYear = 100;
    private int _heifersEnteringPerYear = 30;
    private int _lactatingCowsEnteringPerYear = 30;
    private int _dryCowsEnteringPerYear = 100;
    
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
                RaisePropertyChanged(nameof(SteadyStateDry));
                RaisePropertyChanged(nameof(TotalSteadyStateHerdSize));
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

    #region Properties - Herd Production Defaults

    /// <summary>
    /// Default milk production for the herd (used when creating management periods)
    /// 
    /// ARCHITECTURE NOTE: This is a herd-level default that will be used to populate
    /// ManagementPeriod.MilkProduction when auto-generating animal groups.
    /// 
    /// TWO-LEVEL PATTERN:
    /// 1. User enters this value once (simplified input)
    /// 2. System uses it to populate all lactating cow management periods
    /// 3. Advanced users can override individual management period values later
    /// 
    /// (kg head?¹ day?¹)
    /// </summary>
    [Units(MetricUnitsOfMeasurement.KilogramPerHeadPerDay)]
    public double DefaultMilkProduction
    {
        get => _defaultMilkProduction;
        set => SetProperty(ref _defaultMilkProduction, value);
    }

    /// <summary>
    /// Default milk fat content for the herd (used when creating management periods)
    /// 
    /// ARCHITECTURE NOTE: This is a herd-level default that will be used to populate
    /// ManagementPeriod.MilkFatContent when auto-generating animal groups.
    /// 
    /// Typical values by breed:
    /// - Holstein: 3.5-3.9%
    /// - Jersey: 4.5-5.0%
    /// - Guernsey: 4.2-4.7%
    /// 
    /// (%)
    /// </summary>
    [Units(MetricUnitsOfMeasurement.Percentage)]
    public double DefaultMilkFatContent
    {
        get => _defaultMilkFatContent;
        set => SetProperty(ref _defaultMilkFatContent, value);
    }

    /// <summary>
    /// Default milk protein content for the herd (used when creating management periods)
    /// 
    /// ARCHITECTURE NOTE: This is a herd-level default that will be used to populate
    /// ManagementPeriod.MilkProteinContentAsPercentage when auto-generating animal groups.
    /// 
    /// Typical values by breed:
    /// - Holstein: 3.0-3.2%
    /// - Jersey: 3.6-3.9%
    /// - Guernsey: 3.3-3.6%
    /// 
    /// (%)
    /// </summary>
    [Units(MetricUnitsOfMeasurement.Percentage)]
    public double DefaultMilkProteinContent
    {
        get => _defaultMilkProteinContent;
        set => SetProperty(ref _defaultMilkProteinContent, value);
    }
    
    #endregion
    
    #region Properties - Staggered Progression Flow Rates

    /// <summary>
    /// Number of calves entering the calf stage (birth) per year.
    /// This represents the continuous flow of animals being born into the dairy operation.
    /// 
    /// STAGGERED PROGRESSION MODEL:
    /// Instead of modeling all animals moving through stages synchronously, this approach
    /// models a steady flow of animals entering each stage. This better represents
    /// real dairy operations where calving occurs year-round.
    /// 
    /// CALCULATION NOTE:
    /// This value can be used to calculate steady-state populations:
    /// - Steady-state calves = (CalvesEnteringPerYear) × (Duration in calf stage / 365 days)
    /// - Example: 100 calves/year × (4 months / 12 months) = ~33 calves at any given time
    /// 
    /// (number of animals per year)
    /// </summary>
    public int CalvesEnteringPerYear
    {
        get => _calvesEnteringPerYear;
        set
        {
            if (SetProperty(ref _calvesEnteringPerYear, value))
            {
                RaisePropertyChanged(nameof(SteadyStateCalves));
                RaisePropertyChanged(nameof(TotalSteadyStateHerdSize));
            }
        }
    }
    
    /// <summary>
    /// Number of heifers entering the heifer stage (from calf to heifer transition) per year.
    /// This represents the continuous flow of animals moving from the calf stage into the replacement heifer population.
    /// 
    /// STAGGERED PROGRESSION MODEL:
    /// This value represents heifers that have survived the calf stage and are entering the replacement
    /// stock pool. In a steady-state operation, this equals the number of calves surviving to 4 months.
    /// 
    /// CALCULATION NOTE:
    /// - Steady-state heifers = (HeifersEnteringPerYear) × (Duration in heifer stage / 365 days)
    /// - Example: 30 heifers/year × (20 months / 12 months) = ~50 heifers at any given time
    /// 
    /// (number of animals per year)
    /// </summary>
    public int HeifersEnteringPerYear
    {
        get => _heifersEnteringPerYear;
        set
        {
            if (SetProperty(ref _heifersEnteringPerYear, value))
            {
                RaisePropertyChanged(nameof(SteadyStateHeifers));
                RaisePropertyChanged(nameof(TotalSteadyStateHerdSize));
            }
        }
    }
    
    /// <summary>
    /// Number of cows entering lactation (calving and starting milk production) per year.
    /// This represents the continuous flow of first-calf heifers and re-calving cows entering the lactating herd.
    /// 
    /// STAGGERED PROGRESSION MODEL:
    /// This includes both first-calf heifers completing their first pregnancy and mature cows
    /// calving again after their dry period. In steady-state, this equals the culling rate
    /// plus any herd expansion.
    /// 
    /// CALCULATION NOTE:
    /// - Steady-state lactating = (LactatingCowsEnteringPerYear) × (Lactation period / 365 days)
    /// - Example: 100 cows/year × (305 days / 365 days) = ~84 lactating cows at any given time
    /// 
    /// (number of animals per year)
    /// </summary>
    public int LactatingCowsEnteringPerYear
    {
        get => _lactatingCowsEnteringPerYear;
        set
        {
            if (SetProperty(ref _lactatingCowsEnteringPerYear, value))
            {
                RaisePropertyChanged(nameof(SteadyStateLactating));
                RaisePropertyChanged(nameof(TotalSteadyStateHerdSize));
            }
        }
    }
    
    /// <summary>
    /// Number of cows entering the dry period (stopping milk production before calving) per year.
    /// This represents the continuous flow of lactating cows being dried off in preparation for their next calving.
    /// 
    /// STAGGERED PROGRESSION MODEL:
    /// In steady-state, this should equal the number of cows entering lactation since every cow
    /// goes through a dry period before each lactation. The dry period is a rest phase that allows
    /// the udder to regenerate.
    /// 
    /// CALCULATION NOTE:
    /// - Steady-state dry cows = (DryCowsEnteringPerYear) × (Dry period / 365 days)
    /// - Example: 100 cows/year × (60 days / 365 days) = ~16 dry cows at any given time
    /// 
    /// (number of animals per year)
    /// </summary>
    public int DryCowsEnteringPerYear
    {
        get => _dryCowsEnteringPerYear;
        set
        {
            if (SetProperty(ref _dryCowsEnteringPerYear, value))
            {
                RaisePropertyChanged(nameof(SteadyStateDry));
                RaisePropertyChanged(nameof(TotalSteadyStateHerdSize));
            }
        }
    }

    #endregion
    
    #region Properties - Calculated Steady-State Populations
    
    /// <summary>
    /// Calculated steady-state number of calves based on flow rate and duration in stage.
    /// 
    /// CALCULATION:
    /// Steady-state population = (Animals entering per year) × (Duration in stage / 365 days)
    /// 
    /// ASSUMPTIONS:
    /// - Calf stage duration: 4 months (120 days)
    /// - Formula: CalvesEnteringPerYear × (120 / 365)
    /// 
    /// (number of animals)
    /// </summary>
    public int SteadyStateCalves
    {
        get
        {
            const int calfStageDurationDays = 120; // 4 months
            return (int)Math.Round(CalvesEnteringPerYear * (calfStageDurationDays / 365.0));
        }
    }
    
    /// <summary>
    /// Calculated steady-state number of heifers based on flow rate and duration in stage.
    /// 
    /// CALCULATION:
    /// Steady-state population = (Animals entering per year) × (Duration in stage / 365 days)
    /// 
    /// ASSUMPTIONS:
    /// - Heifer stage duration: 20 months (608 days) - from 4 months old to 24 months at first calving
    /// - Formula: HeifersEnteringPerYear × (608 / 365)
    /// 
    /// (number of animals)
    /// </summary>
    public int SteadyStateHeifers
    {
        get
        {
            const int heiferStageDurationDays = 608; // 20 months (from 4mo to 24mo)
            return (int)Math.Round(HeifersEnteringPerYear * (heiferStageDurationDays / 365.0));
        }
    }
    
    /// <summary>
    /// Calculated steady-state number of lactating cows based on flow rate and lactation period.
    /// 
    /// CALCULATION:
    /// Steady-state population = (Animals entering per year) × (Lactation period / 365 days)
    /// 
    /// ASSUMPTIONS:
    /// - Lactation period: 305 days (standard lactation length)
    /// - Formula: LactatingCowsEnteringPerYear × (305 / 365)
    /// 
    /// (number of animals)
    /// </summary>
    public int SteadyStateLactating
    {
        get
        {
            const int lactationPeriodDays = 305; // Standard lactation length
            return (int)Math.Round(LactatingCowsEnteringPerYear * (lactationPeriodDays / 365.0));
        }
    }
    
    /// <summary>
    /// Calculated steady-state number of dry cows based on flow rate and dry period.
    /// 
    /// CALCULATION:
    /// Steady-state population = (Animals entering per year) × (Dry period / 365 days)
    /// 
    /// USES USER INPUT:
    /// - Dry period: Uses the DryPeriodDays parameter from herd overview
    /// - Formula: DryCowsEnteringPerYear × (DryPeriodDays / 365)
    /// 
    /// (number of animals)
    /// </summary>
    public int SteadyStateDry
    {
        get
        {
            return (int)Math.Round(DryCowsEnteringPerYear * (DryPeriodDays / 365.0));
        }
    }
    
    /// <summary>
    /// Total steady-state herd size (sum of all stages).
    /// 
    /// (number of animals)
    /// </summary>
    public int TotalSteadyStateHerdSize
    {
        get
        {
            return SteadyStateCalves + SteadyStateHeifers + SteadyStateLactating + SteadyStateDry;
        }
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

    /// <summary>
    /// Validates that the default milk production is within a reasonable range
    /// </summary>
    private void ValidateDefaultMilkProduction()
    {
        var key = nameof(DefaultMilkProduction);
        
        if (DefaultMilkProduction < 0)
        {
            AddError(key, "Milk production cannot be negative");
        }
        else if (DefaultMilkProduction > 100)
        {
            AddError(key, "Milk production cannot exceed 100 kg/day");
        }
        else
        {
            RemoveError(key);
        }
    }

    /// <summary>
    /// Validates that the default milk fat content is within a reasonable range
    /// </summary>
    private void ValidateDefaultMilkFatContent()
    {
        var key = nameof(DefaultMilkFatContent);
        
        if (DefaultMilkFatContent < 0)
        {
            AddError(key, "Milk fat content cannot be negative");
        }
        else if (DefaultMilkFatContent > 10)
        {
            AddError(key, "Milk fat content cannot exceed 10%");
        }
        else
        {
            RemoveError(key);
        }
    }

    /// <summary>
    /// Validates that the default milk protein content is within a reasonable range
    /// </summary>
    private void ValidateDefaultMilkProteinContent()
    {
        var key = nameof(DefaultMilkProteinContent);
        
        if (DefaultMilkProteinContent < 0)
        {
            AddError(key, "Milk protein content cannot be negative");
        }
        else if (DefaultMilkProteinContent > 10)
        {
            AddError(key, "Milk protein content cannot exceed 10%");
        }
        else
        {
            RemoveError(key);
        }
    }
    
    /// <summary>
    /// Validates that the calves entering per year is within a reasonable range
    /// </summary>
    private void ValidateCalvesEnteringPerYear()
    {
        var key = nameof(CalvesEnteringPerYear);
        
        if (CalvesEnteringPerYear < 0)
        {
            AddError(key, "Calves entering per year cannot be negative");
        }
        else if (CalvesEnteringPerYear > 10000)
        {
            AddError(key, "Calves entering per year cannot exceed 10,000");
        }
        else
        {
            RemoveError(key);
        }
    }
    
    /// <summary>
    /// Validates that the heifers entering per year is within a reasonable range
    /// </summary>
    private void ValidateHeifersEnteringPerYear()
    {
        var key = nameof(HeifersEnteringPerYear);
        
        if (HeifersEnteringPerYear < 0)
        {
            AddError(key, "Heifers entering per year cannot be negative");
        }
        else if (HeifersEnteringPerYear > 10000)
        {
            AddError(key, "Heifers entering per year cannot exceed 10,000");
        }
        else
        {
            RemoveError(key);
        }
    }
    
    /// <summary>
    /// Validates that the lactating cows entering per year is within a reasonable range
    /// </summary>
    private void ValidateLactatingCowsEnteringPerYear()
    {
        var key = nameof(LactatingCowsEnteringPerYear);
        
        if (LactatingCowsEnteringPerYear < 0)
        {
            AddError(key, "Lactating cows entering per year cannot be negative");
        }
        else if (LactatingCowsEnteringPerYear > 10000)
        {
            AddError(key, "Lactating cows entering per year cannot exceed 10,000");
        }
        else
        {
            RemoveError(key);
        }
    }
    
    /// <summary>
    /// Validates that the dry cows entering per year is within a reasonable range
    /// </summary>
    private void ValidateDryCowsEnteringPerYear()
    {
        var key = nameof(DryCowsEnteringPerYear);
        
        if (DryCowsEnteringPerYear < 0)
        {
            AddError(key, "Dry cows entering per year cannot be negative");
        }
        else if (DryCowsEnteringPerYear > 10000)
        {
            AddError(key, "Dry cows entering per year cannot exceed 10,000");
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
                
            case nameof(DefaultMilkProduction):
                ValidateDefaultMilkProduction();
                break;
                
            case nameof(DefaultMilkFatContent):
                ValidateDefaultMilkFatContent();
                break;
                
            case nameof(DefaultMilkProteinContent):
                ValidateDefaultMilkProteinContent();
                break;
                
            case nameof(CalvesEnteringPerYear):
                ValidateCalvesEnteringPerYear();
                break;
                
            case nameof(HeifersEnteringPerYear):
                ValidateHeifersEnteringPerYear();
                break;
                
            case nameof(LactatingCowsEnteringPerYear):
                ValidateLactatingCowsEnteringPerYear();
                break;
                
            case nameof(DryCowsEnteringPerYear):
                ValidateDryCowsEnteringPerYear();
                break;
        }
    }

    #endregion
}
