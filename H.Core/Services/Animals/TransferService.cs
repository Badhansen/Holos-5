using AutoMapper;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Converters;
using H.Core.Factories;
using H.Core.Mappers;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Infrastructure;

namespace H.Core.Services.Animals
{
    /// <summary>
    /// Provides functionality to transfer data between domain model objects and their corresponding Data Transfer Objects (DTOs).
    /// Handles mapping, unit conversion, and property value transformation between internal system models and external-facing DTOs.
    /// </summary>
    /// <typeparam name="TModelBase">The type of the domain model, must inherit from ModelBase.</typeparam>
    /// <typeparam name="TDto">The type of the Data Transfer Object, must implement IDto.</typeparam>
    public class TransferService<TModelBase, TDto> : ITransferService<TModelBase, TDto>
        where TModelBase : ModelBase
        where TDto : IDto, new()
    {
        #region Fields

        private readonly IUnitsOfMeasurementCalculator _unitsOfMeasurementCalculator;
        private readonly IFactory<TDto> _dtoFactory;
        private readonly IMapper _dtoToModelMapper;
        private readonly IMapper _modelToDtoMapper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferService{TModelBase, TDto}"/> class.
        /// Configures mapping and unit conversion between model and DTO types.
        /// </summary>
        /// <param name="unitsOfMeasurementCalculator">Calculator for handling unit conversions.</param>
        /// <param name="dtoFactory">Factory for creating DTO instances.</param>
        /// <param name="dtoToModelMapper"></param>
        /// <param name="modelToDtoMapper"></param>
        /// <exception cref="ArgumentNullException">Thrown if required dependencies are null.</exception>
        public TransferService(IUnitsOfMeasurementCalculator unitsOfMeasurementCalculator, IFactory<TDto> dtoFactory, IMapper dtoToModelMapper, IMapper modelToDtoMapper)
        {
            if (dtoFactory != null)
            {
                _dtoFactory = dtoFactory;
            }
            else
            {
                throw new ArgumentNullException(nameof(dtoFactory));
            }

            if (unitsOfMeasurementCalculator != null)
            {
                _unitsOfMeasurementCalculator = unitsOfMeasurementCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(unitsOfMeasurementCalculator));
            }

            if (modelToDtoMapper != null)
            {
                _modelToDtoMapper = modelToDtoMapper; 
            }
            else
            {
                throw new ArgumentNullException(nameof(modelToDtoMapper));
            }

            if (dtoToModelMapper != null)
            {
                _dtoToModelMapper = dtoToModelMapper;
            }
            else 
            {
                throw new ArgumentNullException(nameof(dtoToModelMapper));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Transfers a domain model object to its corresponding DTO, applying property mapping and unit conversion as needed.
        /// </summary>
        /// <param name="model">The domain model instance to transfer.</param>
        /// <returns>A DTO instance with values mapped and converted for external use.</returns>
        public TDto TransferDomainObjectToDto(TModelBase model)
        {
            var dto = _dtoFactory.CreateDto(new Farm());

            // Use the internal mapper
            _modelToDtoMapper.Map(model, dto);

            // All numerical values are stored internally as metric values
            var propertyConverter = new PropertyConverter<IDto>(dto);

            // Get all properties that might need to be converted to imperial units before being shown to the user
            foreach (var property in propertyConverter.PropertyInfos)
            {
                // Convert the value from metric to imperial as needed. Note the converter won't convert anything if the display is in metric units
                var bindingValue = propertyConverter.GetBindingValueFromSystem(property, _unitsOfMeasurementCalculator.GetUnitsOfMeasurement());

                // Set the value of the property before displaying to the user
                property.SetValue(dto, bindingValue);
            }

            return dto;
        }

        /// <summary>
        /// Transfers a DTO to its corresponding domain model object, applying property mapping and unit conversion as needed.
        /// </summary>
        /// <param name="dto">The DTO instance to transfer.</param>
        /// <param name="model">The domain model instance to update.</param>
        /// <returns>A new domain model instance with values mapped and converted for internal use.</returns>
        public TModelBase TransferDtoToDomainObject(TDto dto, TModelBase model)
        {
            // Create a copy of the DTO since we don't want to change values on the original that is still bound to the GUI
            var copy = _dtoFactory.CreateDtoFromDtoTemplate(dto);

            // All numerical values are stored internally as metric values
            var propertyConverter = new PropertyConverter<IDto>(copy);

            // Get all properties that might need to be converted to imperial units before being shown to the user
            foreach (var property in propertyConverter.PropertyInfos)
            {
                // Convert the value from imperial to metric as needed (no conversion will occur if display is using metric)
                var bindingValue = propertyConverter.GetSystemValueFromBinding(property, _unitsOfMeasurementCalculator.GetUnitsOfMeasurement());

                // Set the value on the copy of the DTO
                property.SetValue(copy, bindingValue);
            }

            // Map values from the copy of the DTO to the internal system object
            _dtoToModelMapper.Map(copy, model);

            return model;
        }

        #endregion
    }
}