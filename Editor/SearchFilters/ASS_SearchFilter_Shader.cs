using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KeaneGames.AdvancedSceneSearch
{
    [Serializable]
    public class ASS_SearchFilter_Shader : ASS_SearchFilter
    {
        [SerializeField]
        private Shader _shader;

        private MenuCommand _mc;
        private Rect _buttonPos;

        public ASS_SearchFilter_Shader() : base("Shader")
        {
        }

        private void DisplayShaderContext(Rect r)
        {
            if (_mc == null)
                _mc = new MenuCommand(main, 0);

            Material temp = new Material(Shader.Find("Diffuse"));

            // Rebuild shader menu:
            InternalEditorUtility.SetupShaderMenu(temp);
            // Destroy temporary material:
            Object.DestroyImmediate(temp, true);
            // Display shader popup:
            EditorUtility.DisplayPopupMenu(r, "CONTEXT/ShaderPopup", _mc);
        }

        public override void Reset()
        {
            _shader = null;

            base.Reset();
        }

        public override void DrawSearchGui()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Shader: ", GUILayout.Width(EditorGUIUtility.labelWidth));

            if (GUILayout.Button(_shader != null ? _shader.name : "Not set.", EditorStyles.popup))
            {
                Rect windowRect = main.position;
                windowRect.x = 0;
                windowRect.y = 0;
                DisplayShaderContext(_buttonPos);
            }

            if (_shader != null && GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20)))
            {
                _shader = null;
            }

            if (Event.current.type == EventType.Repaint)
                _buttonPos = GUILayoutUtility.GetLastRect();

            GUILayout.EndHorizontal();

            base.DrawSearchGui();
        }


        public override IEnumerable<GameObject> ApplyFilter(IEnumerable<GameObject> selectedObjs)
        {
            if (_shader == null)
                return selectedObjs;

            return selectedObjs.Where(x => x.GetComponent<Renderer>() != null && x.GetComponent<Renderer>().sharedMaterials.Any(y => y != null && y.shader == _shader));
        }


        public override string GetFilterText()
        {
            string searchInfo = "";
            return searchInfo;
        }

        public void SetShader(Shader shader)
        {
            this._shader = shader;
        }
    }
}