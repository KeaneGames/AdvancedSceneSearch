using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace KeaneGames.AdvancedSceneSearch
{
    public static class EditorGUIExtensions
    {
        private static List<int> _layerNumbers = new List<int>();

        public static LayerMask LayerMaskField(string label, LayerMask layerMask)
        {
            string[] layers = InternalEditorUtility.layers;

            _layerNumbers.Clear();

            for (int i = 0; i < layers.Length; i++)
                _layerNumbers.Add(LayerMask.NameToLayer(layers[i]));

            int maskWithoutEmpty = 0;
            for (int i = 0; i < _layerNumbers.Count; i++)
            {
                if (((1 << _layerNumbers[i]) & layerMask.value) > 0)
                    maskWithoutEmpty |= 1 << i;
            }

            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);

            int mask = 0;
            for (int i = 0; i < _layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                    mask |= 1 << _layerNumbers[i];
            }

            layerMask.value = mask;

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