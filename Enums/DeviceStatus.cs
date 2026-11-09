namespace RepairCenter.Enums
{
    /// <summary>
    /// Lifecycle states of an appliance inside the repair center.
    /// </summary>
    public enum DeviceStatus
    {
        Received,     // استُلم الجهاز
        UnderRepair,  // قيد الصيانة
        Repaired,     // تم الإصلاح
        Delivered     // سُلّم للزبون
    }
}
