﻿namespace Tiver.Fowl.Core
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Configuration;
    using Exceptions;
    using Serilog;

    public static class Wait
    {
        public static TResult Until<TResult>(Func<TResult> condition, params Type[] ignoredExceptions)
        {
            IWaitConfiguration config = (WaitConfigurationSection)ConfigurationManager.GetSection("waitConfigurationGroup/waitConfiguration");
            return Until(condition, config, ignoredExceptions);
        }

        public static TResult Until<TResult>(Func<TResult> condition, IWaitConfiguration configuration, params Type[] ignoredExceptions)
        {
            return Until(condition, configuration.Timeout, configuration.PollingInterval, ignoredExceptions);
        }

        private static TResult Until<TResult>(Func<TResult> condition, int timeout, int pollingInterval, params Type[] ignoredExceptions)
        {
            // Start continious checking
            var stopwatch = Stopwatch.StartNew();
            Exception lastException = null;

            while (true)
            {
                try
                {
                    var result = condition.Invoke();

                    // Exit condition - some non-default result
                    if (!result.Equals(default(TResult)))
                    {
                        Log.ForContext("LogType", "Wait").Debug("Waiting completed in {ms}ms", stopwatch.ElapsedMilliseconds);
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    var ignored = ignoredExceptions.Any(type => type.IsInstanceOfType(ex));
                    lastException = ex;

                    if (!ignored)
                    {
                        throw;
                    }
                }
                finally
                {
                    // Exit condition - timeout is reached
                    var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                    if (elapsedMilliseconds > timeout)
                    {
                        Log.ForContext("LogType", "Wait").Debug("Waiting failed after {ms}ms", elapsedMilliseconds);
                        stopwatch.Stop();
                        throw new WaitTimeoutException(
                            $"Wait timeout reached after {elapsedMilliseconds} milliseconds waiting.",
                            lastException);
                    }

                    // No exit conditions met - Sleep for polling interval
                    Thread.Sleep(pollingInterval);
                }
            }
        }
    }
}
