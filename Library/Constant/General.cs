﻿using System.Text;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;

namespace Milimoe.FunGame.Core.Library.Constant
{
    /// <summary>
    /// 此类保存常用的对象常量
    /// </summary>
    public class General
    {
        #region  Static Variable

        /// <summary>
        /// 空的实体类 用于object返回
        /// </summary>
        public static Empty EntityInstance => new();

        /// <summary>
        /// 默认的未知用户
        /// </summary>
        public static User UnknownUserInstance => new();

        /// <summary>
        /// 大厅（空房间）实例
        /// </summary>
        public static Room HallInstance => new();

        /// <summary>
        /// 默认的字符编码
        /// </summary>
        public static Encoding DefaultEncoding => Encoding.Unicode;

        /// <summary>
        /// 默认的时间格式
        /// </summary>
        public static string GeneralDateTimeFormat => "yyyy-MM-dd HH:mm:ss.fff";

        /// <summary>
        /// 默认的时间值（1970年8月1日8点0分0秒）
        /// </summary>
        public static DateTime DefaultTime => new(1970, 1, 1, 8, 0, 0);

        /// <summary>
        /// 默认的混战模式模组
        /// </summary>
        public static GameMode DefaultOfficialModeMix => new OfficialModeMix8Players();

        /// <summary>
        /// 默认的团队模式模组
        /// </summary>
        public static GameMode DefaultOfficialModeTeam => new OfficialModeTeam4Players();

        /// <summary>
        /// 默认的对弈模式模组
        /// </summary>
        public static GameMode DefaultOfficialModeSolo => new OfficialModeSolo3Characters();
        
        /// <summary>
        /// 默认的快速自走模式模组
        /// </summary>
        public static GameMode DefaultOfficialModeFastAuto => new OfficialModeFastAuto8Players();
        
        /// <summary>
        /// 默认的游戏地图
        /// </summary>
        public static GameMap DefaultOfficialMap => new OfficialMap16x16();

        #endregion

        #region Const

        /// <summary>
        /// 最多自动重试连接次数
        /// </summary>
        public const int MaxRetryTimes = 20;

        /// <summary>
        /// 1C2G推荐的任务数量
        /// </summary>
        public const int MaxTask_1C2G = 10;

        /// <summary>
        /// 2C2G推荐的任务数量
        /// </summary>
        public const int MaxTask_2C2G = 20;

        /// <summary>
        /// 4C4G推荐的任务数量
        /// </summary>
        public const int MaxTask_4C4G = 40;

        /// <summary>
        /// 默认Socket数据包大小
        /// </summary>
        public const int SocketByteSize = 512 * 1024;

        /// <summary>
        /// 默认Stream传输大小
        /// </summary>
        public const int StreamByteSize = 2048;

        #endregion
    }
}
