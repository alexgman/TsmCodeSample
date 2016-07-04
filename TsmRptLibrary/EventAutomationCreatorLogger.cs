using log4net;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class yawnwrappingCreatorLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}