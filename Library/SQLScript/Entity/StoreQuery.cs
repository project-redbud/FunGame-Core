﻿using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class StoreQuery : Constant
    {
        public const string TableName = "Stores";
        public const string Column_Id = "Id";
        public const string Column_StoreName = "StoreName";
        public const string Column_StartTime = "StartTime";
        public const string Column_EndTime = "EndTime";

        public const string Select_Stores = $"{Command_Select} {Command_All} {Command_From} {TableName}";

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

            string sql = $"{Command_Select} {TableName}.{Column_Id} StoreId, {TableName}.{Column_StoreName}, " +
                $"{GoodsQuery.TableName}.{GoodsQuery.Column_Id} GoodsId, {GoodsQuery.TableName}.{GoodsQuery.Column_Name} GoodsName, " +
                $"{GoodsQuery.TableName}.{GoodsQuery.Column_Description}, {GoodsQuery.TableName}.{GoodsQuery.Column_Stock}, " +
                $"{GoodItemsQuery.TableName}.{GoodItemsQuery.Column_ItemId}, {GoodPricesQuery.TableName}.{GoodPricesQuery.Column_Currency}, {GoodPricesQuery.TableName}.{GoodPricesQuery.Column_Price} " +
                $"{Command_From} {StoreGoodsQuery.TableName}\r\n" +
                $"{Command_LeftJoin} {TableName} {Command_On} {TableName}.{Column_Id} = {StoreGoodsQuery.TableName}.{StoreGoodsQuery.Column_StoreId}\r\n" +
                $"{Command_LeftJoin} {GoodsQuery.TableName} {Command_On} {GoodsQuery.TableName}.{GoodsQuery.Column_Id} = {StoreGoodsQuery.TableName}.{StoreGoodsQuery.Column_GoodsId}\r\n" +
                $"{Command_LeftJoin} {GoodItemsQuery.TableName} {Command_On} {GoodItemsQuery.TableName}.{GoodItemsQuery.Column_GoodsId} = {GoodsQuery.TableName}.{GoodsQuery.Column_Id}\r\n" +
                $"{Command_LeftJoin} {GoodPricesQuery.TableName} {Command_On} {GoodPricesQuery.TableName}.{GoodPricesQuery.Column_GoodsId} = {GoodsQuery.TableName}.{GoodsQuery.Column_Id}\r\n" +
                (StoreId != 0 ? $"{Command_Where} {StoreGoodsQuery.TableName}.{StoreGoodsQuery.Column_StoreId} = @StoreId" : "");

            return sql;
        }
    }
}
