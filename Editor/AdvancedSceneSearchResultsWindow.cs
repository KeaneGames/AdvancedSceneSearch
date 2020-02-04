using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace KeaneGames.AdvancedSceneSearch
{
    public class AdvancedSceneSearchResultsWindow : EditorWindow
    {

        public class Result
        {
            public GameObject GameObj;
            public int InstanceID;
            public Scene Scene;
            public string ScenePath;
            public string Name;
            public string Path;
            public bool Ticked;

            public string guid;
            public long localId;

            public Result(GameObject obj)
            {
                this.GameObj = obj;
                this.InstanceID = obj.GetInstanceID();
                this.Scene = obj.scene;
                this.ScenePath = obj.scene.path;
                this.Name = obj.name;
                this.Path = GetPath(obj);
                this.Ticked = false;

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

                for (int i = parents.Count -1; i >= 0; i--)
                {
                    sb.Append(parents[i].name + "/");
                }
                sb.Append(obj.name);
                return sb.ToString();
            }

            

            public void TrySelect()
            {
                if (GameObj == null)
                {
                    // Are we in the right scene?
                   
                    if (String.IsNullOrEmpty(Scene.name))
                    {
                        // awww
                        Scene = EditorSceneManager.OpenScene(ScenePath);
                    }

                    Object instanceIdToObject = EditorUtility.InstanceIDToObject(InstanceID);
                    GameObj = (GameObject)instanceIdToObject;

                    if (GameObj == null)
                    {
                        // sigh. TODO: Replace this with a better solution. Either store transform orders or atleast search via the path, not just the object name!
                        Debug.LogWarning("GameObject referenece was lost - finding the object by name. If multiple things in this scene share a name, this may return the wrong prefab." + Environment.NewLine + "Rerun your search in the current scene to fix this.");
                        GameObj = GameObject.Find(Name);

                    }
                }

                Selection.activeObject = GameObj;
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
            if (obj == null)
            {
                Debug.LogWarning("Tried to store a null object?");
                return;
            }

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
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("Select All"))
                    {
                        Selection.objects = resultPair.Value.Results.Select(x => x.GameObj).ToArray();
                    }
                    if (GUILayout.Button("Select Ticked"))
                    {
                        Selection.objects = resultPair.Value.Results.Where(x => x.Ticked).Select(x => x.GameObj).ToArray();
                    }

                    if (GUILayout.Button("Toggle Selection"))
                    {
                        foreach (Result result in resultPair.Value.Results)
                        {
                            result.Ticked = !result.Ticked;
                        }
                    }

                    GUILayout.EndHorizontal();

                    foreach (Result result in resultPair.Value.Results)
                    {
                        GUILayout.BeginHorizontal();


                        result.Ticked = GUILayout.Toggle(result.Ticked, "", GUILayout.ExpandWidth(false));
                        if (GUILayout.Button(result.Path, EditorStyles.largeLabel, GUILayout.ExpandWidth(false)))
                        {
                            result.TrySelect();
                            //Selection.activeGameObject = result.GameObj;
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