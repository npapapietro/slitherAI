namespace SlitherPlayer.Environment
{
    public interface IEnvironmentState
    {
        float[] ScreenState { get; }

        int Length { get; }

        bool Dead { get; }

        /// <summary>
        /// Unix epoch format
        /// </summary>
        /// <value></value>
        int TimeStamp { get; }   
    }
}