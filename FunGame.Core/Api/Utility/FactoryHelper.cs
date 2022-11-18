using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class FactoryHelper
    {
        /// <summary>
        /// 获取一个可能为NULL的实例
        /// </summary>
        /// <typeparam name="T">Entity类</typeparam>
        /// <param name="objs">构造函数的参数</param>
        /// <returns></returns>
        public static object? GetInstance<T>(params object[]? objs)
        {
            if (!IsEntity<T>()) return null;
            object? instance = null;
            if (objs is null || objs.Length == 0) return instance;
            if (typeof(T) == typeof(Entity.General.User))
            {
                instance = Factory.UserFactory.GetInstance("Mili");
            }
            else if (typeof(T) == typeof(Entity.General.Skill))
            {

            }
            return instance;
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
        public static object New<T>(params object[]? objs)
        {
            object instance = General.EntityInstance;
            if (!IsEntity<T>()) return instance;
            if (objs is null || objs.Length == 0) return instance;
            if (typeof(T) == typeof(Entity.General.User))
            {
                instance = Factory.UserFactory.GetInstance("Mili");
            }
            else if (typeof(T) == typeof(Entity.General.Skill))
            {

            }
            return instance;
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
            if (typeof(T) == typeof(Entity.General.User))
            {
                instance = Factory.UserFactory.GetInstance("Mili");
            }
            else if (typeof(T) == typeof(Entity.General.Skill))
            {

            }
            Singleton.Add(instance);
            return instance;
        }

        private static bool IsEntity<T>()
        {
            if (typeof(T) == typeof(Entity.General.ActiveItem) || typeof(T) == typeof(Entity.General.ActiveSkill)
                || typeof(T) == typeof(Entity.General.Character) || typeof(T) == typeof(Entity.General.CharacterStatistics)
                || typeof(T) == typeof(Entity.General.GameStatistics) || typeof(T) == typeof(Entity.General.Inventory)
                || typeof(T) == typeof(Entity.General.Item) || typeof(T) == typeof(Entity.General.PassiveItem)
                || typeof(T) == typeof(Entity.General.PassiveSkill) || typeof(T) == typeof(Entity.General.Room)
                || typeof(T) == typeof(Entity.General.Skill) || typeof(T) == typeof(Entity.General.User)
                || typeof(T) == typeof(Entity.General.UserStatistics))
                return true;
            return false;
        }
    }
}
