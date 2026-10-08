using System;
using System.Collections.Generic;
using System.Reflection;
using Damdor.Vario;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Damdor.Statio.Tests
{
    // Reflection is limited to serialized configuration that has no public setter.
    // Behaviour is verified through the public API and real Unity components.
    public abstract class StatioTestSupport
    {
        private readonly List<GameObject> objects = new();
        protected readonly List<string> Errors = new();
        private bool previousLogging;
        private StatioErrorHandler previousHandler;
        private Dictionary<Type, INumericOperations> previousAlgorithms;

        [SetUp]
        public void PrepareEnvironment()
        {
            Errors.Clear();
            previousLogging = StatioSettings.LogErrorsToConsole;
            previousHandler = ReadStatic<StatioErrorHandler>(typeof(StatioSettings), "errorHandler");
            StatioSettings.LogErrorsToConsole = false;
            StatioSettings.RegisterErrorHandler((_, error) => Errors.Add(error));
            var algorithms = ReadStatic<Dictionary<Type, INumericOperations>>(typeof(VarioSettings), "algorithms");
            previousAlgorithms = new Dictionary<Type, INumericOperations>(algorithms);
            VarioSettings.RegisterNumericOperations(new FloatNumericOperations());
            VarioSettings.RegisterNumericOperations(new Vector2NumericOperations());
            VarioSettings.RegisterNumericOperations(new Vector3NumericOperations());
            VarioSettings.RegisterNumericOperations(new ColorNumericOperations());
            VarioSettings.RegisterNumericOperations(new BoolNumericOperations());
        }

        [TearDown]
        public void RestoreEnvironment()
        {
            foreach (var obj in objects)
                if (obj != null) Object.DestroyImmediate(obj);
            objects.Clear();
            StatioSettings.LogErrorsToConsole = previousLogging;
            StatioSettings.RegisterErrorHandler(previousHandler);
            var algorithms = ReadStatic<Dictionary<Type, INumericOperations>>(typeof(VarioSettings), "algorithms");
            algorithms.Clear();
            foreach (var entry in previousAlgorithms) algorithms.Add(entry.Key, entry.Value);
        }

        protected GameObject NewObject(params Type[] components)
        {
            var obj = new GameObject("Statio test", components);
            objects.Add(obj);
            return obj;
        }

        protected VisualState NewState(params string[] states)
        {
            var state = NewObject().AddComponent<VisualState>();
            foreach (var name in states) state.AddState(name);
            return state;
        }

        protected CanvasGroupAlphaStatioParameter Alpha(VisualState state, params float[] values)
        {
            var parameter = new CanvasGroupAlphaStatioParameter
            {
                Target = state.gameObject.AddComponent<CanvasGroup>(), DefaultValue = 0f
            };
            for (var i = 0; i < values.Length; i++) parameter.SetValue(i, values[i]);
            state.AddParameter(parameter);
            return parameter;
        }

        protected static void Animations(VisualState state, params StatioAnimation[] animations)
            => SetField(state, "animations", new List<StatioAnimation>(animations));

        protected static StatioAnimation Animation(float duration, int from = -1, int to = -1,
            AnimationCurve easing = null) => new()
        {
            InitialStateId = from, TargetStateId = to, Duration = duration, Easing = easing
        };

        protected static void SetField(object target, string name, object value)
        {
            var field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Missing configuration field {name}");
            field.SetValue(target, value);
        }

        protected static T ReadStatic<T>(Type type, string name)
            => (T)type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

        protected static void Lifecycle(VisualState state, string method)
            => typeof(VisualState).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(state, null);

        protected static void Activate(VisualState state)
        {
            state.enabled = true;
            // EditMode tests do not depend on the editor delivering MonoBehaviour callbacks.
            Lifecycle(state, "OnEnable");
        }

        protected static void Near(float actual, float expected)
            => Assert.That(actual, Is.EqualTo(expected).Within(0.0001f));
    }
}
