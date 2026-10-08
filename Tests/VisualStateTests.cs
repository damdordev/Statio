using System;
using Damdor.Vario;
using NUnit.Framework;
using UnityEngine;

namespace Damdor.Statio.Tests
{
    public class VisualStateTests : StatioTestSupport
    {
        private VisualState state;
        private CanvasGroup group;
        private CanvasGroupAlphaStatioParameter parameter;

        [SetUp]
        public void CreateState()
        {
            state = NewState("Idle", "Hover", "Pressed");
            parameter = Alpha(state, 0f, 1f, 0.5f);
            group = state.GetComponent<CanvasGroup>();
            state.ChangeStateImmediately("Idle");
            Animations(state, Animation(2f));
        }

        [Test]
        public void AddState_AddsUniqueStateInOrder()
        {
            state.AddState("Disabled");
            CollectionAssert.AreEqual(new[] { "Idle", "Hover", "Pressed", "Disabled" }, state.States);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("Idle")]
        public void AddState_InvalidName_DoesNotModifyStates(string name)
        {
            state.AddState(name);
            Assert.That(state.States.Count, Is.EqualTo(3));
            Assert.That(Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public void RemoveState_MissingState_ReportsErrorWithoutChangingState()
        {
            state.RemoveState("Missing");
            Assert.That(state.States.Count, Is.EqualTo(3));
            Assert.That(state.CurrentState, Is.EqualTo("Idle"));
            Assert.That(Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public void RemoveState_PreservesCurrentAndInitialStateAndOverrides()
        {
            state.InitialState = "Pressed";
            state.ChangeStateImmediately("Pressed");
            state.RemoveState("Idle");
            Assert.That(state.CurrentState, Is.EqualTo("Pressed"));
            Assert.That(state.InitialState, Is.EqualTo("Pressed"));
            state.ChangeStateImmediately("Hover");
            Near(group.alpha, 1f);
            state.ChangeStateImmediately("Pressed");
            Near(group.alpha, 0.5f);
        }

        [Test]
        public void RemoveState_RemovingCurrentAndInitialState_ClearsTheirNames()
        {
            state.InitialState = "Idle";
            state.RemoveState("Idle");
            Assert.That(state.CurrentState, Is.Empty);
            Assert.That(state.InitialState, Is.Empty);
        }

        [TestCase("Idle", 2)]
        [TestCase("Pressed", 0)]
        [TestCase("Hover", 1)]
        public void ChangeStateId_PreservesStateIdentityAndParameterValues(string moved, int index)
        {
            state.InitialState = "Idle";
            state.ChangeStateId(moved, index);
            Assert.That(state.States[index], Is.EqualTo(moved));
            Assert.That(state.InitialState, Is.EqualTo("Idle"));
            Assert.That(state.CurrentState, Is.EqualTo("Idle"));
            state.ChangeStateImmediately("Hover");
            Near(group.alpha, 1f);
            state.ChangeStateImmediately("Pressed");
            Near(group.alpha, 0.5f);
        }

        [TestCase(-1)]
        [TestCase(3)]
        [TestCase(100)]
        public void ChangeStateId_InvalidIndex_DoesNotMutateStates(int index)
        {
            state.ChangeStateId("Hover", index);
            CollectionAssert.AreEqual(new[] { "Idle", "Hover", "Pressed" }, state.States);
            Assert.That(Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public void ChangeStateId_MissingState_DoesNotMutateStates()
        {
            state.ChangeStateId("Missing", 0);
            Assert.That(state.States.Count, Is.EqualTo(3));
            Assert.That(Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public void InitialState_MissingName_ClearsSelection()
        {
            state.InitialState = "Missing";
            Assert.That(state.InitialState, Is.Empty);
        }

        [Test]
        public void Awake_AppliesInitialStateWithoutAnimating()
        {
            var fresh = NewState("Idle", "Hover");
            Alpha(fresh, 0f, 1f);
            Animations(fresh, Animation(10f));
            fresh.InitialState = "Hover";
            Lifecycle(fresh, "Awake");
            Assert.That(fresh.CurrentState, Is.EqualTo("Hover"));
            Near(fresh.GetComponent<CanvasGroup>().alpha, 1f);
        }

        [Test]
        public void Awake_DoesNotOverwriteAlreadySelectedState()
        {
            state.InitialState = "Hover";
            Lifecycle(state, "Awake");
            Assert.That(state.CurrentState, Is.EqualTo("Idle"));
        }

        [Test]
        public void ChangeStateImmediately_AppliesTargetAndStopsPreviousAnimation()
        {
            state.ChangeState("Hover");
            state.UpdateTime(0.5f);
            state.ChangeStateImmediately("Pressed");
            Near(group.alpha, 0.5f);
            state.UpdateTime(5f);
            Near(group.alpha, 0.5f);
        }

        [TestCase(0f, 0f)]
        [TestCase(0.5f, 0.25f)]
        [TestCase(1f, 0.5f)]
        [TestCase(2f, 1f)]
        [TestCase(20f, 1f)]
        public void ChangeState_InterpolatesAndAppliesExactFinalValue(float elapsed, float expected)
        {
            state.ChangeState("Hover");
            Assert.That(state.CurrentState, Is.EqualTo("Hover"));
            state.UpdateTime(elapsed);
            Near(group.alpha, expected);
        }

        [Test]
        public void CompletedAnimation_DoesNotWriteAgain()
        {
            state.ChangeState("Hover");
            state.UpdateTime(2f);
            group.alpha = 0.2f;
            state.UpdateTime(1f);
            Near(group.alpha, 0.2f);
        }

        [Test]
        public void InterruptedAnimation_UsesActualCurrentValueAsSnapshot()
        {
            state.ChangeState("Hover");
            state.UpdateTime(1f);
            Near(group.alpha, 0.5f);
            state.ChangeState("Idle");
            state.UpdateTime(1f);
            Near(group.alpha, 0.25f);
        }

        [Test]
        public void RepeatedAnimatedRequest_DoesNotRestartAnimation()
        {
            state.ChangeState("Hover");
            state.UpdateTime(1f);
            state.ChangeState("Hover");
            state.UpdateTime(1f);
            Near(group.alpha, 1f);
        }

        [TestCase(0f)]
        [TestCase(-1f)]
        public void NonPositiveDuration_AppliesImmediately(float duration)
        {
            Animations(state, Animation(duration));
            state.ChangeState("Hover");
            Near(group.alpha, 1f);
        }

        [Test]
        public void MissingAnimationRule_AppliesImmediately()
        {
            Animations(state, Animation(10f, 1, 2));
            state.ChangeState("Hover");
            Near(group.alpha, 1f);
        }

        [Test]
        public void ExactAnimationRule_TakesPriorityOverEarlierWildcard()
        {
            Animations(state, Animation(10f), Animation(2f, 0, 1));
            state.ChangeState("Hover");
            state.UpdateTime(1f);
            Near(group.alpha, 0.5f);
        }

        [TestCase(-1, 1)]
        [TestCase(0, -1)]
        [TestCase(-1, -1)]
        public void WildcardRule_MatchesExpectedTransition(int from, int to)
        {
            Animations(state, Animation(2f, from, to));
            state.ChangeState("Hover");
            state.UpdateTime(1f);
            Near(group.alpha, 0.5f);
        }

        [Test]
        public void CompetingWildcardRules_FirstMatchingRuleWins()
        {
            Animations(state, Animation(4f), Animation(2f, -1, 1));
            state.ChangeState("Hover");
            state.UpdateTime(1f);
            Near(group.alpha, 0.25f);
        }

        [Test]
        public void EasingCurve_TransformsInterpolationProgress()
        {
            Animations(state, Animation(2f, easing: AnimationCurve.Linear(0f, 0f, 1f, 0.5f)));
            state.ChangeState("Hover");
            state.UpdateTime(1f);
            Near(group.alpha, 0.25f);
            state.UpdateTime(1f);
            Near(group.alpha, 1f);
        }

        [Test]
        public void Storage_DrivesTargetValueDurationAndEasing()
        {
            var storage = new VarioStorage();
            storage.Update("duration", 4f);
            storage.Update("value", 0.8f);
            storage.Update("curve", AnimationCurve.Linear(0f, 0f, 1f, 0.5f));
            SetField(state, "storage", storage);
            parameter.SetValue(1, VarioValue<float>.FromStorage("value"));
            Animations(state, new StatioAnimation
            {
                InitialStateId = -1, TargetStateId = -1,
                Duration = VarioValue<float>.FromStorage("duration"),
                Easing = VarioValue<AnimationCurve>.FromStorage("curve")
            });
            state.ChangeState("Hover");
            Assert.That(((IStatioParameterLifecycle)parameter).Storage, Is.SameAs(storage));
            state.UpdateTime(2f);
            Near(group.alpha, 0.2f);
            storage.Update("value", 0.6f);
            state.UpdateTime(2f);
            Near(group.alpha, 0.6f);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void DisabledComponent_DefersLatestRequestUntilEnabled(bool animate)
        {
            state.enabled = false;
            state.ChangeState(2, false);
            state.ChangeState(1, animate);
            Assert.That(state.CurrentState, Is.EqualTo("Idle"));
            Near(group.alpha, 0f);
            Activate(state);
            Assert.That(state.CurrentState, Is.EqualTo("Hover"));
            if (animate) state.UpdateTime(1f);
            Near(group.alpha, animate ? 0.5f : 1f);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ModifyingStateOrderDuringAnimation_PreservesDestination(bool remove)
        {
            state.ChangeState("Hover");
            state.UpdateTime(0.5f);
            if (remove) state.RemoveState("Idle");
            else state.ChangeStateId("Hover", 2);
            state.UpdateTime(1.5f);
            Assert.That(state.CurrentState, Is.EqualTo("Hover"));
            Near(group.alpha, 1f);
        }

        [Test]
        public void NullParameter_IsSkippedDuringAllTransitionOperations()
        {
            state.AddParameter<CanvasGroup, float>(null);
            Assert.DoesNotThrow(() =>
            {
                state.ChangeState("Hover");
                state.UpdateTime(1f);
                state.UpdateTime(1f);
                state.ChangeStateImmediately("Pressed");
                state.ChangeStateId("Idle", 2);
                state.RemoveState("Idle");
            });
            Assert.That(Errors, Is.Empty);
        }

        private sealed class ThrowingParameter : StatioParameter<CanvasGroup, float>
        {
            protected override float GetValue(CanvasGroup target) => throw new InvalidOperationException("snapshot failure");
            protected override void SetValue(CanvasGroup target, float value) => throw new InvalidOperationException("write failure");
        }

        [TestCase(false)]
        [TestCase(true)]
        public void FailingParameter_ReportsErrorAndDoesNotPreventOtherParameters(bool animate)
        {
            var fresh = NewState("Idle", "Hover");
            fresh.AddParameter(new ThrowingParameter { Target = NewObject().AddComponent<CanvasGroup>() });
            var healthy = Alpha(fresh, 0f, 1f);
            healthy.Target.Value.alpha = 0f;
            Animations(fresh, Animation(2f));
            fresh.ChangeState(1, animate);
            if (animate)
            {
                fresh.UpdateTime(1f);
                Near(healthy.Target.Value.alpha, 0.5f);
                fresh.UpdateTime(1f);
            }
            Near(healthy.Target.Value.alpha, 1f);
            Assert.That(Errors.Count, Is.EqualTo(animate ? 3 : 1));
        }

        [Test]
        public void UnknownStateName_DoesNotChangeValuesOrInterruptAnimation()
        {
            state.ChangeState("Hover");
            state.ChangeStateImmediately("Missing");
            state.UpdateTime(1f);
            Near(group.alpha, 0.5f);
            Assert.That(Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public void Timescale_RoundTripsBothModes()
        {
            foreach (StatioTimescale timescale in Enum.GetValues(typeof(StatioTimescale)))
            {
                state.Timescale = timescale;
                Assert.That(state.Timescale, Is.EqualTo(timescale));
            }
        }
    }
}
