using Milimoe.FunGame.Core.Api.Utility;
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
        /// 模组的容纳人数
        /// </summary>
        public abstract int MaxUsers { get; }

        /// <summary>
        /// 是否隐藏主界面
        /// </summary>
        public abstract bool HideMain { get; }

        /// <summary>
        /// 是否连接其他的服务器模组
        /// </summary>
        public bool IsConnectToOtherServerModule { get; set; } = false;

        /// <summary>
        /// 如果将 <see cref="IsConnectToOtherServerModule"/> 设置为true，那么此属性必须指定一个存在的服务器模组的 <see cref="Name"/> 名称。
        /// </summary>
        public string AssociatedServerModuleName
        {
            get => IsConnectToOtherServerModule ? _associatedServerModuleName : Name;
            set => _associatedServerModuleName = value;
        }

        /// <summary>
        /// 记录该模组的加载器
        /// </summary>
        public GameModuleLoader? ModuleLoader { get; set; } = null;

        /// <summary>
        /// 包含了一些常用方法的控制器
        /// </summary>
        public AddonController<IGameModule> Controller
        {
            get => _controller ?? throw new NotImplementedException();
            internal set => _controller = value;
        }

        /// <summary>
        /// base控制器，没有DataRequest
        /// </summary>
        BaseAddonController<IGameModule> IAddonController<IGameModule>.Controller
        {
            get => Controller;
            set => _controller = (AddonController<IGameModule>?)value;
        }

        /// <summary>
        /// 控制器内部变量
        /// </summary>
        private AddonController<IGameModule>? _controller;

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
        private bool _isLoaded = false;

        /// <summary>
        /// 加载模组
        /// </summary>
        public bool Load(params object[] objs)
        {
            if (_isLoaded)
            {
                return false;
            }
            // BeforeLoad可以阻止加载此模组
            if (BeforeLoad(objs))
            {
                // 模组加载后，不允许再次加载此模组
                _isLoaded = true;
                // 初始化此模组（传入委托或者Model）
                Init(objs);
                // 触发绑定事件
                BindEvent();
            }
            return _isLoaded;
        }

        /// <summary>
        /// 卸载模组
        /// </summary>
        /// <param name="objs"></param>
        public void UnLoad(params object[] objs)
        {
            BindEvent(false);
        }

        /// <summary>
        /// 模组完全加载后需要做的事
        /// </summary>
        public virtual void AfterLoad(GameModuleLoader loader, params object[] args)
        {
            // override
        }

        /// <summary>
        /// 允许返回false来阻止加载此模组
        /// </summary>
        /// <returns></returns>
        protected virtual bool BeforeLoad(params object[] objs)
        {
            return true;
        }

        /// <summary>
        /// 传递委托以便让模组调用
        /// </summary>
        private void Init(params object[] objs)
        {
            if (objs.Length > 0 && objs[0] is Session session)
            {
                _session = session;
            }
            if (objs.Length > 1 && objs[1] is FunGameConfig config)
            {
                _config = config;
            }
        }

        /// <summary>
        /// Session对象
        /// </summary>
        protected Session _session = new();

        /// <summary>
        /// Config对象
        /// </summary>
        protected FunGameConfig _config = new();

        /// <summary>
        /// 关联的服务器模组名称
        /// </summary>
        private string _associatedServerModuleName = "";

        /// <summary>
        /// 绑定事件。在<see cref="BeforeLoad"/>后触发
        /// </summary>
        private void BindEvent(bool isAdd = true)
        {
            if (this is IGamingConnectEvent connect)
            {
                if (isAdd)
                {
                    GamingConnect += connect.GamingConnectEvent;
                }
                else
                {
                    GamingConnect -= connect.GamingConnectEvent;
                }
            }

            if (this is IGamingDisconnectEvent disconnect)
            {
                if (isAdd)
                {
                    GamingDisconnect += disconnect.GamingDisconnectEvent;
                }
                else
                {
                    GamingDisconnect -= disconnect.GamingDisconnectEvent;
                }
            }

            if (this is IGamingReconnectEvent reconnect)
            {
                if (isAdd)
                {
                    GamingReconnect += reconnect.GamingReconnectEvent;
                }
                else
                {
                    GamingReconnect -= reconnect.GamingReconnectEvent;
                }
            }

            if (this is IGamingBanCharacterEvent ban)
            {
                if (isAdd)
                {
                    GamingBanCharacter += ban.GamingBanCharacterEvent;
                }
                else
                {
                    GamingBanCharacter -= ban.GamingBanCharacterEvent;
                }
            }

            if (this is IGamingPickCharacterEvent pick)
            {
                if (isAdd)
                {
                    GamingPickCharacter += pick.GamingPickCharacterEvent;
                }
                else
                {
                    GamingPickCharacter -= pick.GamingPickCharacterEvent;
                }
            }

            if (this is IGamingRandomEvent random)
            {
                if (isAdd)
                {
                    GamingRandom += random.GamingRandomEvent;
                }
                else
                {
                    GamingRandom -= random.GamingRandomEvent;
                }
            }

            if (this is IGamingRoundEvent round)
            {
                if (isAdd)
                {
                    GamingRound += round.GamingRoundEvent;
                }
                else
                {
                    GamingRound -= round.GamingRoundEvent;
                }
            }

            if (this is IGamingLevelUpEvent levelUp)
            {
                if (isAdd)
                {
                    GamingLevelUp += levelUp.GamingLevelUpEvent;
                }
                else
                {
                    GamingLevelUp -= levelUp.GamingLevelUpEvent;
                }
            }

            if (this is IGamingMoveEvent move)
            {
                if (isAdd)
                {
                    GamingMove += move.GamingMoveEvent;
                }
                else
                {
                    GamingMove -= move.GamingMoveEvent;
                }
            }

            if (this is IGamingAttackEvent attack)
            {
                if (isAdd)
                {
                    GamingAttack += attack.GamingAttackEvent;
                }
                else
                {
                    GamingAttack -= attack.GamingAttackEvent;
                }
            }

            if (this is IGamingSkillEvent skill)
            {
                if (isAdd)
                {
                    GamingSkill += skill.GamingSkillEvent;
                }
                else
                {
                    GamingSkill -= skill.GamingSkillEvent;
                }
            }

            if (this is IGamingItemEvent item)
            {
                if (isAdd)
                {
                    GamingItem += item.GamingItemEvent;
                }
                else
                {
                    GamingItem -= item.GamingItemEvent;
                }
            }

            if (this is IGamingMagicEvent magic)
            {
                if (isAdd)
                {
                    GamingMagic += magic.GamingMagicEvent;
                }
                else
                {
                    GamingMagic -= magic.GamingMagicEvent;
                }
            }

            if (this is IGamingBuyEvent buy)
            {
                if (isAdd)
                {
                    GamingBuy += buy.GamingBuyEvent;
                }
                else
                {
                    GamingBuy -= buy.GamingBuyEvent;
                }
            }

            if (this is IGamingSuperSkillEvent super)
            {
                if (isAdd)
                {
                    GamingSuperSkill += super.GamingSuperSkillEvent;
                }
                else
                {
                    GamingSuperSkill -= super.GamingSuperSkillEvent;
                }
            }

            if (this is IGamingPauseEvent pause)
            {
                if (isAdd)
                {
                    GamingPause += pause.GamingPauseEvent;
                }
                else
                {
                    GamingPause -= pause.GamingPauseEvent;
                }
            }

            if (this is IGamingUnpauseEvent unpause)
            {
                if (isAdd)
                {
                    GamingUnpause += unpause.GamingUnpauseEvent;
                }
                else
                {
                    GamingUnpause -= unpause.GamingUnpauseEvent;
                }
            }

            if (this is IGamingSurrenderEvent surrender)
            {
                if (isAdd)
                {
                    GamingSurrender += surrender.GamingSurrenderEvent;
                }
                else
                {
                    GamingSurrender -= surrender.GamingSurrenderEvent;
                }
            }

            if (this is IGamingUpdateInfoEvent update)
            {
                if (isAdd)
                {
                    GamingUpdateInfo += update.GamingUpdateInfoEvent;
                }
                else
                {
                    GamingUpdateInfo -= update.GamingUpdateInfoEvent;
                }
            }

            if (this is IGamingPunishEvent punish)
            {
                if (isAdd)
                {
                    GamingPunish += punish.GamingPunishEvent;
                }
                else
                {
                    GamingPunish -= punish.GamingPunishEvent;
                }
            }
        }

        public event IGamingEventHandler.GamingEventHandler? GamingConnect;
        public event IGamingEventHandler.GamingEventHandler? GamingDisconnect;
        public event IGamingEventHandler.GamingEventHandler? GamingReconnect;
        public event IGamingEventHandler.GamingEventHandler? GamingBanCharacter;
        public event IGamingEventHandler.GamingEventHandler? GamingPickCharacter;
        public event IGamingEventHandler.GamingEventHandler? GamingRandom;
        public event IGamingEventHandler.GamingEventHandler? GamingRound;
        public event IGamingEventHandler.GamingEventHandler? GamingLevelUp;
        public event IGamingEventHandler.GamingEventHandler? GamingMove;
        public event IGamingEventHandler.GamingEventHandler? GamingAttack;
        public event IGamingEventHandler.GamingEventHandler? GamingSkill;
        public event IGamingEventHandler.GamingEventHandler? GamingItem;
        public event IGamingEventHandler.GamingEventHandler? GamingMagic;
        public event IGamingEventHandler.GamingEventHandler? GamingBuy;
        public event IGamingEventHandler.GamingEventHandler? GamingSuperSkill;
        public event IGamingEventHandler.GamingEventHandler? GamingPause;
        public event IGamingEventHandler.GamingEventHandler? GamingUnpause;
        public event IGamingEventHandler.GamingEventHandler? GamingSurrender;
        public event IGamingEventHandler.GamingEventHandler? GamingUpdateInfo;
        public event IGamingEventHandler.GamingEventHandler? GamingPunish;

        public void OnGamingConnectEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingConnect?.Invoke(sender, e, data);
        }

        public void OnGamingDisconnectEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingDisconnect?.Invoke(sender, e, data);
        }

        public void OnGamingReconnectEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingReconnect?.Invoke(sender, e, data);
        }

        public void OnGamingBanCharacterEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingBanCharacter?.Invoke(sender, e, data);
        }

        public void OnGamingPickCharacterEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingPickCharacter?.Invoke(sender, e, data);
        }

        public void OnGamingRandomEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingRandom?.Invoke(sender, e, data);
        }

        public void OnGamingRoundEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingRound?.Invoke(sender, e, data);
        }

        public void OnGamingLevelUpEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingLevelUp?.Invoke(sender, e, data);
        }

        public void OnGamingMoveEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingMove?.Invoke(sender, e, data);
        }

        public void OnGamingAttackEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingAttack?.Invoke(sender, e, data);
        }

        public void OnGamingSkillEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingSkill?.Invoke(sender, e, data);
        }

        public void OnGamingItemEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingItem?.Invoke(sender, e, data);
        }

        public void OnGamingMagicEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingMagic?.Invoke(sender, e, data);
        }

        public void OnGamingBuyEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingBuy?.Invoke(sender, e, data);
        }

        public void OnGamingSuperSkillEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingSuperSkill?.Invoke(sender, e, data);
        }

        public void OnGamingPauseEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingPause?.Invoke(sender, e, data);
        }

        public void OnGamingUnpauseEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingUnpause?.Invoke(sender, e, data);
        }

        public void OnGamingSurrenderEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingSurrender?.Invoke(sender, e, data);
        }

        public void OnGamingUpdateInfoEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingUpdateInfo?.Invoke(sender, e, data);
        }

        public void OnGamingPunishEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            GamingPunish?.Invoke(sender, e, data);
        }
    }
}
