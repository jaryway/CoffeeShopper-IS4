
using AutoMapper;

namespace Jaryway.DynamicSpace.DynamicWebApi.Mappers
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicClassMapperProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public DynamicClassMapperProfile()
        {
            // TSource, TDestination
            CreateMap<Entities.DynamicClass, Models.DynamicClassModel>()
                 .ForMember(x => x.EntityProperties, opts => opts.MapFrom(x => x.EntityProperties_))
                 .ForMember(x => x.EntityPropertiesHasChanged, opts => opts.MapFrom(x => x.EntityPropertiesHasChanged))
                 .ReverseMap()
                 .ForMember(x => x.EntityProperties_, opts => opts.MapFrom(x => x.EntityProperties));
        }
    }
}
