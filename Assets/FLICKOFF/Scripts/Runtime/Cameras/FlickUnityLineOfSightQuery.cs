using UnityEngine;

namespace FlickOff
{
    /// <summary>Unity physics translation of the donor's map/object/vehicle raycast.</summary>
    public sealed class FlickUnityLineOfSightQuery : IFlickLineOfSightQuery
    {
        private readonly int _blockingLayers;

        public FlickUnityLineOfSightQuery()
            : this(Physics.DefaultRaycastLayers)
        {
        }

        public FlickUnityLineOfSightQuery(int blockingLayers)
        {
            _blockingLayers = blockingLayers;
        }

        public bool HasLineOfSight(Vector3 origin, Vector3 target)
        {
            Vector3 displacement = target - origin;
            float distance = displacement.magnitude;
            if (distance <= 0.0001f)
            {
                return true;
            }

            return !Physics.Raycast(
                origin,
                displacement / distance,
                distance,
                _blockingLayers,
                QueryTriggerInteraction.Ignore);
        }
    }
}
