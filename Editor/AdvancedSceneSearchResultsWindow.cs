using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KeaneGames.AdvancedSceneSearch
{
    public class AdvancedSceneSearchResultsWindow : EditorWindow
    {

        public struct Result
        {
            public GameObject GameObj;
            public int InstanceID;
            public Scene Scene;
            public string Name;

            public Result(GameObject obj) : this()
            {
                this.GameObj = obj;
                this.InstanceID = obj.GetInstanceID();
                this.Scene = obj.scene;
                this.Name = obj.name;
            }
        }

        public class ResultData
        {
            public List<Result> Results;
            public bool Expanded;
            public Scene Scene;

            public ResultData(Scene scene)
            {
                Results = new List<Result>();
                Expanded = true;
                Scene = scene;
            }

            public void Add(Result result)
            {
                Results.Add(result);
            }
        }

        public Dictionary<Scene, ResultData> Results;

        public void StoreResult(GameObject obj)
        {
            if (!Results.ContainsKey(obj.scene))
                Results.Add(obj.scene, new ResultData(obj.scene));

            Results[obj.scene].Add(new Result(obj));

        }

        Vector2 scrollPos;
        public void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (KeyValuePair<Scene, ResultData> resultPair in Results)
            {
                GUILayout.BeginVertical();

                if(GUILayout.Button("Scene: " + resultPair.Key.name))
                {
                    resultPair.Value.Expanded = !resultPair.Value.Expanded;
                }

                if(resultPair.Value.Expanded)
                {
                    foreach (Result result in resultPair.Value.Results)
                    {
                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button(result.Name, EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                        {
                            Selection.activeGameObject = result.GameObj;
                        }

                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();

        }

        internal void SetResults(GameObject[] gameObject)
        {
            Results = new Dictionary<Scene, ResultData>();
            foreach (var item in gameObject)
            {
                StoreResult(item);
            }
        }
    }

}