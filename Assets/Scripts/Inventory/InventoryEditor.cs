#if !UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Inventory))]
public class InventoryEditor : Editor {
	private SerializedProperty itemImagesProperty;
    private SerializedProperty itemsProperty;
    private SerializedProperty itemButtonsProperty;

    private bool[] showItemSlots = new bool[Inventory.NUM_ITEMS];

    private const string inventoryPropItemImagesName = "itemImages";
    private const string inventoryPropItemsName = "items";
    private const string inventoryPropItemButton = "itemButtons";

    private void OnEnable() {
        itemImagesProperty = serializedObject.FindProperty(inventoryPropItemImagesName);
        itemsProperty = serializedObject.FindProperty(inventoryPropItemsName);
        itemButtonsProperty = serializedObject.FindProperty(inventoryPropItemButton);
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        for (int i = 0; i < Inventory.NUM_ITEMS; i++) {
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
            EditorGUILayout.PropertyField(itemButtonsProperty.GetArrayElementAtIndex(index));
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }
}
#endif