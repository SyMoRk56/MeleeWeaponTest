using Game;
using UnityEngine;
using Zenject;

public class PlayerMoveController : MonoBehaviour
{
    IMoveInput _input;
    PlayerManager _manager;

    [Inject]
    public void Construct(PlayerManager playerManager)
    {
        _manager = playerManager;
        _input = playerManager.PlayerMoveInput;
    }
}
