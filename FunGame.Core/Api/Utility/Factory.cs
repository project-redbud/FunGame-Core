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
        /// <param name="DrRoom">RoomRow</param>
        /// <param name="DrUser">UserRow(RoomMaster)</param>
        /// <returns></returns>
        public static Room GetRoom(DataRow? DrRoom, DataRow? DrUser)
        {
            return RoomFactory.GetInstance(DrRoom, DrUser);
        }

        /// <summary>
        /// 获取Skill实例，默认返回PassiveSkill
        /// </summary>
        /// <param name="DataRow">SkillRow</param>
        /// <param name="SkillType">Skill类型</param>
        /// <returns></returns>
        public static Skill GetSkill(DataRow? DataRow, SkillType SkillType = SkillType.Passive)
        {
            return SkillFactory.GetInstance(DataRow, SkillType);
        }

        /// <summary>
        /// 获取User实例
        /// </summary>
        /// <param name="DataRow">UserRow</param>
        /// <returns></returns>
        public static User GetUser(DataRow? DataRow)
        {
            return UserFactory.GetInstance(DataRow);
        }

        /// <summary>
        /// 获取一个不为NULL的实例
        /// <para>Item默认返回PassiveItem</para>
        /// <para>Skill默认返回PassiveSkill</para>
        /// <para>若无法找到T，返回唯一的空对象</para>
        /// </summary>
        /// <typeparam name="T">Entity类</typeparam>
        /// <param name="DataRows">使用DataRow构造对象</param>
        /// <returns>T</returns>
        public static T GetInstance<T>(params DataRow?[] DataRows)
        {
            if (DataRows is null || DataRows.Length == 0) throw new GetInstanceException();
            object instance = General.EntityInstance;
            if (typeof(T) == typeof(User))
            {
                instance = GetUser(DataRows[0]);
            }
            else if (typeof(T) == typeof(Skill) || typeof(T) == typeof(PassiveSkill))
            {
                instance = GetSkill(DataRows[0]);
            }
            else if (typeof(T) == typeof(ActiveSkill))
            {
                instance = GetSkill(DataRows[0], SkillType.Active);
            }
            else if (typeof(T) == typeof(Room))
            {
                instance = GetRoom(DataRows[0], DataRows[1]);
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
                    foreach (DataRow? row in ds.Tables[0].Rows)
                    {
                        object entity = GetUser(row);
                        list.Add((T)entity);
                    }
                }
            }
            else if (typeof(T) == typeof(Skill) || typeof(T) == typeof(PassiveSkill))
            {
                DataSet? ds = DataSets[0];
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow? row in ds.Tables[0].Rows)
                    {
                        object entity = GetSkill(row);
                        list.Add((T)entity);
                    }
                }
            }
            else if (typeof(T) == typeof(ActiveSkill))
            {
                DataSet? ds = DataSets[0];
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow? row in ds.Tables[0].Rows)
                    {
                        object entity = GetSkill(row, SkillType.Active);
                        list.Add((T)entity);
                    }
                }
            }
            else if (typeof(T) == typeof(Room))
            {
                DataSet? ds = DataSets[0];
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow? row in ds.Tables[0].Rows)
                    {
                        if (row != null)
                        {
                            DataRow[] rows = ds.Tables[1].Select($"{Library.SQLScript.Entity.UserQuery.Column_Username} = '{row[Library.SQLScript.Entity.RoomQuery.Column_RoomMasterName]}'");
                            if (rows != null && rows.Length > 0)
                            {
                                object entity = GetRoom(row, rows[0]);
                                list.Add((T)entity);
                            }
                        }
                    }
                }
            }
            return new List<T>();
        }
    }
}
