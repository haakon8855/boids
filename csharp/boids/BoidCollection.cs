using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace boids
{
    public class BoidCollection
    {

        private Boid[] _boids;

        public BoidCollection(int numBoids)
        {
            int[] dimensions = BoidsDrawer.Dimensions;
            _boids = new Boid[numBoids];
            Random rand = new Random();
            for (var i = 0; i < numBoids; i++)
            {
                _boids[i] = new Boid(
                    rand.Next(dimensions[0]), rand.Next(dimensions[1]));
            }
        }

        public void Move()
        {
            float viewDist = _boids[0].GetAttribute("viewDist");
            float[][] visiblePositions;
            float[] visibleAngles;
            for (int i = 0; i < _boids.Length; i++)
            {
                visiblePositions = new float[][] { };
                visibleAngles = new float[] { };
                for (int j = 0; j < _boids.Length; j++)
                {
                    if (_boids[i].DistanceTo(_boids[j]) < viewDist)
                    {
                        visiblePositions.Append(_boids[j].GetPosition());
                        visibleAngles.Append(_boids[j].GetAngle());
                    }
                }
                _boids[i].Move(visiblePositions, visibleAngles);
            }
        }

        public float[][] GetPositions()
        {
            float[][] positions = new float[_boids.Length][];
            for (int i = 0; i < _boids.Length; i++)
            {
                positions[i] = _boids[i].GetPosition();
            }
            return positions;
        }

        public float[] GetAngles()
        {
            float[] angles = new float[_boids.Length];
            for (var i = 0; i < _boids.Length; i++)
            {
                angles[i] = _boids[i].GetAngle();
            }
            return angles;
        }

    }
}