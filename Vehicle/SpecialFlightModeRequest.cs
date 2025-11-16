using GTA;

namespace Advanced_Trainer
{
    internal abstract class SpecialFlightModeRequest : AsyncRequestNoReturn
    {
        private Vehicle vehicle;
        public SpecialFlightModeRequest(Vehicle vehicle) : base()
        {
            this.vehicle = vehicle;
        }
        public Vehicle GetVehicle()
        {
            return vehicle;
        }

        public abstract override void Process();
    }
}
