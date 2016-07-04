using Revampness.Services.Contracts;
using Revampness.Services.device.Model;
using System;
using System.Collections.Generic;

namespace TsmRptLibrary
{
    internal class EventAutomationWrapper
    {
        private readonly IAtlasConfigurationItems _atlasConfigItems;
        private readonly IConfigHelper _configHelper;
        private readonly List<EcgServiceEnums.DataRequestType> _dataRequestTypes;
        private readonly IPatientEnrollmentDates _patientEnrollmentDates;
        private readonly IdeviceRepository _deviceRepository;

        public EventAutomationWrapper(IConfigHelper configHelper, IdeviceRepository deviceRepository, IPatientEnrollmentDates patientEnrollmentDates, IAtlasConfigurationItems atlasConfigItems, List<EcgServiceEnums.DataRequestType> dataRequestTypes = null)

        {
            this._configHelper = configHelper;
            this._deviceRepository = deviceRepository;
            this._patientEnrollmentDates = patientEnrollmentDates;
            this._atlasConfigItems = atlasConfigItems;
            this._dataRequestTypes = dataRequestTypes;

            if (dataRequestTypes == null)
            {
                this._dataRequestTypes.Add(EcgServiceEnums.DataRequestType.MinimumHr);
                this._dataRequestTypes.Add(EcgServiceEnums.DataRequestType.MaximumHr);
                this._dataRequestTypes.Add(EcgServiceEnums.DataRequestType.Timed);
                this._dataRequestTypes.Add(EcgServiceEnums.DataRequestType.Telemed);
            }

            this._dataRequestTypes = dataRequestTypes;
        }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public Device Device { get; set; }

        public long DeviceId { get; set; }

        public DateTime EndDate { get; set; }

        public ICollection<EventAutomationEntry> EventAutomationEntries { get; set; }

        public long Id { get; set; }

        public bool IsActive { get; set; }

        public Guid PatientGuid { get; set; }

        public DateTime StartDate { get; set; }

        public void CreateEventAutomationEntryRecords(List<EcgServiceEnums.DataRequestType> dataRequestTypeList, bool hasExistingAutomations)
        {
            DateTime automationDate;
            var dayCounter = this.CalculateStartDayCounter(hasExistingAutomations, out automationDate);

            var automationEntryWrapper = new EventAutomationEntryWrapper(this._atlasConfigItems, this._patientEnrollmentDates);

            for (; automationDate < this._patientEnrollmentDates.EndDate; automationDate = automationDate.AddDays(1))
            {
                foreach (var dataRequestType in dataRequestTypeList)
                {
                    if (this.IgnoreCurrentDataRequest(dataRequestType))
                    {
                        continue;
                    }

                    automationEntryWrapper.CreateEventFromDataRequest(dataRequestType, automationDate, dayCounter);
                }

                dayCounter++;
            }

            //TODO: still not sure about this part
            this.EventAutomationEntries = automationEntryWrapper.EventAutomationEntries;
        }

        private int CalculateStartDayCounter(bool hasExistingAutomations, out DateTime automationDate)
        {
            var dayCounter = 0;
            automationDate = this._patientEnrollmentDates.StartDate;

            if (!hasExistingAutomations)
            {
                return dayCounter;
            }

            if (this.IsEnrollmentStartMoreRecentThanAutomationEnd())
            {
                return dayCounter;
            }

            var timeSpanOfStudy = this.EndDate - this._patientEnrollmentDates.StartDate;
            dayCounter = timeSpanOfStudy.Days;
            return dayCounter;
        }

        public void InitializeMembers(bool isActive = true)
        {
            this.CreatedAt = DateTime.Now;
            this.CreatedBy = this._configHelper.CreatedByProcessName;
            this.DeviceId = this.DeviceId;
            this.EndDate = this._patientEnrollmentDates.EndDate;
            this.IsActive = isActive;
            this.PatientGuid = this._patientEnrollmentDates.PatientGuid;
            this.StartDate = this._patientEnrollmentDates.StartDate;
        }

        public void SaveEventAutomations()
            => this._deviceRepository
            .SaveEventAutomations(this.ToEventAutomation());

        public EventAutomation ToEventAutomation()
        {
            var eventAutomation = new EventAutomation
            {
                CreatedAt = this.CreatedAt,
                CreatedBy = this.CreatedBy,
                DeviceId = this.DeviceId,
                EndDate = this.EndDate,
                IsActive = this.IsActive,
                PatientGuid = this.PatientGuid,
                StartDate = this.StartDate,
                EventAutomationEntries = this.EventAutomationEntries
            };
            return eventAutomation;
        }

        private bool IgnoreCurrentDataRequest(EcgServiceEnums.DataRequestType dataRequestType)
        {
            if (this.IgnoreDataRequest(dataRequestType)) return true;

            if (this.ProcessDataRequest(dataRequestType)) return false;

            return false;
        }

        private bool ProcessDataRequest(EcgServiceEnums.DataRequestType dataRequestType)
        {
            if (this.ShouldProcessPvcEvents(dataRequestType))
            {
                return false;
            }

            if (this.ShouldProcessTimedEvents(dataRequestType))
            {
                return false;
            }

            return true;
        }

        private bool IgnoreDataRequest(EcgServiceEnums.DataRequestType dataRequestType)
        {
            if (this.UnknownDataRequestType(dataRequestType))
            {
                return true;
            }

            if (this.IsCurrentModeCem())
            {
                if (IsDataRequestNonCem(dataRequestType))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsDataRequestNonCem(EcgServiceEnums.DataRequestType dataRequestType)
        {
            return (dataRequestType == EcgServiceEnums.DataRequestType.Telemed) || (dataRequestType == EcgServiceEnums.DataRequestType.Timed);
        }

        private bool IsCurrentModeCem()
        {
            return this._atlasConfigItems.CurrentServiceMode == PatientServiceMode.ServiceMode.Cem;
        }

        private bool ShouldProcessTimedEvents(EcgServiceEnums.DataRequestType dataRequestType)
        {
            return this._atlasConfigItems.EnableTimedEvents && (dataRequestType == EcgServiceEnums.DataRequestType.Timed);
        }

        private bool ShouldProcessPvcEvents(EcgServiceEnums.DataRequestType dataRequestType)
        {
            return this._atlasConfigItems.EnableEctopyCounts && (dataRequestType == EcgServiceEnums.DataRequestType.Telemed);
        }

        private bool UnknownDataRequestType(EcgServiceEnums.DataRequestType dataRequestType)
        {
            return !this._dataRequestTypes.Contains(dataRequestType);
        }

        /// <summary>
        ///     Ensures that the start date does not includes events from previous enrollment
        /// </summary>
        /// <returns>Corrected start date corresponding to the current enrollment</returns>
        private bool IsEnrollmentStartMoreRecentThanAutomationEnd()
        {
            return this._patientEnrollmentDates.StartDate > this.EndDate;
        }
    }
}