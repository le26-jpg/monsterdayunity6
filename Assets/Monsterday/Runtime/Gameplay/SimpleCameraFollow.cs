using UnityEngine;

namespace Monsterday.Gameplay
{
    public sealed class SimpleCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -10f);
        [SerializeField, Min(0f)] private float smoothTime = 0.12f;
        [SerializeField, Min(0f)] private float rotationSpeed = 2.5f;
        [SerializeField, Min(10f)] private float minPitch = 10f;
        [SerializeField, Min(20f)] private float maxPitch = 60f;
        private Vector3 velocity;
        private float yaw;
        private float pitch = 20f;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            yaw = transform.eulerAngles.y;
        }

        private void LateUpdate()
        {
            if (target == null) return;
            HandleRotationInput();
            var rotation = Quaternion.Euler(pitch, yaw, 0f);
            var targetPosition = target.position + rotation * offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            transform.LookAt(target.position + Vector3.up * 1.2f);
        }

        private void HandleRotationInput()
        {
            float yawInput = 0f;
            float pitchInput = 0f;

            if (Input.GetMouseButton(1))
            {
                yawInput += Input.GetAxis("Mouse X");
                pitchInput -= Input.GetAxis("Mouse Y");
            }
            else if (Input.GetMouseButton(0) && Input.mousePosition.x >= Screen.width * 0.5f)
            {
                yawInput += Input.GetAxis("Mouse X");
                pitchInput -= Input.GetAxis("Mouse Y");
            }

            if (Input.touchCount > 0)
            {
                foreach (var touch in Input.touches)
                {
                    if (touch.position.x < Screen.width * 0.5f) continue;
                    if (touch.phase == TouchPhase.Moved)
                    {
                        yawInput += touch.deltaPosition.x * 0.01f;
                        pitchInput -= touch.deltaPosition.y * 0.01f;
                    }
                }
            }

            if (Mathf.Abs(yawInput) > 0f || Mathf.Abs(pitchInput) > 0f)
            {
                yaw += yawInput * rotationSpeed;
                pitch = Mathf.Clamp(pitch + pitchInput * rotationSpeed, minPitch, maxPitch);
            }
        }
    }
}
