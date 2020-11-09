using System;
using Zutari.PackageCreator.General;

namespace Zutari.PackageCreator.Serializables
{
    [Serializable]
    public class License
    {
        #region VARIABLES

        public string Component;
        public string LicenseType;
        public string Description;

        #endregion

        #region METHODS

        public string CreateLicense()
        {
            return GeneralTemplates.AddThirdPartyNotice(Component, LicenseType, Description);
        }

        #endregion
    }
}