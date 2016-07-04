using Profusion.Services.coffee.OsdRptLibrary.DIWebService;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class personModeGetter : IpersonModeGetter
    {
        private readonly personModeGetterLogger _personModeGetterLogger = new personModeGetterLogger();

        private TaskCreatorAndWorkerServiceClient _taskCreatorAndWorkerServiceClient;
        private PhysicianPortalClient _physicianPortalClient;
        private DeviceStatusReader _deviceStatusReader;
        private personTableStand _personTableStand;

        public virtual void Init()
        {
            this._taskCreatorAndWorkerServiceClient = new TaskCreatorAndWorkerServiceClient();
            this._physicianPortalClient = new PhysicianPortalClient();
            this._deviceStatusReader = new DeviceStatusReader(this._physicianPortalClient, this._taskCreatorAndWorkerServiceClient);
            this._personTableStand = new personTableStand();
        }

        public personTableStand.TableStand GetpersonMode(Guid ptGuid)
        {
            this.Init();

            var mode = this._personTableStand.GetTableStand(this._deviceStatusReader, ptGuid);
            this._personModeGetterLogger.personMode(mode);
            return mode;
        }
    }
}