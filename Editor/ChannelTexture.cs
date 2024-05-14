using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DreadScripts.TextureUtility
{
    [System.Serializable]
    public class ChannelTexture
    {
        public string name;
        public Texture2D texture;
        public bool invert;
        public Color defaultColor;
        public ColorMode mode = ColorMode.Red;

        public enum ColorMode
        {
            Red,
            Green,
            Blue,
            Alpha
        }

        public ChannelTexture(string n, int mode)
        {
            name = n;
            SetMode(mode, true);
            if (n == "Alpha")
                defaultColor = Color.white;
        }

        public void SetMode(int i, bool ignoreSave = false)
        {
            switch (i)
            {
                case 0:
                    mode = ColorMode.Red;
                    break;
                case 1:
                    mode = ColorMode.Green;
                    break;
                case 2:
                    mode = ColorMode.Blue;
                    break;
                case 3:
                    mode = ColorMode.Alpha;
                    break;
            }

            if (!ignoreSave)
            {
                EditorPrefs.SetInt("TextureUtilityChannel" + name, i);
            }
        }

        public Texture2D GetChannelColors(int width, int height, out float[] colors, bool unloadTempTexture)
        {
            Texture2D textureToUse;
            if (texture)
                textureToUse = texture;
            else
            {
                textureToUse = new Texture2D(1, 1, TextureFormat.RGBA32, false, true);
                textureToUse.SetPixel(0, 0, defaultColor);
                textureToUse.Apply();
            }

            Texture2D newTexture = TextureUtility.GetColors(textureToUse, width, height, out Color[] myColors, unloadTempTexture);
            colors = myColors.Select(c =>
            {
                switch (mode)
                {
                    case ColorMode.Red:
                        return c.r;
                    case ColorMode.Green:
                        return c.g;
                    case ColorMode.Blue:
                        return c.b;
                    default:
                        return c.a;
                }
            }).ToArray();
            if (invert)
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = 1 - colors[i];
                }
            }

            if (!texture && unloadTempTexture)
                Object.DestroyImmediate(textureToUse);

            return newTexture;

        }

        public void DrawGUI()
        {
            GUIStyle buttonGroupStyle = new GUIStyle(GUI.skin.GetStyle("toolbarbutton")) {padding = new RectOffset(1, 1, 1, 1), margin = new RectOffset(0, 0, 1, 1)};
            using (new GUILayout.VerticalScope("box"))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(name, "boldlabel");
                    GUILayout.FlexibleSpace();
                }

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    bool dummy;
                    EditorGUI.BeginChangeCheck();
                    dummy = GUILayout.Toggle(mode == ColorMode.Red, "R", buttonGroupStyle, GUILayout.Width(16));
                    if (EditorGUI.EndChangeCheck())
                        if (dummy)
                            SetMode(0);

                    EditorGUI.BeginChangeCheck();
                    dummy = GUILayout.Toggle(mode == ColorMode.Green, "G", buttonGroupStyle, GUILayout.Width(16));
                    if (EditorGUI.EndChangeCheck())
                        if (dummy)
                            SetMode(1);

                    EditorGUI.BeginChangeCheck();
                    dummy = GUILayout.Toggle(mode == ColorMode.Blue, "B", buttonGroupStyle, GUILayout.Width(16));
                    if (EditorGUI.EndChangeCheck())
                        if (dummy)
                            SetMode(2);

                    EditorGUI.BeginChangeCheck();
                    dummy = GUILayout.Toggle(mode == ColorMode.Alpha, "A", buttonGroupStyle, GUILayout.Width(16));
                    if (EditorGUI.EndChangeCheck())
                        if (dummy)
                            SetMode(3);
                    GUILayout.FlexibleSpace();
                }

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    Rect myTextureRect = GUILayoutUtility.GetRect(66, 66);
                    Rect myColorRect = new Rect(myTextureRect) {x = myTextureRect.x + 1, y = myTextureRect.y + 45, width = 20, height = 20};

                    if (!texture)
                        defaultColor = EditorGUI.ColorField(myColorRect, GUIContent.none, defaultColor, false, false, false);
                    texture = (Texture2D) EditorGUI.ObjectField(myTextureRect, texture, typeof(Texture2D), false);
                    if (!texture)
                        defaultColor = EditorGUI.ColorField(myColorRect, GUIContent.none, defaultColor, false, false, false);


                    GUILayout.FlexibleSpace();
                }

                invert = GUILayout.Toggle(invert, "Invert", "toolbarbutton");
            }
        }
    }
}

