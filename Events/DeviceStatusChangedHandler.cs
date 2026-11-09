namespace RepairCenter.Events
{
    /// <summary>
    /// Delegate used to notify subscribers that a device changed state
    /// or that its delivery is overdue.
    /// </summary>
    public delegate void DeviceStatusChangedHandler(object sender, DeviceStatusChangedEventArgs e);
}
