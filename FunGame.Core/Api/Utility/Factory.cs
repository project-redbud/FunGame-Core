using System.Data;
using Milimoe.FunGame.Core.Api.Factory;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class Factory
    {
        /// <summary>
        /// 获取一个不为NULL的实例
        /// <para>Item默认返回PassiveItem</para>
        /// <para>Skill默认返回PassiveSkill</para>
        /// <para>若无法找到T，返回唯一的空对象</para>
        /// </summary>
        /// <typeparam name="T">Entity类</typeparam>
        /// <param name="DataSet">使用DataSet构造对象（真香）</param>
        /// <returns></returns>
        public static T GetInstance<T>(DataSet? DataSet)
        {
            object instance = General.EntityInstance;
            if (typeof(T) == typeof(User))
            {
                instance = UserFactory.GetInstance(DataSet);
            }
            else if (typeof(T) == typeof(Skill) || typeof(T) == typeof(PassiveSkill))
            {
                instance = SkillFactory.GetInstance(DataSet);
            }
            else if (typeof(T) == typeof(ActiveSkill))
            {
                instance = SkillFactory.GetInstance(DataSet, SkillType.Active);
            }
            else if (typeof(T) == typeof(Room))
            {
                instance = RoomFactory.GetInstance(DataSet);
            }
            return (T)instance;
        }
    }
}
