using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class MarketItem : BaseEntity
    {
        public User User { get; set; }
        public Item Item { get; set; }
        public double Price { get; set; } = 0;
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime? FinishTime { get; set; } = null;
        public MarketItemState Status { get; set; } = MarketItemState.Listed;
        public User? Buyer { get; set; } = null;

        public override bool Equals(IBaseEntity? other)
        {
            return other is MarketItem && other?.Id == Id;
        }

        public MarketItem()
        {
            User = Factory.GetUser();
            Item = Factory.GetItem();
        }
    }
}
