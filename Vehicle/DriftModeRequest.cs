using GTA;

namespace Advanced_Trainer
{
    internal abstract class DriftModeRequest : AsyncRequestNoReturn
    {
        Vehicle vehicle;
        public DriftModeRequest(Vehicle vehicle) : base() {
            this.vehicle = vehicle;
        }
        public Vehicle GetVehicle()
        {
            return vehicle;
        }
        public abstract override void Process();
    }
}
