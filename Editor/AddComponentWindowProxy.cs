using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KeaneGames.AdvancedSceneSearch
{
    public class AddComponentWindowProxy
    {
        private Object _addComponentWindow;
        private bool _opened;
        private GameObject _dummyObj;
        private Action<Component[]> _callback;

        public AddComponentWindowProxy()
        {
            if (_acw_Show == null)
                InitRef();
        }

        /// <summary>
        /// Sets up the add component window
        /// </summary>
        /// <param name="callback">A list of components added.
        /// <param name="rect">The area on screen to show the AddComponentWindow
        /// Must be consumed immediately.
        /// Will be destroyed once callback is executed</param>
        public void Show(Action<Component[]> callback, Rect area)
        {
            _callback = callback;
            EditorApplication.update += Update;

            // We spawn an empty, hidden object to add components to
            // Then use the add component window to have the user add components to it
            _dummyObj = new GameObject("_advancedSceneSearchTempObj");
            _dummyObj.hideFlags = HideFlags.HideAndDontSave;

            //static bool Show(Rect rect, GameObject[] gos)
            bool result = (bool)_acw_Show.Invoke(null, new object[] {area, new[] {_dummyObj}});

            if (result)
            {
                Object[] acws = Resources.FindObjectsOfTypeAll(_addComponentWindowType);

                if (acws.Length != 1)
                    Debug.LogErrorFormat("We didn't get 1 AddComponentWindow? Got: {0}", acws.Length);

                _addComponentWindow = acws[0];
                _opened = true;
            }
        }


        private void Update()
        {
            // If the AddComponentWindow has been opened, but is now closed, a component was (probably) selected
            if (_opened && _addComponentWindow == null)
            {
                // Grab any components off it, add them to our query, then destroy the temp object
                Component[] components = _dummyObj.GetComponents<Component>();


                _callback.Invoke(components);

                Cleanup();
            }
        }

        private void Cleanup()
        {
            Object.DestroyImmediate(_dummyObj);
            _opened = false;

            EditorApplication.update -= Update;
        }

        #region Nasty reflection stuff!

        private static Type _addComponentWindowType;
        private static MethodInfo _acw_Show;
        private static Assembly _editorAssembly;


        private static void InitRef()
        {
            _editorAssembly = Assembly.GetAssembly(typeof(EditorWindow));

            // There's no nice inbuilt way to open the "AddComponent" menu, so we use reflection to do it.
            // However, unity has moved it in a few versions!
#if UNITY_2019_1_OR_NEWER
            _addComponentWindowType = _editorAssembly.GetType("UnityEditor.AddComponent.AddComponentWindow");
#elif UNITY_2018_3_OR_NEWER
            _addComponentWindowType = _editorAssembly.GetType("UnityEditor.AdvancedDropdown.AddComponentWindow");
#else
            _addComponentWindowType = _editorAssembly.GetType("UnityEditor.AddComponentWindow");
#endif
            _acw_Show = _addComponentWindowType.GetMethod("Show", BindingFlags.Static | BindingFlags.NonPublic);
        }

        #endregion;
    }
}