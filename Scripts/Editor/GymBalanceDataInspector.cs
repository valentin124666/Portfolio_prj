using Data;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GymBalanceData))]
    public class GymBalanceDataInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var gymData = target as GymBalanceData;

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Update Gym Balance"))
            {
                Undo.RecordObject(gymData, "Update Gym Balance");
                gymData.UpdateGymDatabase();
                EditorUtility.SetDirty(gymData);
            }
        }
    }
}
