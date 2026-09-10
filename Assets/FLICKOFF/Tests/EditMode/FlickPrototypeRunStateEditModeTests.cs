using NUnit.Framework;

namespace FlickOff.Tests
{
    public sealed class FlickPrototypeRunStateEditModeTests
    {
        [Test]
        public void DestructionCountsEachCameraOnceAndUsesDonorScrapBundle()
        {
            FlickPrototypeRunState state = new FlickPrototypeRunState(3);

            Assert.That(state.TryRecordDestruction(" camera-01 "), Is.True);
            Assert.That(state.TryRecordDestruction("camera-01"), Is.False);
            Assert.That(state.DestroyedCameraCount, Is.EqualTo(1));
            Assert.That(state.RemainingQuota, Is.EqualTo(2));
            Assert.That(state.Scrap.Copper, Is.EqualTo(3));
            Assert.That(state.Scrap.Electronics, Is.EqualTo(2));
            Assert.That(state.Scrap.GoldPlatedContacts, Is.EqualTo(1));
        }

        [Test]
        public void ScrapPickupRequiresOnFootPlayerWithinDonorDistanceAndIsOneTime()
        {
            FlickPrototypeRunState state = new FlickPrototypeRunState(1);
            state.TryRecordDestruction("camera-01");

            Assert.That(state.TryCollectScrap("camera-01", false, 0.5f), Is.False);
            Assert.That(state.TryCollectScrap("camera-01", true, 1.251f), Is.False);
            Assert.That(state.TryCollectScrap("camera-01", true, 1.25f), Is.True);
            Assert.That(state.TryCollectScrap("camera-01", true, 0.1f), Is.False);
            Assert.That(state.CollectedScrapCameraCount, Is.EqualTo(1));
        }

        [Test]
        public void BlankCameraIdsCannotChangeRunState()
        {
            FlickPrototypeRunState state = new FlickPrototypeRunState(1);

            Assert.That(state.TryRecordDestruction(" "), Is.False);
            Assert.That(state.DestroyedCameraCount, Is.EqualTo(0));
        }
    }
}
