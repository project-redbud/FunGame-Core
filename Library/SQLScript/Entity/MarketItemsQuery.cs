﻿using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class MarketItemsQuery : Constant
    {
        public const string TableName = "MarketItems";
        public const string Column_Id = "Id";
        public const string Column_ItemGuid = "ItemGuid";
        public const string Column_UserId = "UserId";
        public const string Column_Price = "Price";
        public const string Column_CreateTime = "CreateTime";
        public const string Column_FinishTime = "FinishTime";
        public const string Column_Status = "Status";
        public const string Column_Buyer = "Buyer";

        public const string Select_MarketItems = $"{Command_Select} {Command_All} {Command_From} {TableName}";

        public static string Select_MarketItemById(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Select_MarketItems} {Command_Where} {Column_Id} = @Id";
        }

        public static string Select_MarketItemsByItemId(SQLHelper SQLHelper, Guid ItemGuid)
        {
            SQLHelper.Parameters["@ItemGuid"] = ItemGuid.ToString();
            return $"{Select_MarketItems} {Command_Where} {Column_ItemGuid} = @ItemGuid";
        }

        public static string Select_MarketItemsByUserId(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Select_MarketItems} {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Select_MarketItemsByState(SQLHelper SQLHelper, MarketItemState state)
        {
            SQLHelper.Parameters["@Status"] = (int)state;
            return $"{Select_MarketItems} {Command_Where} {Column_Status} = @Status";
        }

        public static string Insert_MarketItem(SQLHelper SQLHelper, Guid ItemGuid, long UserId, double Price, MarketItemState state = MarketItemState.Listed)
        {
            SQLHelper.Parameters["@ItemGuid"] = ItemGuid.ToString();
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@Price"] = Price;
            SQLHelper.Parameters["@Status"] = (int)state;

            return $"{Command_Insert} {Command_Into} {TableName} ({Column_ItemGuid}, {Column_UserId}, {Column_Price}, {Column_Status}) {Command_Values} (@ItemId, @UserId, @Price, @Status)";
        }

        public static string Update_MarketItemPrice(SQLHelper SQLHelper, long Id, double Price)
        {
            SQLHelper.Parameters["@Id"] = Id;
            SQLHelper.Parameters["@Price"] = Price;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Price} = @Price {Command_Where} {Column_Id} = @Id";
        }

        public static string Update_MarketItemState(SQLHelper SQLHelper, long Id, MarketItemState state)
        {
            SQLHelper.Parameters["@Id"] = Id;
            SQLHelper.Parameters["@Status"] = (int)state;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Status} = @Status {Command_Where} {Column_Id} = @Id";
        }

        public static string Update_MarketItemBuyer(SQLHelper SQLHelper, long Id, long Buyer)
        {
            SQLHelper.Parameters["@Id"] = Id;
            SQLHelper.Parameters["@Buyer"] = Buyer;
            SQLHelper.Parameters["@Status"] = (int)MarketItemState.Purchased;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Buyer} = @Buyer, {Column_Status} = @Status {Command_Where} {Column_Id} = @Id";
        }

        public static string Update_MarketItemFinishTime(SQLHelper SQLHelper, long Id, DateTime FinishTime)
        {
            SQLHelper.Parameters["@Id"] = Id;
            SQLHelper.Parameters["@FinishTime"] = FinishTime;
            return $"{Command_Update} {TableName} {Command_Set} {Column_FinishTime} = @FinishTime {Command_Where} {Column_Id} = @Id";
        }

        public static string Delete_MarketItem(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Id} = @Id";
        }
        
        public static string Delete_MarketItemByUserId(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Id} = @UserId";
        }

        public static string Select_AllMarketItems(SQLHelper SQLHelper, long UserId = 0, MarketItemState? state = null)
        {
            string sql = Select_MarketItems;
            string whereClause = "";

            if (UserId != 0)
            {
                SQLHelper.Parameters["@UserId"] = UserId;
                whereClause += $"{Command_And} {Column_UserId} = @UserId\r\n";
            }

            if (state.HasValue)
            {
                SQLHelper.Parameters["@Status"] = (int)state.Value;
                whereClause += $"{Command_And} {Column_Status} = @Status\r\n";
            }

            if (!string.IsNullOrEmpty(whereClause))
            {
                sql += $" {Command_Where} {whereClause[Command_And.Length..]}";
            }

            return sql.Trim();
        }
    }
}
