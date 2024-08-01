using System.Collections;
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

        private Gaming(GameModule module, Room room, List<User> users)
        {
            GameModule = module;
            EventArgs = new(room, users);
        }

        /// <summary>
        /// 传入游戏所需的参数，构造一个Gaming实例
        /// </summary>
        /// <param name="module"></param>
        /// <param name="room"></param>
        /// <param name="users"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Gaming StartGame(GameModule module, Room room, List<User> users, params object[] args)
        {
            Gaming instance = new(module, room, users);
            // 新建线程来启动模组的界面
            TaskUtility.NewTask(() =>
            {
                module.StartUI();
            });
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
        /// <returns>底层会将哈希表中的数据发送给服务器</returns>
        public Hashtable GamingHandler(GamingType type, Hashtable data)
        {
            Hashtable result = [];
            switch (type)
            {
                case GamingType.Connect:
                    Connect(data, result);
                    break;
                case GamingType.Disconnect:
                    Disconnect(data, result);
                    break;
                case GamingType.Reconnect:
                    Reconnect(data, result);
                    break;
                case GamingType.BanCharacter:
                    BanCharacter(data, result);
                    break;
                case GamingType.PickCharacter:
                    PickCharacter(data, result);
                    break;
                case GamingType.Random:
                    Random(data, result);
                    break;
                case GamingType.Round:
                    Round(data, result);
                    break;
                case GamingType.LevelUp:
                    LevelUp(data, result);
                    break;
                case GamingType.Move:
                    Move(data, result);
                    break;
                case GamingType.Attack:
                    Attack(data, result);
                    break;
                case GamingType.Skill:
                    Skill(data, result);
                    break;
                case GamingType.Item:
                    Item(data, result);
                    break;
                case GamingType.Magic:
                    Magic(data, result);
                    break;
                case GamingType.Buy:
                    Buy(data, result);
                    break;
                case GamingType.SuperSkill:
                    SuperSkill(data, result);
                    break;
                case GamingType.Pause:
                    Pause(data, result);
                    break;
                case GamingType.Unpause:
                    Unpause(data, result);
                    break;
                case GamingType.Surrender:
                    Surrender(data, result);
                    break;
                case GamingType.UpdateInfo:
                    UpdateInfo(data, result);
                    break;
                case GamingType.Punish:
                    Punish(data, result);
                    break;
                case GamingType.None:
                default:
                    break;
            }
            return result;
        }

        private void Connect(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingConnectEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingConnectEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingConnectEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingConnectEvent(this, EventArgs, data, result);
        }

        private void Disconnect(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingDisconnectEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingDisconnectEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingDisconnectEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingDisconnectEvent(this, EventArgs, data, result);
        }

        private void Reconnect(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingReconnectEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingReconnectEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingReconnectEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingReconnectEvent(this, EventArgs, data, result);
        }

        private void BanCharacter(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingBanCharacterEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingBanCharacterEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingBanCharacterEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingBanCharacterEvent(this, EventArgs, data, result);
        }

        private void PickCharacter(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingPickCharacterEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingPickCharacterEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingPickCharacterEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingPickCharacterEvent(this, EventArgs, data, result);
        }

        private void Random(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingRandomEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingRandomEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingRandomEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingRandomEvent(this, EventArgs, data, result);
        }

        private void Round(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingRoundEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingRoundEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingRoundEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingRoundEvent(this, EventArgs, data, result);
        }

        private void LevelUp(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingLevelUpEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingLevelUpEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingLevelUpEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingLevelUpEvent(this, EventArgs, data, result);
        }

        private void Move(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingMoveEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingMoveEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingMoveEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingMoveEvent(this, EventArgs, data, result);
        }

        private void Attack(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingAttackEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingAttackEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingAttackEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingAttackEvent(this, EventArgs, data, result);
        }

        private void Skill(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingSkillEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingSkillEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingSkillEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingSkillEvent(this, EventArgs, data, result);
        }

        private void Item(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingItemEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingItemEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingItemEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingItemEvent(this, EventArgs, data, result);
        }

        private void Magic(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingMagicEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingMagicEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingMagicEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingMagicEvent(this, EventArgs, data, result);
        }

        private void Buy(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingBuyEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingBuyEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingBuyEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingBuyEvent(this, EventArgs, data, result);
        }

        private void SuperSkill(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingSuperSkillEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingSuperSkillEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingSuperSkillEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingSuperSkillEvent(this, EventArgs, data, result);
        }

        private void Pause(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingPauseEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingPauseEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingPauseEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingPauseEvent(this, EventArgs, data, result);
        }

        private void Unpause(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingUnpauseEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingUnpauseEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingUnpauseEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingUnpauseEvent(this, EventArgs, data, result);
        }

        private void Surrender(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingSurrenderEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingSurrenderEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingSurrenderEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingSurrenderEvent(this, EventArgs, data, result);
        }

        private void UpdateInfo(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingUpdateInfoEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingUpdateInfoEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingUpdateInfoEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingUpdateInfoEvent(this, EventArgs, data, result);
        }

        private void Punish(Hashtable data, Hashtable result)
        {
            GameModule.OnBeforeGamingPunishEvent(this, EventArgs, data, result);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameModule.OnSucceedGamingPunishEvent(this, EventArgs, data, result);
            }
            else
            {
                GameModule.OnFailedGamingPunishEvent(this, EventArgs, data, result);
            }
            GameModule.OnAfterGamingPunishEvent(this, EventArgs, data, result);
        }
    }
}
