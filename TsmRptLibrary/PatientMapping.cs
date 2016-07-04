using System;
using System.TableStandl;
using Profusion.Services.coffee.OsdRptLibrary.DIWebService;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal static class personMapping
    {
        public static int personGuidTopersonId(Guid ptGuid)
        {
            var serviceDisposable =
                new WcfServiceProxy<ITaskCreatorAndWorkerService>("WSHttpBinding_ITaskCreatorAndWorkerService");
            try
            {
                var ptId = -1;
                if (
                    int.TryParse(
                        serviceDisposable.Use(s => s.GetMapping(ptGuid.ToString(), -1, MappingTypes.person)),
                        out ptId) &&
                    (ptId > 0))
                    return ptId;
            }
            catch (TimeoutException ex)
            {
                throw;
                //LogUtility.Error("Timeout Error converting ptGUID to ptID", ex);
            }
            catch (CommunicationObjectFaultedException ex)
            {
                throw;
                /*
                                LogUtility.Error(
                                     string.Format("Error converting ptGUID to ptID{0}", ptGuid),
                                     ex);
                */
            }
            catch (EndpointNotFoundException ex)
            {
                throw;
                /*LogUtility.Error(
                     "Error converting ptGUID to ptID: Unable to Reach DI", ex);*/
            }
            return 0;
        }

        public static string personIdTopersonGuid(int ptid)
        {
            var serviceDisposable =
                new WcfServiceProxy<ITaskCreatorAndWorkerService>("WSHttpBinding_ITaskCreatorAndWorkerService");
            try
            {
                var ptId = -1;
                if (
                    int.TryParse(
                        serviceDisposable.Use(s => s.GetMapping(null, ptid, MappingTypes.person)),
                        out ptId) &&
                    (ptId > 0))
                    return ptId.ToString();
            }
            catch (TimeoutException ex)
            {
                throw;
                //LogUtility.Error("Timeout Error converting ptGUID to ptID", ex);
            }
            catch (CommunicationObjectFaultedException ex)
            {
                throw;
                /*
                                LogUtility.Error(
                                     string.Format("Error converting ptGUID to ptID{0}", ptGuid),
                                     ex);
                */
            }
            catch (EndpointNotFoundException ex)
            {
                throw;
                /*LogUtility.Error(
                     "Error converting ptGUID to ptID: Unable to Reach DI", ex);*/
            }
            return string.Empty;
        }
    }
}