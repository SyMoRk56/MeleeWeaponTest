using UnityEngine;

namespace Game
{
    public interface IMoveInput
    {
        Vector2 Axis { get; }
        bool IsJumpPressed { get; }

        bool IsRunPressed { get; }

        Vector2 MouseAxis { get; }
    }
}
