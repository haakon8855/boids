using System;
using System.Collections.Generic;
using System.Linq;
using Boids.Models;
using Microsoft.Extensions.Configuration;

namespace Boids.DataStructures;

public class Boid(double positionX, double positionY, Config config)
{
    public double[] Position { get; } = [positionX, positionY];
    public double[] Heading { get; private set; } = [0.5, 0.5];
    public double Angle => ((Heading[1] < 0) ? -1 : 1) * Math.Acos(Heading[0]);

    private double[] CalculateEdgeAvoidanceVector()
    {
        var offsetX = config.Dimensions.Width * config.Boid.EdgeOffset;
        var offsetY = config.Dimensions.Height * config.Boid.EdgeOffset;
        double[] edgeAvoidanceVector = [0, 0];

        // X component
        if (Position[0] < offsetX)
            edgeAvoidanceVector[0] = offsetX - Position[0];
        else if (Position[0] > config.Dimensions.Width - offsetX)
            edgeAvoidanceVector[0] = config.Dimensions.Width - offsetX - Position[0];

        // Y component
        if (Position[1] < offsetY)
            edgeAvoidanceVector[1] = offsetY - Position[1];
        else if (Position[1] > config.Dimensions.Height - offsetY)
            edgeAvoidanceVector[1] = config.Dimensions.Height - offsetY - Position[1];

        return edgeAvoidanceVector;
    }

    private double[] CalculateBoidAvoidanceVector(List<double[]> positions)
    {
        double[] avoidVector = [0, 0];
        List<double[]> tooCloseBoids = new();

        foreach (var position in positions)
        {
            if (DistanceBetween(Position, position) < config.Boid.TooCloseDist)
            {
                tooCloseBoids.Add(position);
            }
        }

        double[] diff;
        foreach (var position in tooCloseBoids)
        {
            diff =
            [
                position[0] - Position[0],
                position[1] - Position[1]
            ];
            avoidVector[0] = avoidVector[0] - diff[0];
            avoidVector[1] = avoidVector[1] - diff[1];
        }

        return avoidVector;
    }

    private double[] CalculateCenterVector(List<double[]> positions)
    {
        return
        [
            (positions.Select(p => p[0]).Sum() / positions.Count()) - Position[0],
            (positions.Select(p => p[1]).Sum() / positions.Count()) - Position[1]
        ];
    }

    private double[] CalculatePercievedHeadingVector(List<double[]> headings)
    {
        var percievedHeadingVector = new double[] { 0, 0 };
        foreach (var heading in headings)
        {
            for (int i = 0; i < percievedHeadingVector.Length; i++)
            {
                percievedHeadingVector[i] += heading[i];
            }
        }

        for (int i = 0; i < percievedHeadingVector.Length; i++)
        {
            percievedHeadingVector[i] /= headings.Count();
            percievedHeadingVector[i] -= Heading[i];
            percievedHeadingVector[i] *= config.Boid.Conformity;
        }

        return percievedHeadingVector;
    }

    private void UpdateHeading(List<double[]> positions, List<double[]> headings)
    {
        var edgeAvoidanceVector = Normalize(CalculateEdgeAvoidanceVector(), config.Boid.EdgeAvoidance);
        var avoidVector = Normalize(CalculateBoidAvoidanceVector(positions), config.Boid.Avoidance);
        var centerVector = Normalize(CalculateCenterVector(positions), config.Boid.Coherence);
        var percievedHeadingVector = CalculatePercievedHeadingVector(headings);

        for (int i = 0; i < Heading.Length; i++)
        {
            Heading[i] = Heading[i] +
                         edgeAvoidanceVector[i] +
                         avoidVector[i] +
                         centerVector[i] +
                         percievedHeadingVector[i];
        }

        Heading = Normalize(Heading, 1f);
    }

    public void Move(List<double[]> positions, List<double[]> headings)
    {
        UpdateHeading(positions, headings);
        for (int i = 0; i < Position.Length; i++)
            Position[i] += Heading[i] * config.Boid.Velocity;
    }

    public static double[] Normalize(double[] vector, double scale)
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

    public static double DistanceBetween(double[] pos1, double[] pos2)
    {
        return Math.Sqrt((
            Math.Pow(pos2[0] - pos1[0], 2) +
            Math.Pow(pos2[1] - pos1[1], 2)
        ));
    }
}