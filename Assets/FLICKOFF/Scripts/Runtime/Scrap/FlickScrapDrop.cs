using UnityEngine;

namespace FlickOff
{
    public sealed class FlickScrapDrop : MonoBehaviour
    {
        private string _cameraId;
        private FlickPrototypeGame _game;

        public void Configure(string cameraId, FlickPrototypeGame game)
        {
            _cameraId = cameraId;
            _game = game;
        }

        private void Update()
        {
            FlickPrototypePlayer player = FlickPrototypePlayer.Active;
            if (_game == null || player == null)
            {
                return;
            }

            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (_game.TryCollectScrap(_cameraId, player.IsOnFoot, distance))
            {
                Destroy(gameObject);
            }
        }
    }
}
