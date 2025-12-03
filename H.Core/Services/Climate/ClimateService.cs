using System;
using H.Core.Factories.Climate;
using H.Core.Models;
using H.Core.Models.Climate;
using H.Core.Providers.Climate;
using H.Core.Services.Animals;
using Microsoft.Extensions.Logging;

namespace H.Core.Services.Climate
{
    /// <summary>
    /// Orchestrates operations for DailyClimateData and DailyClimateDto objects.
    /// - Creates and transfers data between domain models and DTOs using <see cref="IDailyClimateTransferService"/>.
    /// - Applies unit conversions via configured transfer services.
    /// - Provides climate data operations for UI-bound workflows.
    /// </summary>
    public class ClimateService : ComponentServiceBase, IClimateService
    {
        #region Fields

        private readonly IDailyClimateDataFactory _dailyClimateDataFactory;
        private readonly ITransferService<DailyClimateData, DailyClimateDto> _climateTransferService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ClimateService"/>.
        /// </summary>
        /// <param name="dailyClimateDataFactory">Factory used to create climate-related DTOs and data objects.</param>
        /// <param name="climateTransferService">
        /// Transfer service that maps between <see cref="DailyClimateData"/> (domain) and <see cref="DailyClimateDto"/> (DTO),
        /// including unit conversions for UI binding and persistence.
        /// </param>
        /// <param name="logger">Logger injected into <see cref="ComponentServiceBase"/> for diagnostics.</param>
        /// <exception cref="ArgumentNullException">Thrown when any required dependency is null.</exception>
        public ClimateService(
            IDailyClimateDataFactory dailyClimateDataFactory,
            ITransferService<DailyClimateData, DailyClimateDto> climateTransferService,
            ILogger logger) : base(logger)
        {
            if (climateTransferService != null)
            {
                _climateTransferService = climateTransferService;
            }
            else
            {
                throw new ArgumentNullException(nameof(climateTransferService));
            }

            if (dailyClimateDataFactory != null)
            {
                _dailyClimateDataFactory = dailyClimateDataFactory;
            }
            else
            {
                throw new ArgumentNullException(nameof(dailyClimateDataFactory));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts a <see cref="DailyClimateData"/> domain object to a <see cref="DailyClimateDto"/> for UI binding.
        /// </summary>
        /// <param name="dailyClimateData">Domain model instance.</param>
        /// <returns>Mapped DTO instance.</returns>
        public DailyClimateDto TransferDailyClimateDataToDto(DailyClimateData dailyClimateData)
        {
            Logger?.LogDebug("Transferring DailyClimateData to DTO for year: {Year}", dailyClimateData?.Year);
            
            return _climateTransferService.TransferDomainObjectToDto(dailyClimateData);
        }

        /// <summary>
        /// Applies values from a <see cref="DailyClimateDto"/> to an existing <see cref="DailyClimateData"/> domain object.
        /// </summary>
        /// <param name="dailyClimateDto">Source DTO bound to the UI.</param>
        /// <param name="dailyClimateData">Target domain model to update.</param>
        /// <returns>The updated <see cref="DailyClimateData"/>.</returns>
        public DailyClimateData TransferClimateDtoToSystem(DailyClimateDto dailyClimateDto, DailyClimateData dailyClimateData)
        {
            Logger?.LogDebug("Transferring DailyClimateDto to system for year: {Year}", dailyClimateDto?.Year);
            
            return _climateTransferService.TransferDtoToDomainObject(dailyClimateDto, dailyClimateData);
        }

        /// <summary>
        /// Creates a new domain object from a DTO for adding to the system.
        /// </summary>
        /// <param name="dailyClimateDto">DTO used to create the new domain object.</param>
        /// <returns>New domain object instance.</returns>
        public DailyClimateData CreateDataFromDto(DailyClimateDto dailyClimateDto)
        {
            Logger?.LogDebug("Creating new DailyClimateData from DTO for year: {Year}", dailyClimateDto?.Year);
            
            if (dailyClimateDto == null)
            {
                Logger?.LogWarning("Cannot create DailyClimateData from null DTO");
                return null;
            }

            var newData = _dailyClimateDataFactory.CreateData(dailyClimateDto);
            
            Logger?.LogInformation("Successfully created new DailyClimateData for year: {Year}", newData.Year);
            
            return newData;
        }

        #endregion

        #region Private Methods

        #endregion
    }
}