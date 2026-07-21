using Monsterday.Data;
using Monsterday.Save;

namespace Monsterday.Collection
{
    public sealed class MonsterCollectionService
    {
        private readonly PlayerSaveData profile;

        public MonsterCollectionService(PlayerSaveData profile) => this.profile = profile;

        public OwnedMonsterRecord Add(MonsterDefinition monster)
        {
            var record = profile.monsters.Find(entry => entry.monsterId == monster.Id);
            if (record == null)
            {
                record = new OwnedMonsterRecord { monsterId = monster.Id, copies = 1 };
                profile.monsters.Add(record);
            }
            else
            {
                record.copies++;
            }
            return record;
        }

        public bool Owns(string monsterId) => profile.monsters.Exists(entry => entry.monsterId == monsterId && entry.copies > 0);
    }
}
