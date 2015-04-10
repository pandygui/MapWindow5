﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace MW5.Shared
{
    public static class DataContractSerializationHelper
    {
        private const int MaxStringContentLength = 1048576;

        public static T Deserialize<T>(this string targetString)
        {
            return Deserialize<T>(targetString, null);
        }


        public static T Deserialize<T>(this string targetString, IEnumerable<Type> knownTypes)
        {
            Encoding encoding = XmlSerializationHelper.GetXmlDocEncoding(targetString);

            using (var stream = new MemoryStream(encoding.GetBytes(targetString)))
            {
                var quota = new XmlDictionaryReaderQuotas() { MaxStringContentLength = MaxStringContentLength };
                using (var reader = XmlDictionaryReader.CreateTextReader(stream, quota))
                {
                    var ser = new DataContractSerializer(typeof(T), knownTypes, Int32.MaxValue, false, false, null);
                    return (T)ser.ReadObject(reader);
                }
            }
        }

        public static string Serialize<T>(this T target)
        {
            return Serialize(target, null, true);
        }

        public static string Serialize<T>(this T target, bool preserveObjectReferences)
        {
            return Serialize(target, null, preserveObjectReferences);
        }

        public static string Serialize<T>(this T target, IEnumerable<Type> knownTypes, bool preserveObjectReferences)
        {
            using (var writer = new StringWriter())
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(writer))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    var ser = new DataContractSerializer(typeof(T), knownTypes, Int32.MaxValue, false, preserveObjectReferences, null);
                    ser.WriteObject(xmlWriter, target);
                    return writer.ToString();
                }
            }
        }
    }
}