"""Haakon8855"""

import pygame


class SBoid(pygame.sprite.Sprite):
    """
    SBoid class contaning the sprite of a single boid.
    """

    def __init__(self, width, height, pos, color_bg, color_fg):
        super().__init__()
        self.size = self.width, self.height = [width, height]

        self.image = pygame.Surface(self.size, pygame.SRCALPHA, 32)
        self.image.fill(color_bg)
        self.image.set_colorkey((0, 0, 0, 0))

        self.orig_image = self.image

        points = [(0, 0), (0, height), (width, height / 2)]
        pygame.draw.polygon(self.image, color_fg, points)

        self.rect = self.image.get_rect()
        self.rect.x = pos[0]
        self.rect.y = pos[1]

    def rotate(self, angle):
        """
        Rotates the boid to the given angle. Rotation is applied from original
        drawn angle, not from most recent angle.
        """
        self.image = pygame.transform.rotozoom(self.orig_image, angle, 0.28)
        self.rect = self.image.get_rect(center=self.rect.center)
