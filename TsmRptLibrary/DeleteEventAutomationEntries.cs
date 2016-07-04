using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class DeleteyawnwrappingEntries : IDeleteyawnwrappingEntries
    {
        private readonly string _connectionString;

        private readonly DeleteyawnwrappingEntriesLogger _logger =
        new DeleteyawnwrappingEntriesLogger();

        public DeleteyawnwrappingEntries(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException("Configuration string for 'coffee' is not found.");
            }

            this._connectionString = connectionString;
        }

        public void DeleteAllChildEntries(int yawnwrappingId, string sproc = "spDeleteyawnwrappingEntries")
        {
            if (yawnwrappingId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(yawnwrappingId));
            }

            SqlConnection conn = null;
            SqlCommand cmd = null;

            try
            {
                conn = new SqlConnection(this._connectionString);
                cmd = new SqlCommand(sproc, conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@yawnwrappingId", yawnwrappingId);
                this._logger.Executing(sproc);
                conn.Open();
                this._logger.Deleted(cmd.ExecuteNonQuery());
            }
            catch (SqlException sqlException)
            {
                this._logger.SprocInvalid("The stored procedure: " + sproc + " does not exist." + sqlException.Message);
                throw;
            }
            catch (Exception e)
            {
                this._logger.ErrorDeleting(e.Message);
                throw;
            }
            finally
            {
                conn?.Dispose();
                cmd?.Dispose();
            }
        }

        public void DeleteAllNonTimedEntries(string serialNumber, Guid ptGuid, string sproc = "spDeleteNonTimedyawnwrappingEntries")
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                throw new ArgumentOutOfRangeException(nameof(serialNumber));
            }

            if (ptGuid == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(ptGuid));
            }

            SqlConnection conn = null;
            SqlCommand cmd = null;

            try
            {
                conn = new SqlConnection(this._connectionString);
                cmd = new SqlCommand(sproc, conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@personguid", ptGuid);
                cmd.Parameters.AddWithValue("@serialnumber", serialNumber);
                this._logger.Executing(sproc);
                conn.Open();
                this._logger.Deleted(cmd.ExecuteNonQuery());
            }
            catch (SqlException sqlException)
            {
                this._logger.SprocInvalid("The stored procedure: " + sproc + " does not exist." + sqlException.Message);
                throw;
            }
            catch (Exception e)
            {
                this._logger.ErrorDeleting(e.Message);
                throw;
            }
            finally
            {
                conn?.Dispose();
                cmd?.Dispose();
            }
        }
    }
}