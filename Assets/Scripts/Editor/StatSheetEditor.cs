using JunkMage.Stats;

namespace JunkMage.Editor
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(StatSheet))]
    public class StatSheetEditor : Editor
    {
        private SerializedProperty valuesProp;

        private void OnEnable()
        {
            valuesProp = serializedObject.FindProperty("values");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var sheet = (StatSheet)target;
            if (sheet == null)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.HelpBox(
                "StatSheet: use StatDefinition-backed entries (preferred). Legacy 'entries' UI has been removed —\n" +
                "If you still have legacy data, run your migration tool before removing legacy fields from code.",
                MessageType.Info);

            // Draw StatDef-backed values (editable inline)
            EditorGUILayout.PropertyField(valuesProp, new GUIContent("StatDef Values"), true);

            EditorGUILayout.Space();

            // Manual actions
            if (GUILayout.Button("Rebuild Runtime Cache (safe)"))
            {
                sheet.RebuildCache();
                EditorUtility.SetDirty(sheet);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}