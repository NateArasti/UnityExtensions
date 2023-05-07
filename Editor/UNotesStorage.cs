using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UNotes
{
    /// <summary>
    /// Storage SO that contains all UNotes
    /// It should auto-create itself if not exists already
    /// </summary>
    public class UNotesStorage : ScriptableObject
    {
        private const string k_SavePath = "Assets/Editor/UNotesStorage.asset";

        private static UNotesStorage s_Instance;
        /// <summary>
        /// Current instance of storage
        /// </summary>
        public static UNotesStorage Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = AssetDatabase.LoadAssetAtPath(k_SavePath, typeof(UNotesStorage)) as UNotesStorage;
                    if (s_Instance == null)
                    {
                        s_Instance = CreateInstance<UNotesStorage>();
                        if (!AssetDatabase.IsValidFolder("Assets/Editor"))
                        {
                            AssetDatabase.CreateFolder("Assets", "Editor");
                        }
                        AssetDatabase.CreateAsset(s_Instance, k_SavePath);
                    }
                }

                return s_Instance;
            }
        }

        private static GUID s_StorageGUID;
        private static GUID StorageGUID
        {
            get
            {
                if (s_StorageGUID.Empty())
                {
                    s_StorageGUID = AssetDatabase.GUIDFromAssetPath(k_SavePath);
                }

                return s_StorageGUID;
            }
        }

        [SerializeField] private List<UNote> m_ProjectNotes = new();
        [SerializeField] private List<ObjectUNote> m_ObjectNotes = new();

        /// <summary>
        /// All notes that not connected to a specific object but live in a project
        /// </summary>
        public IReadOnlyList<UNote> ProjectNotes => m_ProjectNotes;
        /// <summary>
        /// All notes that corresponds to specific objects
        /// </summary>
        public IReadOnlyList<ObjectUNote> ObjectNotes => m_ObjectNotes;

        /// <summary>
        /// Add a note to storage
        /// </summary>
        /// <param name="note"></param>
        public void AddNote(UNote note)
        {
            Undo.RecordObject(this, "Adding note");
            if (note is ObjectUNote objectNote)
                m_ObjectNotes.Add(objectNote);
            else m_ProjectNotes.Add(note);
            Save();
        }

        /// <summary>
        /// Remove a note from storage
        /// </summary>
        /// <param name="note"></param>
        public void RemoveNote(UNote note)
        {
            Undo.RecordObject(this, "Removing note");
            if (note is ObjectUNote objectNote)
                m_ObjectNotes.Remove(objectNote);
            else m_ProjectNotes.Remove(note);
            Save();
        }

        /// <summary>
        /// Try to get note that is connected to an object
        /// </summary>
        /// <param name="obj">Object for searching</param>
        /// <param name="objectUNote">Found note</param>
        /// <returns>Returns true if found any note, otherwise false</returns>
        public bool TryGetObjectNote(Object obj, out ObjectUNote objectUNote)
        {
            var objectID = UNoteUtility.GetObjectID(obj);
            //TODO: Can speed up this by caching?
            objectUNote = m_ObjectNotes
                .Where(note => note.ObjectID == objectID)
                .FirstOrDefault();

            return objectUNote != null;
        }

        /// <summary>
        /// Force save of storage
        /// </summary>
        public void Save()
        {
            EditorUtility.SetDirty(Instance);
            AssetDatabase.SaveAssetIfDirty(StorageGUID);
        }

        /// <summary>
        /// Clear all notes(!without returning!)
        /// </summary>
        public void ClearAll()
        {
            Undo.RecordObject(this, "Removing all notes");
            m_ProjectNotes.Clear();
            m_ObjectNotes.Clear();
            Save();
        }
    }

    [CustomEditor(typeof(UNotesStorage))]
    public class UNotesStorageEditor : Editor
    {
        private ObjectUNote m_CurrentEditNote;
        private Vector2 m_ScrollPosition;

        public override void OnInspectorGUI()
        {
            if (UNotesStorage.Instance == null || UNotesStorage.Instance.ObjectNotes.Count == 0) return;

            var cachedColor = GUI.color;
            GUI.color = Color.red;
            if(GUILayout.Button("Clear all notes"))
            {
                if(EditorUtility.DisplayDialog(
                    "Delete all notes", 
                    "Are you sure you want to delete all notes?", 
                    "Yes",
                    "No"))
                {
                    UNotesStorage.Instance.ClearAll();
                }
            }
            GUI.color = cachedColor;

            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

            var rect = EditorGUILayout.GetControlRect();
            for (int i = 0; i < UNotesStorage.Instance.ObjectNotes.Count; i++)
            {
                var note = UNotesStorage.Instance.ObjectNotes[i];
                var isEditing = m_CurrentEditNote == note;
                UNoteUtility.DrawNoteInlineGUI(note, rect.width, ref isEditing);

                if (isEditing) m_CurrentEditNote = note;
                else if (note == m_CurrentEditNote) m_CurrentEditNote = null;

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}