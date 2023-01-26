using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NoMoreRogueClans
{
    internal class RogueClanJoinKingdomsBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.OnClanDailyTick));
        }

        private void OnClanDailyTick(Clan clan)
        {
            // Only interested in AI noble clans
            if (clan.IsClanTypeMercenary || clan.IsEliminated || clan.IsRebelClan || clan == Clan.PlayerClan || clan.IsMinorFaction || clan == null)
            {
                return;
            }

            // Only interested in those without a kingdom
            if (clan.Kingdom != null)
            {
                return;
            }

            if (clan.IsNoble)
            {
                Kingdom kingdomToJoin = getKingdomToJoin(clan.Culture);

                if (kingdomToJoin == null)
                {
                    // All kingdoms are gone.
                    return;
                }

                clan.Kingdom = kingdomToJoin;
                InformationMessage msg = new InformationMessage(
                                    "Clan " + clan.GetName() + " have joined the " + kingdomToJoin.InformalName + "."
                                );
                InformationManager.DisplayMessage(msg);
            }
        }

        private Kingdom getKingdomToJoin(CultureObject clanCulture)
        {
            // Kingdom selection priority:
            // 1) Weakest kingdom of clan's culture
            // 2) Weakest kingdom over all

            List<Kingdom> activeKingdoms = new List<Kingdom>();
            List<Kingdom> kingdoms = new List<Kingdom>();
            Kingdom weakest = null;

            foreach (Kingdom k in Kingdom.All)
            {
                if (!k.IsEliminated)
                {
                    activeKingdoms.Add(k);
                }
            }

            foreach (Kingdom k in activeKingdoms)
            {
                if (k.Culture == clanCulture)
                {
                    kingdoms.Add(k);
                }
            }

            if (kingdoms.Count != 0)
            {
                weakest = getWeakestKingdomInList(kingdoms);
            }

            if (weakest == null)
            {
                weakest = getWeakestKingdomInList(activeKingdoms);
            }

            return weakest;
        }

        private Kingdom getWeakestKingdomInList(List<Kingdom> kingdoms)
        {
            if (kingdoms == null || kingdoms.Count == 0)
            {
                return null;
            }

            float minStrength = float.MaxValue;
            Kingdom weakestKingdom = null;

            foreach (Kingdom k in kingdoms)
            {
                if (k.TotalStrength < minStrength)
                {
                    minStrength = k.TotalStrength;
                    weakestKingdom = k;
                }
            }

            return weakestKingdom;
        }

        public override void SyncData(IDataStore dataStore) {}
    }
}
