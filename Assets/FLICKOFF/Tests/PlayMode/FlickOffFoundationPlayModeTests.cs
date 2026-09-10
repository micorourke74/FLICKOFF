using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace FlickOff.Tests
{
    public sealed class FlickOffFoundationPlayModeTests
    {
        [UnityTest]
        public IEnumerator FoundationAssemblyLoadsInPlayMode()
        {
            yield return null;
            Assert.That(FlickOffFoundation.ProjectName, Is.EqualTo("FLICK OFF"));
        }
    }
}
