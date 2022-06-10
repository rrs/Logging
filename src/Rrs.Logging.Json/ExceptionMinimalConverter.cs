using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

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
            var serializationInfo = new SerializationInfo(value.GetType(), new FormatterConverter());
            ((ISerializable)value).GetObjectData(serializationInfo, serializer.Context);

            writer.WriteStartObject();

            foreach (SerializationEntry serializationEntry in serializationInfo)
            {
                if (serializationEntry.Name == "WatsonBuckets") continue;
                writer.WritePropertyName(serializationEntry.Name);
                serializer.Serialize(writer, serializationEntry.Value);
            }

            writer.WriteEndObject();
        }
    }
}
