using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Milimoe.FunGame.Core.Library.Constant;

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
                            ConverterUtility.ReadColumns(reader, dt);
                            break;

                        case "Rows":
                            reader.Read();
                            ConverterUtility.ReadRows(reader, dt);
                            break;
                    }
                }
            }

            if (ds.Tables.Count == 0) ds.Tables.Add(new DataTable());

            return ds;
        }

        public override void Write(Utf8JsonWriter writer, DataSet value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Tables");
            writer.WriteStartArray();

            if (value.Tables.Count > 0) writer.WriteStartObject();

            for (int count = 0; count < value.Tables.Count; count++)
            {
                writer.WriteString("TableName", value.Tables[count].TableName);

                writer.WritePropertyName("Columns");
                writer.WriteStartArray();

                foreach (DataColumn column in value.Tables[count].Columns)
                {
                    writer.WriteStartObject();

                    writer.WriteString("ColumnName", column.ColumnName);
                    writer.WriteString("DataType", column.DataType.FullName);

                    writer.WriteEndObject();
                }

                writer.WriteEndArray();

                writer.WritePropertyName("Rows");
                writer.WriteStartArray();


                foreach (DataRow row in value.Tables[count].Rows)
                {
                    writer.WriteStartArray();

                    for (int i = 0; i < value.Tables[count].Columns.Count; i++)
                    {
                        var rowValue = row[i];

                        switch (value.Tables[count].Columns[i].DataType.FullName)
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

                if (count + 1 < value.Tables.Count)
                {
                    writer.WriteEndObject();
                    writer.WriteStartObject();
                }
                else writer.WriteEndObject();
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
