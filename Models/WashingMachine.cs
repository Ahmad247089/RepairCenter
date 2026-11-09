using System;

namespace RepairCenter.Models
{
    /// <summary>Washing machine: cost depends on motor faults and load capacity.</summary>
    public class WashingMachine : Appliance
    {
        private const decimal MotorRepairFee = 60m;
        private const decimal HeavyDutyFee = 15m;

        private int _loadCapacityKg;
        private bool _isMotorFaulty;

        public WashingMachine(string serialNumber, string ownerName, string brand,
                              string faultDescription, DateTime receivedDate,
                              DateTime expectedDeliveryDate, decimal partsCost, int laborHours,
                              int loadCapacityKg, bool isMotorFaulty)
            : base(serialNumber, ownerName, brand, faultDescription, receivedDate,
                   expectedDeliveryDate, partsCost, laborHours)
        {
            LoadCapacityKg = loadCapacityKg;
            _isMotorFaulty = isMotorFaulty;
        }

        public int LoadCapacityKg
        {
            get => _loadCapacityKg;
            private set
            {
                if (value <= 0)
                    throw new ArgumentException("Load capacity must be positive.");
                _loadCapacityKg = value;
            }
        }

        public bool IsMotorFaulty
        {
            get => _isMotorFaulty;
            set => _isMotorFaulty = value;
        }

        public override string DeviceType => "Washing Machine";

        public override decimal CalculateRepairCost()
        {
            decimal cost = BaseCost();
            if (_isMotorFaulty) cost += MotorRepairFee;
            if (_loadCapacityKg >= 10) cost += HeavyDutyFee;
            return cost;
        }
    }
}
