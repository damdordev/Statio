using Damdor.Vario;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Damdor.VisualStates.Tests
{
    [TestFixture]
    public class VisualStateParameterTest
    {
        private class TestComponent : MonoBehaviour
        {
            public int Value;
        }

        private class TestVisualStateParameter : VisualStateParameter<TestComponent, int>
        {
            protected override int GetValue(TestComponent target) => target.Value;
            protected override void SetValue(TestComponent target, int value) => target.Value = value;
            protected override int Lerp(int a, int b, float t) => (int)Mathf.LerpUnclamped(a, b, t);
        }

        private GameObject gameObject;
        private TestComponent component;
        private TestVisualStateParameter parameter;
        private IVisualStateParameterLifecycle lifecycle;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject();
            component = gameObject.AddComponent<TestComponent>();
            parameter = new TestVisualStateParameter();
            parameter.Target = new VarioValue<TestComponent> { Value = component, Source = ValueSource.Raw };
            lifecycle = parameter;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void LoadDefaultValue_SetsComponentToDefault()
        {
            parameter.DefaultValue = new VarioValue<int> { Value = 10, Source = ValueSource.Raw };
            lifecycle.LoadDefaultValue();
            Assert.AreEqual(10, component.Value);
        }

        [Test]
        public void LoadValue_StateExists_SetsValue()
        {
            parameter.DefaultValue = new VarioValue<int> { Value = 10, Source = ValueSource.Raw };
            parameter.SetValue(1, new VarioValue<int> { Value = 20, Source = ValueSource.Raw });

            lifecycle.LoadValue(1);
            Assert.AreEqual(20, component.Value);
        }

        [Test]
        public void LoadValue_StateDoesNotExist_SetsDefaultValue()
        {
            parameter.DefaultValue = new VarioValue<int> { Value = 10, Source = ValueSource.Raw };
            lifecycle.LoadValue(99);
            Assert.AreEqual(10, component.Value);
        }

        [Test]
        [TestCase(-1, -50)]
        [TestCase(0, 50)]
        [TestCase(0.5f, 75)]
        [TestCase(1f, 100)]
        [TestCase(2f, 150)]
        public void SaveSnapshot_CapturesCurrentValue(float t, int expectedValue)
        {
            component.Value = 50;
            lifecycle.SaveSnapshot();
            
            component.Value = 0;
            
            parameter.DefaultValue = new VarioValue<int> { Value = 100, Source = ValueSource.Raw };
            
            lifecycle.LoadValue(0, 0.5f);
            Assert.AreEqual(75, component.Value);
        }

        [Test]
        public void NotifyStateRemoved_RemovesStateAndShiftsIndices()
        {
            parameter.SetValue(0, new VarioValue<int> { Value = 10 });
            parameter.SetValue(1, new VarioValue<int> { Value = 20 }); // To be removed
            parameter.SetValue(2, new VarioValue<int> { Value = 30 }); // Should shift to 1

            lifecycle.NotifyStateRemoved(1);

            Assert.IsTrue(parameter.HasOverride(0));
            Assert.AreEqual(10, parameter.GetValue(0).Value);

            Assert.IsTrue(parameter.HasOverride(1));
            Assert.AreEqual(30, parameter.GetValue(1).Value);
            
            Assert.IsFalse(parameter.HasOverride(2));
        }

        [Test]
        public void NotifyStateChanged_MoveUp_ShiftsIndicesCorrectly()
        {
            // Initial: [10:100]
            parameter.SetValue(10, new VarioValue<int> { Value = 100 });
            
            // Move 10 to 20
            lifecycle.NotifyStateChanged(10, 20);
            
            Assert.IsFalse(parameter.HasOverride(10));
            Assert.IsTrue(parameter.HasOverride(20));
            Assert.AreEqual(100, parameter.GetValue(20).Value);
        }

        [Test]
        public void SetValue_UpdatesExistingState()
        {
            parameter.SetValue(1, new VarioValue<int> { Value = 10 });
            parameter.SetValue(1, new VarioValue<int> { Value = 20 });

            Assert.AreEqual(20, parameter.GetValue(1).Value);
        }

        [Test]
        public void RemoveOverride_RemovesState()
        {
            parameter.SetValue(1, new VarioValue<int> { Value = 10 });
            parameter.RemoveOverride(1);

            Assert.IsFalse(parameter.HasOverride(1));
        }
    }
}