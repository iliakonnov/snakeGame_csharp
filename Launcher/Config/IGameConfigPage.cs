using Eto.Forms;

namespace Launcher.Config
{
    /// <summary>
    ///     Интерфейс для вкладки настроек игры. Не для бонусов
    /// </summary>
    /// <typeparam name="TConfig">Какие настройки настравиаются в этой вкладке</typeparam>
    public interface IGameConfigPage<out TConfig>
    {
        /// <summary>
        ///     Получает настройки, за которые отвечает реализация интерфейса
        /// </summary>
        /// <returns></returns>
        TConfig GetConfig();

        /// <summary>
        ///     Получает вкладку, которая отображается пользователю
        /// </summary>
        /// <returns></returns>
        TabPage GetPage();
    }
}