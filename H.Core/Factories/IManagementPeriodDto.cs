namespace H.Core.Factories;

public interface IManagementPeriodDto : IDto
{
    int NumberOfDays { get; set; }
    DateTime Start { get; set; }
    DateTime End { get; set; }
    
    // Properties with Units attributes for conversion
    double EnergyRequiredForMilk { get; set; }
    double EnergyRequiredForWool { get; set; }
    double StartWeight { get; set; }
    double EndWeight { get; set; }
    double PeriodDailyGain { get; set; }
    double MilkProduction { get; set; }
    double WoolProduction { get; set; }
    double GainCoefficientA { get; set; }
    double GainCoefficientB { get; set; }
    double LiveWeightChangeOfPregnantAnimal { get; set; }
    double LiveWeightOfYoungAtWeaningAge { get; set; }
    double LiveWeightOfYoungAtBirth { get; set; }
}