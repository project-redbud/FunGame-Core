using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// 此标记意味着属性允许初始设定，但不是强制的。适用于 <see cref="BaseEntity"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class InitOptional : Attribute
    {
        public InitOptional() { }
    }

    /// <summary>
    /// 此标记意味着属性需要经过初始设定。适用于 <see cref="BaseEntity"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class InitRequired : Attribute
    {
        public InitRequired() { }
    }
}
