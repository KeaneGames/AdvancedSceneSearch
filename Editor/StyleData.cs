using System;
using UnityEditor;
using UnityEngine;

namespace KeaneGames.AdvancedSceneSearch
{
    [Serializable]
    public class StyleData
    {
        public bool Initialized;

        public GUIStyle ComponentButton;
        public GUIStyle DropFieldAll;
        public GUIStyle DropFieldComponents;
        public GUIStyle Header;
        public GUIStyle MiniSearchField;
        public GUIStyle SearchField;
        public GUIStyle SearchFieldCancelButton;
        public GUIStyle SearchFieldCancelButtonEmpty;
        public GUIStyle TextField;

        public Texture2D GetBox(Color col)
        {
            int size = 5;
            Color fade = new Color(col.r, col.g, col.b, col.a * 0.5f);
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    bool edge = x == 0 || y == 0 || x == size - 1 || y == size - 1;

                    tex.SetPixel(x, y, edge ? col : fade);
                }
            }

            tex.filterMode = FilterMode.Point;
            tex.Apply();
            return tex;
        }

        public void Init()
        {
            Header = new GUIStyle(EditorStyles.helpBox);
            ComponentButton = new GUIStyle("PR Label");
            Header.font = EditorStyles.boldLabel.font;
            Header.alignment = TextAnchor.MiddleCenter;
            ComponentButton.alignment = TextAnchor.MiddleLeft;
            ComponentButton.padding.left -= 15;
            ComponentButton.fixedHeight = 20f;

            SearchField = GetStyle("SearchTextField");
            TextField = GetStyle("SearchTextField");
            TextField.border.left = 16;
            MiniSearchField = GetStyle("ToolbarSeachTextField");
            SearchFieldCancelButton = GetStyle("SearchCancelButton");
            SearchFieldCancelButtonEmpty = GetStyle("SearchCancelButtonEmpty");


            DropFieldAll = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                border = new RectOffset(1, 1, 1, 1),
                padding = new RectOffset(8, 8, 8, 8),
                margin = new RectOffset(8, 8, 8, 8),
                normal = {background = GetBox(new Color(1f, 1f, 1f))}
            };
            DropFieldAll.normal.background = GetBox(new Color(0.4f, 0.9f, 0.2f));

            DropFieldComponents = new GUIStyle(DropFieldAll);
            DropFieldComponents.normal.background = GetBox(new Color(0.6f, 0.7f, 0.2f));
            Initialized = true;
        }

        private GUIStyle GetStyle(string styleName)
        {
            GUIStyle guiStyle = GUI.skin.FindStyle(styleName);
            if (guiStyle == null)
            {
                guiStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            }

            return guiStyle;
        }
    }
}