using GTA;

namespace Advanced_Trainer
{
    internal class DeactivateDriftModeRequest : DriftModeRequest
    {
        public DeactivateDriftModeRequest(Vehicle vehicle) : base(vehicle) { }

        public override void Process()
        {
            Vehicle vehicle = GetVehicle();
            if (vehicle != null)
            {
                vehicle.DeactivateDriftMode();
            }
        }
    }
}
