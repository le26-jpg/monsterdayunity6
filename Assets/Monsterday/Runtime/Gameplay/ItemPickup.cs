using Monsterday.Core;
using Monsterday.Gameplay;
using Monsterday.Save;
using UnityEngine;

namespace Monsterday.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public sealed class ItemPickup : MonoBehaviour
    {
        [SerializeField] private string itemId = "health-potion";
        [SerializeField, Min(1)] private int quantity = 1;
        [SerializeField] private string itemName = "Trank";

        private InventoryService inventory;
        private ISaveService saveService;

        private void Awake()
        {
            ServiceRegistry.TryGet(out inventory);
            ServiceRegistry.TryGet(out saveService);
            var collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = true;
                if (GetComponent<Rigidbody>() == null)
                {
                    var rb = gameObject.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                }
            }
        }

        public void Configure(string itemId, int quantity, string itemName)
        {
            this.itemId = itemId;
            this.quantity = quantity;
            this.itemName = itemName;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>() == null) return;
            if (inventory == null) return;

            inventory.AddItem(itemId, quantity);
            if (ServiceRegistry.TryGet(out PlayerSaveData profile)) saveService?.Save(profile);
            Destroy(gameObject);
        }
    }
}
