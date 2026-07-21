using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Monsterday.UI
{
    public sealed class MobileButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField, Range(0.8f, 1f)] private float pressedScale = 0.94f;
        [SerializeField, Min(1f)] private float recoverySpeed = 16f;
        private Vector3 originalScale;
        private Coroutine recovery;

        private void Awake() => originalScale = transform.localScale;
        public void OnPointerDown(PointerEventData eventData) => transform.localScale = originalScale * pressedScale;
        public void OnPointerUp(PointerEventData eventData) => Recover();
        public void OnPointerExit(PointerEventData eventData) => Recover();

        private void Recover()
        {
            if (recovery != null) StopCoroutine(recovery);
            recovery = StartCoroutine(RecoverRoutine());
        }

        private IEnumerator RecoverRoutine()
        {
            while ((transform.localScale - originalScale).sqrMagnitude > 0.0001f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.unscaledDeltaTime * recoverySpeed);
                yield return null;
            }
            transform.localScale = originalScale;
        }
    }
}
