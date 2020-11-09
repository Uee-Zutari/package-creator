using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Zutari.PackageCreator.Serializables
{
    [Serializable]
    public class Folders
    {
        #region VARIABLES

        public List<string> paths = new List<string>();

        #endregion

        #region METHODS

        public void CreateFolders()
        {
            foreach (string path in paths)
            {
                if (Directory.Exists(path)) continue;
                Debug.Log($"Created Folder: {path}");
                Directory.CreateDirectory(Path.Combine(Application.dataPath, path));
            }
        }

        #endregion
    }
}