using ApiKickstart.Queries;
using AutoMapper;

namespace ApiKickstart.QueryHandlers
{
    /// <summary>
    /// This Automapper profile will be scanned by the IoC container and the maps will be auto-registered.
    /// http://docs.automapper.org/en/stable/Projection.html
    /// </summary>
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            //CreateMap<DataModel.Entities.Widget, Domain.Widget>();
                //.ReverseMap();
        }
    }
}
