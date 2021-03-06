﻿namespace Tiver.Fowl.Logging
{
    using Core.Context;
    using Serilog.Core;
    using Serilog.Events;

    public class TestNameEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
               "TestName", TestExecutionContext.TestName));
        }
    }
}
