using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace KeaneGames.AdvancedSceneSearch
{
    [Serializable]
    public class ASS_SearchFilter_MeshName : ASS_SearchFilter
    {
        [SerializeField]
        private string _nameSearch;

        [SerializeField]
        private RegexOptions _regexOpts = RegexOptions.None;

        private bool _reselectInput; // Do we need to reselect the search box?

        public ASS_SearchFilter_MeshName() : base("Mesh Name")
        {
        }

        public override void DrawSearchGui()
        {
            GUILayout.BeginHorizontal();
            GUI.SetNextControlName("nameSearchy");
            _nameSearch = EditorGUILayout.TextField("Mesh Name:", _nameSearch, StyleData.TextField);

            if (_reselectInput)
            {
                GUI.FocusControl("nameSearchy");
                _reselectInput = false;
            }

            if (GUILayout.Button("X",
                string.IsNullOrEmpty(_nameSearch)
                    ? StyleData.SearchFieldCancelButtonEmpty
                    : StyleData.SearchFieldCancelButton,
                GUILayout.Width(20f)))
            {
                GUI.FocusControl("nameSearchy");
                _nameSearch = "";
                _reselectInput = true;
                GUIUtility.keyboardControl = 0;
            }

            GUILayout.EndHorizontal();


            base.DrawSearchGui();
        }

        public override void ObjectDragged(GameObject draggedObject)
        {
            if (string.IsNullOrEmpty(_nameSearch))
            {
                MeshFilter filter = draggedObject.GetComponent<MeshFilter>();

                if(filter != null && filter.sharedMesh != null)
                {

                    _nameSearch = filter.sharedMesh.name;
                }


            }



            base.ObjectDragged(draggedObject);
        }

        public override void Reset()
        {
            _nameSearch = "";
            base.Reset();
        }

        public override bool Actionable
        {
            get
            {
                return !string.IsNullOrEmpty(_nameSearch);
            }
        }

        public override IEnumerable<GameObject> ApplyFilter(IEnumerable<GameObject> selectedObjs)
        {
            if (Actionable)
            {
                selectedObjs = selectedObjs.Where(x => IsPass(x));
            }

            return selectedObjs;
        }

    public bool IsPass(GameObject obj)
    {
            if (string.IsNullOrEmpty(_nameSearch))
                return true;

        if (obj == null)
            return false;

        MeshFilter filt = obj.GetComponent<MeshFilter>();

        if (filt == null)
            return false;

        if (filt.sharedMesh == null)
            return false;

        string meshName = filt.sharedMesh.name;

            Regex search;

            _regexOpts = Settings.CaseSensitive.State ? RegexOptions.None : RegexOptions.IgnoreCase;

            if (Settings.UseRegex.State)
                search = new Regex(_nameSearch, _regexOpts);
            else
            {
                if (Settings.MatchWholeWord.State)
                    search = new Regex(WildcardToRegex(_nameSearch), _regexOpts);
                else
                    search = new Regex(WildcardToRegex("*" + _nameSearch + "*"), _regexOpts);
            }

          return search.IsMatch(meshName);


        }


        public override string GetFilterText()
        {
            string searchInfo = "";
            if (!string.IsNullOrEmpty(_nameSearch))
                searchInfo += "A MeshFilter with a mesh name containing \"" + _nameSearch + "\"" + Environment.NewLine;
            return searchInfo;
        }

        public static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        }
    }
}