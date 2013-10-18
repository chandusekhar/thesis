using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DMT.Core.Interfaces.Serialization;

namespace DMT.Core.Test.Utils
{
    public static class SerializerHelper
    {
        public const string RootWrapperTag = "root";

        public static XDocument SerializeObject(ISerializable obj)
        {
            using (var ms = new MemoryStream())
            {
                return XDocument.Load(SerializeObjectIntoStream(ms, obj));
            }
        }

        public static T SerializeAndDeserialize<T>(T obj, IContext context = null) where T : ISerializable
        {
            return DeserializeObject<T>(SerializeObject(obj), context);
        }

        public static T DeserializeObject<T>(XDocument doc, IContext context = null) where T : ISerializable
        {
            T obj = Activator.CreateInstance<T>();
            using (XmlReader reader = doc.CreateReader())
            {
                reader.ReadToFollowing(SerializerHelper.RootWrapperTag);
                obj.Deserialize(reader, context);
            }
            return (T)obj;
        }

        private static Stream SerializeObjectIntoStream(Stream stream, ISerializable obj)
        {
            using (XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(SerializerHelper.RootWrapperTag);

                obj.Serialize(writer);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
