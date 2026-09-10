using UnityEngine;
using UnityEngine.InputSystem;

namespace FlickOff
{
    /// <summary>Minimal first-person test vehicle used by the street prototype.</summary>
    public sealed class FlickPrototypePlayer : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float lookSensitivity = 0.08f;
        [SerializeField] private Transform view;

        public static FlickPrototypePlayer Active { get; private set; }
        public bool IsOnFoot => false;
        public Transform View => view;

        public void ConfigureView(Transform playerView)
        {
            view = playerView;
        }

        private void Awake()
        {
            Active = this;
        }

        private void OnDestroy()
        {
            if (Active == this)
            {
                Active = null;
            }
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                Vector2 input = Vector2.zero;
                if (keyboard.wKey.isPressed) input.y += 1f;
                if (keyboard.sKey.isPressed) input.y -= 1f;
                if (keyboard.dKey.isPressed) input.x += 1f;
                if (keyboard.aKey.isPressed) input.x -= 1f;

                if (input.sqrMagnitude > 1f)
                {
                    input.Normalize();
                }

                Vector3 movement = (transform.right * input.x + transform.forward * input.y) * moveSpeed * Time.deltaTime;
                transform.position += movement;
            }

            Mouse mouse = Mouse.current;
            if (mouse != null)
            {
                Vector2 delta = mouse.delta.ReadValue();
                transform.Rotate(0f, delta.x * lookSensitivity, 0f, Space.World);
                if (view != null)
                {
                    Vector3 angles = view.localEulerAngles;
                    float pitch = angles.x > 180f ? angles.x - 360f : angles.x;
                    pitch = Mathf.Clamp(pitch - delta.y * lookSensitivity, -75f, 75f);
                    view.localRotation = Quaternion.Euler(pitch, 0f, 0f);
                }
            }

            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                TryDamageCameraInFront();
            }
        }

        private void TryDamageCameraInFront()
        {
            if (view == null || !Physics.Raycast(view.position, view.forward, out RaycastHit hit, 10f))
            {
                return;
            }

            FlickCameraActor camera = hit.collider.GetComponentInParent<FlickCameraActor>();
            if (camera != null)
            {
                camera.TryDestroy(FlickDestructionCause.Damage);
            }
        }
    }
}
