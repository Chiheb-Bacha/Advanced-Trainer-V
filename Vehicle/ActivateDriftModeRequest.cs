using GTA;

namespace Advanced_Trainer
{
    internal class ActivateDriftModeRequest : DriftModeRequest
    {
        private DriftCarHash dritftType;
        public ActivateDriftModeRequest(Vehicle vehicle, DriftCarHash driftType) : base(vehicle)
        {
            this.dritftType = driftType;
        }

        public DriftCarHash GetDriftType()
        {
            return dritftType;
        }

        public override void Process()
        {
            Vehicle vehicle = GetVehicle();
            if (vehicle != null)
            {
                vehicle.ActivateDriftMode(GetDriftType());
            }
        }
    }
}
