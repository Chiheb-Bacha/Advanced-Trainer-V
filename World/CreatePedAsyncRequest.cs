using GTA;
using GTA.Math;

namespace Advanced_Trainer
{
    internal class CreatePedAsyncRequest : AsyncRequest
    {
        private Vector3 position;
        private PedHash hash;

        public CreatePedAsyncRequest(AdvancedTrainerAsync asyncHandler, long id, Vector3 position, PedHash hash) : base(asyncHandler, id)
        {
            this.position = position;
            this.hash = hash;
        }

        public Vector3 GetPosition() 
        { 
            return position; 
        }
        
        public PedHash GetHash() 
        { 
            return hash; 
        }

        public override void Process() 
        {
            Ped ped = World.CreatePed(new Model(GetHash()), GetPosition());
            this.GetAsyncHandler().addCreatedPed(this.GetId(), ped);
        }
    }
}
