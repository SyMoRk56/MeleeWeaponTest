using System;
using System.Collections;
using com.marufhow.meshslicer.core;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using Zenject;

public class PlayerSword : MonoBehaviour
{
    private ESwordState _currentState = ESwordState.Idle;

    [Header("Transform References")]
    [SerializeField] private Transform _tip;
    [SerializeField] private Collider _bladeCollider;

    [Header("Positions & Rotations")]
    [SerializeField] private Vector3 _idlePos, _preparePos;
    [SerializeField] private Vector3 _idleRot, _prepareRot;

    [Header("Settings")]
    [SerializeField] private float _lerpSpeed = 10f;
    [SerializeField] private float _mouseSensitivity = 0.5f;
    [SerializeField] private float _prepareDuration = 0.2f;

    [Header("Slicing Settings")]
    [SerializeField] private float _minSliceVelocity = 2.0f;
    [Range(1f, 20f)][SerializeField] private float _weightIntensity = 5f;

    private float _holdTime;
    private Vector2 _rawMouseDelta;
    private Vector2 _smoothedMouseDelta;

    private Vector3 _lastTipPosition;
    private float _currentVelocity;
    private float _currentZ;
    private float _zVelocity;
    private Quaternion _lastTipRotation;


    private enum ESwordState { Idle, Prepare, ActiveSwing }
    private IMoveInput _input;
    private MHCutter _cutter;

    [Inject]
    public void Construct(PlayerManager manager, MHCutter cutter)
    {
        _input = manager.PlayerMoveInput;
        _cutter = cutter;
    }

    private void Start()
    {
        _lastTipPosition = _tip.position;
    }

    private void Update()
    {
        CalculateVelocity();
        HandleInput();
        UpdateState();
        ApplyTransform();
        _lastTipRotation = _tip.rotation;
    }

    private void CalculateVelocity()
    {
        _currentVelocity = (_tip.position - _lastTipPosition).magnitude / Time.deltaTime;
        _lastTipPosition = _tip.position;
    }

    [SerializeField] private float _topThreshold = 30f; // Угол, при котором меч считается "наверху"

    private void HandleInput()
    {
        if (_input.IsLeftMousePressed)
        {
            _holdTime += Time.deltaTime;

            // Читаем дельту мыши
            float deltaX = _input.MouseAxis.x * _mouseSensitivity;
            float deltaY = _input.MouseAxis.y * _mouseSensitivity;

            // Если замах по вертикали (Y) уже высоко, блокируем изменение горизонтали (X)
            // В твоей логике _rawMouseDelta.y отвечает за наклон (pitch)
            print(_rawMouseDelta.y);
            if (_rawMouseDelta.y > _topThreshold)
            {
                deltaX = 0; // Блокируем движение вбок
            }

            _rawMouseDelta.x = Mathf.Clamp(_rawMouseDelta.x + deltaX, -90, 90);
            _rawMouseDelta.y = Mathf.Clamp(_rawMouseDelta.y + deltaY, -180, 180);
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
            _currentState = (_holdTime > _prepareDuration) ? ESwordState.ActiveSwing : ESwordState.Prepare;
        else
            _currentState = ESwordState.Idle;

        if (_bladeCollider)
            _bladeCollider.enabled = (_currentState == ESwordState.ActiveSwing);
    }

    private void ApplyTransform()
    {
        Vector3 targetPos = (_currentState == ESwordState.Idle) ? _idlePos : _preparePos;
        Quaternion targetRot;

        if (_currentState == ESwordState.ActiveSwing)
        {
            float deltaX = Mathf.Abs(Mathf.DeltaAngle(_tip.eulerAngles.x, _lastTipRotation.eulerAngles.x));
            float deltaY = Mathf.Abs(Mathf.DeltaAngle(_tip.eulerAngles.y, _lastTipRotation.eulerAngles.y));

            float horizontalInfluence = deltaY;
            float verticalInfluence = deltaX;
            float targetZ = 0f;
            if (horizontalInfluence > verticalInfluence)
            {
                targetZ = -90;
            }

            _currentZ = Mathf.SmoothDamp(_currentZ, targetZ, ref _zVelocity, 0.1f);

            targetRot = Quaternion.Euler(
                _prepareRot.x - _smoothedMouseDelta.y ,
                _prepareRot.y + _smoothedMouseDelta.x,
                _currentZ
            );
        }

        else if (_currentState == ESwordState.Prepare)
            targetRot = Quaternion.Euler(_prepareRot);
        else
            targetRot = Quaternion.Euler(_idleRot);

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * _lerpSpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * _lerpSpeed);


    }

    public async UniTask CheckCollision(Collider other)
    {
        await UniTask.WaitForSeconds(.05f);
        if (_currentState != ESwordState.ActiveSwing)
            return;

        Vector3 swingDirection = _tip.position - _lastTipPosition;
        float speed = swingDirection.magnitude / Time.deltaTime;

        if (speed < _minSliceVelocity)
            return;

        if (other.CompareTag("Slicable"))
        {
            Vector3 bladeFlatSide = transform.right;
            float hitFlatness = Mathf.Abs(Vector3.Dot(swingDirection.normalized, bladeFlatSide));

            if (hitFlatness > 0.5f)
                return;

            Slice(other.gameObject, _tip.position, _tip.right);
        }
    }

    private void OnDrawGizmos()
    {
        if (_tip == null)
            return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_tip.position, _tip.position + transform.right * 0.2f);
        Gizmos.DrawSphere(_tip.position, 0.1f);
    }

    private void Slice(GameObject target, Vector3 point, Vector3 normal)
    {
        _cutter.Cut(target, point, normal);
    }
}
