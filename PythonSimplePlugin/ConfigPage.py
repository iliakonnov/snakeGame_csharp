from Eto import Forms
from snake_game import Bonuses


class ConfigPage(Bonuses.IConfigPage):
    def __init__(self, config):
        self._config = config

    def GetConfig(self):
        result = Config()
        result.IsEnabled = bool(self._enabledCheckbox.Checked)
        return result

    def GetPage(self):
        self._enabledCheckbox = Forms.CheckBox()
        self._enabledCheckbox.Checked = self._config.IsEnabled

        lbl = Forms.Label()
        lbl.Text = "Bonus enabled"

        enabledContent = Forms.StackLayout()
        enabledContent.Orientation = Forms.Orientation.Horizontal
        enabledContent.Items.Add(self._enabledCheckbox)
        enabledContent.Items.Add(lbl)

        content = Forms.StackLayout()
        content.Items.Add(enabledContent)

        result = Forms.TabPage(content)
        result.Text = "PySimple"

        return result
