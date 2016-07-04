using Revampness.Services.Contracts;
using Revampness.Services.device.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TsmRptLibrary
{
    internal class EventAutomationEntryWrapper
    {
        protected readonly IAtlasConfigurationItems _atlasConfiguration;
        protected readonly IPatientEnrollmentDates _patientEnrollmentDates;

        public EventAutomationEntryWrapper(IAtlasConfigurationItems atlasConfigurationItems, IPatientEnrollmentDates patientEnrollmentDates)
        {
            this._atlasConfiguration = atlasConfigurationItems;
            this._patientEnrollmentDates = patientEnrollmentDates;
        }

        public ICollection<EventAutomationEntry> EventAutomationEntries { get; set; }

        public void CreateEventFromDataRequest(EcgServiceEnums.DataRequestType dataRequestType, DateTime automationDateTime, int dayCounter)
        {
            if (this._atlasConfiguration.CurrentServiceMode == PatientServiceMode.ServiceMode.Cem)
            {
                this.CreateEventsForCem(dataRequestType, automationDateTime, dayCounter);
            }
            else
            {
                this.CreateEventsForNonCem(dataRequestType, automationDateTime, dayCounter);
            }
        }

        public void DeleteOldEventAutomationEntryData(string serialNumber)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["device"].ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException("Configuration string for 'device' is not found.");
            }

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                var cmd = new SqlCommand("spDeleteOldEventAutomationEntryData", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@patientguid", this._patientEnrollmentDates.PatientGuid);
                cmd.Parameters.AddWithValue("@serialnumber", serialNumber);
                var rowsaffected = cmd.ExecuteNonQuery();
            }
        }

        protected EventAutomationEntry CreateEventAutomationEntries(DateTime automationDate, EcgServiceEnums.DataRequestType dataRequestType, string createdByProcessName)
        {
            return new EventAutomationEntry
            {
                CreatedAt = DateTime.Now,
                CreatedBy = createdByProcessName,
                DataRequestTypeId = (int)dataRequestType,
                AutomationDate = automationDate
            };
        }

        public void CreateEventsForCem(EcgServiceEnums.DataRequestType dataRequestType, DateTime automationDateTime, int dayCounter)
        {
            if (dataRequestType != EcgServiceEnums.DataRequestType.Timed)
            {
                return;
            }

            if (this._atlasConfiguration.AreTimedEventsEnabled(dayCounter))
            {
                this.CreateTimedEvent(automationDateTime);
            }
        }

        protected void CreateEventsForNonCem(EcgServiceEnums.DataRequestType dataRequestType, DateTime automationDateTime, int dayCounter)
        {
            switch (dataRequestType)
            {
                case EcgServiceEnums.DataRequestType.Telemed:
                    if (this._atlasConfiguration.EnableEctopyCounts)
                    {
                        this.CreateTelemedEvent(automationDateTime);
                    }
                    break;

                case EcgServiceEnums.DataRequestType.Timed:
                    if (this._atlasConfiguration.AreTimedEventsEnabled(dayCounter))
                    {
                        this.CreateTimedEvent(automationDateTime);
                    }
                    break;

                case EcgServiceEnums.DataRequestType.MinimumHr:
                    this.CreateGenericEvent(automationDateTime, dataRequestType);
                    break;

                case EcgServiceEnums.DataRequestType.MaximumHr:
                    this.CreateGenericEvent(automationDateTime, dataRequestType);
                    break;
            }
        }

        protected void CreateGenericEvent(DateTime automationDate, EcgServiceEnums.DataRequestType dataRequestType)
        {
            this.EventAutomationEntries.Add(this.CreateEventAutomationEntries(automationDate, dataRequestType, "ProcessName"));
        }

        protected void CreateTelemedEvent(DateTime automationDate)
        {
            this.EventAutomationEntries.Add(this.CreateEventAutomationEntries(automationDate.Next2Am(), EcgServiceEnums.DataRequestType.Telemed, "ProcessName"));

            if (automationDate.AddDays(1) >= this._patientEnrollmentDates.EndDate)
            {
                this.EventAutomationEntries.Add(this.CreateEventAutomationEntries(automationDate.Next2Am().AddDays(1), EcgServiceEnums.DataRequestType.Telemed, "ProcessName"));
            }
        }

        protected void CreateTimedEvent(DateTime automationDate)
        {
            var hourIncrementor = this._atlasConfiguration.CalculateTimedStripsHourIncrementor;
            var timedStripHour = this._atlasConfiguration.TimedStripHour;
            for (var i = 0; i < this._atlasConfiguration.TimedStripsPerDay; i++)
            {
                this.CreateGenericEvent(automationDate.AddHours(timedStripHour), EcgServiceEnums.DataRequestType.Timed);
                timedStripHour += hourIncrementor;
            }
        }
    }
}