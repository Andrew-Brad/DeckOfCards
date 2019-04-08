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
            // Our most common mapping, from a Deck domain object to the clien'ts veiw of each card
            CreateMap<Deck, List<NewDeckView.CardViewDto>>()
                .ConvertUsing(x => x == null ? new List<NewDeckView.CardViewDto>() : x.ShowCards()
                .Select<PlayingCard, NewDeckView.CardViewDto>(y => new NewDeckView.CardViewDto()
                {
                    Suit = y.Template.Suit.Name,
                    Rank = y.Template.Rank.Name,
                    Id = y.Id.ToString(),
                    Name = y.Template.CardName,
                    ImageUrl = y.Template.ImageUrl
                }).ToList());


            // Map the result models to Views (not Domain objects directly)
            CreateMap<CardTemplateQueryResult, GetCardTemplateView>()
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.Card.Rank.Name))
                .ForMember(dest => dest.Suit, opt => opt.MapFrom(src => src.Card.Suit.Name))
                .ForMember(dest => dest.CardName, opt => opt.MapFrom(src => src.Card.CardName))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Card.ImageUrl));

            CreateMap<DeckOfCardsQueryResult, GetDeckByIdView>()
                .ForMember(dest => dest.DeckId, opt => opt.MapFrom(src => src.Deck.Id))
                .ForMember(dest => dest.Cards, opt => opt.MapFrom(src => src.Deck)); // resuse above mapping

            CreateMap<NewDeckOfCardsCommandResult, NewDeckView>()
                .ForMember(dest => dest.Cards, opt => opt.MapFrom(src => src.Deck));




            CreateMap<PlayingCard, NewDeckView.CardViewDto>();
        }
    }
}
