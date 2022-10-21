using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            object instance = Core.Others.Config.EntityInstance;
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
    }
}
