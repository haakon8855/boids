using Boids;
using Boids.Models.Config;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("config.json")
    .Build()
    .Get<Config>();

var window = new Window(config);
window.Run();