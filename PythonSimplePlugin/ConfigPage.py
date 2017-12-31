from Eto import Forms
from snake_game import Bonuses


class ConfigPage(Bonuses.IConfigPage):
    def __init__(self, config):
        self._config = config

    def GetConfig(self):
        result = Config()
        result.IsEnabled = bool(self._enabledCheckbox.Checked)
        result.Speed = int(self._speedNum.Value)
        result.Radius = int(self._radiusNum.Value)
        result.MoveRadius = int(self._moveRadiusNum.Value)
        return result

    def GetPage(self):
        content = Forms.StackLayout()

        # Enabled
        self._enabledCheckbox = Forms.CheckBox()
        self._enabledCheckbox.Checked = self._config.IsEnabled

        '''enabledLbl = Forms.Label()
        enabledLbl.Text = "Bonus enabled"'''

        enabledContent = Forms.StackLayout()
        enabledContent.Orientation = Forms.Orientation.Horizontal
        enabledContent.Items.Add(self._enabledCheckbox)
        enabledContent.Items.Add("Bonus enabled")

        content.Items.Add(enabledContent)

        # Speed
        self._speedNum = Forms.NumericUpDown()
        self._speedNum.Value = self._config.Speed

        speedContent = Forms.StackLayout()
        speedContent.Orientation = Forms.Orientation.Horizontal
        speedContent.Items.Add(self._speedNum)
        speedContent.Items.Add("Star speed")

        content.Items.Add(speedContent)

        # Radius
        self._radiusNum = Forms.NumericUpDown()
        self._radiusNum.Value = self._config.Radius

        radiusContent = Forms.StackLayout()
        radiusContent.Orientation = Forms.Orientation.Horizontal
        radiusContent.Items.Add(self._radiusNum)
        radiusContent.Items.Add("Star radius")

        content.Items.Add(radiusContent)

        # Move radius
        self._moveRadiusNum = Forms.NumericUpDown()
        self._moveRadiusNum.Value = self._config.MoveRadius

        moveContent = Forms.StackLayout()
        moveContent.Orientation = Forms.Orientation.Horizontal
        moveContent.Items.Add(self._moveRadiusNum)
        moveContent.Items.Add("Move radius")

        content.Items.Add(moveContent)

        # End
        result = Forms.TabPage(content)
        result.Text = "PySimple"

        return result
