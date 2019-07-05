using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace KeaneGames.AdvancedSceneSearch
{
    /// <summary>
    /// A unity editor window that allows you to search the scene for objects with
    /// specific properties
    /// </summary>
    public class AdvancedSceneSearch : EditorWindow, IHasCustomMenu
    {
        private readonly GUIContent _settingsMenuItem = new GUIContent("Settings");
        private readonly GUIContent _searchMenuItem = new GUIContent("Search");

        [SerializeField]
        private CurrentPage _currentPage;

        [SerializeField]
        private SettingsData _settings;

        private bool _dragActive; // Are we currently dragging something?


        private StyleData _styleData;
        private Vector2 _scrollPos;


        private enum CurrentPage
        {
            Search = 0,
            Settings = 1
        }

        #region Filters - References kept out of array for serialization.

        [SerializeField]
        private ASS_SearchFilter_Components _filterComponents;

        [SerializeField]
        private ASS_SearchFilter_Name _filterName;

        [SerializeField]
        private ASS_SearchFilter_Tag _filterTag;

        [SerializeField]
        private ASS_SearchFilter_Layer _filterLayer;

        [SerializeField]
        private ASS_SearchFilter_Shader _filterShader;

        [SerializeField]
        private ASS_SearchFilter[] _filters;

        #endregion


        public SettingsData SettingsDataHolder
        {
            get { return _settings; }
        }

        public StyleData Styles
        {
            get
            {
                if (_styleData == null)
                    OnEnable();
                return _styleData;
            }
        }

        [MenuItem("Tools/Advanced scene search")]
        private static void Init()
        {
            AdvancedSceneSearch window = (AdvancedSceneSearch)GetWindow(typeof(AdvancedSceneSearch));
            window.autoRepaintOnSceneChange = true;
            window.name = "Advanced Search";
            window.titleContent = new GUIContent("Adv. Search", EditorGUIUtility.FindTexture("d_ViewToolZoom"));
            window.Show();
        }


        public void AddItemsToMenu(GenericMenu menu)
        {
            switch (_currentPage)
            {
                case CurrentPage.Search:
                    menu.AddItem(_settingsMenuItem, false, SettingsSelected);
                    break;
                case CurrentPage.Settings:
                    menu.AddItem(_searchMenuItem, false, SearchSelected);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnSelectedShaderPopup(string command, Shader shader)
        {
            if (shader != null)
            {
                _filterShader.SetShader(shader);
            }
        }


        public void OnEnable()
        {
            _settings = new SettingsData();
            _styleData = new StyleData();

            if (_filterComponents == null)
                _filterComponents = new ASS_SearchFilter_Components();

            if (_filterName == null)
                _filterName = new ASS_SearchFilter_Name();
            if (_filterTag == null)
                _filterTag = new ASS_SearchFilter_Tag();
            if (_filterLayer == null)
                _filterLayer = new ASS_SearchFilter_Layer();
            if (_filterShader == null)
                _filterShader = new ASS_SearchFilter_Shader();

            if (_filters == null)
                _filters = new ASS_SearchFilter[]
                {
                    _filterName, _filterTag, _filterLayer, _filterComponents, _filterShader
                };


            foreach (ASS_SearchFilter assSearchFilter in _filters)
            {
                assSearchFilter.Setup(this);
            }
        }

        public void AddComponent(Object component)
        {
            //Debug.Log(dragged_object.GetType().Name);

            if (component is Transform)
            {
                // We don't want to search for transform everytime, since anything in the scene will have one.
                return;
            }

            if (component is Component)
            {
                OnSelected(component.GetType());
            }
            else if (component is GameObject)
            {
                foreach (ASS_SearchFilter assSearchFilter in _filters)
                {
                    assSearchFilter.ObjectDragged((GameObject)component);
                }


                foreach (Component comp in ((GameObject)component).GetComponents<Component>())
                {
                    OnSelected(comp.GetType());
                }
            }
            else if (component is MonoScript)
            {
                OnSelected(((MonoScript)component).GetClass());
            }
        }


        public Rect GetDropArea(Rect baseArea, float start, float end)
        {
            Rect newArea = baseArea;

            float size = baseArea.height * (end - start);

            newArea.yMin = (int)(baseArea.height * start);
            newArea.height = (int)size;

            return newArea;
        }

        public void DropAreaGUI()
        {
            Event evt = Event.current;
            Rect drop_area = position; // GUILayoutUtility.GetRect (0.0f, 50.0f, GUILayout.ExpandWidth (true));
            drop_area.x = 0;
            drop_area.y = 0;

            Rect drop_area_all = GetDropArea(drop_area, 0f, 0.1f);
            Rect drop_area_allbutname = GetDropArea(drop_area, 0.1f, 0.2f);
            Rect drop_area_componentsonly = GetDropArea(drop_area, 0.2f, 0.6f);
            Rect drop_area_namelayertags = GetDropArea(drop_area, 0.6f, 1f);
            Rect drop_area_fullwindow = GetDropArea(drop_area, 0f, 1f);


            switch (evt.type)
            {
                case EventType.DragUpdated:
                    _dragActive = true;
                    break;
                case EventType.DragPerform:
                    _dragActive = true;
                    break;
                case EventType.DragExited:
                    _dragActive = false;
                    break;
            }

            bool isGameObject = DragAndDrop.objectReferences.Length > 0 &&
                                DragAndDrop.objectReferences[0] is GameObject;


            if (_dragActive)
            { 
                //TODO
                /*if (isGameObject)
                {
                    GUI.Label(drop_area, "Drop in an area below!");
                    GUI.Box(drop_area_all, "Copy everything", Styles.dropFieldAll);
                    GUI.Box(drop_area_allbutname, "Copy every but the name", Styles.dropFieldComponents);
                    GUI.Box(drop_area_componentsonly, "Copy components & name", Styles.dropFieldAll);
                    GUI.Box(drop_area_namelayertags, "Copy name, layer and tags", Styles.dropFieldComponents);
                }
                else*/
                {
                    //GUI.Box(drop_area_fullwindow, "Drop the component here to add it to the search", Styles.dropFieldAll);
                    GUI.Box(drop_area_fullwindow, "Drop the object here to add it to the search", Styles.DropFieldAll);
                }
            }


            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:


                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                    if (DragAndDrop.objectReferences[0] is Component)
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    else if (DragAndDrop.objectReferences[0] is GameObject)
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    else if (DragAndDrop.objectReferences[0] is MonoScript)
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    else
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;


                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object dragged_object in DragAndDrop.objectReferences)
                        {
                            AddComponent(dragged_object);
                        }
                    }

                    break;
            }
        }

        private void OnGUI()
        {
            if (!Styles.Initialized)
                Styles.Init();


            _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, false);

            switch (_currentPage)
            {
                case CurrentPage.Search:
                    OnGUI_DrawSearch();
                    break;
                case CurrentPage.Settings:
                    OnGUI_DrawSettings();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GUILayout.EndScrollView();
        }

        private void OnGUI_DrawSettings()
        {
            GUILayout.Label(" * Adv Scene Search Settings * ", EditorStyles.boldLabel);

            GUILayout.Label("Enabled filters:", EditorStyles.boldLabel);
            foreach (ASS_SearchFilter assSearchFilter in _filters)
            {
                assSearchFilter.DrawSettingsGui();
            }
        }

        private void OnGUI_DrawSearch()
        {
            EditorGUIUtility.labelWidth = 85;
            foreach (ASS_SearchFilter assSearchFilter in _filters)
            {
                if (assSearchFilter.Enabled)
                    assSearchFilter.DrawSearchGui();
            }


            // Settings
            GUILayout.BeginHorizontal();
            {
                foreach (SettingData setting in SettingsDataHolder.AllSettings)
                {
                    setting.DrawButton();
                }
            }
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Reset", GUILayout.Height(24f)))
            {
                DoReset();
            }

            if (GUILayout.Button("Search", GUILayout.Width(position.width * 0.75f), GUILayout.Height(24f)))
            {
                DoSearch();
            }

            /*
            if (GUILayout.Button("S", GUILayout.Height(24f)))
            {
               currentPage = CurrentPage.Settings;
            }*/


            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            string searchInfo = "You are searching for gameObjects with: " + Environment.NewLine;

            foreach (ASS_SearchFilter assSearchFilter in _filters)
            {
                searchInfo += assSearchFilter.GetFilterText();
            }

            GUILayout.Label(searchInfo, EditorStyles.centeredGreyMiniLabel);


            DropAreaGUI();
        }

        private void DoSearch()
        {
            List<GameObject> objs = null;

            if (SettingsDataHolder.IncludeDisabled.State)
            {
                objs = new List<GameObject>();
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var s = SceneManager.GetSceneAt(i);
                    if (s.isLoaded)
                    {
                        var allGameObjects = s.GetRootGameObjects();
                        for (int j = 0; j < allGameObjects.Length; j++)
                        {
                            var go = allGameObjects[j];
                            objs.AddRange(go.GetComponentsInChildren<Transform>(true).Select(x => x.gameObject));
                        }
                    }
                }
            }
            else
            {
                objs = FindObjectsOfType<Transform>().Select(x => x.gameObject).ToList();
            }

            IEnumerable<GameObject> selectedObjs = objs;

            foreach (ASS_SearchFilter assSearchFilter in _filters)
            {
                if (assSearchFilter.Enabled)
                    selectedObjs = assSearchFilter.ApplyFilter(selectedObjs);
            }

            Selection.objects = selectedObjs.ToArray();
        }

        private void DoReset()
        {
            foreach (ASS_SearchFilter assSearchFilter in _filters)
            {
                assSearchFilter.Reset();
            }
        }


        private void OnSelected(Type type)
        {
            _filterComponents.AddComponent(type);
            Repaint();
        }

        private void SettingsSelected()
        {
            _currentPage = CurrentPage.Settings;
        }

        private void SearchSelected()
        {
            _currentPage = CurrentPage.Search;
        }


    }


    #region SerializableSystemType

    // Simple helper class that allows you to serialize System.Type objects.
    // Use it however you like, but crediting or even just contacting the author would be appreciated (Always 
    // nice to see people using your stuff!)
    //
    // Written by Bryan Keiren (http://www.bryankeiren.com)


    [Serializable]
    public class SerializableSystemType
    {
        [SerializeField]
        private string m_AssemblyName;

        [SerializeField]
        private string m_AssemblyQualifiedName;

        [SerializeField]
        private string m_Name;

        private Type m_SystemType;

        public SerializableSystemType(Type _SystemType)
        {
            m_SystemType = _SystemType;
            m_Name = _SystemType.Name;
            m_AssemblyQualifiedName = _SystemType.AssemblyQualifiedName;
            m_AssemblyName = _SystemType.Assembly.FullName;
        }

        public string Name
        {
            get { return m_Name; }
        }

        public string AssemblyQualifiedName
        {
            get { return m_AssemblyQualifiedName; }
        }

        public string AssemblyName
        {
            get { return m_AssemblyName; }
        }

        public Type SystemType
        {
            get
            {
                if (m_SystemType == null)
                {
                    GetSystemType();
                }

                return m_SystemType;
            }
        }

        private void GetSystemType()
        {
            m_SystemType = Type.GetType(m_AssemblyQualifiedName);
        }


        public static implicit operator Type(SerializableSystemType m)
        {
            return m.SystemType;
        }

        public static implicit operator SerializableSystemType(Type m)
        {
            return new SerializableSystemType(m);
        }

        public override bool Equals(object obj)
        {
            SerializableSystemType temp = obj as SerializableSystemType;
            if ((object)temp == null)
            {
                return false;
            }

            return Equals(temp);
        }

        public bool Equals(SerializableSystemType _Object)
        {
            //return m_AssemblyQualifiedName.Equals(_Object.m_AssemblyQualifiedName);
            return _Object.SystemType.Equals(SystemType);
        }

        public override int GetHashCode()
        {
            return m_SystemType.GetHashCode();
        }

        public static bool operator ==(SerializableSystemType a, SerializableSystemType b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(SerializableSystemType a, SerializableSystemType b)
        {
            return !(a == b);
        }
    }

    #endregion
}