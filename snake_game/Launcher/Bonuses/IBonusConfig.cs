using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snake_game.Launcher.Bonuses
{
    interface IBonusConfig<T>
    {
        T GetConfig();
        bool IsEnabled();
        TabPage GetPage();
    }
}
