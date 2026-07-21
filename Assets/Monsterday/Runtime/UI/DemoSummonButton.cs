using Monsterday.Core;
using Monsterday.Data;
using Monsterday.Gacha;
using UnityEngine;
using UnityEngine.UI;

namespace Monsterday.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class DemoSummonButton : MonoBehaviour
    {
        [SerializeField] private GachaBannerDefinition banner;
        [SerializeField] private int pullCount = 1;
        [SerializeField] private Text resultLabel;
        [SerializeField] private SummonSequenceController sequence;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Pull);
        }

        private void OnDestroy()
        {
            if (button != null) button.onClick.RemoveListener(Pull);
        }

        private void Pull()
        {
            if (sequence != null && sequence.IsPlaying) return;
            if (!ServiceRegistry.TryGet<GachaService>(out var service)) return;
            var result = service.Pull(banner, pullCount);
            if (!result.Success)
            {
                if (resultLabel != null) resultLabel.text = result.Error;
                return;
            }

            var last = result.Monsters[result.Monsters.Count - 1];
            if (resultLabel != null)
                resultLabel.text = pullCount == 10
                    ? $"{result.Monsters.Count} Monster · +{result.BonusShards} Splitter\nLetztes: {last.DisplayName} ({last.Rarity})"
                    : $"{last.DisplayName}\n{last.Rarity} · {last.Element}";
            if (sequence != null) sequence.Play(last);
        }

#if UNITY_EDITOR
        public void EditorConfigure(GachaBannerDefinition definition, int count, Text label)
        {
            banner = definition;
            pullCount = count;
            resultLabel = label;
        }
#endif
    }
}
