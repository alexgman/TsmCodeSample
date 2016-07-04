using Revampness.Services.Contracts;
using Revampness.Services.device.Model;
using System;
using System.Collections.Generic;

namespace TsmRptLibrary
{
    internal class EventAutomationAutomator
    {
        private readonly IAtlasConfigurationItems _atlasConfigurationItems;
        private IdeviceRepository _deviceRepository;

        public EventAutomationAutomator(IAtlasConfigurationItems atlasConfigurationItems, IdeviceRepository deviceRepository)
        {
            this._atlasConfigurationItems = atlasConfigurationItems;
            this._deviceRepository = deviceRepository;
        }

        private EventAutomationEntry CreateEventAutomationEntries(DateTime automationDate, EcgServiceEnums.DataRequestType dataRequestType, string createdByProcessName)//shouldwork for min max t
        {
            if (dataRequestType == EcgServiceEnums.DataRequestType.Telemed && !this._atlasConfigurationItems.EnableEctopyCounts)
            {
                return null;
            }

            return new EventAutomationEntry
            {
                CreatedAt = DateTime.Now,
                CreatedBy = createdByProcessName,
                DataRequestTypeId = (int)dataRequestType,
                AutomationDate = automationDate
            };
        }

        public virtual EventAutomation InitializeEventAutomationRecord(IPatientEnrollmentDates patientDatesOfEnrollment, string createdByProcessName, long deviceId)
        {
            return new EventAutomation
            {
                CreatedAt = DateTime.Now
                                ,
                CreatedBy = createdByProcessName
                                ,
                DeviceId = deviceId
                                ,
                EndDate = patientDatesOfEnrollment.EndDate
                                ,
                IsActive = true
                                ,
                PatientGuid = patientDatesOfEnrollment.PatientGuid
                                ,
                StartDate = patientDatesOfEnrollment.StartDate
            };
        }

        private DateTime Next2Am(DateTime automationDate)
        {
            DateTime automationDatePlus2Am = automationDate.Date.AddHours(2);
            return automationDate <= automationDatePlus2Am ? automationDatePlus2Am : automationDatePlus2Am.AddDays(1);
        }

        public virtual EventAutomation CreateEventAutomationEntryRecords(EventAutomation eventAutomationInitialized
            , List<EcgServiceEnums.DataRequestType> dataRequestTypeList, bool enableEctopyCounts, bool enabledTimedEvents, DateTime endDateOfEnrollment)
        {
            var eventAutomation = eventAutomationInitialized;
            var automationDate = eventAutomation.StartDate;
            for (; automationDate < endDateOfEnrollment; automationDate = automationDate.AddDays(1))
            {
                foreach (var dataRequestType in dataRequestTypeList)
                {
                    if (!enableEctopyCounts && (dataRequestType == EcgServiceEnums.DataRequestType.Telemed))
                    {
                        continue;
                    }
                    if (!enabledTimedEvents && (dataRequestType == EcgServiceEnums.DataRequestType.Timed))
                    {
                        continue;
                    }

                    if (dataRequestType == EcgServiceEnums.DataRequestType.Telemed)
                    {
                        eventAutomation.EventAutomationEntries.Add(this.CreateEventAutomationEntries(this.Next2Am(automationDate), dataRequestType, "ProcessName"));

                        if (automationDate.AddDays(1) >= endDateOfEnrollment)
                        {
                            eventAutomation.EventAutomationEntries.Add(this.CreateEventAutomationEntries(this.Next2Am(automationDate.AddDays(1)), dataRequestType, "ProcessName"));
                        }
                    }

                    eventAutomation.EventAutomationEntries.Add(this.CreateEventAutomationEntries(automationDate, dataRequestType, "ProcessName"));
                }
            }

            return eventAutomation;
        }
    }
}