using System.Data;
using System.Text.Json;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    internal class ConverterUtility
    {
        internal static void ReadColumns(Utf8JsonReader reader, DataTable dataTable)
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

        internal static void ReadRows(Utf8JsonReader reader, DataTable dataTable)
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
