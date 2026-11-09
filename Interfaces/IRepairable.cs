using RepairCenter.Enums;

namespace RepairCenter.Interfaces
{
    /// <summary>
    /// Core operations shared by every appliance handled by the center.
    /// </summary>
    public interface IRepairable
    {
        string SerialNumber { get; }
        DeviceStatus Status { get; }

        void StartRepair();
        void CompleteRepair();
        void Deliver();

        decimal CalculateRepairCost();
        string GetSummary();
    }
}
