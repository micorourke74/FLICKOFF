using System.Collections.Generic;

namespace FlickOff
{
    public readonly struct FlickScrapBundle
    {
        public readonly int Copper;
        public readonly int Electronics;
        public readonly int GoldPlatedContacts;

        public FlickScrapBundle(int copper, int electronics, int goldPlatedContacts)
        {
            Copper = copper;
            Electronics = electronics;
            GoldPlatedContacts = goldPlatedContacts;
        }
    }

    /// <summary>Small deterministic run-state slice for the first playable camera hunt.</summary>
    public sealed class FlickPrototypeRunState
    {
        public const float AutoCollectDistanceMeters = 1.25f;

        private readonly HashSet<string> _destroyedCameraIds = new HashSet<string>();
        private readonly HashSet<string> _collectedScrapCameraIds = new HashSet<string>();

        public FlickPrototypeRunState(int quota)
        {
            Quota = quota < 0 ? 0 : quota;
        }

        public int Quota { get; }
        public int DestroyedCameraCount => _destroyedCameraIds.Count;
        public int RemainingQuota => Quota > DestroyedCameraCount ? Quota - DestroyedCameraCount : 0;
        public int CollectedScrapCameraCount => _collectedScrapCameraIds.Count;
        public FlickScrapBundle Scrap { get; private set; }

        public bool TryRecordDestruction(string cameraId)
        {
            string normalizedId = NormalizeId(cameraId);
            if (normalizedId.Length == 0 || !_destroyedCameraIds.Add(normalizedId))
            {
                return false;
            }

            Scrap = new FlickScrapBundle(
                Scrap.Copper + 3,
                Scrap.Electronics + 2,
                Scrap.GoldPlatedContacts + 1);
            return true;
        }

        public bool TryCollectScrap(string cameraId, bool playerIsOnFoot, float distanceMeters)
        {
            string normalizedId = NormalizeId(cameraId);
            if (!playerIsOnFoot || distanceMeters > AutoCollectDistanceMeters || normalizedId.Length == 0)
            {
                return false;
            }

            return _destroyedCameraIds.Contains(normalizedId) && _collectedScrapCameraIds.Add(normalizedId);
        }

        private static string NormalizeId(string cameraId)
        {
            return cameraId == null ? string.Empty : cameraId.Trim();
        }
    }
}
