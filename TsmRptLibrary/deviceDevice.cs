using eCardio.EnterpriseServices.Atlas.Client;
using eCardio.EnterpriseServices.Atlas.Contracts;
using log4net;
using Revampness.Services.device.Adapter;
using Revampness.Services.device.DataParser;
using Revampness.Services.device.TsmRptLibrary.EppWebService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel;

namespace Revampness.Services.device.TsmRptLibrary
{
    internal class deviceDevice : IdeviceDevice, INotifyPropertyChanged
    {
        private const int MinimumValidFacilityId = 1;
        private const int MinimumValidPatientId = 2;
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string[] _mctOrCommon =
        {
            "MCT","COMMON"
        };

        private readonly string[] _deviceTypeDeviceList =
        {
            "BodyGuardian Verité","Some Other Device"
        };

        public deviceDevice(string serialNumber, int patientid)
        {
            int intSerialNumber;
            if (String.IsNullOrEmpty(serialNumber))
            {
                throw new ArgumentException("The serial number cannot be null or empty.", nameof(serialNumber));
            }
            if (!int.TryParse(serialNumber, out intSerialNumber))
            {
                throw new ArgumentException("The serial number is not numeric.", nameof(serialNumber));
            }
            if (patientid < MinimumValidPatientId)
            {
                throw new ArgumentException("The patient id must be greater than 1.", nameof(patientid));
            }

            try
            {
            }
            catch (Exception)
            {
                this.deviceInteractions = new VeritéDeviceInteractions();
            }

            this.SerialNumber = serialNumber.Trim();
            this.PatientId = patientid;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool DoNewSettingsNeedToBeQueued => this.IsPvcEnabled != this.deviceDeviceSettings.RealTimeMonitoring;
        public bool IsInMctOrCommonMode => Enumerable.Contains(this._mctOrCommon, this.PatientDeviceStatus.ServiceType);
        public bool IsPvcEnabled => this.IsInMctOrCommonMode && this.IsGettingPvcCounts;
        public bool IsdeviceDevice => Enumerable.Contains(this._deviceTypeDeviceList, this.PatientDeviceStatus.DeviceName);
        public DeviceStatus PatientDeviceStatus { get; set; } = new DeviceStatus();
        public DeviceSettings deviceDeviceSettings { get; private set; } = new DeviceSettings();

        public VeritéDeviceInteractions deviceInteractions { get; set; }

        public bool IsGettingPvcCounts { get; private set; }
        public int PatientId { get; }
        public string SerialNumber { get; }

        public void InitializeDeviceSettings()
        {
            if (!this.DoNewSettingsNeedToBeQueued)
            {
                return;
            }
            try
            {
                this.deviceDeviceSettings = this.deviceInteractions.GetDeviceSettingsForSerialNumber(this.SerialNumber);
            }
            catch (TargetInvocationException)
            {
                this._logger.Error("Serial number: " + this.SerialNumber + " does not exist in the device database.");
                throw;
            }
        }

        public void InitializePatientDeviceStatus()
        {
            try
            {
                this.PatientDeviceStatus = this.GetService().GetPatientDeviceStatus(this.PatientId);
            }
            catch (FaultException ex)
            {
                this._logger.Error("Please contact the Physician Portal Webservice developer for information on this. " + ex);
                throw;
            }
            catch (NullReferenceException ex)
            {
                this._logger.Error("Patient id: " + this.PatientId + " does not exist in epp." + ex);
                throw;
            }

            this.IsGettingPvcCounts = this.IsFacilityGettingPvcCounts(this.PatientDeviceStatus.FacilityID);
        }

        public bool IsFacilityGettingPvcCounts(int facilityid)
        {
            if (facilityid <= MinimumValidFacilityId)
            {
                throw new ArgumentOutOfRangeException(nameof(facilityid), "This facility id is invalid");
            }
            const string hierarchy = "Facility";
            var id = facilityid.ToString();
            var client = new AtlasClient();
            var application = ApplicationReference.ByName("EPPWebsite");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, hierarchy);
            var node = HierarchyNodeReference.ByExternalKey(level, id);
            var result = client.GetCurrentValues(node);
            var results = Convert.ToBoolean(result.ToDictionary(record => record.Setting.Name, record => record.Value)["EnableEctopyCounts"]);

            this._logger.Info("Facility ID: " + facilityid);
            this._logger.Info("This facility is getting pvc counts? " + results);

            return results;
        }

        public void QueueDeviceSettings()
        {
            try
            {
                this.deviceInteractions.SaveDeviceSettingsForSerialNumber(this.deviceDeviceSettings, this.SerialNumber);
            }
            catch (NullReferenceException)
            {
                this._logger.Error("Check to see if serialnumber: " + this.SerialNumber + " exists in the device database.");
                throw;
            }
        }

        private PhysicianPortalServiceContract GetService() => new PhysicianPortalClient();

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            field = value;
            if (propertyName != null)
            {
                this.OnPropertyChanged(propertyName);
            }
            return true;
        }
    }
}