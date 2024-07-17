using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaryway.DynamicSpace.DynamicWebApi.Attributes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityIdAttribute : Attribute
    {
        public long EntityId { get; }

        public EntityIdAttribute(long entityId)
        {
            EntityId = entityId;
        }
    }
}
