using AutoMapper;
using Jaryway.DynamicSpace.DynamicWebApi.Entities;

namespace Jaryway.DynamicSpace.DynamicWebApi.Mappers
{
    /// <summary>
    /// 
    /// </summary>
    public static class DynamicClassMappers
    {

        static DynamicClassMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<DynamicClassMapperProfile>())
                .CreateMapper();
        }
        /// <summary>
        /// 
        /// </summary>
        public static IMapper Mapper { get; }


        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Models.DynamicClassModel? ToModel(this DynamicClass entity)
        {
            return entity == null ? null : Mapper.Map<Models.DynamicClassModel>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static DynamicClass? ToEntity(this Models.DynamicClassModel model)
        {
            return model == null ? null : Mapper.Map<DynamicClass>(model);
        }
    }


}
