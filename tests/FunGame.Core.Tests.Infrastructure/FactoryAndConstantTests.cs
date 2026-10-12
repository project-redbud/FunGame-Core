using FunGame.Core;
using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Common.JsonConverter;
using FunGame.Core.Library.Constant;
using FunGame.Core.Library.Exception;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Infrastructure;

public class FactoryTests
{
    [Fact]
    public void GetInstance_WithoutRegistrationFallsBackToDefaultTypes()
    {
        Character character = Factory.OpenFactory.GetInstance<Character>(1, "甲", []);
        Item item = Factory.OpenFactory.GetInstance<Item>(2, "物品", []);
        Skill skill = Factory.OpenFactory.GetInstance<Skill>(3, "技能", []);

        Assert.IsType<Character>(character);
        Assert.IsType<OpenItem>(item);
        Assert.IsType<OpenSkill>(skill);
        Assert.Equal("物品", item.Name);
        Assert.Equal("技能", skill.Name);
    }

    [Fact]
    public void GetInstance_RejectsEffectWithoutDedicatedFactory()
    {
        Assert.Throws<NotSupportedInstanceClassException>(() => Factory.OpenFactory.GetInstance<Effect>(1, "特效", []));
    }

    [Fact]
    public void RegisterFactory_TakesPrecedenceOverFallback()
    {
        Skill skill = new OpenSkill(2001, "宿主", []);
        Factory.EffectFactoryDelegate custom = (id, name, owner, args) => id == 999999 ? new RegisteredEffect(owner) : null;

        Factory.OpenFactory.RegisterFactory(custom);
        try
        {
            Assert.IsType<RegisteredEffect>(Factory.OpenFactory.GetInstance(999999, "", skill, []));
            Assert.IsType<Effect>(Factory.OpenFactory.GetInstance(1001, "", skill, []));
        }
        finally
        {
            Factory.OpenFactory.UnRegisterFactory(custom);
        }

        Assert.IsType<Effect>(Factory.OpenFactory.GetInstance(999999, "", skill, []));
    }

    [Fact]
    public void RegisterFactory_SupportsEntityDelegates()
    {
        Factory.EntityFactoryDelegate<Item> custom = (id, name, args) => new OpenItem(id, $"[定制]{name}", args) { IsTradable = false };

        Factory.OpenFactory.RegisterFactory(custom);
        try
        {
            Item item = Factory.OpenFactory.GetInstance<Item>(5, "物品", []);
            Assert.Equal("[定制]物品", item.Name);
            Assert.False(item.IsTradable);
        }
        finally
        {
            Factory.OpenFactory.UnRegisterFactory(custom);
        }
    }

    [Fact]
    public void OpenFactory_IsASingleton()
    {
        Assert.Same(Factory.OpenFactory, Factory.OpenFactory);
        Assert.Same(Factory.OpenFactory, FunGame.Core.Library.Constant.General.OpenFactory);
    }

    private sealed class RegisteredEffect(Skill skill) : Effect(skill);
}

public class JsonToolRegistrationTests
{
    [Fact]
    public void JsonTool_BuildsIndependentOptionsPerInstance()
    {
        JsonTool first = new();
        JsonTool second = new();

        Assert.NotSame(first.Options, second.Options);
        Assert.Same(JsonService.GeneralOptions, JsonTool.JsonSerializerOptions);
    }
}

public class ConstantSetTests
{
    [Fact]
    public void SkillSet_ResolvesTypeNamesAndLevelCaps()
    {
        EquilibriumConstant constants = Library.Constant.General.GameplayEquilibriumConstant;

        Assert.Equal(constants.MaxSkillLevel, SkillSet.GetSkillMaxLevel(SkillType.Skill, constants));
        Assert.Equal(constants.MaxSuperSkillLevel, SkillSet.GetSkillMaxLevel(SkillType.SuperSkill, constants));
        Assert.Equal(constants.MaxPassiveSkillLevel, SkillSet.GetSkillMaxLevel(SkillType.Passive, constants));

        foreach (SkillType type in Enum.GetValues<SkillType>())
        {
            string name = SkillSet.GetSkillTypeName(type);
            Assert.False(string.IsNullOrWhiteSpace(name));
            Assert.Equal(type, SkillSet.GetSkillTypeFromName(name));
        }
    }

    [Fact]
    public void ItemSet_RoundTripsItemWeaponAndSlotNames()
    {
        foreach (ItemType type in Enum.GetValues<ItemType>())
        {
            Assert.Equal(type, ItemSet.GetItemTypeFromName(ItemSet.GetItemTypeName(type)));
        }

        foreach (WeaponType type in Enum.GetValues<WeaponType>())
        {
            Assert.Equal(type, ItemSet.GetWeaponTypeFromName(ItemSet.GetWeaponTypeName(type)));
        }

        foreach (EquipSlotType type in Enum.GetValues<EquipSlotType>())
        {
            Assert.Equal(type, ItemSet.GetEquipSlotTypeFromName(ItemSet.GetEquipSlotTypeName(type)));
        }
    }

    [Fact]
    public void RoomAndCharacterSets_ExposeReadableNames()
    {
        Assert.Equal(RoomType.Team, RoomSet.GetRoomType(RoomSet.Team));
        Assert.False(string.IsNullOrWhiteSpace(RoomSet.GetTypeString(RoomType.FastAuto)));

        Assert.False(string.IsNullOrWhiteSpace(CharacterSet.GetDamageTypeName(DamageType.Physical, MagicType.None)));
        Assert.False(string.IsNullOrWhiteSpace(CharacterSet.GetMagicResistanceName(MagicType.Bright)));
        Assert.False(string.IsNullOrWhiteSpace(CharacterSet.GetCharacterState(CharacterState.NotActionable)));
        Assert.False(string.IsNullOrWhiteSpace(CharacterSet.GetImmuneTypeName(ImmuneType.All)));
    }

    [Fact]
    public void FunGameInfo_ExposesVersionAndInfo()
    {
        Assert.False(string.IsNullOrWhiteSpace(FunGameInfo.FunGame_Version));
        Assert.False(string.IsNullOrWhiteSpace(FunGameInfo.GetInfo(FunGameInfo.FunGame.FunGame_Core)));
    }
}

public class ExceptionTests
{
    [Fact]
    public void FrameworkExceptions_CarryErrorCodes()
    {
        Assert.Contains("#10001", new SystemError().Message);
        Assert.Contains("#10038", new AddonInvalidException("addon.dll").Message);

        NotSupportedInstanceClassException unsupported =
            Assert.Throws<NotSupportedInstanceClassException>(() => Factory.OpenFactory.GetInstance<Effect>(1, "特效", []));
        Assert.Contains("#10037", unsupported.Message);
    }

    [Fact]
    public void ExceptionExtension_WrapsInnerException()
    {
        System.Exception inner = new InvalidOperationException("内层错误");

        string info = new Exception("外层错误", inner).GetErrorInfo();

        Assert.Contains("InnerExceoption", info);
        Assert.Contains("内层错误", info);

        Assert.DoesNotContain("InnerExceoption", new Exception("无内层").GetErrorInfo());
    }
}
