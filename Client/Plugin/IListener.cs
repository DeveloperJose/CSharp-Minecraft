namespace Client
{
    /// <summary>
    /// Identifies a class as a Listener of events.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Initializes the Listener.
        /// Add all your listening calls here!
        /// </summary>
        void Init();
        /// <summary>
        /// 
        /// </summary>
        void Stop();
    }
}
