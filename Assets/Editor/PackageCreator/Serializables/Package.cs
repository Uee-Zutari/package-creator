using System;

namespace Zutari.PackageCreator.Serializables
{
    [Serializable]
    public class Package
    {
        public string     name = "com.company.packagename";
        public string     displayName = "Your Package Name";
        public string     version = "1.0.0";
        public string     unity = "2019.1";
        public string     description = "Description of your Package.";
        public string[]   keywords;
        public string     category;
        public Repository repository = new Repository();
    }
}