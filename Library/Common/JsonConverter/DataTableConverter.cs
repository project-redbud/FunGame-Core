using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class DataTableConverter : JsonConverter<DataTable>
    {
        public override DataTable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
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
                            break;

                        case "Columns":
                            reader.Read();
                            ConverterUtility.ReadColumns(reader, dt);
                            break;

                        case "Rows":
                            reader.Read();
                            ConverterUtility.ReadRows(reader, dt);
                            break;
                    }
                }
            }

            return dt;
        }

        public override void Write(Utf8JsonWriter writer, DataTable value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("TableName", value.TableName);

            writer.WritePropertyName("Columns");
            writer.WriteStartArray();

            foreach (DataColumn column in value.Columns)
            {
                writer.WriteStartObject();

                writer.WriteString("ColumnName", column.ColumnName);
                writer.WriteString("DataType", column.DataType.FullName);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();

            writer.WritePropertyName("Rows");
            writer.WriteStartArray();

            foreach (DataRow row in value.Rows)
            {
                writer.WriteStartArray();

                for (int i = 0; i < value.Columns.Count; i++)
                {
                    var rowValue = row[i];

                    switch (value.Columns[i].DataType.FullName)
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
    }
}