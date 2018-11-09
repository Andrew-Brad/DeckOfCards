using DeckOfCards.CommandResults;
using DeckOfCards.QueryResults;
using DeckOfCards.WebApi.Views;
using AutoMapper;

namespace DeckOfCards.WebApi.View
{
    /// <summary>
    /// This Automapper profile will be scanned by the IoC container and the maps will be auto-registered.
    /// http://docs.automapper.org/en/stable/Projection.html
    /// </summary>
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CardTemplateQueryResult, GetCardTemplateView>()
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.Card.Rank.Name))
                .ForMember(dest => dest.Suit, opt => opt.MapFrom(src => src.Card.Suit.Name))
                .ForMember(dest => dest.CardName, opt => opt.MapFrom(src => src.Card.CardName));

            //CreateMap<CreateWidgetCommandResult, CreateWidgetView>();
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Widget.Id));
            //CreateMap<DeprecateWidgetCommandResult,DeprecateWidgetView>();
        }
    }
}
