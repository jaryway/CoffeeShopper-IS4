using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicBuilder.Metadata
{
    public class MetadataHolder
    {
        public List<MetadataEntity> Entities { get; set; } = new List<MetadataEntity>();

        public string Version { get; set; }
    }
}
