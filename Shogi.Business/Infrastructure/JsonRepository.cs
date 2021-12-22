using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Shogi.Business.Infrastructure
{
    public class JsonRepository
    {
        private static readonly DataContractJsonSerializerSettings SerializeSettings = 
            new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true
            };

        public T Load<T>(string path)
        {
            T obj;
            string json = File.ReadAllText(path, Encoding.UTF8);
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {

                System.Xml.XmlDictionaryReader xmlReader = null;

                try {
                    using (xmlReader = JsonReaderWriterFactory.CreateJsonReader(ms, System.Xml.XmlDictionaryReaderQuotas.Max))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(T), SerializeSettings);
                        obj = (T)serializer.ReadObject(xmlReader);
                    }

                } finally {
                    if (xmlReader != null) {
                        GC.SuppressFinalize(xmlReader);
                        xmlReader = null;
                    }
                }
            }
            return obj;
        }

        public void Save<T>(string path, T obj)
        {
            using (var stream = File.Create(path))
            using (var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, true, true, "  "))
            {
                var serializer = new DataContractJsonSerializer(typeof(T), SerializeSettings);
                serializer.WriteObject(writer, obj);
                writer.Flush();
            }

        }
    }
}
