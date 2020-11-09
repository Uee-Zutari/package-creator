#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Zutari.PackageCreator.General;

namespace Zutari.PackageCreator
{
    public class ChangeLogWindow : EditorWindow
    {
        #region WINDOW

        private static ChangeLogWindow _window;

        [MenuItem("Zutari/Package/File/ChangeLog", false,20)]
        public static void OpenWindow()
        {
            _window = GetWindow<ChangeLogWindow>("Change Log");
            _window.Show();
        }

        #endregion

        #region VARIABLES

        private TextAsset _changelogMd;

        private string _data = "";


        private string _packageVersion;
        private string _unityVersion;
        private string _changeType;
        private List<string> _changes = new List<string>();

        private bool _showChanges = false;
        private Vector2 _changesScroll = Vector2.zero;
        private int _length = 0;

        private static readonly GUILayoutOption _miniButtonWidth = GUILayout.Width(25f);
        private static readonly GUIContent _addButtonContent = new GUIContent("+", "Add");
        private static readonly GUIContent _deleteButtonContent = new GUIContent("-", "Delete");

        #endregion

        #region UNITY METHODS

        private void OnEnable()
        {
            if (_changelogMd == null)
                _changelogMd = Resources.Load<TextAsset>("MD/CHANGELOG");
        }

        public void OnGUI()
        {
            MdFileField();
            PackageVersionField();
            UnityVersionField();
            ChangeDescription();
            ChangesField();

            CreateMdFileButton();
            AppendableChangesButtons();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
            }
        }

        #endregion

        #region METHODS

        private void MdFileField()
        {
            _changelogMd =
                EditorGUILayout.ObjectField(".MD File:", _changelogMd, typeof(TextAsset), false) as TextAsset;
            EditorGUILayout.Space();
        }

        private void PackageVersionField()
        {
            _packageVersion = EditorGUILayout.TextField("Package Version: ", _packageVersion);
            EditorGUILayout.Space();
        }

        private void UnityVersionField()
        {
            _unityVersion = EditorGUILayout.TextField("Unity Version: ", _unityVersion);
            EditorGUILayout.Space();
        }

        private void ChangeDescription()
        {
            _changeType = EditorGUILayout.TextField("Change Type:", _changeType);
            EditorGUILayout.Space();
        }

        private void ChangesField()
        {
            _showChanges = EditorGUILayout.Foldout(_showChanges, "Changes");
            if (_showChanges)
            {
                using (EditorGUILayout.ScrollViewScope sView = new EditorGUILayout.ScrollViewScope(_changesScroll))
                {
                    _length = _changes.Count;
                    using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField($"Size {_length}");
                        if (_length < 1)
                            ShowButtons(_length);
                    }

                    for (int i = 0; i < _length; i++)
                    {
                        using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
                        {
                            _changes[i] = EditorGUILayout.TextField($"Change {i}:", _changes[i]);
                            ShowButtons(i);
                        }
                    }

                    _changesScroll = sView.scrollPosition;
                }
            }

            EditorGUILayout.Space();
        }

        private void ShowButtons(int index)
        {
            if (GUILayout.Button(_addButtonContent, _miniButtonWidth))
            {
                _changes.Insert(index, "");
                _length = _changes.Count;
            }

            if (GUILayout.Button(_deleteButtonContent, _miniButtonWidth))
            {
                if (_changes.Count == 0) return;
                _changes.RemoveAt(index);
                _length = _changes.Count;
            }
        }

        private void CreateMdFileButton()
        {
            if (GUILayout.Button("Create ChangeLog"))
            {
                CompileChangeLog();
            }
        }

        private void AppendableChangesButtons()
        {
            using(EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Append Changes to Log"))
                {
                    AppendChanges();
                }
                if (GUILayout.Button("Append New Changelog"))
                {
                    AppendNewChangelog();
                }
            }
        }

        private void CompileChangeLog()
        {
            if (_changelogMd == null) return;

            _data = _changelogMd.text.Replace(GeneralTemplates.MoreChangesTag,
                                              $"{GeneralTemplates.AddChangeLog(_packageVersion, _unityVersion, _changeType, _changes)}{GeneralTemplates.MoreChangesTag}\n");

            string path = Path.Combine(Application.dataPath, "CHANGELOG.md");
            File.WriteAllText(path, _data);
            AssetDatabase.Refresh();
            _changelogMd = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/CHANGELOG.md");
        }

        private void AppendNewChangelog()
        {
            if (_changelogMd == null) return;

            _data = _changelogMd.text.Replace(GeneralTemplates.MoreChangesTag,
                                              $"\n{GeneralTemplates.AddChangeLog(_packageVersion, _unityVersion, _changeType, _changes)}{GeneralTemplates.MoreChangesTag}\n");

            string path =
                EditorUtility.OpenFilePanel("Open ChangeLog", Application.dataPath, "md");
            File.WriteAllText(path, _data);
            AssetDatabase.Refresh();
        }

        private void AppendChanges()
        {
            if (_changelogMd == null) return;

            _data = _changelogMd.text;
            _data = _data.Replace(GeneralTemplates.MoreChangesTag,
                                  $"{GeneralTemplates.AddChanges(_changes)}{GeneralTemplates.MoreChangesTag}\n");

            string path =
                EditorUtility.OpenFilePanel("Open ChangeLog", Application.dataPath, "md");
            File.WriteAllText(path, _data);
            AssetDatabase.Refresh();
        }

        #endregion
    }
}
#endif