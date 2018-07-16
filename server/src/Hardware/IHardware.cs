namespace server.Hardware
{
    public interface IHardware
    {
        HardwareIdentifer Identify();

        ISensorBase[] GetSensors();

        void TickUpdate();
    }
}
