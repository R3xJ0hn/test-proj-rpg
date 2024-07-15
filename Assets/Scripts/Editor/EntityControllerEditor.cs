using RPG.Combat;
using RPG.Controls;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntityController))]
public class EntityControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var entityController = (EntityController)target;

        entityController.SelectedControllerType = (EntityController.ControllerType)EditorGUILayout.EnumPopup("Controller Type", entityController.SelectedControllerType);

        if (entityController.SelectedControllerType == EntityController.ControllerType.AI)
        {
            entityController.PatrolPath = (PatrolPath)EditorGUILayout.ObjectField("Patrol Path", entityController.PatrolPath, typeof(PatrolPath), true);
        }

        DrawPropertiesExcluding(serializedObject, "m_Script", "controllerType", "patrolPath");

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
