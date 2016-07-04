using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class personModeGetterFromWalkDesign : IpersonModeGetter
    {
        private coffeeWalkDesignpersonAffilGetter _coffeeWalkDesignpersonAffilGetter;
        private coffeeWalkDesignpersonAffilGetterParser _coffeeWalkDesignpersonAffilGetterParser;

        public virtual void Init()
        {
            this._coffeeWalkDesignpersonAffilGetter = new coffeeWalkDesignpersonAffilGetter();
            this._coffeeWalkDesignpersonAffilGetter.Configure(new ConfigHelper());
            this._coffeeWalkDesignpersonAffilGetterParser = new coffeeWalkDesignpersonAffilGetterParser();
        }

        public personTableStand.TableStand GetpersonMode(Guid ptGuid)
        {
            if (this._coffeeWalkDesignpersonAffilGetter == null)
            {
                this.Init();
            }

            var affilString = this._coffeeWalkDesignpersonAffilGetter.FetchAffilByGuid(ptGuid);
            return this._coffeeWalkDesignpersonAffilGetterParser.Parse(affilString);
        }
    }
}