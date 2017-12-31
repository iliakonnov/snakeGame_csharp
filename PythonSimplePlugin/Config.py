from System.Collections.Generic import Dictionary
from snake_game import Bonuses


class Config(Bonuses.IPythonPluginConfig):
    _isEnabled = True
    _speed = 5
    _radius = 30
    _moveRadius = 300

    @staticmethod
    def Create():
        return Config()

    def Serialize(self):
        return Dictionary[str, str]({
            "IsEnabled": str(int(self._isEnabled)),
            "Speed": str(self._speed)
        })

    def Deserialize(self, data):
        d = dict(data)
        self._isEnabled = bool(int(d["IsEnabled"]))
        self._speed = int(d["Speed"])

    @property
    def IsEnabled(self):
        return self._isEnabled

    @IsEnabled.setter
    def IsEnabled(self, value):
        self._isEnabled = value

    @property
    def Speed(self):
        return self._speed

    @Speed.setter
    def Speed(self, value):
        self._speed = value

    @property
    def Radius(self):
        return self._radius

    @Radius.setter
    def Radius(self, value):
        self._radius = value

    @property
    def MoveRadius(self):
        return self._moveRadius

    @MoveRadius.setter
    def MoveRadius(self, value):
        self.__oveRadius = value
