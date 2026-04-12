using UnityEngine;
using Zenject;

namespace Game
{
    public class PlayerCameraController : MonoBehaviour
    {
        PlayerManager _player;

        [Inject]
        public void Construct(PlayerManager playerManager)
        {
            _player = playerManager;
        }
    }

}
