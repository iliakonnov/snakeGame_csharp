using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using snake_game.Bonuses;

namespace snake_game.Launcher.Config
{
    public class BonusConfig
    {
        Dictionary<string, IPluginConfig> _config;
        Dictionary<string, IPlugin> _plugins;
        Dictionary<string, IConfigPage> _pluginPages = new Dictionary<string, IConfigPage>();

        public BonusConfig(Dictionary<string, IPluginConfig> config, Dictionary<string, IPlugin> plugins)
        {
            _config = config;
            _plugins = plugins;
        }

        public Dictionary<string, IPluginConfig> GetConfig() =>
            _pluginPages.ToDictionary(x => x.Key, x => x.Value.GetConfig());

        public TabPage GetPage(Eto.Drawing.Size size, int height)
        {
            var tabControl = new TabControl
            {
                Size = new Eto.Drawing.Size(size.Width, size.Height - height)
            };
            foreach (var pluginPair in _plugins)
            {
                IPluginConfig config;
                config = _config.ContainsKey(pluginPair.Key) ? _config[pluginPair.Key] : pluginPair.Value.Config;
                var page = pluginPair.Value.GetPage(config);
                _pluginPages[pluginPair.Key] = page;
                tabControl.Pages.Add(page.GetPage());   
            }                

            return new TabPage(tabControl)
            {
                Text = "Bonuses"
            };
        }
    }
}