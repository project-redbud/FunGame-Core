using System.Text.Json;
using FunGame.Core.Entity;
using FunGame.Core.Library.Architecture;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Library.Common.JsonConverter
{
    public class RankingEntryConverter : BaseEntityConverter<RankingEntry>
    {
        public override RankingEntry NewInstance()
        {
            return new RankingEntry();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref RankingEntry result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(RankingEntry.Rank):
                    result.Rank = reader.GetInt32();
                    break;
                case nameof(RankingEntry.IsWinner):
                    result.IsWinner = reader.GetBoolean();
                    break;
                case nameof(RankingEntry.IsTeam):
                    result.IsTeam = reader.GetBoolean();
                    break;
                case nameof(RankingEntry.Character):
                    result.Character = CharacterRefHelper.Read(ref reader);
                    break;
                case nameof(RankingEntry.Team):
                    result.Team = TeamRefHelper.Read(ref reader);
                    break;
                case nameof(RankingEntry.Kills):
                    result.Kills = reader.GetInt32();
                    break;
                case nameof(RankingEntry.Deaths):
                    result.Deaths = reader.GetInt32();
                    break;
                case nameof(RankingEntry.Assists):
                    result.Assists = reader.GetInt32();
                    break;
                case nameof(RankingEntry.FirstKills):
                    result.FirstKills = reader.GetInt32();
                    break;
                case nameof(RankingEntry.TotalEarnedMoney):
                    result.TotalEarnedMoney = reader.GetInt32();
                    break;
                case nameof(RankingEntry.MaxContinuousKilling):
                    result.MaxContinuousKilling = reader.GetInt32();
                    break;
                case nameof(RankingEntry.Score):
                    result.Score = reader.GetInt32();
                    break;

                default:
                    reader.Skip();
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, RankingEntry value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber(nameof(RankingEntry.Rank), value.Rank);
            writer.WriteBoolean(nameof(RankingEntry.IsWinner), value.IsWinner);
            writer.WriteBoolean(nameof(RankingEntry.IsTeam), value.IsTeam);
            writer.WritePropertyName(nameof(RankingEntry.Character));
            CharacterRefHelper.Write(writer, value.Character);
            writer.WritePropertyName(nameof(RankingEntry.Team));
            TeamRefHelper.Write(writer, value.Team);
            writer.WriteNumber(nameof(RankingEntry.Kills), value.Kills);
            writer.WriteNumber(nameof(RankingEntry.Deaths), value.Deaths);
            writer.WriteNumber(nameof(RankingEntry.Assists), value.Assists);
            writer.WriteNumber(nameof(RankingEntry.FirstKills), value.FirstKills);
            writer.WriteNumber(nameof(RankingEntry.TotalEarnedMoney), value.TotalEarnedMoney);
            writer.WriteNumber(nameof(RankingEntry.MaxContinuousKilling), value.MaxContinuousKilling);
            writer.WriteNumber(nameof(RankingEntry.Score), value.Score);
            writer.WriteEndObject();
        }
    }
}
