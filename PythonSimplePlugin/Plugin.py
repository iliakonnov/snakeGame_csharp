from snake_game import Bonuses


class Plugin(Bonuses.IPlugin):
    def __init__(self):
        self._config = Config()

    @property
    def Config(self):
        return self._config

    @Config.setter
    def Config(self, value):
        self._config = value

    @property
    def Name(self):
        return "PySimple"

    def GetBonus(self, config, random, game):
        return Bonus(config, random, game)

    def GetPage(self, config):
        return ConfigPage(config)