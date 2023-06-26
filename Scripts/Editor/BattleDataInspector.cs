using Data;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(BattleData))]
    public class BattleDataInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var battleData = target as BattleData;

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Update Enemy Database"))
            {
                Undo.RecordObject(battleData, "Update Enemy Database");
                battleData.UpdateEnemyDatabase();
                EditorUtility.SetDirty(battleData);
            }
        }
    }
}