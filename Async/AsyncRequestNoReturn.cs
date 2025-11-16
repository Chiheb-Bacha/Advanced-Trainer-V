namespace Advanced_Trainer
{
    /// <summary>
    /// This class represents Async requests that do not return any value, and thus don't use any id attribute to retrieve the result.
    /// </summary>
    internal abstract class AsyncRequestNoReturn
    {
        public AsyncRequestNoReturn() { }

        public abstract void Process();
    }
}
