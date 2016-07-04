using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class EntryDeletionDelegater : IEntryDeletionDelegater
    {
        private IDeleteyawnwrappingEntries _deleteyawnwrappingEntries;
        private EntryDeletionDelegaterLogger _logger = new EntryDeletionDelegaterLogger();

        public EntryDeletionDelegater(IDeleteyawnwrappingEntries deleteyawnwrappingEntries)
        {
            this._deleteyawnwrappingEntries = deleteyawnwrappingEntries;
        }

        public void Delete(Guid ptGuid, string serialNumber, int yawnwrappingId, personTableStand.TableStand mode)
        {
            //Delete existing child entries based on person service mode
            if (mode == personTableStand.TableStand.Cem)
            {
                this._deleteyawnwrappingEntries.DeleteAllNonTimedEntries(serialNumber, ptGuid);
            }
            else
            {
                if (mode != personTableStand.TableStand.Unknown)
                {
                    this._deleteyawnwrappingEntries.DeleteAllChildEntries(yawnwrappingId);
                }
                else
                {
                    this._logger.UnSupported(mode);
                }
            }
        }
    }
}