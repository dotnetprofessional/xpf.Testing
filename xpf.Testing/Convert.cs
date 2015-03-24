using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

using Newtonsoft.Json;

namespace xpf.Testing
{
    public static class Convert
    {
        public static string ToXml<T>(this T instance, params Type[] extraTypes)
        {
            string xml = "";
            using (var ms = new MemoryStream())
            {
                var xser = new XmlSerializer(typeof (T), extraTypes);
                xser.Serialize(ms, instance);

                ms.Position = 0;
                using (var s = new StreamReader(ms))
                {
                    xml = s.ReadToEnd();
                }
            }
            return xml;
        }

        public static T FromXmlToInstance<T>(this string xml, params Type[] extraTypes)
            where T : class
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(xml)).FromXmlStreamToInstance<T>();
        }

        //var assembly = resourceName.GetType().GetTypeInfo().Assembly.;
        public static T FromXmlResourceToInstance<T>(this string resourceName, object caller, params Type[] extraTypes)
            where T : class
        {
            return resourceName.FromXmlResourceToInstance<T>(caller.GetType().GetTypeInfo().Assembly, extraTypes);
        }

        public static T FromXmlResourceToInstance<T>(this string resourceName, Type caller, params Type[] extraTypes)
            where T : class
        {
            return resourceName.FromXmlResourceToInstance<T>(caller.GetTypeInfo().Assembly, extraTypes);
        }

        public static T FromXmlResourceToInstance<T>(this string resourceName, Assembly assembly, params Type[] extraTypes)
            where T : class
        {
            return EmbeddedResources.GetResourceStream(resourceName, assembly).FromXmlStreamToInstance<T>();
        }

        public static T FromXmlStreamToInstance<T>(this Stream stream, params Type[] extraTypes)
            where T : class
        {
            T entity;
            using (stream)
            {
                var xser = new XmlSerializer(typeof (T), extraTypes);
                entity = xser.Deserialize(stream) as T;
            }
            return entity;
        }

        public static T FromJsonToInstance<T>(this string json)
            where T : class
        {
            // To support serializing and deserializing object types (ie not type descriptor) 
            // We add the type handling support
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }

        public static T FromJsonResourceToInstance<T>(this string resourceName, Assembly assembly)
            where T : class
        {
            return EmbeddedResources.GetResourceString(resourceName, assembly).FromJsonToInstance<T>();
        }

        public static T FromJsonResourceToInstance<T>(this string resourceName, object caller)
            where T : class
        {
            return EmbeddedResources.GetResourceString(resourceName, caller.GetType().GetTypeInfo().Assembly).FromJsonToInstance<T>();
        }

        public static T FromJsonResourceToInstance<T>(this string resourceName, Type caller)
            where T : class
        {
            return EmbeddedResources.GetResourceString(resourceName, caller.GetTypeInfo().Assembly).FromJsonToInstance<T>();
        }

        public static string ToJson<T>(this T instance)
        {
            // To support serializing and deserializing object types (ie not type descriptor) 
            // We add the type handling support
            return JsonConvert.SerializeObject(instance, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }

        public static string FromResourceToString(this string resourceName, object caller)
        {
            return EmbeddedResources.GetResourceString(resourceName, caller.GetType().GetTypeInfo().Assembly);
        }

        public static string FromResourceToString(this string resourceName, Type caller)
        {
            return EmbeddedResources.GetResourceString(resourceName, caller.GetTypeInfo().Assembly);
        }

        public static Stream FromResourceToStream(this string resourceName, object caller)
        {
            return EmbeddedResources.GetResourceStream(resourceName, caller.GetType().GetTypeInfo().Assembly);
        }

        public static Stream FromResourceToStream(this string resourceName, Type caller)
        {
            return EmbeddedResources.GetResourceStream(resourceName, caller.GetTypeInfo().Assembly);
        }
    }
}
