using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlickOff
{
    /// <summary>Behavioral constants copied from the pinned GTALPR snapshot.</summary>
    public readonly struct FlickCameraTuning
    {
        public readonly float FieldOfViewDegrees;
        public readonly float RangeMeters;
        public readonly float EyeHeightMeters;
        public readonly float ActivationDistanceMeters;
        public readonly int ActivationCheckIntervalMilliseconds;
        public readonly int SightingCooldownMilliseconds;

        public FlickCameraTuning(
            float fieldOfViewDegrees,
            float rangeMeters,
            float eyeHeightMeters,
            float activationDistanceMeters,
            int activationCheckIntervalMilliseconds,
            int sightingCooldownMilliseconds)
        {
            FieldOfViewDegrees = fieldOfViewDegrees;
            RangeMeters = rangeMeters;
            EyeHeightMeters = eyeHeightMeters;
            ActivationDistanceMeters = activationDistanceMeters;
            ActivationCheckIntervalMilliseconds = activationCheckIntervalMilliseconds;
            SightingCooldownMilliseconds = sightingCooldownMilliseconds;
        }

        public static FlickCameraTuning Donor
        {
            get
            {
                return new FlickCameraTuning(
                    120f,
                    44.86f,
                    3.49f,
                    150f,
                    1000,
                    2000);
            }
        }
    }

    public static class FlickCameraMath
    {
        public static float CompassHeadingToGtaHeading(float compassHeading)
        {
            float normalized = compassHeading % 360f;
            if (normalized < 0f)
            {
                normalized += 360f;
            }

            return (360f - normalized) % 360f;
        }

        public static Vector3 HeadingToForward(float headingDegrees)
        {
            float radians = headingDegrees * Mathf.Deg2Rad;
            return new Vector3(-Mathf.Sin(radians), 0f, Mathf.Cos(radians));
        }
    }

    public sealed class FlickCameraDefinition
    {
        public string Id { get; }
        public string OsmType { get; }
        public string OsmId { get; }
        public Vector3 Position { get; }
        public float HeadingDegrees { get; }

        public FlickCameraDefinition(
            string id,
            Vector3 position,
            float compassHeading,
            string osmType = null,
            string osmId = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("A FLICK camera requires a stable id.", nameof(id));
            }

            Id = id.Trim();
            OsmType = osmType == null ? string.Empty : osmType.Trim();
            OsmId = osmId == null ? string.Empty : osmId.Trim();
            Position = position;
            HeadingDegrees = FlickCameraMath.CompassHeadingToGtaHeading(compassHeading);
        }

        public Vector3 EyePosition(float eyeHeightMeters)
        {
            return Position + Vector3.up * eyeHeightMeters;
        }
    }

    public sealed class FlickCameraCatalog
    {
        private readonly Dictionary<string, FlickCameraDefinition> _byId;

        public FlickCameraCatalog(IEnumerable<FlickCameraDefinition> definitions)
        {
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            _byId = new Dictionary<string, FlickCameraDefinition>(StringComparer.Ordinal);
            foreach (FlickCameraDefinition definition in definitions)
            {
                if (definition == null)
                {
                    throw new ArgumentException("Camera catalog cannot contain null definitions.", nameof(definitions));
                }

                if (!_byId.TryAdd(definition.Id, definition))
                {
                    throw new ArgumentException("Camera catalog contains a duplicate stable id: " + definition.Id, nameof(definitions));
                }
            }
        }

        public int Count => _byId.Count;

        public bool TryGet(string id, out FlickCameraDefinition definition)
        {
            return _byId.TryGetValue(id, out definition);
        }
    }

    public enum FlickTargetKind
    {
        OnFootPlayer,
        PlayerVehicle
    }

    public interface IFlickLineOfSightQuery
    {
        bool HasLineOfSight(Vector3 origin, Vector3 target);
    }

    public readonly struct FlickVisibility
    {
        public readonly bool InRange;
        public readonly bool InFieldOfView;
        public readonly bool HasLineOfSight;
        public readonly bool CanSee;

        public FlickVisibility(bool inRange, bool inFieldOfView, bool hasLineOfSight, bool canSee)
        {
            InRange = inRange;
            InFieldOfView = inFieldOfView;
            HasLineOfSight = hasLineOfSight;
            CanSee = canSee;
        }
    }

    public sealed class FlickVisionSensor
    {
        private readonly IFlickLineOfSightQuery _lineOfSightQuery;
        private readonly FlickCameraTuning _tuning;

        public FlickVisionSensor(IFlickLineOfSightQuery lineOfSightQuery)
            : this(lineOfSightQuery, FlickCameraTuning.Donor)
        {
        }

        public FlickVisionSensor(IFlickLineOfSightQuery lineOfSightQuery, FlickCameraTuning tuning)
        {
            _lineOfSightQuery = lineOfSightQuery ?? throw new ArgumentNullException(nameof(lineOfSightQuery));
            _tuning = tuning;
        }

        public FlickVisibility Evaluate(
            FlickCameraDefinition camera,
            Vector3 targetPosition,
            FlickTargetKind targetKind)
        {
            if (camera == null)
            {
                throw new ArgumentNullException(nameof(camera));
            }

            if (targetKind != FlickTargetKind.PlayerVehicle)
            {
                return new FlickVisibility(false, false, false, false);
            }

            Vector3 offset = targetPosition - camera.Position;
            float distanceSquared = offset.x * offset.x + offset.z * offset.z;
            bool inRange = distanceSquared <= _tuning.RangeMeters * _tuning.RangeMeters;
            if (!inRange)
            {
                return new FlickVisibility(false, false, false, false);
            }

            bool inFieldOfView;
            if (distanceSquared < 0.0001f)
            {
                inFieldOfView = true;
            }
            else
            {
                float distance = Mathf.Sqrt(distanceSquared);
                Vector3 direction = new Vector3(offset.x / distance, 0f, offset.z / distance);
                Vector3 forward = FlickCameraMath.HeadingToForward(camera.HeadingDegrees);
                float minimumVisibleDot = Mathf.Cos(_tuning.FieldOfViewDegrees * 0.5f * Mathf.Deg2Rad);
                inFieldOfView = Vector3.Dot(forward, direction) >= minimumVisibleDot;
            }

            if (!inFieldOfView)
            {
                return new FlickVisibility(true, false, false, false);
            }

            Vector3 eyePosition = camera.EyePosition(_tuning.EyeHeightMeters);
            Vector3 vehicleTargetPosition = targetPosition + Vector3.up * 0.5f;
            bool hasLineOfSight = _lineOfSightQuery.HasLineOfSight(eyePosition, vehicleTargetPosition);
            return new FlickVisibility(true, true, hasLineOfSight, hasLineOfSight);
        }
    }

    public readonly struct FlickObservationResult
    {
        public readonly bool IsNewSighting;
        public readonly bool ManualCapture;

        public FlickObservationResult(bool isNewSighting, bool manualCapture)
        {
            IsNewSighting = isNewSighting;
            ManualCapture = manualCapture;
        }
    }

    public sealed class FlickObservationState
    {
        public bool WasSeeing { get; private set; }
        public long SightingCooldownUntilMilliseconds { get; private set; }

        public FlickObservationResult Tick(
            bool cameraCanSee,
            bool manualPhotoRequested,
            long currentTimeMilliseconds)
        {
            bool cooldownElapsed = currentTimeMilliseconds >= SightingCooldownUntilMilliseconds;
            bool isNewSighting = cameraCanSee && !WasSeeing && cooldownElapsed;
            bool manualCapture = manualPhotoRequested && cameraCanSee;

            WasSeeing = cameraCanSee;
            if (isNewSighting)
            {
                SightingCooldownUntilMilliseconds = currentTimeMilliseconds + FlickCameraTuning.Donor.SightingCooldownMilliseconds;
            }

            return new FlickObservationResult(isNewSighting, manualCapture);
        }
    }

    public readonly struct FlickCameraTick
    {
        public readonly FlickVisibility Visibility;
        public readonly FlickObservationResult Observation;

        public FlickCameraTick(FlickVisibility visibility, FlickObservationResult observation)
        {
            Visibility = visibility;
            Observation = observation;
        }
    }

    /// <summary>Coordinates one camera's sensor and edge-triggered observation state.</summary>
    public sealed class FlickCamera
    {
        private readonly FlickVisionSensor _sensor;
        private readonly FlickObservationState _observationState = new FlickObservationState();

        public FlickCameraDefinition Definition { get; }

        public FlickCamera(FlickCameraDefinition definition, FlickVisionSensor sensor)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            _sensor = sensor ?? throw new ArgumentNullException(nameof(sensor));
        }

        public FlickCameraTick Tick(
            Vector3 targetPosition,
            FlickTargetKind targetKind,
            bool manualPhotoRequested,
            long currentTimeMilliseconds)
        {
            FlickVisibility visibility = _sensor.Evaluate(Definition, targetPosition, targetKind);
            FlickObservationResult observation = _observationState.Tick(
                visibility.CanSee,
                manualPhotoRequested,
                currentTimeMilliseconds);
            return new FlickCameraTick(visibility, observation);
        }
    }

    public enum FlickDestructionCause
    {
        Damage,
        VehicleImpact
    }

    public sealed class FlickBreakableAssembly
    {
        public bool IsDestroyed { get; private set; }
        public int DestructionCount { get; private set; }
        public FlickDestructionCause LastCause { get; private set; }

        public bool TryDestroy(FlickDestructionCause cause)
        {
            if (IsDestroyed)
            {
                return false;
            }

            IsDestroyed = true;
            DestructionCount++;
            LastCause = cause;
            return true;
        }
    }
}
