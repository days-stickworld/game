using System;
using Mirror;
using Newtonsoft.Json;
using StackExchange.Redis;
using UnityEngine;

public class OrchestrationConnector : NetworkBehaviour
{
    private readonly string _serverId = Environment.GetEnvironmentVariable("SERVER_ID")!;
    private ConnectionMultiplexer redis;
    void Start()
    {
        if (isServer)
        {
            
            redis = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_HOST")!);
            var sub = redis.GetSubscriber();
            var networkManager = NetworkManager.singleton;
            sub.Subscribe("server:ping", (channel, msg) =>
            {
                var response = new PingResponse
                {
                    Identifier = _serverId,
                    Status = "OK",
                    OnlinePlayers = networkManager.numPlayers,
                    MaxPlayers = networkManager.maxConnections,
                };

                sub.Publish("server:status", JsonConvert.SerializeObject(response));
            });

            var registerMessage = new ServerNode()
            {
                Identifier = _serverId,
                Host = Environment.GetEnvironmentVariable("SERVER_HOST"),
                Port = Convert.ToInt32(Environment.GetEnvironmentVariable("SERVER_PORT")),
                Type = "GAME",
                Cluster = Environment.GetEnvironmentVariable("SERVER_CLUSTER"),
                MaxPlayers = networkManager.maxConnections
            };
        
            sub.Publish("server:register", JsonConvert.SerializeObject(registerMessage));
        }
    }
}

public class PingResponse
{
    public string Identifier { get; set; } = "";
    public string Status { get; set; } = "";
    public int OnlinePlayers { get; set; }
    public int MaxPlayers { get; set; }
}

public class ServerNode
{
    public string Identifier { get; set; } = "";
    public string Host { get; set; } = "";
    public string Type { get; set; } = "";
    public string Cluster { get; set; } = "";
    public int Port { get; set; }
    public int MaxPlayers { get; set; }
}