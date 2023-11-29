using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class GameModeLoader
    {
        public Dictionary<string, GameMode> Modes { get; } = [];
        public Dictionary<string, GameMap> Maps { get; } = [];
        public List<Character> Characters { get; } = [];
        public List<Skill> Skills { get; } = [];
        public List<Item> Items { get; } = [];

        private GameModeLoader()
        {

        }

        public static GameModeLoader LoadGameModes(params object[] objs)
        {
            GameModeLoader loader = new();
            AddonManager.LoadGameModes(loader.Modes, loader.Characters, loader.Skills, loader.Items, objs);
            AddonManager.LoadGameMaps(loader.Maps, objs);
            return loader;
        }

        public GameMode this[string name]
        {
            get
            {
                return Modes[name];
            }
            set
            {
                Modes.TryAdd(name, value);
            }
        }

        public GameMap GetGameMap(string name)
        {
            return Maps[name];
        }

        public void OnBeforeConnectEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingConnectEvent(sender, e);
            });
        }

        public void OnAfterConnectEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingConnectEvent(sender, e);
            });
        }

        public void OnSucceedConnectEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingConnectEvent(sender, e);
            });
        }

        public void OnFailedConnectEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingConnectEvent(sender, e);
            });
        }

        public void OnBeforeDisonnectEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingDisconnectEvent(sender, e);
            });
        }

        public void OnAfterDisconnectEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingDisconnectEvent(sender, e);
            });
        }

        public void OnSucceedDisconnectEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingDisconnectEvent(sender, e);
            });
        }

        public void OnFailedDisconnectEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingDisconnectEvent(sender, e);
            });
        }

        public void OnBeforeReconnectEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingReconnectEvent(sender, e);
            });
        }

        public void OnAfterReconnectEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingReconnectEvent(sender, e);
            });
        }

        public void OnSucceedReconnectEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingReconnectEvent(sender, e);
            });
        }

        public void OnFailedReconnectEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingReconnectEvent(sender, e);
            });
        }

        public void OnBeforeBanCharacterEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingBanCharacterEvent(sender, e);
            });
        }

        public void OnAfterBanCharacterEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingBanCharacterEvent(sender, e);
            });
        }

        public void OnSucceedBanCharacterEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingBanCharacterEvent(sender, e);
            });
        }

        public void OnFailedBanCharacterEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingBanCharacterEvent(sender, e);
            });
        }

        public void OnBeforePickCharacterEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingPickCharacterEvent(sender, e);
            });
        }

        public void OnAfterPickCharacterEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingPickCharacterEvent(sender, e);
            });
        }

        public void OnSucceedPickCharacterEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingPickCharacterEvent(sender, e);
            });
        }

        public void OnFailedPickCharacterEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingPickCharacterEvent(sender, e);
            });
        }

        public void OnBeforeRandomEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingRandomEvent(sender, e);
            });
        }

        public void OnAfterRandomEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingRandomEvent(sender, e);
            });
        }

        public void OnSucceedRandomEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingRandomEvent(sender, e);
            });
        }

        public void OnFailedRandomEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingRandomEvent(sender, e);
            });
        }

        public void OnBeforeMoveEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingMoveEvent(sender, e);
            });
        }

        public void OnAfterMoveEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingMoveEvent(sender, e);
            });
        }

        public void OnSucceedMoveEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingMoveEvent(sender, e);
            });
        }

        public void OnFailedMoveEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingMoveEvent(sender, e);
            });
        }

        public void OnBeforeAttackEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingAttackEvent(sender, e);
            });
        }

        public void OnAfterAttackEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingAttackEvent(sender, e);
            });
        }

        public void OnSucceedAttackEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingAttackEvent(sender, e);
            });
        }

        public void OnFailedAttackEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingAttackEvent(sender, e);
            });
        }

        public void OnBeforeSkillEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingSkillEvent(sender, e);
            });
        }

        public void OnAfterSkillEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingSkillEvent(sender, e);
            });
        }

        public void OnSucceedSkillEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingSkillEvent(sender, e);
            });
        }

        public void OnFailedSkillEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingSkillEvent(sender, e);
            });
        }

        public void OnBeforeItemEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingItemEvent(sender, e);
            });
        }

        public void OnAfterItemEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingItemEvent(sender, e);
            });
        }

        public void OnSucceedItemEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingItemEvent(sender, e);
            });
        }

        public void OnFailedItemEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingItemEvent(sender, e);
            });
        }

        public void OnBeforeMagicEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingMagicEvent(sender, e);
            });
        }

        public void OnAfterMagicEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingMagicEvent(sender, e);
            });
        }

        public void OnSucceedMagicEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingMagicEvent(sender, e);
            });
        }

        public void OnFailedMagicEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingMagicEvent(sender, e);
            });
        }

        public void OnBeforeBuyEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingBuyEvent(sender, e);
            });
        }

        public void OnAfterBuyEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingBuyEvent(sender, e);
            });
        }

        public void OnSucceedBuyEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingBuyEvent(sender, e);
            });
        }

        public void OnFailedBuyEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingBuyEvent(sender, e);
            });
        }

        public void OnBeforeSuperSkillEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingSuperSkillEvent(sender, e);
            });
        }

        public void OnAfterSuperSkillEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingSuperSkillEvent(sender, e);
            });
        }

        public void OnSucceedSuperSkillEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingSuperSkillEvent(sender, e);
            });
        }

        public void OnFailedSuperSkillEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingSuperSkillEvent(sender, e);
            });
        }

        public void OnBeforePauseEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingPauseEvent(sender, e);
            });
        }

        public void OnAfterPauseEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingPauseEvent(sender, e);
            });
        }

        public void OnSucceedPauseEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingPauseEvent(sender, e);
            });
        }

        public void OnFailedPauseEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingPauseEvent(sender, e);
            });
        }

        public void OnBeforeUnpauseEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingUnpauseEvent(sender, e);
            });
        }

        public void OnAfterUnpauseEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingUnpauseEvent(sender, e);
            });
        }

        public void OnSucceedUnpauseEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingUnpauseEvent(sender, e);
            });
        }

        public void OnFailedUnpauseEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingUnpauseEvent(sender, e);
            });
        }

        public void OnBeforeSurrenderEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingSurrenderEvent(sender, e);
            });
        }

        public void OnAfterSurrenderEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingSurrenderEvent(sender, e);
            });
        }

        public void OnSucceedSurrenderEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingSurrenderEvent(sender, e);
            });
        }

        public void OnFailedSurrenderEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingSurrenderEvent(sender, e);
            });
        }

        public void OnBeforeUpdateInfoEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnBeforeGamingUpdateInfoEvent(sender, e);
            });
        }

        public void OnAfterUpdateInfoEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnAfterGamingUpdateInfoEvent(sender, e);
            });
        }

        public void OnSucceedUpdateInfoEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnSucceedGamingUpdateInfoEvent(sender, e);
            });
        }

        public void OnFailedUpdateInfoEvent(object sender, GamingEventArgs e)
        {
            Parallel.ForEach(Modes.Values, mode =>
            {
                mode.OnFailedGamingUpdateInfoEvent(sender, e);
            });
        }
    }
}
