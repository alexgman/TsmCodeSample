using System;

namespace TsmRptLibrary
{
    internal static class DiService
    {
        public static string PatientGuidToPatientId(Guid patientGuid)
        {
            var serviceDisposable = new WcfServiceProxy<ITaskCreatorAndWorkerService>("WSHttpBinding_ITaskCreatorAndWorkerService");
            try
            {
                var patientId = -1;
                if (int.TryParse(serviceDisposable.Use(s => s.GetMapping(patientGuid.ToString(), -1, MappingTypes.Patient)), out patientId) && (patientId > 0)) return patientId.ToString();
            }
            catch (TimeoutException)
            {
                throw;
                //LogUtility.Error("Timeout Error converting patientGUID to patientID", ex);
            }
            catch (CommunicationObjectFaultedException)
            {
                throw;
                /*
                                LogUtility.Error(
                                     string.Format("Error converting patientGUID to patientID{0}", patientGuid),
                                     ex);
                */
            }
            catch (EndpointNotFoundException)
            {
                throw;
                /*LogUtility.Error(
                     "Error converting patientGUID to patientID: Unable to Reach DI", ex);*/
            }
            return string.Empty;
        }

        public static string PatientIdToPatientGuid(int patientid)
        {
            var serviceDisposable = new WcfServiceProxy<ITaskCreatorAndWorkerService>("WSHttpBinding_ITaskCreatorAndWorkerService");
            try
            {
                var patientId = -1;
                if (int.TryParse(serviceDisposable.Use(s => s.GetMapping(null, patientid, MappingTypes.Patient)), out patientId) && (patientId > 0)) return patientId.ToString();
            }
            catch (TimeoutException)
            {
                throw;
                //LogUtility.Error("Timeout Error converting patientGUID to patientID", ex);
            }
            catch (CommunicationObjectFaultedException)
            {
                throw;
                /*
                                LogUtility.Error(
                                     string.Format("Error converting patientGUID to patientID{0}", patientGuid),
                                     ex);
                */
            }
            catch (EndpointNotFoundException)
            {
                throw;
                /*LogUtility.Error(
                     "Error converting patientGUID to patientID: Unable to Reach DI", ex);*/
            }
            return string.Empty;
        }
    }
}