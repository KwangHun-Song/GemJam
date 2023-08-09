using System.IO;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor {
    public class GlobalUtility {
        public static int screenshot;
        private static string RootPath => Application.dataPath + "/../ScreenShot";

        [MenuItem("GemJam/Screen Capture _F4", false, 22)]
        public static void ScreenCaptureFunction () {
            if (!Directory.Exists(RootPath)) Directory.CreateDirectory(RootPath);

            var fileName = $"{RootPath}/{screenshot}.png";
            while (File.Exists(fileName)) {
                fileName = $"{RootPath}/{++screenshot}.png";
            }

            ScreenCapture.CaptureScreenshot(fileName);
            EditorUtility.RevealInFinder(RootPath);
        }

    }
}