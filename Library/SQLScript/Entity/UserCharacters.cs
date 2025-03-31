using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class UserCharactersQuery : Constant
    {
        public const string TableName = "UserCharacters";
        public const string Column_Id = "Id";
        public const string Column_CharacterId = "CharacterId";
        public const string Column_UserId = "UserId";
        public const string Column_Name = "Name";
        public const string Column_FirstName = "FirstName";
        public const string Column_NickName = "NickName";
        public const string Column_PrimaryAttribute = "PrimaryAttribute";
        public const string Column_InitialATK = "InitialATK";
        public const string Column_InitialDEF = "InitialDEF";
        public const string Column_InitialHP = "InitialHP";
        public const string Column_InitialMP = "InitialMP";
        public const string Column_InitialAGI = "InitialAGI";
        public const string Column_InitialINT = "InitialINT";
        public const string Column_InitialSTR = "InitialSTR";
        public const string Column_InitialSPD = "InitialSPD";
        public const string Column_InitialHR = "InitialHR";
        public const string Column_InitialMR = "InitialMR";
        public const string Column_InSquad = "InSquad";
        public const string Column_TrainingTime = "TrainingTime";

        public const string Select_UserCharacters = $"{Command_Select} {Command_All} {Command_From} {TableName}";

        public static string Select_UserCharacterById(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Select_UserCharacters} {Command_Where} {Column_Id} = @Id";
        }

        public static string Select_UserCharactersByCharacterId(SQLHelper SQLHelper, long CharacterId)
        {
            SQLHelper.Parameters["@CharacterId"] = CharacterId;
            return $"{Select_UserCharacters} {Command_Where} {Column_CharacterId} = @CharacterId";
        }

        public static string Select_UserCharactersByUserId(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Select_UserCharacters} {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Insert_UserCharacter(SQLHelper SQLHelper, long CharacterId, long UserId, string Name, string FirstName, string NickName,
            int PrimaryAttribute, double InitialATK, double InitialDEF, double InitialHP, double InitialMP, double InitialAGI, double InitialINT,
            double InitialSTR, double InitialSPD, double InitialHR, double InitialMR, int InSquad, DateTime? TrainingTime)
        {
            SQLHelper.Parameters["@CharacterId"] = CharacterId;
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@Name"] = Name;
            SQLHelper.Parameters["@FirstName"] = FirstName;
            SQLHelper.Parameters["@NickName"] = NickName;
            SQLHelper.Parameters["@PrimaryAttribute"] = PrimaryAttribute;
            SQLHelper.Parameters["@InitialATK"] = InitialATK;
            SQLHelper.Parameters["@InitialDEF"] = InitialDEF;
            SQLHelper.Parameters["@InitialHP"] = InitialHP;
            SQLHelper.Parameters["@InitialMP"] = InitialMP;
            SQLHelper.Parameters["@InitialAGI"] = InitialAGI;
            SQLHelper.Parameters["@InitialINT"] = InitialINT;
            SQLHelper.Parameters["@InitialSTR"] = InitialSTR;
            SQLHelper.Parameters["@InitialSPD"] = InitialSPD;
            SQLHelper.Parameters["@InitialHR"] = InitialHR;
            SQLHelper.Parameters["@InitialMR"] = InitialMR;
            SQLHelper.Parameters["@InSquad"] = InSquad;
            if (TrainingTime.HasValue) SQLHelper.Parameters["@TrainingTime"] = TrainingTime;

            string sql = $"{Command_Insert} {Command_Into} {TableName} (" +
                $"{Column_CharacterId}, {Column_UserId}, {Column_Name}, {Column_FirstName}, {Column_NickName}, {Column_PrimaryAttribute}, " +
                $"{Column_InitialATK}, {Column_InitialDEF}, {Column_InitialHP}, {Column_InitialMP}, {Column_InitialAGI}, {Column_InitialINT}, " +
                $"{Column_InitialSTR}, {Column_InitialSPD}, {Column_InitialHR}, {Column_InitialMR}, {Column_InSquad}" +
                $"{(TrainingTime.HasValue ? $", {Column_TrainingTime}" : "")}) " +
                $"{Command_Values} (" +
                $"@CharacterId, @UserId, @Name, @FirstName, @NickName, @PrimaryAttribute, " +
                $"@InitialATK, @InitialDEF, @InitialHP, @InitialMP, @InitialAGI, @InitialINT, " +
                $"@InitialSTR, @InitialSPD, @InitialHR, @InitialMR, @InSquad" +
                $"{(TrainingTime.HasValue ? ", @TrainingTime" : "")})";
            return sql;
        }

        public static string Update_UserCharacter(SQLHelper SQLHelper, long Id, long CharacterId, long UserId, string Name, string FirstName, string NickName,
            int PrimaryAttribute, double InitialATK, double InitialDEF, double InitialHP, double InitialMP, double InitialAGI, double InitialINT,
            double InitialSTR, double InitialSPD, double InitialHR, double InitialMR, int InSquad, DateTime? TrainingTime)
        {
            SQLHelper.Parameters["@Id"] = Id;
            SQLHelper.Parameters["@CharacterId"] = CharacterId;
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@Name"] = Name;
            SQLHelper.Parameters["@FirstName"] = FirstName;
            SQLHelper.Parameters["@NickName"] = NickName;
            SQLHelper.Parameters["@PrimaryAttribute"] = PrimaryAttribute;
            SQLHelper.Parameters["@InitialATK"] = InitialATK;
            SQLHelper.Parameters["@InitialDEF"] = InitialDEF;
            SQLHelper.Parameters["@InitialHP"] = InitialHP;
            SQLHelper.Parameters["@InitialMP"] = InitialMP;
            SQLHelper.Parameters["@InitialAGI"] = InitialAGI;
            SQLHelper.Parameters["@InitialINT"] = InitialINT;
            SQLHelper.Parameters["@InitialSTR"] = InitialSTR;
            SQLHelper.Parameters["@InitialSPD"] = InitialSPD;
            SQLHelper.Parameters["@InitialHR"] = InitialHR;
            SQLHelper.Parameters["@InitialMR"] = InitialMR;
            SQLHelper.Parameters["@InSquad"] = InSquad;
            if (TrainingTime.HasValue) SQLHelper.Parameters["@TrainingTime"] = TrainingTime;

            string sql = $"{Command_Update} {TableName} {Command_Set} " +
                $"{Column_CharacterId} = @CharacterId, {Column_UserId} = @UserId, {Column_Name} = @Name, {Column_FirstName} = @FirstName, {Column_NickName} = @NickName, " +
                $"{Column_PrimaryAttribute} = @PrimaryAttribute, {Column_InitialATK} = @InitialATK, {Column_InitialDEF} = @InitialDEF, {Column_InitialHP} = @InitialHP, " +
                $"{Column_InitialMP} = @InitialMP, {Column_InitialAGI} = @InitialAGI, {Column_InitialINT} = @InitialINT, {Column_InitialSTR} = @InitialSTR, " +
                $"{Column_InitialSPD} = @InitialSPD, {Column_InitialHR} = @InitialHR, {Column_InitialMR} = @InitialMR, {Column_InSquad} = @InSquad" +
                $"{(TrainingTime.HasValue ? $", {Column_TrainingTime} = @TrainingTime" : "")} " +
                $"{Command_Where} {Column_Id} = @Id";
            return sql;
        }

        public static string Update_UserCharacterSquadState(SQLHelper SQLHelper, long Id, int InSquad)
        {
            SQLHelper.Parameters["@Id"] = Id;
            SQLHelper.Parameters["@InSquad"] = InSquad;
            return $"{Command_Update} {TableName} {Command_Set} {Column_InSquad} = @InSquad {Command_Where} {Column_Id} = @Id";
        }

        public static string Delete_UserCharacter(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Id} = @Id";
        }

        public static string Delete_UserCharacterByUserId(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Delete_UserCharacterByCharacterId(SQLHelper SQLHelper, long CharacterId)
        {
            SQLHelper.Parameters["@CharacterId"] = CharacterId;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_CharacterId} = @CharacterId";
        }
    }
}
