using System.Collections.Generic;
using Damdor.Vario;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Damdor.Statio.Tests
{
    public class StatioParameterLifecycleTests : StatioTestSupport
    {
        private CanvasGroup group;
        private CanvasGroupAlphaStatioParameter parameter;
        private IStatioParameterLifecycle lifecycle;

        [SetUp]
        public void CreateParameter()
        {
            group = NewObject().AddComponent<CanvasGroup>();
            parameter = new CanvasGroupAlphaStatioParameter { Target = group, DefaultValue = 0.2f };
            lifecycle = parameter;
        }

        [Test]
        public void CaptureDefaultValue_ReplacesStorageReferenceWithRawSnapshot()
        {
            parameter.DefaultValue = VarioValue<float>.FromStorage("old");
            group.alpha = 0.7f;
            lifecycle.SaveCurrentValueToDefaultValue();
            Assert.That(parameter.DefaultValue.Source, Is.EqualTo(ValueSource.Raw));
            Near(parameter.DefaultValue.Value, 0.7f);
            group.alpha = 0f;
            lifecycle.LoadDefaultValue();
            Near(group.alpha, 0.7f);
        }

        [Test]
        public void CaptureStateValue_UpdatesExistingOverrideWithoutDuplicatingIt()
        {
            parameter.SetValue(1, 0.3f);
            group.alpha = 0.8f;
            lifecycle.SaveCurrentValueToState(1);
            Assert.That(parameter.GetValue(1).Source, Is.EqualTo(ValueSource.Raw));
            Near(parameter.GetValue(1).Value, 0.8f);
            parameter.RemoveOverride(1);
            Assert.That(parameter.HasOverride(1), Is.False);
            Near(parameter.GetValue(1).Value, 0.2f);
        }

        [Test]
        public void CaptureStateValue_CreatesNewOverride()
        {
            group.alpha = 0.6f;
            lifecycle.SaveCurrentValueToState(2);
            Assert.That(parameter.HasOverride(2), Is.True);
            Near(parameter.GetValue(2).Value, 0.6f);
        }

        [Test]
        public void LoadValue_ResolvesTargetAndValueFromAssignedStorage()
        {
            var storage = new VarioStorage();
            storage.Update("target", group);
            storage.Update("alpha", 0.9f);
            lifecycle.Storage = storage;
            parameter.Target = VarioValue<CanvasGroup>.FromStorage("target");
            parameter.SetValue(2, VarioValue<float>.FromStorage("alpha"));
            lifecycle.LoadValue(2);
            Near(group.alpha, 0.9f);
            storage.Update("alpha", 0.4f);
            lifecycle.LoadValue(2);
            Near(group.alpha, 0.4f);
        }

        [Test]
        public void LoadDefaultValue_ResolvesStorageValue()
        {
            lifecycle.Storage = new VarioStorage();
            lifecycle.Storage.Update("alpha", 0.65f);
            parameter.DefaultValue = VarioValue<float>.FromStorage("alpha");
            lifecycle.LoadDefaultValue();
            Near(group.alpha, 0.65f);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void MissingOrDestroyedTarget_IsSkippedWhenApplyingValues(bool destroyed)
        {
            if (destroyed) Object.DestroyImmediate(group);
            else parameter.Target = VarioValue<CanvasGroup>.Raw(null);
            Assert.DoesNotThrow(() =>
            {
                lifecycle.SaveSnapshot();
                lifecycle.LoadDefaultValue();
                lifecycle.LoadValue(1);
                lifecycle.LoadValue(1, 0.5f);
            });
        }

        [Test]
        public void RemovingUnknownOverride_DoesNotAffectOtherValues()
        {
            parameter.SetValue(1, 0.8f);
            parameter.RemoveOverride(99);
            Near(parameter.GetValue(1).Value, 0.8f);
        }

        [TestCase(0, 3)]
        [TestCase(3, 0)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        public void ReorderingOverrides_PreservesValuesForEveryState(int from, int to)
        {
            var expected = new List<float> { 0.1f, 0.2f, 0.3f, 0.4f };
            for (var i = 0; i < expected.Count; i++) parameter.SetValue(i, expected[i]);
            var moved = expected[from];
            expected.RemoveAt(from);
            expected.Insert(to, moved);
            lifecycle.NotifyStateChanged(from, to);
            for (var i = 0; i < expected.Count; i++) Near(parameter.GetValue(i).Value, expected[i]);
            Assert.That(parameter.HasOverride(4), Is.False);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(3)]
        public void RemovingState_RemovesOverrideAndShiftsRemainingValues(int removed)
        {
            var expected = new List<float> { 0.1f, 0.2f, 0.3f, 0.4f };
            for (var i = 0; i < expected.Count; i++) parameter.SetValue(i, expected[i]);
            expected.RemoveAt(removed);
            lifecycle.NotifyStateRemoved(removed);
            for (var i = 0; i < expected.Count; i++) Near(parameter.GetValue(i).Value, expected[i]);
            Assert.That(parameter.HasOverride(3), Is.False);
        }

        [TestCase(-0.5f, -0.2f)]
        [TestCase(0f, 0.2f)]
        [TestCase(0.5f, 0.6f)]
        [TestCase(1f, 1f)]
        public void NumericInterpolation_UsesRegisteredUnclampedOperations(float t, float expected)
        {
            // Transform position does not clamp output, unlike some visual properties.
            var position = new PositionStatioParameter { Target = group.transform, DefaultValue = Vector3.one };
            group.transform.localPosition = Vector3.one * 0.2f;
            var positionLifecycle = (IStatioParameterLifecycle)position;
            positionLifecycle.SaveSnapshot();
            positionLifecycle.LoadValue(0, t);
            Near(group.transform.localPosition.x, expected);
        }

        private enum Choice { First, Second }
        private sealed class ChoiceParameter : StatioParameter<CanvasGroup, Choice>
        {
            public Choice Current;
            protected override Choice GetValue(CanvasGroup target) => Current;
            protected override void SetValue(CanvasGroup target, Choice value) => Current = value;
        }

        [TestCase(0f, false)]
        [TestCase(0.49f, false)]
        [TestCase(0.5f, true)]
        [TestCase(1f, true)]
        public void UnsupportedNumericType_SwitchesAtHalfwayWithoutThrowing(float t, bool second)
        {
            var choice = new ChoiceParameter { Target = group, DefaultValue = Choice.Second };
            var choiceLifecycle = (IStatioParameterLifecycle)choice;
            choiceLifecycle.SaveSnapshot();
            Assert.DoesNotThrow(() => choiceLifecycle.LoadValue(0, t));
            Assert.That(choice.Current, Is.EqualTo(second ? Choice.Second : Choice.First));
        }
    }
}
