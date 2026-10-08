using System;
using Damdor.Vario;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Damdor.Statio.Tests
{
    public class BuiltInParameterTests : StatioTestSupport
    {
        [TestCase(false)]
        [TestCase(true)]
        public void Position_ReadsAndWritesConfiguredCoordinateSpace(bool local)
        {
            var parent = NewObject().transform;
            parent.position = new Vector3(10f, 20f, 30f);
            var child = NewObject().transform;
            child.SetParent(parent, false);
            child.localPosition = Vector3.one;
            var parameter = new PositionStatioParameter { Target = child };
            SetField(parameter, "local", VarioValue<bool>.Raw(local));
            var lifecycle = (IStatioParameterLifecycle)parameter;
            lifecycle.SaveCurrentValueToDefaultValue();
            Assert.That(parameter.DefaultValue.Value, Is.EqualTo(local ? Vector3.one : parent.position + Vector3.one));
            parameter.SetValue(0, new Vector3(2f, 3f, 4f));
            lifecycle.LoadValue(0);
            Assert.That(local ? child.localPosition : child.position, Is.EqualTo(new Vector3(2f, 3f, 4f)));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void EulerAngles_ReadsAndWritesConfiguredCoordinateSpace(bool local)
        {
            var parent = NewObject().transform;
            parent.rotation = Quaternion.Euler(0f, 30f, 0f);
            var child = NewObject().transform;
            child.SetParent(parent, false);
            child.localRotation = Quaternion.Euler(0f, 20f, 0f);
            var parameter = new EulerAnglesStatioParameter { Target = child };
            SetField(parameter, "local", VarioValue<bool>.Raw(local));
            var lifecycle = (IStatioParameterLifecycle)parameter;
            lifecycle.SaveCurrentValueToDefaultValue();
            Near(parameter.DefaultValue.Value.y, local ? 20f : 50f);
            parameter.SetValue(0, new Vector3(0f, 60f, 0f));
            lifecycle.LoadValue(0);
            Near(Quaternion.Angle(local ? child.localRotation : child.rotation, Quaternion.Euler(0f, 60f, 0f)), 0f);
        }

        [Test]
        public void Position_LocalFlagCanBeResolvedFromStorage()
        {
            var parent = NewObject().transform;
            parent.position = Vector3.one * 10f;
            var child = NewObject().transform;
            child.SetParent(parent, false);
            var parameter = new PositionStatioParameter { Target = child, DefaultValue = Vector3.one };
            SetField(parameter, "local", VarioValue<bool>.FromStorage("local"));
            var lifecycle = (IStatioParameterLifecycle)parameter;
            lifecycle.Storage = new VarioStorage();
            lifecycle.Storage.Update("local", false);
            lifecycle.LoadDefaultValue();
            Assert.That(child.position, Is.EqualTo(Vector3.one));
            lifecycle.Storage.Update("local", true);
            lifecycle.LoadDefaultValue();
            Assert.That(child.localPosition, Is.EqualTo(Vector3.one));
        }

        [Test]
        public void Scale_CapturesAndInterpolatesLocalScale()
        {
            var transform = NewObject().transform;
            transform.localScale = Vector3.one * 2f;
            var parameter = new ScaleStatioParameter { Target = transform, DefaultValue = Vector3.one * 4f };
            var lifecycle = (IStatioParameterLifecycle)parameter;
            lifecycle.SaveSnapshot();
            lifecycle.LoadValue(0, 0.5f);
            Assert.That(transform.localScale, Is.EqualTo(Vector3.one * 3f));
            lifecycle.SaveCurrentValueToState(1);
            Assert.That(parameter.GetValue(1).Value, Is.EqualTo(Vector3.one * 3f));
        }

        [Test]
        public void GraphicColor_CapturesAndInterpolatesAllChannels()
        {
            var graphic = NewObject(typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
            graphic.color = Color.black;
            var parameter = new GraphicColorStatioParameter { Target = graphic, DefaultValue = Color.white };
            var lifecycle = (IStatioParameterLifecycle)parameter;
            lifecycle.SaveSnapshot();
            lifecycle.LoadValue(0, 0.5f);
            Assert.That(graphic.color, Is.EqualTo(new Color(0.5f, 0.5f, 0.5f, 1f)));
            lifecycle.SaveCurrentValueToDefaultValue();
            Assert.That(parameter.DefaultValue.Value, Is.EqualTo(graphic.color));
        }

        [TestCase(typeof(RectTransformAnchorMinStatioParameter), "anchorMin")]
        [TestCase(typeof(RectTransformAnchorMaxStatioParameter), "anchorMax")]
        [TestCase(typeof(RectTransformAnchoredPositionStatioParameter), "anchoredPosition")]
        [TestCase(typeof(RectTransformPivotStatioParameter), "pivot")]
        public void RectTransformVectorParameter_CapturesAndInterpolatesCorrectProperty(Type type, string property)
        {
            var rect = NewObject(typeof(RectTransform)).GetComponent<RectTransform>();
            var unityProperty = typeof(RectTransform).GetProperty(property);
            unityProperty.SetValue(rect, new Vector2(0.2f, 0.4f));
            var parameter = (StatioParameter<RectTransform, Vector2>)Activator.CreateInstance(type);
            parameter.Target = rect;
            parameter.DefaultValue = new Vector2(0.6f, 0.8f);
            var lifecycle = (IStatioParameterLifecycle)parameter;
            lifecycle.SaveSnapshot();
            lifecycle.LoadValue(0, 0.5f);
            var actual = (Vector2)unityProperty.GetValue(rect);
            Near(actual.x, 0.4f);
            Near(actual.y, 0.6f);
            lifecycle.SaveCurrentValueToState(1);
            Assert.That(parameter.GetValue(1).Value, Is.EqualTo(actual));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void RectTransformSize_WorksWithStretchedAnchors(bool height)
        {
            var parent = NewObject(typeof(RectTransform)).GetComponent<RectTransform>();
            parent.sizeDelta = new Vector2(400f, 300f);
            var rect = NewObject(typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            StatioParameter<RectTransform, float> parameter = height
                ? new RectTransformHeightStatioParameter() : new RectTransformWidthStatioParameter();
            parameter.Target = rect;
            parameter.DefaultValue = 80f;
            var lifecycle = (IStatioParameterLifecycle)parameter;
            lifecycle.LoadDefaultValue();
            Near(height ? rect.rect.height : rect.rect.width, 80f);
            lifecycle.SaveCurrentValueToState(1);
            Near(parameter.GetValue(1).Value, 80f);
        }

        [TestCase(false, false, 0f, false)]
        [TestCase(false, true, 0f, true)]
        [TestCase(true, false, 0.5f, true)]
        [TestCase(true, false, 0.98f, true)]
        [TestCase(true, false, 0.99f, false)]
        [TestCase(true, false, 1f, false)]
        [TestCase(false, true, 1f, true)]
        public void Activity_KeepsObjectActiveUntilDeactivationThreshold(bool initial, bool target, float t, bool expected)
        {
            var obj = NewObject();
            obj.SetActive(initial);
            var parameter = new GameObjectActivityStatioParameter { Target = obj, DefaultValue = target };
            var lifecycle = (IStatioParameterLifecycle)parameter;
            lifecycle.SaveSnapshot();
            lifecycle.LoadValue(0, t);
            Assert.That(obj.activeSelf, Is.EqualTo(expected));
        }

        [Test]
        public void TransformParent_ImmediateLoadPreservesLocalTransformAndMovesToLastSibling()
        {
            var parent = NewObject().transform;
            NewObject().transform.SetParent(parent, false);
            var child = NewObject().transform;
            child.localPosition = new Vector3(2f, 3f, 4f);
            var parameter = new TransformParentStatioParameter { Target = child, DefaultValue = parent };
            ((IStatioParameterLifecycle)parameter).LoadDefaultValue();
            Assert.That(child.parent, Is.SameAs(parent));
            Assert.That(child.localPosition, Is.EqualTo(new Vector3(2f, 3f, 4f)));
            Assert.That(child.GetSiblingIndex(), Is.EqualTo(parent.childCount - 1));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void NestedState_RespectsAnimationFlag(bool animate)
        {
            var state = NewState("Idle", "Hover");
            var alpha = Alpha(state, 0f, 1f);
            state.ChangeStateImmediately("Idle");
            Animations(state, Animation(2f));
            var parameter = new StatioStatioParameter { Target = state, DefaultValue = "Hover" };
            SetField(parameter, "animate", VarioValue<bool>.Raw(animate));
            ((IStatioParameterLifecycle)parameter).LoadDefaultValue();
            Near(alpha.Target.Value.alpha, animate ? 0f : 1f);
            if (animate)
            {
                state.UpdateTime(1f);
                Near(alpha.Target.Value.alpha, 0.5f);
            }
        }
    }
}
