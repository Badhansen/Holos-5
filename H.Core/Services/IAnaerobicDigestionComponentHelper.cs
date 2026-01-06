using H.Core.Models.Infrastructure;

namespace H.Core.Services
{
    public interface IAnaerobicDigestionComponentHelper
    {

        string GetUniqueAnaerobicDigestionName(IEnumerable<AnaerobicDigestionComponent> components);

    }
}
