#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (MapFileGenerator))]
public class CustomeEditorMap : Editor {
    public override void OnInspectorGUI () {
        DrawDefaultInspector ();
        if (GUILayout.Button ("Generate Map Data")) {
            MapFileGenerator generator = (MapFileGenerator) target;
            generator.GenerateMap ();
        }
    }
}
#endif