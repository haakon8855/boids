"""haakon8855"""

import numpy as np
from sklearn.neighbors import BallTree


class Boids():
    """
    Boids class containing data for all boids
    """

    def __init__(self, amount, dim) -> None:
        self.positions = np.random.rand(amount, 2) * dim
        angles = np.random.rand(amount) * np.pi * 2
        self.velocities = np.array([np.cos(angles), np.sin(angles)]).T

        self.attributes = {
            # Speed of boids
            "speed": 3.5,
            # Distance when boid will try to avoid other boids
            "too_close_dist": 15,
            # View distance of each boid
            "view_dist": 65,
            # How hard boids will turn when reaching the edge
            "edge_avoidance": 0.03,
            # How hard boids will turn when avoiding other boids
            "avoidance": 0.05,
            # How much boid will try to be in middle of its group
            "coherence": 0.15,
            # How much boid will try to follow direction of nearby boids
            "conformity": 0.04
        }

    def get_positions(self):
        """
        Returns the positions of all boids.
        """
        return self.positions

    def get_boid_filter(self):
        """
        Returns a list of lists of indices of all boids visible every boid.
        """
        tree = BallTree(self.positions, leaf_size=2)
        indices = tree.query_radius(self.positions,
                                    r=self.attributes["view_dist"])
        return indices

    def get_avoid_vector(self, index: int, visible_positions: np.array):
        """
        Returns the avoid vector for the boid given by index. This vector
        points away from nearby boids to avoid crashing into them.
        """
        position = self.positions[index]
        avoid_vector = np.zeros(2)
        too_close_boids = visible_positions[np.linalg.norm(
            visible_positions -
            position, axis=1) < self.attributes["too_close_dist"]]
        for pos in too_close_boids:
            diff = pos - position
            avoid_vector = avoid_vector - diff
        return avoid_vector

    def get_percieved_velocity_vector(self, index: int,
                                      visible_velocities: np.array):
        """
        Returns the vector of the percieved velocity of nearby boids. This is
        the average of all the velocities of the visible boids.
        """
        percieved_velocity = np.zeros(2)
        for velocity in visible_velocities:
            percieved_velocity = percieved_velocity + velocity
        percieved_velocity = percieved_velocity / len(visible_velocities)
        return (percieved_velocity -
                self.velocities[index]) * self.attributes["conformity"]

    def get_percieved_center(self, index: int, visible_positions: np.array):
        """
        Returns the vector pointing toward the mass center of visible boids.
        """
        position = self.positions[index]
        mass_center = visible_positions.sum(axis=0) / len(visible_positions)
        return mass_center - position

    def get_edge_correction_vector(self, index: int, max_values: "list[int]"):
        """
        Returns the edge avoidance vector. A vector pointing away from the edge
        of the screen.
        """
        position = self.positions[index]
        edge_correction_velocity = np.zeros(2)
        offset = max_values * 0.2
        if position[0] < offset[0]:
            edge_correction_velocity[0] = offset[0] - position[0]
        elif position[0] > max_values[0] - offset[0]:
            edge_correction_velocity[
                0] = max_values[0] - offset[0] - position[0]
        if position[1] < offset[1]:
            edge_correction_velocity[1] = offset[1] - position[1]
        elif position[1] > max_values[1] - offset[1]:
            edge_correction_velocity[
                1] = max_values[1] - offset[1] - position[1]
        return edge_correction_velocity

    @staticmethod
    def normalize(vector, scale: float):
        """
        Returns the normalized vector multiplied by a scale factor.
        """
        if vector.any():
            vector = (vector / np.linalg.norm(vector)) * scale
        return vector

    def move(self, max_values: "list[int]"):
        """
        Combines all types of vector to get a velocity vector for each boid.
        Each boid is then moved in the direction of this vector.
        """
        max_values = np.array(max_values)
        new_velocities = self.velocities.copy()
        visibile_indices = self.get_boid_filter()
        for i in range(len(self.positions)):
            visible_positions = self.positions[visibile_indices[i]]
            visible_velocities = self.velocities[visibile_indices[i]]
            edge_correction_vector = self.get_edge_correction_vector(
                i, max_values)
            edge_correction_vector = Boids.normalize(
                edge_correction_vector, self.attributes["edge_avoidance"])
            avoid_vector = self.get_avoid_vector(i, visible_positions)
            avoid_vector = Boids.normalize(avoid_vector,
                                           self.attributes["avoidance"])
            percieved_velocity = self.get_percieved_velocity_vector(
                i, visible_velocities)
            center_vector = self.get_percieved_center(i, visible_positions)
            center_vector = Boids.normalize(center_vector,
                                            self.attributes["coherence"])

            new_velocity = self.velocities[
                i] + avoid_vector + percieved_velocity + edge_correction_vector
            new_velocity = new_velocity / np.linalg.norm(new_velocity)
            new_velocities[i] = new_velocity
        new_positions = (self.positions +
                         new_velocities * self.attributes["speed"])
        self.positions = new_positions
        self.velocities = new_velocities
        return self.positions

    def __str__(self) -> str:
        return f"pos: {self.positions}, dir: {self.velocities}"
