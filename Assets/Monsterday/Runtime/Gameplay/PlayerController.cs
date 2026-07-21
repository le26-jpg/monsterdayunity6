using System.Linq;
using Monsterday.Combat;
using UnityEngine;

namespace Monsterday.Gameplay
{
    [RequireComponent(typeof(MonsterCombatant))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float moveSpeed = 5f;
        [SerializeField, Min(0.1f)] private float rotationSpeed = 12f;
        [SerializeField, Min(0.1f)] private float attackSearchRadius = 4f;
        [SerializeField] private LayerMask enemyLayer = 1;

        private MonsterCombatant combatant;
        private Camera mainCamera;
        private Vector3 moveDirection;

        private void Awake()
        {
            combatant = GetComponent<MonsterCombatant>();
            mainCamera = Camera.main;
        }

        private void Update()
        {
            moveDirection = GetMovementInput();
            if (moveDirection.sqrMagnitude > 0.001f)
            {
                Move(moveDirection);
                Rotate(moveDirection);
            }

            if (IsAttackPressed())
            {
                var target = FindNearestEnemy();
                if (target != null)
                {
                    combatant.SetTarget(target);
                }
            }
        }

        private Vector3 GetMovementInput()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            var input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            if (input.sqrMagnitude > 0.001f) return input.normalized;

            if (Input.touchCount > 0)
            {
                foreach (var touch in Input.touches)
                {
                    if (touch.position.x >= Screen.width * 0.5f) continue;
                    var drag = new Vector2(touch.deltaPosition.x, touch.deltaPosition.y);
                    if (drag.sqrMagnitude > 100f)
                    {
                        var direction = new Vector3(drag.x, 0f, drag.y).normalized;
                        if (mainCamera != null)
                        {
                            var forward = mainCamera.transform.forward;
                            forward.y = 0f;
                            forward.Normalize();
                            var right = mainCamera.transform.right;
                            right.y = 0f;
                            right.Normalize();
                            return (forward * direction.z + right * direction.x).normalized;
                        }
                        return direction;
                    }
                }
            }

            return Vector3.zero;
        }

        private void Move(Vector3 direction)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        private void Rotate(Vector3 direction)
        {
            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        private bool IsAttackPressed()
        {
            if (Input.GetKeyDown(KeyCode.Space)) return true;
            if (Input.GetMouseButtonDown(0) && Input.mousePosition.x >= Screen.width * 0.5f) return true;
            if (Input.touchCount <= 0) return false;
            foreach (var touch in Input.touches)
            {
                if (touch.position.x < Screen.width * 0.5f) continue;
                if (touch.phase == TouchPhase.Began) return true;
            }
            return false;
        }

        private MonsterCombatant FindNearestEnemy()
        {
            var enemies = FindObjectsOfType<MonsterCombatant>();
            return enemies
                .Where(enemy => enemy != null && enemy != combatant && enemy.IsAlive)
                .OrderBy(enemy => (enemy.transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault(enemy => Vector3.SqrMagnitude(enemy.transform.position - transform.position) <= attackSearchRadius * attackSearchRadius);
        }
    }
}
