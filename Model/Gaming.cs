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
        private readonly GameMode GameMode;
        private readonly Room Room;
        private readonly List<User> Users;
        private readonly List<Character> Characters;
        private readonly GamingEventArgs EventArgs;

        private Gaming(GameMode GameMode, Room Room, List<User> Users)
        {
            this.GameMode = GameMode;
            this.Room = Room;
            this.Users = Users;
            Characters = [];
            EventArgs = new(Room, Users, Characters);
        }

        /// <summary>
        /// 传入游戏所需的参数，构造一个Gaming实例
        /// </summary>
        /// <param name="GameMode"></param>
        /// <param name="Room"></param>
        /// <param name="Users"></param>
        /// <returns></returns>
        public static Gaming StartGame(GameMode GameMode, Room Room, List<User> Users)
        {
            Gaming instance = new(GameMode, Room, Users);
            // 新建线程来启动模组的界面
            TaskUtility.NewTask(() =>
            {
                GameMode.StartUI(instance.EventArgs);
            });
            return instance;
        }

        /// <summary>
        /// 需在RunTimeController的SocketHandler_Gaming方法中调用此方法
        /// <para>客户端也可以参照此方法自行实现</para>
        /// <para>此方法目的是为了触发 <see cref="Library.Common.Addon.GameMode"/> 的局内事件实现</para>
        /// </summary>
        /// <param name="GamingType"></param>
        /// <param name="Data"></param>
        public void GamingHandler(GamingType GamingType, Hashtable Data)
        {
            switch (GamingType)
            {
                case GamingType.Connect:
                    Connect(Data);
                    break;
                case GamingType.Disconnect:
                    Disconnect(Data);
                    break;
                case GamingType.Reconnect:
                    Reconnect(Data);
                    break;
                case GamingType.BanCharacter:
                    BanCharacter(Data);
                    break;
                case GamingType.PickCharacter:
                    PickCharacter(Data);
                    break;
                case GamingType.Random:
                    Random(Data);
                    break;
                case GamingType.Round:
                    Round(Data);
                    break;
                case GamingType.LevelUp:
                    LevelUp(Data);
                    break;
                case GamingType.Move:
                    Move(Data);
                    break;
                case GamingType.Attack:
                    Attack(Data);
                    break;
                case GamingType.Skill:
                    Skill(Data);
                    break;
                case GamingType.Item:
                    Item(Data);
                    break;
                case GamingType.Magic:
                    Magic(Data);
                    break;
                case GamingType.Buy:
                    Buy(Data);
                    break;
                case GamingType.SuperSkill:
                    SuperSkill(Data);
                    break;
                case GamingType.Pause:
                    Pause(Data);
                    break;
                case GamingType.Unpause:
                    Unpause(Data);
                    break;
                case GamingType.Surrender:
                    Surrender(Data);
                    break;
                case GamingType.UpdateInfo:
                    UpdateInfo(Data);
                    break;
                case GamingType.Punish:
                    Punish(Data);
                    break;
                case GamingType.None:
                default:
                    break;
            }
        }

        private void Connect(Hashtable data)
        {
            GameMode.OnBeforeGamingConnectEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingConnectEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingConnectEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingConnectEvent(this, EventArgs, data);
        }

        private void Disconnect(Hashtable data)
        {
            GameMode.OnBeforeGamingDisconnectEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingDisconnectEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingDisconnectEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingDisconnectEvent(this, EventArgs, data);
        }

        private void Reconnect(Hashtable data)
        {
            GameMode.OnBeforeGamingReconnectEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingReconnectEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingReconnectEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingReconnectEvent(this, EventArgs, data);
        }

        private void BanCharacter(Hashtable data)
        {
            GameMode.OnBeforeGamingBanCharacterEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingBanCharacterEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingBanCharacterEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingBanCharacterEvent(this, EventArgs, data);
        }

        private void PickCharacter(Hashtable data)
        {
            GameMode.OnBeforeGamingPickCharacterEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingPickCharacterEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingPickCharacterEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingPickCharacterEvent(this, EventArgs, data);
        }

        private void Random(Hashtable data)
        {
            GameMode.OnBeforeGamingRandomEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingRandomEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingRandomEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingRandomEvent(this, EventArgs, data);
        }

        private void Round(Hashtable data)
        {
            GameMode.OnBeforeGamingRoundEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingRoundEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingRoundEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingRoundEvent(this, EventArgs, data);
        }

        private void LevelUp(Hashtable data)
        {
            GameMode.OnBeforeGamingLevelUpEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingLevelUpEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingLevelUpEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingLevelUpEvent(this, EventArgs, data);
        }

        private void Move(Hashtable data)
        {
            GameMode.OnBeforeGamingMoveEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingMoveEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingMoveEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingMoveEvent(this, EventArgs, data);
        }

        private void Attack(Hashtable data)
        {
            GameMode.OnBeforeGamingAttackEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingAttackEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingAttackEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingAttackEvent(this, EventArgs, data);
        }

        private void Skill(Hashtable data)
        {
            GameMode.OnBeforeGamingSkillEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingSkillEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingSkillEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingSkillEvent(this, EventArgs, data);
        }

        private void Item(Hashtable data)
        {
            GameMode.OnBeforeGamingItemEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingItemEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingItemEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingItemEvent(this, EventArgs, data);
        }

        private void Magic(Hashtable data)
        {
            GameMode.OnBeforeGamingMagicEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingMagicEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingMagicEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingMagicEvent(this, EventArgs, data);
        }

        private void Buy(Hashtable data)
        {
            GameMode.OnBeforeGamingBuyEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingBuyEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingBuyEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingBuyEvent(this, EventArgs, data);
        }

        private void SuperSkill(Hashtable data)
        {
            GameMode.OnBeforeGamingSuperSkillEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingSuperSkillEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingSuperSkillEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingSuperSkillEvent(this, EventArgs, data);
        }

        private void Pause(Hashtable data)
        {
            GameMode.OnBeforeGamingPauseEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingPauseEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingPauseEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingPauseEvent(this, EventArgs, data);
        }

        private void Unpause(Hashtable data)
        {
            GameMode.OnBeforeGamingUnpauseEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingUnpauseEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingUnpauseEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingUnpauseEvent(this, EventArgs, data);
        }

        private void Surrender(Hashtable data)
        {
            GameMode.OnBeforeGamingSurrenderEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingSurrenderEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingSurrenderEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingSurrenderEvent(this, EventArgs, data);
        }

        private void UpdateInfo(Hashtable data)
        {
            GameMode.OnBeforeGamingUpdateInfoEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingUpdateInfoEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingUpdateInfoEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingUpdateInfoEvent(this, EventArgs, data);
        }

        private void Punish(Hashtable data)
        {
            GameMode.OnBeforeGamingPunishEvent(this, EventArgs, data);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingPunishEvent(this, EventArgs, data);
            }
            else
            {
                GameMode.OnFailedGamingPunishEvent(this, EventArgs, data);
            }
            GameMode.OnAfterGamingPunishEvent(this, EventArgs, data);
        }
    }
}
