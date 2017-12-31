import math
import clr
import MonoGame
from Microsoft.Xna.Framework import Vector2, Color
from snake_game import Bonuses, Utils
clr.ImportExtensions(MonoGame.Extended)


class Bonus(Bonuses.BonusBase):
    _drawMethod = 0

    def __init__(self, config, random, game):
        self._config = config

    def CheckDependincies(self, plugins):
        return []

    def LoadContent(self, gd):
        self._speedMilliseconds = self._config.Speed * 1000
        self._realSpeed = (2 * math.pi) / self._speedMilliseconds
        self._step = self._speedMilliseconds / 3
        self._zeroPos = Utils.Point(
                            2 * (gd.Viewport.Width / 3),
                            gd.Viewport.Height / 2
                        )
        self._star = Utils.StarFactory(5, 2, self._config.Radius, gd).GetStar(self._zeroPos)

    def Update(self, gameTime, fullTime, keyboardState, plugins, size, events):
        normalTime = fullTime % self._speedMilliseconds
        t = self._realSpeed * normalTime
        self._star.Position = self._zeroPos.Add(Utils.Point(
            self._config.MoveRadius * math.cos(t),
            self._config.MoveRadius * math.sin(t)
        ))

        self._drawMethod = normalTime // self._step

        return None

    def Draw(self, sb):
        if self._drawMethod == 0:
            self._star.BorderDraw(sb, Color.ForestGreen, 3)
        elif self._drawMethod == 1:
            self._star.PrettyDraw(sb, Color.DarkRed, 3)
        elif self._drawMethod == 2:
            self._star.FillDraw(sb, Color.Yellow)
