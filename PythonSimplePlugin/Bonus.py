import clr
import MonoGame
from Microsoft.Xna.Framework import Vector2, Color
from snake_game import Bonuses
clr.ImportExtensions(MonoGame.Extended)


class Bonus(Bonuses.BonusBase):
    def __init__(self, config, random, game):
        pass

    def CheckDependincies(self, plugins):
        return []

    def LoadContent(self, gd):
        pass

    def Update(self, gameTime, fullTime, keyboardState, plugins, size, events):
        return None

    def Draw(self, sb):
        sb.DrawLine(
            Vector2(300, 300),
            Vector2(400, 400),
            Color.Black,
            5
        )
