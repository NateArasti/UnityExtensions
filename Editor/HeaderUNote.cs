using System;
using UnityEditor;

namespace UNotes
{
    /// <summary>
    /// Static class that showing note in a header of an object's inspector
    /// </summary>
    [InitializeOnLoad]
    internal static class HeaderUNote
    {
        private static readonly Type[] s_IgnoredTypes = new[] { typeof(AssetImporter) };

        private static Editor s_CachedEditor;
        private static bool s_IsEditingNote;

        static HeaderUNote()
        {
            Editor.finishedDefaultHeaderGUI += OnHeaderGUI;
        }

        private static void InitializeHeaderEditor(Editor header)
        {
            s_CachedEditor = header;
            s_IsEditingNote = false;
        }

        private static void OnHeaderGUI(Editor header)
        {
            //Can't show note for multiple objects
            if (header.targets.Length > 1) return;

            var targetType = header.target.GetType();
            foreach (var type in s_IgnoredTypes)
            {
                if (type.IsAssignableFrom(targetType))
                {
                    return;
                }
            }

            if(s_CachedEditor != header)
            {
                InitializeHeaderEditor(header);
            }

            EditorGUILayout.Space(15);
            UNoteUtility.DrawObjectNoteInspectorGUI(header.target, ref s_IsEditingNote);
        }
    }
}