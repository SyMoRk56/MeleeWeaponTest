using Game;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class PlayerSword : MonoBehaviour
{
    private ESwordState _currentState = ESwordState.Idle;

    [SerializeField] private Vector3 _idlePos;
    [SerializeField] private Vector3 _preparePos;
    [SerializeField] private Vector3 _idleRot;
    [SerializeField] private Vector3 _prepareRot;

    [SerializeField] private float _lerpSpeed = 10f;
    [SerializeField] private float _mouseSensitivity = 0.5f;
    [SerializeField] private float _prepareDuration = 0.2f;

    [SerializeField] private float _maxHorizontalAngle = 90f;
    [SerializeField] private float _maxVerticalAngle = 180f;
    [Range(1f, 20f)][SerializeField] private float _weightIntensity = 5f;

    private float _holdTime;
    private Vector2 _rawMouseDelta;
    private Vector2 _smoothedMouseDelta;

    private enum ESwordState { Idle, Prepare, ActiveSwing }
    private IMoveInput _input;

    [Inject]
    public void Construct(PlayerManager manager)
    {
        _input = manager.PlayerMoveInput;
    }

    private void Update()
    {
        HandleInput();
        UpdateState();
        ApplyTransform();
    }
    
    private void HandleInput()
    {
        print(_input.IsLeftMousePressed);
        if (_input.IsLeftMousePressed)
        {
            _holdTime += Time.deltaTime;

            _rawMouseDelta.x += _input.MouseAxis.x * _mouseSensitivity;
            _rawMouseDelta.y += _input.MouseAxis.y * _mouseSensitivity;

            _rawMouseDelta.x = Mathf.Clamp(_rawMouseDelta.x, -_maxHorizontalAngle, _maxHorizontalAngle);
            _rawMouseDelta.y = Mathf.Clamp(_rawMouseDelta.y, -_maxVerticalAngle, _maxVerticalAngle);
        }
        else
        {
            _holdTime = 0;
            _rawMouseDelta = Vector2.zero;
        }

        _smoothedMouseDelta = Vector2.Lerp(_smoothedMouseDelta, _rawMouseDelta, Time.deltaTime * _weightIntensity);
    }

    private void UpdateState()
    {
        if (_input.IsLeftMousePressed)
        {
            _currentState = (_holdTime > _prepareDuration) ? ESwordState.ActiveSwing : ESwordState.Prepare;
        }
        else
        {
            _currentState = ESwordState.Idle;
        }
    }

    private void ApplyTransform()
    {
        Vector3 targetPos;
        Quaternion targetRot;

        switch (_currentState)
        {
            case ESwordState.ActiveSwing:
                targetPos = _preparePos;
                targetRot = Quaternion.Euler(_prepareRot.x - _smoothedMouseDelta.y, _prepareRot.y + _smoothedMouseDelta.x, _prepareRot.z);
                break;

            case ESwordState.Prepare:
                targetPos = _preparePos;
                targetRot = Quaternion.Euler(_prepareRot);
                break;

            default:
                targetPos = _idlePos;
                targetRot = Quaternion.Euler(_idleRot);
                _smoothedMouseDelta = Vector2.Lerp(_smoothedMouseDelta, Vector2.zero, Time.deltaTime * _lerpSpeed);
                break;
        }

        transform.SetLocalPositionAndRotation(Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * _lerpSpeed), Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * _lerpSpeed));
    }
}
