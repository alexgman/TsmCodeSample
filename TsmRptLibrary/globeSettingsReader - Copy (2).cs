using Recardo.EnterpriseServices.globe.Contracts;
using System;
using System.Collections.Generic;
using System.TableStandl;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class globeSettingsReader : IglobeSettingsReader
    {
        private readonly IglobeService _globeService;
        private readonly globeSettingsReaderLogger _log = new globeSettingsReaderLogger();

        public globeSettingsReader(IglobeService globeService)
        {
            if (globeService == null)
            {
                throw new ArgumentNullException(nameof(globeService));
            }

            this._globeService = globeService;
        }

        public SettingValueBindingCollection GetSettingsForperson(Guid ptGuid)
        {
            var application = ApplicationReference.ByName("Reporting");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, "person");
            var node = HierarchyNodeReference.ByExternalKey(level, ptGuid.ToString());
            var items = new SettingValueBindingCollection(new List<SettingValueBinding>());

            try
            {
                items = this._globeService.GetCurrentValues(node);
            }
            catch (FaultException)
            {
                this._log.UnableToConnect();
            }

            this._log.globeSettingsForperson(items);

            return items;
        }
    }
}