﻿using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class ClubConverter : BaseEntityConverter<Club>
    {
        public override Club NewInstance()
        {
            return new();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Club result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(Club.Id):
                    result.Id = reader.GetInt64();
                    break;
                case nameof(Club.Guid):
                    result.Guid = NetworkUtility.JsonDeserialize<Guid>(ref reader, options);
                    break;
                case nameof(Club.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(Club.CreateTime):
                    string createTime = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(createTime, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime CreateTime))
                    {
                        result.CreateTime = CreateTime;
                    }
                    break;
                case nameof(Club.Prefix):
                    result.Prefix = reader.GetString() ?? "";
                    break;
                case nameof(Club.Description):
                    result.Description = reader.GetString() ?? "";
                    break;
                case nameof(Club.IsPublic):
                    result.IsPublic = reader.GetBoolean();
                    break;
                case nameof(Club.IsNeedApproval):
                    result.IsNeedApproval = reader.GetBoolean();
                    break;
                case nameof(Club.ClubPoins):
                    result.ClubPoins = reader.GetDouble();
                    break;
                case nameof(Club.Master):
                    long master = reader.GetInt64();
                    result.Master = new(master);
                    break;
                case nameof(Club.Admins):
                    List<long> admins = NetworkUtility.JsonDeserialize<List<long>>(ref reader, options) ?? [];
                    foreach (long id in admins)
                    {
                        result.Admins[id] = new(id);
                    }
                    break;
                case nameof(Club.Members):
                    List<long> members = NetworkUtility.JsonDeserialize<List<long>>(ref reader, options) ?? [];
                    foreach (long id in members)
                    {
                        result.Members[id] = new(id);
                    }
                    break;
                case nameof(Club.Applicants):
                    List<long> applicants = NetworkUtility.JsonDeserialize<List<long>>(ref reader, options) ?? [];
                    foreach (long id in applicants)
                    {
                        result.Applicants[id] = new(id);
                    }
                    break;
                case nameof(Club.Invitees):
                    List<long> invitees = NetworkUtility.JsonDeserialize<List<long>>(ref reader, options) ?? [];
                    foreach (long id in invitees)
                    {
                        result.Invitees[id] = new(id);
                    }
                    break;
                case nameof(Club.MemberJoinTime):
                    Dictionary<long, DateTime> memberJoinTime = NetworkUtility.JsonDeserialize<Dictionary<long, DateTime>>(ref reader, options) ?? [];
                    foreach (long id in memberJoinTime.Keys)
                    {
                        result.MemberJoinTime[id] = memberJoinTime[id];
                    }
                    break;
                case nameof(Club.ApplicationTime):
                    Dictionary<long, DateTime> applicationTime = NetworkUtility.JsonDeserialize<Dictionary<long, DateTime>>(ref reader, options) ?? [];
                    foreach (long id in applicationTime.Keys)
                    {
                        result.ApplicationTime[id] = applicationTime[id];
                    }
                    break;
                case nameof(Club.InvitedTime):
                    Dictionary<long, DateTime> invitedTime = NetworkUtility.JsonDeserialize<Dictionary<long, DateTime>>(ref reader, options) ?? [];
                    foreach (long id in invitedTime.Keys)
                    {
                        result.InvitedTime[id] = invitedTime[id];
                    }
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Club value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Club.Id), value.Id);
            writer.WritePropertyName(nameof(Club.Guid));
            JsonSerializer.Serialize(writer, value.Guid, options);
            writer.WriteString(nameof(Club.Name), value.Name);
            writer.WriteString(nameof(Club.CreateTime), value.CreateTime);
            writer.WriteString(nameof(Club.Prefix), value.Prefix);
            writer.WriteString(nameof(Club.Description), value.Description);
            writer.WriteBoolean(nameof(Club.IsPublic), value.IsPublic);
            writer.WriteBoolean(nameof(Club.IsNeedApproval), value.IsNeedApproval);
            writer.WriteNumber(nameof(Club.ClubPoins), value.ClubPoins);
            writer.WriteNumber(nameof(Club.Master), value.Master?.Id ?? 0);
            writer.WritePropertyName(nameof(Club.Admins));
            JsonSerializer.Serialize(writer, value.Admins.Keys, options);
            writer.WritePropertyName(nameof(Club.Members));
            JsonSerializer.Serialize(writer, value.Members.Keys, options);
            writer.WritePropertyName(nameof(Club.Applicants));
            JsonSerializer.Serialize(writer, value.Applicants.Keys, options);
            writer.WritePropertyName(nameof(Club.Invitees));
            JsonSerializer.Serialize(writer, value.Invitees.Keys, options);
            writer.WritePropertyName(nameof(Club.MemberJoinTime));
            JsonSerializer.Serialize(writer, value.MemberJoinTime, options);
            writer.WritePropertyName(nameof(Club.ApplicationTime));
            JsonSerializer.Serialize(writer, value.ApplicationTime, options);
            writer.WritePropertyName(nameof(Club.InvitedTime));
            JsonSerializer.Serialize(writer, value.InvitedTime, options);

            writer.WriteEndObject();
        }
    }
}
