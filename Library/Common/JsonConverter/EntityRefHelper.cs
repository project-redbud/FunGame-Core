using System.Text.Json;
using FunGame.Core.Entity;

namespace FunGame.Core.Library.Common.JsonConverter
{
    /// <summary>
    /// 角色引用（轻量快照）的读写辅助：只保留展示所需字段，避免在回合记录中序列化完整的角色对象图
    /// </summary>
    internal static class CharacterRefHelper
    {
        public static void Write(Utf8JsonWriter writer, Character? character)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(Character.Guid), character?.Guid ?? Guid.Empty);
            writer.WriteString(nameof(Character.Name), character?.Name ?? "");
            writer.WriteString(nameof(Character.FirstName), character?.FirstName ?? "");
            writer.WriteString(nameof(Character.NickName), character?.NickName ?? "");
            writer.WriteString("UserName", character?.User?.Username ?? "");
            writer.WriteEndObject();
        }

        public static Character Read(ref Utf8JsonReader reader)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            return ReadElement(doc.RootElement);
        }

        public static void WriteList(Utf8JsonWriter writer, IEnumerable<Character> characters)
        {
            writer.WriteStartArray();
            foreach (Character character in characters)
            {
                Write(writer, character);
            }
            writer.WriteEndArray();
        }

        public static List<Character> ReadList(ref Utf8JsonReader reader)
        {
            List<Character> list = [];
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                list.Add(ReadElement(element));
            }
            return list;
        }

        internal static Character ReadElement(JsonElement root)
        {
            Character character = new();
            if (root.TryGetProperty(nameof(Character.Guid), out JsonElement guid) && guid.ValueKind == JsonValueKind.String)
            {
                character.Guid = guid.GetGuid();
            }
            if (root.TryGetProperty(nameof(Character.Name), out JsonElement name) && name.ValueKind == JsonValueKind.String)
            {
                character.Name = name.GetString() ?? "";
            }
            if (root.TryGetProperty(nameof(Character.FirstName), out JsonElement firstName) && firstName.ValueKind == JsonValueKind.String)
            {
                character.FirstName = firstName.GetString() ?? "";
            }
            if (root.TryGetProperty(nameof(Character.NickName), out JsonElement nickName) && nickName.ValueKind == JsonValueKind.String)
            {
                character.NickName = nickName.GetString() ?? "";
            }
            if (root.TryGetProperty("UserName", out JsonElement userName) && userName.ValueKind == JsonValueKind.String)
            {
                string username = userName.GetString() ?? "";
                if (username != "")
                {
                    character.User = new User() { Username = username };
                }
            }
            return character;
        }
    }

    /// <summary>
    /// 技能引用（轻量快照）的读写辅助：只保留 Id 与名称，供展示与消耗匹配
    /// </summary>
    internal static class SkillRefHelper
    {
        public static void Write(Utf8JsonWriter writer, Skill? skill)
        {
            writer.WriteStartObject();
            writer.WriteNumber(nameof(Skill.Id), skill?.Id ?? 0);
            writer.WriteString(nameof(Skill.Name), skill?.Name ?? "");
            writer.WriteEndObject();
        }

        public static Skill? Read(ref Utf8JsonReader reader)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            return ReadElement(doc.RootElement);
        }

        internal static Skill? ReadElement(JsonElement root)
        {
            long id = root.TryGetProperty(nameof(Skill.Id), out JsonElement idElement) && idElement.ValueKind == JsonValueKind.Number ? idElement.GetInt64() : 0;
            string name = root.TryGetProperty(nameof(Skill.Name), out JsonElement nameElement) && nameElement.ValueKind == JsonValueKind.String ? nameElement.GetString() ?? "" : "";
            return id == 0 && name == "" ? null : new OpenSkill(id, name, []);
        }
    }

    /// <summary>
    /// 物品引用（轻量快照）的读写辅助：只保留 Id 与名称，供展示与消耗匹配
    /// </summary>
    internal static class ItemRefHelper
    {
        public static void Write(Utf8JsonWriter writer, Item? item)
        {
            writer.WriteStartObject();
            writer.WriteNumber(nameof(Item.Id), item?.Id ?? 0);
            writer.WriteString(nameof(Item.Name), item?.Name ?? "");
            writer.WriteEndObject();
        }

        public static Item? Read(ref Utf8JsonReader reader)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            return ReadElement(doc.RootElement);
        }

        internal static Item? ReadElement(JsonElement root)
        {
            long id = root.TryGetProperty(nameof(Item.Id), out JsonElement idElement) && idElement.ValueKind == JsonValueKind.Number ? idElement.GetInt64() : 0;
            string name = root.TryGetProperty(nameof(Item.Name), out JsonElement nameElement) && nameElement.ValueKind == JsonValueKind.String ? nameElement.GetString() ?? "" : "";
            return id == 0 && name == "" ? null : new OpenItem(id, name, []);
        }
    }
}
