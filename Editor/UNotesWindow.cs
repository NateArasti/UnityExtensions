using UnityEditor;
using UnityEngine;

namespace UNotes
{
    /// <summary>
    /// Class to show window for handling all notes
    /// </summary>
    public class UNotesWindow : EditorWindow
    {
        /// <summary>
        /// Type of notes window can handle
        /// </summary>
        public enum NotesType
        {
            Project,
            Object
        }

        private const string ProjectTitleText = "Here you can see all notes about this project";
        private const string ObjectTitleText = "Here you can see all notes that were put on some object/stuff.\nClick ping to view which object's note you're seeing";
        private const string NoNotesText = "Don't see any Notes on current objects! To add them, click + button in inspector header of some object.";

        /// <summary>
        /// Current instance of a window
        /// </summary>
        public static UNotesWindow Instance { get; private set; }
        /// <summary>
        /// Check if window is exists/opened
        /// </summary>
        public static bool Exists => Instance != null;
        /// <summary>
        /// Current type window is handling
        /// </summary>
        public static NotesType CurrentNotesType { get; set; }

        private Editor m_NotesListEditor;
        private UNote m_CurrentEditNote;
        private Vector2 m_ScrollPosition;

        [MenuItem("Window/UNotes")]
        public static void ShowWindow()
        {
            Instance = GetWindow<UNotesWindow>("UNotes");

            Instance.titleContent = new GUIContent(Instance.titleContent.text, UNoteUtility.s_NotesIcon.Texture);

            Instance.Show();
        }

        private void OnEnable()
        {
            if (!Exists)
            {
                Instance = this;
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (CurrentNotesType == NotesType.Project) GUI.enabled = false;
            if(GUILayout.Button("Project Notes", EditorStyles.toolbarButton))
            {
                CurrentNotesType = NotesType.Project;
            }
            GUI.enabled = true;

            if (CurrentNotesType == NotesType.Object) GUI.enabled = false;
            if (GUILayout.Button("Object Notes", EditorStyles.toolbarButton))
            {
                CurrentNotesType = NotesType.Object;
            }
            GUI.enabled = true;

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (CurrentNotesType == NotesType.Object)
            {
                EditorGUILayout.Space();
                GUILayout.Label(ObjectTitleText, EditorStyles.centeredGreyMiniLabel);
                EditorGUILayout.Space();

                if (UNotesStorage.Instance != null && UNotesStorage.Instance.ObjectNotes.Count > 0)
                {
                    if (m_NotesListEditor == null)
                        m_NotesListEditor = Editor.CreateEditor(UNotesStorage.Instance);

                    m_NotesListEditor.OnInspectorGUI();
                }
                else
                {
                    GUILayout.FlexibleSpace();

                    GUILayout.Label(NoNotesText, EditorStyles.centeredGreyMiniLabel);

                    GUILayout.FlexibleSpace();
                }
            }
            else if (CurrentNotesType == NotesType.Project)
            {
                EditorGUILayout.Space();
                GUILayout.Label(ProjectTitleText, EditorStyles.centeredGreyMiniLabel);
                EditorGUILayout.Space();

                if (UNotesStorage.Instance == null) return;

                m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

                for (int i = 0; i < UNotesStorage.Instance.ProjectNotes.Count; i++)
                {
                    var note = UNotesStorage.Instance.ProjectNotes[i];
                    var isEditing = m_CurrentEditNote == note;
                    UNoteUtility.DrawNoteInlineGUI(note, position.width, ref isEditing);

                    if (isEditing) m_CurrentEditNote = note;
                    else if (note == m_CurrentEditNote) m_CurrentEditNote = null;

                    EditorGUILayout.Space();
                }
                var addedNewNote = false;
                UNoteUtility.DrawAddNoteGUI(ref addedNewNote);
                if (addedNewNote) m_CurrentEditNote = UNotesStorage.Instance.ProjectNotes[^1];
                EditorGUILayout.Space();

                EditorGUILayout.EndScrollView();
            }
        }
    }
}