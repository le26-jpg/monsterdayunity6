using System;
using System.Collections.Generic;
using Monsterday.Collection;
using Monsterday.Data;
using Monsterday.Economy;
using Monsterday.Save;

namespace Monsterday.Gacha
{
    public sealed class GachaResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public IReadOnlyList<MonsterDefinition> Monsters { get; set; } = Array.Empty<MonsterDefinition>();
        public int BonusShards { get; set; }
    }

    public sealed class GachaService
    {
        private readonly MonsterCatalog catalog;
        private readonly PlayerSaveData profile;
        private readonly WalletService wallet;
        private readonly MonsterCollectionService collection;
        private readonly ISaveService save;
        private readonly Random random = new();

        public GachaService(MonsterCatalog catalog, PlayerSaveData profile, WalletService wallet, MonsterCollectionService collection, ISaveService save)
        {
            this.catalog = catalog;
            this.profile = profile;
            this.wallet = wallet;
            this.collection = collection;
            this.save = save;
        }

        public GachaResult Pull(GachaBannerDefinition banner, int count)
        {
            if (banner == null || catalog == null) return Failure("Beschwörungsdaten fehlen.");
            if (count != 1 && count != 10) return Failure("Es sind nur 1er- und 10er-Beschwörungen erlaubt.");

            var cost = count == 10 ? banner.TenPullCost : banner.SinglePullCost;
            if (wallet.Get(banner.Currency) < cost) return Failure("Nicht genug Währung.");

            var pity = profile.GetPity(banner.Id);
            var simulatedPulls = pity.pulls;
            var results = new List<MonsterDefinition>(count);
            for (var index = 0; index < count; index++)
            {
                simulatedPulls++;
                var minimum = ResolvePityMinimum(banner, simulatedPulls);
                if (count == 10 && index == count - 1 && minimum < MonsterRarity.Rare) minimum = MonsterRarity.Rare;
                var rarity = RollRarity(banner, minimum);
                var monster = PickMonster(banner, rarity);
                if (monster == null) return Failure($"Für {rarity} ist noch kein passendes Monster angelegt.");
                results.Add(monster);
            }

            if (!wallet.TrySpend(banner.Currency, cost)) return Failure("Währung hat sich während der Beschwörung geändert.");
            pity.pulls = simulatedPulls;
            foreach (var monster in results) collection.Add(monster);
            var bonusShards = count == 10 ? banner.TenPullBonusShards : 0;
            profile.evolutionShards += bonusShards;
            save.Save(profile);
            return new GachaResult { Success = true, Monsters = results, BonusShards = bonusShards };
        }

        private MonsterRarity ResolvePityMinimum(GachaBannerDefinition banner, int pullNumber)
        {
            var minimum = MonsterRarity.Common;
            foreach (var milestone in banner.PityMilestones)
            {
                if (milestone.everyPulls > 0 && pullNumber % milestone.everyPulls == 0 && milestone.minimumRarity > minimum)
                    minimum = milestone.minimumRarity;
            }
            return minimum;
        }

        private MonsterRarity RollRarity(GachaBannerDefinition banner, MonsterRarity minimum)
        {
            var total = 0f;
            foreach (var entry in banner.RarityWeights) total += Math.Max(0f, entry.weight);
            if (total <= 0f) return minimum;

            var roll = random.NextDouble() * total;
            var cumulative = 0d;
            var rarity = MonsterRarity.Common;
            foreach (var entry in banner.RarityWeights)
            {
                cumulative += Math.Max(0f, entry.weight);
                if (roll <= cumulative)
                {
                    rarity = entry.rarity;
                    break;
                }
            }
            return rarity < minimum ? minimum : rarity;
        }

        private MonsterDefinition PickMonster(GachaBannerDefinition banner, MonsterRarity rarity)
        {
            var candidates = new List<MonsterDefinition>();
            var featured = new List<MonsterDefinition>();
            foreach (var monster in catalog.Monsters)
            {
                if (monster == null || monster.Rarity != rarity) continue;
                candidates.Add(monster);
                for (var i = 0; i < banner.FeaturedMonsterIds.Count; i++)
                    if (banner.FeaturedMonsterIds[i] == monster.Id) featured.Add(monster);
            }

            if (candidates.Count == 0)
            {
                foreach (var monster in catalog.Monsters)
                    if (monster != null && monster.Rarity >= rarity) candidates.Add(monster);
            }

            if (featured.Count > 0 && random.NextDouble() < banner.FeaturedChance)
                return featured[random.Next(featured.Count)];
            return candidates.Count > 0 ? candidates[random.Next(candidates.Count)] : null;
        }

        private static GachaResult Failure(string error) => new() { Success = false, Error = error };
    }
}
