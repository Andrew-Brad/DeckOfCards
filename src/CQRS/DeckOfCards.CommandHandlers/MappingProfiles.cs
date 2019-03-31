using DeckOfCards.CommandResults;
using DeckOfCards.Commands;
using DeckOfCards.Domain;
using DeckOfCards.DomainEvents;
using AutoMapper;

namespace DeckOfCards.CommandHandlers
{
    /// <summary>
    /// This Automapper profile will be scanned by the IoC container and the maps will be auto-registered.
    /// http://docs.automapper.org/en/stable/Projection.html
    /// </summary>
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<DeprecateWidgetCommandResult, WidgetDeprecatedV1Event>();
            //.ForMember(dest => dest.Classification, opt => opt.MapFrom(src => WidgetClassification.FromValue(ushort.Parse(src.Classification))));
            CreateMap<CreateWidgetCommand, CardTemplate>();
                //.ForMember(dest => dest.CreationDate, opt => opt.Ignore());
            CreateMap<CreateWidgetCommandResult, CardTemplate>();
                //.ForMember(dest => dest.CreationDate, opt => opt.Ignore());

        }
    }
}
