using System;
using System.Data;
using System.Data.SqlClient;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class yawnwrappingChecker : IyawnwrappingChecker
    {
        private readonly string _connectionString;
        private readonly yawnwrappingCheckerLogger _logger = new yawnwrappingCheckerLogger();

        public yawnwrappingChecker(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new EmptyConnectionStringException("The connection string for the event automation checker is empty");
            }

            this._connectionString = connectionString;
        }

        public int GetLastyawnwrappingId(Guid ptGuid, string serialNumber)
        {
            if (ptGuid == Guid.Empty)
            {
                throw new MissingptGuidForCurrentSerialException("pt Guid not found for serial number: " + serialNumber);
            }

            if (string.IsNullOrEmpty(serialNumber))
            {
                throw new SerialNumberIsEmptyForCurrentpersonException("There is not an associated serial number for pt: " + ptGuid);
            }

            return this.GetyawnwrappingIdForSerialNumberperson(ptGuid, serialNumber);
        }

        private int GetyawnwrappingIdForSerialNumberperson(Guid ptGuid, string serialNumber,
            string sproc = "spGetyawnwrappingIDForSerialNumberperson")
        {
            using (var con = new SqlConnection(this._connectionString))
            {
                using (var cmd = new SqlCommand(sproc, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@serialnumber", SqlDbType.NVarChar, 7).Value = serialNumber;
                    cmd.Parameters.Add("@personguid", SqlDbType.UniqueIdentifier).Value = ptGuid;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    var yawnwrappingId = Convert.ToInt32(cmd.Parameters["@Id"].Value);
                    con.Close();

                    this._logger.yawnwrappingIdForpersonGuid(ptGuid, serialNumber, yawnwrappingId);
                    return yawnwrappingId;
                }
            }
        }

        private Guid GetLastpersonGuidForDevice(string serialNumber, string sproc = "spGetLastptGuidForDevice")
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                throw new ArgumentOutOfRangeException(serialNumber);
            }

            using (var con = new SqlConnection(this._connectionString))
            {
                using (var cmd = new SqlCommand(sproc, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@serialnumber", serialNumber);

                    con.Open();

                    using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            return rdr.GetGuid(0);
                        }
                    }
                }
            }

            return Guid.Empty;
        }
    }
}