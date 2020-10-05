#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Zutari.PackageCreator.General;
using Zutari.PackageCreator.Serializables;

namespace Zutari.PackageCreator
{
    public class ThirdPartyLicenseWindow : EditorWindow
    {
        #region WINDOW

        private static ThirdPartyLicenseWindow _window;

        [MenuItem("Zutari/Package/File/Third Party License", false, 20)]
        public static void OpenWindow()
        {
            _window = GetWindow<ThirdPartyLicenseWindow>("Third Party License");
            _window.Show();
        }

        #endregion

        #region VARIABLES

        private TextAsset _mdFile;

        private bool _showLicenses;
        private int _length;
        private List<License> _licenses = new List<License>();
        private List<bool> _foldouts = new List<bool>();

        private Vector2 _descriptionScroll = Vector2.zero;
        private Vector2 _licensesScroll = Vector2.zero;

        private static readonly GUILayoutOption _miniButtonWidth = GUILayout.Width(20f);
        private static readonly GUIContent _addButtonContent = new GUIContent("+", "Add");
        private static readonly GUIContent _deleteButtonContent = new GUIContent("-", "Delete");

        #endregion

        #region UNITY METHODS

        private void OnEnable()
        {
            if (_mdFile == null)
                _mdFile = Resources.Load<TextAsset>("MD/ThirdPartyNotice");
        }

        public void OnGUI()
        {
            MdFileField();
            LicenseFields();
            LicenseButtons();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
            }
        }

        #endregion

        #region METHODS

        private void MdFileField()
        {
            _mdFile =
                EditorGUILayout.ObjectField(".MD File:", _mdFile, typeof(TextAsset), false) as TextAsset;
            EditorGUILayout.Space();
        }

        private void LicenseFields()
        {
            _length = _licenses.Count;
            using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField($"Licenses {_length}");
                ShowButtons(0);
            }

            using (EditorGUILayout.ScrollViewScope sScope = new EditorGUILayout.ScrollViewScope(_licensesScroll))
            {
                for (int i = 0; i < _length; i++)
                {
                    _foldouts[i] = EditorGUILayout.Foldout(_foldouts[i], $"Show License {i + 1}");
                    if (_foldouts[i])
                    {
                        ComponentField(ref _licenses[i].Component);
                        LicenseTypeField(ref _licenses[i].LicenseType);
                        DescriptionField(ref _licenses[i].Description);
                        using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
                        {
                            ShowButtons(i);
                        }
                    }
                }

                _licensesScroll = sScope.scrollPosition;
            }
        }

        private void ComponentField(ref string component)
        {
            component = EditorGUILayout.TextField("Component:", component);
            EditorGUILayout.Space();
        }

        private void LicenseTypeField(ref string licenseType)
        {
            licenseType = EditorGUILayout.TextField("License Type: ", licenseType);
            EditorGUILayout.Space();
        }

        private void DescriptionField(ref string description)
        {
            using (EditorGUILayout.ScrollViewScope sView = new EditorGUILayout.ScrollViewScope(_descriptionScroll))
            {
                EditorGUILayout.LabelField("Description:");
                description = EditorGUILayout.TextArea(description, GUILayout.Height(256f));
                _descriptionScroll = sView.scrollPosition;
            }

            EditorGUILayout.Space();
        }

        private void LicenseButtons()
        {
            using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Create License"))
                {
                    CompileLicense();
                }

                if (GUILayout.Button("Add New License"))
                {
                    AddNewLicense();
                }
            }
        }

        private void ShowButtons(int index)
        {
            if (GUILayout.Button(_addButtonContent, _miniButtonWidth))
            {
                _licenses.Insert(index, new License());
                _foldouts.Add(true);
                _length = _licenses.Count;
            }

            if (GUILayout.Button(_deleteButtonContent, _miniButtonWidth))
            {
                if (_licenses.Count == 0) return;
                _licenses.RemoveAt(index);
                _foldouts.RemoveAt(index);
                _length = _licenses.Count;
            }
        }

        private string ReplaceData()
        {
            string data = _mdFile.text;
            for (int i = 0; i < _length; i++)
            {
                data = data.Replace(GeneralTemplates.MoreChangesTag,
                                    $"{_licenses[i].CreateLicense()}\n{GeneralTemplates.MoreChangesTag}\n");
            }

            return data;
        }

        private void CompileLicense()
        {
            if (_mdFile == null) return;

            string path = Path.Combine("Assets", "ThirdPartyNotice.md");
            File.WriteAllText(path, ReplaceData());
            AssetDatabase.Refresh();
            _mdFile = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/ThirdPartyNotice.md");
        }

        private void AddNewLicense()
        {
            if (_mdFile == null) return;

            string path = EditorUtility.OpenFilePanel("Third Party Notice", Application.dataPath, "md");
            File.WriteAllText(path, ReplaceData());
            AssetDatabase.Refresh();
        }

        #endregion
    }
}
#endif