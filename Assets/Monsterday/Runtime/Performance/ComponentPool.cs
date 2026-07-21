using System.Collections.Generic;
using UnityEngine;

namespace Monsterday.Performance
{
    public sealed class ComponentPool<T> where T : Component
    {
        private readonly T prefab;
        private readonly Transform root;
        private readonly Stack<T> inactive = new();

        public ComponentPool(T prefab, Transform root, int preload = 0)
        {
            this.prefab = prefab;
            this.root = root;
            for (var i = 0; i < preload; i++) Release(Create());
        }

        public T Get(Vector3 position, Quaternion rotation)
        {
            var instance = inactive.Count > 0 ? inactive.Pop() : Create();
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.gameObject.SetActive(true);
            return instance;
        }

        public void Release(T instance)
        {
            if (instance == null) return;
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(root, false);
            inactive.Push(instance);
        }

        private T Create()
        {
            var instance = Object.Instantiate(prefab, root);
            instance.gameObject.SetActive(false);
            return instance;
        }
    }
}
