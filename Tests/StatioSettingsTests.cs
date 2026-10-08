using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Damdor.Statio.Tests
{
    public class StatioSettingsTests : StatioTestSupport
    {
        private List<Type> types;
        private Dictionary<Type, string> names;

        [SetUp]
        public void CaptureRegistry()
        {
            types = new List<Type>(StatioSettings.SupportedParameterTypes);
            names = new Dictionary<Type, string>(ReadStatic<Dictionary<Type, string>>(typeof(StatioSettings), "parameterTypeToName"));
        }

        [TearDown]
        public void RestoreRegistry()
        {
            var actualTypes = ReadStatic<List<Type>>(typeof(StatioSettings), "supportedParameterTypes");
            actualTypes.Clear();
            actualTypes.AddRange(types);
            var actualNames = ReadStatic<Dictionary<Type, string>>(typeof(StatioSettings), "parameterTypeToName");
            actualNames.Clear();
            foreach (var entry in names) actualNames.Add(entry.Key, entry.Value);
        }

        public static IEnumerable<Type> BuiltInTypes => typeof(VisualState).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(StatioParameter).IsAssignableFrom(type)
                && Attribute.IsDefined(type, typeof(StatioParameterAttribute)));

        [TestCaseSource(nameof(BuiltInTypes))]
        public void GeneratedRegistration_ExposesEveryBuiltInParameterWithAttributeName(Type type)
        {
            var attribute = (StatioParameterAttribute)Attribute.GetCustomAttribute(type, typeof(StatioParameterAttribute));
            Assert.That(StatioSettings.SupportedParameterTypes, Does.Contain(type));
            Assert.That(StatioSettings.GetParameterTypeName(type), Is.EqualTo(attribute.Name));
        }

        [TestCaseSource(nameof(BuiltInTypes))]
        public void BuiltInParameter_CanBeCreatedByInspectorAndHasLifecycle(Type type)
        {
            Assert.That(type.IsSerializable, Is.True);
            Assert.That(Activator.CreateInstance(type), Is.InstanceOf<IStatioParameterLifecycle>());
        }

        [Test]
        public void RegisterParameterType_RegistersMenuName()
        {
            StatioSettings.RegisterParameterType(typeof(CanvasGroupAlphaStatioParameter), "Test/Alpha");
            Assert.That(StatioSettings.SupportedParameterTypes, Does.Contain(typeof(CanvasGroupAlphaStatioParameter)));
            Assert.That(StatioSettings.GetParameterTypeName(typeof(CanvasGroupAlphaStatioParameter)), Is.EqualTo("Test/Alpha"));
        }

        [Test]
        public void RegisterParameterType_RepeatedRegistrationReplacesNameWithoutDuplicates()
        {
            var type = typeof(CanvasGroupAlphaStatioParameter);
            StatioSettings.RegisterParameterType(type, "First");
            StatioSettings.RegisterParameterType(type, "Second");
            StatioSettings.RegisterParameterType(type, "Second");
            Assert.That(StatioSettings.SupportedParameterTypes.Count(t => t == type), Is.EqualTo(1));
            Assert.That(StatioSettings.GetParameterTypeName(type), Is.EqualTo("Second"));
        }

        [Test]
        public void UnknownParameterType_ReturnsEmptyMenuName()
            => Assert.That(StatioSettings.GetParameterTypeName(typeof(StatioSettingsTests)), Is.Empty);

        [Test]
        public void ErrorHandler_ReceivesContextAndCanBeUnregistered()
        {
            var state = NewState("Idle");
            VisualState context = null;
            string message = null;
            StatioSettings.RegisterErrorHandler((source, error) => { context = source; message = error; });
            state.AddState("Idle");
            Assert.That(context, Is.SameAs(state));
            Assert.That(message, Does.Contain("Idle"));
            StatioSettings.RegisterErrorHandler(null);
            Assert.DoesNotThrow(() => state.AddState("Idle"));
        }

        [Test]
        public void ConsoleLogging_CanBeEnabledAlongsideErrorHandler()
        {
            var state = NewState("Idle");
            StatioSettings.LogErrorsToConsole = true;
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("Visual state error.*Trying to add existing state: Idle"));
            state.AddState("Idle");
            Assert.That(Errors.Count, Is.EqualTo(1));
        }
    }
}
