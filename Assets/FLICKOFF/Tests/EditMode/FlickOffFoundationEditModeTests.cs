using NUnit.Framework;

namespace FlickOff.Tests
{
    public sealed class FlickOffFoundationEditModeTests
    {
        [Test]
        public void ProjectIdentityUsesShippingTerminology()
        {
            Assert.That(FlickOffFoundation.ProjectName, Is.EqualTo("FLICK OFF"));
            Assert.That(FlickOffFoundation.RootNamespace, Is.EqualTo("FlickOff"));
            Assert.That(FlickOffFoundation.CameraTerm, Is.EqualTo("FLICK camera"));
        }
    }
}
