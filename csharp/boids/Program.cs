using Boids.Models;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("config.json")
    .Build();
var model = config.Get<Config>();

using var game = new Boids.Drawer(model);

game.Run();