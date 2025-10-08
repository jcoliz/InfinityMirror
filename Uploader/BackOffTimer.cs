namespace InfinityMirror.Uploader;

/// <summary>
/// Timer to manage delays between message generation cycles. 
/// </summary>
public class BackOffTimer(TimeSpan initialDelay, TimeSpan maxDelay, double backoffFactor)
{
    /// <summary>
    /// When the timer was started
    /// </summary>
    private DateTimeOffset _startTime = DateTimeOffset.UtcNow;

    /// <summary>
    /// How long the timer has been running
    /// </summary>
    private TimeSpan LifeSpan => DateTimeOffset.UtcNow - _startTime;

    /// <summary>
    /// Calculates the current delay based on the life span of the timer, using the backoff factor,
    /// and considering the initial and maximum delay.
    /// </summary>
    public TimeSpan CurrentDelay
    {
        get
        {
            var delay = ( LifeSpan - initialDelay * 5 ) * backoffFactor ;

            if (delay < initialDelay)
                return initialDelay;
            if (delay > maxDelay)
                return maxDelay;
            return delay;
        }
    }

    public async Task WaitAsync(CancellationToken? cancellationToken = null)
    {
        var delay = CurrentDelay;
        await Task.Delay(delay, cancellationToken ?? CancellationToken.None);
    }
}