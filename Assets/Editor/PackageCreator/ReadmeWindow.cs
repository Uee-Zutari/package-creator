#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Zutari.PackageCreator
{
    public class ReadmeWindow : EditorWindow
    {
        #region WINDOW

        private static ReadmeWindow _window;

        [MenuItem("Zutari/Package/File/Readme", false, 20)]
        public static void OpenWindow()
        {
            _window = GetWindow<ReadmeWindow>("Readme Creator");
            _window.Show();
        }

        #endregion

        #region VARIABLES

        private TextAsset _mdFile;
        private string _data = "";

        private string _title = "";
        private string _description = "";
        private string _workCodes = "";
        private string _client = "";
        private string _social = "https://zutarilive.sharepoint.com/sites/ctiv-experts-live/SitePages/Home.aspx";

        private Vector2 _descriptionScroll = Vector2.zero;

        private bool _showContributors = false;
        private Vector2 _contributorsScroll = Vector2.zero;
        private int _length = 0;
        private List<string> _contributorsList = new List<string>();

        private static readonly GUILayoutOption _miniButtonWidth = GUILayout.Width(20f);
        private static readonly GUIContent _addButtonContent = new GUIContent("+", "Add");
        private static readonly GUIContent _deleteButtonContent = new GUIContent("-", "Delete");

        #endregion

        #region UNITY METHODS

        private void OnEnable()
        {
            _mdFile = Resources.Load<TextAsset>("MD/README");
        }

        public void OnGUI()
        {
            MdFileField();
            TitleField();
            DescriptionField();
            WorkCodesField();
            ContributorsField();
            ClientField();
            SocialField();
            CreateMdFile();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
            }
        }

        #endregion

        #region METHODS

        private void MdFileField()
        {
            _mdFile = EditorGUILayout.ObjectField(".MD File:", _mdFile, typeof(TextAsset), false) as TextAsset;
            EditorGUILayout.Space();
        }

        private void TitleField()
        {
            _title = EditorGUILayout.TextField("Title:", _title);
            EditorGUILayout.Space();
        }

        private void DescriptionField()
        {
            using (EditorGUILayout.ScrollViewScope sView = new EditorGUILayout.ScrollViewScope(_descriptionScroll))
            {
                EditorGUILayout.LabelField("Description:");
                _description = EditorGUILayout.TextArea(_description, GUILayout.ExpandHeight(true));
                _descriptionScroll = sView.scrollPosition;
            }

            EditorGUILayout.Space();
        }

        private void WorkCodesField()
        {
            _workCodes = EditorGUILayout.TextField("Work Codes:", _workCodes);
            EditorGUILayout.Space();
        }

        private void ContributorsField()
        {
            _showContributors = EditorGUILayout.Foldout(_showContributors, "Contributors");
            if (_showContributors)
            {
                using (EditorGUILayout.ScrollViewScope sView = new EditorGUILayout.ScrollViewScope(_contributorsScroll))
                {
                    _length = _contributorsList.Count;
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
                            _contributorsList[i] = EditorGUILayout.TextField($"Contributor {i}:", _contributorsList[i]);
                            ShowButtons(i);
                        }
                    }

                    _contributorsScroll = sView.scrollPosition;
                }
            }

            EditorGUILayout.Space();
        }

        private void ClientField()
        {
            _client = EditorGUILayout.TextField("Client", _client);
            EditorGUILayout.Space();
        }

        private void SocialField()
        {
            _social = EditorGUILayout.TextField("Social URL:", _social);
            EditorGUILayout.Space();
        }

        private void CreateMdFile()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create MD File", EditorStyles.miniButtonLeft))
            {
                CompileReadme();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ShowButtons(int index)
        {
            if (GUILayout.Button(_addButtonContent, _miniButtonWidth))
            {
                _contributorsList.Insert(index, "");
                _length = _contributorsList.Count;
            }

            if (GUILayout.Button(_deleteButtonContent, _miniButtonWidth))
            {
                if (_contributorsList.Count == 0) return;
                _contributorsList.RemoveAt(index);
                _length = _contributorsList.Count;
            }
        }

        private void CompileReadme()
        {
            if (_mdFile == null) return;

            _data = _mdFile.text;

            _data = _data.Replace("%title", _title);
            _data = _data.Replace("%description", _description);
            _data = _data.Replace("%codes", _workCodes);
            _data = _data.Replace("%client", _client);
            _data = _data.Replace("%social", _social);

            StringBuilder builder = new StringBuilder();
            foreach (string contributor in _contributorsList)
            {
                builder.Append($"+ {contributor}\n");
            }

            _data = _data.Replace("%contributors", builder.ToString());

            string path = Path.Combine(Application.dataPath, "README.md");
            File.WriteAllText(path, _data);
            AssetDatabase.Refresh();
        }

        #endregion
    }
}

#endif