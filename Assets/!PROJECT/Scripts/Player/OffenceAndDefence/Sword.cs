using com.marufhow.meshslicer.core;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using Zenject;

public class Sword : HoldableItem
{
    [SerializeField] private Transform _tip;
    [SerializeField] private Collider _bladeCollider;
    [SerializeField] private float _minSliceVelocity = 2.0f;
    [SerializeField] private float _topThreshold = 30f;
    private MHCutter _cutter;
    private Vector3 _lastTipPosition, _preLastTipPosition;
    private Quaternion _lastTipRotation;
    private float _currentZ;
    private float _zVelocity;
    private float _angularSpeed;

    [Inject]
    public void Construct(PlayerManager manager, MHCutter cutter)
    {
        base.Construct(manager);
        _cutter = cutter;
    }

    private void Start()
    {
        if (_tip)
        {
            _lastTipPosition = _tip.position;
            _preLastTipPosition = _tip.position;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (_bladeCollider)
            _bladeCollider.enabled = (CurrentState == EItemState.Active);
    }

    private void FixedUpdate()
    {
        if (!_tip)
            return;

        _preLastTipPosition = _lastTipPosition;
        _lastTipPosition = _tip.position;
        _lastTipRotation = _tip.rotation;

        _angularSpeed = (_lastTipPosition - _preLastTipPosition).magnitude / Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Slicable"))
            return;
        if (!Slice(other.gameObject))
        {
            LockMovement(.2f).Forget();
        }
    }

    protected override void HandleInput()
    {
        if (_input.IsLeftMousePressed)
        {
            float deltaX = _input.MouseAxis.x * _mouseSensitivity;
            float deltaY = _input.MouseAxis.y * _mouseSensitivity * 2;

            float totalDelta = Mathf.Abs(_rawMouseDelta.x) + Mathf.Abs(_rawMouseDelta.y) + 0.0001f;
            float horizontalRatio = Mathf.Abs(_rawMouseDelta.x) / totalDelta;
            float verticalRatio = Mathf.Abs(_rawMouseDelta.y) / totalDelta;

            float combinedDelta = Mathf.Max(Mathf.Abs(_rawMouseDelta.x), Mathf.Abs(_rawMouseDelta.y));
            float suppressionFactor = Mathf.Clamp01((combinedDelta - _topThreshold) / _topThreshold);

            deltaX *= Mathf.Lerp(1f, horizontalRatio, suppressionFactor);
            deltaY *= Mathf.Lerp(1f, verticalRatio, suppressionFactor);

            _holdTime += Time.deltaTime;
            _rawMouseDelta.x = Mathf.Clamp(_rawMouseDelta.x + deltaX, -90, 90);
            _rawMouseDelta.y = Mathf.Clamp(_rawMouseDelta.y + deltaY, -180, 180);

            _rawMouseDelta.x = Mathf.Lerp(_rawMouseDelta.x, 0, Time.deltaTime * 3);
            _rawMouseDelta.y = Mathf.Lerp(_rawMouseDelta.y, 0, Time.deltaTime * 3);
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
            _currentZ = Mathf.SmoothDamp(_currentZ, targetZ, ref _zVelocity, 0.2f);

            Vector3 movementDelta = _tip.position - _preLastTipPosition;
            float totalMovement = Mathf.Abs(movementDelta.x) + Mathf.Abs(movementDelta.y) + 0.0001f;
            float horizontalRatio = Mathf.Abs(movementDelta.x) / totalMovement;

            float sharpRatio = Mathf.Clamp01((horizontalRatio*1.4f)-.4f);
            float finalZ = Mathf.Lerp(0f, -90f, sharpRatio);

            print(sharpRatio);
            return Quaternion.Euler(
                _prepareRot.x - _smoothedMouseDelta.y,
                _prepareRot.y + _smoothedMouseDelta.x,
                finalZ
            );
        }

        return (CurrentState == EItemState.Prepare)
            ? Quaternion.Euler(_prepareRot)
            : Quaternion.Euler(_idleRot);
    }
    public void CheckCollision(Collider other)
    {
        if (other.CompareTag("Slicable"))
        {
            if (!Slice(other.gameObject))
            {
                LockMovement(.2f).Forget();
            }
        }
    }
    private bool Slice(GameObject go)
    {
        if (CurrentState != EItemState.Active)
            return false;
        var angularSpeed = (_tip.position - _preLastTipPosition).magnitude / Time.deltaTime;
        var dotProduct = Mathf.Abs(Vector2.Dot((_tip.position - _preLastTipPosition).normalized, _tip.right));

        Debug.Log("Sword slice: " + "Angular Speed: " + angularSpeed + " " + "Hit flatness: " + dotProduct);

        if (angularSpeed < _minSliceVelocity)
            return false;
        if (dotProduct > 0.4f)
            return false;
        LockMovement(0.51f).Forget();

        _cutter.Cut(go, _tip.position, _tip.right);
        return true;
    }
    protected async UniTaskVoid LockMovement(float secs)
    {
        _canMove = false;
        _rawMouseDelta = Vector2.zero;
        await UniTask.WaitForSeconds(secs);
        _canMove = true;

    }

}

