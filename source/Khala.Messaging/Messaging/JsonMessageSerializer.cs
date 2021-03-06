﻿namespace Khala.Messaging
{
    using System;
    using System.IO;
    using Newtonsoft.Json;

    public sealed class JsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializer _serializer;

        public JsonMessageSerializer()
            : this(new JsonSerializerSettings())
        {
        }

        public JsonMessageSerializer(JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            settings.TypeNameHandling = TypeNameHandling.Objects;
#if DEBUG
            settings.Formatting = Formatting.Indented;
#else
            settings.Formatting = Formatting.None;
#endif

            _serializer = JsonSerializer.Create(settings);
        }

        public string Serialize(object message)
        {
            using (var writer = new StringWriter())
            {
                _serializer.Serialize(writer, message);
                return writer.ToString();
            }
        }

        public object Deserialize(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            StringReader reader = null;
            try
            {
                reader = new StringReader(value);
                using (var jsonReader = new JsonTextReader(reader))
                {
                    reader = null;
                    try
                    {
                        return _serializer.Deserialize(jsonReader);
                    }
                    catch (JsonSerializationException)
                    {
                        return JsonConvert.DeserializeObject(value);
                    }
                }
            }
            finally
            {
                reader?.Dispose();
            }
        }
    }
}
