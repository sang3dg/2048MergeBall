using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(UI_Switch))]
public class UI_Switch_Selection : Editor
{
    private SerializedObject obj;
    private SerializedProperty handleMoveType, handleHorizontalMoveDirection, handleVerticalMoveDirection, handleDiagonalMoveDirection,
        handleMoveArea, OnValueChanged;
    private void OnEnable()
    {
        obj = new SerializedObject(target);
        handleMoveType = obj.FindProperty("handleMoveType");
        handleHorizontalMoveDirection = obj.FindProperty("handleHorizontalMoveDirection");
        handleVerticalMoveDirection = obj.FindProperty("handleVerticalMoveDirection");
        handleDiagonalMoveDirection = obj.FindProperty("handleDiagonalMoveDirection");
        handleMoveArea = obj.FindProperty("handleMoveArea");
        OnValueChanged = obj.FindProperty("OnValueChanged");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        obj.Update();
        EditorGUILayout.PropertyField(handleMoveType);
        if (handleMoveType.enumValueIndex == (int)UI_Switch.SwitchHandleMoveType.Horizontal)
        {
            EditorGUILayout.PropertyField(handleHorizontalMoveDirection);
        }
        else if (handleMoveType.enumValueIndex == (int)UI_Switch.SwitchHandleMoveType.Vertical)
        {
            EditorGUILayout.PropertyField(handleVerticalMoveDirection);
        }
        else if (handleMoveType.enumValueIndex == (int)UI_Switch.SwitchHandleMoveType.Diagonal)
        {
            EditorGUILayout.PropertyField(handleDiagonalMoveDirection);
        }
        EditorGUILayout.PropertyField(handleMoveArea);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(OnValueChanged);
        obj.ApplyModifiedProperties();
    }
}
