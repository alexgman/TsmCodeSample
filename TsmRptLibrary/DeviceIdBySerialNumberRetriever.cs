using System;
using System.Data;
using System.Data.SqlClient;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class DeviceIdBySerialNumberRetriever
    {
        private readonly DeviceIdBySerialNumberRetrieverLogger _logger = new DeviceIdBySerialNumberRetrieverLogger();
        private readonly string _coffeeConnection;

        public DeviceIdBySerialNumberRetriever(string coffeeConnection)
        {
            this._coffeeConnection = coffeeConnection;
        }

        public long GetDeviceBySerialNumber(string serialNumber, string sproc = "spGetDeviceBySerialNumber")
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                this._logger.EmptySerialNumber();
                throw new ApplicationException();
            }

            long deviceId = 0;

            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                connection = new SqlConnection(this._coffeeConnection);
                command = new SqlCommand(sproc, connection);

                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SerialNumber", serialNumber);
                var dr = command.ExecuteReader();
                if (dr.Read())
                {
                    deviceId = dr.GetInt64(0);
                }

                dr.Close();

                this._logger.DeviceIdIs(deviceId);
            }
            catch (SqlException sqlException)
            {
                this._logger.SprocInvalid("The stored procedure: " + sproc + " does not exist." + sqlException.Message);
                throw;
            }
            catch (Exception e)
            {
                this._logger.CannotGetId(e.Message);
                throw;
            }
            finally
            {
                connection?.Dispose();
                command?.Dispose();
            }

            return deviceId;
        }
    }
}