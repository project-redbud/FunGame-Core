using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;
using FunGame.Core.Model.Queue;

namespace FunGame.Core.Library.Module.Example
{
    /// <summary>
    /// 建议使用一个类来存储常量，方便重用
    /// </summary>
    public class ExampleGameModuleConstant
    {
        public const string ExampleGameModule = "fungame.example.gamemodule";
        public const string ExampleMap = "fungame.example.gamemap";
        public const string ExampleCharacter = "fungame.example.character";
        public const string ExampleSkill = "fungame.example.skill";
        public const string ExampleItem = "fungame.example.item";
    }

    /// <summary>
    /// 地图：必须继承基类：<see cref="GameMap"/><para/>
    /// </summary>
    public class ExampleGameMap : GameMap
    {
        public override string Name => ExampleGameModuleConstant.ExampleMap;

        public override string Description => "My First GameMap";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public override int Length => 12;

        public override int Width => 12;

        public override int Height => 6;

        public override float Size => 4.0f;

        public override GameMap InitGamingQueue(IGamingQueue queue)
        {
            // 因为模组在模组管理器中都是单例的，所以每次游戏都需要返回一个新的地图对象给队列
            GameMap map = new ExampleGameMap();
            map.Load();

            // 做一些绑定，以便介入游戏队列
            /// 但是，传入的 queue 可能不是 <see cref="GamingQueue"/>，要做类型检查
            // 不使用框架的实现时，需要地图作者与游戏队列的作者做好适配
            if (queue is GamingQueue gq)
            {
                gq.SelectTargetGridEvent += Gq_SelectTargetGrid;
            }

            return map;
        }

        private Grid Gq_SelectTargetGrid(GamingQueue queue, Character character, List<Character> enemys, List<Character> teammates, GameMap map, List<Grid> canMoveGrids)
        {
            // 介入选择，假设这里更新界面，让玩家选择目的地
            return Grid.Empty;
        }
    }

    /// <summary>
    /// 角色：必须继承基类：<see cref="CharacterModule"/><para/>
    /// </summary>
    public class ExampleCharacterModule : CharacterModule
    {
        public override string Name => ExampleGameModuleConstant.ExampleCharacter;

        public override string Description => "My First CharacterModule";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public override Dictionary<string, Character> Characters
        {
            get
            {
                Dictionary<string, Character> dict = [];
                // 构建一个你想要的角色
                Character c = new()
                {
                    Name = "Oshima",
                    FirstName = "Shiya",
                    NickName = "OSM",
                    MagicType = MagicType.PurityNatural,
                    InitialHP = 30,
                    InitialSTR = 20,
                    InitialAGI = 10,
                    InitialINT = 5,
                    InitialATK = 100,
                    InitialDEF = 10
                };
                dict.Add(c.Name, c);
                return dict;
            }
        }

        protected override Factory.EntityFactoryDelegate<Character> CharacterFactory()
        {
            // 上面示例用 Characters 是预定义的
            // 这里的工厂模式则是根据传进来的参数定制生成角色，只要重写这个方法就能注册工厂了
            return (id, name, args) =>
            {
                return null;
            };
        }

        public static Character CreateCharacter(long id, string name, Dictionary<string, object> args)
        {
            // 注册工厂后，后续创建角色只需要这样调用
            return Factory.OpenFactory.GetInstance<Character>(id, name, args);
        }
    }

    /// <summary>
    /// 技能：必须继承基类：<see cref="SkillModule"/><para/>
    /// </summary>
    public class ExampleSkillModule : SkillModule
    {
        public override string Name => ExampleGameModuleConstant.ExampleSkill;

        public override string Description => "My First SkillModule";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public override Dictionary<string, Skill> Skills
        {
            get
            {
                Dictionary<string, Skill> dict = [];
                /// 技能应该在新建类继承Skill实现，再自行构造并加入此列表。
                /// 技能的实现示例参见：<see cref="ExampleSkill"/>
                return dict;
            }
        }

        protected override Factory.EntityFactoryDelegate<Skill> SkillFactory()
        {
            // 注册一个工厂，根据id和name，返回一个你继承实现了的类对象。所有的工厂使用方法参考 Character，都是一样的
            return (id, name, args) =>
            {
                return null;
            };
        }

        protected override Factory.EntityFactoryDelegate<Effect> EffectFactory()
        {
            return (id, name, args) =>
            {
                // 以下是一个示例，实际开发中 id,name,args 怎么处置，看你心情
                Skill? skill = null;
                if (args.TryGetValue("skill", out object? value) && value is Skill s)
                {
                    skill = s;
                }
                skill ??= new OpenSkill(id, name, args);
                /// 如 <see cref="ExampleOpenItemByJson"/> 中所说，特效需要在工厂中注册，方便重用
                if (id == 1001)
                {
                    return new ExampleOpenEffectExATK2(skill, args);
                }
                return null;
            };
        }
    }

    /// <summary>
    /// 物品：必须继承基类：<see cref="ItemModule"/><para/>
    /// </summary>
    public class ExampleItemModule : ItemModule
    {
        public override string Name => ExampleGameModuleConstant.ExampleItem;

        public override string Description => "My First ItemModule";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public override Dictionary<string, Item> Items
        {
            get
            {
                Dictionary<string, Item> dict = [];
                /// 物品应该新建类继承Item实现，再自行构造并加入此列表。
                /// 物品的实现示例参见：<see cref="ExampleItem"/>
                return dict;
            }
        }

        protected override Factory.EntityFactoryDelegate<Item> ItemFactory()
        {
            // 注册一个工厂，根据id和name，返回一个你继承实现了的类对象。
            return (id, name, args) =>
            {
                return null;
            };
        }
    }
}
