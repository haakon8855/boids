"""Haakon8855"""

import pygame
import numpy as np

from boids import Boids
from s_boid import SBoid

COLOR_BG = (30, 30, 30)
COLOR_BOIDS = (100, 100, 170)


class Window():
    """
    Window class for drawing all boids on a canvas.
    """

    def __init__(self, fps=60, amount=150) -> None:
        self.fps = fps
        self.amount = amount
        self.pos = self.top, self.left = 200, 100
        self.size = self.width, self.height = [1280, 720]
        self.title = "Boids"

        self._running = True
        self._display_surf = None

        self.sboid_list = pygame.sprite.Group()
        self.boids = None
        self.clock = pygame.time.Clock()

    def initialize(self) -> None:
        """
        Initiate pygame, boids and boid sprites.
        """
        pygame.init()
        self._display_surf = pygame.display.set_mode(self.size,
                                                     pygame.HWSURFACE)

        pygame.display.set_caption(self.title)
        self.boids = Boids(self.amount, self.size)

        self.create_sboids()

        self._running = True

    def create_sboids(self) -> None:
        """
        Create boid sprite objects with defined size, position and colors.
        """
        for position in self.boids.positions:
            sboid = SBoid(50, 30, position, COLOR_BG, COLOR_BOIDS)
            self.sboid_list.add(sboid)

    def handle_event(self, event: pygame.event):
        """
        Handles pygame events such as quitting or keypresses.
        """
        if event.type == pygame.QUIT:
            self._running = False

    def iteration(self):
        """
        Run one iteration of the program (one frame). Updates the positions of
        the boids and their velocities.
        """
        self.boids.move(self.size)

    def render(self):
        """
        Draws all boids on the screen with their current position and angle.
        """
        for position, velocity, sboid in zip(self.boids.positions,
                                             self.boids.velocities,
                                             self.sboid_list):
            angle = (np.arccos(velocity[0]) / (np.pi * 2)) * 360
            if velocity[1] > 0:
                angle = -angle
            sboid.rotate(angle)
            sboid.rect.centerx = position[0]
            sboid.rect.centery = position[1]
        self.sboid_list.update()
        self._display_surf.fill(COLOR_BG)
        self.sboid_list.draw(self._display_surf)
        pygame.display.flip()

    def quit(self):
        """
        Quits the pygame window and exits the program gracefully.
        """
        pygame.quit()

    def run(self):
        """
        Runs the main loop handling state updates and rendering.
        """
        if self.initialize() is False:
            self._running = False

        while (self._running):
            for event in pygame.event.get():
                self.handle_event(event)
            self.iteration()
            self.render()
            self.clock.tick(self.fps)
        self.quit()


if __name__ == "__main__":
    window = Window()
    window.run()
