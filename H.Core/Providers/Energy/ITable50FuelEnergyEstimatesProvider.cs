using H.Core.Enumerations;

namespace H.Core.Providers.Energy;

public interface ITable50FuelEnergyEstimatesProvider
{
    /// <summary>
    /// This method takes in a set of characteristics and finds the fuel energy estimate given those characteristics.
    /// </summary>
    /// <param name="province">The province for which fuel energy estimate data is required</param>
    /// <param name="soilCategory">The functional soil category for the province and crop</param>
    /// <param name="tillageType">The tillage type used for the crop</param>
    /// <param name="cropType">The type of crop for which fuel energy estimate data is required</param>
    /// <returns> The method returns an instance of FuelEnergyEstimateData based on the characteristics in the parameters. Returns an empty instance of <see cref="Table50FuelEnergyEstimatesData"/> if nothing found
    ///  Unit of measurement of fuel energy estimate value = GJ ha-1</returns>
    Table50FuelEnergyEstimatesData GetFuelEnergyEstimatesDataInstance(Province province, SoilFunctionalCategory soilCategory, TillageType tillageType, CropType cropType);
}