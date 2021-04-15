using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SoundSystem
{
    [CustomEditor(typeof(SFXOneShot), true)]
    public class SoundEventEditor : Editor
    {

        [SerializeField] private AudioSource _previewer;

        public void OnEnable()
        {
            _previewer = EditorUtility.CreateGameObjectWithHideFlags
                ("Audio preview", HideFlags.HideAndDontSave,
                typeof(AudioSource)).GetComponent<AudioSource>();
        }

        private void OnDisable()
        {
            DestroyImmediate(_previewer.gameObject);
        }

        public override void OnInspectorGUI()
        {
            var soundEvent = target as SFXOneShot;

            DrawDefaultInspector();

            DrawPreviewButton();
        }

        private void DrawPreviewButton()
        {
            EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

            GUILayout.Space(20);

            if (GUILayout.Button("Preview"))
            {
                ((SFXOneShot)target).Preview(_previewer);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}


