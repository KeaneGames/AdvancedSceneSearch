using System;
using System.Collections.Generic;
using UnityEngine;

namespace KeaneGames.AdvancedSceneSearch
{
    [Serializable]
    public abstract class ASS_SearchFilter
    {
        [SerializeField]
        private string FilterName;

        [SerializeField]
        private bool _enabled = true;

        protected AdvancedSceneSearch main;

        protected ASS_SearchFilter(string name)
        {
            FilterName = name;
        }

        protected StyleData StyleData
        {
            get { return main.Styles; }
        }

        protected SettingsData Settings
        {
            get { return main.SettingsDataHolder; }
        }

        public bool Enabled
        {
            get { return _enabled; }
        }

        public void Setup(AdvancedSceneSearch main)
        {
            this.main = main;
        }

        public virtual void DrawSearchGui()
        {
        }

        public virtual void Reset()
        {
        }

        public abstract IEnumerable<GameObject> ApplyFilter(IEnumerable<GameObject> selectedObjs);
        public abstract string GetFilterText();

        public virtual void ObjectDragged(GameObject draggedObject)
        {
        }

        public void DrawSettingsGui()
        {
            GUILayout.BeginHorizontal();

            _enabled = GUILayout.Toggle(_enabled, FilterName);

            GUILayout.EndHorizontal();
        }
    }
}