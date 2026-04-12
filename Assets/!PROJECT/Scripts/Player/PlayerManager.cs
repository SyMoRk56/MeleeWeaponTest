using UnityEngine;
using Zenject;
namespace Game
{
    public class PlayerManager : MonoBehaviour
    {
        [field: SerializeField] public PlayerMoveController MoveController { get; private set; }
        [field: SerializeField] public PlayerCameraController CameraController { get; private set; }
        [field: SerializeField] public PlayerMoveInput PlayerMoveInput { get; private set; }

    }
}
