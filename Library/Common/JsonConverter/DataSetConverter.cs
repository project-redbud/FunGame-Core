using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class DataSetConverter : JsonConverter<DataSet>
    {
        public override DataSet Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            DataSet ds = new();
            DataTable dt = new();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string property = reader.GetString() ?? "";

                    switch (property)
                    {
                        case "TableName":
                            reader.Read();
                            string tableName = reader.GetString() ?? "";
                            dt = new DataTable(tableName);
                            ds.Tables.Add(dt);
                            break;

                        case "Columns":
                            reader.Read();
                            ReadColumns(reader, dt);
                            break;

                        case "Rows":
                            reader.Read();
                            ReadRows(reader, dt);
                            break;
                    }
                }
            }

            return ds;
        }

        public override void Write(Utf8JsonWriter writer, DataSet value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("TableName", value.Tables[0].TableName);

            writer.WritePropertyName("Columns");
            writer.WriteStartArray();

            foreach (DataColumn column in value.Tables[0].Columns)
            {
                writer.WriteStartObject();

                writer.WriteString("ColumnName", column.ColumnName);
                writer.WriteString("DataType", column.DataType.FullName);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();

            writer.WritePropertyName("Rows");
            writer.WriteStartArray();

            foreach (DataRow row in value.Tables[0].Rows)
            {
                writer.WriteStartArray();

                for (int i = 0; i < value.Tables[0].Columns.Count; i++)
                {
                    object? rowValue = row[i];

                    switch (value.Tables[0].Columns[i].DataType.FullName)
                    {
                        case "System.Boolean":
                            writer.WriteBooleanValue((bool)rowValue);
                            break;

                        case "System.Byte":
                            writer.WriteNumberValue((byte)rowValue);
                            break;

                        case "System.Char":
                            writer.WriteStringValue(value.ToString());
                            break;

                        case "System.DateTime":
                            writer.WriteStringValue(((DateTime)rowValue).ToString(General.GeneralDateTimeFormat));
                            break;

                        case "System.Decimal":
                            writer.WriteNumberValue((decimal)rowValue);
                            break;

                        case "System.Double":
                            writer.WriteNumberValue((double)rowValue);
                            break;

                        case "System.Guid":
                            writer.WriteStringValue(value.ToString());
                            break;

                        case "System.Int16":
                            writer.WriteNumberValue((short)rowValue);
                            break;

                        case "System.Int32":
                            writer.WriteNumberValue((int)rowValue);
                            break;

                        case "System.Int64":
                            writer.WriteNumberValue((long)rowValue);
                            break;

                        case "System.SByte":
                            writer.WriteNumberValue((sbyte)rowValue);
                            break;

                        case "System.Single":
                            writer.WriteNumberValue((float)rowValue);
                            break;

                        case "System.String":
                            writer.WriteStringValue((string)rowValue);
                            break;

                        case "System.UInt16":
                            writer.WriteNumberValue((ushort)rowValue);
                            break;

                        case "System.UInt32":
                            writer.WriteNumberValue((uint)rowValue);
                            break;

                        case "System.UInt64":
                            writer.WriteNumberValue((ulong)rowValue);
                            break;
                    }
                }

                writer.WriteEndArray();
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        #pragma warning disable CA1822 // 不需要设为静态
        private void ReadColumns(Utf8JsonReader reader, DataTable dataTable)
        #pragma warning restore CA1822 // 不需要设为静态
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    DataColumn column = new();

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.PropertyName)
                        {
                            string propertyName = reader.GetString() ?? "";

                            switch (propertyName)
                            {
                                case "ColumnName":
                                    reader.Read();
                                    column.ColumnName = reader.GetString();
                                    break;

                                case "DataType":
                                    reader.Read();
                                    Type dataType = Type.GetType(reader.GetString() ?? "") ?? typeof(object);
                                    column.DataType = dataType;
                                    break;
                            }
                        }

                        if (reader.TokenType == JsonTokenType.EndObject)
                        {
                            break;
                        }
                    }

                    dataTable.Columns.Add(column);
                }
            }
        }

        private void ReadRows(Utf8JsonReader reader, DataTable dataTable)
        {
            object[] values = new object[dataTable.Columns.Count];

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    int index = 0;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            break;
                        }

                        switch (dataTable.Columns[index].DataType.ToString())
                        {
                            case "System.Boolean":
                                values[index] = reader.GetBoolean();
                                break;

                            case "System.Byte":
                                values[index] = reader.GetByte();
                                break;

                            case "System.Char":
                                values[index] = (reader.GetString() ?? "")[0];
                                break;

                            case "System.DateTime":
                                string dateString = reader.GetString() ?? "";
                                if (DateTime.TryParseExact(dateString, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime result))
                                {
                                    values[index] = result;
                                }
                                values[index] = DateTime.MinValue;
                                break;

                            case "System.Decimal":
                                values[index] = reader.GetDecimal();
                                break;

                            case "System.Double":
                                values[index] = reader.GetDouble();
                                break;

                            case "System.Guid":
                                values[index] = Guid.Parse(reader.GetString() ?? Guid.Empty.ToString());
                                break;

                            case "System.Int16":
                                values[index] = reader.GetInt16();
                                break;

                            case "System.Int32":
                                values[index] = reader.GetInt32();
                                break;

                            case "System.Int64":
                                values[index] = reader.GetInt64();
                                break;

                            case "System.SByte":
                                values[index] = reader.GetSByte();
                                break;

                            case "System.Single":
                                values[index] = reader.GetSingle();
                                break;

                            case "System.String":
                                values[index] = reader.GetString() ?? "";
                                break;

                            case "System.UInt16":
                                values[index] = reader.GetUInt16();
                                break;

                            case "System.UInt32":
                                values[index] = reader.GetUInt32();
                                break;

                            case "System.UInt64":
                                values[index] = reader.GetUInt64();
                                break;
                        }
                        index++;
                    }
                    dataTable.Rows.Add(values);
                }
            }
        }
    }
}
