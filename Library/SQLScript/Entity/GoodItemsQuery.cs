using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class GoodItemsQuery : Constant
    {
        public const string TableName = "GoodItems";
        public const string Column_Id = "Id";
        public const string Column_GoodsId = "GoodsId";
        public const string Column_ItemId = "ItemId";

        public const string Select_GoodItems = $"{Command_Select} {Command_All} {Command_From} {TableName}";

        public static string Select_GoodItemsByGoodsId(SQLHelper SQLHelper, long GoodsId)
        {
            SQLHelper.Parameters["@GoodsId"] = GoodsId;
            return $"{Select_GoodItems} {Command_Where} {Column_GoodsId} = @GoodsId";
        }

        public static string Insert_GoodItem(SQLHelper SQLHelper, long GoodsId, long ItemId)
        {
            SQLHelper.Parameters["@GoodsId"] = GoodsId;
            SQLHelper.Parameters["@ItemId"] = ItemId;
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_GoodsId}, {Column_ItemId}) {Command_Values} (@GoodsId, @ItemId)";
        }

        public static string Delete_GoodItem(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Id} = @Id";
        }

        public static string Delete_GoodItemsByGoodsId(SQLHelper SQLHelper, long GoodsId)
        {
            SQLHelper.Parameters["@GoodsId"] = GoodsId;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_GoodsId} = @GoodsId";
        }
    }

}
