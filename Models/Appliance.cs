using System;
using System.Collections.Generic;
using RepairCenter.Enums;
using RepairCenter.Events;
using RepairCenter.Helpers;
using RepairCenter.Interfaces;

namespace RepairCenter.Models
{
    /// <summary>
    /// Abstract base class for every appliance. Implements IRepairable and holds
    /// the shared data; each subclass defines its own repair-cost rules.
    /// </summary>
    public abstract class Appliance : IRepairable
    {
        protected const decimal HourlyRate = 15m;   // labor cost per hour

        // ---------- static state (counts devices per status) ----------
        private static readonly Dictionary<DeviceStatus, int> _countByStatus =
            new Dictionary<DeviceStatus, int>();
        private static int _totalDevices;

        static Appliance()
        {
            foreach (DeviceStatus status in Enum.GetValues(typeof(DeviceStatus)))
                _countByStatus[status] = 0;
        }

        public static int TotalDevices => _totalDevices;
        public static int ReceivedCount => _countByStatus[DeviceStatus.Received];
        public static int UnderRepairCount => _countByStatus[DeviceStatus.UnderRepair];
        public static int RepairedCount => _countByStatus[DeviceStatus.Repaired];
        public static int DeliveredCount => _countByStatus[DeviceStatus.Delivered];

        // ---------- private fields ----------
        private readonly string _serialNumber;
        private readonly DateTime _receivedDate;
        private string _ownerName;
        private string _brand;
        private string _faultDescription;
        private DateTime _expectedDeliveryDate;
        private DeviceStatus _status;
        private decimal _partsCost;
        private int _laborHours;

        /// <summary>Raised whenever the device status changes.</summary>
        public event DeviceStatusChangedHandler StatusChanged;

        protected Appliance(string serialNumber, string ownerName, string brand,
                            string faultDescription, DateTime receivedDate,
                            DateTime expectedDeliveryDate, decimal partsCost, int laborHours)
        {
            if (!Validator.IsValidSerialNumber(serialNumber))
                throw new ArgumentException("Invalid serial number. Expected format: XX-12345.");
            if (!Validator.IsValidDeliveryDate(receivedDate, expectedDeliveryDate))
                throw new ArgumentException("Expected delivery date cannot be before the received date.");

            _serialNumber = serialNumber;
            _receivedDate = receivedDate;
            _expectedDeliveryDate = expectedDeliveryDate;
            _status = DeviceStatus.Received;

            // Go through the properties so validation rules are applied once.
            OwnerName = ownerName;
            Brand = brand;
            FaultDescription = faultDescription;
            PartsCost = partsCost;
            LaborHours = laborHours;

            _countByStatus[DeviceStatus.Received]++;
            _totalDevices++;
        }

        // ---------- properties ----------
        public string SerialNumber => _serialNumber;          // read-only after creation
        public DateTime ReceivedDate => _receivedDate;        // read-only after creation
        public DeviceStatus Status => _status;                // changes only through repair methods

        public string OwnerName
        {
            get => _ownerName;
            set
            {
                if (!Validator.IsValidName(value))
                    throw new ArgumentException("Owner name is invalid.");
                _ownerName = value.Trim();
            }
        }

        public string Brand
        {
            get => _brand;
            set
            {
                if (!Validator.IsValidName(value))
                    throw new ArgumentException("Brand is invalid.");
                _brand = value.Trim();
            }
        }

        public string FaultDescription
        {
            get => _faultDescription;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Fault description is required.");
                _faultDescription = value.Trim();
            }
        }

        public DateTime ExpectedDeliveryDate
        {
            get => _expectedDeliveryDate;
            set
            {
                if (!Validator.IsValidDeliveryDate(_receivedDate, value))
                    throw new ArgumentException("Expected delivery date cannot be before the received date.");
                _expectedDeliveryDate = value;
            }
        }

        public decimal PartsCost
        {
            get => _partsCost;
            set
            {
                if (!Validator.IsNonNegative(value))
                    throw new ArgumentException("Parts cost cannot be negative.");
                _partsCost = value;
            }
        }

        public int LaborHours
        {
            get => _laborHours;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Labor hours cannot be negative.");
                _laborHours = value;
            }
        }

        /// <summary>Device type name shown in reports (Refrigerator, ...).</summary>
        public abstract string DeviceType { get; }

        // ---------- abstract behavior ----------
        /// <summary>Each device type calculates its repair cost differently.</summary>
        public abstract decimal CalculateRepairCost();

        /// <summary>Parts + labor, shared by all subclasses.</summary>
        protected decimal BaseCost()
        {
            return _partsCost + _laborHours * HourlyRate;
        }

        // ---------- repair workflow ----------
        public void StartRepair()
        {
            ChangeStatus(DeviceStatus.Received, DeviceStatus.UnderRepair, "Repair started.");
        }

        public void CompleteRepair()
        {
            ChangeStatus(DeviceStatus.UnderRepair, DeviceStatus.Repaired, "Repair completed.");
        }

        public void Deliver()
        {
            ChangeStatus(DeviceStatus.Repaired, DeviceStatus.Delivered, "Delivered to the customer.");
        }

        private void ChangeStatus(DeviceStatus required, DeviceStatus next, string message)
        {
            if (_status != required)
                throw new InvalidOperationException(
                    $"Cannot move device {_serialNumber} from {_status} to {next}.");

            DeviceStatus old = _status;
            _status = next;
            _countByStatus[old]--;
            _countByStatus[next]++;

            OnStatusChanged(new DeviceStatusChangedEventArgs(this, old, next, message));
        }

        protected virtual void OnStatusChanged(DeviceStatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }

        /// <summary>True when the device is still in the center after its promised date.</summary>
        public bool IsOverdue(DateTime today)
        {
            bool stillInCenter = _status == DeviceStatus.Received || _status == DeviceStatus.UnderRepair;
            return stillInCenter && today.Date > _expectedDeliveryDate.Date;
        }

        public virtual string GetSummary()
        {
            return $"[{DeviceType}] {_serialNumber} | {Brand} | Owner: {OwnerName} | " +
                   $"Status: {_status} | Cost: {CalculateRepairCost():C}";
        }
    }
}
