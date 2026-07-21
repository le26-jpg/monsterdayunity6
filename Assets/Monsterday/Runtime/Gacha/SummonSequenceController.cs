using System.Collections;
using Monsterday.Data;
using UnityEngine;
using UnityEngine.UI;
#if MONSTERDAY_ADDRESSABLES
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace Monsterday.Gacha
{
    public sealed class SummonSequenceController : MonoBehaviour
    {
        [SerializeField] private Animator portalAnimator;
        [SerializeField] private Transform monsterSpawnRoot;
        [SerializeField] private CanvasGroup revealGroup;
        [SerializeField] private Text monsterNameLabel;
        [SerializeField] private Text rarityLabel;
        [SerializeField, Min(0.1f)] private float portalDuration = 1.4f;
        [SerializeField, Min(0.1f)] private float revealDuration = 0.8f;

        public bool IsPlaying { get; private set; }

        public void Play(MonsterDefinition monster)
        {
            if (!IsPlaying && monster != null) StartCoroutine(PlayRoutine(monster));
        }

        private IEnumerator PlayRoutine(MonsterDefinition monster)
        {
            IsPlaying = true;
            if (revealGroup != null) revealGroup.alpha = 0f;
            if (portalAnimator != null) portalAnimator.SetTrigger("Summon");
            yield return new WaitForSeconds(portalDuration);

#if MONSTERDAY_ADDRESSABLES
            AsyncOperationHandle<GameObject>? handle = null;
            if (monster.Prefab != null && monster.Prefab.RuntimeKeyIsValid())
            {
                var operation = monster.Prefab.InstantiateAsync(monsterSpawnRoot);
                yield return operation;
                if (operation.Status == AsyncOperationStatus.Succeeded) handle = operation;
            }
#else
            GameObject spawned = null;
            if (monster.Prefab != null) spawned = Instantiate(monster.Prefab, monsterSpawnRoot);
#endif

            if (monsterNameLabel != null) monsterNameLabel.text = monster.DisplayName;
            if (rarityLabel != null) rarityLabel.text = monster.Rarity.ToString().ToUpperInvariant();
            var elapsed = 0f;
            while (elapsed < revealDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                if (revealGroup != null) revealGroup.alpha = Mathf.Clamp01(elapsed / revealDuration);
                yield return null;
            }
            yield return new WaitForSecondsRealtime(1f);

#if MONSTERDAY_ADDRESSABLES
            if (handle.HasValue && handle.Value.IsValid()) monster.Prefab.ReleaseInstance(handle.Value.Result);
#else
            if (spawned != null) Destroy(spawned);
#endif
            IsPlaying = false;
        }
    }
}
