"""haakon8855"""

import numpy as np


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
            "speed": 7,
            # Distance when boid will try to avoid other boids
            "too_close_dist": 30,
            # View distance of each boid
            "view_dist": 100,
            # How hard boids will turn when reaching the edge
            "edge_avoidance": 0.09,
            # How hard boids will turn when avoiding other boids
            "avoidance": 0.1,
            # How much boid will try to be in middle of its group
            "coherence": 0.3,
            # How much boid will try to follow direction of nearby boids
            "conformity": 0.08
        }

    def get_positions(self):
        """
        Returns the positions of all boids.
        """
        return self.positions

    def get_filtered_boids(self, index: int):
        """
        Returns all boids visible by the boid given by index.
        """
        position = self.positions[index]
        np.delete(self.positions, index, axis=0)
        np.delete(self.velocities, index, axis=0)
        filter_array = np.linalg.norm(self.positions - position,
                                      axis=1) < self.attributes["view_dist"]
        return self.positions[filter_array], self.velocities[filter_array]

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
        for i in range(len(self.positions)):
            visible_positions, visible_velocities = self.get_filtered_boids(i)
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
