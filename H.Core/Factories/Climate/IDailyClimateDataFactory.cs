using H.Core.Models.Climate;
using H.Core.Providers.Climate;

namespace H.Core.Factories.Climate;

public interface IDailyClimateDataFactory : IFactory<DailyClimateDto>
{
    #region Public Methods

    /// <summary>
    /// Creates a new instance of <see cref="DailyClimateDto"/> based on the state of an existing <see cref="DailyClimateData"/>. 
    /// This method is used to convert daily climate data to a DTO for view binding.
    /// </summary>
    /// <param name="dailyClimateData">The <see cref="DailyClimateData"/> that will be used to provide values for the new <see cref="DailyClimateDto"/> instance</param>
    /// <returns>A new <see cref="DailyClimateDto"/> instance</returns>
    DailyClimateDto CreateDto(DailyClimateData dailyClimateData);

    /// <summary>
    /// Creates a new instance of <see cref="DailyClimateData"/> based on the state of an existing <see cref="DailyClimateDto"/>. 
    /// This method is used to convert DTO data back to the domain model.
    /// </summary>
    /// <param name="dailyClimateDto">The <see cref="DailyClimateDto"/> that will be used to provide values for the new <see cref="DailyClimateData"/> instance</param>
    /// <returns>A new <see cref="DailyClimateData"/> instance</returns>
    DailyClimateData CreateData(DailyClimateDto dailyClimateDto);

    #endregion
}