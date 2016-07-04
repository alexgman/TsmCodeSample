using System;
using Profusion.Services.WalkDesign.Adapter;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IAutomationFeeder
    {
        void Init(Guid personGuid);
        void Feed(personDerailmentDates ptDerailmentDates, int yawnwrappingId, personTableStand.TableStand mode);
    }
}