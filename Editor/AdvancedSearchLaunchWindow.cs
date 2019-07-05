using UnityEditor;
using UnityEngine;

namespace KeaneGames.AdvancedSceneSearch
{
    public class AdvancedSearchLaunchWindow : EditorWindow
    {
        private bool _dragActive;

        [MenuItem("Tools/Advanced scene search launch window")]
        private static void Init()
        {
            AdvancedSearchLaunchWindow window =
                (AdvancedSearchLaunchWindow)GetWindow(typeof(AdvancedSearchLaunchWindow));
            window.autoRepaintOnSceneChange = true;
            window.name = "Advanced Search";
            window.titleContent = new GUIContent("Adv. Search", EditorGUIUtility.FindTexture("d_ViewToolZoom"));
            window.Show();
            window.maxSize = new Vector2(window.maxSize.x, 24);
            window.minSize = new Vector2(window.minSize.x, 24);
        }

        private AdvancedSceneSearch FindOrOpenAssWindow()
        {
            AdvancedSceneSearch window = (AdvancedSceneSearch)GetWindow(typeof(AdvancedSceneSearch));
            window.autoRepaintOnSceneChange = true;
            window.name = "Advanced Search";
            window.titleContent = new GUIContent("Adv. Search", EditorGUIUtility.FindTexture("d_ViewToolZoom"));
            window.Show();
            return window;
        }

        private void OnGUI()
        {
            DropAreaGUI();
            //   if (styles == null)
            //     styles = new Styles();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Drop an object here or ", GUILayout.ExpandWidth(false));
            if (GUILayout.Button("Search", EditorStyles.miniButton))
            {
                FindOrOpenAssWindow();
            }

            GUILayout.EndHorizontal();
        }

        public void DropAreaGUI()
        {
            Event evt = Event.current;
            Rect drop_area = position; // GUILayoutUtility.GetRect (0.0f, 50.0f, GUILayout.ExpandWidth (true));
            drop_area.x = 0;
            drop_area.y = 0;

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


            if (_dragActive)
                GUI.Box(drop_area, "Drop here to copy components & name");


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

                        var assWindow = FindOrOpenAssWindow();

                        foreach (Object dragged_object in DragAndDrop.objectReferences)
                        {
                            assWindow.AddComponent(dragged_object);
                        }
                    }

                    break;
            }
        }
    }
}