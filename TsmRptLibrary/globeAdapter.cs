namespace TsmRptLibrary
{
    internal class AtlasAdapter
    {
        private readonly IConfigHelper _configHelper;
        private readonly IAtlasClient _atlasClient;

        public AtlasAdapter(IConfigHelper configHelper, IAtlasClient atlasClient)
        {
            this._configHelper = configHelper;
            this._atlasClient = atlasClient;
        }

        private SettingValueBindingCollection ConnectAndGetAtlasResultsForPatient(string patientGuid)
        {
            if (this._configHelper.IsEmailConfigured)
            {
                this._atlasClient.SetWindowsCredentials(this._configHelper.DomainUserName, this._configHelper.DomainPassword, this._configHelper.Domain);
            }

            var application = ApplicationReference.ByName("Reporting");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, "Patient");
            var node = HierarchyNodeReference.ByExternalKey(level, patientGuid);

            return this._atlasClient.GetCurrentValues(node);
        }
    }
}