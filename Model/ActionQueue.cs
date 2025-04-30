using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    /// <summary>
    /// 行动顺序表管理器，对 <see cref="GamingQueue"/> 的封装
    /// </summary>
    public class ActionQueue : GamingQueue
    {
        private ActionQueue() { }

        /// <summary>
        /// 按房间类型创建行动顺序表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="characters"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public static GamingQueue NewGame(RoomType type, List<Character>? characters = null, Action<string>? writer = null)
        {
            characters ??= [];
            return type switch
            {
                RoomType.Team => new TeamGamingQueue(writer),
                _ => new MixGamingQueue(writer)
            };
        }
    }
}
