using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace KeaneGames.AdvancedSceneSearch
{
    [Serializable]
    public class ASS_SearchFilter_Tag : ASS_SearchFilter
    {
        [SerializeField]
        private int _tagMask = ~0;

        [SerializeField]
        private List<string> _tags = new List<string>();

        public ASS_SearchFilter_Tag() : base("Tag")
        {
        }

        public override void Reset()
        {
            _tags.Clear();

            _tagMask = ~0;
            base.Reset();
        }

        public override void DrawSearchGui()
        {
            string defTxt = "(select a tag)";

            string newtag = defTxt;
            _tagMask = EditorGUIExtensions.TagMaskField("Tags:", _tagMask); //EditorGUILayout.TagField("Tags:", defTxt);

            if (newtag != defTxt)
                _tags.Add(newtag);

            base.DrawSearchGui();
        }

        public override IEnumerable<GameObject> ApplyFilter(IEnumerable<GameObject> selectedObjs)
        {
            if (_tags.Count > 0)
            {
                selectedObjs = selectedObjs.Where(x => _tags.Contains(x.tag));
            }

            return selectedObjs;
        }

        public override string GetFilterText()
        {
            string searchInfo = "";

            int allTags = (1 << InternalEditorUtility.tags.Length) - 1;
            if (_tagMask == ~0 || _tagMask == allTags || _tagMask == 0)
            {
                //searchInfo += "Any tag" + Environment.NewLine;
            }
            else
            {
                if (_tags.Count == 1)
                    searchInfo += "A tag of: ";
                else if (_tags.Count > 1)
                    searchInfo += "One of the following tags: ";

                _tags = EditorGUIExtensions.TaskMaskToTags(_tagMask);

                foreach (string tag in _tags)
                {
                    searchInfo += tag + " ";
                }

                if (_tags.Count > 0)
                    searchInfo += Environment.NewLine;
            }

            return searchInfo;
        }
    }
}