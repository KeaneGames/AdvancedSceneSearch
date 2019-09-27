using System;
using System.Collections.Generic;
using System.Text;
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
            public string Path;

            public Result(GameObject obj) : this()
            {
                this.GameObj = obj;
                this.InstanceID = obj.GetInstanceID();
                this.Scene = obj.scene;
                this.Name = obj.name;
                this.Path = GetPath(obj);
            }

            public string GetPath(GameObject obj)
            {
                List<Transform> parents = new List<Transform>();

                Transform current = obj.transform.parent;

                while(current != null)
                {
                    parents.Add(current);
                    current = current.parent;
                }

                StringBuilder sb = new StringBuilder();

                for (int i = parents.Count -1; i > 0; i--)
                {
                    sb.Append(parents[i].name + "/");
                }
                sb.Append(obj.name);
                return sb.ToString();
            }
        }

        public class SceneData
        {
            public Scene Scene;
            public string SceneName;

            public SceneData(Scene scene)
            {
                this.Scene = scene;
                this.SceneName = scene.name;
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

        public Dictionary<string, ResultData> Results;

        public void StoreResult(GameObject obj)
        {
            string scenePath = (obj.scene != null && obj.scene.path != null) ? obj.scene.path : "Project";

            if (!Results.ContainsKey(scenePath))
                Results.Add(scenePath, new ResultData(obj.scene));

            Results[scenePath].Add(new Result(obj));

        }

        Vector2 scrollPos;
        public void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            if(Results != null)
            foreach (KeyValuePair<string, ResultData> resultPair in Results)
            {
                GUILayout.BeginVertical();

                int resultCount = resultPair.Value.Results.Count;

                resultPair.Value.Expanded = EditorGUILayout.Foldout(resultPair.Value.Expanded, "[" + resultCount + "] Scene: " + resultPair.Key);

                if(resultPair.Value.Expanded)
                {
                    foreach (Result result in resultPair.Value.Results)
                    {
                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button(result.Path, EditorStyles.largeLabel, GUILayout.ExpandWidth(false)))
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

        public void SetResults(GameObject[] gameObject, bool clearResults = true)
        {
            if (clearResults || Results == null)
                Clear();

            foreach (var item in gameObject)
            {
                StoreResult(item);
            }
        }

        public void Clear()
        {
            Results = new Dictionary<string, ResultData>();
        }
    }

}