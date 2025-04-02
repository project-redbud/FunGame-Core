using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class InventoriesQuery : Constant
    {
        public const string TableName = "Inventories";
        public const string Column_UserId = "UserId";
        public const string Column_Name = "Name";
        public const string Column_Credits = "Credits";
        public const string Column_Materials = "Materials";
        public const string Column_MainCharacter = "MainCharacter";

        public const string Select_Inventories = $"{Command_Select} {Command_All} {Command_From} {TableName}";

        public static string Select_InventoryByUserId(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Select_Inventories} {Command_Where} {Column_UserId} = @UserId";
        }
        
        public static string Select_MainCharacterByUserId(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Command_Select} {Column_MainCharacter} {Command_From} {TableName} {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Insert_Inventory(SQLHelper SQLHelper, long UserId, string Name, double Credits, double Materials, long MainCharacter)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@Name"] = Name;
            SQLHelper.Parameters["@Credits"] = Credits;
            SQLHelper.Parameters["@Materials"] = Materials;
            SQLHelper.Parameters["@MainCharacter"] = MainCharacter;

            return $"{Command_Insert} {Command_Into} {TableName} ({Column_UserId}, {Column_Name}, {Column_Credits}, {Column_Materials}, {Column_MainCharacter}) " +
                $"{Command_Values} (@UserId, @Name, @Credits, @Materials, @MainCharacter)";
        }

        public static string Update_Inventory(SQLHelper SQLHelper, long UserId, string Name, double Credits, double Materials, long MainCharacter)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@Name"] = Name;
            SQLHelper.Parameters["@Credits"] = Credits;
            SQLHelper.Parameters["@Materials"] = Materials;
            SQLHelper.Parameters["@MainCharacter"] = MainCharacter;

            return $"{Command_Update} {TableName} {Command_Set} {Column_Name} = @Name, {Column_Credits} = @Credits, {Column_Materials} = @Materials, {Column_MainCharacter} = @MainCharacter {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Update_InventoryCredits(SQLHelper SQLHelper, long UserId, double Credits)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@Credits"] = Credits;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Credits} = @Credits {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Update_InventoryMaterials(SQLHelper SQLHelper, long UserId, double Materials)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@Materials"] = Materials;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Materials} = @Materials {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Update_InventoryMainCharacter(SQLHelper SQLHelper, long UserId, long MainCharacter)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@MainCharacter"] = MainCharacter;
            return $"{Command_Update} {TableName} {Command_Set} {Column_MainCharacter} = @MainCharacter {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Delete_Inventory(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_UserId} = @UserId";
        }
    }
}
