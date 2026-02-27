using NUnit.Framework;

namespace Damdor.VisualStates.Tests
{
    [TestFixture]
    public class VisualStateHelperTest
    {

        [Test]
        [TestCase(1, 1, -1)]
        [TestCase(5, 5, -1)]
        [TestCase(5, 3, 3)]
        [TestCase(3, 5, 4)]
        public void Test_RecalculateStateIdAfterStateRemoved(int removedStateId, int stateId, int expectedId)
        {
            Assert.AreEqual(expectedId, VisualStateHelper.RecalculateStateIdAfterStateRemoved(removedStateId, stateId));
        }

        [Test]
        [TestCase(0, 1, 0, 1)]
        [TestCase(0, 1, 1, 0)]
        [TestCase(0, 2, 1, 0)]
        [TestCase(2, 3, 1, 1)]
        [TestCase(1, 0, 0, 1)]
        [TestCase(1, 0, 1, 0)]
        public void Test_RecalculateStateIdAfterStateIdChanged(int oldMovedStateId, int newMovedStateId, int stateId, int expectedId)
        {
            Assert.AreEqual(expectedId, VisualStateHelper.RecalculateStateIdAfterStateIdChanged(oldMovedStateId, newMovedStateId, stateId));
        }
        
    }
}