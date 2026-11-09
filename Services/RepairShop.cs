using System;
using System.Collections.Generic;
using System.Linq;
using RepairCenter.Enums;
using RepairCenter.Events;
using RepairCenter.Models;

namespace RepairCenter.Services
{
    /// <summary>
    /// Main class: holds the list of appliances and processes them polymorphically.
    /// </summary>
    public class RepairShop
    {
        private readonly List<Appliance> _devices = new List<Appliance>();

        /// <summary>Raised when any device in the shop changes status.</summary>
        public event DeviceStatusChangedHandler DeviceStatusChanged;

        /// <summary>Raised when a device passes its promised delivery date.</summary>
        public event DeviceStatusChangedHandler DeliveryOverdue;

        public IReadOnlyList<Appliance> Devices => _devices.AsReadOnly();

        public void AddDevice(Appliance device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            if (FindBySerial(device.SerialNumber) != null)
                throw new InvalidOperationException($"Serial {device.SerialNumber} already exists.");

            device.StatusChanged += RelayStatusChanged;   // listen to the device
            _devices.Add(device);
        }

        private void RelayStatusChanged(object sender, DeviceStatusChangedEventArgs e)
        {
            DeviceStatusChanged?.Invoke(this, e);
        }

        public Appliance FindBySerial(string serial)
        {
            return _devices.FirstOrDefault(d => d.SerialNumber == serial);
        }

        public IEnumerable<Appliance> GetByStatus(DeviceStatus status)
        {
            return _devices.Where(d => d.Status == status);
        }

        /// <summary>Polymorphism: each device computes its own cost.</summary>
        public decimal GetTotalCost(DeviceStatus status)
        {
            decimal total = 0;
            foreach (Appliance device in GetByStatus(status))
                total += device.CalculateRepairCost();
            return total;
        }

        public void PrintAllDevices()
        {
            foreach (Appliance device in _devices)
                Console.WriteLine(device.GetSummary());
        }

        /// <summary>Fires DeliveryOverdue for every late device.</summary>
        public int CheckOverdueDevices(DateTime today)
        {
            int count = 0;
            foreach (Appliance device in _devices.Where(d => d.IsOverdue(today)))
            {
                count++;
                DeliveryOverdue?.Invoke(this, new DeviceStatusChangedEventArgs(
                    device, device.Status, device.Status,
                    $"Delivery overdue since {device.ExpectedDeliveryDate:yyyy-MM-dd}."));
            }
            return count;
        }
    }
}
