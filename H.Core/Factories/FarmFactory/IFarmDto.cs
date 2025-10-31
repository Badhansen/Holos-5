using H.Core.Providers.Feed;

namespace H.Core.Factories.FarmFactory;

public interface IFarmDto
{
    IList<IDietDto> Diets { get; set; }
}