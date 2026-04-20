using com.marufhow.meshslicer.core;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using Zenject;

public class Sword : HoldableItem
{
    [Header("Sword Specific")]
    [SerializeField] private Transform _tip;
    [SerializeField] private Collider _bladeCollider;
    [SerializeField] private float _minSliceVelocity = 2.0f;
    [SerializeField] private float _topThreshold = 30f;

    private MHCutter _cutter;
    private Vector3 _lastTipPosition, _preLastTipPosition;
    private Quaternion _lastTipRotation;
    private float _currentZ;
    private float _zVelocity;

    [Inject]
    public void Construct(PlayerManager manager, MHCutter cutter)
    {
        base.Construct(manager);
        _cutter = cutter;
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
            _preLastTipPosition = _lastTipPosition;
            _lastTipPosition = _tip.position;
            _lastTipRotation = _tip.rotation;
            
        }

        if (_bladeCollider)
            _bladeCollider.enabled = (CurrentState == EItemState.Active);
    }

    protected override void HandleInput()
    {
        if (_input.IsLeftMousePressed)
        {
            float deltaX = _input.MouseAxis.x * _mouseSensitivity;
            float deltaY = _input.MouseAxis.y * _mouseSensitivity * 2;

            if (_rawMouseDelta.y > _topThreshold)
                deltaX = 0;

            _holdTime += Time.deltaTime;
            _rawMouseDelta.x = Mathf.Clamp(_rawMouseDelta.x + deltaX, -90, 90);
            _rawMouseDelta.y = Mathf.Clamp(_rawMouseDelta.y + deltaY, -180, 180);
            _rawMouseDelta.x = Mathf.Lerp(_rawMouseDelta.x, 0, Time.deltaTime * 3);
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

            float targetZ = (deltaY > deltaX) ? -90f : 0f;
            _currentZ = Mathf.SmoothDamp(_currentZ, targetZ, ref _zVelocity, 0.1f);

            return Quaternion.Euler(
                _prepareRot.x - _smoothedMouseDelta.y,
                _prepareRot.y + _smoothedMouseDelta.x,
                0//_currentZ
            );
        }

        return (CurrentState == EItemState.Prepare)
            ? Quaternion.Euler(_prepareRot)
            : Quaternion.Euler(_idleRot);
    }

    public async UniTask CheckCollision(Collider other)
    {
        if (other.CompareTag("Slicable"))
        {
            if (CurrentState != EItemState.Active)
                return;
            var angularSpeed = (_tip.position - _preLastTipPosition).magnitude / Time.deltaTime;
            var dotProduct = Mathf.Abs(Vector2.Dot((_tip.position - _preLastTipPosition).normalized, _tip.right));
            Debug.Log("Sword slice: " + "Angular Speed: " + angularSpeed + " " + "Hit flatness: " + dotProduct);

            if (angularSpeed < _minSliceVelocity)
                return;
            if (dotProduct > 0.3f)
                return;

            _cutter.Cut(other.gameObject, _tip.position, _tip.right);
        }
    }

}
