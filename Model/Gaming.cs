using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    /// <summary>
    /// 游戏局内类
    /// 客户端需要使用创建此类单例
    /// </summary>
    public class Gaming
    {
        /// <summary>
        /// 使用的模组实例
        /// </summary>
        public GameModule GameModule { get; }

        /// <summary>
        /// 游戏的参数
        /// </summary>
        public GamingEventArgs EventArgs { get; }

        /// <summary>
        /// 此实例所属的玩家
        /// </summary>
        public User CurrentUser { get; }

        private Gaming(GameModule module, Room room, User user, List<User> users)
        {
            GameModule = module;
            EventArgs = new(room, users);
            CurrentUser = user;
        }

        /// <summary>
        /// 传入游戏所需的参数，构造一个Gaming实例
        /// </summary>
        /// <param name="module"></param>
        /// <param name="room"></param>
        /// <param name="user"></param>
        /// <param name="users"></param>
        /// <param name="loader"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Gaming StartGame(GameModule module, Room room, User user, List<User> users, GameModuleLoader loader, params object[] args)
        {
            Gaming instance = new(module, room, user, users);
            // 读取模组的依赖集合
            module.GameModuleDepend.GetDependencies(loader);
            // 新建线程来启动模组的界面
            TaskUtility.NewTask(() => module.StartUI());
            // 启动模组主线程
            module.StartGame(instance, args);
            return instance;
        }

        /// <summary>
        /// 需在RunTimeController的SocketHandler_Gaming方法中调用此方法
        /// <para>客户端也可以参照此方法自行实现</para>
        /// <para>此方法目的是为了触发 <see cref="Library.Common.Addon.GameModule"/> 的局内事件实现</para>
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="data">接收到的数据</param>
        public void GamingHandler(GamingType type, Dictionary<string, object> data)
        {
            switch (type)
            {
                case GamingType.Connect:
                    Connect(data);
                    break;
                case GamingType.Disconnect:
                    Disconnect(data);
                    break;
                case GamingType.Reconnect:
                    Reconnect(data);
                    break;
                case GamingType.BanCharacter:
                    BanCharacter(data);
                    break;
                case GamingType.PickCharacter:
                    PickCharacter(data);
                    break;
                case GamingType.Random:
                    Random(data);
                    break;
                case GamingType.Round:
                    Round(data);
                    break;
                case GamingType.LevelUp:
                    LevelUp(data);
                    break;
                case GamingType.Move:
                    Move(data);
                    break;
                case GamingType.Attack:
                    Attack(data);
                    break;
                case GamingType.Skill:
                    Skill(data);
                    break;
                case GamingType.Item:
                    Item(data);
                    break;
                case GamingType.Magic:
                    Magic(data);
                    break;
                case GamingType.Buy:
                    Buy(data);
                    break;
                case GamingType.SuperSkill:
                    SuperSkill(data);
                    break;
                case GamingType.Pause:
                    Pause(data);
                    break;
                case GamingType.Unpause:
                    Unpause(data);
                    break;
                case GamingType.Surrender:
                    Surrender(data);
                    break;
                case GamingType.UpdateInfo:
                    UpdateInfo(data);
                    break;
                case GamingType.Punish:
                    Punish(data);
                    break;
                case GamingType.None:
                default:
                    break;
            }
        }

        private void Connect(Dictionary<string, object> data)
        {
            GameModule.OnGamingConnectEvent(this, EventArgs, data);
        }

        private void Disconnect(Dictionary<string, object> data)
        {
            GameModule.OnGamingDisconnectEvent(this, EventArgs, data);
        }

        private void Reconnect(Dictionary<string, object> data)
        {
            GameModule.OnGamingReconnectEvent(this, EventArgs, data);
        }

        private void BanCharacter(Dictionary<string, object> data)
        {
            GameModule.OnGamingBanCharacterEvent(this, EventArgs, data);
        }

        private void PickCharacter(Dictionary<string, object> data)
        {
            GameModule.OnGamingPickCharacterEvent(this, EventArgs, data);
        }

        private void Random(Dictionary<string, object> data)
        {
            GameModule.OnGamingRandomEvent(this, EventArgs, data);
        }

        private void Round(Dictionary<string, object> data)
        {
            GameModule.OnGamingRoundEvent(this, EventArgs, data);
        }

        private void LevelUp(Dictionary<string, object> data)
        {
            GameModule.OnGamingLevelUpEvent(this, EventArgs, data);
        }

        private void Move(Dictionary<string, object> data)
        {
            GameModule.OnGamingMoveEvent(this, EventArgs, data);
        }

        private void Attack(Dictionary<string, object> data)
        {
            GameModule.OnGamingAttackEvent(this, EventArgs, data);
        }

        private void Skill(Dictionary<string, object> data)
        {
            GameModule.OnGamingSkillEvent(this, EventArgs, data);
        }

        private void Item(Dictionary<string, object> data)
        {
            GameModule.OnGamingItemEvent(this, EventArgs, data);
        }

        private void Magic(Dictionary<string, object> data)
        {
            GameModule.OnGamingMagicEvent(this, EventArgs, data);
        }

        private void Buy(Dictionary<string, object> data)
        {
            GameModule.OnGamingBuyEvent(this, EventArgs, data);
        }

        private void SuperSkill(Dictionary<string, object> data)
        {
            GameModule.OnGamingSuperSkillEvent(this, EventArgs, data);
        }

        private void Pause(Dictionary<string, object> data)
        {
            GameModule.OnGamingPauseEvent(this, EventArgs, data);
        }

        private void Unpause(Dictionary<string, object> data)
        {
            GameModule.OnGamingUnpauseEvent(this, EventArgs, data);
        }

        private void Surrender(Dictionary<string, object> data)
        {
            GameModule.OnGamingSurrenderEvent(this, EventArgs, data);
        }

        private void UpdateInfo(Dictionary<string, object> data)
        {
            GameModule.OnGamingUpdateInfoEvent(this, EventArgs, data);
        }

        private void Punish(Dictionary<string, object> data)
        {
            GameModule.OnGamingPunishEvent(this, EventArgs, data);
        }
    }
}
