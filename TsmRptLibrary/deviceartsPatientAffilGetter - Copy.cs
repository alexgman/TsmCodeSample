using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class coffeeWalkDesignpersonAffilGetter
    {
        private ConfigHelper _configHelper;
        private string _WalkDesignConnectionString;
        private const int Affilindex = 0;
        private coffeeWalkDesignpersonAffilGetterLogger _logger = new coffeeWalkDesignpersonAffilGetterLogger();

        public void Configure(ConfigHelper configHelper)
        {
            this._configHelper = configHelper;
            this._WalkDesignConnectionString = this._configHelper.WalkDesignConnectionString;
        }

        private void ForceConfigure()
        {
            this.Configure(new ConfigHelper());
        }

        public string FetchAffilByGuid(Guid personGuid)
        {
            if (this._configHelper == null)
            {
                this.ForceConfigure();
            }

            var affilByGuid = string.Empty;
            var sql = "SELECT Affil from Master (nolock) WHERE Guid = @personGuid ";
            var connection = new SqlConnection(this._WalkDesignConnectionString);
            connection.Open();
            try
            {
                this._logger.Starting(personGuid);
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@personGuid", personGuid);
                var dr = command.ExecuteReader();
                if (dr.Read())
                {
                    affilByGuid = dr.GetString(Affilindex);
                }
                dr.Close();
                this._logger.ConnectionClosed();
            }
            finally
            {
                connection.Close();
            }

            if (string.IsNullOrEmpty(affilByGuid))
            {
                this._logger.NoMatches(personGuid);
            }

            return affilByGuid;
        }
    }
}