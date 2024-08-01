using System.Collections;
using Milimoe.FunGame.Core.Controller;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public abstract class GameModule : IGameModule
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
        /// 默认地图
        /// </summary>
        public abstract string DefaultMap { get; }

        /// <summary>
        /// 模组的依赖集合
        /// </summary>
        public abstract GameModuleDepend GameModuleDepend { get; }

        /// <summary>
        /// 适用的房间模式
        /// </summary>
        public abstract RoomType RoomType { get; }

        /// <summary>
        /// 包含了一些常用方法的控制器
        /// </summary>
        public AddonController<IGameModule> Controller
        {
            get => _Controller ?? throw new NotImplementedException();
            internal set => _Controller = value;
        }

        /// <summary>
        /// base控制器，没有DataRequest
        /// </summary>
        BaseAddonController<IGameModule> IAddonController<IGameModule>.Controller
        {
            get => Controller;
            set => _Controller = (AddonController<IGameModule>?)value;
        }

        /// <summary>
        /// 控制器内部变量
        /// </summary>
        private AddonController<IGameModule>? _Controller;

        /// <summary>
        /// 必须重写此方法，游戏的主要逻辑写在这里面<para/>
        /// 此方法会在 <see cref="Gaming.StartGame"/> 时调用<para/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public abstract void StartGame(Gaming instance, params object[] args);

        /// <summary>
        /// 如模组有界面，请重写此方法
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual void StartUI(params object[] args)
        {

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
            if (objs.Length > 0) Session = (Session)objs[0];
            if (objs.Length > 1) Config = (FunGameConfig)objs[1];
        }

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

            if (this is IGamingRoundEvent)
            {
                IGamingRoundEvent bind = (IGamingRoundEvent)this;
                BeforeGamingRound += bind.BeforeGamingRoundEvent;
                AfterGamingRound += bind.AfterGamingRoundEvent;
                SucceedGamingRound += bind.SucceedGamingRoundEvent;
                FailedGamingRound += bind.FailedGamingRoundEvent;
            }

            if (this is IGamingLevelUpEvent)
            {
                IGamingLevelUpEvent bind = (IGamingLevelUpEvent)this;
                BeforeGamingLevelUp += bind.BeforeGamingLevelUpEvent;
                AfterGamingLevelUp += bind.AfterGamingLevelUpEvent;
                SucceedGamingLevelUp += bind.SucceedGamingLevelUpEvent;
                FailedGamingLevelUp += bind.FailedGamingLevelUpEvent;
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

            if (this is IGamingPunishEvent)
            {
                IGamingPunishEvent bind = (IGamingPunishEvent)this;
                BeforeGamingPunish += bind.BeforeGamingPunishEvent;
                AfterGamingPunish += bind.AfterGamingPunishEvent;
                SucceedGamingPunish += bind.SucceedGamingPunishEvent;
                FailedGamingPunish += bind.FailedGamingPunishEvent;
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
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingRound;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingRound;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingRound;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingRound;
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingLevelUp;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingLevelUp;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingLevelUp;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingLevelUp;
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
        public event IGamingEventHandler.BeforeEventHandler? BeforeGamingPunish;
        public event IGamingEventHandler.AfterEventHandler? AfterGamingPunish;
        public event IGamingEventHandler.SucceedEventHandler? SucceedGamingPunish;
        public event IGamingEventHandler.FailedEventHandler? FailedGamingPunish;

        public void OnBeforeGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingConnect?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingConnect?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingConnect?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingConnect?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingDisconnect?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingDisconnect?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingDisconnect?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingDisconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingDisconnect?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingReconnect?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingReconnect?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingReconnect?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingReconnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingReconnect?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingBanCharacter?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingBanCharacter?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingBanCharacter?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingBanCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingBanCharacter?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingPickCharacter?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingPickCharacter?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingPickCharacter?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingPickCharacterEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingPickCharacter?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingRandom?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingRandom?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingRandom?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingRandomEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingRandom?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingRound?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingRound?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingRound?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingRoundEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingRound?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingLevelUp?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingLevelUp?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingLevelUp?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingLevelUpEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingLevelUp?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingMove?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingMove?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingMove?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingMoveEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingMove?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingAttack?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingAttack?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingAttack?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingAttackEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingAttack?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingSkill?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingSkill?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingSkill?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingSkill?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingItemEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingItem?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingItemEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingItem?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingItemEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingItem?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingItemEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingItem?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingMagic?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingMagic?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingMagic?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingMagicEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingMagic?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingBuy?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingBuy?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingBuy?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingBuyEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingBuy?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingSuperSkill?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingSuperSkill?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingSuperSkill?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingSuperSkillEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingSuperSkill?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingPause?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingPause?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingPause?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingPauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingPause?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingUnpause?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingUnpause?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingUnpause?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingUnpauseEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingUnpause?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingSurrender?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingSurrender?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingSurrender?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingSurrenderEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingSurrender?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingUpdateInfo?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingUpdateInfo?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingUpdateInfo?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingUpdateInfo?.Invoke(sender, e, data, result);
        }

        public void OnBeforeGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            BeforeGamingPunish?.Invoke(sender, e, data, result);
        }

        public void OnAfterGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            AfterGamingPunish?.Invoke(sender, e, data, result);
        }

        public void OnSucceedGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            SucceedGamingPunish?.Invoke(sender, e, data, result);
        }

        public void OnFailedGamingPunishEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            FailedGamingPunish?.Invoke(sender, e, data, result);
        }
    }
}
