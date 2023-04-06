using System.Data;
using Milimoe.FunGame.Core.Api.Factory;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class Factory
    {
        /// <summary>
        /// 获取Room实例
        /// </summary>
        /// <param name="DsRoom">Room</param>
        /// <param name="DsUser">User(RoomMaster)</param>
        /// <param name="Index">取指定行</param>
        /// <returns></returns>
        public static Room GetRoom(DataSet? DsRoom, DataSet? DsUser, int Index = 0)
        {
            return RoomFactory.GetInstance(DsRoom, DsUser, Index);
        }

        /// <summary>
        /// 获取大厅（-1号房）
        /// </summary>
        /// <returns></returns>
        internal static Room GetHall()
        {
            return GetRoom(null, null);
        }

        /// <summary>
        /// 获取Skill实例，默认返回PassiveSkill
        /// </summary>
        /// <param name="DataSet">SkillRow</param>
        /// <param name="SkillType">Skill类型</param>
        /// <param name="Index">取指定行</param>
        /// <returns></returns>
        public static Skill GetSkill(DataSet? DataSet, SkillType SkillType = SkillType.Passive, int Index = 0)
        {
            return SkillFactory.GetInstance(DataSet, SkillType, Index);
        }

        /// <summary>
        /// 获取User实例
        /// </summary>awaa
        /// <param name="DataSet">UserRow</param>
        /// <param name="Index">取指定行</param>
        /// <returns></returns>
        public static User GetUser(DataSet? DataSet, int Index = 0)
        {
            return UserFactory.GetInstance(DataSet, Index);
        }

        /// <summary>
        /// 获取一个不为NULL的实例
        /// <para>Item默认返回PassiveItem</para>
        /// <para>Skill默认返回PassiveSkill</para>
        /// <para>若无法找到T，返回唯一的空对象</para>
        /// </summary>
        /// <typeparam name="T">Entity类</typeparam>
        /// <param name="DataSets">使用DataSets构造对象</param>
        /// <returns>T</returns>
        public static T GetInstance<T>(params DataSet?[] DataSets)
        {
            if (DataSets is null || DataSets.Length == 0) throw new GetInstanceException();
            object instance = General.EntityInstance;
            if (typeof(T) == typeof(User))
            {
                instance = GetUser(DataSets[0]);
            }
            else if (typeof(T) == typeof(Skill) || typeof(T) == typeof(PassiveSkill))
            {
                instance = GetSkill(DataSets[0]);
            }
            else if (typeof(T) == typeof(ActiveSkill))
            {
                instance = GetSkill(DataSets[0], SkillType.Active);
            }
            else if (typeof(T) == typeof(Room))
            {
                instance = GetRoom(DataSets[0], DataSets[1]);
            }
            return (T)instance;
        }

        /// <summary>
        /// 获取T的数组
        /// <para>Item默认返回PassiveItem数组</para>
        /// <para>Skill默认返回PassiveSkill数组</para>
        /// <para>若无法找到T，返回空数组</para>
        /// </summary>
        /// <typeparam name="T">Entity类</typeparam>
        /// <param name="DataSets">使用DataSet构造对象数组</param>
        /// <returns>List T</returns>
        public static List<T> GetList<T>(params DataSet?[] DataSets)
        {
            List<T> list = new();
            if (DataSets is null || DataSets.Length == 0) throw new GetInstanceException();
            if (typeof(T) == typeof(User))
            {
                DataSet? ds = DataSets[0];
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        object entity = GetUser(ds, i);
                        list.Add((T)entity);
                    }
                }
            }
            else if (typeof(T) == typeof(Skill) || typeof(T) == typeof(PassiveSkill))
            {
                DataSet? ds = DataSets[0];
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        object entity = GetSkill(ds, SkillType.Passive, i);
                        list.Add((T)entity);
                    }
                }
            }
            else if (typeof(T) == typeof(ActiveSkill))
            {
                DataSet? ds = DataSets[0];
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        object entity = GetSkill(ds, SkillType.Active, i);
                        list.Add((T)entity);
                    }
                }
            }
            else if (typeof(T) == typeof(Room))
            {
                DataSet? DsRoom = DataSets[0];
                DataSet? DsUser = DataSets[1];
                if (DsRoom != null && DsRoom.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < DsRoom.Tables[0].Rows.Count; i++)
                    {
                        object entity = GetRoom(DsRoom, DsUser, i);
                        list.Add((T)entity);
                    }
                }
            }
            return list;
        }
    }
}
