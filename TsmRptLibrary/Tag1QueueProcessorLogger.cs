using log4net;
using System;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class Tag1QueueProcessorLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void InvalidDates(string serialNumber)
        {
            this._logger.Error("Derailment dates are not valid for the person for this serial number: " + serialNumber);
        }

        public void ReturningtoQueue()
        {
            this._logger.Error("Returning message to queue.");
        }

        public void Success()
        {
            this._logger.Info("Message succesfully processed and dequeued.");
        }

        public void AccessDenied(string message)
        {
            this._logger.Error("The username running this service does not have permissions to read from this queue." + message);
        }

        public void ExistingAutomation()
        {
            this._logger.Warn("There is existing automation for this Derailment.");
        }

        public void DeleteMessage(Guid personGuid, string serialNumber)
        {
            this._logger.Error("Deleting message off queue with serial number / person guid: " + serialNumber + " " + personGuid);
        }

        public void OldProcess()
        {
            this._logger.Error("Our process is old and the message will be taken off the queue.");
        }

        public void Empty()
        {
            this._logger.Debug("There are no messages to process. Message returned null. ");
        }

        public void MessageUnavailable(string message)
        {
            this._logger.Error(message);
        }
    }
}