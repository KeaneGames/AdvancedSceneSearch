using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace KeaneGames.AdvancedSceneSearch
{
    [Serializable]
    public class ASS_SearchFilter_CollectPrefabReferences : ASS_SearchFilter
    {
        private bool enabled;

        public ASS_SearchFilter_CollectPrefabReferences() : base("Collect Prefab Refs", false)
        {
        }


        public override void DrawSearchGui()
        {
            GUILayout.Label("If enabled, this will return any prefab *project* assets used in the scene");

            enabled = GUILayout.Toggle(enabled, "enabled");
            base.DrawSearchGui();
        }

        public override bool Actionable
        {
            get { return enabled; }
        }


        public override IEnumerable<GameObject> ApplyFilter(IEnumerable<GameObject> selectedObjs)
        {
            if (Actionable)
            {
                List<GameObject> prefabs = new List<GameObject>();

                foreach (GameObject selectedObj in selectedObjs)
                {
                    if(PrefabUtility.IsOutermostPrefabInstanceRoot(selectedObj))
                    {
                        GameObject prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(selectedObj);

                        if (prefab == null)
                        {
                            Debug.LogWarning("Couldn't find prefab for " + selectedObj.name);
                            continue;
                        }

                        if (!prefabs.Contains(prefab))
                        {
                            prefabs.Add(prefab);
                        }
                    }
                }

                return prefabs;
            }

            return selectedObjs;
        }

        public override string GetFilterText()
        {
           return enabled ? "Returning project prefabs instead of scene objects!" : "";
        }
    }
}