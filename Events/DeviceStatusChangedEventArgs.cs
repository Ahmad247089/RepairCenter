using System;
using RepairCenter.Enums;
using RepairCenter.Models;

namespace RepairCenter.Events
{
    /// <summary>
    /// Data sent with notifications about a device (status change or overdue delivery).
    /// </summary>
    public class DeviceStatusChangedEventArgs : EventArgs
    {
        public Appliance Device { get; }
        public DeviceStatus OldStatus { get; }
        public DeviceStatus NewStatus { get; }
        public string Message { get; }

        public DeviceStatusChangedEventArgs(Appliance device, DeviceStatus oldStatus,
                                            DeviceStatus newStatus, string message)
        {
            Device = device;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            Message = message;
        }
    }
}
