using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class UserProfilesQuery : Constant
    {
        public const string TableName = "UserProfiles";
        public const string Column_UserId = "UserId";
        public const string Column_AvatarUrl = "AvatarUrl";
        public const string Column_Signature = "Signature";
        public const string Column_Gender = "Gender";
        public const string Column_BirthDay = "BirthDay";
        public const string Column_Followers = "Followers";
        public const string Column_Following = "Following";
        public const string Column_Title = "Title";
        public const string Column_UserGroup = "UserGroup";

        public const string Select_UserProfiles = $"{Command_Select} {Command_All} {Command_From} {TableName}";

        public static string Select_UserProfileByUserId(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Select_UserProfiles} {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Insert_UserProfile(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@BirthDay"] = General.DefaultTime;

            return $"{Command_Insert} {Command_Into} {TableName} ({Column_UserId}, {Column_BirthDay}) " +
                   $"{Command_Values} (@UserId, @BirthDay)";
        }

        public static string Update_UserProfile(SQLHelper SQLHelper, long UserId, string AvatarUrl, string Signature, string Gender, DateTime BirthDay, int Followers, int Following, string Title, string UserGroup)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@AvatarUrl"] = AvatarUrl;
            SQLHelper.Parameters["@Signature"] = Signature;
            SQLHelper.Parameters["@Gender"] = Gender;
            SQLHelper.Parameters["@BirthDay"] = BirthDay;
            SQLHelper.Parameters["@Followers"] = Followers;
            SQLHelper.Parameters["@Following"] = Following;
            SQLHelper.Parameters["@Title"] = Title;
            SQLHelper.Parameters["@UserGroup"] = UserGroup;

            return $"{Command_Update} {TableName} {Command_Set} {Column_AvatarUrl} = @AvatarUrl, {Column_Signature} = @Signature, {Column_Gender} = @Gender, {Column_BirthDay} = @BirthDay, " +
                   $"{Column_Followers} = @Followers, {Column_Following} = @Following, {Column_Title} = @Title, {Column_UserGroup} = @UserGroup {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Update_UserProfileAvatarUrl(SQLHelper SQLHelper, long UserId, string AvatarUrl)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@AvatarUrl"] = AvatarUrl;
            return $"{Command_Update} {TableName} {Command_Set} {Column_AvatarUrl} = @AvatarUrl {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Update_UserProfileSignature(SQLHelper SQLHelper, long UserId, string Signature)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@Signature"] = Signature;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Signature} = @Signature {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Update_UserProfileGender(SQLHelper SQLHelper, long UserId, string Gender)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@Gender"] = Gender;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Gender} = @Gender {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Update_UserProfileBirthDay(SQLHelper SQLHelper, long UserId, DateTime BirthDay)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@BirthDay"] = BirthDay;
            return $"{Command_Update} {TableName} {Command_Set} {Column_BirthDay} = @BirthDay {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Update_UserProfileFollowers(SQLHelper SQLHelper, long UserId, int Followers)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@Followers"] = Followers;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Followers} = @Followers {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Update_UserProfileFollowing(SQLHelper SQLHelper, long UserId, int Following)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@Following"] = Following;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Following} = @Following {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Update_UserProfileTitle(SQLHelper SQLHelper, long UserId, string Title)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@Title"] = Title;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Title} = @Title {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Update_UserProfileUserGroup(SQLHelper SQLHelper, long UserId, string UserGroup)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            SQLHelper.Parameters["@UserGroup"] = UserGroup;
            return $"{Command_Update} {TableName} {Command_Set} {Column_UserGroup} = @UserGroup {Command_Where} {Column_UserId} = @UserId";
        }

        public static string Delete_UserProfile(SQLHelper SQLHelper, long UserId)
        {
            SQLHelper.Parameters["@UserId"] = UserId;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_UserId} = @UserId";
        }
    }
}
