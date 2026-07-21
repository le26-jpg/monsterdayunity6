using System;
using System.Collections.Generic;
using System.Linq;
using Monsterday.Save;

namespace Monsterday.Gameplay
{
    public sealed class InventoryService
    {
        private readonly PlayerSaveData profile;

        public event Action InventoryChanged;

        public InventoryService(PlayerSaveData profile)
        {
            this.profile = profile;
        }

        public IReadOnlyList<OwnedMonsterRecord> OwnedMonsters => profile.monsters;
        public IReadOnlyList<ItemRecord> Items => profile.items;

        public ItemRecord AddItem(string itemId, int quantity)
        {
            var record = profile.GetItem(itemId);
            record.quantity = System.Math.Max(0, record.quantity + quantity);
            InventoryChanged?.Invoke();
            return record;
        }

        public int GetItemCount(string itemId)
        {
            return profile.GetItem(itemId).quantity;
        }
    }
