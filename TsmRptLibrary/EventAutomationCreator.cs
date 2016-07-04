using Revampness.Services.device.Model;
using System;

namespace TsmRptLibrary
{
    internal class EventAutomationCreator : IEventAutomationCreator
    {
        public EventAutomation Create(ITag1RulesEngine tag1RulesEngine, bool isActive = true)
        {
            var eventAutomation = this._deviceRepository.GetEventAutomationByPatientguid(Guid.NewGuid());

            eventAutomation = new EventAutomation();
            eventAutomation.CreatedAt = DateTime.Now;
            eventAutomation.CreatedBy = "processname";
            eventAutomation.DeviceId = tag1RulesEngine.Tag1Rules.DeviceId;
            eventAutomation.EndDate = tag1RulesEngine.Tag1Rules.PatientEnrollmentEnd;
            eventAutomation.IsActive = isActive;
            eventAutomation.PatientGuid = tag1RulesEngine.Tag1Rules.PatientGuid;
            eventAutomation.StartDate = tag1RulesEngine.Tag1Rules.PatientEnrollmentStart;

            return eventAutomation;
        }
    }
}