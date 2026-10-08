using Damdor.Vario;
using NUnit.Framework;
using UnityEngine;

namespace Damdor.Statio.Tests
{
    public class VisualStateRegressionTests : StatioTestSupport
    {
        [Test]
        public void ImmediateRequestForCurrentDestination_CompletesAnimation()
        {
            var state = NewState("Idle", "Hover");
            var parameter = Alpha(state, 0f, 1f);
            state.ChangeStateImmediately("Idle");
            Animations(state, Animation(2f));
            state.ChangeState("Hover");
            state.UpdateTime(0.5f);
            state.ChangeStateImmediately("Hover");
            Near(parameter.Target.Value.alpha, 1f);
            parameter.Target.Value.alpha = 0.3f;
            state.UpdateTime(5f);
            Near(parameter.Target.Value.alpha, 0.3f);
        }

        [TestCase(-2)]
        [TestCase(2)]
        [TestCase(100)]
        public void InvalidStateIndex_IsRejectedWithoutChangingCurrentState(int index)
        {
            var state = NewState("Idle", "Hover");
            var parameter = Alpha(state, 0.2f, 0.8f);
            state.ChangeStateImmediately("Idle");
            state.ChangeState(index, false);
            Assert.That(state.CurrentState, Is.EqualTo("Idle"));
            Near(parameter.Target.Value.alpha, 0.2f);
            Assert.That(Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public void UnknownAnimatedState_ReportsOneError()
        {
            var state = NewState("Idle");
            state.ChangeState("Missing");
            Assert.That(Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public void RemovingRuleEndpoint_DoesNotRetargetRuleToAnotherStateOrWildcard()
        {
            var state = NewState("Idle", "Removed", "Hover");
            var parameter = Alpha(state, 0f, 0.2f, 1f);
            state.ChangeStateImmediately("Idle");
            state.RemoveState("Removed");
            state.ChangeStateImmediately("Hover");
            Near(parameter.Target.Value.alpha, 1f);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ChangingOrderWhileDisabled_PreservesQueuedStateIdentity(bool remove)
        {
            var state = NewState("Idle", "Hover", "Pressed");
            var parameter = Alpha(state, 0f, 1f, 0.5f);
            state.ChangeStateImmediately("Idle");
            state.enabled = false;
            state.ChangeStateImmediately("Hover");
            if (remove) state.RemoveState("Idle");
            else state.ChangeStateId("Hover", 2);
            Activate(state);
            Assert.That(state.CurrentState, Is.EqualTo("Hover"));
            Near(parameter.Target.Value.alpha, 1f);
        }

        [Test]
        public void RemovingQueuedState_DoesNotSelectFollowingStateOnEnable()
        {
            var state = NewState("Idle", "Removed", "Hover");
            Alpha(state, 0f, 0.2f, 1f);
            state.ChangeStateImmediately("Idle");
            state.enabled = false;
            state.ChangeStateImmediately("Removed");
            state.RemoveState("Removed");
            Activate(state);
            Assert.That(state.CurrentState, Is.EqualTo("Idle"));
        }

        [TestCase(false)]
        [TestCase(true)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void DiscreteParameter_UsesSwitchMomentAndMovesFromSnapshotToTarget(bool parent, bool stored = false)
        {
            var storage = new VarioStorage();
            storage.Update("switch", 0.75f);
            var moment = stored ? VarioValue<float>.FromStorage("switch") : VarioValue<float>.Raw(0.75f);
            IStatioParameterLifecycle lifecycle;
            System.Func<object> read;
            object initial;
            object target;
            if (parent)
            {
                var a = NewObject().transform;
                var b = NewObject().transform;
                var child = NewObject().transform;
                child.SetParent(a);
                var parameter = new TransformParentStatioParameter { Target = child, DefaultValue = b, SwitchMoment = moment };
                lifecycle = parameter;
                read = () => child.parent;
                initial = a;
                target = b;
            }
            else
            {
                var state = NewState("Idle", "Hover");
                state.ChangeStateImmediately("Idle");
                var parameter = new StatioStatioParameter { Target = state, DefaultValue = "Hover", SwitchMoment = moment };
                lifecycle = parameter;
                read = () => state.CurrentState;
                initial = "Idle";
                target = "Hover";
            }
            lifecycle.Storage = storage;
            lifecycle.SaveSnapshot();
            lifecycle.LoadValue(0, 0f);
            Assert.That(read(), Is.EqualTo(initial), "t=0 must preserve the snapshot");
            lifecycle.LoadValue(0, 0.6f);
            Assert.That(read(), Is.EqualTo(initial), "Do not switch before configured moment");
            lifecycle.LoadValue(0, 0.75f);
            Assert.That(read(), Is.EqualTo(target), "Switch at configured moment");
            lifecycle.LoadValue(0, 1f);
            Assert.That(read(), Is.EqualTo(target), "Do not return to the snapshot");
        }
    }
}
