using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class OfferItemsQuery : Constant
    {
        public const string TableName = "OfferItems";
        public const string TableName_Backup = "OfferItemsBackup";
        public const string Column_Id = "Id";
        public const string Column_OfferId = "OfferId";
        public const string Column_UserId = "UserId";
        public const string Column_ItemGuid = "ItemGuid";

        public const string Select_OfferItems = $"{Command_Select} {Command_All} {Command_From} {TableName}";
        public const string Select_OfferItemsBackup = $"{Command_Select} {Command_All} {Command_From} {TableName_Backup}";

        public static string Select_OfferItemsByOfferIdAndUserId(SQLHelper SQLHelper, long OfferId, long UserId)
        {
            SQLHelper.Parameters["@OfferId"] = OfferId;
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Select_OfferItems} {Command_Where} {Column_OfferId} = @OfferId {Command_And} {Column_UserId} = @UserId";
        }

        public static string Select_OfferItemsBackupByOfferIdAndUserId(SQLHelper SQLHelper, long OfferId, long UserId)
        {
            SQLHelper.Parameters["@OfferId"] = OfferId;
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Select_OfferItemsBackup} {Command_Where} {Column_OfferId} = @OfferId {Command_And} {Column_UserId} = @UserId";
        }

        public static string Insert_OfferItem(SQLHelper SQLHelper, long OfferId, long UserId, Guid ItemGuid)
        {
            SQLHelper.Parameters["@OfferId"] = OfferId;
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@ItemGuid"] = ItemGuid.ToString();

            return $"{Command_Insert} {Command_Into} {TableName} ({Column_OfferId}, {Column_UserId}, {Column_ItemGuid}) " +
                   $"{Command_Values} (@OfferId, @UserId, @ItemGuid)";
        }

        public static string Insert_OfferItemBackup(SQLHelper SQLHelper, long OfferId, long UserId, Guid ItemGuid)
        {
            SQLHelper.Parameters["@OfferId"] = OfferId;
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@ItemGuid"] = ItemGuid.ToString();

            return $"{Command_Insert} {Command_Into} {TableName_Backup} ({Column_OfferId}, {Column_UserId}, {Column_ItemGuid}) " +
                   $"{Command_Values} (@OfferId, @UserId, @ItemGuid)";
        }

        public static string Delete_OfferItem(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Id} = @Id";
        }

        public static string Delete_OfferItemsByOfferId(SQLHelper SQLHelper, long OfferId)
        {
            SQLHelper.Parameters["@OfferId"] = OfferId;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_OfferId} = @OfferId";
        }

        public static string Delete_OfferItemsBackupByOfferId(SQLHelper SQLHelper, long OfferId)
        {
            SQLHelper.Parameters["@OfferId"] = OfferId;
            return $"{Command_Delete} {Command_From} {TableName_Backup} {Command_Where} {Column_OfferId} = @OfferId";
        }
    }
}
