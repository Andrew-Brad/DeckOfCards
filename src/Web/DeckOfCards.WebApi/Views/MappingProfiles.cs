using DeckOfCards.CommandResults;
using DeckOfCards.QueryResults;
using DeckOfCards.WebApi.Views;
using AutoMapper;
using DeckOfCards.Commands;
using DeckOfCards.Domain;
using System.Collections.Generic;
using System.Linq;

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

            CreateMap<NewDeckOfCardsCommandResult, NewDeckView>()
                .ForMember(dest => dest.Cards, opt => opt.MapFrom(src => src.Deck));
            CreateMap<Deck, List<NewDeckView.CardDto>>()
                .ProjectUsing(x => x.ShowCards()
                    .Select<PlayingCard, NewDeckView.CardDto>(y => new NewDeckView.CardDto()
                    {
                        Suit = y.Template.Suit.Name,
                        Rank = y.Template.Rank.Name,
                        Id = y.Id.ToString(),
                        Name = y.Template.CardName
                    }).ToList());

            CreateMap<PlayingCard, NewDeckView.CardDto>();


            //CreateMap<CreateWidgetCommandResult, CreateWidgetView>();
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Widget.Id));
            //CreateMap<DeprecateWidgetCommandResult,DeprecateWidgetView>();
        }
    }
}
