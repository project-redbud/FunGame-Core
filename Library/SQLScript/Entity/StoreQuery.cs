using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class StoreQuery : Constant
    {
        public const string TableName = "Stores";
        public const string Column_Id = "Id";
        public const string Column_StoreName = "StoreName";
        public const string Column_StartTime = "StartTime";
        public const string Column_EndTime = "EndTime";
        public const string Column_StoreId = "StoreId";
        public const string Column_GoodsId = "GoodsId";
        public const string Column_GoodsName = "GoodsName";

        public const string Select_Stores = $"{Command_Select} {Command_All} {Command_From} {TableName}";
        public const string Select_StoresWithGoods = $"{Command_Select} {TableName}.{Column_Id} {Column_StoreId}, {TableName}.{Column_StoreName}, " +
            $"{GoodsQuery.TableName}.{GoodsQuery.Column_Id} {Column_GoodsId}, {GoodsQuery.TableName}.{GoodsQuery.Column_Name} {Column_GoodsName}, " +
            $"{GoodsQuery.TableName}.{GoodsQuery.Column_Description}, {GoodsQuery.TableName}.{GoodsQuery.Column_Stock}, " +
            $"{GoodsItemsQuery.TableName}.{GoodsItemsQuery.Column_ItemId}, {GoodsPricesQuery.TableName}.{GoodsPricesQuery.Column_Currency}, {GoodsPricesQuery.TableName}.{GoodsPricesQuery.Column_Price} " +
            $"{Command_From} {StoreGoodsQuery.TableName}\r\n" +
            $"{Command_LeftJoin} {TableName} {Command_On} {TableName}.{Column_Id} = {StoreGoodsQuery.TableName}.{StoreGoodsQuery.Column_StoreId}\r\n" +
            $"{Command_LeftJoin} {GoodsQuery.TableName} {Command_On} {GoodsQuery.TableName}.{GoodsQuery.Column_Id} = {StoreGoodsQuery.TableName}.{StoreGoodsQuery.Column_GoodsId}\r\n" +
            $"{Command_LeftJoin} {GoodsItemsQuery.TableName} {Command_On} {GoodsItemsQuery.TableName}.{GoodsItemsQuery.Column_GoodsId} = {GoodsQuery.TableName}.{GoodsQuery.Column_Id}\r\n" +
            $"{Command_LeftJoin} {GoodsPricesQuery.TableName} {Command_On} {GoodsPricesQuery.TableName}.{GoodsPricesQuery.Column_GoodsId} = {GoodsQuery.TableName}.{GoodsQuery.Column_Id}";

        public static string Select_StoreById(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Select_Stores} {Command_Where} {Column_Id} = @Id";
        }

        public static string Insert_Store(SQLHelper SQLHelper, string StoreName, DateTime? StartTime, DateTime? EndTime)
        {
            SQLHelper.Parameters["@StoreName"] = StoreName;
            if (StartTime.HasValue) SQLHelper.Parameters["@StartTime"] = StartTime;
            if (EndTime.HasValue) SQLHelper.Parameters["@EndTime"] = EndTime;

            return $"{Command_Insert} {Command_Into} {TableName} ({Column_StoreName}{(StartTime.HasValue ? $", {Column_StartTime}" : "")}{(EndTime.HasValue ? $", {Column_EndTime}" : "")}) " +
                $"{Command_Values} (@StoreName{(StartTime.HasValue ? ", @StartTime" : "")}{(EndTime.HasValue ? ", @EndTime" : "")})";
        }

        public static string Update_Store(SQLHelper SQLHelper, long Id, string StoreName, DateTime? StartTime, DateTime? EndTime)
        {
            SQLHelper.Parameters["@Id"] = Id;
            SQLHelper.Parameters["@StoreName"] = StoreName;
            if (StartTime.HasValue) SQLHelper.Parameters["@StartTime"] = StartTime;
            if (EndTime.HasValue) SQLHelper.Parameters["@EndTime"] = EndTime;
            return $"{Command_Update} {TableName} {Command_Set} {Column_StoreName} = @StoreName{(StartTime.HasValue ? $", {Column_StartTime} = @StartTime" : "")}{(EndTime.HasValue ? $", {Column_EndTime} = @EndTime" : "")} {Command_Where} {Column_Id} = @Id";
        }

        public static string Delete_Store(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Id} = @Id";
        }

        public static string Select_AllGoodsInStore(SQLHelper SQLHelper, long StoreId = 0)
        {
            if (StoreId != 0)
            {
                SQLHelper.Parameters["@StoreId"] = StoreId;
            }

            string sql = $"{Select_StoresWithGoods}{(StoreId != 0 ? $"\r\n{Command_Where} {StoreGoodsQuery.TableName}.{StoreGoodsQuery.Column_StoreId} = @StoreId" : "")}";

            return sql;
        }
    }
}
