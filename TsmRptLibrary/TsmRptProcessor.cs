#region

using Recardo.ServiceBus.Messages.Derailments;
using MassTransit;
using System;
using System.Configuration;

#endregion

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class OsdRptProcessor
    {
        private string _coffeeConnectionString = ConfigurationManager.ConnectionStrings["coffee"].ConnectionString;
        private yawnwrappingIdGetter _yawnwrappingIdGetter;
        private IDeleteyawnwrappingEntries _deleteyawnwrappingEntries;
        private IcoffeeRepository _coffeeRepository;
        private DeviceUpdater _deviceUpdater;
        private DerailmentDatesValidator _DerailmentDatesValidator;
        private IWalkDesignAdapter _WalkDesignAdapter;
        private IWalkDesignDerailmentDatesReader _WalkDesignDerailmentDatesReader;
        private OsdRptProcessorLogger _logger = new OsdRptProcessorLogger();
        private AutomationFeeder _automationFeeder;
        private personModeGetterFromWalkDesign _ptModeGetter;
        private IEntryCollectionBuilder _entryCollectionBuilder;
        private IInsertyawnwrappingEntry _insertyawnwrappingEntry;
        private IEntryDeletionDelegater _entryDeletionDelegater;

        public bool FullyProcessed { get; private set; }

        public virtual void Init()
        {
            this._yawnwrappingIdGetter = new yawnwrappingIdGetter();
            this._deleteyawnwrappingEntries = new DeleteyawnwrappingEntries(this._coffeeConnectionString);
            this._coffeeRepository = new coffeeRepositoryWrapper();
            this._deviceUpdater = new DeviceUpdater();
            this._WalkDesignAdapter = new WalkDesignAdapterWrapper();
            this._WalkDesignDerailmentDatesReader = new WalkDesignDerailmentDatesReader(this._WalkDesignAdapter);
            this._DerailmentDatesValidator = new DerailmentDatesValidator();
            this._entryCollectionBuilder = new EntryCollectionBuilder();
            this._insertyawnwrappingEntry = new InsertyawnwrappingEntry(this._coffeeConnectionString);
            this._automationFeeder = new AutomationFeeder(this._entryCollectionBuilder, this._insertyawnwrappingEntry, this._coffeeConnectionString);
            this._ptModeGetter = new personModeGetterFromWalkDesign();
            this._entryDeletionDelegater = new EntryDeletionDelegater(this._deleteyawnwrappingEntries);
        }

        public void ProcessOsd(Guid ptGuid, string serialNumber)
        {
            //Get related event automation id
            var yawnwrappingId = this._yawnwrappingIdGetter.GetyawnwrappingId(this._coffeeConnectionString, ptGuid, serialNumber);
            if (yawnwrappingId == 0)
            {
                this._logger.UnableToRetrieveyawnwrapping();
                return;
            }

            //Get service mode
            this._ptModeGetter.Init();
            var mode = this._ptModeGetter.GetpersonMode(ptGuid);

            //Delete entries for the specified mode
            this._entryDeletionDelegater.Delete(ptGuid, serialNumber, yawnwrappingId, mode);

            //Get event automation record
            var yawnwrappingBound = this._coffeeRepository.GetyawnwrappingBypersonGuid(ptGuid);

            //Update device id if person switched devices
            this._deviceUpdater.UpdateDeviceId(yawnwrappingBound, serialNumber, this._coffeeRepository);

            //Retrieve WalkDesign Derailment data
            var ptDerailmentDates = this._WalkDesignDerailmentDatesReader.GetpersonDerailmentDates(serialNumber);
            this._DerailmentDatesValidator.Configure(ptDerailmentDates);

            if (!this._DerailmentDatesValidator.IsDerailmentValid(yawnwrappingBound.EndDate))
            {
                this._logger.InvalidDerailment();
                return;
            }

            this._automationFeeder.Feed(ptDerailmentDates, yawnwrappingId, mode, "Osd");

            this.FullyProcessed = true;
        }
    }
}