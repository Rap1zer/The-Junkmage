#if UNITY_EDITOR
using System.Linq;
using JunkMage.Stats;
using UnityEditor;
using UnityEngine;

namespace JunkMage.Editor
{
    public static class StatDefinitionDatabaseBuilder
    {
        private const string ResourcesPath = "Assets/ScriptableObjects/Stats/Definitions";
        private const string DbAssetPath = "Assets/ScriptableObjects/Stats/Definitions/StatDefinitionDatabase.asset";

        [MenuItem("Tools/Stats/Build StatDefinition Database")]
        public static void BuildDatabaseMenu()
        {
            BuildDatabase();
            Debug.Log("StatDefinitionDatabase built.");
        }

        public static void BuildDatabase()
        {
            // find all StatDefinition assets anywhere in the project
            var guids = AssetDatabase.FindAssets("t:StatDefinition");
            var defs = guids
                .Select(g => AssetDatabase.GUIDToAssetPath(g))
                .Select(p => AssetDatabase.LoadAssetAtPath<StatDefinition>(p))
                .Where(d => d != null)
                .ToArray();

            // ensure Resources folder exists (optional)
            if (!AssetDatabase.IsValidFolder(ResourcesPath))
                AssetDatabase.CreateFolder("Assets", "Resources");

            // load or create DB asset
            var db = AssetDatabase.LoadAssetAtPath<StatDefinitionDatabase>(DbAssetPath);
            if (db == null)
            {
                db = ScriptableObject.CreateInstance<StatDefinitionDatabase>();
                AssetDatabase.CreateAsset(db, DbAssetPath);
            }

            // populate and save
            db.definitions = defs;
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif