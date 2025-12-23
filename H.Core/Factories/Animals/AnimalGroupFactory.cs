using AutoMapper;
using H.Core.Mappers;
using H.Core.Models;
using Prism.Ioc;

namespace H.Core.Factories.Animals;

public class AnimalGroupFactory : IAnimalGroupFactory
{
    #region Fields

    private readonly IMapper _animalGroupDtoToAnimalGroupDtoMapper;

    #endregion

    #region Constructors

    public AnimalGroupFactory()
    {
    }

    public AnimalGroupFactory(IContainerProvider containerProvider)
    {
        if (containerProvider != null)
        {
            _animalGroupDtoToAnimalGroupDtoMapper = containerProvider.Resolve<IMapper>(nameof(AnimalGroupDtoToAnimalGroupDtoMapper));
        }
        else
        {
            throw new ArgumentNullException(nameof(containerProvider));
        }
    }

    #endregion

    #region Public Methods
    
    public AnimalGroupDto CreateDto()
    {
        return new AnimalGroupDto();
    }

    public AnimalGroupDto CreateDto(Farm farm)
    {
        return new AnimalGroupDto();
    }

    public IDto CreateDtoFromDtoTemplate(IDto template)
    {
        var result = new AnimalGroupDto();

        if (_animalGroupDtoToAnimalGroupDtoMapper != null)
        {
            _animalGroupDtoToAnimalGroupDtoMapper.Map(template, result);
        }

        return result;
    } 

    #endregion
}