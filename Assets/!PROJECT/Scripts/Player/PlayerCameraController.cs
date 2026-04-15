using UnityEngine;
using Zenject;

namespace Game
{
    public class PlayerCameraController : MonoBehaviour
    {

        [Inject] private PlayerParams _params;
        [Inject] private PlayerMoveInput _input;
        private Transform _orientation;
        private float _xRotation, _yRotation;
        public void Construct(PlayerManager playerManager)
        {
            _orientation = playerManager.Orientation;
        }
        private void Update()
        {
            float mouseX = _input.MouseAxis.x * Time.deltaTime * _params.sensitivity.x;
            float mouseY = _input.MouseAxis.y * Time.deltaTime * _params.sensitivity.y;

            _yRotation += mouseX;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
            _orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
        }

    }

}
