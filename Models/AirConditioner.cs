using System;

namespace RepairCenter.Models
{
    /// <summary>Air conditioner: cost depends on gas refill and on-site visits.</summary>
    public class AirConditioner : Appliance
    {
        private const decimal GasRefillFee = 50m;
        private const decimal OnSiteVisitFee = 30m;

        private int _capacityBtu;
        private bool _needsGasRefill;
        private bool _requiresOnSiteVisit;

        public AirConditioner(string serialNumber, string ownerName, string brand,
                              string faultDescription, DateTime receivedDate,
                              DateTime expectedDeliveryDate, decimal partsCost, int laborHours,
                              int capacityBtu, bool needsGasRefill, bool requiresOnSiteVisit)
            : base(serialNumber, ownerName, brand, faultDescription, receivedDate,
                   expectedDeliveryDate, partsCost, laborHours)
        {
            CapacityBtu = capacityBtu;
            _needsGasRefill = needsGasRefill;
            _requiresOnSiteVisit = requiresOnSiteVisit;
        }

        public int CapacityBtu
        {
            get => _capacityBtu;
            private set
            {
                if (value <= 0)
                    throw new ArgumentException("Capacity must be positive.");
                _capacityBtu = value;
            }
        }

        public bool NeedsGasRefill
        {
            get => _needsGasRefill;
            set => _needsGasRefill = value;
        }

        public bool RequiresOnSiteVisit
        {
            get => _requiresOnSiteVisit;
            set => _requiresOnSiteVisit = value;
        }

        public override string DeviceType => "Air Conditioner";

        public override decimal CalculateRepairCost()
        {
            decimal cost = BaseCost();
            if (_needsGasRefill) cost += GasRefillFee;
            if (_requiresOnSiteVisit) cost += OnSiteVisitFee;
            return cost;
        }
    }
}
