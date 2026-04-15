using UnityEngine;

namespace Game
{
    public interface IMoveInput
    {
        Vector2 MoveAxis { get; }
        bool IsJumpPressed { get; }

        bool IsRunPressed { get; }

        Vector2 MouseAxis { get; }

        bool IsLeftMousePressed { get; }

        bool IsRightMousePressed { get; }

        bool IsLeftMouseDown { get; }
    }
}
