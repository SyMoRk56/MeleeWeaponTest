using Game;
using UnityEngine;
using UnityEngine.Windows;
using Zenject;

public class PlayerMoveController : MonoBehaviour
{
    private IMoveInput _input;
    private PlayerManager _manager;

    private PlayerParams _stats;
    private Rigidbody _rb;

    private float _stamina;
    private float _accelerationTime;
    [Inject]
    public void Construct(PlayerManager playerManager, PlayerParams stats, Rigidbody rb)
    {
        _manager = playerManager;
        _input = playerManager.PlayerMoveInput;
        _stats = stats;
        _rb = rb;
    }
    private void Update()
    {
        //if (_input.IsRunPressed)
        //{
        //    _stamina += Mathf.Lerp(0.2f, 1, 1 - (_manager.PlayerSword.IsSwordHolding ? 0.5f : 0));
        //    _stamina = Mathf.Clamp(_stamina, 0, _stats.maxStamina);
        //}

    }
    private void FixedUpdate()
    {
        Vector2 inputDirection = _manager.Orientation.forward * _input.Axis.y + _manager.Orientation.right * _input.Axis.x;
        Vector3 moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);

        if(moveDirection.magnitude > 0.1f)
        {
            _accelerationTime += Time.fixedDeltaTime / _stats.accelerationTime;
            float curveModifier = _stats.movementCurve.Evaluate(_accelerationTime);

            float speed = _input.IsRunPressed ? _stats.runSpeed : _stats.walkSpeed;

            _rb.AddForce(moveDirection.normalized * speed * curveModifier);
        }
        else
        {
            _accelerationTime = 0;
        }

    }
}
