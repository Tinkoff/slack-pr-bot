using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace SlackPrBot.JsonConverters
{
    internal abstract class JsonConverterBase<T> : CustomCreationConverter<T>
    {
        public override T Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        public abstract T Create(Type objectType, JObject jObject);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            try
            {
                // Load JObject from stream
                var jObject = JObject.Load(reader);

                // Create target object based on JObject
                var target = Create(objectType, jObject);

                // Populate the object properties
                serializer.Populate(jObject.CreateReader(), target);

                return target;
            }
            catch (Exception)
            {
                return existingValue;
            }
        }
    }
}
