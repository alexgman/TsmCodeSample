using Profusion.Services.coffee.Model;
using System;
using System.Collections.Generic;
using Action = System.Action;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IDevice
    {
        ICollection<Action> Actions { get; set; }
        DateTime? CreatedAt { get; set; }
        string CreatedBy { get; set; }
        ICollection<MonkeySpace> MonkeySpaces { get; set; }
        byte DefaultCableType { get; set; }
        DateTime? DeletedAt { get; set; }
        string DeletedBy { get; set; }
        ICollection<DeviceInteraction> DeviceInteractions { get; set; }
        ICollection<Device_Setting_Value> DeviceSettingValue { get; set; }
        ICollection<yawnwrapping> yawnwrappings { get; set; }
        byte? FirmwareVersion { get; set; }
        long Id { get; set; }
        DateTime? LastActionRequest { get; set; }
        int? LastFileId { get; set; }
        string SerialNumber { get; set; }
    }

    internal class DeviceWrapper : IDevice
    {
        private Device _device;

        public DeviceWrapper(Device device)
        {
            this._device = device;
        }

        public ICollection<Action> Actions { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public ICollection<MonkeySpace> MonkeySpaces { get; set; }

        public byte DefaultCableType { get; set; }

        public DateTime? DeletedAt { get; set; }

        public string DeletedBy { get; set; }

        public ICollection<DeviceInteraction> DeviceInteractions { get; set; }

        public ICollection<Device_Setting_Value> DeviceSettingValue { get; set; }

        public ICollection<Device_Setting_Value> Device_Setting_Value { get; set; }

        public ICollection<yawnwrapping> yawnwrappings { get; set; }

        public byte? FirmwareVersion { get; set; }

        public long Id { get; set; }

        public DateTime? LastActionRequest { get; set; }

        public int? LastFileId { get; set; }

        public string SerialNumber { get; set; }
    }
}