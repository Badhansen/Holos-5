using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Energy;
using H.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace H.Core.Services.Initialization;

public partial class CropInitializationService : ICropInitializationService
{
    #region Fields

    private readonly ITable50FuelEnergyEstimatesProvider _table50FuelEnergyEstimatesProvider;
    private ICacheService _cahCacheService;
    private ILogger _logger;

    #endregion

    #region Constructors

    public CropInitializationService(
        ILogger logger,
        ICacheService cahCacheService,
        ITable50FuelEnergyEstimatesProvider table50FuelEnergyEstimatesProvider)
    {
        if (logger != null)
        {
            _logger = logger; 
        }
        else
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (cahCacheService != null)
        {
            _cahCacheService = cahCacheService;
        }
        else
        {
            throw new ArgumentNullException(nameof(cahCacheService));
        }

        if (table50FuelEnergyEstimatesProvider != null)
        {
            _table50FuelEnergyEstimatesProvider = table50FuelEnergyEstimatesProvider;
        }
        else
        {
            throw new ArgumentNullException(nameof(table50FuelEnergyEstimatesProvider));
        }
    }

    #endregion

    #region Public Methods

    public void Initialize(Farm farm)
    {
        this.InitializeFuelEnergy(farm);
    }

    public void Initialize(CropViewItem viewItem, Farm farm)
    {
        this.InitializeFuelEnergy(farm, viewItem);
    }

    #endregion
}