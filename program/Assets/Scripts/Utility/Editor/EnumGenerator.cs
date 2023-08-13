using System.IO;
using UnityEditor;

namespace Utility.Editor {

    public class EnumGenerator {
        [MenuItem("Tools/Generate Enum From Folder")]
        public static void GenerateEnumFromFolder() {
            // 원하는 폴더 경로로 변경
            string folderPath = "Assets/Sound";
            string targetEnumName = "SoundName";

            GenerateEnum(folderPath, targetEnumName);
        }

        private static void GenerateEnum(string folderPath, string enumName) {
            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

            var enumContent = $"public enum {enumName}\n{{\n";

            foreach (var file in files) {
                if (Path.GetExtension(file) == ".meta") continue; // .meta 파일 제외

                string name = Path.GetFileNameWithoutExtension(file); // 확장자 제거
                enumContent += $"    {name},\n";
            }

            enumContent += "}";

            File.WriteAllText(Path.Combine(folderPath, $"{enumName}.cs"), enumContent);

            AssetDatabase.Refresh();
        }
    }
}