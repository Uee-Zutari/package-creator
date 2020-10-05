using System;

namespace Zutari.PackageCreator.Serializables
{
    [Serializable]
    public class Repository
    {
        public string url      = "";
        public string type     = "git";
        public string revision = "";
    }
}