using System;
using System.Collections.Generic;
using System.Linq;
using Boids.Models;

namespace Boids.DataStructures;

public class BoidCollection
{
    private readonly Configuration _configuration;
    private Boid[] Boids { get; }
    public double[][] Positions => Boids.Select(b => b.Position).ToArray();
    public double[] Angles => Boids.Select(b => b.Angle).ToArray();

    public BoidCollection(Configuration configuration)
    {
        _configuration = configuration;
        Boids = new Boid[_configuration.Boids.NumberOfBoids];
        var rand = new Random();

        for (var i = 0; i < _configuration.Boids.NumberOfBoids; i++)
        {
            Boids[i] = new Boid(
                rand.Next(_configuration.Window.Width),
                rand.Next(_configuration.Window.Height),
                _configuration
            );
        }
    }

    public void Move()
    {
        var viewDist = _configuration.Boids.ViewDist;
        List<double[]> positions = [];
        List<double[]> headings = [];
        foreach (var boid in Boids)
        {
            positions.Add(boid.Position);
            headings.Add(boid.Heading);
        }

        foreach (var boid in Boids)
        {
            List<double[]> visiblePositions = [];
            List<double[]> visibleHeadings = [];
            for (var j = 0; j < Boids.Length; j++)
            {
                if (boid.DistanceTo(Boids[j]) < viewDist)
                {
                    visiblePositions.Add(positions[j]);
                    visibleHeadings.Add(headings[j]);
                }
            }

            boid.Move(visiblePositions, visibleHeadings);
        }
    }
}