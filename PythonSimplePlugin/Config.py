from System.Collections.Generic import Dictionary
from snake_game import Bonuses


class Config(Bonuses.IPythonPluginConfig):
    _isEnabled = True

    @staticmethod
    def Create():
        return Config()

    def Serialize(self):
        return Dictionary[str, str]({"IsEnabled": str(int(self._isEnabled))})

    def Deserialize(self, data):
        d = dict(data)
        self._isEnabled = bool(int(d["IsEnabled"]))

    @property
    def IsEnabled(self):
        return self._isEnabled

    @IsEnabled.setter
    def IsEnabled(self, value):
        self._isEnabled = value
