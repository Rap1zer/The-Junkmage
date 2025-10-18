using JunkMage.Stats;

namespace JunkMage.Editor
{
    #if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;
    
    [CustomEditor(typeof(StatSheet))]
    public class StatSheetEditor : Editor
    {
        private SerializedProperty valuesProp;
        private SerializedProperty entriesProp;
    
        private void OnEnable()
        {
            valuesProp = serializedObject.FindProperty("values");
            entriesProp = serializedObject.FindProperty("entries");
        }
    
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var sheet = (StatSheet)target;
    
            EditorGUILayout.HelpBox("StatSheet: preferred StatDef-backed entries. Legacy entries are kept as fallback.", MessageType.Info);
    
            // Draw StatDef-backed values (editable inline)
            EditorGUILayout.PropertyField(valuesProp, new GUIContent("StatDef Values"), true);
    
            EditorGUILayout.Space();
    
            // Draw legacy entries for visibility / editing
            EditorGUILayout.PropertyField(entriesProp, new GUIContent("Legacy Entries (fallback)"), true);
    
            EditorGUILayout.Space();
    
            // Manual actions (deferred)
            if (GUILayout.Button("Rebuild Runtime Cache (safe)"))
            {
                // safe: local operation
                sheet.RebuildCache();
                EditorUtility.SetDirty(sheet);
            }
    
            if (GUILayout.Button("Migrate legacy -> StatDef entries (manual)"))
            {
                // defer execution to the next editor tick to avoid doing heavy work while UI is drawing
                EditorApplication.delayCall += () => MigrateLegacyToStatDefEntries(sheet);
            }
    
            serializedObject.ApplyModifiedProperties();
        }
    
        private void MigrateLegacyToStatDefEntries(StatSheet sheet)
        {
            if (sheet == null) return;
    
            if (!EditorUtility.DisplayDialog("Migrate legacy StatEntry -> StatDef entries",
                $"This will try to migrate {sheet.entries?.Count ?? 0} legacy entries into the StatDef-backed list. It will NOT delete legacy entries. Continue?",
                "Migrate", "Cancel"))
                return;
    
            // Gather all StatDef assets once (safe operation)
            string[] guids = AssetDatabase.FindAssets("t:StatDefinition");
            var defs = new List<StatDefinition>();
            foreach (var g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                var d = AssetDatabase.LoadAssetAtPath<StatDefinition>(path);
                if (d != null) defs.Add(d);
            }
    
            // Build map enum -> StatDef (fast lookup)
            var map = new Dictionary<Stat, StatDefinition>();
            foreach (var d in defs)
            {
                if (d != null) map[d.stat] = d;
            }
    
            Undo.RecordObject(sheet, "Migrate legacy StatSheet to StatDef entries");
    
            int migrated = 0;
    
            if (sheet.entries != null)
            {
                for (int i = 0; i < sheet.entries.Count; i++)
                {
                    var e = sheet.entries[i];
                    if (e == null) continue;
    
                    // Skip if values already contains an entry for this enum (via statDef or already migrated)
                    bool exists = false;
                    if (sheet.values != null)
                    {
                        for (int j = 0; j < sheet.values.Count; j++)
                        {
                            var v = sheet.values[j];
                            if (v == null) continue;
                            if (v.statDef != null && v.statDef.stat == e.type) { exists = true; break; }
                        }
                    }
    
                    if (exists) continue;
    
                    // If we have a StatDef for this enum, use it; otherwise create an entry with null statDef (designer can fix)
                    StatDefinition found = null;
                    map.TryGetValue(e.type, out found);
    
                    if (sheet.values == null) sheet.values = new List<StatValueEntry>();
                    sheet.values.Add(new StatValueEntry(found, e.baseValue));
                    migrated++;
                }
            }
    
            EditorUtility.SetDirty(sheet);
            AssetDatabase.SaveAssets();
            Debug.Log($"Migrated {migrated} legacy entries into StatDef-backed entries for {sheet.name}.");
        }
    }
    #endif
}