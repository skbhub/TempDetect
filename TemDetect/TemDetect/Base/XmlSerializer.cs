﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Base
{
    
        public class xml_serializer_manager
        {
            /// <summary>
            /// serialize object to xml file.
            /// </summary>
            /// <param name="path">the path to save the xml file</param>
            /// <param name="obj">the object you want to serialize</param>
            public void serialize_to_xml(string path, object obj)
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                string content = string.Empty;
                //serialize
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, obj);
                    content = writer.ToString();
                }
                //save to file
                using (StreamWriter stream_writer = new StreamWriter(path))
                {
                    stream_writer.Write(content);
                }
            }

            /// <summary>
            /// deserialize xml file to object
            /// </summary>
            /// <param name="path">the path of the xml file</param>
            /// <param name="object_type">the object type you want to deserialize</param>
            public object deserialize_from_xml(string path, Type object_type)
            {
                XmlSerializer serializer = new XmlSerializer(object_type);
                using (StreamReader reader = new StreamReader(path))
                {
                    return serializer.Deserialize(reader);
                }
            }
        }
    
}
