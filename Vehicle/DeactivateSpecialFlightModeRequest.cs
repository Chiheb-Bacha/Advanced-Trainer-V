using GTA;

namespace Advanced_Trainer
{
    internal class DeactivateSpecialFlightModeRequest : SpecialFlightModeRequest
    {
        public DeactivateSpecialFlightModeRequest(Vehicle vehicle) : base(vehicle) {}

        public override void Process()
        {
            Vehicle vehicle = GetVehicle();
            if (vehicle != null)
            {
                vehicle.DeactivateSpecialFlightMode();
            }
        }
    }
}
