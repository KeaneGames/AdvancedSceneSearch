using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace KeaneGames.AdvancedSceneSearch
{
    public static class EditorGUIExtensions
    {

        public static LayerMask LayerMaskField(string label, LayerMask layerMask)
        {
            var layers = new string[32];

            for (int i = 0; i < 32; i++)
                layers[i] = i + ": " + LayerMask.LayerToName(i);

            layerMask.value = EditorGUILayout.MaskField(label, layerMask.value, layers);

            return layerMask;
        }

        public static List<string> TaskMaskToTags(int tagMask)
        {
            string[] tagNames = InternalEditorUtility.tags;

            var tags = new List<string>();
            for (int i = 0; i < tagNames.Length; i++)
            {
                if (((1 << i) & tagMask) > 0)
                    tags.Add(tagNames[i]);
            }

            return tags;
        }

        public static int TagMaskField(string label, int tagMask)
        {
            string[] tags = InternalEditorUtility.tags;

            int maskWithoutEmpty = 0;
            for (int i = 0; i < tags.Length; i++)
            {
                if (((1 << i) & tagMask) > 0)
                    maskWithoutEmpty |= 1 << i;
            }

            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, tags);

            int mask = 0;
            for (int i = 0; i < tags.Length; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                    mask |= 1 << i;
            }

            tagMask = mask;

            return tagMask;
        }
    }
}