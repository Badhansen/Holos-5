using H.Core.Models.Climate;
using H.Core.Providers.Climate;

namespace H.Core.Factories.Climate;

public interface IDailyClimateDataFactory
{
    #region Public Methods

    public DailyClimateDto CreateDto(DailyClimateData dailyClimateData);

    #endregion
}