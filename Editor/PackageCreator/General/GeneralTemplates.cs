using System.Collections.Generic;
using System.Text;

namespace Zutari.PackageCreator.General
{
    public enum Headers
    {
        H1 = 1,
        H2 = 2,
        H3 = 3,
        H4 = 4,
        H5 = 5,
        H6 = 6
    }

    public static class GeneralTemplates
    {
        #region VARIABLES

        public static string MoreChangesTag = "<!-- MORE CHANGES -->";
        
        public static string ChangeLogTemplate = "## [%version] - %unity-release" +
                                         "\n### %adjective"               +
                                         "\n%changes";
        
        public static string ThirdPartyNotice = "Component Name: %component" +
                                                "\nLicense Type: %license"   +
                                                "\n%description\n";

        public static string Header = "%header-type %header-name";


        private static Dictionary<Headers, string> _headers = new Dictionary<Headers, string>
        {
            {Headers.H1, "#"},
            {Headers.H2, "##"},
            {Headers.H3, "###"},
            {Headers.H4, "####"},
            {Headers.H5, "#####"},
            {Headers.H6, "######"}
        };

        private static string _changelog = "";
        private static string _thirdPartyNotice = "";
        private static string _header = "";

        #endregion

        #region PROPERTIES

        #endregion

        #region METHODS

        public static string AddChangeLog(string version, string unityRelease, string adjective, List<string> changes)
        {
            _changelog = ChangeLogTemplate.Replace("%version", version);
            _changelog = _changelog.Replace("%unity-release", unityRelease);
            _changelog = _changelog.Replace("%adjective", adjective);

            StringBuilder builder = new StringBuilder();
            foreach (string change in changes)
            {
                builder.Append($"-{change}\n");
            }

            _changelog = _changelog.Replace("%changes", builder.ToString());
            return _changelog;
        }

        public static string AddChanges(List<string> changes)
        {
            return $"{AddList(changes, "-")}";

        }

        public static string AddThirdPartyNotice(string component, string license, string description)
        {
            _thirdPartyNotice =  ThirdPartyNotice.Replace("%component", component);
            _thirdPartyNotice = _thirdPartyNotice.Replace("%license", license);
            _thirdPartyNotice = _thirdPartyNotice.Replace("%description", description);
            return _thirdPartyNotice;
        }

        public static string AddHeader(Headers header, string headerName)
        {
            return Header.Replace("%header-type", _headers[header]).Replace("%header-name", headerName);
        }

        public static string AddContentToHeader(string header, string content)
        {
            return $"{header}\n{content}";
        }

        public static string AddListContentToHeader(string header, List<string> contents)
        {
            _header = header;
            return $"{_header}\n{AddList(contents)}";
        }

        public static string AddUrlContentToHeader(string header, string url)
        {
            return $"{header}\n{url}";
        }

        private static string AddList(List<string> items, string preText = "+")
        {
            StringBuilder builder = new StringBuilder();
            foreach (string item in items)
            {
                builder.Append($"{preText}{item}\n");
            }

            return $"{builder}";
        }

        #endregion
    }
}