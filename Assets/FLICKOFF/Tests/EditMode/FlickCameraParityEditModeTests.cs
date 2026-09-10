using System;
using NUnit.Framework;
using UnityEngine;

namespace FlickOff.Tests
{
    public sealed class FlickCameraParityEditModeTests
    {
        [Test]
        public void DonorTuningMatchesGtalprCameraValues()
        {
            FlickCameraTuning tuning = FlickCameraTuning.Donor;

            Assert.That(tuning.FieldOfViewDegrees, Is.EqualTo(120f));
            Assert.That(tuning.RangeMeters, Is.EqualTo(44.86f));
            Assert.That(tuning.EyeHeightMeters, Is.EqualTo(3.49f));
            Assert.That(tuning.ActivationDistanceMeters, Is.EqualTo(150f));
            Assert.That(tuning.ActivationCheckIntervalMilliseconds, Is.EqualTo(1000));
            Assert.That(tuning.SightingCooldownMilliseconds, Is.EqualTo(2000));
        }

        [TestCase(0f, 0f)]
        [TestCase(90f, 270f)]
        [TestCase(360f, 0f)]
        [TestCase(-90f, 90f)]
        public void CompassHeadingUsesDonorGtaConversion(float compass, float expected)
        {
            Assert.That(FlickCameraMath.CompassHeadingToGtaHeading(compass), Is.EqualTo(expected));
        }

        [Test]
        public void HeadingZeroFacesUnityForwardOnTheXZGroundPlane()
        {
            Assert.That(FlickCameraMath.HeadingToForward(0f), Is.EqualTo(Vector3.forward));
        }

        [Test]
        public void DefinitionTrimsStableIdAndExposesEyePosition()
        {
            FlickCameraDefinition definition = new FlickCameraDefinition(
                "  camera-01  ", new Vector3(2f, 3f, 4f), 0f);

            Assert.That(definition.Id, Is.EqualTo("camera-01"));
            Assert.That(definition.EyePosition(3.49f), Is.EqualTo(new Vector3(2f, 6.49f, 4f)));
        }

        [Test]
        public void CatalogRejectsBlankAndDuplicateStableIds()
        {
            Assert.Throws<ArgumentException>(() => new FlickCameraDefinition(" ", Vector3.zero, 0f));

            FlickCameraDefinition first = new FlickCameraDefinition("camera-01", Vector3.zero, 0f);
            FlickCameraDefinition duplicate = new FlickCameraDefinition(" camera-01 ", Vector3.one, 0f);
            Assert.Throws<ArgumentException>(() => new FlickCameraCatalog(new[] { first, duplicate }));
        }

        [Test]
        public void VisionAcceptsZeroDistanceVehicleLikeDonor()
        {
            FlickCameraDefinition camera = new FlickCameraDefinition("camera-01", Vector3.zero, 0f);
            FlickVisionSensor sensor = new FlickVisionSensor(new FixedLineOfSight(true));

            FlickVisibility visibility = sensor.Evaluate(
                camera, Vector3.zero, FlickTargetKind.PlayerVehicle);

            Assert.That(visibility.InRange, Is.True);
            Assert.That(visibility.InFieldOfView, Is.True);
            Assert.That(visibility.CanSee, Is.True);
        }

        [Test]
        public void VisionUsesHorizontalRangeAndSixtyDegreeHalfAngle()
        {
            FlickCameraDefinition camera = new FlickCameraDefinition("camera-01", Vector3.zero, 0f);
            FlickVisionSensor sensor = new FlickVisionSensor(new FixedLineOfSight(true));

            FlickVisibility inside = sensor.Evaluate(
                camera, new Vector3(10f, 0f, 10f), FlickTargetKind.PlayerVehicle);
            FlickVisibility outside = sensor.Evaluate(
                camera, new Vector3(18f, 0f, 10f), FlickTargetKind.PlayerVehicle);
            FlickVisibility outOfRange = sensor.Evaluate(
                camera, new Vector3(0f, 0f, 45f), FlickTargetKind.PlayerVehicle);

            Assert.That(inside.CanSee, Is.True);
            Assert.That(outside.InRange, Is.True);
            Assert.That(outside.InFieldOfView, Is.False);
            Assert.That(outside.CanSee, Is.False);
            Assert.That(outOfRange.InRange, Is.False);
        }

        [Test]
        public void VisionRequiresPlayerVehicleAndLineOfSight()
        {
            FlickCameraDefinition camera = new FlickCameraDefinition("camera-01", Vector3.zero, 0f);
            FlickVisionSensor blockedSensor = new FlickVisionSensor(new FixedLineOfSight(false));

            FlickVisibility onFoot = blockedSensor.Evaluate(
                camera, new Vector3(0f, 0f, 10f), FlickTargetKind.OnFootPlayer);
            FlickVisibility blockedVehicle = blockedSensor.Evaluate(
                camera, new Vector3(0f, 0f, 10f), FlickTargetKind.PlayerVehicle);

            Assert.That(onFoot.CanSee, Is.False);
            Assert.That(blockedVehicle.InFieldOfView, Is.True);
            Assert.That(blockedVehicle.HasLineOfSight, Is.False);
            Assert.That(blockedVehicle.CanSee, Is.False);
        }

        [Test]
        public void ObservationTriggersOnlyOnVisibilityEdgeAfterCooldown()
        {
            FlickObservationState state = new FlickObservationState();

            Assert.That(state.Tick(true, false, 0).IsNewSighting, Is.True);
            Assert.That(state.Tick(true, false, 1000).IsNewSighting, Is.False);
            Assert.That(state.Tick(false, false, 1500).IsNewSighting, Is.False);
            Assert.That(state.Tick(true, false, 1500).IsNewSighting, Is.False);
            Assert.That(state.Tick(false, false, 1999).IsNewSighting, Is.False);
            Assert.That(state.Tick(true, false, 2000).IsNewSighting, Is.True);
        }

        [Test]
        public void ManualCaptureWorksWhileVehicleRemainsVisible()
        {
            FlickObservationState state = new FlickObservationState();

            state.Tick(true, false, 0);
            FlickObservationResult result = state.Tick(true, true, 100);

            Assert.That(result.IsNewSighting, Is.False);
            Assert.That(result.ManualCapture, Is.True);
        }

        [Test]
        public void DestructionIsAOneTimeTransition()
        {
            FlickBreakableAssembly assembly = new FlickBreakableAssembly();

            Assert.That(assembly.TryDestroy(FlickDestructionCause.Damage), Is.True);
            Assert.That(assembly.TryDestroy(FlickDestructionCause.VehicleImpact), Is.False);
            Assert.That(assembly.IsDestroyed, Is.True);
            Assert.That(assembly.DestructionCount, Is.EqualTo(1));
        }

        [Test]
        public void CameraCoordinatorCombinesVisibilityAndSightingEdge()
        {
            FlickCameraDefinition definition = new FlickCameraDefinition("camera-01", Vector3.zero, 0f);
            FlickCamera camera = new FlickCamera(definition, new FlickVisionSensor(new FixedLineOfSight(true)));

            FlickCameraTick first = camera.Tick(new Vector3(0f, 0f, 10f), FlickTargetKind.PlayerVehicle, false, 0);
            FlickCameraTick stable = camera.Tick(new Vector3(0f, 0f, 10f), FlickTargetKind.PlayerVehicle, false, 100);

            Assert.That(first.Visibility.CanSee, Is.True);
            Assert.That(first.Observation.IsNewSighting, Is.True);
            Assert.That(stable.Visibility.CanSee, Is.True);
            Assert.That(stable.Observation.IsNewSighting, Is.False);
        }

        private sealed class FixedLineOfSight : IFlickLineOfSightQuery
        {
            private readonly bool _result;

            public FixedLineOfSight(bool result)
            {
                _result = result;
            }

            public bool HasLineOfSight(Vector3 origin, Vector3 target)
            {
                return _result;
            }
        }
    }
}
