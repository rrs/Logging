using System;

namespace Rrs.Logging.Yaml
{
    public class YamlLogObjectSerializer : ILogObjectSerializer
    {
        public string Serialize(object o) => YamlConvert.SerializeObject(o);
    }
}
