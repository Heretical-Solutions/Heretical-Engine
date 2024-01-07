namespace HereticalSolutions.Time
{
    public interface ITimer
    {
        string ID { get; }

        #region Timer state

        ETimerState State { get; }

        #endregion

        #region Progress

        float Progress { get; }

        #endregion

        #region Controls

        bool Accumulate { get; set; }

        bool Repeat { get; set; }

        bool FlushTimeElapsedOnRepeat { get; set; }

        void Reset();

        void Start();

        void Pause();

        void Resume();

        void Abort();

        void Finish();

        #endregion
    }
}