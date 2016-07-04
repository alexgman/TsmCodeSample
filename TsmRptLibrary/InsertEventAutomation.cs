using System;
using System.Data;
using System.Data.SqlClient;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class Insertyawnwrapping : IInsertyawnwrapping
    {
        private readonly string _connectionString;
        private readonly InsertyawnwrappingLogger _logger = new InsertyawnwrappingLogger();

        public Insertyawnwrapping(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this._connectionString = connectionString;
        }

        public int InsertIntoyawnwrapping(yawnwrappingDto yawnwrappingDto, string sproc = "spInsertyawnwrapping")
        {
            using (var con = new SqlConnection(this._connectionString))
            {
                con.Open();
                var cmd = new SqlCommand(sproc, con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@personGuid", yawnwrappingDto.PtGuid);
                cmd.Parameters.AddWithValue("@DeviceId", yawnwrappingDto.DeviceId);
                cmd.Parameters.AddWithValue("@StartDate", yawnwrappingDto.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", yawnwrappingDto.EndDate);
                cmd.Parameters.AddWithValue("@CreatedAt", yawnwrappingDto.CreatedAt);
                cmd.Parameters.AddWithValue("@CreatedBy", yawnwrappingDto.CreatedBy);
                cmd.Parameters.AddWithValue("@IsActive", yawnwrappingDto.IsActive);

                this._logger.Inserting(yawnwrappingDto);
                var myReturnedId = int.Parse(cmd.ExecuteScalar().ToString());
                this._logger.Inserted(myReturnedId);

                return myReturnedId;
            }
        }
    }
}