using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.SQLScript.Entity
{
    public class OffersQuery : Constant
    {
        public const string TableName = "Offers";
        public const string Column_Id = "Id";
        public const string Column_Offeror = "Offeror";
        public const string Column_Offeree = "Offeree";
        public const string Column_CreateTime = "CreateTime";
        public const string Column_FinishTime = "FinishTime";
        public const string Column_Status = "Status";
        public const string Column_NegotiatedTimes = "NegotiatedTimes";

        public const string Select_Offers = $"{Command_Select} {Command_All} {Command_From} {TableName}";

        public static string Select_OfferById(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Select_Offers} {Command_Where} {Column_Id} = @Id";
        }

        public static string Select_OffersByOfferor(SQLHelper SQLHelper, long Offeror)
        {
            SQLHelper.Parameters["@Offeror"] = Offeror;
            return $"{Select_Offers} {Command_Where} {Column_Offeror} = @Offeror";
        }

        public static string Select_OffersByOfferee(SQLHelper SQLHelper, long Offeree)
        {
            SQLHelper.Parameters["@Offeree"] = Offeree;
            return $"{Select_Offers} {Command_Where} {Column_Offeree} = @Offeree";
        }

        public static string Insert_Offer(SQLHelper SQLHelper, long Offeror, long Offeree, OfferState Status, int NegotiatedTimes)
        {
            SQLHelper.Parameters["@Offeror"] = Offeror;
            SQLHelper.Parameters["@Offeree"] = Offeree;
            SQLHelper.Parameters["@Status"] = (int)Status;
            SQLHelper.Parameters["@CreateTime"] = DateTime.Now;
            SQLHelper.Parameters["@NegotiatedTimes"] = NegotiatedTimes;

            return $"{Command_Insert} {Command_Into} {TableName} ({Column_Offeror}, {Column_Offeree}, {Column_Status}, {Column_CreateTime}), {Column_NegotiatedTimes}) " +
                   $"{Command_Values} (@Offeror, @Offeree, @Status, @CreateTime, @NegotiatedTimes)";
        }

        public static string Update_OfferStatus(SQLHelper SQLHelper, long Id, OfferState Status)
        {
            SQLHelper.Parameters["@Id"] = Id;
            SQLHelper.Parameters["@Status"] = (int)Status;
            return $"{Command_Update} {TableName} {Command_Set} {Column_Status} = @Status {Command_Where} {Column_Id} = @Id";
        }

        public static string Update_OfferNegotiatedTimes(SQLHelper SQLHelper, long Id, int NegotiatedTimes)
        {
            SQLHelper.Parameters["@Id"] = Id;
            SQLHelper.Parameters["@NegotiatedTimes"] = NegotiatedTimes;
            return $"{Command_Update} {TableName} {Command_Set} {Column_NegotiatedTimes} = @NegotiatedTimes {Command_Where} {Column_Id} = @Id";
        }

        public static string Update_OfferFinishTime(SQLHelper SQLHelper, long Id, DateTime FinishTime)
        {
            SQLHelper.Parameters["@Id"] = Id;
            SQLHelper.Parameters["@FinishTime"] = FinishTime;
            return $"{Command_Update} {TableName} {Command_Set} {Column_FinishTime} = @FinishTime {Command_Where} {Column_Id} = @Id";
        }

        public static string Delete_Offer(SQLHelper SQLHelper, long Id)
        {
            SQLHelper.Parameters["@Id"] = Id;
            return $"{Command_Delete} {Command_From} {TableName} {Command_Where} {Column_Id} = @Id";
        }
    }
}
