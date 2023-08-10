using UnityEngine;
using System.Linq;

namespace Utility.CustomMouse {
    public class CustomMouse : MonoBehaviour {
        [Header("0번은 마우스를 뗐을 때, 1번은 마우스를 클릭한 이미지를 지정해주세요.")]
        public Texture2D[] cursorTexture;

        [Header("이미지상 커서의 위치를 지정해주세요. (2번 손가락 기준 20, 20)")]
        public Vector2 cursorPoint = Vector2.zero;

        private Texture2D[] textureCaches;
        private Vector2 cursorPointCache = Vector2.zero;

        public bool IsShowing { get; set; }

        private float size = 1f;

        private float Size {
            get => size;
            set => size = Mathf.Clamp(value, 0, float.MaxValue);
        }

        private void Update() {
            if (IsShowing == false) return;

            if (Input.GetMouseButtonDown(0)) { //drag start
                Cursor.SetCursor(textureCaches[1], cursorPointCache, CursorMode.ForceSoftware);
            }

            if (Input.GetMouseButtonUp(0)) {
                Cursor.SetCursor(textureCaches[0], cursorPointCache, CursorMode.ForceSoftware);
            }
        }

        public void ShowCustomMouse(float size = 1F) {
            CacheScaledTextures(size);
            Cursor.SetCursor(textureCaches[0], cursorPointCache, CursorMode.ForceSoftware);
            IsShowing = true;
        }

        public void HideCustomMouse() {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            IsShowing = false;
        }

        private void CacheScaledTextures(float size = 1F) {
            textureCaches = cursorTexture.Select(texture => ScaleTexture(texture, size)).ToArray();
        }

        private Texture2D ScaleTexture(Texture2D source, float targetScale = 1F) {
            var targetWidth = (int)(source.width * targetScale);
            var targetHeight = (int)(source.height * targetScale);

            var result = new Texture2D(targetWidth, targetHeight, source.format, true);
            var pixels = result.GetPixels(0);
            var incX = 1.0f / targetWidth;
            var incY = 1.0f / targetHeight;
            for (int px = 0; px < pixels.Length; px++) {
                pixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth),
                    incY * Mathf.Floor(px / targetWidth));
            }

            result.SetPixels(pixels, 0);
            result.Apply();

            cursorPointCache.x = cursorPoint.x * targetScale;
            cursorPointCache.y = cursorPoint.y * targetScale;

            return result;
        }


        private static CustomMouse customMouse;

        private static CustomMouse CustomMouseInstance =>
            customMouse ??= Instantiate(Resources.Load<CustomMouse>(nameof(CustomMouse)));


        public static void Show() {
            if (CustomMouseInstance.IsShowing) return;
            CustomMouseInstance.ShowCustomMouse();

        }

        public static void Hide() {
            if (CustomMouseInstance == null) return;
            CustomMouseInstance.HideCustomMouse();
        }
    }
}