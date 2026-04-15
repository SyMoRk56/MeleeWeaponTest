using UnityEngine;

[CreateAssetMenu(fileName ="Player params", menuName ="Params", order=0)]
public class PlayerParams : ScriptableObject
{
    public float walkSpeed;
    public float runSpeed;
    public LayerMask whatIsGround = LayerMask.NameToLayer("Ground");
    public float maxStamina;
    public AnimationCurve movementCurve;
    public float accelerationTime;

    public Vector2 sensitivity;
}
