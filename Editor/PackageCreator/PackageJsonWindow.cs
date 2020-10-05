#if UNITY_EDITOR
using Zutari.PackageCreator.Serializables;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zutari.PackageCreator
{
    public class PackageJsonWindow : EditorWindow
    {
        #region WINDOW

        private static PackageJsonWindow _window;

        [MenuItem("Zutari/Package/File/Package File",false, 20)]
        public static void OpenWindow()
        {
            _window = GetWindow<PackageJsonWindow>("Package Json Creator");
            _window.Show();
        }

        #endregion

        #region VARIABLES

        private Package _package = new Package();
        private bool    _showKeywords;

        private readonly List<string> _keywords = new List<string>();
        private          int          _length;

        private static readonly GUIContent _nameContent        = new GUIContent("Package", "com.company.packagename");
        private static readonly GUIContent _displayNameContent = new GUIContent("Display Name:", "Display Name");

        private static readonly GUILayoutOption _miniButtonWidth     = GUILayout.Width(20f);
        private static readonly GUIContent      _addButtonContent    = new GUIContent("+", "Add");
        private static readonly GUIContent      _deleteButtonContent = new GUIContent("-", "Delete");

        #endregion

        #region UNITY METHODS

        public void OnGUI()
        {
            PackageNamesField();
            VersionFields();
            CategoryField();
            KeywordsFields();
            DescriptionTextArea();
            RepositoryFields();
            CreatePackageButton();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
            }
        }

        #endregion

        #region METHODS

        private void PackageNamesField()
        {
            _package.name        = EditorGUILayout.TextField(_nameContent, _package.name);
            _package.displayName = EditorGUILayout.TextField(_displayNameContent, _package.displayName);

            EditorGUILayout.Space();
        }

        private void VersionFields()
        {
            _package.version = EditorGUILayout.TextField("Package Version:", _package.version);
            _package.unity   = EditorGUILayout.TextField("Unity Version:", _package.unity);

            EditorGUILayout.Space();
        }

        private void CategoryField()
        {
            _package.category = EditorGUILayout.TextField("Category:", _package.category);

            EditorGUILayout.Space();
        }

        private void KeywordsFields()
        {
            _showKeywords = EditorGUILayout.Foldout(_showKeywords, "Keywords");
            if (_showKeywords)
            {
                _length = _keywords.Count;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Size {_length}");
                if (_length < 1)
                    ShowButtons(0);
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < _length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    _keywords[i] = EditorGUILayout.TextField($"Keyword {i}:", _keywords[i]);
                    ShowButtons(i);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void DescriptionTextArea()
        {
            EditorGUILayout.LabelField("Description");
            _package.description = EditorGUILayout.TextArea(_package.description, GUILayout.Height(128f));

            EditorGUILayout.Space();
        }

        private void RepositoryFields()
        {
            _package.repository.url  = EditorGUILayout.TextField("Repository URL:", _package.repository.url);
            _package.repository.type = EditorGUILayout.TextField("Repository Type:", _package.repository.type);
            _package.repository.revision =
                EditorGUILayout.TextField("Repository Revision:", _package.repository.revision);
        }

        private void ShowButtons(int index)
        {
            if (GUILayout.Button(_addButtonContent, EditorStyles.miniButtonLeft, _miniButtonWidth))
            {
                _keywords.Insert(index, "");
                _length = _keywords.Count;
            }

            if (GUILayout.Button(_deleteButtonContent, EditorStyles.miniButtonRight, _miniButtonWidth))
            {
                if (_keywords.Count == 0) return;
                _keywords.RemoveAt(index);
                _length = _keywords.Count;
            }
        }

        private void CreatePackageButton()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Package Json", EditorStyles.miniButtonLeft))
            {
                _package.keywords = _keywords.ToArray();
                File.WriteAllText(Path.Combine(Application.dataPath, "package.json"),
                                  JsonUtility.ToJson(_package, true));
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Load Package Json", EditorStyles.miniButtonRight))
            {
                string path = EditorUtility.OpenFilePanel("Package Json", Application.dataPath, "json");
                _package = JsonUtility.FromJson<Package>(File.ReadAllText(path));
                _keywords.AddRange(_package.keywords);
            }

            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }
}

#endif