using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class Factory
   {
        /// <summary>
        /// 获取一个可能为NULL的实例
        /// </summary>
        /// <typeparam name="T">Entity类</typeparam>
        /// <param name="objs">构造函数的参数</param>
        /// <returns></returns>
        public static T? GetInstance<T>(params object[]? objs)
        {
            if (!IsEntity<T>()) return default;
            object? instance = null;
            if (objs is null || objs.Length == 0) return (T?)instance;
            if (typeof(T) == typeof(Entity.User))
            {
                instance = Api.Factory.UserFactory.GetInstance("Mili");
            }
            else if (typeof(T) == typeof(Skill))
            {

            }
            return (T?)instance;
        }

        /// <summary>
        /// 获取一个不可能为NULL的实例
        /// Item默认返回PassiveItem
        /// Skill默认返回PassiveSkill
        /// 若无法找到T，返回唯一的空对象
        /// </summary>
        /// <typeparam name="T">Entity类</typeparam>
        /// <param name="objs">构造函数的参数</param>
        /// <returns></returns>
        public static T New<T>(params object[]? objs)
        {
            object instance = General.EntityInstance;
            if (!IsEntity<T>()) return (T)instance;
            if (objs is null || objs.Length == 0) return (T)instance;
            if (typeof(T) == typeof(Entity.User))
            {
                instance = Api.Factory.UserFactory.GetInstance("Mili");
            }
            else if (typeof(T) == typeof(Skill))
            {

            }
            return (T)instance;
        }

        /// <summary>
        /// 获取一个不可能为NULL的单例
        /// Item默认返回PassiveItem
        /// Skill默认返回PassiveSkill
        /// 若无法找到T，返回唯一的空对象
        /// </summary>
        /// <typeparam name="T">Entity类</typeparam>
        /// <param name="objs">构造函数的参数</param>
        /// <returns></returns>
        public static object NewSingle<T>(params object[]? objs)
        {
            object instance = General.EntityInstance;
            if (!IsEntity<T>()) return instance;
            if (objs is null || objs.Length == 0) return instance;
            if (typeof(T) == typeof(Entity.User))
            {
                instance = Api.Factory.UserFactory.GetInstance("Mili");
            }
            else if (typeof(T) == typeof(Skill))
            {

            }
            Singleton.Add(instance);
            return instance;
        }

        private static bool IsEntity<T>()
        {
            if (typeof(T) == typeof(Entity.ActiveItem) || typeof(T) == typeof(ActiveSkill)
                || typeof(T) == typeof(Entity.Character) || typeof(T) == typeof(Entity.CharacterStatistics)
                || typeof(T) == typeof(Entity.GameStatistics) || typeof(T) == typeof(Inventory)
                || typeof(T) == typeof(Entity.Item) || typeof(T) == typeof(Entity.PassiveItem)
                || typeof(T) == typeof(PassiveSkill) || typeof(T) == typeof(Entity.Room)
                || typeof(T) == typeof(Skill) || typeof(T) == typeof(Entity.User)
                || typeof(T) == typeof(Entity.UserStatistics))
                return true;
            return false;
        }
    }
}
