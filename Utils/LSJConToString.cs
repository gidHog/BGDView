using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace LSLib.LS
{
    //Copy from : https://github.com/Norbyte/lslib
    class LSJConToString : JsonConverter
    {
        private LSMetadata Metadata;

        private NodeSerializationSettings SerializationSettings;

        public LSJConToString(NodeSerializationSettings settings)
        {
            SerializationSettings = settings;
        }

        public override bool CanConvert(Type objectType)
        {
            if (!(objectType == typeof(Node)))
            {
                return objectType == typeof(Resource);
            }

            return true;
        }

        private TranslatedFSStringArgument ReadFSStringArgument(JsonReader reader)
        {
            TranslatedFSStringArgument translatedFSStringArgument = new TranslatedFSStringArgument();
            string text = null;
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    text = reader.Value!.ToString();
                }
                else if (reader.TokenType == JsonToken.String)
                {
                    if (text == "key")
                    {
                        translatedFSStringArgument.Key = reader.Value!.ToString();
                        continue;
                    }

                    if (!(text == "value"))
                    {
                        throw new InvalidDataException("Unknown property encountered during TranslatedFSString argument parsing: " + text);
                    }

                    translatedFSStringArgument.Value = reader.Value!.ToString();
                }
                else
                {
                    if (reader.TokenType != JsonToken.StartObject || !(text == "string"))
                    {
                        throw new InvalidDataException("Unexpected JSON token during parsing of TranslatedFSString argument: " + reader.TokenType);
                    }

                    translatedFSStringArgument.String = ReadTranslatedFSString(reader);
                }
            }

            return translatedFSStringArgument;
        }

        private TranslatedFSString ReadTranslatedFSString(JsonReader reader)
        {
            TranslatedFSString translatedFSString = new TranslatedFSString();
            string text = "";
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    text = reader.Value!.ToString();
                    continue;
                }

                if (reader.TokenType == JsonToken.String)
                {
                    if (text == "value")
                    {
                        if (reader.Value != null)
                        {
                            translatedFSString.Value = reader.Value!.ToString();
                        }
                        else
                        {
                            translatedFSString.Value = null;
                        }

                        continue;
                    }

                    if (text == "handle")
                    {
                        translatedFSString.Handle = reader.Value!.ToString();
                        continue;
                    }

                    throw new InvalidDataException("Unknown TranslatedFSString property: " + text);
                }

                if (reader.TokenType == JsonToken.StartArray && text == "arguments")
                {
                    translatedFSString.Arguments = ReadFSStringArguments(reader);
                    continue;
                }

                if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }

                throw new InvalidDataException("Unexpected JSON token during parsing of TranslatedFSString: " + reader.TokenType);
            }

            return translatedFSString;
        }

        private List<TranslatedFSStringArgument> ReadFSStringArguments(JsonReader reader)
        {
            List<TranslatedFSStringArgument> list = new List<TranslatedFSStringArgument>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    list.Add(ReadFSStringArgument(reader));
                    continue;
                }

                if (reader.TokenType == JsonToken.EndArray)
                {
                    break;
                }

                throw new InvalidDataException("Unexpected JSON token during parsing of TranslatedFSString argument list: " + reader.TokenType);
            }

            return list;
        }

        private NodeAttribute ReadAttribute(JsonReader reader)
        {
            string text = "";
            string handle = null;
            List<TranslatedFSStringArgument> arguments = null;
            NodeAttribute nodeAttribute = null;
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    text = reader.Value!.ToString();
                }
                else if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Boolean || reader.TokenType == JsonToken.Null)
                {
                    switch (text)
                    {
                        case "type":
                            {
                                if (!uint.TryParse((string)reader.Value, out var result))
                                {
                                    result = (uint)AttributeTypeMaps.TypeToId[(string)reader.Value];
                                }

                                nodeAttribute = new NodeAttribute((NodeAttribute.DataType)result);
                                switch (result)
                                {
                                    case 28u:
                                        nodeAttribute.Value = new TranslatedString
                                        {
                                            Handle = handle
                                        };
                                        break;
                                    case 33u:
                                        nodeAttribute.Value = new TranslatedFSString
                                        {
                                            Handle = handle,
                                            Arguments = arguments
                                        };
                                        break;
                                }

                                break;
                            }
                        case "value":
                            switch (nodeAttribute.Type)
                            {
                                case NodeAttribute.DataType.DT_Byte:
                                    nodeAttribute.Value = Convert.ToByte(reader.Value);
                                    break;
                                case NodeAttribute.DataType.DT_Short:
                                    nodeAttribute.Value = Convert.ToInt16(reader.Value);
                                    break;
                                case NodeAttribute.DataType.DT_UShort:
                                    nodeAttribute.Value = Convert.ToUInt16(reader.Value);
                                    break;
                                case NodeAttribute.DataType.DT_Int:
                                    nodeAttribute.Value = Convert.ToInt32(reader.Value);
                                    break;
                                case NodeAttribute.DataType.DT_UInt:
                                    nodeAttribute.Value = Convert.ToUInt32(reader.Value);
                                    break;
                                case NodeAttribute.DataType.DT_Float:
                                    nodeAttribute.Value = Convert.ToSingle(reader.Value);
                                    break;
                                case NodeAttribute.DataType.DT_Double:
                                    nodeAttribute.Value = Convert.ToDouble(reader.Value);
                                    break;
                                case NodeAttribute.DataType.DT_Bool:
                                    nodeAttribute.Value = Convert.ToBoolean(reader.Value);
                                    break;
                                case NodeAttribute.DataType.DT_String:
                                case NodeAttribute.DataType.DT_Path:
                                case NodeAttribute.DataType.DT_FixedString:
                                case NodeAttribute.DataType.DT_LSString:
                                case NodeAttribute.DataType.DT_WString:
                                case NodeAttribute.DataType.DT_LSWString:
                                    nodeAttribute.Value = reader.Value!.ToString();
                                    break;
                                case NodeAttribute.DataType.DT_ULongLong:
                                    if (reader.Value!.GetType() == typeof(long))
                                    {
                                        nodeAttribute.Value = Convert.ToUInt64((long)reader.Value);
                                    }
                                    else if (reader.Value!.GetType() == typeof(BigInteger))
                                    {
                                        nodeAttribute.Value = (ulong)(BigInteger)reader.Value;
                                    }
                                    else
                                    {
                                        nodeAttribute.Value = (ulong)reader.Value;
                                    }

                                    break;
                                case NodeAttribute.DataType.DT_ScratchBuffer:
                                    nodeAttribute.Value = Convert.FromBase64String(reader.Value!.ToString());
                                    break;
                                case NodeAttribute.DataType.DT_Long:
                                case NodeAttribute.DataType.DT_Int64:
                                    nodeAttribute.Value = Convert.ToInt64(reader.Value);
                                    break;
                                case NodeAttribute.DataType.DT_Int8:
                                    nodeAttribute.Value = Convert.ToSByte(reader.Value);
                                    break;
                                case NodeAttribute.DataType.DT_TranslatedString:
                                    {
                                        if (nodeAttribute.Value == null)
                                        {
                                            nodeAttribute.Value = new TranslatedString();
                                        }

                                        TranslatedString obj = (TranslatedString)nodeAttribute.Value;
                                        obj.Value = reader.Value!.ToString();
                                        obj.Handle = handle;
                                        break;
                                    }
                                case NodeAttribute.DataType.DT_TranslatedFSString:
                                    {
                                        if (nodeAttribute.Value == null)
                                        {
                                            nodeAttribute.Value = new TranslatedFSString();
                                        }

                                        TranslatedFSString translatedFSString = (TranslatedFSString)nodeAttribute.Value;
                                        translatedFSString.Value = ((reader.Value != null) ? reader.Value!.ToString() : null);
                                        translatedFSString.Handle = handle;
                                        translatedFSString.Arguments = arguments;
                                        nodeAttribute.Value = translatedFSString;
                                        break;
                                    }
                                case NodeAttribute.DataType.DT_UUID:
                                    if (SerializationSettings.ByteSwapGuids)
                                    {
                                        nodeAttribute.Value = NodeAttribute.ByteSwapGuid(new Guid(reader.Value!.ToString()));
                                    }
                                    else
                                    {
                                        nodeAttribute.Value = new Guid(reader.Value!.ToString());
                                    }

                                    break;
                                case NodeAttribute.DataType.DT_IVec2:
                                case NodeAttribute.DataType.DT_IVec3:
                                case NodeAttribute.DataType.DT_IVec4:
                                    {
                                        string[] array3 = reader.Value!.ToString()!.Split(new char[1] { ' ' });
                                        int columns2 = nodeAttribute.GetColumns();
                                        if (columns2 != array3.Length)
                                        {
                                            throw new FormatException($"A vector of length {columns2} was expected, got {array3.Length}");
                                        }

                                        int[] array4 = new int[columns2];
                                        for (int j = 0; j < columns2; j++)
                                        {
                                            array4[j] = int.Parse(array3[j]);
                                        }

                                        nodeAttribute.Value = array4;
                                        break;
                                    }
                                case NodeAttribute.DataType.DT_Vec2:
                                case NodeAttribute.DataType.DT_Vec3:
                                case NodeAttribute.DataType.DT_Vec4:
                                    {
                                        string[] array = reader.Value!.ToString()!.Split(new char[1] { ' ' });
                                        int columns = nodeAttribute.GetColumns();
                                        if (columns != array.Length)
                                        {
                                            throw new FormatException($"A vector of length {columns} was expected, got {array.Length}");
                                        }

                                        float[] array2 = new float[columns];
                                        for (int i = 0; i < columns; i++)
                                        {
                                            array2[i] = float.Parse(array[i]);
                                        }

                                        nodeAttribute.Value = array2;
                                        break;
                                    }
                                case NodeAttribute.DataType.DT_Mat2:
                                case NodeAttribute.DataType.DT_Mat3:
                                case NodeAttribute.DataType.DT_Mat3x4:
                                case NodeAttribute.DataType.DT_Mat4x3:
                                case NodeAttribute.DataType.DT_Mat4:
                                    {
                                        Matrix matrix = Matrix.Parse(reader.Value!.ToString());
                                        if (matrix.cols != nodeAttribute.GetColumns() || matrix.rows != nodeAttribute.GetRows())
                                        {
                                            throw new FormatException("Invalid column/row count for matrix");
                                        }

                                        nodeAttribute.Value = matrix;
                                        break;
                                    }
                                default:
                                    throw new NotImplementedException("Don't know how to unserialize type " + nodeAttribute.Type);
                            }

                            break;
                        case "handle":
                            if (nodeAttribute != null)
                            {
                                if (nodeAttribute.Type == NodeAttribute.DataType.DT_TranslatedString)
                                {
                                    if (nodeAttribute.Value == null)
                                    {
                                        nodeAttribute.Value = new TranslatedString();
                                    }

                                    ((TranslatedString)nodeAttribute.Value).Handle = reader.Value!.ToString();
                                }
                                else if (nodeAttribute.Type == NodeAttribute.DataType.DT_TranslatedFSString)
                                {
                                    if (nodeAttribute.Value == null)
                                    {
                                        nodeAttribute.Value = new TranslatedFSString();
                                    }

                                    ((TranslatedFSString)nodeAttribute.Value).Handle = reader.Value!.ToString();
                                }
                            }
                            else
                            {
                                handle = reader.Value!.ToString();
                            }

                            break;
                        case "version":
                            if (nodeAttribute.Value == null)
                            {
                                nodeAttribute.Value = new TranslatedString();
                            }

                            ((TranslatedString)nodeAttribute.Value).Version = ushort.Parse(reader.Value!.ToString());
                            break;
                        default:
                            throw new InvalidDataException("Unknown property encountered during attribute parsing: " + text);
                    }
                }
                else
                {
                    if (reader.TokenType != JsonToken.StartArray || !(text == "arguments"))
                    {
                        throw new InvalidDataException("Unexpected JSON token during parsing of attribute: " + reader.TokenType);
                    }

                    List<TranslatedFSStringArgument> list = ReadFSStringArguments(reader);
                    if (nodeAttribute.Value != null)
                    {
                        ((TranslatedFSString)nodeAttribute.Value).Arguments = list;
                    }
                    else
                    {
                        arguments = list;
                    }
                }
            }

            return nodeAttribute;
        }

        private Node ReadNode(JsonReader reader, Node node)
        {
            string text = "";
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    text = reader.Value!.ToString();
                    continue;
                }

                if (reader.TokenType == JsonToken.StartObject)
                {
                    NodeAttribute value = ReadAttribute(reader);
                    node.Attributes.Add(text, value);
                    continue;
                }

                if (reader.TokenType == JsonToken.StartArray)
                {
                    while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                    {
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            Node node2 = new Node();
                            node2.Name = text;
                            ReadNode(reader, node2);
                            node.AppendChild(node2);
                            node2.Parent = node;
                            continue;
                        }

                        throw new InvalidDataException("Unexpected JSON token during parsing of child node list: " + reader.TokenType);
                    }

                    continue;
                }

                throw new InvalidDataException("Unexpected JSON token during parsing of node: " + reader.TokenType);
            }

            return node;
        }

        private Resource ReadResource(JsonReader reader, Resource resource)
        {
            if (resource == null)
            {
                resource = new Resource();
            }

            if (!reader.Read() || reader.TokenType != JsonToken.PropertyName || !reader.Value!.Equals("save"))
            {
                throw new InvalidDataException("Expected JSON property 'save'");
            }

            if (!reader.Read() || reader.TokenType != JsonToken.StartObject)
            {
                throw new InvalidDataException("Expected JSON object start token for 'save': " + reader.TokenType);
            }

            if (!reader.Read() || reader.TokenType != JsonToken.PropertyName || !reader.Value!.Equals("header"))
            {
                throw new InvalidDataException("Expected JSON property 'header'");
            }

            if (!reader.Read() || reader.TokenType != JsonToken.StartObject)
            {
                throw new InvalidDataException("Expected JSON object start token for 'header': " + reader.TokenType);
            }

            string text = "";
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    text = reader.Value!.ToString();
                    continue;
                }

                if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Integer)
                {
                    if (text == "time")
                    {
                        resource.Metadata.Timestamp = Convert.ToUInt32(reader.Value);
                        continue;
                    }

                    if (text == "version")
                    {
                        Match match = new Regex("^([0-9]+)\\.([0-9]+)\\.([0-9]+)\\.([0-9]+)$").Match(reader.Value!.ToString());
                        if (match.Success)
                        {
                            resource.Metadata.MajorVersion = Convert.ToUInt32(match.Groups[1].Value);
                            resource.Metadata.MinorVersion = Convert.ToUInt32(match.Groups[2].Value);
                            resource.Metadata.Revision = Convert.ToUInt32(match.Groups[3].Value);
                            resource.Metadata.BuildNumber = Convert.ToUInt32(match.Groups[4].Value);
                            continue;
                        }

                        throw new InvalidDataException("Malformed version string: " + reader.Value!.ToString());
                    }

                    throw new InvalidDataException("Unknown property encountered during header parsing: " + text);
                }

                throw new InvalidDataException("Unexpected JSON token during parsing of header: " + reader.TokenType);
            }

            if (!reader.Read() || reader.TokenType != JsonToken.PropertyName || !reader.Value!.Equals("regions"))
            {
                throw new InvalidDataException("Expected JSON property 'regions'");
            }

            if (!reader.Read() || reader.TokenType != JsonToken.StartObject)
            {
                throw new InvalidDataException("Expected JSON object start token for 'regions': " + reader.TokenType);
            }

            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    text = reader.Value!.ToString();
                    continue;
                }

                if (reader.TokenType == JsonToken.StartObject)
                {
                    Region region = new Region();
                    ReadNode(reader, region);
                    region.Name = text;
                    region.RegionName = text;
                    resource.Regions.Add(text, region);
                    continue;
                }

                throw new InvalidDataException("Unexpected JSON token during parsing of region list: " + reader.TokenType);
            }

            return resource;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(Node))
            {
                return ReadNode(reader, existingValue as Node);
            }

            if (objectType == typeof(Resource))
            {
                return ReadResource(reader, existingValue as Resource);
            }

            throw new InvalidOperationException("Cannot unserialize unknown type");
        }

        private void WriteResource(JsonWriter writer, Resource resource, JsonSerializer serializer)
        {
            Metadata = resource.Metadata;
            writer.WriteStartObject();
            writer.WritePropertyName("save");
            writer.WriteStartObject();
            writer.WritePropertyName("header");
            writer.WriteStartObject();
            writer.WritePropertyName("time");
            writer.WriteValue(resource.Metadata.Timestamp);
            writer.WritePropertyName("version");
            string value = resource.Metadata.MajorVersion + "." + resource.Metadata.MinorVersion + "." + resource.Metadata.Revision + "." + resource.Metadata.BuildNumber;
            writer.WriteValue(value);
            writer.WriteEndObject();
            writer.WritePropertyName("regions");
            writer.WriteStartObject();
            foreach (KeyValuePair<string, Region> region in resource.Regions)
            {
                writer.WritePropertyName(region.Key);
                WriteNode(writer, region.Value, serializer);
            }

            writer.WriteEndObject();
            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        private void WriteTranslatedFSString(JsonWriter writer, TranslatedFSString fs)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("value");
            WriteTranslatedFSStringInner(writer, fs);
            writer.WriteEndObject();
        }

        private void WriteTranslatedFSStringInner(JsonWriter writer, TranslatedFSString fs)
        {
            writer.WriteValue(fs.Value);
            writer.WritePropertyName("handle");
            writer.WriteValue(fs.Handle);
            writer.WritePropertyName("arguments");
            writer.WriteStartArray();
            for (int i = 0; i < fs.Arguments.Count; i++)
            {
                TranslatedFSStringArgument translatedFSStringArgument = fs.Arguments[i];
                writer.WriteStartObject();
                writer.WritePropertyName("key");
                writer.WriteValue(translatedFSStringArgument.Key);
                writer.WritePropertyName("string");
                WriteTranslatedFSString(writer, translatedFSStringArgument.String);
                writer.WritePropertyName("value");
                writer.WriteValue(translatedFSStringArgument.Value);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        private void WriteNode(JsonWriter writer, Node node, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            foreach (KeyValuePair<string, NodeAttribute> attribute in node.Attributes)
            {
                writer.WritePropertyName(attribute.Key);
                writer.WriteStartObject();
                writer.WritePropertyName("type");
                if (Metadata.MajorVersion >= 4)
                {
                    writer.WriteValue(AttributeTypeMaps.IdToType[attribute.Value.Type]);
                }
                else
                {
                    writer.WriteValue((int)attribute.Value.Type);
                }

                if (attribute.Value.Type != NodeAttribute.DataType.DT_TranslatedString)
                {
                    writer.WritePropertyName("value");
                }

                switch (attribute.Value.Type)
                {
                    case NodeAttribute.DataType.DT_Byte:
                        writer.WriteValue(Convert.ToByte(attribute.Value.Value));
                        break;
                    case NodeAttribute.DataType.DT_Short:
                        writer.WriteValue(Convert.ToInt16(attribute.Value.Value));
                        break;
                    case NodeAttribute.DataType.DT_UShort:
                        writer.WriteValue(Convert.ToUInt16(attribute.Value.Value));
                        break;
                    case NodeAttribute.DataType.DT_Int:
                        writer.WriteValue(Convert.ToInt32(attribute.Value.Value));
                        break;
                    case NodeAttribute.DataType.DT_UInt:
                        writer.WriteValue(Convert.ToUInt32(attribute.Value.Value));
                        break;
                    case NodeAttribute.DataType.DT_Float:
                        writer.WriteValue(Convert.ToSingle(attribute.Value.Value));
                        break;
                    case NodeAttribute.DataType.DT_Double:
                        writer.WriteValue(Convert.ToDouble(attribute.Value.Value));
                        break;
                    case NodeAttribute.DataType.DT_Bool:
                        writer.WriteValue(Convert.ToBoolean(attribute.Value.Value));
                        break;
                    case NodeAttribute.DataType.DT_String:
                    case NodeAttribute.DataType.DT_Path:
                    case NodeAttribute.DataType.DT_FixedString:
                    case NodeAttribute.DataType.DT_LSString:
                    case NodeAttribute.DataType.DT_WString:
                    case NodeAttribute.DataType.DT_LSWString:
                        writer.WriteValue(attribute.Value.AsString(SerializationSettings));
                        break;
                    case NodeAttribute.DataType.DT_ULongLong:
                        writer.WriteValue(Convert.ToUInt64(attribute.Value.Value));
                        break;
                    case NodeAttribute.DataType.DT_ScratchBuffer:
                        writer.WriteValue(Convert.ToBase64String((byte[])attribute.Value.Value));
                        break;
                    case NodeAttribute.DataType.DT_Long:
                    case NodeAttribute.DataType.DT_Int64:
                        writer.WriteValue(Convert.ToInt64(attribute.Value.Value));
                        break;
                    case NodeAttribute.DataType.DT_Int8:
                        writer.WriteValue(Convert.ToSByte(attribute.Value.Value));
                        break;
                    case NodeAttribute.DataType.DT_TranslatedString:
                        {
                            TranslatedString translatedString = (TranslatedString)attribute.Value.Value;
                            if (translatedString.Value != null)
                            {
                                writer.WritePropertyName("value");
                                writer.WriteValue(translatedString.Value);
                            }

                            if (translatedString.Version > 0)
                            {
                                writer.WritePropertyName("version");
                                writer.WriteValue(translatedString.Version);
                            }

                            writer.WritePropertyName("handle");
                            writer.WriteValue(translatedString.Handle);
                            break;
                        }
                    case NodeAttribute.DataType.DT_TranslatedFSString:
                        {
                            TranslatedFSString fs = (TranslatedFSString)attribute.Value.Value;
                            WriteTranslatedFSStringInner(writer, fs);
                            break;
                        }
                    case NodeAttribute.DataType.DT_UUID:
                        if (SerializationSettings.ByteSwapGuids)
                        {
                            writer.WriteValue(NodeAttribute.ByteSwapGuid((Guid)attribute.Value.Value).ToString());
                        }
                        else
                        {
                            writer.WriteValue(((Guid)attribute.Value.Value).ToString());
                        }

                        break;
                    case NodeAttribute.DataType.DT_Vec2:
                    case NodeAttribute.DataType.DT_Vec3:
                    case NodeAttribute.DataType.DT_Vec4:
                        {
                            float[] values2 = (float[])attribute.Value.Value;
                            writer.WriteValue(string.Join(" ", values2));
                            break;
                        }
                    case NodeAttribute.DataType.DT_IVec2:
                    case NodeAttribute.DataType.DT_IVec3:
                    case NodeAttribute.DataType.DT_IVec4:
                        {
                            int[] values = (int[])attribute.Value.Value;
                            writer.WriteValue(string.Join(" ", values));
                            break;
                        }
                    case NodeAttribute.DataType.DT_Mat2:
                    case NodeAttribute.DataType.DT_Mat3:
                    case NodeAttribute.DataType.DT_Mat3x4:
                    case NodeAttribute.DataType.DT_Mat4x3:
                    case NodeAttribute.DataType.DT_Mat4:
                        {
                            Matrix matrix = (Matrix)attribute.Value.Value;
                            string text = "";
                            for (int i = 0; i < matrix.rows; i++)
                            {
                                for (int j = 0; j < matrix.cols; j++)
                                {
                                    text = text + matrix[i, j] + " ";
                                }

                                text += Environment.NewLine;
                            }

                            writer.WriteValue(text);
                            break;
                        }
                    default:
                        throw new NotImplementedException("Don't know how to serialize type " + attribute.Value.Type);
                }

                writer.WriteEndObject();
            }

            foreach (KeyValuePair<string, List<Node>> child in node.Children)
            {
                writer.WritePropertyName(child.Key);
                writer.WriteStartArray();
                foreach (Node item in child.Value)
                {
                    WriteNode(writer, item, serializer);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Node)
            {
                WriteNode(writer, value as Node, serializer);
                return;
            }

            if (value is Resource)
            {
                WriteResource(writer, value as Resource, serializer);
                return;
            }

            throw new InvalidOperationException("Cannot serialize unknown type");
        }
    }

  


    public class LSJWriterFixed2String
    {
        public bool PrettyPrint;

        public NodeSerializationSettings SerializationSettings = new NodeSerializationSettings();

        public String LSXtoLSJ(Resource rsrc)
        {
            String json = "";
            try
            {
                json = JsonConvert.SerializeObject(rsrc, new JsonSerializerSettings
                {
                Formatting = Formatting.Indented,
                Converters = { (JsonConverter)new LSJConToString(SerializationSettings) },
                MissingMemberHandling = MissingMemberHandling.Error
                });

            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return json;
        }
    }
}
