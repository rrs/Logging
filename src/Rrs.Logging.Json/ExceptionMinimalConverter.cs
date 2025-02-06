using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Rrs.Logging.Json
{
    public class ExceptionMinimalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(Exception).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            var contract = serializer.ContractResolver.ResolveContract(value.GetType()) as JsonObjectContract;
            if (contract == null)
            {
                throw new JsonSerializationException($"Could not resolve contract for type {value.GetType()}");
            }

            foreach (var property in contract.Properties)
            {
                if (property.PropertyName == "WatsonBuckets") continue;
                if (!property.Ignored && property.Readable)
                {
                    var propertyValue = property.ValueProvider.GetValue(value);
                    writer.WritePropertyName(property.PropertyName);
                    serializer.Serialize(writer, propertyValue);
                }
            }

            writer.WriteEndObject();
        }
    }
}
