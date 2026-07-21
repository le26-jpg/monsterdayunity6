using UnityEngine;
#if MONSTERDAY_CINEMACHINE
using Unity.Cinemachine;
#endif

namespace Monsterday.CameraSystem
{
    public sealed class CameraDirector : MonoBehaviour
    {
#if MONSTERDAY_CINEMACHINE
        [SerializeField] private CinemachineCamera explorationCamera;
        [SerializeField] private CinemachineCamera battleCamera;
        [SerializeField] private CinemachineCamera ultimateCamera;
        [SerializeField] private CinemachineImpulseSource hitImpulse;
#endif

        public void ShowExploration() => SetPriority(30, 10, 0);
        public void ShowBattle() => SetPriority(10, 30, 0);
        public void ShowUltimate() => SetPriority(0, 10, 40);

        public void PlayHitImpulse(float strength = 0.35f)
        {
#if MONSTERDAY_CINEMACHINE
            if (hitImpulse != null) hitImpulse.GenerateImpulse(strength);
#endif
        }

        private void SetPriority(int exploration, int battle, int ultimate)
        {
#if MONSTERDAY_CINEMACHINE
            if (explorationCamera != null) explorationCamera.Priority = exploration;
            if (battleCamera != null) battleCamera.Priority = battle;
            if (ultimateCamera != null) ultimateCamera.Priority = ultimate;
#endif
        }
    }
}
