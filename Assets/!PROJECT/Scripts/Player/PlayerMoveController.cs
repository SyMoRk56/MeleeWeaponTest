using Game;
using Unity.VisualScripting;
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
        Vector3 moveDirection = (_manager.Orientation.forward * _input.MoveAxis.y) + (_manager.Orientation.right * _input.MoveAxis.x);
        moveDirection.y = 0;
        moveDirection.Normalize();

        float targetSpeed = _input.IsRunPressed ? _stats.runSpeed : _stats.walkSpeed;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            _accelerationTime = Mathf.Clamp01(_accelerationTime + Time.fixedDeltaTime / _stats.accelerationTime);
            float curveModifier = _stats.movementCurve.Evaluate(_accelerationTime);

            _rb.AddForce(10f * curveModifier * targetSpeed * moveDirection, ForceMode.Acceleration);

            Vector3 horizontalVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
            if (horizontalVelocity.sqrMagnitude > targetSpeed * targetSpeed)
            {
                Vector3 limitedVelocity = horizontalVelocity.normalized * targetSpeed;
                _rb.linearVelocity = new Vector3(limitedVelocity.x, _rb.linearVelocity.y, limitedVelocity.z);
            }
        }
        else
        {
            _accelerationTime = 0;
            Vector3 horizontalVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
            _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, new Vector3(0, _rb.linearVelocity.y, 0), Time.fixedDeltaTime * 10f);
        }
    }
}
