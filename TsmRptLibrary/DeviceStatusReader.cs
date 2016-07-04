using Profusion.Services.coffee.OsdRptLibrary.DIWebService;
using Profusion.Services.coffee.OsdRptLibrary.EppWebService;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class DeviceStatusReader : IDeviceStatusReader
    {
        private readonly DeviceStatusReaderLogger _logger = new DeviceStatusReaderLogger();
        private readonly ITaskCreatorAndWorkerService _taskCreatorAndWorkerService;
        private PhysicianPortalServiceContract _physicianPortalServiceContract;

        public DeviceStatusReader(PhysicianPortalServiceContract physicianPortalServiceContract,
            ITaskCreatorAndWorkerService taskCreatorAndWorkerService
            )
        {
            if (physicianPortalServiceContract == null)
            {
                throw new ArgumentNullException();
            }

            if (taskCreatorAndWorkerService == null)
            {
                throw new ArgumentNullException();
            }
            this._physicianPortalServiceContract = physicianPortalServiceContract;
            this._taskCreatorAndWorkerService = taskCreatorAndWorkerService;
        }

        public void Configure(ConfigHelper configHelper)
        {
        }

        public DeviceStatus GetpersonDeviceStatus(Guid ptGuid)
        {
            if (this._physicianPortalServiceContract == null)
            {
                this._physicianPortalServiceContract = new PhysicianPortalClient();
            }
            var deviceStatus = this._physicianPortalServiceContract.GetpersonDeviceStatus(this.PtGuidToId(ptGuid));

            if (deviceStatus == null)
            {
                this._logger.personNotFoundInEpp(ptGuid);
                return null;
            }

            this._logger.Status(deviceStatus);
            return deviceStatus;
        }

        private int PtGuidToId(Guid ptGuid)
        {
            int ptId;
            var ptIdStr = this._taskCreatorAndWorkerService.GetMapping(ptGuid.ToString(), -1, MappingTypes.person);
            if (!int.TryParse(ptIdStr, out ptId))
            {
                throw new ApplicationException("The person guid: " + ptGuid + " could not be mapped to a person id.");
            }
            this._logger.FoundpersonId(ptGuid, ptId);
            return ptId;
        }
    }
}