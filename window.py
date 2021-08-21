from boids import Boids

from PyQt5 import QtWidgets, QtGui, QtCore
import numpy as np
import sys


class Window(QtWidgets.QMainWindow):
    def __init__(self, fps=60, amount=100) -> None:
        super().__init__()
        title = "Boids"
        top = 200
        left = 100
        self.dim = [1400, 900]
        self.boidSize = 10
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
        self.timer.start(int(1000 / self.fps))

    def stop(self) -> None:
        self.timer.stop()

    def iteration(self) -> None:
        self.boids.move(self.dim)
        self.drawBoids()

    def paintEvent(self, event) -> None:
        canvasPainter = QtGui.QPainter(self)
        canvasPainter.drawImage(self.rect(), self.image, self.image.rect())

    def drawBoids(self) -> None:
        self.image.fill(self.colors["bg"])
        painter = QtGui.QPainter(self.image)

        painter.setRenderHint(QtGui.QPainter.Antialiasing)
        painter.setPen(QtGui.QPen(self.colors["boids"], 1))
        painter.setBrush(self.colors["boids"])

        for i, position in enumerate(self.boids.positions):
            angle = np.arccos(self.boids.velocities[i][0])
            if (self.boids.velocities[i][1] < 0):
                angle = -angle
            angleA = angle
            angleB = angle - np.pi * (3 / 4)
            angleC = angle + np.pi * (3 / 4)
            A = np.array([np.cos(angleA), np.sin(angleA)])
            A = Boids.normalize(A, 10) + position
            B = np.array([np.cos(angleB), np.sin(angleB)])
            B = Boids.normalize(B, 6) + position
            C = np.array([np.cos(angleC), np.sin(angleC)])
            C = Boids.normalize(C, 6) + position
            triangle = QtGui.QPolygon([
                QtCore.QPoint(int(A[0]), int(A[1])),
                QtCore.QPoint(int(B[0]), int(B[1])),
                QtCore.QPoint(int(C[0]), int(C[1]))
            ])
            painter.drawPolygon(triangle)

        self.update()


if __name__ == "__main__":
    app = QtWidgets.QApplication(sys.argv)
    window = Window()
    window.start()
    window.show()

    sys.exit(app.exec())