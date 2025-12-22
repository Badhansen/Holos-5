using H.Core.Factories;
using H.Core.Models.Animals;
using Microsoft.Extensions.Logging;

namespace H.Core.Services.Animals;

public class AnimalComponentService : ComponentServiceBase, IAnimalComponentService
{
    #region Fields

    private readonly IAnimalComponentFactory _animalComponentFactory;

    private readonly ITransferService<AnimalComponentBase, AnimalComponentDto> _animalComponentTransferService;

    #endregion

    #region Constructors

    public AnimalComponentService(ILogger logger, IAnimalComponentFactory animalComponentFactory, ITransferService<AnimalComponentBase, AnimalComponentDto> animalComponentTransferService) : base(logger)
    {
        if (animalComponentTransferService != null)
        {
            _animalComponentTransferService = animalComponentTransferService; 
        }
        else
        {
            throw new ArgumentNullException(nameof(animalComponentTransferService));
        }

        if (animalComponentFactory != null)
        {
            _animalComponentFactory = animalComponentFactory;
        }
        else
        {
            throw new ArgumentNullException(nameof(animalComponentFactory));
        }
    }

    #endregion

    #region Public Methods

    public IAnimalComponentDto TransferToAnimalComponentDto(AnimalComponentBase animalComponent)
    {
        return _animalComponentTransferService.TransferDomainObjectToDto(animalComponent);
    }

    public AnimalComponentBase TransferAnimalComponentDtoToSystem(AnimalComponentDto animalComponentDto,
        AnimalComponentBase animalComponent)
    {
        return _animalComponentTransferService.TransferDtoToDomainObject(animalComponentDto, animalComponent);
    }

    #endregion
}