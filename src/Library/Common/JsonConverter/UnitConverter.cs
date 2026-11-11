using System.Text.Json;
using FunGame.Core.Entity;
using FunGame.Core.Library.Architecture;

namespace FunGame.Core.Library.Common.JsonConverter
{
    /// <summary>
    /// 单位转换器，与角色共用可写属性的持久化规则
    /// </summary>
    public sealed class UnitConverter : BaseEntityConverter<Unit>
    {
        private static readonly CharacterConverter CharacterFields = new();

        /// <summary>
        /// 创建用于读取属性的单位
        /// </summary>
        /// <returns>单位实例</returns>
        public override Unit NewInstance() => new();

        /// <summary>
        /// 读取单位的可写属性
        /// </summary>
        /// <param name="reader">JSON 读取器</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="options">序列化选项</param>
        /// <param name="result">单位实例</param>
        /// <param name="convertingContext">转换上下文</param>
        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Unit result, Dictionary<string, object> convertingContext)
        {
            Character character = result;
            CharacterFields.ReadPropertyName(ref reader, propertyName, options, ref character, convertingContext);
            result = (Unit)character;
        }

        /// <summary>
        /// 写入单位的可写属性
        /// </summary>
        /// <param name="writer">JSON 写入器</param>
        /// <param name="value">单位实例</param>
        /// <param name="options">序列化选项</param>
        public override void Write(Utf8JsonWriter writer, Unit value, JsonSerializerOptions options)
        {
            CharacterFields.Write(writer, value, options);
        }

        /// <summary>
        /// 在属性读取完成后回填当前资源
        /// </summary>
        /// <param name="result">单位实例</param>
        /// <param name="convertingContext">转换上下文</param>
        public override void AfterConvert(ref Unit result, Dictionary<string, object> convertingContext)
        {
            Character character = result;
            CharacterFields.AfterConvert(ref character, convertingContext);
            result = (Unit)character;
        }
    }
}
