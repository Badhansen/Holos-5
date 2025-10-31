using AutoMapper;
using H.Core.Providers.Feed;

namespace H.Core.Mappers;

public class FeedIngredientToFeedIngredientMapper : Profile
{
    public FeedIngredientToFeedIngredientMapper()
    {
        CreateMap<FeedIngredient, FeedIngredient>();
    }
}