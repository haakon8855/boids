using System;
using System.Collections.Generic;
using System.Linq;
using Boids.Models;

namespace Boids.DataStructures;

public class Boid(double positionX, double positionY, Configuration configuration)
{
    public double[] Position { get; } = [positionX, positionY];
    public double[] Heading { get; private set; } = GetRandomStartVelocity();
    public double Angle => (Heading[1] < 0 ? -1 : 1) * Math.Acos(Heading[0]);

    private double[] CalculateEdgeAvoidanceVector()
    {
        var offsetX = configuration.Window.Width * configuration.Boids.EdgeOffset;
        var offsetY = configuration.Window.Height * configuration.Boids.EdgeOffset;
        double[] edgeAvoidanceVector = [0, 0];

        // X component
        if (Position[0] < offsetX)
            edgeAvoidanceVector[0] = offsetX - Position[0];
        else if (Position[0] > configuration.Window.Width - offsetX)
            edgeAvoidanceVector[0] = configuration.Window.Width - offsetX - Position[0];

        // Y component
        if (Position[1] < offsetY)
            edgeAvoidanceVector[1] = offsetY - Position[1];
        else if (Position[1] > configuration.Window.Height - offsetY)
            edgeAvoidanceVector[1] = configuration.Window.Height - offsetY - Position[1];

        return edgeAvoidanceVector;
    }

    private double[] CalculateBoidAvoidanceVector(List<double[]> positions)
    {
        double[] avoidVector = [0, 0];
        List<double[]> tooCloseBoids = [];

        tooCloseBoids.AddRange(
            positions.Where(
                position => DistanceBetween(Position, position) < configuration.Boids.TooCloseDist
            )
        );

        foreach (var position in tooCloseBoids)
        {
            double[] diff =
            [
                position[0] - Position[0],
                position[1] - Position[1]
            ];
            avoidVector[0] -= diff[0];
            avoidVector[1] -= diff[1];
        }

        return avoidVector;
    }

    private double[] CalculateCenterVector(List<double[]> positions)
    {
        return
        [
            positions.Select(p => p[0]).Sum() / positions.Count - Position[0],
            positions.Select(p => p[1]).Sum() / positions.Count - Position[1]
        ];
    }

    private double[] CalculatePerceivedHeadingVector(List<double[]> headings)
    {
        var perceivedHeadingVector = new double[] { 0, 0 };
        foreach (var heading in headings)
        {
            for (var i = 0; i < perceivedHeadingVector.Length; i++)
            {
                perceivedHeadingVector[i] += heading[i];
            }
        }

        for (var i = 0; i < perceivedHeadingVector.Length; i++)
        {
            perceivedHeadingVector[i] /= headings.Count;
            perceivedHeadingVector[i] -= Heading[i];
            perceivedHeadingVector[i] *= configuration.Boids.Conformity;
        }

        return perceivedHeadingVector;
    }

    private void UpdateHeading(List<double[]> positions, List<double[]> headings)
    {
        var edgeAvoidanceVector = Normalize(CalculateEdgeAvoidanceVector(), configuration.Boids.EdgeAvoidance);
        var avoidVector = Normalize(CalculateBoidAvoidanceVector(positions), configuration.Boids.Avoidance);
        var centerVector = Normalize(CalculateCenterVector(positions), configuration.Boids.Coherence);
        var perceivedHeadingVector = CalculatePerceivedHeadingVector(headings);

        for (var i = 0; i < Heading.Length; i++)
        {
            Heading[i] = Heading[i] +
                         edgeAvoidanceVector[i] +
                         avoidVector[i] +
                         centerVector[i] +
                         perceivedHeadingVector[i];
        }

        Heading = Normalize(Heading, 1f);
    }

    public void Move(List<double[]> positions, List<double[]> headings)
    {
        UpdateHeading(positions, headings);
        for (var i = 0; i < Position.Length; i++)
        {
            Position[i] += Heading[i] * configuration.Boids.Velocity;
        }
    }

    private static double[] Normalize(double[] vector, double scale)
    {
        if (vector[0] == 0 && vector[1] == 0)
            return vector;
        var length = Math.Sqrt(vector.Select(x => x * x).Sum());
        return vector.Select(value => value / length * scale).ToArray();
    }

    public double DistanceTo(Boid boid)
    {
        return DistanceBetween(Position, boid.Position);
    }

    private static double DistanceBetween(double[] pos1, double[] pos2)
    {
        return Math.Sqrt((
            Math.Pow(pos2[0] - pos1[0], 2) +
            Math.Pow(pos2[1] - pos1[1], 2)
        ));
    }

    private static double[] GetRandomStartVelocity()
    {
        var rand = new Random();
        return [rand.NextDouble() - 0.5, rand.NextDouble() - 0.5];
    }
}