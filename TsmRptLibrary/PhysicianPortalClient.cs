using System;
using System.Threading.Tasks;
using Profusion.Services.coffee.OsdRptLibrary.EppWebService;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class PhysicianPortalClient : PhysicianPortalServiceContract
    {
        private readonly WcfServiceProxy<PhysicianPortalServiceContract> _proxy =
            new WcfServiceProxy<PhysicianPortalServiceContract>("WSHttpBinding_PhysicianPortalServiceContract");

        public virtual DeviceStatus GetpersonDeviceStatus(int ptId)
        {
            return this._proxy.Use(x => x.GetpersonDeviceStatus(ptId));
        }

        public Task<DeviceStatus> GetpersonDeviceStatusAsync(int ptId)
        {
            throw new NotImplementedException();
        }

        public string GetpersonNonClinicalNotes(string ptGuid)
        {
            return this._proxy.Use(x => x.GetpersonNonClinicalNotes(ptGuid));
        }

        public Task<string> GetpersonNonClinicalNotesAsync(string ptGuid)
        {
            throw new NotImplementedException();
        }

        public bool IsBusinessHours(int facilityId)
        {
            return this._proxy.Use(x => x.IsBusinessHours(facilityId));
        }

        public Task<bool> IsBusinessHoursAsync(int facilityId)
        {
            throw new NotImplementedException();
        }
    }
}