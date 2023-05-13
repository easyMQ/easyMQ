﻿using EasyMQ.Abstractions;
using EasyMQ.Abstractions.Consumer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EasyMQ.Console;

public class EasyMqHeaderEvent : IEvent
{
    public string EventName { get; set; }
}

public class EasyMqHeaderEventHandler: IEventHandler<EasyMqHeaderEvent>
{
    private readonly ILogger<EasyMqHeaderEventHandler> _logger;

    public EasyMqHeaderEventHandler(ILogger<EasyMqHeaderEventHandler> logger)
    {
        _logger = logger;
    }
    public Task Handle(ReceiverContext receiverContext, EasyMqHeaderEvent @event)
    {
        _logger.LogInformation("Received a new message, {event}",
            JsonConvert.SerializeObject(@event, Formatting.Indented));
        return Task.CompletedTask;
    }
}