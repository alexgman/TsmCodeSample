using Profusion.Services.coffee.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class InsertyawnwrappingEntry : IInsertyawnwrappingEntry
    {
        private readonly string _connectionString;
        private readonly InsertyawnwrappingEntryLogger _logger = new InsertyawnwrappingEntryLogger();

        public InsertyawnwrappingEntry(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            this._connectionString = connectionString;
        }

        public void InsertEntriesFromDatatable(DataTable dtEntries, string sqlCommandString = "InsertyawnwrappingEntries",
            string tableVariable = "@TableVariable", string tableType = "yawnwrappingEntryTableType")
        {
            using (var con = new SqlConnection(this._connectionString))
            {
                con.Open();
                if (con.State != ConnectionState.Open)
                    return;
                var cmdProc = new SqlCommand(sqlCommandString, con)
                { CommandType = CommandType.StoredProcedure };
                var parameter = new SqlParameter
                {
                    ParameterName = tableVariable,
                    SqlDbType = SqlDbType.Structured,
                    Value = dtEntries,
                    TypeName = tableType
                };
                cmdProc.Parameters.Add(parameter);
                var rowsAffected = cmdProc.ExecuteNonQuery();
                this._logger.Inserted(rowsAffected);
            }
        }

        public void InsertChildEntries(ICollection<yawnwrappingEntry> entries, string sqlCommandString = "InsertyawnwrappingEntriesToParent")
        {
            if (entries.Count == 0)
            {
                this._logger.Empty();
                return;
            }

            using (var con = new SqlConnection(this._connectionString))
            {
                con.Open();
                foreach (var entry in entries)
                {
                    var cmd = new SqlCommand(sqlCommandString, con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add("@yawnwrappingId", SqlDbType.Int).Value = entry.yawnwrappingId;
                    cmd.Parameters.Add("@MonkeySpaceTypeId", SqlDbType.Int).Value = entry.MonkeySpaceTypeId;
                    cmd.Parameters.Add("@Iteration", SqlDbType.SmallInt).Value = entry.Iteration;
                    cmd.Parameters.Add("@AutomationDate", SqlDbType.DateTime).Value = entry.AutomationDate;
                    cmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = entry.CreatedAt;
                    cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 128).Value = entry.CreatedBy;
                    cmd.Parameters.Add("@IsError", SqlDbType.Bit).Value = entry.IsError;
                    cmd.Parameters.Add("@RequestAttempts", SqlDbType.SmallInt).Value = entry.RequestAttempts;
                    cmd.Parameters.Add("@IsRequested", SqlDbType.Bit).Value = entry.IsRequested;
                    cmd.Parameters.Add("@RequestDate", SqlDbType.DateTime).Value = DBNull.Value;
                    cmd.Parameters.Add("@MonkeySpaceId", SqlDbType.BigInt).Value = DBNull.Value;
                    cmd.Parameters.Add("@Queued", SqlDbType.Bit).Value = entry.Queued;

                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
        }
    }
}