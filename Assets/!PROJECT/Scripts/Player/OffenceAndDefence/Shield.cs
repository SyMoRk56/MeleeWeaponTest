using com.marufhow.meshslicer.core;
using Game;
using UnityEngine;
using Zenject;

public class Shield : HoldableItem
{
    [SerializeField] private Transform _tip;
    [SerializeField] private float _minSliceVelocity = 2.0f;
    [SerializeField] private float _topThreshold = 30f;

    private Vector3 _lastTipPosition;
    private Quaternion _lastTipRotation;
    private float _currentZ;
    private float _zVelocity;
    [Inject]
    public void Construct(PlayerManager manager, MHCutter cutter)
    {
        base.Construct(manager);
        
    }

    private void Start()
    {
        if (_tip)
            _lastTipPosition = _tip.position;
    }

    protected override void Update()
    {
        base.Update();

        if (_tip)
        {
            _lastTipPosition = _tip.position;
            _lastTipRotation = _tip.rotation;
        }
    }
    protected override bool GetInput() => _input.IsRightMousePressed;
    protected override void HandleInput()
    {
        if (_input.IsRightMousePressed)
        {
            print("righ");
            float deltaX = _input.MouseAxis.x * _mouseSensitivity;
            float deltaY = _input.MouseAxis.y * _mouseSensitivity;

            _holdTime += Time.deltaTime;
            _rawMouseDelta.x = Mathf.Clamp(_rawMouseDelta.x + deltaX, -20, 20);
            _rawMouseDelta.y = Mathf.Clamp(_rawMouseDelta.y + deltaY, -30, 30);
        }
        else
        {
            _holdTime = 0;
            _rawMouseDelta = Vector2.zero;
        }

        _smoothedMouseDelta = Vector2.Lerp(_smoothedMouseDelta, _rawMouseDelta, Time.deltaTime * _weightIntensity);
    }

    protected override Quaternion CalculateTargetRotation()
    {
        if (CurrentState == EItemState.Active)
        {
            float deltaX = Mathf.Abs(Mathf.DeltaAngle(_tip.eulerAngles.x, _lastTipRotation.eulerAngles.x));
            float deltaY = Mathf.Abs(Mathf.DeltaAngle(_tip.eulerAngles.y, _lastTipRotation.eulerAngles.y));

            

            return Quaternion.Euler(
                _prepareRot.x - _smoothedMouseDelta.y,
                _prepareRot.y + _smoothedMouseDelta.x,
                _currentZ
            );
        }

        return (CurrentState == EItemState.Prepare)
            ? Quaternion.Euler(_prepareRot)
            : Quaternion.Euler(_idleRot);
    }
}
