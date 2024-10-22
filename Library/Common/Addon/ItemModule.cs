using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public abstract class ItemModule : IAddon
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
        /// 此模组中包含的物品
        /// </summary>
        public abstract List<Item> Items { get; }

        /// <summary>
        /// 加载标记
        /// </summary>
        private bool IsLoaded = false;

        /// <summary>
        /// [可选实现] 在使用 <see cref="EntityModuleConfig{Item}"/> 后，可以按 id + name + type 来匹配已编码的物品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual Item? GetItem(long id, string name, ItemType type)
        {
            return null;
        }

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
    }
}
