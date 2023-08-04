using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace GemMatch.LevelEditor {
    [CustomEditor((typeof(EditInspector)))]
    public class EditInspectorEditor : Editor {
        private void Awake() {
            var t = target as EditInspector;
            if (t == null) {
                throw new NullReferenceException("NEED EditorInspector");
            }
            // t.OnLoadLevel += OnLoadLevel;
        }

        private void OnDisable() {
            isLevelLoaded = false;
        }

        private bool isLevelLoaded = false;
        private void OnLoadLevel(IEditGameController controller) {
            // todo : EditInspector가 controller갱신 시 notify
            isLevelLoaded = true;
        }

#region Fields
        private readonly List<ColorIndex> colorCandidates = new List<ColorIndex>() { ColorIndex.None };

#endregion

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var setting = target as EditInspector;
            if (setting == null) return;

            setting.SavePath = EditorGUILayout.TextField("Save Path", setting.SavePath);

            var levelIndex = EditorGUILayout.IntField("Level Index", setting.LevelIndex);
            if (GUILayout.Button("Load")) {
                setting.LoadLevel(levelIndex);
            }

            if (GUILayout.Button("New")) {
                GUIUtility.keyboardControl = 0;
                setting.StartLevel1();
            }

            if (GUILayout.Button("Reset")) {
                setting.ResetLevel();
            }

            if (GUILayout.Button("Add Usable Color")) {
                colorCandidates.Add(ColorIndex.None);
            }

            { // 색상 후보 정하기
                    var viewWidth = EditorGUIUtility.currentViewWidth;
                    EditorGUILayout.BeginHorizontal();
                    for (int i = 0; i < colorCandidates.Count; i++) {
                        EditorGUILayout.LabelField($"{i+1}번째", GUILayout.Width(viewWidth * 0.2F));
                        colorCandidates[i] = (ColorIndex)EditorGUILayout.EnumPopup(colorCandidates[i], ButtonSmallStyle);
                        if (GUILayout.Button("Remove", ButtonSmallStyle)) {
                            colorCandidates.RemoveAt(i);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Accept Color Candidates")) {
                setting.SetColorCandidates(colorCandidates);
            }

            if (GUILayout.Button("Save")) {
                setting.SaveLevel();
            }
        }

        GUIStyle ButtonDefaultStyle {
            get {
                var buttonDefaultStyle = new GUIStyle(EditorStyles.miniButton);
                buttonDefaultStyle.fixedWidth = 350;
                buttonDefaultStyle.margin = new RectOffset(30,30,1,1);
                return buttonDefaultStyle;
            }
        }

        GUIStyle ButtonSmallStyle {
            get {
                var buttonSmallStyle = new GUIStyle(EditorStyles.miniButton);
                buttonSmallStyle.fixedWidth = 100;
                buttonSmallStyle.margin = new RectOffset(50,50,1,1);
                return buttonSmallStyle;
            }
        }

        GUIStyle AddButtonStyle {
            get {
                var addButtonStyle = new GUIStyle(ButtonDefaultStyle);
                addButtonStyle.normal.textColor = Color.yellow;
                return addButtonStyle;
            }
        }


        private void DrawToolbar() {
            // set style
            var buttonDefaultStyle = new GUIStyle(EditorStyles.miniButton);
            buttonDefaultStyle.fixedWidth = 350;
            buttonDefaultStyle.margin = new RectOffset(30,30,1,1);
            GUIStyle addButtonStyle = new GUIStyle(buttonDefaultStyle);
            addButtonStyle.normal.textColor = Color.yellow;
            GUIStyle delButtonStyle = new GUIStyle(buttonDefaultStyle);
            delButtonStyle.normal.textColor = Color.red;
            GUIStyle addPosButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
            addPosButtonStyle.fixedWidth = 200;
            addPosButtonStyle.margin = new RectOffset(60,60,1,1);
            addPosButtonStyle.normal.textColor = Color.green;
            GUIStyle delPosButtonStyle = new GUIStyle(addPosButtonStyle);
            delPosButtonStyle.normal.textColor = Color.red;
        }
    }
}