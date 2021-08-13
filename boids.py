"""haakon8855"""
import numpy as np


class Boids():
    def __init__(self, amount, dim) -> None:
        self.positions = np.random.rand(amount, 2) * dim
        angles = np.random.rand(amount) * np.pi * 2
        self.velocities = np.array([np.cos(angles), np.sin(angles)]).T

        # Speed of boids
        self.speed = 10

        # Distance when boid will try to avoid other boids
        self.tooCloseDist = 30

        # View distance of each boid
        self.viewDist = 120

        # How hard boids will turn when reaching the edge
        self.edgeAvoidance = 0.12

        # How hard boids will turn when avoiding other boids
        self.avoidance = 0.1

        # How much boid will try to be in middle of its group
        self.coherence = 0.3

        # How much boid will try to follow direction of nearby boids
        self.conformity = 0.1

    def getPositions(self):
        return self.positions

    def getFilteredBoids(self, index: int):
        position = self.positions[index]
        np.delete(self.positions, index, axis=0)
        np.delete(self.velocities, index, axis=0)
        filterArray = np.linalg.norm(self.positions - position,
                                     axis=1) < self.viewDist
        return self.positions[filterArray], self.velocities[filterArray]

    def getAvoidVector(self, index: int, visiblePositions: np.array):
        position = self.positions[index]
        avoidVector = np.zeros(2)
        tooCloseBoids = visiblePositions[np.linalg.norm(
            visiblePositions - position, axis=1) < self.tooCloseDist]
        for pos in tooCloseBoids:
            diff = pos - position
            avoidVector = avoidVector - diff
        return avoidVector

    def getPercievedVelocityVector(self, index: int,
                                   visibleVelocities: np.array):
        percievedVelocity = np.zeros(2)
        for velocity in visibleVelocities:
            percievedVelocity = percievedVelocity + velocity
        percievedVelocity = percievedVelocity / len(visibleVelocities)
        return (percievedVelocity - self.velocities[index]) * self.conformity

    def getPercievedCenter(self, index: int, visiblePositions: np.array):
        position = self.positions[index]
        massCenter = visiblePositions.sum(axis=0) / len(visiblePositions)
        return massCenter - position

    def getEdgeCorrectionVector(self, index: int, maxValues: "list[int]"):
        position = self.positions[index]
        edgeCorrectionVelocity = np.zeros(2)
        offset = maxValues * 0.2
        if position[0] < offset[0]:
            edgeCorrectionVelocity[0] = offset[0] - position[0]
        elif position[0] > maxValues[0] - offset[0]:
            edgeCorrectionVelocity[0] = maxValues[0] - offset[0] - position[0]
        if position[1] < offset[1]:
            edgeCorrectionVelocity[1] = offset[1] - position[1]
        elif position[1] > maxValues[1] - offset[1]:
            edgeCorrectionVelocity[1] = maxValues[1] - offset[1] - position[1]
        return edgeCorrectionVelocity

    @staticmethod
    def normalize(vector, scale: float):
        if vector.any():
            vector = (vector / np.linalg.norm(vector)) * scale
        return vector

    def move(self, maxValues: "list[int]"):
        maxValues = np.array(maxValues)
        newVelocities = self.velocities.copy()
        for i in range(len(self.positions)):
            visiblePositions, visibleVelocities = self.getFilteredBoids(i)
            edgeCorrectionVector = self.getEdgeCorrectionVector(i, maxValues)
            edgeCorrectionVector = Boids.normalize(edgeCorrectionVector,
                                                   self.edgeAvoidance)
            avoidVector = self.getAvoidVector(i, visiblePositions)
            avoidVector = Boids.normalize(avoidVector, self.avoidance)
            percievedVelocity = self.getPercievedVelocityVector(
                i, visibleVelocities)
            centerVector = self.getPercievedCenter(i, visiblePositions)
            centerVector = Boids.normalize(centerVector, self.coherence)

            newVelocity = self.velocities[
                i] + avoidVector + percievedVelocity + edgeCorrectionVector
            newVelocity = newVelocity / np.linalg.norm(newVelocity)
            newVelocities[i] = newVelocity
        newPositions = (self.positions + newVelocities * self.speed)
        self.positions = newPositions
        self.velocities = newVelocities
        return self.positions

    def __str__(self) -> str:
        return f"pos: {self.position}, dir: {self.direction}"
