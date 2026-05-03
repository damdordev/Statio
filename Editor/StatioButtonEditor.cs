using UnityEditor;
using UnityEditor.UI;

namespace Damdor.Statio.Editor
{
    [CustomEditor(typeof(StatioButton), true)]
    [CanEditMultipleObjects]
    public class StatioButtonEditor : ButtonEditor
    {
        private SerializedProperty visualStateProperty;
        private SerializedProperty interactableProperty;
        private SerializedProperty navigationProperty;
        private SerializedProperty onClickProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            visualStateProperty = serializedObject.FindProperty("visualState");
            interactableProperty = serializedObject.FindProperty("m_Interactable");
            navigationProperty = serializedObject.FindProperty("m_Navigation");
            onClickProperty = serializedObject.FindProperty("m_OnClick");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(interactableProperty);
            EditorGUILayout.PropertyField(visualStateProperty, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(navigationProperty);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(onClickProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}