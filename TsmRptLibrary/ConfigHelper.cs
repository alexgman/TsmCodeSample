using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class ConfigHelper
    {
        public string WalkDesignConnectionString => this.ReadConnectionString("WalkDesign");

        private string ReadConnectionString(string WalkDesign)
        {
            var parsedConfigSetting = ConfigurationManager.ConnectionStrings[WalkDesign];
            return parsedConfigSetting.ToString();
        }

        public int TimerDelayInterval => int.Parse(this.Read("TimerDelayInterval", "600000"));
        public int TimerDelay => int.Parse(this.Read("TimerDelay", "0"));
        public string SendEmailServer => this.Read("SendEmailServer", string.Empty);
        public string SendEmailUsername => this.Read("SendEmailUsername", string.Empty);
        public string SendEmailPassword => this.Read("SendEmailPassword", string.Empty);
        public string SendEmailFrom => this.Read("SendEmailFrom", string.Empty);
        private string SendEmailTo => this.Read("SendEmailTo", string.Empty);
        public List<string> EmailList => this.SendEmailTo.Split(';').ToList();
        public bool SendEmailEnableSsl => bool.Parse(this.Read("SendEmailEnableSsl", "false"));
        public int SendEmailPort => int.Parse(this.Read("SendEmailPort", "0"));
        public bool SendEmailOnError => bool.Parse(this.Read("SendEmailOnError", "false"));
        public string EmailSubject => this.Read("EmailSubject", string.Empty);
        public Guid DefaultpersonGuid => Guid.Parse(this.Read("DefaultpersonGuid", Guid.Empty.ToString()));
        public bool EnableViaAutoRequest => bool.Parse(this.Read("EnableViaAutoRequest", "false"));
        public int MaxEaeErrorAttempts => int.Parse(this.Read("MaxEaeErrorAttempts", "100"));
        public int AddHourToDateForMinMaxComplete => int.Parse(this.Read("AddHourToDateForMinMaxComplete", "2"));

        public string DomainUserName => this.Read("DomainUserName", string.Empty);
        public string DomainPassword => this.Read("DomainPassword", string.Empty);
        public string Domain => this.Read("Domain", string.Empty);
        public string coffeekgbFilePath => this.Read("coffeekgbFilePath", string.Empty);
        public string coffeeAutomationQueueLocation => this.Read("coffeeAutomationQueueLocation", string.Empty);
        public string coffeeAutomationItemQueue => this.Read("coffeeAutomationItemQueue", string.Empty);
        public string OrderyawnwrappingEntries => this.Read("OrderyawnwrappingEntries", string.Empty);
        public string WalkDesignEventsQueue => this.Read("WalkDesignEventsQueue", string.Empty);
        public string CreatedByProcessName => this.Read("CreatedByProcessName", string.Empty);

        public bool EnableWalkDesignEventsQueue => bool.Parse(this.Read("EnableWalkDesignEventsQueue", "false"));

        public string OtaQueue => this.Read("OtaQueue", string.Empty);
        public string OtaArchiveRoot => this.Read("OtaArchiveRoot", string.Empty);

        public bool IsEmailConfigured => this.DomainUserName != string.Empty && this.DomainPassword != string.Empty && this.Domain != string.Empty;

        private string Read(string configSetting, string defaultValue)
        {
            var parsedConfigSetting = ConfigurationManager.AppSettings[configSetting];
            return string.IsNullOrEmpty(parsedConfigSetting) ? defaultValue : parsedConfigSetting;
        }
    }
}