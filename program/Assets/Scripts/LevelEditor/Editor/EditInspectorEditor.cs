using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;

namespace GemMatch.LevelEditor {
    [CustomEditor((typeof(EditInspector)))]
    public class EditInspectorEditor : Editor {
        private void Awake() {
            var t = target as EditInspector;
            if (t == null) {
                throw new NullReferenceException("NEED EditorInspector");
            }

            t.OnLoadLevel += UpdateFields;
        }

        public void UpdateFields(EditInspector inspector) {
            ColorCandidates = inspector.GetColorCandidates().ToList();
            SelectedMissions = inspector.GetMissions().ToList();
        }

#region Fields
        public enum UsableColorIndex {
            // None = -1,
            Red = 0,
            Orange = 1,
            Yellow = 2,
            Green = 3,
            Blue = 4,
            Purple = 5,
            Brown = 6,
            Pink = 7,
            Cyan = 8,
            // Random = 1000, // 이 컬러로 지정되었다면 게임 시작시 랜덤으로 컬러가 계산되어 지정된다.
            // Sole = 2000, // 시뮬레이션 용도로 사용되는 더미 컬러로, 픽할 수 있다.
        }
        public List<ColorIndex> ColorCandidates { get; private set; } = new List<ColorIndex>() { ColorIndex.Red };
        public enum UsableMission {
            // None = 0,
            NormalPiece = 1,
            // SpawnerPiece = 2,
            GoalPiece = 3,
            // VisibleCover = 4,
            // InvisibleCover = 5,
        }
        public List<Mission> SelectedMissions { get; private set; } = new List<Mission>();
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
                UpdateFields(inspector);
            }
            // New
            if (GUILayout.Button("New")) {
                GUIUtility.keyboardControl = 0;
                inspector.NewLevel();
                UpdateFields(inspector);
            }
            // Reset
            if (GUILayout.Button("Reset")) {
                inspector.ResetLevel();
                UpdateFields(inspector);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            // 색상 후보 정하기
            if (CustomButton("Add Random Candidate Colors", Color.yellow)) {
                var colorRecommand = (ColorCandidates.Count >= 1 && ColorCandidates.Last() + 1 <= ColorIndex.Cyan) ? ColorCandidates.Last() + 1 : ColorIndex.Red;
                ColorCandidates.Add(colorRecommand);
                SetColorCandidates(inspector);
            }
            {
                var viewWidth = EditorGUIUtility.currentViewWidth;
                for (int i = 0; i < ColorCandidates.Count; i++) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{i+1}번째 랜덤 가능성 : ", GUILayout.Width(viewWidth * 0.3F));
                    var tmpColor = ColorCandidates[i];
                    bool colorChanged = false;
                    ColorCandidates[i] = ColorEnumPopup(ColorCandidates[i], ButtonSmallStyle);
                    if (tmpColor != ColorCandidates[i]) colorChanged = true;
                    if (GUILayout.Button("Remove", RemoveButtonStyle, GUILayout.Width(viewWidth * 0.3F))) {
                        ColorCandidates.RemoveAt(i);
                        colorChanged = true;
                    }
                    if (colorChanged) SetColorCandidates(inspector);
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            // 미션 추가
            if (CustomButton("Add Missions", "#AE72B1") && SelectedMissions.Count >= 4) {
                SelectedMissions.Add(new Mission() {
                    count = 10,
                    entity = ModelTemplates.NormalEntityModel(ColorIndex.Red)
                });
                SetMission(inspector);
            }
            {
                var viewWidth = EditorGUIUtility.currentViewWidth;
                for (int i = 0; i < SelectedMissions.Count; i++) {
                    EditorGUILayout.BeginHorizontal();
                    // EditorGUILayout.LabelField($"{i+1}번째 미션", GUILayout.Width(viewWidth * 0.1F));
                    var cursor = SelectedMissions[i];
                    bool missionChanged = false;
                    EditorGUILayout.LabelField($"{i+1}번째 미션 : ", GUILayout.Width(viewWidth * 0.14F));
                    if (SelectedMissions[i].entity.index is EntityIndex.NormalPiece) {
                        SelectedMissions[i].entity.index = EntityEnumPopup(SelectedMissions[i].entity.index, ButtonSmallStyle);
                        SelectedMissions[i].entity.color = ColorEnumPopup(SelectedMissions[i].entity.color, ButtonVerySmallStyle);
                        SelectedMissions[i].count = (int)EditorGUILayout.IntField(SelectedMissions[i].count, ButtonVerySmallStyle);
                        missionChanged = true;
                    } else if (SelectedMissions[i].entity.index is EntityIndex.GoalPiece) {
                        SelectedMissions[i].entity.index = EntityEnumPopup(SelectedMissions[i].entity.index, ButtonSmallStyle);
                        SelectedMissions[i].count = (int)EditorGUILayout.IntField(SelectedMissions[i].count, ButtonVerySmallStyle);
                        missionChanged = true;
                    }
                    if (cursor != SelectedMissions[i]) missionChanged = true;
                    if (GUILayout.Button("Remove", RemoveButtonStyle)) {
                        SelectedMissions.RemoveAt(i);
                        missionChanged = true;
                    }
                    if (missionChanged) SetMission(inspector);
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.Separator();
            EditorGUILayout.Space();
            // Save
            if (CustomButton("Save", Color.green)) {
                inspector.SaveLevel();
            }
        }

        private ColorIndex ColorEnumPopup(ColorIndex index, GUIStyle style) {
            var viewWidth = EditorGUIUtility.currentViewWidth;
            var selection = (UsableColorIndex)EditorGUILayout.EnumPopup((UsableColorIndex)index, style);
            return (ColorIndex)selection;
        }

        private EntityIndex EntityEnumPopup(EntityIndex index, GUIStyle style) {
            var viewWidth = EditorGUIUtility.currentViewWidth;
            var selection = (UsableMission)EditorGUILayout.EnumPopup((UsableMission)index, style);
            return (EntityIndex)selection;
        }

        private static bool CustomButton(string msg, Color textColor) {
            var style = new GUIStyle(GUI.skin.button);
            style.richText = true;
            style.normal.textColor = textColor;
            return GUILayout.Button(msg, style);
        }

        private static bool CustomButton(string msg, string hexColorCode) { // ex) #7F7E83FF
            ColorUtility.TryParseHtmlString(hexColorCode, out var color);
            return CustomButton(msg, color);
        }

        private static bool CustomButton(string msg, bool condition, Color trueColor, Color falseColor) {
            if (condition) {
                return CustomButton(msg, trueColor);
            }
            return CustomButton(msg, falseColor);
        }

#region Setter
        private void SetColorCandidates(EditInspector inspector) {
            ColorCandidates = ColorCandidates.Distinct().ToList();
            inspector.SetColorCandidates(ColorCandidates);
        }

        private void SetMission(EditInspector inspector) {
            SelectedMissions = SelectedMissions.Distinct().ToList();
            inspector.SetMissions(SelectedMissions);
        }
#endregion

#region Templates

        GUIStyle ButtonDefaultStyle {
            get {
                var style = new GUIStyle(EditorStyles.miniButton);
                style.fixedWidth = 350;
                style.margin = new RectOffset(30,30,1,1);
                style.clipping = TextClipping.Clip;

                return style;
            }
        }

        GUIStyle ButtonSmallStyle {
            get {
                var style = new GUIStyle(EditorStyles.miniButton);
                style.fixedWidth = 120;
                style.margin = new RectOffset(30,50,1,1);
                return style;
            }
        }

        GUIStyle ButtonVerySmallStyle {
            get {
                var style = new GUIStyle(EditorStyles.miniButton);
                style.fixedWidth = 60;
                style.margin = new RectOffset(10,10,1,1);
                return style;
            }
        }

        GUIStyle RemoveButtonStyle {
            get {
                var style = new GUIStyle(EditorStyles.miniButton);
                style.fixedWidth = 80;
                style.margin = new RectOffset(20,20,1,1);
                style.normal.textColor = Color.red;
                style.padding = new RectOffset(20, 20, 0, 0);
                return style;
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

        GUIStyle DarkInputStyle {
            get {
                var style = new GUIStyle(EditorStyles.miniLabel);
                style.fixedWidth = 20;
                style.margin = new RectOffset(5,10,1,1);
                style.normal.textColor = Color.gray;
                return style;
            }
        }
#endregion

    }
}