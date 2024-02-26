﻿using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace DragaliaAPI.Photon.StateManager.Test;

public class TestContainersHelper
{
    private const int RedisContainerPort = 6379;

    private readonly IContainer redisContainer;

    public string RedisHost { get; private set; }

    public int RedisPort =>
        IsGithubActions
            ? RedisContainerPort
            : this.redisContainer.GetMappedPublicPort(RedisContainerPort);

    private bool IsGithubActions =>
        Environment.GetEnvironmentVariable("GITHUB_ACTIONS") is not null;

    public TestContainersHelper()
    {
        if (IsGithubActions)
        {
            RedisHost = "localhost";
        }
        else
        {
            redisContainer = new ContainerBuilder()
                .WithImage("redis/redis-stack")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("redis-cli PING"))
                .WithPortBinding(RedisContainerPort, true)
                .WithPortBinding(8001, true)
                .Build();

            RedisHost = redisContainer.Hostname;
        }
    }

    public async Task StartAsync()
    {
        if (IsGithubActions)
            return;

        await this.redisContainer.StartAsync();
    }

    public async Task StopAsync()
    {
        if (IsGithubActions)
            return;

        await this.redisContainer.StopAsync();
    }
}
