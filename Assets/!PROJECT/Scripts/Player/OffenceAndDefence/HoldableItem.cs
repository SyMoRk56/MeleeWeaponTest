using Game;
using UnityEngine;
using Zenject;

public abstract class HoldableItem : MonoBehaviour
{
    protected enum EItemState { Idle, Prepare, Active }
    protected EItemState CurrentState = EItemState.Idle;

    [Header("Base Transform Settings")]
    [SerializeField] protected Vector3 _idlePos, _preparePos;
    [SerializeField] protected Vector3 _idleRot, _prepareRot;
    [SerializeField] protected float _lerpSpeed = 10f;
    [SerializeField] protected float _mouseSensitivity = 0.5f;
    [SerializeField] protected float _prepareDuration = 0.2f;
    [Range(1f, 20f)][SerializeField] protected float _weightIntensity = 5f;

    protected IMoveInput _input;
    protected float _holdTime;
    protected Vector2 _rawMouseDelta;
    protected Vector2 _smoothedMouseDelta;

    [Inject]
    public virtual void Construct(PlayerManager manager)
    {
        _input = manager.PlayerMoveInput;
    }

    protected virtual void Update()
    {
        HandleInput();
        UpdateState();
        ApplyBaseTransform();
    }

    protected virtual void HandleInput()
    {
        if (GetInput())
        {
            _holdTime += Time.deltaTime;

            float deltaX = _input.MouseAxis.x * _mouseSensitivity;
            float deltaY = _input.MouseAxis.y * _mouseSensitivity;
            

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

    protected virtual bool GetInput() => _input.IsLeftMousePressed;

    protected virtual void UpdateState()
    {
        if (GetInput())
            CurrentState = (_holdTime > _prepareDuration) ? EItemState.Active : EItemState.Prepare;
        else
            CurrentState = EItemState.Idle;
    }

    protected virtual void ApplyBaseTransform()
    {
        Vector3 targetPos = (CurrentState == EItemState.Idle) ? _idlePos : _preparePos;
        Quaternion targetRot = CalculateTargetRotation();

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * _lerpSpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * _lerpSpeed);
    }
    protected abstract Quaternion CalculateTargetRotation();
}
