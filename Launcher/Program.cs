using System;
using Eto.Forms;

namespace Launcher
{
    /// <summary>
    ///     Класс с точкой входа настроек игры
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     Точка входа настроек игры
        /// </summary>
        [STAThread]
        public static void Main()
        {
#if RELEASE
            try
            {
            #endif
            var application = new Application();
            var form = new LauncherForm();
            application.Run(form);
#if RELEASE
            }
            catch (Exception e)
            {
                using (var writer = new StreamWriter(@"log.log", true))
                {
                    writer.WriteLine(e);
                    writer.WriteLine(e.Message);
                    writer.WriteLine(Environment.StackTrace);
                }
                throw;
            }
            #endif
        }
    }
}