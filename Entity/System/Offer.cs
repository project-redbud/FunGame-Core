using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Offer : BaseEntity
    {
        public long Offeror { get; set; } = 0;
        public long Offeree { get; set; } = 0;
        public HashSet<Item> OfferorItems { get; set; } = [];
        public HashSet<Item> OffereeItems { get; set; } = [];
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime? FinishTime { get; set; } = null;
        public OfferState Status { get; set; } = OfferState.Created;
        public int NegotiatedTimes { get; set; } = 0;

        public override bool Equals(IBaseEntity? other)
        {
            return other is Offer && other?.Id == Id;
        }
    }
}
