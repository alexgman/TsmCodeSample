using System;
using System.Configuration;
using System.Messaging;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class Tag1QueueProcessor
    {
        private DateTime _whenThisMessageProcessingStarted;
        private MessageReader _messageReader;
        private readonly string _queueLocation;
        private MessageQueueWrapper _queue;
        private yawnwrappingDtoCreatorForTag1 _creatyawnwrappingDto;
        private string _coffeeConnectionString;
        private WalkDesignDerailmentDatesReader _WalkDesignDerailmentDatesReader;
        private IWalkDesignAdapter _WalkDesignAdapter;
        private DerailmentDatesValidator _DerailmentDatesValidator;
        private Tag1QueueProcessorLogger _logger = new Tag1QueueProcessorLogger();
        private Message _message;
        private personModeGetterFromWalkDesign _personModeGetter;
        private AutomationFeeder _automationFeeder;
        private yawnwrappingIdGetter _yawnwrappingIdGetter;
        private DeleteyawnwrappingEntries _deleteyawnwrappingEntries;
        private IEntryCollectionBuilder _entryCollectionBuilder;
        private IInsertyawnwrappingEntry _insertyawnwrappingEntry;

        public Tag1QueueProcessor(string queueLocation)
        {
            this._queueLocation = queueLocation;
        }

        public virtual void Init()
        {
            this._messageReader = new MessageReader();
            this._queue = new MessageQueueWrapper(this._queueLocation);
            this._creatyawnwrappingDto = new yawnwrappingDtoCreatorForTag1();
            this._coffeeConnectionString = ConfigurationManager.ConnectionStrings["coffee"].ConnectionString;
            this._WalkDesignAdapter = new WalkDesignAdapterWrapper();
            this._WalkDesignDerailmentDatesReader = new WalkDesignDerailmentDatesReader(this._WalkDesignAdapter);
            this._personModeGetter = new personModeGetterFromWalkDesign();
            this._entryCollectionBuilder = new EntryCollectionBuilder();
            this._insertyawnwrappingEntry = new InsertyawnwrappingEntry(this._coffeeConnectionString);
            this._automationFeeder = new AutomationFeeder(this._entryCollectionBuilder, this._insertyawnwrappingEntry, this._coffeeConnectionString);
            this._yawnwrappingIdGetter = new yawnwrappingIdGetter();
            this._deleteyawnwrappingEntries = new DeleteyawnwrappingEntries(this._coffeeConnectionString);
        }

        //TODO: this needs to be pushed up to the parent process
        public void Start()
        {
            this._whenThisMessageProcessingStarted = DateTime.Now;

            try
            {
                this._message = this._messageReader.GetNextMessage(this._queue, this._queueLocation);
            }
            catch (MessageQueueException exception)
            {
                this._logger.AccessDenied(exception.Message);
                return;
            }
            catch (Exception exception)
            {
                this._logger.MessageUnavailable(exception.Message);
                return;
            }
            if (this._message == null)
            {
                this._logger.Empty();
                return;
            }

            var automationMessage = this._messageReader.ParseMessage(this._message);

            if (this.IsOurProcessOld(automationMessage.LastAttemptDate))
            {
                this._logger.OldProcess();
                this._messageReader.Receive(this._queue);
                return;
            }

            var personGuid = automationMessage.Tze.personGuid.GetValueOrDefault();
            var serialNumber = automationMessage.Tze.SerialNumber;

            var personDates = this._WalkDesignDerailmentDatesReader.GetpersonDerailmentDates(serialNumber);

            this._personModeGetter.Init();
            var mode = this._personModeGetter.GetpersonMode(personGuid);

            //Get related event automation id
            var currentyawnwrappingId = this._yawnwrappingIdGetter.GetyawnwrappingId(this._coffeeConnectionString, personGuid, serialNumber);
            if (currentyawnwrappingId != 0)
            {
                this._logger.ExistingAutomation();
                if (mode == personTableStand.TableStand.Cem)
                {
                    this._deleteyawnwrappingEntries.DeleteAllNonTimedEntries(serialNumber, personGuid);
                }
                else
                {
                    this._deleteyawnwrappingEntries.DeleteAllChildEntries(currentyawnwrappingId);
                }

                this._automationFeeder.Feed(personDates, currentyawnwrappingId, mode, "rpt");
                this._messageReader.Receive(this._queue);
                this._logger.Success();
                return;
            }

            this._creatyawnwrappingDto.Init(serialNumber, personGuid, this._coffeeConnectionString);
            var yawnwrappingDto = this._creatyawnwrappingDto.InitializeRecord(personDates.EndDate, personDates.StartDate);

            var yawnwrappingInserter = new Insertyawnwrapping(this._coffeeConnectionString);
            var yawnwrappingId = yawnwrappingInserter.InsertIntoyawnwrapping(yawnwrappingDto);

            this._automationFeeder.Feed(personDates, yawnwrappingId, mode, "rpt");

            this._messageReader.Receive(this._queue);
            this._logger.Success();
        }

        private bool IsOurProcessOld(DateTime lastAttemptDateTime)
        {
            return lastAttemptDateTime > this._whenThisMessageProcessingStarted;
        }
    }
}