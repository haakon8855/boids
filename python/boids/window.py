"""Haakon8855"""

import sys
from PyQt5 import QtWidgets, QtGui, QtCore
import numpy as np

from boids import Boids


class Window(QtWidgets.QMainWindow):
    """
    Window class for drawing all boids on a canvas.
    """
    def __init__(self, fps=60, amount=200) -> None:
        super().__init__()
        title = "Boids"
        top = 200
        left = 100
        self.dim = [1400, 900]
        self.boid_size = 10
        self.fps = fps
        self.amount = amount

        self.colors = {
            "bg": QtGui.QColor(30, 30, 30),
            "boids": QtGui.QColor(100, 100, 170)
        }

        self.setWindowTitle(title)
        self.setGeometry(top, left, self.dim[0], self.dim[1])
        self.image = QtGui.QImage(self.size(), QtGui.QImage.Format_RGB32)
        self.image.fill(self.colors["bg"])

        self.timer = QtCore.QTimer()
        self.timer.timeout.connect(self.iteration)
        self.boids = Boids(amount, self.dim)

    def start(self) -> None:
        """
        Start the timer, calling self.iteration to update the boids' positions
        and velocities, then drawing all boids.
        """
        self.timer.start(int(1000 / self.fps))

    def stop(self) -> None:
        """
        Stop the timer, stopping updating of boids' positions.
        """
        self.timer.stop()

    def iteration(self) -> None:
        """
        Run one iteration of the program (one frame). Updates the positions of
        the boids and their velocities. Then it draws all the boids on the
        screen in their new positions.
        """
        self.boids.move(self.dim)
        self.draw_boids()

    def paintEvent(self, event) -> None:
        """
        Paint event called by the PyQt
        """
        canvas_painter = QtGui.QPainter(self)
        canvas_painter.drawImage(self.rect(), self.image, self.image.rect())

    def draw_boids(self) -> None:
        """
        Draws all boids as triangles
        """
        self.image.fill(self.colors["bg"])
        painter = QtGui.QPainter(self.image)

        painter.setRenderHint(QtGui.QPainter.Antialiasing)
        painter.setPen(QtGui.QPen(self.colors["boids"], 1))
        painter.setBrush(self.colors["boids"])

        for i, position in enumerate(self.boids.positions):
            angle = np.arccos(self.boids.velocities[i][0])
            if self.boids.velocities[i][1] < 0:
                angle = -angle
            angle_a = angle
            angle_b = angle - np.pi * (3 / 4)
            angle_c = angle + np.pi * (3 / 4)
            point_a = np.array([np.cos(angle_a), np.sin(angle_a)])
            point_a = Boids.normalize(point_a, 10) + position
            point_b = np.array([np.cos(angle_b), np.sin(angle_b)])
            point_b = Boids.normalize(point_b, 6) + position
            point_c = np.array([np.cos(angle_c), np.sin(angle_c)])
            point_c = Boids.normalize(point_c, 6) + position
            triangle = QtGui.QPolygon([
                QtCore.QPoint(int(point_a[0]), int(point_a[1])),
                QtCore.QPoint(int(point_b[0]), int(point_b[1])),
                QtCore.QPoint(int(point_c[0]), int(point_c[1]))
            ])
            painter.drawPolygon(triangle)

        self.update()


if __name__ == "__main__":
    app = QtWidgets.QApplication(sys.argv)
    window = Window()
    window.start()
    window.show()

    sys.exit(app.exec())
