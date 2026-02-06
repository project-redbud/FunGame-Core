using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Addon.Example
{
    public class ExampleItem : Item
    {
        public override long Id => 1;
        public override string Name => "ExampleItem";
        public override string Description => $"{Skills.Passives.First().Description}{Skills.Passives.Last().Name}：{Skills.Passives.Last().Description}";
        public override string BackgroundStory => "Item's Background Story";
        public override QualityType QualityType => QualityType.Gold;
        public override WeaponType WeaponType => WeaponType.Staff;

        public ExampleItem(Character? character = null) : base(ItemType.Weapon)
        {
            // 如果属性不支持重写，可以在构造函数中初始化
            Price = 0;
            IsSellable = false;
            IsTradable = false;
            IsLock = true;
            // 作为装备物品，添加被动技能是必要的。技能一定要设置等级大于0，否则不会生效
            Skills.Passives.Add(new ExampleItemSkill(character, this));
            Skills.Passives.Add(new ExamplePassiveSkill(character)
            {
                Level = 1
            });
            // 也可以添加主动技能
            Skills.Active = new ExampleNonDirectionalSkill2(character)
            {
                Level = 4
            };
        }
    }

    public class ExampleItemSkill : Skill
    {
        public override long Id => 6;
        public override string Name => "ExampleItemSkill";
        public override string Description => string.Join("", Effects.Select(e => e.Description));

        private readonly double 攻击力加成 = 0.46;

        public ExampleItemSkill(Character? character = null, Item? item = null) : base(SkillType.Passive, character)
        {
            Level = 1;
            Item = item;
            Dictionary<string, object> values = new()
            {
                { "exatk", 攻击力加成 }
            };
            Effects.Add(new ExampleOpenEffectExATK2(this, values, character));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class ExampleOpenEffectExATK2 : Effect
    {
        public override long Id => 1001; // 赋予独特ID可以方便重用（在SkillModule的工厂方法中注册有利于动态创建）
        public override string Name { get; set; } = "攻击力加成";
        public override string Description => $"{(ActualBonus >= 0 ? "增加" : "减少")}角色 {Math.Abs(BonusFactor) * 100:0.##}% [ {(ActualBonus == 0 ? "基于基础攻击力" : $"{Math.Abs(ActualBonus):0.##}")} ] 点攻击力。" + (Source != null && (Skill.Character != Source || Skill is not OpenSkill) ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : (Skill is OpenSkill ? "" : $" 的 [ {Skill.Name} ]")) : "");
        public double Value => ActualBonus;

        private readonly double BonusFactor = 0;
        private double ActualBonus = 0;

        public override void OnEffectGained(Character character)
        {
            if (Durative && RemainDuration == 0)
            {
                RemainDuration = Duration;
            }
            else if (RemainDurationTurn == 0)
            {
                RemainDurationTurn = DurationTurn;
            }
            ActualBonus = character.BaseATK * BonusFactor;
            character.ExATKPercentage += BonusFactor;
        }

        public override void OnEffectLost(Character character)
        {
            character.ExATKPercentage -= BonusFactor;
        }

        public override void OnAttributeChanged(Character character)
        {
            // 刷新加成
            OnEffectLost(character);
            OnEffectGained(character);
        }

        public ExampleOpenEffectExATK2(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            EffectType = EffectType.Item;
            GamingQueue = skill.GamingQueue;
            Source = source;
            if (Values.Count > 0)
            {
                // 如果希望技能可以动态读取参数和创建，就这么写
                string key = Values.Keys.FirstOrDefault(s => s.Equals("exatk", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                if (key.Length > 0 && double.TryParse(Values[key].ToString(), out double exATK))
                {
                    BonusFactor = exATK;
                }
            }
        }
    }

    public class ExampleOpenItemByJson
    {
        public static Item CreateAJsonItem()
        {
            // 演示使用JSON动态创建物品
            string json = @"
            {
                ""Id"": 10001,
                ""Name"": ""木杖"",
                ""Description"": ""增加角色 20 点攻击力。"",
                ""BackgroundStory"": ""魔法使的起点。"",
                ""ItemType"": 1,
                ""WeaponType"": 8,
                ""QualityType"": 0,
                ""Skills"": {
                  ""Active"": null,
                  ""Passives"": [
                    {
                      ""Id"": 2001,
                      ""Name"": ""木杖"",
                      ""SkillType"": 3,
                      ""Effects"": [
                        {
                          ""Id"": 1001,
                          ""exatk"": 20
                        }
                      ]
                    }
                  ]
                }
            }";
            /// 如果想了解JSON结构，参见<see cref="JsonConverter.ItemConverter"/>和<see cref="JsonConverter.SkillConverter"/>

            /// 属性和值解释：
            /// Active 的值是一个技能对象；Passives 则是一个技能对象的数组
            /// 这里的技能是是动态创建的，Id可以随便填一个没有经过编码的
            /// SkillType = 3 代表枚举 <see cref="SkillType.Passive"/>
            /// Effects 中传入一个特效对象的数组，其JSON结构参见<see cref="JsonConverter.EffectConverter"/>

            /// 通常，框架要求所有的特效都要有一个编码的类并经过工厂注册，这样才能正常的动态构建对象
            /// 没有在转换器上出现的属性，都会进入 <see cref="Effect.Values"/> 字典，特效可以自行解析，就像上面的 ExATK2 一样

            /// 如果1001这个特效已经过工厂注册，那么它的工作流程如下：
            Skill skill = new OpenSkill(2001, "木杖", []);
            Effect effect = Factory.OpenFactory.GetInstance<Effect>(1001, "", new()
            {
                { "skill", skill },
                { "exatk", 20 }
            });
            skill.Effects.Add(effect);
            Item item = new OpenItem(10001, "木杖", [])
            {
                Description = "增加角色 20 点攻击力。",
                BackgroundStory = "魔法使的起点。",
                WeaponType = WeaponType.Staff,
                QualityType = QualityType.White,
            };
            item.Skills.Passives.Add(skill);

            /// 以下代码等效于上述代码
            item = NetworkUtility.JsonDeserialize<Item>(json) ?? Factory.GetItem();

            /// 如果你有一个JSON文件专门用来定义这些动态物品，可以这样加载
            /// 此方法使用 <see cref="EntityModuleConfig{T}"/> 配置文件读取器
            Dictionary<string, Item> exItems = Factory.GetGameModuleInstances<Item>("module_name", "file_name");
            if (exItems.Count > 0)
            {
                item = exItems.Values.First();
            }

            /// 不止物品，角色和技能都支持动态创建，在工厂中注册较为关键，既能享受动态数值，也能享受编码的具体逻辑
            return item;
        }
    }
}
