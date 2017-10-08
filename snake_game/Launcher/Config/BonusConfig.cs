using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
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

        public TabPage GetPage(Size tabControlSize)
        {
            var tabControl = new TabControl();
            foreach (var pluginPair in _plugins)
            {
                var type = pluginPair.Value.GetType();
                var name = type.FullName;
                var assemblyName = type.Assembly.GetName().Name;
                
                var config = _config.ContainsKey(pluginPair.Key) ? _config[pluginPair.Key] : pluginPair.Value.Config;
                var page = pluginPair.Value.GetPage(config);
                _pluginPages[pluginPair.Key] = page;

                var tabPage = page.GetPage();
                var label = new Label
                {
                    Text = $"Plugin '{pluginPair.Value.Name}' from '{name}, {assemblyName}'",
                    TextAlignment = TextAlignment.Right,
                    TextColor = SystemColors.DisabledText,
                    Height = 13
                };
                var newContent = tabPage.Content;
                newContent.Height = -1;
                tabPage.Content = new StackLayout
                {
                    Orientation = Orientation.Vertical,
                    Items =
                    {
                        newContent,
                        label
                    }
                };
                tabControl.Pages.Add(tabPage);
            }   

            return new TabPage(tabControl)
            {
                Text = "Bonuses"
            };
        }
    }
}