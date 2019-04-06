using DeckOfCards.Commands;
using DeckOfCards.Domain;
using DeckOfCards.Queries;
using AutoMapper;
using Sieve.Models;

namespace DeckOfCards.WebApi.ViewModels
{
    /// <summary>
    /// This Automapper profile will be scanned by the IoC container and the maps will be auto-registered.
    /// http://docs.automapper.org/en/stable/Projection.html
    /// </summary>
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            //CreateMap<PagedRequestViewModel, IPagedQuery>() // member mappings explicit because of an externally defined class
            //    .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src.Page))
            //    .ForMember(dest => dest.ResultsPerPage, opt => opt.MapFrom(src => src.ResultsPerPage));

            CreateMap<GetCardTemplateViewModel, CardTemplateQuery>()
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => RanksEnumeration.FromName(src.Rank, true)))
                .ForMember(dest => dest.Suit, opt => opt.MapFrom(src => SuitsEnumeration.FromName(src.Suit, true)));


            CreateMap<GetWidgetsViewModelV2, CardTemplateQuery>();

            //CreateMap<SieveModel, WidgetsQuery>() // might need to extend the models with an interface to avoid excessive mapping expressions
                //.ForMember(dest => dest.SortFilterPaging, opt => opt.MapFrom(src => src));                

            CreateMap<CreateWidgetViewModel, CreateWidgetCommand>()
                .ForMember(dest => dest.Classification, opt => opt.MapFrom(src => SuitsEnumeration.FromValue(ushort.Parse(src.Classification))));
            CreateMap<DeprecateWidgetViewModel, DeprecateWidgetCommand>();
        }
    }
}
