using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Tests.Entities;

public class TradeAndExploreTests
{
    [Fact]
    public void Market_AddItemMergesDuplicatesByListedItem()
    {
        User seller = new(1001, "甲");
        Item item = EntitiesHelper.NewWeapon("法杖");
        Market market = new("集市");

        market.AddItem(seller, item, 100, 2);

        MarketItem listed = Assert.Single(market.MarketItems.Values);
        Assert.Equal(2, listed.Stock);
        Assert.Equal(100d, listed.Price, 3);
        Assert.Equal(1001L, listed.User);
        Assert.Equal(MarketItemState.Listed, listed.Status);

        market.AddItem(seller, item, 100, 3);

        listed = Assert.Single(market.MarketItems.Values);
        Assert.Equal(5, listed.Stock);
    }

    [Fact]
    public void Store_AddItemCreatesGoodsAndResolvesPrice()
    {
        Store store = new("商店");
        store.AddItem(EntitiesHelper.NewWeapon("法杖"), 5, "法杖", "一把法杖");

        Goods goods = Assert.Single(store.Goods.Values);
        Assert.Equal(5, goods.Stock);

        store.SetPrice(goods.Id, "金币", 250);

        Assert.True(store.GetPrice(goods.Id, "金币", out double price));
        Assert.Equal(250d, price, 3);
        Assert.Equal(250d, store.GetPrice(goods.Id), 3);
    }

    [Fact]
    public void Goods_PriceLookupIsCaseInsensitiveOnMissingCurrency()
    {
        Goods goods = new(1, EntitiesHelper.NewWeapon("法杖"), 3, "法杖", "");

        Assert.False(goods.GetPrice("金币", out _));

        goods.SetPrice("金币", 30);
        Assert.True(goods.GetPrice("金币", out double price));
        Assert.Equal(30d, price, 3);

        goods.SetPrice("金币", -5);
        Assert.Equal(30d, price, 3);
    }

    [Fact]
    public void Offer_StartsWithCreatedState()
    {
        Offer offer = new() { Id = 7, Offeror = 1001, Offeree = 1002 };

        Assert.Equal(OfferState.Created, offer.Status);
        Assert.Equal(0, offer.NegotiatedTimes);
        Assert.True(offer.Equals(new Offer { Id = 7 }));
    }

    [Fact]
    public void Region_ChangeWeatherRequiresKnownWeather()
    {
        Region region = new() { Name = "平原", Weather = "晴天" };

        Assert.False(region.ChangeWeather("暴雨"));

        region.Weathers["暴雨"] = 1;

        Assert.True(region.ChangeWeather("暴雨"));
        Assert.Equal("暴雨", region.Weather);
    }

    [Fact]
    public void Region_ChangeRandomWeatherNeedsWeatherTable()
    {
        Region region = new() { Name = "平原" };

        Assert.False(region.ChangeRandomWeather());

        region.Weathers["晴天"] = 1;
        Assert.True(region.ChangeRandomWeather());
        Assert.Contains(region.Weather, region.Weathers.Keys);
    }

    [Fact]
    public void Activity_TracksAwardedUsersPerQuest()
    {
        Activity activity = new(1, "活动");
        Quest quest = new() { Id = 11, Name = "任务" };
        Quest outsider = new() { Id = 12, Name = "场外任务" };

        Assert.False(activity.RegisterAwardedUser(1001, quest));
        Assert.False(activity.RegisterAwardedUser(1001, outsider));

        activity.Quests.Add(quest);

        Assert.True(activity.RegisterAwardedUser(1001, quest));
        Assert.True(activity.HasUserAwarded(1001, quest));
        Assert.True(activity.HasUserAwarded(1001, 11L));
        Assert.False(activity.HasUserAwarded(1002, quest));
        Assert.Contains(1001L, activity.QuestsAwardedUsers[quest.Id]);
    }

    [Fact]
    public void Activity_AwardTimeIsSevenDaysAfterEnd()
    {
        Activity activity = new(1, "活动", null, new DateTime(2026, 1, 1));

        Assert.Equal(new DateTime(2026, 1, 8), activity.EndAwardedTime);
        Assert.Contains("活动", activity.ToString(false));
    }

    [Fact]
    public void Quest_ExposesAwardProgressAndString()
    {
        Quest quest = new() { Id = 1, Name = "任务", CreditsAward = 100 };
        quest.Awards.Add(EntitiesHelper.NewWeapon("奖励"));

        Assert.Equal(0, quest.Progress);
        Assert.Equal(100, quest.MaxProgress);
        Assert.Equal(QuestState.NotStarted, quest.Status);
        Assert.Contains("奖励", quest.AwardsString);
    }

    [Fact]
    public void InteractionPoint_ComparesByIdName()
    {
        InteractionPoint first = new() { Id = 1, Name = "矿脉", CustomValue = 5 };
        InteractionPoint second = new() { Id = 1, Name = "矿脉" };

        Assert.True(first.Equals(second));
        Assert.Equal(5, first.CustomValue);
    }
}
