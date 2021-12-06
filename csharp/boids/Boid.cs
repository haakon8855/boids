using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace boids
{
    public class Boid
    {

        private float[] _position;
        private float[] _heading = { 0.5f, 0.5f };
        private Dictionary<string, float> _attributes = new Dictionary<string, float>();

        public Boid(float[] position)
        {
            _position = position;
            Initialize();
        }

        public Boid(float positionX, float positionY)
        {
            _position = new float[] { positionX, positionY };
            Initialize();
        }

        private void Initialize()
        {
            // Velocity of boids
            _attributes["velocity"] = 6f;
            // Distance when boid will try to avoid other boids
            _attributes["viewDist"] = 100f;
            // View distance of each boid
            _attributes["tooCloseDist"] = 35f;
            // How hard boids will turn when reaching the edge
            _attributes["edgeAvoidance"] = 0.07f;
            // How hard boids will turn when avoiding other boids
            _attributes["avoidance"] = 0.05f;
            // How much boid will try to be in middle of its group
            _attributes["coherence"] = 0.02f;
            // How much boid will try to follow direction of nearby boids
            _attributes["conformity"] = 0.06f;
            // When will edge avoidance kick in
            _attributes["edgeOffset"] = 0.2f;
        }

        public float GetAttribute(string key)
        {
            return _attributes[key];
        }

        public float[] GetPosition()
        {
            return _position;
        }

        public float[] GetHeading()
        {
            return _heading;
        }

        public float GetAngle()
        {
            float angle = (float)Math.Acos(_heading[0]);
            return _heading[1] < 0 ? -angle : angle;
        }

        private float[] GetEdgeAvoidanceVector()
        {
            float offsetX = (float)(BoidsDrawer.Dimensions[0] * _attributes["edgeOffset"]);
            float offsetY = (float)(BoidsDrawer.Dimensions[1] * _attributes["edgeOffset"]);
            float[] edgeAvoidanceVector = { 0, 0 };
            // X component
            if (_position[0] < offsetX)
            {
                edgeAvoidanceVector[0] = offsetX - _position[0];
            }
            else if (_position[0] > BoidsDrawer.Dimensions[0] - offsetX)
            {
                edgeAvoidanceVector[0] =
                    BoidsDrawer.Dimensions[0] - offsetX - _position[0];
            }
            // Y component
            if (_position[1] < offsetY)
            {
                edgeAvoidanceVector[1] = offsetY - _position[1];
            }
            else if (_position[1] > BoidsDrawer.Dimensions[1] - offsetY)
            {
                edgeAvoidanceVector[1] =
                    BoidsDrawer.Dimensions[1] - offsetY - _position[1];
            }

            return edgeAvoidanceVector;
        }

        private float[] GetAvoidVector(List<float[]> positions)
        {
            float[] avoidVector = { 0, 0 };
            List<float[]> tooCloseBoids = new List<float[]>();
            float[] diff;
            foreach (float[] position in positions)
            {
                if (DistanceBetween(_position, position) < _attributes["tooCloseDist"])
                {
                    tooCloseBoids.Add(position);
                }
            }
            foreach (float[] position in tooCloseBoids)
            {
                diff = new float[] {
                    position[0] - _position[0],
                    position[1] - _position[1]
                };
                avoidVector[0] = avoidVector[0] - diff[0];
                avoidVector[1] = avoidVector[1] - diff[1];
            }
            return avoidVector;
        }

        private float[] GetCenterVector(List<float[]> positions)
        {
            return new float[] {
                (positions.Select(point => point[0]).Sum() / positions.Count())
                     - _position[0],
                (positions.Select(point => point[1]).Sum() / positions.Count())
                     - _position[1]
            };
        }

        private float[] GetPercievedHeadingVector(List<float[]> headings)
        {
            float[] percievedHeadingVector = new float[] { 0, 0 };
            foreach (float[] heading in headings)
            {
                for (int i = 0; i < percievedHeadingVector.Length; i++)
                {
                    percievedHeadingVector[i] += heading[i];
                }
            }

            for (int i = 0; i < percievedHeadingVector.Length; i++)
            {
                percievedHeadingVector[i] /= headings.Count();
                percievedHeadingVector[i] -= _heading[i];
                // May have to scale with "conformity" here
                percievedHeadingVector[i] *= _attributes["conformity"];
            }
            return percievedHeadingVector;
        }

        private void UpdateHeading(List<float[]> positions, List<float[]> headings)
        {
            float[] edgeAvoidanceVector =
                Normalize(GetEdgeAvoidanceVector(), _attributes["edgeAvoidance"]);
            float[] avoidVector =
                Normalize(GetAvoidVector(positions), _attributes["avoidance"]);
            float[] centerVector =
                Normalize(GetCenterVector(positions), _attributes["coherence"]);
            // float[] percievedHeadingVector =
            //     Normalize(GetPercievedHeadingVector(headings), _attributes["conformity"]);
            float[] percievedHeadingVector = GetPercievedHeadingVector(headings);
            for (int i = 0; i < _heading.Length; i++)
            {
                _heading[i] = _heading[i] +
                    edgeAvoidanceVector[i] +
                    avoidVector[i] +
                    centerVector[i] +
                    percievedHeadingVector[i];
            }
            _heading = Normalize(_heading, 1f);
        }

        public void Move(List<float[]> positions, List<float[]> headings)
        {
            UpdateHeading(positions, headings);
            for (int i = 0; i < _position.Length; i++)
            {
                _position[i] += _heading[i] * _attributes["velocity"];
            }
        }

        public static float[] Normalize(float[] vector, float scale)
        {
            if (vector[0] == 0 && vector[1] == 0)
            {
                return vector;
            }
            float length = (float)Math.Sqrt(vector
                                      .Select(value => value * value)
                                      .Sum());
            return vector.Select(value => (value / length) * scale).ToArray();
        }

        public float DistanceTo(Boid boid)
        {
            float[] other = boid.GetPosition();
            return DistanceBetween(_position, other);
        }

        public static float DistanceBetween(float[] pos1, float[] pos2)
        {
            return (float)Math.Sqrt((
                Math.Pow((pos2[0] - pos1[0]), 2) +
                Math.Pow((pos2[1] - pos1[1]), 2)
            ));
        }

    }
}