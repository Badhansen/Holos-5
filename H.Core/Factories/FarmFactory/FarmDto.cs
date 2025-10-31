using H.Core.Providers.Feed;

namespace H.Core.Factories.FarmFactory;

public class FarmDto : DtoBase, IFarmDto
{
    #region Constructors
    
    public FarmDto()
    {
        this.Diets = new List<IDietDto>();
    }

    #endregion

    #region Properties

    public IList<IDietDto> Diets { get; set; } 

    #endregion
}