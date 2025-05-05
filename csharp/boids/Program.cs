using Boids;
using Boids.Models;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("config.json")
    .Build();
var configModel = config.Get<Configuration>();

using var game = new Monogame(configModel);

game.Run();