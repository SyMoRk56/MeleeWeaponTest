using UnityEngine;

namespace Game
{
    public class PlayerMoveInput : MonoBehaviour, IMoveInput
    {
        public Vector2 MoveAxis => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        public bool IsJumpPressed => Input.GetKeyDown(KeyCode.Space);

        public bool IsRunPressed => Input.GetKey(KeyCode.LeftShift);

        public Vector2 MouseAxis => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        public bool IsRightMousePressed => Input.GetMouseButton(1);

        public bool IsLeftMousePressed => Input.GetMouseButton(0);

        public bool IsLeftMouseDown => Input.GetMouseButtonDown(0);
    }

}
