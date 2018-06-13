#if !UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// TODO: delete this class
[CustomEditor (typeof(Pocket))]
public class PocketEditor : Editor {
	private SerializedProperty itemImagesProperty;
    private SerializedProperty itemsProperty;

    private bool[] showItemSlots = new bool[Pocket.NUM_ITEMS];

    private const string pocketPropItemImagesName = "itemImages";
    private const string pocketPropItemsName = "items";

    private void OnEnable() {
        itemImagesProperty = serializedObject.FindProperty(pocketPropItemImagesName);
        itemsProperty = serializedObject.FindProperty(pocketPropItemsName);
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        for (int i = 0; i < Pocket.NUM_ITEMS; i++) {
            ItemSlotGUI(i);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ItemSlotGUI(int index) {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        showItemSlots[index] = EditorGUILayout.Foldout(showItemSlots[index], "Item Slot " + index);

        if (showItemSlots[index]) {
            EditorGUILayout.PropertyField(itemImagesProperty.GetArrayElementAtIndex(index));
            EditorGUILayout.PropertyField(itemsProperty.GetArrayElementAtIndex(index));
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }
}
#endif