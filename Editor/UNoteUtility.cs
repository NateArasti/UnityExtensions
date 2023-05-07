using UnityEditor;
using UnityEngine;

namespace UNotes
{
    /// <summary>
    /// Utility class for handling most of common low-level operations
    /// </summary>
    public static class UNoteUtility
    {
        internal const string k_NotePlaceholderText = "<i>Enter note text</i>";

        #region GUI

        internal readonly static BuiltInEditorIcon s_NotesIcon = new ("d_FilterByLabel");
        internal readonly static BuiltInEditorIcon s_EndEditIcon = new ("d_FilterSelectedOnly@2x");
        internal readonly static BuiltInEditorIcon s_EditIcon = new ("d_editicon.sml");
        internal readonly static BuiltInEditorIcon s_RemoveIcon = new ("Grid.EraserTool");
        internal readonly static BuiltInEditorIcon s_AddIcon = new ("d_Toolbar Plus@2x");
        internal readonly static BuiltInEditorIcon s_PingIcon = new ("animationvisibilitytoggleon");
        internal readonly static BuiltInEditorIcon s_CopyIcon = new ("d_UnityEditor.ConsoleWindow");

        private static GUIStyle s_RichLargeLabel;
        private static GUIStyle s_TextArea;

        private static GUIContent s_EndEditContent;
        private static GUIContent s_EditContent;
        private static GUIContent s_RemoveEditContent;
        private static GUIContent s_AddContent;
        private static GUIContent s_CopyContent;
        private static GUIContent s_PingContent;

        #endregion

        private static void Initialize()
        {
            s_RichLargeLabel = new GUIStyle(EditorStyles.largeLabel);
            s_RichLargeLabel.richText = true;

            s_TextArea = new GUIStyle(EditorStyles.textArea);
            s_TextArea.wordWrap = true;

            s_EditContent = new GUIContent(s_EditIcon.Texture, "Edit note");
            s_RemoveEditContent = new GUIContent(s_RemoveIcon.Texture, "Clear note");
            s_EndEditContent = new GUIContent(s_EndEditIcon.Texture, "End editing note");
            s_AddContent = new GUIContent(s_AddIcon.Texture, "Add note");
            s_CopyContent = new GUIContent(s_CopyIcon.Texture, "Copy note text");
            s_PingContent = new GUIContent(s_PingIcon.Texture, "Ping note");
        }

        private static bool CanDraw()
        {
            if (s_RichLargeLabel == null)
            {
                Initialize();
            }

            if (UNotesStorage.Instance == null)
            {
                GUILayout.Label("No NotesStorage, something went wrong...");
                return false;
            }

            return true;
        }

        public static int GetObjectID(Object target)
        {
            if (PrefabUtility.IsPartOfAnyPrefab(target))
            {
                return PrefabUtility
                    .GetCorrespondingObjectFromSourceAtPath(
                    target,
                    PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target))
                    .GetInstanceID();
            }
            else
            {
                return target.GetInstanceID();
            }
        }

        /// <summary>
        /// Drawing "Add Note" UI for object note
        /// </summary>
        /// <param name="objectID">Instance ID of an object</param>
        /// <param name="editing">Flag to check if we are editing a note</param>
        public static void DrawAddNoteGUI(int objectID, ref bool editing)
        {
            if (!CanDraw()) return;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(s_AddContent, EditorStyles.iconButton))
            {
                UNotesStorage.Instance.AddNote(new ObjectUNote(objectID, string.Empty));
                editing = true;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Drawing "Add Note" UI for project note
        /// </summary>
        /// <param name="editing">Flag to check if we are editing a note</param>
        public static void DrawAddNoteGUI(ref bool editing)
        {
            if (!CanDraw()) return;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(s_AddContent, EditorStyles.iconButton))
            {
                UNotesStorage.Instance.AddNote(new UNote(string.Empty));
                editing = true;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Drawing whole GUI of Inspector's header with a note
        /// </summary>
        /// <param name="objectID">Instance ID of an object</param>
        /// <param name="editing">Flag to check if we are editing a note</param>
        public static void DrawObjectNoteInspectorGUI(Object obj, ref bool editing)
        {
            if (!CanDraw()) return;

            var objectID = GetObjectID(obj);

            if (UNotesStorage.Instance.TryGetObjectNote(obj, out var objectNote))
            {
                EditorGUILayout.BeginHorizontal();

                if (editing)
                {
                    DrawEditingGUI(objectNote, ref editing);
                }
                else
                {
                    DrawNoteTextLabel(objectNote);

                    EditorGUILayout.Space();

                    DrawInspectorNoteButtons(objectNote);
                    DrawDefaultNoteButtons(objectNote, ref editing);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                DrawAddNoteGUI(objectID, ref editing);
            }
        }

        /// <summary>
        /// Single line GUI of a note
        /// </summary>
        /// <param name="note">Note which GUI is drawing</param>
        /// <param name="lineWidth">Width of a GUI line</param>
        /// <param name="editing">Flag to check if we are editing a note</param>
        public static void DrawNoteInlineGUI(UNote note, float lineWidth, ref bool editing)
        {
            if (!CanDraw()) return;

            EditorGUILayout.BeginHorizontal();

            if (editing)
            {
                DrawEditingGUI(note, ref editing);
            }
            else
            {
                if(note is ObjectUNote objectNote)
                {
                    var obj = objectNote.GetObject();
                    if (obj == null)
                    {
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
                    var noteAssetContent = new GUIContent(obj.name, AssetPreview.GetMiniThumbnail(obj));

                    var nameHeight = EditorStyles.boldLabel.CalcHeight(new GUIContent(obj.name), lineWidth);
                    var nameWidth = EditorStyles.boldLabel
                        .CalcSize(new GUIContent(noteAssetContent.text, s_NotesIcon.Texture)).x;

                    GUILayout.Label(noteAssetContent, EditorStyles.boldLabel, GUILayout.Width(nameWidth), GUILayout.Height(nameHeight));

                    GUILayout.FlexibleSpace();
                }

                DrawNoteTextLabel(note);

                GUILayout.FlexibleSpace();

                DrawInlineNoteButtons(note);
                DrawDefaultNoteButtons(note, ref editing);
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawNoteTextLabel(UNote note)
        {
            var text = note.NoteText == "" ?
                k_NotePlaceholderText :
                note.NoteText;

            var content = new GUIContent(text);
            var size = s_RichLargeLabel.CalcSize(content);
            EditorGUILayout.LabelField(content, s_RichLargeLabel, GUILayout.Width(size.x), GUILayout.Height(size.y));
        }

        private static void DrawInspectorNoteButtons(ObjectUNote note)
        {
            if (!UNotesWindow.Exists)
            {
                if (GUILayout.Button(s_PingContent, EditorStyles.iconButton))
                {
                    UNotesWindow.CurrentNotesType = UNotesWindow.NotesType.Object;
                    UNotesWindow.ShowWindow();
                }
            }
        }

        private static void DrawDefaultNoteButtons(UNote note, ref bool editing)
        {
            if (string.IsNullOrWhiteSpace(note.NoteText))
                GUI.enabled = false;

            if (GUILayout.Button(s_CopyContent, EditorStyles.iconButton))
            {
                EditorGUIUtility.systemCopyBuffer = note.NoteText;
            }
            GUI.enabled = true;

            if (GUILayout.Button(s_EditContent, EditorStyles.iconButton))
            {
                editing = true;
            }
            if (GUILayout.Button(s_RemoveEditContent, EditorStyles.iconButton))
            {
                UNotesStorage.Instance.RemoveNote(note);
            }
        }

        private static void DrawInlineNoteButtons(UNote note)
        {
            if(note is ObjectUNote objectNote)
            {
                var obj = objectNote.GetObject();

                if (GUILayout.Button(s_PingContent, EditorStyles.iconButton))
                {
                    EditorGUIUtility.PingObject(obj);
                    Selection.activeObject = obj;
                }
            }
        }

        private static void DrawEditingGUI(UNote note, ref bool editing)
        {
            var noteText = EditorGUILayout.TextArea(note.NoteText, s_TextArea);

            if (noteText != note.NoteText)
            {
                note.NoteText = noteText;
                UNotesStorage.Instance.Save();
            }

            if (GUILayout.Button(s_EndEditContent, EditorStyles.iconButton))
            {
                editing = false;
            }
        }

        /// <summary>
        /// Utility class to get Built-in icon's textures
        /// </summary>
        [System.Serializable]
        public class BuiltInEditorIcon
        {
            public string name;
            public Texture Texture
            {
                get
                {
                    if (texture == null)
                    {
                        texture = EditorGUIUtility.FindTexture(name);
                    }

                    return texture;
                }
            }

            private Texture texture;

            public BuiltInEditorIcon(string name)
            {
                this.name = name;
            }
        }
    }
}