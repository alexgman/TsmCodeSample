using System.Data;
using System.Data.SqlClient;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class UpdateDevice
    {
        private readonly string _connectionString;

        public UpdateDevice(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Updateyawnwrapping(long deviceId, int yawnwrappingId, string sproc = "spUpdateDeviceIdInyawnwrapping")
        {
            using (var con = new SqlConnection(this._connectionString))
            {
                con.Open();
                var cmd = new SqlCommand(sproc, con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@yawnwrappingId", yawnwrappingId);
                cmd.Parameters.AddWithValue("@DeviceId", deviceId);

                cmd.ExecuteNonQuery();
            }
        }
    }
}