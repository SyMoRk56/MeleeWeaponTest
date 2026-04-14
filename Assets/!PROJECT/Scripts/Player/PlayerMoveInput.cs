using UnityEngine;

namespace Game
{
    public class PlayerMoveInput : MonoBehaviour, IMoveInput
    {
        public Vector2 Axis => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        public bool IsJumpPressed => Input.GetKeyDown(KeyCode.Space);

        public bool IsRunPressed => Input.GetKey(KeyCode.LeftShift);
    }

}
