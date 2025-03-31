using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class StoreGoodsQuery : Constant
    {
        public const string TableName = "StoreGoods";
        public const string Column_Id = "Id";
        public const string Column_StoreId = "StoreId";
        public const string Column_GoodsId = "GoodsId";

        public const string Select_StoreGoods = $"{Command_Select} {Command_All} {Command_From} {TableName}";

        public static string Select_StoreGoodsByStoreId(SQLHelper SQLHelper, long StoreId)
        {
            SQLHelper.Parameters["@StoreId"] = StoreId;
            return $"{Select_StoreGoods} {Command_Where} {Column_StoreId} = @StoreId";
        }
        
        public static string Select_StoreGoodsByGoodsId(SQLHelper SQLHelper, long GoodsId)
        {
            SQLHelper.Parameters["@GoodsId"] = GoodsId;
            return $"{Select_StoreGoods} {Command_Where} {Column_GoodsId} = @GoodsId";
        }
        
        public static string Select_StoreGoodsByStoreIdAndGoodsId(SQLHelper SQLHelper, long StoreId, long goodsId)
        {
            SQLHelper.Parameters["@StoreId"] = StoreId;
            SQLHelper.Parameters["@GoodsId"] = goodsId;
            return $"{Select_StoreGoods} {Command_Where} {Column_StoreId} = @StoreId {Command_And} {Column_GoodsId} = @GoodsId";
        }

        public static string Insert_StoreGood(SQLHelper SQLHelper, long StoreId, long GoodsId)
        {
            SQLHelper.Parameters["@StoreId"] = StoreId;
            SQLHelper.Parameters["@GoodsId"] = GoodsId;
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_StoreId}, {Column_GoodsId}) {Command_Values} (@StoreId, @GoodsId)";
        }

        public static string Delete_StoreGood(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Id} = @Id";
        }

        public static string Delete_StoreGoodByStoreId(SQLHelper SQLHelper, long StoreId)
        {
            SQLHelper.Parameters["@StoreId"] = StoreId;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_StoreId} = @StoreId";
        }

        public static string Delete_StoreGoodByGoodsId(SQLHelper SQLHelper, long GoodsId)
        {
            SQLHelper.Parameters["@GoodsId"] = GoodsId;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_GoodsId} = @GoodsId";
        }

        public static string Delete_StoreGoodByStoreIdAndGoodsId(SQLHelper SQLHelper, long StoreId, long GoodsId)
        {
            SQLHelper.Parameters["@StoreId"] = StoreId;
            SQLHelper.Parameters["@GoodsId"] = GoodsId;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_StoreId} = @StoreId {Command_And} {Column_GoodsId} = @GoodsId";
        }
    }
}
