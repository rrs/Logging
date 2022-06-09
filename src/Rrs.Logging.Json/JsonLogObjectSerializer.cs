using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rrs.Logging.Json
{
    public class JsonLogObjectSerializer : ILogObjectSerializer
    {
        private readonly bool _removeWatsonBuckets;
        public JsonLogObjectSerializer(bool removeWatsonBuckets = true) => _removeWatsonBuckets = removeWatsonBuckets;

        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new ExceptionMinimalConverter()
            },
        };

        public string Serialize(object o) => JsonConvert.SerializeObject(o, _removeWatsonBuckets ? _settings : null);
    }
}
