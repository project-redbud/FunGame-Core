using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class GoodsPricesQuery : Constant
    {
        public const string TableName = "GoodsPrices";
        public const string Column_Id = "Id";
        public const string Column_GoodsId = "GoodsId";
        public const string Column_Currency = "Currency";
        public const string Column_Price = "Price";

        public const string Select_GoodsPrices = $"{Command_Select} {Command_All} {Command_From} {TableName}";

        public static string Select_GoodsPricesByGoodsId(SQLHelper SQLHelper, long GoodsId)
        {
            SQLHelper.Parameters["@GoodsId"] = GoodsId;
            return $"{Select_GoodsPrices} {Command_Where} {Column_GoodsId} = @GoodsId";
        }

        public static string Insert_GoodsPrice(SQLHelper SQLHelper, long GoodsId, string Currency, double Price)
        {
            SQLHelper.Parameters["@GoodsId"] = GoodsId;
            SQLHelper.Parameters["@Currency"] = Currency;
            SQLHelper.Parameters["@Price"] = Price;
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_GoodsId}, {Column_Currency}, {Column_Price}) {Command_Values} (@GoodsId, @Currency, @Price)";
        }

        public static string Update_GoodsPrice(SQLHelper SQLHelper, long GoodsId, string Currency, double Price)
        {
            SQLHelper.Parameters["@GoodsId"] = GoodsId;
            SQLHelper.Parameters["@Currency"] = Currency;
            SQLHelper.Parameters["@Price"] = Price;
            return $"{Command_Update} {TableName} {Command_Set} {Column_GoodsId} = @GoodsId, {Column_Price} = @Price {Command_Where} {Column_Currency} = @Currency";
        }

        public static string Delete_GoodsPrice(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Id} = @Id";
        }

        public static string Delete_GoodsPriceByGoodsId(SQLHelper SQLHelper, long GoodsId)
        {
            SQLHelper.Parameters["@GoodsId"] = GoodsId;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_GoodsId} = @GoodsId";
        }
    }
}
