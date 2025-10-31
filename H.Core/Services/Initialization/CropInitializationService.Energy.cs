using H.Core.Factories;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Energy;
using Microsoft.Extensions.Logging;

namespace H.Core.Services.Initialization;

public partial class CropInitializationService
{
    #region Public Methods

    public virtual void InitializeFuelEnergy(Farm farm)
    {
        var viewItems = farm.GetAllCropViewItems();
        foreach (var viewItem in viewItems)
        {
            InitializeFuelEnergy(farm, viewItem);
        }
    }

    public virtual void InitializeFuelEnergy(Farm farm, CropViewItem viewItem)
    {
        var soilData = farm.GetPreferredSoilData(viewItem);
        var province = soilData.Province;
        var soilCategory = soilData.SoilFunctionalCategory;
        var tillageType = viewItem.TillageType;
        var cropType = viewItem.CropType;

        _logger.LogDebug($"Getting fuel data for {nameof(viewItem)}: {viewItem.Name}, {nameof(province)}: {province}, {nameof(soilCategory)}: {soilCategory}, {nameof(tillageType)}: {tillageType}, {nameof(cropType)}: {cropType}'");

        // Build a unique cache key based on the parameters
        var cacheKey = $"{nameof(CropInitializationService)}_{nameof(InitializeFuelEnergy)}_{province}_{soilCategory}_{tillageType}_{cropType}";

        // Try to get from cache
        var fuelEnergyEstimates = _cahCacheService.Get<Table50FuelEnergyEstimatesData>(cacheKey);

        if (fuelEnergyEstimates == null)
        {
            fuelEnergyEstimates = _table50FuelEnergyEstimatesProvider.GetFuelEnergyEstimatesDataInstance(
                province: province,
                soilCategory: soilCategory,
                tillageType: tillageType,
                cropType: cropType);

            if (fuelEnergyEstimates != null)
            {
                _cahCacheService.Set(cacheKey, fuelEnergyEstimates);
            }
        }

        if (fuelEnergyEstimates != null)
        {
            viewItem.FuelEnergy = fuelEnergyEstimates.FuelEstimate;
        }
    }

    #endregion
}