using System;
using RepairCenter.Enums;
using RepairCenter.Events;
using RepairCenter.Models;
using RepairCenter.Services;

namespace RepairCenter
{
    internal class Program
    {
        private static void Main()
        {
            var shop = new RepairShop();

            // Subscribe to the events
            shop.DeviceStatusChanged += OnDeviceStatusChanged;
            shop.DeliveryOverdue += OnDeliveryOverdue;

            DateTime today = DateTime.Today;

            // Three different appliance types handled through the same base class
            shop.AddDevice(new Refrigerator("RF-10001", "Ahmad Zaitoun", "Samsung",
                "Not cooling", today.AddDays(-2), today.AddDays(3), 80m, 2, 600, true));

            shop.AddDevice(new WashingMachine("WM-20002", "Mohammad Hadid", "LG",
                "Drum does not spin", today.AddDays(-10), today.AddDays(-3), 45m, 3, 8, true));

            shop.AddDevice(new AirConditioner("AC-30003", "Raslan Hafez", "Gree",
                "Weak cooling", today.AddDays(-1), today.AddDays(5), 25m, 1, 18000, true, true));

            Console.WriteLine("=== Devices after reception ===");
            shop.PrintAllDevices();

            Console.WriteLine("\n=== Checking overdue devices ===");
            shop.CheckOverdueDevices(today);

            Console.WriteLine("\n=== Repair workflow ===");
            shop.FindBySerial("RF-10001").StartRepair();
            shop.FindBySerial("RF-10001").CompleteRepair();
            shop.FindBySerial("AC-30003").StartRepair();

            Console.WriteLine("\n=== Statistics (static properties) ===");
            Console.WriteLine($"Total devices : {Appliance.TotalDevices}");
            Console.WriteLine($"Under repair  : {Appliance.UnderRepairCount}");
            Console.WriteLine($"Repaired      : {Appliance.RepairedCount}");
            Console.WriteLine($"Repaired total cost: {shop.GetTotalCost(DeviceStatus.Repaired):C}");

            Console.WriteLine("\n=== Invalid serial number test ===");
            try
            {
                shop.AddDevice(new Refrigerator("bad-serial", "Test", "Test",
                    "Test", today, today, 0m, 0, 300, false));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Rejected: " + ex.Message);
            }
        }

        private static void OnDeviceStatusChanged(object sender, DeviceStatusChangedEventArgs e)
        {
            Console.WriteLine($"[EVENT] {e.Device.SerialNumber}: {e.OldStatus} -> {e.NewStatus} ({e.Message})");
        }

        private static void OnDeliveryOverdue(object sender, DeviceStatusChangedEventArgs e)
        {
            Console.WriteLine($"[ALERT] {e.Device.SerialNumber} ({e.Device.DeviceType}): {e.Message}");
        }
    }
}
