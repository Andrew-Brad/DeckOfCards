using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiKickstart.WebApi.Mappings
{
    /// <summary>
    /// This Automapper profile will be scanned by the IoC container and the maps will be auto-registered.
    /// http://docs.automapper.org/en/stable/Projection.html
    /// </summary>
    public class Mappings : Profile
    {
        public Mappings()
        {
            // Domain DbSet Entities -> Outbound Domain classes/ViewModels
            //CreateMap<DataModel.Entities.Widget, Domain.Widget>()
                //.ForMember(dest => dest.WidgetsClassification, opt => opt.MapFrom(src => src.Classification.Name));
            //CreateMap<Domain.Widget, DataModel.Entities.Widget>();//needed?
        }
    }
}
