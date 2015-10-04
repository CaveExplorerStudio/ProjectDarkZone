using UnityEngine;
using UnityEditor;
using System.Collections;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using UnityEditorInternal.VersionControl;

using UnityEditor.AnimatedValues;
using UnityEngine.Events;

public class MenuEditor : MonoBehaviour {



	[MenuItem("Dark Zone/Map Generator", false, 50)]
	static void OpenMapGeneratorWindow(MenuCommand menuCommand) {
		EditorWindow meshWindow = EditorWindow.GetWindow<MapGeneratorEditor> ("Map Generator");
	}















}
