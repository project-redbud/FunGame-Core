﻿using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class ItemConverter : BaseEntityConverter<Item>
    {
        public override Item NewInstance()
        {
            return new();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Item result)
        {
            switch (propertyName)
            {
                case nameof(Item.Id):
                    result.Id = reader.GetInt64();
                    break;
                case nameof(Item.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(Item.Description):
                    result.Description = reader.GetString() ?? "";
                    break;
                case nameof(Item.GeneralDescription):
                    result.GeneralDescription = reader.GetString() ?? "";
                    break;
                case nameof(Item.BackgroundStory):
                    result.BackgroundStory = reader.GetString() ?? "";
                    break;
                case nameof(Item.ItemType):
                    result.ItemType = (ItemType)reader.GetInt32();
                    break;
                case nameof(Item.WeaponType):
                    result.WeaponType = (WeaponType)reader.GetInt32();
                    break;
                case nameof(Item.EquipSlotType):
                    result.EquipSlotType = (EquipSlotType)reader.GetInt32();
                    break;
                case nameof(Item.IsInGameItem):
                    result.IsInGameItem = reader.GetBoolean();
                    break;
                case nameof(Item.Equipable):
                    result.Equipable = reader.GetBoolean();
                    break;
                case nameof(Item.IsPurchasable):
                    result.IsPurchasable = reader.GetBoolean();
                    break;
                case nameof(Item.Price):
                    result.Price = reader.GetDouble();
                    break;
                case nameof(Item.IsSellable):
                    result.IsSellable = reader.GetBoolean();
                    break;
                case nameof(Item.NextSellableTime):
                    string dateString = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(dateString, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime sellableDate))
                    {
                        result.NextSellableTime = sellableDate;
                    }
                    else
                    {
                        result.NextSellableTime = DateTime.MinValue;
                    }
                    break;
                case nameof(Item.IsTradable):
                    result.IsTradable = reader.GetBoolean();
                    break;
                case nameof(Item.NextTradableTime):
                    dateString = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(dateString, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime tradableDate))
                    {
                        result.NextTradableTime = tradableDate;
                    }
                    else
                    {
                        result.NextTradableTime = DateTime.MinValue;
                    }
                    break;
                case nameof(Item.Skills):
                    SkillGroup skills = NetworkUtility.JsonDeserialize<SkillGroup>(ref reader, options) ?? new();
                    result.Skills.Active = skills.Active;
                    result.Skills.Passives = skills.Passives;
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Item value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Item.Id), (int)value.Id);
            writer.WriteString(nameof(Item.Name), value.Name);
            writer.WriteString(nameof(Item.Description), value.Description);
            writer.WriteString(nameof(Item.GeneralDescription), value.GeneralDescription);
            writer.WriteString(nameof(Item.BackgroundStory), value.BackgroundStory);
            writer.WriteNumber(nameof(Item.ItemType), (int)value.ItemType);
            writer.WriteNumber(nameof(Item.WeaponType), (int)value.WeaponType);
            writer.WriteNumber(nameof(Item.EquipSlotType), (int)value.EquipSlotType);
            writer.WriteBoolean(nameof(Item.IsInGameItem), value.IsInGameItem);
            writer.WriteBoolean(nameof(Item.Equipable), value.Equipable);
            writer.WriteBoolean(nameof(Item.IsPurchasable), value.IsPurchasable);
            writer.WriteNumber(nameof(Item.Price), value.Price);
            if (!value.IsSellable)
            {
                writer.WriteBoolean(nameof(Item.IsSellable), value.IsSellable);
                writer.WriteString(nameof(Item.NextSellableTime), value.NextSellableTime.ToString(General.GeneralDateTimeFormat));
            }
            if (!value.IsTradable)
            {
                writer.WriteBoolean(nameof(Item.IsTradable), value.IsTradable);
                writer.WriteString(nameof(Item.NextTradableTime), value.NextTradableTime.ToString(General.GeneralDateTimeFormat));
            }
            writer.WritePropertyName(nameof(Item.Skills));
            JsonSerializer.Serialize(writer, value.Skills, options);

            writer.WriteEndObject();
        }
    }
}
