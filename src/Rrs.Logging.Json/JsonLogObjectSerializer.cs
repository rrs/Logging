using Newtonsoft.Json;

namespace Rrs.Logging.Json
{
    public class JsonLogObjectSerializer : ILogObjectSerializer
    {
        public string Serialize(object o) => JsonConvert.SerializeObject(o);
    }
}
