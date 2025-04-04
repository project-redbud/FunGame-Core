using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class GoodsQuery : Constant
    {
        public const string TableName = "Goods";
        public const string Column_Id = "Id";
        public const string Column_Name = "Name";
        public const string Column_Description = "Description";
        public const string Column_Stock = "Stock";

        public const string Select_Goods = $"{Command_Select} {Command_All} {Command_From} {TableName}";
        public const string Select_GoodsWithItemAndPrice = $"{Command_Select} {TableName}.{Column_Id}, {TableName}.{Column_Name}, {TableName}.{Column_Description}, {TableName}.{Column_Stock}, " +
            $"{GoodsItemsQuery.TableName}.{GoodsItemsQuery.Column_ItemId}, {GoodsPricesQuery.TableName}.{GoodsPricesQuery.Column_Currency}, {GoodsPricesQuery.TableName}.{GoodsPricesQuery.Column_Price} " +
            $"{Command_From} {TableName} \r\n" +
            $"{Command_LeftJoin} {GoodsItemsQuery.TableName} {Command_On} {GoodsItemsQuery.TableName}.{GoodsItemsQuery.Column_GoodsId} = {TableName}.{Column_Id}\r\n" +
            $"{Command_LeftJoin} {GoodsPricesQuery.TableName} {Command_On} {GoodsPricesQuery.TableName}.{GoodsPricesQuery.Column_GoodsId} = {TableName}.{Column_Id}";

        public static string Select_GoodsById(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Select_Goods} {Command_Where} {Column_Id} = @Id";
        }

        public static string Insert_Goods(SQLHelper SQLHelper, string Name, string Description, int Stock)
        {
            SQLHelper.Parameters["@Name"] = Name;
            SQLHelper.Parameters["@Description"] = Description;
            SQLHelper.Parameters["@Stock"] = Stock;
            return $"{Command_Insert} {Command_Into} {TableName} ({Column_Name}, {Column_Description}, {Column_Stock}) {Command_Values} (@Name, @Description, @Stock)";
        }

        public static string Update_Goods(SQLHelper SQLHelper, long Id, string Name, string Description, int Stock)
        {
            SQLHelper.Parameters["@Id"] = Id;
            SQLHelper.Parameters["@Name"] = Name;
            SQLHelper.Parameters["@Description"] = Description;
            SQLHelper.Parameters["@Stock"] = Stock;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Name} = @Name, {Column_Description} = @Description, {Column_Stock} = @Stock {Command_Where} {Column_Id} = @Id";
        }

        public static string Delete_Goods(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Id} = @Id";
        }

        public static string Select_AllGoodsWithItemAndPrice(SQLHelper SQLHelper, long GoodsId = 0)
        {
            if (GoodsId != 0)
            {
                SQLHelper.Parameters["@GoodsId"] = GoodsId;
            }

            string sql = $"{Select_GoodsWithItemAndPrice}{(GoodsId != 0 ? $"\r\n{Command_Where} {TableName}.{Column_Id} = @GoodsId" : "")}";

            return sql;
        }
    }
}
