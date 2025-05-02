using System;
using System.Collections.Generic;
using System.Linq;
using Boids.Models;

namespace Boids.DataStructures;

public class BoidCollection
{
    private readonly Config _config;
    private Boid[] Boids { get; }
    public double[][] Positions => Boids.Select(b => b.Position).ToArray();
    public double[] Angles => Boids.Select(b => b.Angle).ToArray();

    public BoidCollection(Config config)
    {
        _config = config;
        var numBoids = config.NumBoids;
        Boids = new Boid[numBoids];
        var rand = new Random();

        for (var i = 0; i < config.NumBoids; i++)
            Boids[i] = new Boid(rand.Next(config.Dimensions.Width),
                rand.Next(config.Dimensions.Height), config);
    }

    public void Move()
    {
        var viewDist = _config.Boid.ViewDist;
        List<double[]> positions = new();
        List<double[]> headings = new();
        for (int i = 0; i < Boids.Length; i++)
        {
            positions.Add(Boids[i].Position);
            headings.Add(Boids[i].Heading);
        }

        List<double[]> visiblePositions;
        List<double[]> visibleHeadings;
        for (int i = 0; i < Boids.Length; i++)
        {
            visiblePositions = new();
            visibleHeadings = new();
            for (int j = 0; j < Boids.Length; j++)
            {
                if (Boids[i].DistanceTo(Boids[j]) < viewDist)
                {
                    visiblePositions.Add(positions[j]);
                    visibleHeadings.Add(headings[j]);
                }
            }

            Boids[i].Move(visiblePositions, visibleHeadings);
        }
    }
}