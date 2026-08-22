using System.Text.Json;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

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
    /// 团队引用（轻量快照）的读写辅助：保留 Id、名称、得分、胜者标记与成员角色引用
    /// </summary>
    internal static class TeamRefHelper
    {
        public static void Write(Utf8JsonWriter writer, Team? team)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(Team.Id), team?.Id ?? Guid.Empty);
            writer.WriteString(nameof(Team.Name), team?.Name ?? "");
            writer.WriteNumber(nameof(Team.Score), team?.Score ?? 0);
            writer.WriteBoolean(nameof(Team.IsWinner), team?.IsWinner ?? false);
            writer.WritePropertyName(nameof(Team.Members));
            CharacterRefHelper.WriteList(writer, team?.Members ?? []);
            writer.WriteEndObject();
        }

        public static Team? Read(ref Utf8JsonReader reader)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            return ReadElement(doc.RootElement);
        }

        internal static Team? ReadElement(JsonElement root)
        {
            Guid id = root.TryGetProperty(nameof(Team.Id), out JsonElement idElement) && idElement.ValueKind == JsonValueKind.String ? idElement.GetGuid() : Guid.Empty;
            string name = root.TryGetProperty(nameof(Team.Name), out JsonElement nameElement) && nameElement.ValueKind == JsonValueKind.String ? nameElement.GetString() ?? "" : "";
            Team team = new(name, []) { Id = id };
            if (root.TryGetProperty(nameof(Team.Score), out JsonElement scoreElement) && scoreElement.ValueKind == JsonValueKind.Number)
            {
                team.Score = scoreElement.GetInt32();
            }
            if (root.TryGetProperty(nameof(Team.IsWinner), out JsonElement winnerElement) && winnerElement.ValueKind == JsonValueKind.True || winnerElement.ValueKind == JsonValueKind.False)
            {
                team.IsWinner = winnerElement.GetBoolean();
            }
            if (root.TryGetProperty(nameof(Team.Members), out JsonElement membersElement) && membersElement.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement element in membersElement.EnumerateArray())
                {
                    team.Members.Add(CharacterRefHelper.ReadElement(element));
                }
            }
            return team;
        }
    }

    /// <summary>
    /// 技能引用（轻量快照）的读写辅助：保留 Guid、Id、名称与类型，供展示与消耗匹配
    /// </summary>
    internal static class SkillRefHelper
    {
        public static void Write(Utf8JsonWriter writer, Skill? skill)
        {
            if (skill == null)
            {
                // null 技能（如普通攻击的操作记录）写 JSON null，避免占位对象被展示端误读为真实技能
                writer.WriteNullValue();
                return;
            }
            writer.WriteStartObject();
            writer.WriteString(nameof(Skill.Guid), skill.Guid);
            writer.WriteNumber(nameof(Skill.Id), skill.Id);
            writer.WriteString(nameof(Skill.Name), skill.Name);
            writer.WriteNumber(nameof(Skill.SkillType), (int)skill.SkillType);
            writer.WriteEndObject();
        }

        public static Skill? Read(ref Utf8JsonReader reader)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            return ReadElement(doc.RootElement);
        }

        internal static Skill? ReadElement(JsonElement root)
        {
            if (root.ValueKind != JsonValueKind.Object)
            {
                return null;
            }
            Guid guid = root.TryGetProperty(nameof(Skill.Guid), out JsonElement guidElement) && guidElement.ValueKind == JsonValueKind.String ? guidElement.GetGuid() : Guid.Empty;
            long id = root.TryGetProperty(nameof(Skill.Id), out JsonElement idElement) && idElement.ValueKind == JsonValueKind.Number ? idElement.GetInt64() : 0;
            string name = root.TryGetProperty(nameof(Skill.Name), out JsonElement nameElement) && nameElement.ValueKind == JsonValueKind.String ? nameElement.GetString() ?? "" : "";
            SkillType skillType = root.TryGetProperty(nameof(Skill.SkillType), out JsonElement stElement) && stElement.ValueKind == JsonValueKind.Number ? (SkillType)stElement.GetInt32() : SkillType.Magic;
            return id == 0 && name == "" ? null : new OpenSkill(id, name, []) { SkillType = skillType, Guid = guid };
        }
    }

    /// <summary>
    /// 物品引用（轻量快照）的读写辅助：保留 Guid、Id 与名称，供展示与消耗匹配
    /// </summary>
    internal static class ItemRefHelper
    {
        public static void Write(Utf8JsonWriter writer, Item? item)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(Item.Guid), item?.Guid ?? Guid.Empty);
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
            Guid guid = root.TryGetProperty(nameof(Item.Guid), out JsonElement guidElement) && guidElement.ValueKind == JsonValueKind.String ? guidElement.GetGuid() : Guid.Empty;
            long id = root.TryGetProperty(nameof(Item.Id), out JsonElement idElement) && idElement.ValueKind == JsonValueKind.Number ? idElement.GetInt64() : 0;
            string name = root.TryGetProperty(nameof(Item.Name), out JsonElement nameElement) && nameElement.ValueKind == JsonValueKind.String ? nameElement.GetString() ?? "" : "";
            return id == 0 && name == "" ? null : new OpenItem(id, name, []) { Guid = guid };
        }
    }
}
