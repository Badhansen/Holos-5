using System.ComponentModel;
using H.Core.CustomAttributes;
using H.Core.Enumerations;

namespace H.Core.Factories
{
    public class ManagementPeriodDto : DtoBase, IManagementPeriodDto
    {
        #region Fields

        private DateTime _start;
        private DateTime _end;
        private int _numberOfDays;
        
        private double _energyRequiredForMilk;
        private double _energyRequiredForWool;
        private double _startWeight;
        private double _endWeight;
        private double _periodDailyGain;
        private double _milkProduction;
        private double _woolProduction;
        private double _gainCoefficientA;
        private double _gainCoefficientB;
        private double _liveWeightChangeOfPregnantAnimal;
        private double _liveWeightOfYoungAtWeaningAge;
        private double _liveWeightOfYoungAtBirth;

        #endregion

        #region Constructors

        public ManagementPeriodDto()
        {
            this.PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Properties

        public DateTime Start
        {
            get => _start;
            set => SetProperty(ref _start, value);
        }

        public DateTime End
        {
            get => _end;
            set => SetProperty(ref _end, value);
        }

        public int NumberOfDays
        {
            get => _numberOfDays;
            set => SetProperty(ref _numberOfDays, value);
        }

        /// <summary>
        /// Energy required to produce 1 kg of milk.
        /// 
        /// MJ kg^-1
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerKilogram)]
        public double EnergyRequiredForMilk
        {
            get => _energyRequiredForMilk;
            set => SetProperty(ref _energyRequiredForMilk, value);
        }

        /// <summary>
        /// MJ kg^-1
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerKilogram)]
        public double EnergyRequiredForWool
        {
            get => _energyRequiredForWool;
            set => SetProperty(ref _energyRequiredForWool, value);
        }

        /// <summary>
        /// Start weight of animals (kg)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double StartWeight
        {
            get => _startWeight;
            set => SetProperty(ref _startWeight, value);
        }

        /// <summary>
        /// End weight of animals (kg)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double EndWeight
        {
            get => _endWeight;
            set => SetProperty(ref _endWeight, value);
        }

        /// <summary>
        /// The daily gain of the animals (kg head-1 day-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double PeriodDailyGain
        {
            get => _periodDailyGain;
            set => SetProperty(ref _periodDailyGain, value);
        }

        /// <summary>
        /// Milk produced per day (kg head⁻¹ day⁻¹)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double MilkProduction
        {
            get => _milkProduction;
            set => SetProperty(ref _milkProduction, value);
        }

        /// <summary>
        /// Wool produced per year (kg year^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double WoolProduction
        {
            get => _woolProduction;
            set => SetProperty(ref _woolProduction, value);
        }

        /// <summary>
        /// MJ kg^-1
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerKilogram)]
        public double GainCoefficientA
        {
            get => _gainCoefficientA;
            set => SetProperty(ref _gainCoefficientA, value);
        }

        /// <summary>
        /// MJ kg^-2
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerKilogramSquared)]
        public double GainCoefficientB
        {
            get => _gainCoefficientB;
            set => SetProperty(ref _gainCoefficientB, value);
        }

        /// <summary>
        /// (kg head^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double LiveWeightChangeOfPregnantAnimal
        {
            get => _liveWeightChangeOfPregnantAnimal;
            set => SetProperty(ref _liveWeightChangeOfPregnantAnimal, value);
        }

        /// <summary>
        /// (kg head^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double LiveWeightOfYoungAtWeaningAge
        {
            get => _liveWeightOfYoungAtWeaningAge;
            set => SetProperty(ref _liveWeightOfYoungAtWeaningAge, value);
        }

        /// <summary>
        /// (kg head ^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double LiveWeightOfYoungAtBirth
        {
            get => _liveWeightOfYoungAtBirth;
            set => SetProperty(ref _liveWeightOfYoungAtBirth, value);
        }

        #endregion

        #region Private Methods

        private void ValidatePeriodName()
        {
            if (string.IsNullOrEmpty(Name))
            {
                AddError(nameof(Name), H.Core.Properties.Resources.ErrorNameCannotBeEmpty);
            }
            else
            {
                RemoveError(nameof(Name));
            }
        }

        private void ValidateStart()
        {
            if ((Start >= End && End != default) || Start == default)
            {
                AddError(nameof(Start), H.Core.Properties.Resources.ErrorStartDate);
            }
            else
            {
                RemoveError(nameof(Start));
            }
        }

        private void ValidateEnd()
        {
            if ((End <= Start && Start != default) || End == default)
            {
                AddError(nameof(End), H.Core.Properties.Resources.ErrorEndDate);
            }
            else
            {
                RemoveError(nameof(End));
            }
        }

        private void ValidateNumberOfDays()
        {
            if (NumberOfDays <= 0)
            {
                AddError(nameof(NumberOfDays), H.Core.Properties.Resources.ErrorMustBeGreaterThan0);
            }
            else
            {
                RemoveError(nameof(NumberOfDays));
            }
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != null)
            {
                if (e.PropertyName.Equals(nameof(Name)))
                {
                    this.ValidatePeriodName();
                }
                else if (e.PropertyName.Equals(nameof(NumberOfDays)))
                {
                    this.ValidateNumberOfDays();
                }
                else if (e.PropertyName.Equals(nameof(Start)))
                {
                    this.ValidateStart();
                }
                else if (e.PropertyName.Equals(nameof(End)))
                {
                    this.ValidateEnd();
                }
            }
        }

        #endregion
    }
}
