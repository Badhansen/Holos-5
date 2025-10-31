using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Models;
using Microsoft.Extensions.Logging;
using Prism.Ioc;

namespace H.Core.Services;

public abstract class ComponentServiceBase : IComponentService
{
    #region Fields

    protected ILogger Logger;
    protected IContainerProvider ContainerProvider;
    protected IUnitsOfMeasurementCalculator UnitsOfMeasurementCalculator;

    #endregion

    protected ComponentServiceBase(ILogger logger, IContainerProvider containerProvider, IUnitsOfMeasurementCalculator unitsOfMeasurementCalculator)
    {
        if (unitsOfMeasurementCalculator != null)
        {
            UnitsOfMeasurementCalculator = unitsOfMeasurementCalculator; 
        }
        else
        {
            throw new ArgumentNullException(nameof(unitsOfMeasurementCalculator));
        }

        if (containerProvider != null)
        {
            ContainerProvider = containerProvider; 
        }
        else
        {
            throw new ArgumentNullException(nameof(containerProvider));
        }

        if (logger != null)
        {
            Logger = logger; 
        }
        else
        {
            throw new ArgumentNullException(nameof(logger));
        }
    }

    protected ComponentServiceBase(ILogger logger, IContainerProvider containerProvider)
    {
        if (containerProvider != null)
        {
            ContainerProvider = containerProvider;
        }
        else
        {
            throw new ArgumentNullException(nameof(containerProvider));
        }

        if (logger != null)
        {
            Logger = logger;
        }
        else
        {
            throw new ArgumentNullException(nameof(logger));
        }
    }

    protected ComponentServiceBase(ILogger logger)
    {
        if (logger != null)
        {
            Logger = logger;
        }
        else
        {
            throw new ArgumentNullException(nameof(logger));
        }
    }

    #region Public Methods
    
    public string GetUniqueComponentName(Farm farm, ComponentBase component)
    {
        var i = 2;

        // Don't add number to component name at first (i.e. just use "Cow-Calf" and not "Cow-Calf #1").
        var proposedName = component.ComponentNameDisplayString;

        // While the names are the same, try and make a unique name for this component.
        while (farm.Components.Where(x => string.IsNullOrWhiteSpace(x.Name) == false).Any(y => y.Name.Equals(proposedName)))
        {
            proposedName = component.ComponentNameDisplayString + " #" + (i++);
        }

        return proposedName;
    }

    public void InitializeComponent(Farm farm, ComponentBase component)
    {
        if (component.IsInitialized)
        {
            return;
        }

        component.IsInitialized = true;
        component.Name = this.GetUniqueComponentName(farm, component);
    }

    #endregion
}