using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public abstract class GameMode : IGameMode
    {
        /// <summary>
        /// 模组名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 模组描述
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 模组版本
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// 模组作者
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// 模组所使用的地图
        /// </summary>
        public abstract string Map { get; }

        /// <summary>
        /// 如模组有界面，请重写此方法
        /// 此方法会在StartGame时调用
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual bool StartUI(params object[] args)
        {
            return true;
        }

        /// <summary>
        /// 加载标记
        /// </summary>
        private bool IsLoaded = false;

        /// <summary>
        /// 加载模组
        /// </summary>
        public bool Load(params object[] objs)
        {
            if (IsLoaded)
            {
                return false;
            }
            // BeforeLoad可以阻止加载此模组
            if (BeforeLoad())
            {
                // 模组加载后，不允许再次加载此模组
                IsLoaded = true;
                // 初始化此模组（传入委托或者Model）
                Init(objs);
                // 触发绑定事件
                BindEvent();
                // 如果加载后需要执行代码，请重写AfterLoad方法
                AfterLoad();
            }
            return IsLoaded;
        }

        /// <summary>
        /// 模组加载后需要做的事
        /// </summary>
        protected virtual void AfterLoad()
        {
            // override
        }

        /// <summary>
        /// 允许返回false来阻止加载此模组
        /// </summary>
        /// <returns></returns>
        protected virtual bool BeforeLoad()
        {
            return true;
        }

        /// <summary>
        /// 传递委托以便让模组调用
        /// </summary>
        private void Init(params object[] objs)
        {
            if (objs.Length > 0) WritelnSystemInfo = (Action<string>)objs[0];
            if (objs.Length > 1) NewDataRequest = (Func<DataRequestType, DataRequest>)objs[1];
            if (objs.Length > 2) NewLongRunningDataRequest = (Func<DataRequestType, DataRequest>)objs[2];
            if (objs.Length > 3) Session = (Session)objs[3];
            if (objs.Length > 4) Config = (FunGameConfig)objs[4];
        }

        /// <summary>
        /// 输出系统消息
        /// </summary>
        protected Action<string> WritelnSystemInfo = new(msg => Console.Write("\r" + msg + "\n\r> "));

        /// <summary>
        /// 基于本地已连接的Socket创建新的数据请求
        /// </summary>
        protected Func<DataRequestType, DataRequest> NewDataRequest = new(type => throw new ConnectFailedException());

        /// <summary>
        /// 基于本地已连接的Socket创建长时间运行的数据请求
        /// </summary>
        protected Func<DataRequestType, DataRequest> NewLongRunningDataRequest = new(type => throw new ConnectFailedException());

        /// <summary>
        /// Session对象
        /// </summary>
        protected Session Session = new();

        /// <summary>
        /// Config对象
        /// </summary>
        protected FunGameConfig Config = new();

        /// <summary>
        /// 绑定事件。在<see cref="BeforeLoad"/>后触发
        /// </summary>
        private void BindEvent()
        {
            if (this is IGamingConnectEvent)
            {
                IGamingConnectEvent bind = (IGamingConnectEvent)this;
                BeforeGamingConnect += bind.BeforeGamingConnectEvent;
                AfterGamingConnect += bind.AfterGamingConnectEvent;
                SucceedGamingConnect += bind.SucceedGamingConnectEvent;
                FailedGamingConnect += bind.FailedGamingConnectEvent;
            }

            if (this is IGamingDisconnectEvent)
            {
                IGamingDisconnectEvent bind = (IGamingDisconnectEvent)this;
                BeforeGamingDisconnect += bind.BeforeGamingDisconnectEvent;
                AfterGamingDisconnect += bind.AfterGamingDisconnectEvent;
                SucceedGamingDisconnect += bind.SucceedGamingDisconnectEvent;
                FailedGamingDisconnect += bind.FailedGamingDisconnectEvent;
            }

            if (this is IGamingReconnectEvent)
            {
                IGamingReconnectEvent bind = (IGamingReconnectEvent)this;
                BeforeGamingReconnect += bind.BeforeGamingReconnectEvent;
                AfterGamingReconnect += bind.AfterGamingReconnectEvent;
                SucceedGamingReconnect += bind.SucceedGamingReconnectEvent;
                FailedGamingReconnect += bind.FailedGamingReconnectEvent;
            }

            if (this is IGamingBanCharacterEvent)
            {
                IGamingBanCharacterEvent bind = (IGamingBanCharacterEvent)this;
                BeforeGamingBanCharacter += bind.BeforeGamingBanCharacterEvent;
                AfterGamingBanCharacter += bind.AfterGamingBanCharacterEvent;
                SucceedGamingBanCharacter += bind.SucceedGamingBanCharacterEvent;
                FailedGamingBanCharacter += bind.FailedGamingBanCharacterEvent;
            }

            if (this is IGamingPickCharacterEvent)
            {
                IGamingPickCharacterEvent bind = (IGamingPickCharacterEvent)this;
                BeforeGamingPickCharacter += bind.BeforeGamingPickCharacterEvent;
                AfterGamingPickCharacter += bind.AfterGamingPickCharacterEvent;
                SucceedGamingPickCharacter += bind.SucceedGamingPickCharacterEvent;
                FailedGamingPickCharacter += bind.FailedGamingPickCharacterEvent;
            }

            if (this is IGamingRandomEvent)
            {
                IGamingRandomEvent bind = (IGamingRandomEvent)this;
                BeforeGamingRandom += bind.BeforeGamingRandomEvent;
                AfterGamingRandom += bind.AfterGamingRandomEvent;
                SucceedGamingRandom += bind.SucceedGamingRandomEvent;
                FailedGamingRandom += bind.FailedGamingRandomEvent;
            }

            if (this is IGamingMoveEvent)
            {
                IGamingMoveEvent bind = (IGamingMoveEvent)this;
                BeforeGamingMove += bind.BeforeGamingMoveEvent;
                AfterGamingMove += bind.AfterGamingMoveEvent;
                SucceedGamingMove += bind.SucceedGamingMoveEvent;
                FailedGamingMove += bind.FailedGamingMoveEvent;
            }

            if (this is IGamingAttackEvent)
            {
                IGamingAttackEvent bind = (IGamingAttackEvent)this;
                BeforeGamingAttack += bind.BeforeGamingAttackEvent;
                AfterGamingAttack += bind.AfterGamingAttackEvent;
                SucceedGamingAttack += bind.SucceedGamingAttackEvent;
                FailedGamingAttack += bind.FailedGamingAttackEvent;
            }

            if (this is IGamingSkillEvent)
            {
                IGamingSkillEvent bind = (IGamingSkillEvent)this;
                BeforeGamingSkill += bind.BeforeGamingSkillEvent;
                AfterGamingSkill += bind.AfterGamingSkillEvent;
                SucceedGamingSkill += bind.SucceedGamingSkillEvent;
                FailedGamingSkill += bind.FailedGamingSkillEvent;
            }

            if (this is IGamingItemEvent)
            {
                IGamingItemEvent bind = (IGamingItemEvent)this;
                BeforeGamingItem += bind.BeforeGamingItemEvent;
                AfterGamingItem += bind.AfterGamingItemEvent;
                SucceedGamingItem += bind.SucceedGamingItemEvent;
                FailedGamingItem += bind.FailedGamingItemEvent;
            }

            if (this is IGamingMagicEvent)
            {
                IGamingMagicEvent bind = (IGamingMagicEvent)this;
                BeforeGamingMagic += bind.BeforeGamingMagicEvent;
                AfterGamingMagic += bind.AfterGamingMagicEvent;
                SucceedGamingMagic += bind.SucceedGamingMagicEvent;
                FailedGamingMagic += bind.FailedGamingMagicEvent;
            }

            if (this is IGamingBuyEvent)
            {
                IGamingBuyEvent bind = (IGamingBuyEvent)this;
                BeforeGamingBuy += bind.BeforeGamingBuyEvent;
                AfterGamingBuy += bind.AfterGamingBuyEvent;
                SucceedGamingBuy += bind.SucceedGamingBuyEvent;
                FailedGamingBuy += bind.FailedGamingBuyEvent;
            }

            if (this is IGamingSuperSkillEvent)
            {
                IGamingSuperSkillEvent bind = (IGamingSuperSkillEvent)this;
                BeforeGamingSuperSkill += bind.BeforeGamingSuperSkillEvent;
                AfterGamingSuperSkill += bind.AfterGamingSuperSkillEvent;
                SucceedGamingSuperSkill += bind.SucceedGamingSuperSkillEvent;
                FailedGamingSuperSkill += bind.FailedGamingSuperSkillEvent;
            }

            if (this is IGamingPauseEvent)
            {
                IGamingPauseEvent bind = (IGamingPauseEvent)this;
                BeforeGamingPause += bind.BeforeGamingPauseEvent;
                AfterGamingPause += bind.AfterGamingPauseEvent;
                SucceedGamingPause += bind.SucceedGamingPauseEvent;
                FailedGamingPause += bind.FailedGamingPauseEvent;
            }

            if (this is IGamingUnpauseEvent)
            {
                IGamingUnpauseEvent bind = (IGamingUnpauseEvent)this;
                BeforeGamingUnpause += bind.BeforeGamingUnpauseEvent;
                AfterGamingUnpause += bind.AfterGamingUnpauseEvent;
                SucceedGamingUnpause += bind.SucceedGamingUnpauseEvent;
                FailedGamingUnpause += bind.FailedGamingUnpauseEvent;
            }

            if (this is IGamingSurrenderEvent)
            {
                IGamingSurrenderEvent bind = (IGamingSurrenderEvent)this;
                BeforeGamingSurrender += bind.BeforeGamingSurrenderEvent;
                AfterGamingSurrender += bind.AfterGamingSurrenderEvent;
                SucceedGamingSurrender += bind.SucceedGamingSurrenderEvent;
                FailedGamingSurrender += bind.FailedGamingSurrenderEvent;
            }

            if (this is IGamingUpdateInfoEvent)
            {
                IGamingUpdateInfoEvent bind = (IGamingUpdateInfoEvent)this;
                BeforeGamingUpdateInfo += bind.BeforeGamingUpdateInfoEvent;
                AfterGamingUpdateInfo += bind.AfterGamingUpdateInfoEvent;
                SucceedGamingUpdateInfo += bind.SucceedGamingUpdateInfoEvent;
                FailedGamingUpdateInfo += bind.FailedGamingUpdateInfoEvent;
            }
        }

        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingConnect;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingConnect;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingConnect;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingConnect;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingDisconnect;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingDisconnect;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingDisconnect;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingDisconnect;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingReconnect;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingReconnect;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingReconnect;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingReconnect;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingBanCharacter;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingBanCharacter;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingBanCharacter;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingBanCharacter;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingPickCharacter;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingPickCharacter;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingPickCharacter;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingPickCharacter;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingRandom;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingRandom;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingRandom;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingRandom;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingMove;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingMove;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingMove;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingMove;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingAttack;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingAttack;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingAttack;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingAttack;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingSkill;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingSkill;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingSkill;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingSkill;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingItem;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingItem;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingItem;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingItem;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingMagic;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingMagic;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingMagic;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingMagic;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingBuy;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingBuy;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingBuy;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingBuy;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingSuperSkill;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingSuperSkill;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingSuperSkill;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingSuperSkill;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingPause;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingPause;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingPause;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingPause;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingUnpause;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingUnpause;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingUnpause;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingUnpause;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingSurrender;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingSurrender;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingSurrender;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingSurrender;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingUpdateInfo;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingUpdateInfo;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingUpdateInfo;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingUpdateInfo;

        public void OnBeforeGamingConnectEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingConnect?.Invoke(sender, e);
        }

        public void OnAfterGamingConnectEvent(object sender, GamingEventArgs e)
        {
            AfterGamingConnect?.Invoke(sender, e);
        }

        public void OnSucceedGamingConnectEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingConnect?.Invoke(sender, e);
        }

        public void OnFailedGamingConnectEvent(object sender, GamingEventArgs e)
        {
            FailedGamingConnect?.Invoke(sender, e);
        }

        public void OnBeforeGamingDisconnectEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingDisconnect?.Invoke(sender, e);
        }

        public void OnAfterGamingDisconnectEvent(object sender, GamingEventArgs e)
        {
            AfterGamingDisconnect?.Invoke(sender, e);
        }

        public void OnSucceedGamingDisconnectEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingDisconnect?.Invoke(sender, e);
        }

        public void OnFailedGamingDisconnectEvent(object sender, GamingEventArgs e)
        {
            FailedGamingDisconnect?.Invoke(sender, e);
        }

        public void OnBeforeGamingReconnectEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingReconnect?.Invoke(sender, e);
        }

        public void OnAfterGamingReconnectEvent(object sender, GamingEventArgs e)
        {
            AfterGamingReconnect?.Invoke(sender, e);
        }

        public void OnSucceedGamingReconnectEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingReconnect?.Invoke(sender, e);
        }

        public void OnFailedGamingReconnectEvent(object sender, GamingEventArgs e)
        {
            FailedGamingReconnect?.Invoke(sender, e);
        }

        public void OnBeforeGamingBanCharacterEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingBanCharacter?.Invoke(sender, e);
        }

        public void OnAfterGamingBanCharacterEvent(object sender, GamingEventArgs e)
        {
            AfterGamingBanCharacter?.Invoke(sender, e);
        }

        public void OnSucceedGamingBanCharacterEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingBanCharacter?.Invoke(sender, e);
        }

        public void OnFailedGamingBanCharacterEvent(object sender, GamingEventArgs e)
        {
            FailedGamingBanCharacter?.Invoke(sender, e);
        }

        public void OnBeforeGamingPickCharacterEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingPickCharacter?.Invoke(sender, e);
        }

        public void OnAfterGamingPickCharacterEvent(object sender, GamingEventArgs e)
        {
            AfterGamingPickCharacter?.Invoke(sender, e);
        }

        public void OnSucceedGamingPickCharacterEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingPickCharacter?.Invoke(sender, e);
        }

        public void OnFailedGamingPickCharacterEvent(object sender, GamingEventArgs e)
        {
            FailedGamingPickCharacter?.Invoke(sender, e);
        }

        public void OnBeforeGamingRandomEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingRandom?.Invoke(sender, e);
        }

        public void OnAfterGamingRandomEvent(object sender, GamingEventArgs e)
        {
            AfterGamingRandom?.Invoke(sender, e);
        }

        public void OnSucceedGamingRandomEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingRandom?.Invoke(sender, e);
        }

        public void OnFailedGamingRandomEvent(object sender, GamingEventArgs e)
        {
            FailedGamingRandom?.Invoke(sender, e);
        }

        public void OnBeforeGamingMoveEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingMove?.Invoke(sender, e);
        }

        public void OnAfterGamingMoveEvent(object sender, GamingEventArgs e)
        {
            AfterGamingMove?.Invoke(sender, e);
        }

        public void OnSucceedGamingMoveEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingMove?.Invoke(sender, e);
        }

        public void OnFailedGamingMoveEvent(object sender, GamingEventArgs e)
        {
            FailedGamingMove?.Invoke(sender, e);
        }

        public void OnBeforeGamingAttackEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingAttack?.Invoke(sender, e);
        }

        public void OnAfterGamingAttackEvent(object sender, GamingEventArgs e)
        {
            AfterGamingAttack?.Invoke(sender, e);
        }

        public void OnSucceedGamingAttackEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingAttack?.Invoke(sender, e);
        }

        public void OnFailedGamingAttackEvent(object sender, GamingEventArgs e)
        {
            FailedGamingAttack?.Invoke(sender, e);
        }

        public void OnBeforeGamingSkillEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingSkill?.Invoke(sender, e);
        }

        public void OnAfterGamingSkillEvent(object sender, GamingEventArgs e)
        {
            AfterGamingSkill?.Invoke(sender, e);
        }

        public void OnSucceedGamingSkillEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingSkill?.Invoke(sender, e);
        }

        public void OnFailedGamingSkillEvent(object sender, GamingEventArgs e)
        {
            FailedGamingSkill?.Invoke(sender, e);
        }

        public void OnBeforeGamingItemEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingItem?.Invoke(sender, e);
        }

        public void OnAfterGamingItemEvent(object sender, GamingEventArgs e)
        {
            AfterGamingItem?.Invoke(sender, e);
        }

        public void OnSucceedGamingItemEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingItem?.Invoke(sender, e);
        }

        public void OnFailedGamingItemEvent(object sender, GamingEventArgs e)
        {
            FailedGamingItem?.Invoke(sender, e);
        }

        public void OnBeforeGamingMagicEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingMagic?.Invoke(sender, e);
        }

        public void OnAfterGamingMagicEvent(object sender, GamingEventArgs e)
        {
            AfterGamingMagic?.Invoke(sender, e);
        }

        public void OnSucceedGamingMagicEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingMagic?.Invoke(sender, e);
        }

        public void OnFailedGamingMagicEvent(object sender, GamingEventArgs e)
        {
            FailedGamingMagic?.Invoke(sender, e);
        }

        public void OnBeforeGamingBuyEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingBuy?.Invoke(sender, e);
        }

        public void OnAfterGamingBuyEvent(object sender, GamingEventArgs e)
        {
            AfterGamingBuy?.Invoke(sender, e);
        }

        public void OnSucceedGamingBuyEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingBuy?.Invoke(sender, e);
        }

        public void OnFailedGamingBuyEvent(object sender, GamingEventArgs e)
        {
            FailedGamingBuy?.Invoke(sender, e);
        }

        public void OnBeforeGamingSuperSkillEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingSuperSkill?.Invoke(sender, e);
        }

        public void OnAfterGamingSuperSkillEvent(object sender, GamingEventArgs e)
        {
            AfterGamingSuperSkill?.Invoke(sender, e);
        }

        public void OnSucceedGamingSuperSkillEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingSuperSkill?.Invoke(sender, e);
        }

        public void OnFailedGamingSuperSkillEvent(object sender, GamingEventArgs e)
        {
            FailedGamingSuperSkill?.Invoke(sender, e);
        }

        public void OnBeforeGamingPauseEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingPause?.Invoke(sender, e);
        }

        public void OnAfterGamingPauseEvent(object sender, GamingEventArgs e)
        {
            AfterGamingPause?.Invoke(sender, e);
        }

        public void OnSucceedGamingPauseEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingPause?.Invoke(sender, e);
        }

        public void OnFailedGamingPauseEvent(object sender, GamingEventArgs e)
        {
            FailedGamingPause?.Invoke(sender, e);
        }

        public void OnBeforeGamingUnpauseEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingUnpause?.Invoke(sender, e);
        }

        public void OnAfterGamingUnpauseEvent(object sender, GamingEventArgs e)
        {
            AfterGamingUnpause?.Invoke(sender, e);
        }

        public void OnSucceedGamingUnpauseEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingUnpause?.Invoke(sender, e);
        }

        public void OnFailedGamingUnpauseEvent(object sender, GamingEventArgs e)
        {
            FailedGamingUnpause?.Invoke(sender, e);
        }

        public void OnBeforeGamingSurrenderEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingSurrender?.Invoke(sender, e);
        }

        public void OnAfterGamingSurrenderEvent(object sender, GamingEventArgs e)
        {
            AfterGamingSurrender?.Invoke(sender, e);
        }

        public void OnSucceedGamingSurrenderEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingSurrender?.Invoke(sender, e);
        }

        public void OnFailedGamingSurrenderEvent(object sender, GamingEventArgs e)
        {
            FailedGamingSurrender?.Invoke(sender, e);
        }

        public void OnBeforeGamingUpdateInfoEvent(object sender, GamingEventArgs e)
        {
            BeforeGamingUpdateInfo?.Invoke(sender, e);
        }

        public void OnAfterGamingUpdateInfoEvent(object sender, GamingEventArgs e)
        {
            AfterGamingUpdateInfo?.Invoke(sender, e);
        }

        public void OnSucceedGamingUpdateInfoEvent(object sender, GamingEventArgs e)
        {
            SucceedGamingUpdateInfo?.Invoke(sender, e);
        }

        public void OnFailedGamingUpdateInfoEvent(object sender, GamingEventArgs e)
        {
            FailedGamingUpdateInfo?.Invoke(sender, e);
        }
    }
}
