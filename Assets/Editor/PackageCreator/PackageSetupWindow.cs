#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Zutari.PackageCreator.Serializables;

namespace Zutari.PackageCreator
{
    public class PackageSetupWindow : EditorWindow
    {
        #region WINDOW

        private static PackageSetupWindow _window;

        [MenuItem("Zutari/Package/Setup", false, 10)]
        public static void OpenWindow()
        {
            _window = GetWindow<PackageSetupWindow>("Setup");
            _window.Show();
            _window.minSize = new Vector2(360f, 128f);
        }

        #endregion

        #region VARIABLES

        private TextAsset _custom;

        #endregion

        #region PROPERTIES

        private TextAsset _editorTool => Resources.Load<TextAsset>("Layouts/editor-tool");
        private TextAsset _runtimeApi => Resources.Load<TextAsset>("Layouts/runtime-api");
        private TextAsset _standard   => Resources.Load<TextAsset>("Layouts/standard");

        #endregion

        #region UNITY METHODS

        private void OnEnable()
        {
            _custom = Resources.Load<TextAsset>("Layouts/template");
        }

        public void OnGUI()
        {
            CustomPackageFileField();
            SetupOptionsButton();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
            }
        }

        #endregion

        #region METHODS

        private void CustomPackageFileField()
        {
            _custom =
                EditorGUILayout.ObjectField("Custom Layout File:", _custom, typeof(TextAsset), false) as TextAsset;
            EditorGUILayout.Space();
        }

        private void SetupOptionsButton()
        {
            if (GUILayout.Button("Editor Tool Layout"))
            {
                SetupPackage(_editorTool.text);
            }

            if (GUILayout.Button("Runtime API Layout"))
            {
                SetupPackage(_runtimeApi.text);
            }

            if (GUILayout.Button("Standard Layout"))
            {
                SetupPackage(_standard.text);
            }

            if (GUILayout.Button("Custom Layout"))
            {
                SetupPackage(_custom?.text);
            }
        }

        private void SetupPackage(string data)
        {
            JsonUtility.FromJson<Folders>(data).CreateFolders();
            AssetDatabase.Refresh();
        }

        #endregion
    }
}

#endif