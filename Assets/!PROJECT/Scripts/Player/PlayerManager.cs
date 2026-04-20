using UnityEngine;
using Zenject;
namespace Game
{
    public class PlayerManager : MonoBehaviour
    {
        [field: SerializeField] public PlayerMoveController MoveController { get; private set; }
        [field: SerializeField] public PlayerCameraController CameraController { get; private set; }
        [field: SerializeField] public PlayerMoveInput PlayerMoveInput { get; private set; }
        [field: SerializeField] public Transform Orientation { get; private set; }
        [field: SerializeField] public Transform CameraGameobject { get; private set; }

        [field: SerializeField] public Sword PlayerSword { get; private set; }
        public void SetMovementEnabled(bool enabled)
        {

        }
        public void SetCameraRotationEnabled(bool enabled)
        {

        }
        public void SetCursorLock(bool value)
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !value;
        }
        public void SetPlayerLock(bool value)
        {
            SetCameraRotationEnabled(value);
            SetMovementEnabled(value);
        }
        private void Start()
        {
            SetCursorLock(true);
        }
    }
}
