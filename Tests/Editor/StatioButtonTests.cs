using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Damdor.Statio.Tests
{
    public class StatioButtonTestDriver : StatioButton
    {
        public void Transition(string selection, bool instant)
        {
            var state = (SelectionState)System.Enum.Parse(typeof(SelectionState), selection);
            DoStateTransition(state, instant);
        }
    }

    public class StatioButtonTests : StatioTestSupport
    {
        private VisualState state;
        private StatioButtonTestDriver button;
        private StatioButtonStates mapping;

        [SetUp]
        public void CreateButton()
        {
            state = NewState("normal", "pressed", "disabled");
            Alpha(state, 0f, 0.5f, 1f);
            button = NewObject(typeof(RectTransform)).AddComponent<StatioButtonTestDriver>();
            button.transition = Selectable.Transition.None;
            mapping = new StatioButtonStates { visualState = state };
            typeof(StatioButton).GetField("visualState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(button, mapping);
        }

        [TestCase("Normal", "normal")]
        [TestCase("Highlighted", "normal")]
        [TestCase("Selected", "normal")]
        [TestCase("Pressed", "pressed")]
        [TestCase("Disabled", "disabled")]
        public void SelectionState_MapsToConfiguredVisualState(string selection, string expected)
        {
            button.Transition(selection, true);
            Assert.That(state.CurrentState, Is.EqualTo(expected));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void InstantFlag_ControlsWhetherVisualTransitionIsAnimated(bool instant)
        {
            state.ChangeStateImmediately("normal");
            Animations(state, Animation(2f));
            button.Transition("Disabled", instant);
            Near(state.GetComponent<CanvasGroup>().alpha, instant ? 1f : 0f);
            if (!instant)
            {
                state.UpdateTime(1f);
                Near(state.GetComponent<CanvasGroup>().alpha, 0.5f);
            }
        }

        [Test]
        public void EmptyMapping_DoesNotChangeStateOrReportError()
        {
            state.ChangeStateImmediately("normal");
            mapping.PressedState = "";
            button.Transition("Pressed", true);
            Assert.That(state.CurrentState, Is.EqualTo("normal"));
            Assert.That(Errors, Is.Empty);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void MissingMappingOrVisualState_DoesNotThrow(bool missingMapping)
        {
            if (missingMapping)
                typeof(StatioButton).GetField("visualState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    .SetValue(button, null);
            else mapping.visualState = null;
            Assert.DoesNotThrow(() => button.Transition("Pressed", true));
        }

        [Test]
        public void Mapping_UsesCustomStateNames()
        {
            state.AddState("Custom");
            mapping.PressedState = "Custom";
            button.Transition("Pressed", true);
            Assert.That(state.CurrentState, Is.EqualTo("Custom"));
        }
    }
}
