using System;

namespace RepairCenter.Models
{
    /// <summary>Refrigerator: cost depends on size and refrigerant (freon) leaks.</summary>
    public class Refrigerator : Appliance
    {
        private const decimal GasRefillFee = 40m;
        private const decimal LargeModelFee = 20m;

        private int _capacityLiters;
        private bool _hasGasLeak;

        public Refrigerator(string serialNumber, string ownerName, string brand,
                            string faultDescription, DateTime receivedDate,
                            DateTime expectedDeliveryDate, decimal partsCost, int laborHours,
                            int capacityLiters, bool hasGasLeak)
            : base(serialNumber, ownerName, brand, faultDescription, receivedDate,
                   expectedDeliveryDate, partsCost, laborHours)
        {
            CapacityLiters = capacityLiters;
            _hasGasLeak = hasGasLeak;
        }

        public int CapacityLiters
        {
            get => _capacityLiters;
            private set
            {
                if (value <= 0)
                    throw new ArgumentException("Capacity must be positive.");
                _capacityLiters = value;
            }
        }

        public bool HasGasLeak
        {
            get => _hasGasLeak;
            set => _hasGasLeak = value;
        }

        public override string DeviceType => "Refrigerator";

        public override decimal CalculateRepairCost()
        {
            decimal cost = BaseCost();
            if (_hasGasLeak) cost += GasRefillFee;
            if (_capacityLiters > 500) cost += LargeModelFee;
            return cost;
        }
    }
}
