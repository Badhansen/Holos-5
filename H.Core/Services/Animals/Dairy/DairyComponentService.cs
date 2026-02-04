using AutoMapper;
using H.Core.Factories.Animals.Dairy;
using H.Core.Mappers;
using H.Core.Models;
using H.Core.Models.Animals.Dairy;
using H.Core.Services.Animals;
using Microsoft.Extensions.Logging;
using Prism.Ioc;

namespace H.Core.Services.Animals.Dairy;

/// <summary>
/// Service for managing dairy component operations and data transfer.
/// Handles initialization, validation, and conversion between domain models and DTOs.
/// </summary>
public class DairyComponentService : ComponentServiceBase, IDairyComponentService
{
    #region Fields

    private readonly IMapper _mapper;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the DairyComponentService
    /// </summary>
    /// <param name="logger">Logger for diagnostic and error logging</param>
    /// <param name="containerProvider">Container provider to resolve the dairy-specific mapper</param>
    /// <exception cref="ArgumentNullException">Thrown if containerProvider is null</exception>
    public DairyComponentService(ILogger logger, IContainerProvider containerProvider) : base(logger)
    {
        if (containerProvider == null)
        {
            throw new ArgumentNullException(nameof(containerProvider));
        }

        // Resolve the dairy-specific mapper by name
        _mapper = containerProvider.Resolve<IMapper>(nameof(DairyComponentToDtoMapper));
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes a new dairy component with default values appropriate for a dairy operation.
    /// Ensures a unique name and marks the component as initialized.
    /// </summary>
    /// <param name="farm">The farm to which this dairy component belongs</param>
    /// <param name="dairyComponent">The dairy component to initialize</param>
    public void InitializeComponent(Farm farm, DairyComponent dairyComponent)
    {
        base.InitializeComponent(farm, dairyComponent);
        
        if (dairyComponent == null)
        {
            return;
        }
        
        // Add any dairy-specific initialization here
        // For example, setting default herd parameters
    }

    /// <summary>
    /// Creates a dairy component DTO from a domain model for UI binding.
    /// Uses AutoMapper to transfer properties from the domain model to the DTO.
    /// </summary>
    /// <param name="dairyComponent">The source dairy component domain model</param>
    /// <returns>A DTO suitable for binding in the view</returns>
    public IDairyComponentDto TransferToDairyComponentDto(DairyComponent dairyComponent)
    {
        var dairyComponentDto = _mapper.Map<DairyComponentDto>(dairyComponent);
        return dairyComponentDto;
    }

    /// <summary>
    /// Transfers data from DTO back to the domain model after validation.
    /// Uses AutoMapper to apply changes from the DTO to the existing domain model.
    /// </summary>
    /// <param name="dairyDto">The source DTO containing user input</param>
    /// <param name="dairyComponent">The target domain model to update</param>
    /// <returns>The updated dairy component</returns>
    public DairyComponent TransferDairyDtoToSystem(DairyComponentDto dairyDto, DairyComponent dairyComponent)
    {
        return _mapper.Map(dairyDto, dairyComponent);
    }

    #endregion
}
