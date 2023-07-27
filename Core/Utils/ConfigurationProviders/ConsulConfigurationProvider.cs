﻿using Consul;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json.Nodes;

namespace Core.Utils.ConfigurationProviders;

public class ConsulConfigurationProvider : ConfigurationProvider
{
    public ConsulConfigurationProvider(string address, string token, string storageName)
    {
        _address = $"http://{address}";
        _storageName = storageName;
        _token = token;
    }


    public override void Load()
    {
        var consulClient = GetConsulClient(_address, _token);
        var configuration = consulClient.KV.Get(_storageName).GetAwaiter().GetResult();
        var configurationString = Encoding.UTF8.GetString(configuration.Response.Value, 0, configuration.Response.Value.Length);
        var jsonNode = JsonNode.Parse(configurationString);

        if (jsonNode is null)
            return;

        foreach (var node in jsonNode.AsObject().AsEnumerable())
        {
            var nodeKey = node.Key;
            switch (node.Value)
            {
                case JsonValue:
                    Data.Add(nodeKey, node.Value.ToString());
                    continue;
                case JsonArray:
                    throw new NotImplementedException();
                case JsonObject: 
                    foreach (var prop in  node.Value.AsObject().AsEnumerable())
                    { 
                        if (prop.Value is null)
                            continue;

                        Data.Add($"{nodeKey}:{prop.Key}", prop.Value.ToString());
                    }
                    continue;
            }
        }
    }


    private static ConsulClient GetConsulClient(string address, string token) 
        => new(options =>
            {
                options.Address = new Uri(address);
                options.Token = token;
            });


    private readonly string _address;
    private readonly string _storageName;
    private readonly string _token;
}