using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DMT.Test.Utils
{
    public static class SerializerHelper
    {
        public static XDocument SerializeObject(object obj)
        {
            using (var ms = new MemoryStream())
            {
                return XDocument.Load(SerializeObjectIntoStream(ms, obj));
            }
        }

        public static T DeserializeObject<T>(XDocument doc)
        {
            var obj = new XmlSerializer(typeof(T)).Deserialize(doc.CreateReader());
            return (T)obj;
        }

        private static Stream SerializeObjectIntoStream(Stream stream, object obj)
        {
            using (XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false }))
            {
                writer.WriteStartDocument();
                new XmlSerializer(obj.GetType()).Serialize(writer, obj);
                writer.WriteEndDocument();
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
