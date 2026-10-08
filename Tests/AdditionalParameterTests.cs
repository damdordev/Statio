using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Damdor.Statio.Tests
{
    public class AdditionalParameterTests : StatioTestSupport
    {
        [Test]
        public void GraphicAlpha_InterpolatesWithoutChangingCurrentRgb()
        {
            var image = NewObject(typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            image.color = new Color(0.2f, 0.3f, 0.4f, 0.2f);
            var parameter = new GraphicAlphaStatioParameter { Target = image, DefaultValue = 0.8f };
            var lifecycle = (IStatioParameterLifecycle)parameter;
            lifecycle.SaveSnapshot();
            image.color = new Color(0.6f, 0.7f, 0.8f, 0.2f);
            lifecycle.LoadValue(0, 0.5f);
            Assert.That(image.color, Is.EqualTo(new Color(0.6f, 0.7f, 0.8f, 0.5f)));
            lifecycle.SaveCurrentValueToState(1);
            Near(parameter.GetValue(1).Value, 0.5f);
        }

        [TestCase(typeof(CanvasGroupInteractableStatioParameter), "interactable")]
        [TestCase(typeof(CanvasGroupBlocksRaycastsStatioParameter), "blocksRaycasts")]
        public void CanvasGroupFlags_SwitchAtMidpointWithoutChangingAlpha(Type type, string property)
        {
            var group = NewObject().AddComponent<CanvasGroup>();
            group.alpha = 0.3f;
            var unityProperty = typeof(CanvasGroup).GetProperty(property);
            unityProperty.SetValue(group, true);
            var parameter = (StatioParameter<CanvasGroup, bool>)Activator.CreateInstance(type);
            parameter.Target = group;
            parameter.DefaultValue = false;
            var lifecycle = (IStatioParameterLifecycle)parameter;
            lifecycle.SaveSnapshot();
            lifecycle.LoadValue(0, 0.49f);
            Assert.That(unityProperty.GetValue(group), Is.EqualTo(true));
            lifecycle.LoadValue(0, 0.5f);
            Assert.That(unityProperty.GetValue(group), Is.EqualTo(false));
            Near(group.alpha, 0.3f);
            lifecycle.SaveCurrentValueToState(1);
            Assert.That(parameter.GetValue(1).Value, Is.False);
        }

        [Test]
        public void BehaviourEnabled_DisablesComponentWithoutDeactivatingObject()
        {
            var group = NewObject().AddComponent<CanvasGroup>();
            var parameter = new BehaviourEnabledStatioParameter { Target = group, DefaultValue = false };
            ((IStatioParameterLifecycle)parameter).LoadDefaultValue();
            Assert.That(group.enabled, Is.False);
            Assert.That(group.gameObject.activeSelf, Is.True);
        }

        [Test]
        public void GraphicRaycastTarget_ChangesHitTestingWithoutChangingColor()
        {
            var image = NewObject(typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            image.color = Color.red;
            var parameter = new GraphicRaycastTargetStatioParameter { Target = image, DefaultValue = false };
            ((IStatioParameterLifecycle)parameter).LoadDefaultValue();
            Assert.That(image.raycastTarget, Is.False);
            Assert.That(image.color, Is.EqualTo(Color.red));
        }

        [Test]
        public void ImageFillAmount_InterpolatesAndUsesUnityClamping()
        {
            var image = NewObject(typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            image.type = Image.Type.Filled;
            image.fillAmount = 0.2f;
            var parameter = new ImageFillAmountStatioParameter { Target = image, DefaultValue = 0.8f };
            var lifecycle = (IStatioParameterLifecycle)parameter;
            lifecycle.SaveSnapshot();
            lifecycle.LoadValue(0, 0.5f);
            Near(image.fillAmount, 0.5f);
            lifecycle.LoadValue(0, 2f);
            Near(image.fillAmount, 1f);
        }

        [Test]
        public void SizeDelta_InterpolatesOffsetsWithStretchedAnchors()
        {
            var parent = NewObject(typeof(RectTransform)).GetComponent<RectTransform>();
            parent.sizeDelta = new Vector2(400f, 300f);
            var rect = NewObject(typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = new Vector2(-100f, -60f);
            var parameter = new RectTransformSizeDeltaStatioParameter { Target = rect, DefaultValue = Vector2.zero };
            var lifecycle = (IStatioParameterLifecycle)parameter;
            lifecycle.SaveSnapshot();
            lifecycle.LoadValue(0, 0.5f);
            Assert.That(rect.sizeDelta, Is.EqualTo(new Vector2(-50f, -30f)));
            Near(rect.rect.width, 350f);
            Near(rect.rect.height, 270f);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PreferredSize_ChangesLayoutInputAndCanRestoreUnsetValue(bool height)
        {
            var layout = NewObject(typeof(RectTransform), typeof(LayoutElement)).GetComponent<LayoutElement>();
            StatioParameter<LayoutElement, float> parameter = height
                ? new LayoutElementPreferredHeightStatioParameter() : new LayoutElementPreferredWidthStatioParameter();
            parameter.Target = layout;
            var lifecycle = (IStatioParameterLifecycle)parameter;
            lifecycle.SaveCurrentValueToDefaultValue();
            Near(parameter.DefaultValue.Value, -1f);
            parameter.SetValue(0, 80f);
            lifecycle.LoadValue(0);
            Near(height ? layout.preferredHeight : layout.preferredWidth, 80f);
            lifecycle.LoadDefaultValue();
            Near(height ? layout.preferredHeight : layout.preferredWidth, -1f);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Sprite_SwitchesAtMidpointAndSupportsNull(bool renderer)
        {
            var texture = new Texture2D(2, 2);
            var sprite = Sprite.Create(texture, new Rect(0, 0, 2, 2), Vector2.one * 0.5f);
            try
            {
                var image = renderer ? null : NewObject(typeof(RectTransform), typeof(Image)).GetComponent<Image>();
                var sr = renderer ? NewObject().AddComponent<SpriteRenderer>() : null;
                StatioParameter parameter;
                if (renderer)
                {
                    sr.sprite = sprite;
                    parameter = new SpriteRendererSpriteStatioParameter { Target = sr, DefaultValue = (Sprite)null };
                }
                else
                {
                    image.sprite = sprite;
                    parameter = new ImageSpriteStatioParameter { Target = image, DefaultValue = (Sprite)null };
                }
                var lifecycle = (IStatioParameterLifecycle)parameter;
                lifecycle.SaveCurrentValueToState(1);
                lifecycle.SaveSnapshot();
                lifecycle.LoadValue(0, 0.49f);
                Assert.That(renderer ? sr.sprite : image.sprite, Is.SameAs(sprite));
                lifecycle.LoadValue(0, 0.5f);
                Assert.That(renderer ? sr.sprite : image.sprite, Is.Null);
                lifecycle.LoadValue(1);
                Assert.That(renderer ? sr.sprite : image.sprite, Is.SameAs(sprite));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(sprite);
                UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        [Test]
        public void SpriteRendererColor_InterpolatesAlphaAlongsideRgb()
        {
            var renderer = NewObject().AddComponent<SpriteRenderer>();
            renderer.color = new Color(0f, 0f, 0f, 0f);
            var parameter = new SpriteRendererColorStatioParameter { Target = renderer, DefaultValue = Color.white };
            var lifecycle = (IStatioParameterLifecycle)parameter;
            lifecycle.SaveSnapshot();
            lifecycle.LoadValue(0, 0.5f);
            Assert.That(renderer.color, Is.EqualTo(new Color(0.5f, 0.5f, 0.5f, 0.5f)));
        }
    }
}
