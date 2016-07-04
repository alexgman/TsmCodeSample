using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IEntryDeletionDelegater
    {
        void Delete(Guid ptGuid, string serialNumber, int yawnwrappingId, personTableStand.TableStand mode);
    }
}