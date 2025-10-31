using H.Core.Models;

namespace H.Core.Factories;

public interface IFactory<T>
{
    T CreateDto();
    T CreateDto(Farm farm);
    IDto CreateDtoFromDtoTemplate(IDto template);
}