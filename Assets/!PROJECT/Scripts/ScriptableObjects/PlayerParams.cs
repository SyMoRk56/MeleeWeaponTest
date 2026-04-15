using UnityEngine;

[CreateAssetMenu(fileName ="Player params", menuName ="Params", order=0)]
public class PlayerParams : ScriptableObject
{
    public float walkSpeed;
    public float runSpeed;
    [HideInInspector] public LayerMask whatIsGround;
    public float maxStamina;
    public AnimationCurve movementCurve;
    public float accelerationTime;

    public Vector2 sensitivity;

    private void OnEnable()
    {
        whatIsGround = LayerMask.NameToLayer("Ground");
    }
}
