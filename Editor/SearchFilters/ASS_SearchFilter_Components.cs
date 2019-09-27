using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KeaneGames.AdvancedSceneSearch
{
    [Serializable]
    public class ASS_SearchFilter_Components : ASS_SearchFilter
    {
        [SerializeField]
        private GameObject dummyObj;

        [SerializeField]
        public List<TypeSearchData> components = new List<TypeSearchData>();

        private AddComponentWindowProxy _addComponentWindowProxy;

        public ASS_SearchFilter_Components() : base("Components")
        {
            _addComponentWindowProxy = new AddComponentWindowProxy();
        }

        public void InitFakeObj()
        {
            if (dummyObj == null)
            {
                dummyObj = new GameObject("_advancedSceneSearchTempObjComponentSearch");

                foreach (TypeSearchData typeSearchData in components)
                {
                    dummyObj.AddComponent(typeSearchData.Type);
                }

                dummyObj.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
            }
        }

        public override void Reset()
        {
            components.Clear();
            base.Reset();
        }

        public override void DrawSearchGui()
        {
            TypeSearchData removeMe = null;

            GUILayout.BeginHorizontal();

            GUILayout.Label("Components:", GUILayout.Width(EditorGUIUtility.labelWidth));

            if (GUILayout.Button("Add Component", GUILayout.Height(EditorGUIUtility.singleLineHeight),
                GUILayout.ExpandWidth(true)))
            {
                Rect r = main.position;
                r.yMax = r.yMin + 80 + components.Count * 18;
                // main.ShowAddComponentWindow(new Rect(0, 0, main.position.width, 140 + components.Count * 19));
                _addComponentWindowProxy.Show(AddComponents, new Rect(0, 0, main.position.width, 76));
            }

            GUILayout.EndHorizontal();

            if (components.Count > 0)
            {
                GUILayout.BeginVertical(EditorStyles.textArea);
                foreach (TypeSearchData typeSearch in components)
                {
                    GUILayout.BeginHorizontal();


                    if (typeSearch.Type != null)
                    {
                        Texture2D tex = AssetPreview.GetMiniTypeThumbnail(typeSearch.Type);
                        GUILayout.Label(new GUIContent(typeSearch.Type.Name, tex), GUILayout.Height(16),
                            GUILayout.Width(main.position.width / 2f));
                    }

                    if (typeSearch.Amount < 0)
                    {
                        GUILayout.Label("Amount: ");

                        if (GUILayout.Button("Any", EditorStyles.miniButton))
                        {
                            typeSearch.Amount = 1;
                        }
                    }
                    else
                    {
                        // GUILayout.Label(" x" + typeSearch.Amount);
                        EditorGUIUtility.labelWidth = 60;
                        typeSearch.Amount = EditorGUILayout.IntField("Amount: ", typeSearch.Amount);
                    }

                    typeSearch.Expanded = GUILayout.Toggle(typeSearch.Expanded, "?", EditorStyles.miniButton, GUILayout.ExpandWidth(false));


                    if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                    {
                        removeMe = typeSearch;
                    }


                    GUILayout.EndHorizontal();

                    // if (typeSearch.Expanded)
                    {
                        InitFakeObj();


                        Component dummyComponent = dummyObj.GetComponent(typeSearch.Type);

                        if (dummyComponent == null)
                        {
                            Debug.LogError("ASS: Internal Error. Dummy Component of type " + typeSearch.Type.Name + " is missing!");
                        }
                        else
                        {

                            SerializedObject obj = new SerializedObject(dummyComponent);
                            SerializedProperty iterator = obj.GetIterator();

                            bool enterChildren = true;
                            while (iterator.NextVisible(enterChildren))
                            {
                                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                                {
                                    if (typeSearch.Expanded)
                                    {
                                        if (typeSearch.SerializedVars.Contains(iterator.name))
                                        {
                                            if (!GUILayout.Toggle(true, iterator.displayName))
                                            {
                                                typeSearch.SerializedVars.Remove(iterator.name);
                                            }
                                        }
                                        else
                                        {
                                            if (GUILayout.Toggle(false, iterator.displayName))
                                            {
                                                typeSearch.SerializedVars.Add(iterator.name);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (typeSearch.SerializedVars.Contains(iterator.name))
                                        {
                                            GUILayout.BeginHorizontal();
                                            EditorGUILayout.PropertyField(iterator, true);

                                            if (GUILayout.Button("X", GUILayout.Height(14), GUILayout.Width(20)))
                                            {
                                                typeSearch.SerializedVars.Remove(iterator.name);
                                            }

                                            GUILayout.EndHorizontal();
                                        }
                                    }
                                }

                                enterChildren = false;
                            }

                            obj.ApplyModifiedProperties();
                        }
                    }
                }

                if (removeMe != null)
                {
                    components.Remove(removeMe);

                    // Remove the dummy component from our dummy object to stop type conflicts.
                    var dummyComp = dummyObj.GetComponent(removeMe.Type.SystemType);
                    GameObject.DestroyImmediate(dummyObj);
                }
                GUILayout.EndVertical();
            }

            base.DrawSearchGui();
        }

        private void AddComponents(Component[] obj)
        {
            foreach (Component component in obj)
            {
                main.AddComponent(component);
            }
        }

        public override bool Actionable
        {
            get
            {
                return components.Count > 0;
            }
        }

        public override IEnumerable<GameObject> ApplyFilter(IEnumerable<GameObject> selectedObjs)
        {
            if (Actionable)
            {
                foreach (TypeSearchData typeData in components)
                {
                    if (typeData != null && typeData.Type != null)
                    {
                        if (typeData.Amount > 0)
                        {
                            selectedObjs = selectedObjs.Where(x => x.GetComponents(typeData.Type).Count(y => ComponentMatch(y, typeData)) >= typeData.Amount);
                        }

                        if (typeData.Amount == 0)
                        {
                            selectedObjs = selectedObjs.Where(x => x.GetComponents(typeData.Type).Length == 0);
                        }
                        else
                        {
                            selectedObjs = selectedObjs.Where(x => x.GetComponents(typeData.Type).Count(y => ComponentMatch(y, typeData)) >= 1);
                        }
                    }
                }
            }

            return selectedObjs;
        }

        private bool ComponentMatch(Component x, TypeSearchData typeData)
        {
            if (typeData.Type.SystemType != x.GetType())
                return false;

            if (typeData.SerializedVars != null && typeData.SerializedVars.Count > 0)
            {
                SerializedObject serializedObject = new SerializedObject(x);

                foreach (string typeDataSerializedVar in typeData.SerializedVars)
                {
                    SerializedProperty property = serializedObject.FindProperty(typeDataSerializedVar);
                    Component dummyComponent = dummyObj.GetComponent(x.GetType());
                    SerializedObject serializedDummyObject = new SerializedObject(dummyComponent);
                    SerializedProperty dummyProperty = serializedDummyObject.FindProperty(typeDataSerializedVar);

                    if (!AreSerializedPropertiesEqualValue(property, dummyProperty))
                        return false;
                }
            }

            return true;
        }

        private static bool AreSerializedPropertiesEqualValue(SerializedProperty property, SerializedProperty dummyProperty)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic:

                    if (property.isArray)
                    {
                        if (property.arraySize != dummyProperty.arraySize)
                            return false;

                        for (int i = 0; i < property.arraySize; ++i)
                        {
                            if (!AreSerializedPropertiesEqualValue(property.GetArrayElementAtIndex(i), dummyProperty.GetArrayElementAtIndex(i)))
                                return false;
                        }

                        return true;
                    }

                    throw new NotImplementedException("Generic comparison for SerializedProperties has not been implemented. TODO.");
                case SerializedPropertyType.Integer:
                    return property.intValue == dummyProperty.intValue;
                case SerializedPropertyType.Boolean:
                    return property.boolValue == dummyProperty.boolValue;
                case SerializedPropertyType.Float:
                    return Math.Abs(property.floatValue - dummyProperty.floatValue) <= float.Epsilon;
                case SerializedPropertyType.String:
                    return property.stringValue == dummyProperty.stringValue;
                case SerializedPropertyType.Color:
                    return property.colorValue == dummyProperty.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue == dummyProperty.objectReferenceValue;
                case SerializedPropertyType.LayerMask:
                    return property.intValue == dummyProperty.intValue;
                case SerializedPropertyType.Enum:
                    return property.enumValueIndex == dummyProperty.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return property.vector2Value == dummyProperty.vector2Value;
                case SerializedPropertyType.Vector3:
                    return property.vector3Value == dummyProperty.vector3Value;
                case SerializedPropertyType.Vector4:
                    return property.vector4Value == dummyProperty.vector4Value;
                case SerializedPropertyType.Rect:
                    return property.rectValue == dummyProperty.rectValue;
                case SerializedPropertyType.ArraySize:
                    return property.arraySize == dummyProperty.arraySize;
                case SerializedPropertyType.Character:
                    return property.intValue == dummyProperty.intValue;
                case SerializedPropertyType.AnimationCurve:
                    return property.animationCurveValue == dummyProperty.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return property.boundsValue == dummyProperty.boundsValue;
                case SerializedPropertyType.Gradient:
                    throw new NotImplementedException("Gradiant comparison for SerializedProperties has not been implemented. TODO.");
                case SerializedPropertyType.Quaternion:
                    return property.quaternionValue == dummyProperty.quaternionValue;
#if UNITY_2017_1_OR_NEWER
                case SerializedPropertyType.ExposedReference:
                    return property.exposedReferenceValue == dummyProperty.exposedReferenceValue;
#endif
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string GetFilterText()
        {
            string searchInfo = "";
            foreach (TypeSearchData typeSearch in components)
            {
                if (typeSearch.Type == null)
                    continue;

                if (typeSearch.Amount == 0)
                    searchInfo += "No " + typeSearch.Type.Name + "s" + Environment.NewLine;
                else if (typeSearch.Amount < 0)
                    searchInfo += "1 or more " + typeSearch.Type.Name + "s" + Environment.NewLine;
                else if (typeSearch.Amount == 1)
                    searchInfo += "Exactly " + typeSearch.Amount + " " + typeSearch.Type.Name + Environment.NewLine;
                else
                    searchInfo += "Exactly " + typeSearch.Amount + " " + typeSearch.Type.Name + "s" +
                                  Environment.NewLine;
            }

            return searchInfo;
        }

        public void AddComponent(Type type)
        {
            // If it already exists, don't add it
            if (components.Exists(x => x.Type.SystemType == type))
                return;

            components.Add(new TypeSearchData(type));

            InitFakeObj();
            dummyObj.AddComponent(type);
        }

        [Serializable]
        public class TypeSearchData
        {
            [SerializeField]
            public int Amount;

            [SerializeField]
            public SerializableSystemType Type;

            [SerializeField]
            public bool Expanded;

            [SerializeField]
            public List<string> SerializedVars;

            public TypeSearchData(Type type, int amount)
            {
                Type = type;
                Amount = amount;
                SerializedVars = new List<string>();
            }

            public TypeSearchData(Type type) : this(type, -1)
            {
            }
        }
    }
}