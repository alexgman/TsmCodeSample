using System;
using System.Runtime.CompilerServices;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class coffeeWalkDesignpersonAffilGetterParser
    {
        private readonly coffeeWalkDesignpersonAffilGetterParserLogger _coffeeWalkDesignpersonAffilGetterParserLogger = new coffeeWalkDesignpersonAffilGetterParserLogger();

        public personTableStand.TableStand Parse(string affilString)
        {
            var mode = personTableStand.TableStand.Unknown;

            if (affilString.IsEquivalentTo("Mct"))
            {
                mode = personTableStand.TableStand.Mct;
            }
            else if (affilString.IsEquivalentTo("Cem"))
            {
                mode = personTableStand.TableStand.Cem;
            }
            else
            {
                this._coffeeWalkDesignpersonAffilGetterParserLogger.Unsupported(affilString);
            }

            this._coffeeWalkDesignpersonAffilGetterParserLogger.CurrentMode(affilString);

            return mode;
        }
    }
}