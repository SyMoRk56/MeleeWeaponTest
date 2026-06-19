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
    private float _dashTimer = 10;
    private bool _dashRequested;
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
        _dashTimer += Time.deltaTime;
        if (_dashTimer > _stats.dashCooldown * .8)
            _dashRequested |= _input.OnRunPressed;
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
        print(_dashTimer);
        float targetSpeed = _input.IsRunPressed ? _stats.runSpeed : _stats.walkSpeed;
        if (_dashRequested && _dashTimer > _stats.dashCooldown)
        {
            _dashRequested = false;
            Debug.Log("PlayerMoveController: dash");
            Vector3 dashDirection = moveDirection.sqrMagnitude > 0.01f ? moveDirection : _manager.Orientation.forward;

            _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);

            _rb.AddForce(dashDirection * _stats.dashForce, ForceMode.VelocityChange);
            _dashTimer = 0;
            return;
        }
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            _accelerationTime = Mathf.Clamp01(_accelerationTime + Time.fixedDeltaTime / _stats.accelerationTime);
            float curveModifier = _stats.movementCurve.Evaluate(_accelerationTime);

            _rb.AddForce(10f * curveModifier * targetSpeed * moveDirection, ForceMode.Acceleration);

            Vector3 horizontalVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
            if (horizontalVelocity.sqrMagnitude > targetSpeed * targetSpeed)
            {
                Vector3 smoothDampVelocity = new();
                Vector3 limitedVelocity = (_dashTimer < .2f) ? Vector3.SmoothDamp(horizontalVelocity, horizontalVelocity.normalized * targetSpeed, ref smoothDampVelocity, .1f) : horizontalVelocity.normalized * targetSpeed;
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
