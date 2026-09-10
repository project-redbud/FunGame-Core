using FunGame.Core.Entity;

namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 职业内容注册表：把「职业 / 流派 / 转换战斗天赋战技」的 IdName 映射到创建工厂
    /// <para/>用途：存档只记录 IdName 与状态，读档时由本表重建定义副本（模组启动时注册一次即可）
    /// <para/>未注册的 IdName 在重建时会失败并上报，不会抛异常
    /// </summary>
    public static class ClassDefinitionRegistry
    {
        private static readonly Dictionary<string, Func<Class>> _classFactories = [];
        private static readonly Dictionary<string, Func<Class, SubClass>> _subClassFactories = [];
        private static readonly Dictionary<string, Func<Skill>> _switchSkillFactories = [];
        private static readonly Lock _lock = new();

        /// <summary>
        /// 注册职业定义工厂（可选同时注册其流派工厂）
        /// </summary>
        /// <param name="classFactory">职业定义工厂，返回新实例</param>
        /// <param name="subClassFactory">流派定义工厂，入参为所属职业</param>
        public static void Register(Func<Class> classFactory, Func<Class, SubClass>? subClassFactory = null)
        {
            ArgumentNullException.ThrowIfNull(classFactory);
            string classKey;
            using (_lock.EnterScope())
            {
                classKey = classFactory().GetIdName();
                _classFactories[classKey] = classFactory;
            }
            if (subClassFactory != null)
            {
                string subKey = subClassFactory(classFactory()).GetIdName();
                using (_lock.EnterScope())
                {
                    _subClassFactories[subKey] = subClassFactory;
                }
            }
        }

        /// <summary>
        /// 注册【转换战斗天赋】战技工厂（key = 技能 IdName）
        /// </summary>
        public static void RegisterSwitchSkill(string idName, Func<Skill> factory)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(idName);
            ArgumentNullException.ThrowIfNull(factory);
            using (_lock.EnterScope())
            {
                _switchSkillFactories[idName] = factory;
            }
        }

        /// <summary>
        /// 按 IdName 创建职业定义；未注册返回 null
        /// </summary>
        public static Class? CreateClass(string idName)
        {
            using (_lock.EnterScope())
            {
                return _classFactories.TryGetValue(idName, out Func<Class>? factory) ? factory() : null;
            }
        }

        /// <summary>
        /// 按 IdName 创建流派定义（绑定到 <paramref name="owner"/>）；未注册返回 null
        /// </summary>
        public static SubClass? CreateSubClass(string idName, Class owner)
        {
            using (_lock.EnterScope())
            {
                return _subClassFactories.TryGetValue(idName, out Func<Class, SubClass>? factory) ? factory(owner) : null;
            }
        }

        /// <summary>
        /// 按 IdName 创建【转换战斗天赋】战技；未注册返回 null
        /// </summary>
        public static Skill? CreateSwitchSkill(string idName)
        {
            using (_lock.EnterScope())
            {
                return _switchSkillFactories.TryGetValue(idName, out Func<Skill>? factory) ? factory() : null;
            }
        }

        /// <summary>
        /// 已注册的职业 IdName 列表
        /// </summary>
        public static IReadOnlyList<string> RegisteredClassIds
        {
            get
            {
                using (_lock.EnterScope())
                {
                    return [.. _classFactories.Keys];
                }
            }
        }

        /// <summary>
        /// 已注册的流派 IdName 列表
        /// </summary>
        public static IReadOnlyList<string> RegisteredSubClassIds
        {
            get
            {
                using (_lock.EnterScope())
                {
                    return [.. _subClassFactories.Keys];
                }
            }
        }

        /// <summary>
        /// 清空注册（测试隔离用）
        /// </summary>
        public static void Clear()
        {
            using (_lock.EnterScope())
            {
                _classFactories.Clear();
                _subClassFactories.Clear();
                _switchSkillFactories.Clear();
            }
        }
    }
}
