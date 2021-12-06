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
            List<float[]> positions = new List<float[]>();
            List<float[]> headings = new List<float[]>();
            for (int i = 0; i < _boids.Length; i++)
            {
                positions.Add(_boids[i].GetPosition());
                headings.Add(_boids[i].GetHeading());
            }

            List<float[]> visiblePositions;
            List<float[]> visibleHeadings;
            for (int i = 0; i < _boids.Length; i++)
            {
                visiblePositions = new List<float[]>();
                visibleHeadings = new List<float[]>();
                for (int j = 0; j < _boids.Length; j++)
                {
                    if (_boids[i].DistanceTo(_boids[j]) < viewDist)
                    {
                        visiblePositions.Add(positions[j]);
                        visibleHeadings.Add(headings[j]);
                    }
                }
                _boids[i].Move(visiblePositions, visibleHeadings);
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