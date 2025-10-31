using DynamicData;
using H.Core.Services.DietService;
using H.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace H.Core.Factories.FarmFactory;

public class FarmFactory : IFarmFactory
{
    #region Fields

    private readonly IDietService _dietService;
    private ICacheService _cacheService;
    private ILogger _logger;

    #endregion

    #region Constructors

    public FarmFactory(IDietService dietService, ICacheService cacheService, ILogger logger)
    {
        if (logger != null)
        {
            _logger = logger; 
        }
        else
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (cacheService != null)
        {
            _cacheService = cacheService;
        }
        else
        {
            throw new ArgumentNullException(nameof(cacheService));
        }

        if (dietService != null)
        {
            _dietService = dietService;
        }
        else
        {
            throw new ArgumentNullException(nameof(dietService));
        }
    }

    #endregion

    #region Public Methods
    
    public IFarmDto Create()
    {
        var farm = new FarmDto();

        var diets = _dietService.GetDiets();
        farm.Diets.AddRange(diets);

        return farm;
    } 

    #endregion
}