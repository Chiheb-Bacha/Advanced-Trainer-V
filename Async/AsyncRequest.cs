namespace Advanced_Trainer
{
    internal abstract class AsyncRequest
    {
        private AdvancedTrainerAsync asyncHandler;
        private long id;
        public AsyncRequest(AdvancedTrainerAsync asyncHandler, long id) { 
            this.asyncHandler = asyncHandler;
            this.id = id;
        }

        public long GetId()
        {
            return id;
        }

        public AdvancedTrainerAsync GetAsyncHandler()
        {
            return asyncHandler;
        }

        public abstract void Process();

    }
}
