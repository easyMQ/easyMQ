﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EasyMQ.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EasyMQ.E2E.Tests;

public class Fixture
{
    private readonly IConfigurationRoot _configuration;
    private readonly ServiceProvider _provider;

    public Fixture()
    {
        var services = new ServiceCollection() as IServiceCollection;
        var configBuilder = new ConfigurationBuilder() as IConfigurationBuilder;
        configBuilder
            .AddJsonFile("appsettings.json", false)
            .AddUserSecrets<EndToEndTests>();
        AddConfigurationSources(configBuilder);
        _configuration = configBuilder.Build();
        AddServices(services, _configuration);
        _provider = services.BuildServiceProvider();
        var hostedServices = _provider.GetServices<IHostedService>();
        foreach (var hostedService in hostedServices)
        {
            hostedService.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
    }

    protected virtual void AddConfigurationSources(IConfigurationBuilder configurationBuilder)
    {
        
    }

    protected virtual void AddServices(IServiceCollection services, IConfigurationRoot configurationRoot)
    {
        services.AddEasyMq(_configuration, builder =>
        {
            builder.WithConnectionFactory(factory =>
            {
                factory.Uri =
                    new Uri(
                        $"amqp://{_configuration["rmq_username"]}:{_configuration["rmq_password"]}@localhost:5672/");
                factory.DispatchConsumersAsync = true;
                factory.TopologyRecoveryEnabled = true;
                factory.AutomaticRecoveryEnabled = true;
                return factory;
            });
            builder.WithConsumerSection("RabbitConsumerConfigurations");
            builder.WithProducerSection("RabbitProducerConfigurations");
        });
    }

    protected async Task When<T>(Func<T, Task> action) where T : class
    {
        var service = _provider.GetRequiredService<T>();
        await action(service);
    }

    protected async Task Then<T>(Func<T, Task> action) where T : class
    {
        var service = _provider.GetRequiredService<T>();
        await action(service);
    }
}