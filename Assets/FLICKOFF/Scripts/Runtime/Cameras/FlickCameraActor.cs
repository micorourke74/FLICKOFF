using UnityEngine;

namespace FlickOff
{
    /// <summary>Scene-facing FLICK camera actor for the first playable street slice.</summary>
    public sealed class FlickCameraActor : MonoBehaviour
    {
        [SerializeField] private string cameraId = "camera";
        [SerializeField] private float compassHeading;
        [SerializeField] private Vector3 groundPosition;

        private FlickCamera _camera;
        private FlickPrototypeGame _game;
        private Renderer[] _renderers;

        public string CameraId => cameraId;
        public bool IsDestroyed { get; private set; }

        public void Configure(string id, float heading, Vector3 position)
        {
            cameraId = id;
            compassHeading = heading;
            groundPosition = position;
        }

        private void Start()
        {
            _game = FindFirstObjectByType<FlickPrototypeGame>();
            _renderers = GetComponentsInChildren<Renderer>();
            FlickCameraDefinition definition = new FlickCameraDefinition(cameraId, groundPosition, compassHeading);
            _camera = new FlickCamera(definition, new FlickVisionSensor(new FlickUnityLineOfSightQuery()));
            SetIndicator(Color.white);
        }

        private void Update()
        {
            if (IsDestroyed || _camera == null || FlickPrototypePlayer.Active == null)
            {
                return;
            }

            FlickCameraTick tick = _camera.Tick(
                FlickPrototypePlayer.Active.transform.position,
                FlickTargetKind.PlayerVehicle,
                false,
                Mathf.RoundToInt(Time.time * 1000f));

            SetIndicator(tick.Visibility.CanSee ? Color.red : Color.white);
            if (tick.Observation.IsNewSighting && _game != null)
            {
                _game.RegisterSighting(this);
            }
        }

        public bool TryDestroy(FlickDestructionCause cause)
        {
            if (IsDestroyed || _game == null)
            {
                return false;
            }

            return _game.TryDestroyCamera(this, cause);
        }

        internal void MarkDestroyed()
        {
            IsDestroyed = true;
            SetIndicator(Color.black);
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }
        }

        private void SetIndicator(Color color)
        {
            if (_renderers == null)
            {
                return;
            }

            foreach (Renderer renderer in _renderers)
            {
                renderer.material.color = color;
            }
        }
    }
}
