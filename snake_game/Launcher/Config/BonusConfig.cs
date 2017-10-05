using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using snake_game.Bonuses;
using snake_game.Launcher.Bonuses;

namespace snake_game.Launcher.Config
{
    public class BonusConfig
    {
        Dictionary<string, IPluginConfig> _config;
        Dictionary<string, IPlugin> _plugins;

        public BonusConfig(Dictionary<string, IPluginConfig> config, Dictionary<string, IPlugin> plugins)
        {
            _config = config;
            _plugins = plugins;
        }

        public Dictionary<string, IPluginConfig> GetConfig()
        {
            return _config;
        }

        public TabPage GetPage(Eto.Drawing.Size size, int height)
        {
            var tabControl = new TabControl
            {
                Size = new Eto.Drawing.Size(size.Width, size.Height - height)
            };
            foreach (var pluginPair in _plugins)
                tabControl.Pages.Add(pluginPair.Value.GetPage(_config[pluginPair.Key]));

            return new TabPage(tabControl)
            {
                Text = "Bonuses"
            };
        }
    }
}