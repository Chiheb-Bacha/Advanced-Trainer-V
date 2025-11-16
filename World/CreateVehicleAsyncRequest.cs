using GTA;
using GTA.Math;

namespace Advanced_Trainer
{
    internal class CreateVehicleAsyncRequest : AsyncRequest
    {
        private Vector3 position;
        private VehicleHash hash;

        public CreateVehicleAsyncRequest(AdvancedTrainerAsync asyncHandler, long id, Vector3 position, VehicleHash hash) : base(asyncHandler, id)
        {
            this.position = position;
            this.hash = hash;
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public VehicleHash GetHash()
        {
            return hash;
        }

        public override void Process()
        {
            Vehicle vehicle = World.CreateVehicle(new Model(GetHash()), GetPosition());
            this.GetAsyncHandler().addCreatedVehicle(GetId(), vehicle);
        }
    }
}
