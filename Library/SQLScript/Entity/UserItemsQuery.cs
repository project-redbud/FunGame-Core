using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class UserItemsQuery : Constant
    {
        public const string TableName = "UserItems";
        public const string Column_Id = "Id";
        public const string Column_ItemId = "ItemId";
        public const string Column_ItemGuid = "ItemGuid";
        public const string Column_UserId = "UserId";
        public const string Column_CharacterGuid = "CharacterGuid";
        public const string Column_ItemName = "ItemName";
        public const string Column_IsLock = "IsLock";
        public const string Column_Equipable = "Equipable";
        public const string Column_Unequipable = "Unequipable";
        public const string Column_EquipSlotType = "EquipSlotType";
        public const string Column_Key = "Key";
        public const string Column_Enable = "Enable";
        public const string Column_Price = "Price";
        public const string Column_IsSellable = "IsSellable";
        public const string Column_IsTradable = "IsTradable";
        public const string Column_NextSellableTime = "NextSellableTime";
        public const string Column_NextTradableTime = "NextTradableTime";
        public const string Column_RemainUseTimes = "RemainUseTimes";

        public const string Select_UserItems = $"{Command_Select} {Command_All} {Command_From} {TableName}";

        public static string Select_UserItemByItemGuid(SQLHelper SQLHelper, Guid ItemGuid)
        {
            SQLHelper.Parameters["@ItemGuid"] = ItemGuid.ToString();
            return $"{Select_UserItems} {Command_Where} {Column_ItemGuid} = @ItemGuid";
        }

        public static string Select_UserItemsByItemId(SQLHelper SQLHelper, long ItemId)
        {
            SQLHelper.Parameters["@ItemId"] = ItemId;
            return $"{Select_UserItems} {Command_Where} {Column_ItemId} = @ItemId";
        }

        public static string Select_UserItemsByUserId(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Select_UserItems} {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Select_UserItemsByCharacterGuid(SQLHelper SQLHelper, Guid CharacterGuid)
        {
            SQLHelper.Parameters["@CharacterGuid"] = CharacterGuid.ToString();
            return $"{Select_UserItems} {Command_Where} {Column_CharacterGuid} = @CharacterGuid";
        }

        public static string Insert_UserItem(SQLHelper SQLHelper, long ItemId, Guid ItemGuid, long UserId, Guid CharacterGuid, string ItemName,
            bool IsLock, bool Equipable, bool Unequipable, EquipSlotType EquipSlotType, int Key, bool Enable, double Price, bool IsSellable, bool IsTradable,
            DateTime? NextSellableTime, DateTime? NextTradableTime, int RemainUseTimes)
        {
            SQLHelper.Parameters["@ItemId"] = ItemId;
            SQLHelper.Parameters["@ItemGuid"] = ItemGuid.ToString();
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@CharacterGuid"] = CharacterGuid.ToString();
            SQLHelper.Parameters["@ItemName"] = ItemName;
            SQLHelper.Parameters["@IsLock"] = IsLock ? 1 : 0;
            SQLHelper.Parameters["@Equipable"] = Equipable ? 1 : 0;
            SQLHelper.Parameters["@Unequipable"] = Unequipable ? 1 : 0;
            SQLHelper.Parameters["@EquipSlotType"] = (int)EquipSlotType;
            SQLHelper.Parameters["@Key"] = Key;
            SQLHelper.Parameters["@Enable"] = Enable ? 1 : 0;
            SQLHelper.Parameters["@Price"] = Price;
            SQLHelper.Parameters["@IsSellable"] = IsSellable ? 1 : 0;
            SQLHelper.Parameters["@IsTradable"] = IsTradable ? 1 : 0;
            if (NextSellableTime.HasValue) SQLHelper.Parameters["@NextSellableTime"] = NextSellableTime;
            if (NextTradableTime.HasValue) SQLHelper.Parameters["@NextTradableTime"] = NextTradableTime;
            SQLHelper.Parameters["@RemainUseTimes"] = RemainUseTimes;

            string sql = $"{Command_Insert} {Command_Into} {TableName} " +
                $"({Column_ItemId}, {Column_ItemGuid}, {Column_UserId}, {Column_CharacterGuid}, {Column_ItemName}, {Column_IsLock}, {Column_Equipable}, {Column_Unequipable}, {Column_EquipSlotType}, {Column_Key}, {Column_Enable}, {Column_Price}, {Column_IsSellable}, {Column_IsTradable}, {Column_RemainUseTimes}" +
                $"{(NextSellableTime.HasValue ? $", {Column_NextSellableTime}" : "")}" +
                $"{(NextTradableTime.HasValue ? $", {Column_NextTradableTime}" : "")}) " +
                $"{Command_Values} (@ItemId, @ItemGuid, @UserId, @CharacterGuid, @ItemName, @IsLock, @Equipable, @Unequipable, @EquipSlotType, @Key, @Enable, @Price, @IsSellable, @IsTradable, @RemainUseTimes" +
                $"{(NextSellableTime.HasValue ? ", @NextSellableTime" : "")}" +
                $"{(NextTradableTime.HasValue ? ", @NextTradableTime" : "")})";
            return sql;
        }

        public static string Update_UserItem(SQLHelper SQLHelper, long ItemId, Guid ItemGuid, long UserId, Guid CharacterGuid, string ItemName,
            bool IsLock, bool Equipable, bool Unequipable, EquipSlotType EquipSlotType, int Key, bool Enable, double Price, bool IsSellable, bool IsTradable,
            DateTime? NextSellableTime, DateTime? NextTradableTime, int RemainUseTimes)
        {
            SQLHelper.Parameters["@ItemId"] = ItemId;
            SQLHelper.Parameters["@ItemGuid"] = ItemGuid.ToString();
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@CharacterGuid"] = CharacterGuid.ToString();
            SQLHelper.Parameters["@ItemName"] = ItemName;
            SQLHelper.Parameters["@IsLock"] = IsLock ? 1 : 0;
            SQLHelper.Parameters["@Equipable"] = Equipable ? 1 : 0;
            SQLHelper.Parameters["@Unequipable"] = Unequipable ? 1 : 0;
            SQLHelper.Parameters["@EquipSlotType"] = (int)EquipSlotType;
            SQLHelper.Parameters["@Key"] = Key;
            SQLHelper.Parameters["@Enable"] = Enable ? 1 : 0;
            SQLHelper.Parameters["@Price"] = Price;
            SQLHelper.Parameters["@IsSellable"] = IsSellable ? 1 : 0;
            SQLHelper.Parameters["@IsTradable"] = IsTradable ? 1 : 0;
            if (NextSellableTime.HasValue) SQLHelper.Parameters["@NextSellableTime"] = NextSellableTime;
            if (NextTradableTime.HasValue) SQLHelper.Parameters["@NextTradableTime"] = NextTradableTime;
            SQLHelper.Parameters["@RemainUseTimes"] = RemainUseTimes;

            string sql = $"{Command_Update} {TableName} {Command_Set} " +
                $"{Column_ItemId} = @ItemId, {Column_UserId} = @UserId, {Column_CharacterGuid} = @CharacterGuid, {Column_ItemName} = @ItemName, " +
                $"{Column_IsLock} = @IsLock, {Column_Equipable} = @Equipable, {Column_Unequipable} = @Unequipable, {Column_EquipSlotType} = @EquipSlotType, " +
                $"{Column_Key} = @Key, {Column_Enable} = @Enable, {Column_Price} = @Price, {Column_IsSellable} = @IsSellable, {Column_IsTradable} = @IsTradable, {Column_RemainUseTimes} = @RemainUseTimes" +
                $"{(NextSellableTime.HasValue ? $", {Column_NextSellableTime} = @NextSellableTime" : "")}" +
                $"{(NextTradableTime.HasValue ? $", {Column_NextTradableTime} = @NextTradableTime" : "")} " +
                $"{Command_Where} {Column_ItemGuid} = @ItemGuid";
            return sql;
        }

        public static string Update_UserItemLockState(SQLHelper SQLHelper, Guid ItemGuid, int IsLock)
        {
            SQLHelper.Parameters["@ItemGuid"] = ItemGuid.ToString();
            SQLHelper.Parameters["@IsLock"] = IsLock;
            return $"{Command_Update} {TableName} {Command_Set} {Column_IsLock} = @IsLock {Command_Where} {Column_ItemGuid} = @ItemGuid";
        }

        public static string Delete_UserItem(SQLHelper SQLHelper, Guid ItemGuid)
        {
            SQLHelper.Parameters["@ItemGuid"] = ItemGuid.ToString();
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_ItemGuid} = @ItemGuid";
        }

        public static string Delete_UserItemsByUserId(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Delete_UserItemByCharacterGuid(SQLHelper SQLHelper, Guid CharacterGuid)
        {
            SQLHelper.Parameters["@CharacterGuid"] = CharacterGuid.ToString();
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_CharacterGuid} = @CharacterGuid";
        }
    }
}
