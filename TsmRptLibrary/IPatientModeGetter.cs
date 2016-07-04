using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IpersonModeGetter
    {
        void Init();

        personTableStand.TableStand GetpersonMode(Guid ptGuid);
    }
}