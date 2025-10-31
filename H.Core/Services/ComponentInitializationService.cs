using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Animals;
using H.Core.Services.LandManagement.Fields;
using H.Core.Services.StorageService;

namespace H.Core.Services;

public class ComponentInitializationService : IComponentInitializationService
{
    #region Fields

    private readonly IFieldComponentService _fieldComponentService;
    private readonly IStorageService _storageService;
    private IAnimalComponentService _animalComponentService;

    #endregion

    #region Constructors

    public ComponentInitializationService(IStorageService storageService, IFieldComponentService fieldComponentService, IAnimalComponentService animalComponentService)
    {
        if (animalComponentService  != null)
        {
            _animalComponentService = animalComponentService; 
        }
        else
        {
            throw new ArgumentNullException(nameof(animalComponentService));
        }

        if (storageService != null)
        {
            _storageService = storageService;
        }
        else
        {
            throw new ArgumentNullException(nameof(storageService));
        }

        if (fieldComponentService != null)
        {
            _fieldComponentService = fieldComponentService;
        }
        else
        {
            throw new ArgumentNullException(nameof(fieldComponentService));
        }
    }

    #endregion

    #region Public Methods

    public void Initialize(ComponentBase componentBase)
    {
        var activeFarm = _storageService.GetActiveFarm();

        if (componentBase is FieldSystemComponent fieldSystemComponent)
        {
            _fieldComponentService.InitializeFieldSystemComponent(activeFarm, fieldSystemComponent);
        }
        else if (componentBase is AnimalComponentBase animalComponentBase)
        {
            _animalComponentService.InitializeComponent(activeFarm, animalComponentBase);
        }
    } 

    #endregion
}