using GTA;

namespace Advanced_Trainer
{
    internal class ActivateSpecialFlightModeRequest : SpecialFlightModeRequest
    {
        public ActivateSpecialFlightModeRequest(Vehicle vehicle) : base(vehicle) { }

        public override void Process()
        {
            Vehicle vehicle = GetVehicle();
            if (vehicle != null)
            {
                vehicle.ActivateSpecialFlightMode();
            }
        }
    }
}
