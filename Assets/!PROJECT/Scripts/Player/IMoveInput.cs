using UnityEngine;

namespace Game
{
    public interface IMoveInput
    {
        Vector2 Axis { get; }
        bool IsJumpPressed { get; }
    }
}
