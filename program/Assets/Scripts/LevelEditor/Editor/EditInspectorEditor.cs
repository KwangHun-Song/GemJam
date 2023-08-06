using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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

            t.ExecuteWhenUpdateLevel(inspector => {
                ColorCandidates = inspector.GetColorCandidates().ToList();
            }).Forget();
        }

#region Fields
        public List<ColorIndex> ColorCandidates { get; private set; } = new List<ColorIndex>() { ColorIndex.None };

#endregion

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (Application.isPlaying == false) return;

            var inspector = target as EditInspector;
            if (inspector == null) return;

            inspector.SavePath = EditorGUILayout.TextField("Save Path", inspector.SavePath);
            var levelStage = EditorGUILayout.IntField("Level Stage", inspector.LevelIndex + 1);
            inspector.LevelIndex = levelStage - 1;

            // Load
            if (GUILayout.Button("Load")) {
                inspector.LoadLevel(inspector.LevelIndex);
            }
            // New
            if (GUILayout.Button("New")) {
                GUIUtility.keyboardControl = 0;
                inspector.LevelIndex++;
                inspector.NewLevel();
            }
            // Reset
            if (GUILayout.Button("Reset")) {
                inspector.ResetLevel();
            }
            EditorGUILayout.Separator();
            // Set ColorCandidate
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Usable Colors")) {
                var colorRecommand = ColorCandidates.Count >= 1 && ColorCandidates.Last() < ColorIndex.Random? ColorCandidates.Last() + 1 : ColorIndex.None;
                ColorCandidates.Add(colorRecommand);
                SetColorCandidates(inspector);
            }
            EditorGUILayout.EndHorizontal();
            { // 색상 후보 정하기
                    var viewWidth = EditorGUIUtility.currentViewWidth;
                    for (int i = 0; i < ColorCandidates.Count; i++) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"{i+1}번째", GUILayout.Width(viewWidth * 0.2F));
                        var tmpColor = ColorCandidates[i];
                        bool colorChanged = false;
                        ColorCandidates[i] = (ColorIndex)EditorGUILayout.EnumPopup(ColorCandidates[i], ButtonSmallStyle);
                        if (tmpColor != ColorCandidates[i]) colorChanged = true;
                        if (GUILayout.Button("Remove", ButtonSmallStyle)) {
                            ColorCandidates.RemoveAt(i);
                            colorChanged = true;
                        }
                        if (colorChanged) SetColorCandidates(inspector);
                        EditorGUILayout.EndHorizontal();
                    }
            }
            EditorGUILayout.Separator();
            // Save
            if (GUILayout.Button("Save")) {
                inspector.SaveLevel();
            }
        }

        private void SetColorCandidates(EditInspector inspector) {
            ColorCandidates = ColorCandidates.Distinct().ToList();
            inspector.SetColorCandidates(ColorCandidates);
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