using UnityEngine;

namespace FlickOff
{
    /// <summary>Wires the first street slice into a playable destruction and quota loop.</summary>
    public sealed class FlickPrototypeGame : MonoBehaviour
    {
        [SerializeField] private int quota = 3;
        private FlickPrototypeRunState _runState;
        private int _sightingCount;
        private string _lastEvent = "Scan the street. E damages a FLICK camera.";

        public FlickPrototypeRunState RunState => _runState;
        public int SightingCount => _sightingCount;
        public string LastEvent => _lastEvent;

        private void Awake()
        {
            _runState = new FlickPrototypeRunState(quota);
        }

        public void RegisterSighting(FlickCameraActor camera)
        {
            _sightingCount++;
            _lastEvent = camera.CameraId + " recorded a sighting.";
        }

        public bool TryDestroyCamera(FlickCameraActor camera, FlickDestructionCause cause)
        {
            if (!_runState.TryRecordDestruction(camera.CameraId))
            {
                return false;
            }

            camera.MarkDestroyed();
            _lastEvent = camera.CameraId + " destroyed. Collect the scrap.";
            SpawnScrap(camera.transform.position + Vector3.up * 0.5f, camera.CameraId);
            return true;
        }

        public bool TryCollectScrap(string cameraId, bool playerIsOnFoot, float distanceMeters)
        {
            bool collected = _runState.TryCollectScrap(cameraId, playerIsOnFoot, distanceMeters);
            if (collected)
            {
                _lastEvent = "Scrap collected. Copper " + _runState.Scrap.Copper + ", electronics " + _runState.Scrap.Electronics + ".";
            }

            return collected;
        }

        private void SpawnScrap(Vector3 position, string cameraId)
        {
            GameObject scrap = GameObject.CreatePrimitive(PrimitiveType.Cube);
            scrap.name = "ScrapDrop_" + cameraId;
            scrap.transform.position = position;
            scrap.transform.localScale = Vector3.one * 0.35f;
            FlickScrapDrop drop = scrap.AddComponent<FlickScrapDrop>();
            drop.Configure(cameraId, this);
        }

        private void OnGUI()
        {
            if (_runState == null)
            {
                return;
            }

            GUI.Box(new Rect(18f, 18f, 390f, 132f), "FLICK OFF // STREET TEST GROUND");
            GUI.Label(new Rect(34f, 48f, 350f, 22f), "Quota: " + _runState.DestroyedCameraCount + " / " + _runState.Quota + " cameras");
            GUI.Label(new Rect(34f, 72f, 350f, 22f), "Scrap: " + _runState.Scrap.Copper + " copper, " + _runState.Scrap.Electronics + " electronics, " + _runState.Scrap.GoldPlatedContacts + " contact");
            GUI.Label(new Rect(34f, 96f, 350f, 22f), "WASD move, mouse look, E damage camera");
            GUI.Label(new Rect(34f, 120f, 350f, 22f), _lastEvent);
        }
    }
}
