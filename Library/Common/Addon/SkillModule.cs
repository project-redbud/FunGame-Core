using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Addons;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public abstract class SkillModule : IAddon
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
        /// 此模组中包含的技能
        /// </summary>
        public abstract Dictionary<string, Skill> Skills { get; }

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
            if (BeforeLoad())
            {
                // 模组加载后，不允许再次加载此模组
                _isLoaded = true;
                // 注册工厂
                Factory.OpenFactory.RegisterFactory(SkillFactory());
                Factory.OpenFactory.RegisterFactory(EffectFactory());
                // 如果加载后需要执行代码，请重写AfterLoad方法
                AfterLoad();
            }
            return _isLoaded;
        }

        /// <summary>
        /// 卸载模组
        /// </summary>
        /// <param name="objs"></param>
        public void UnLoad(params object[] objs)
        {
            Factory.OpenFactory.UnRegisterFactory(SkillFactory());
            Factory.OpenFactory.UnRegisterFactory(EffectFactory());
        }

        /// <summary>
        /// 注册工厂
        /// </summary>
        protected abstract Factory.EntityFactoryDelegate<Skill> SkillFactory();

        /// <summary>
        /// 注册工厂（特效类）
        /// </summary>
        protected abstract Factory.EntityFactoryDelegate<Effect> EffectFactory();

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
    }
}
