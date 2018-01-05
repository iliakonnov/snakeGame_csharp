using System;
using System.Collections.Generic;
using Eto.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace snake_game.Bonuses
{
    /// <inheritdoc />
    /// <summary>
    ///     Методы и свойства, помеченные данным аттрибутом, будут доступны у реализаций <see cref="Accessable" />
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AccessableAttribute : Attribute
    {
    }

    /// <summary>
    ///     Предназначен для получения доступа к методам и свойствам в плагинах
    /// </summary>
    public abstract class Accessable
    {
        /// <summary>
        ///     Вызывает и возвращает результат выполнения метода
        /// </summary>
        /// <param name="methodName">Имя метода</param>
        /// <param name="arguments">Аргументы метода</param>
        /// <typeparam name="TResult">Тип возвращаемого значения метода</typeparam>
        /// <returns>Результат выполнения метода</returns>
        /// <exception cref="ArgumentException">В случае, если данный метод не может быть найден</exception>
        /// <exception cref="InvalidCastException">В случае, если указан неверный <typeparamref name="TResult" /></exception>
        /// <exception cref="NullReferenceException">
        ///     В случае, если <paramref name="arguments" /> null, хотя функция принимает
        ///     параметры
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        ///     В случае, если <paramref name="arguments" /> не передаёт все необходимые для
        ///     функции аргументы
        /// </exception>
        public virtual TResult GetMethodResult<TResult>(string methodName, object[] arguments = null)
        {
            throw new ArgumentException($"Cannot execute method '{methodName}'");
        }

        /// <summary>
        ///     Получает значение свойства
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <typeparam name="TResult">Тип свойства</typeparam>
        /// <returns>Значение свойства</returns>
        /// <exception cref="ArgumentException">В случае, если данное свойство не может быть найдено</exception>
        /// <exception cref="InvalidCastException">В случае, если указан неверный <typeparamref name="TResult" /></exception>
        // ReSharper disable once MemberCanBeProtected.Global
        // ReSharper disable once UnusedMemberHierarchy.Global
        public virtual TResult GetProperty<TResult>(string propertyName)
        {
            throw new ArgumentException($"Cannot get property '{propertyName}'");
        }

        /// <summary>
        ///     Получает элементы свойства-списка
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <typeparam name="TResult">Тип элементов</typeparam>
        /// <returns>Список элементов свойства</returns>
        /// <exception cref="ArgumentException">В случае, если данное свойство не может быть найдено</exception>
        /// <exception cref="InvalidCastException">В случае, если указан неверный <typeparamref name="TResult" /></exception>
        public virtual List<TResult> GetListProperty<TResult>(string propertyName)
        {
            throw new ArgumentException($"Cannot get property '{propertyName}'");
        }

        /// <summary>
        ///     Устанавливает значение свойства
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <param name="newValue">Новое значение</param>
        /// <exception cref="ArgumentException">В случае, если данное свойство не может быть найдено</exception>
        /// <exception cref="InvalidCastException">В случае, если <paramref name="newValue" /> неверного типа</exception>
        // ReSharper disable once MemberCanBeProtected.Global
        // ReSharper disable once UnusedMemberHierarchy.Global
        public virtual void SetProperty(string propertyName, object newValue)
        {
            throw new ArgumentException($"Cannot set property '{propertyName}'");
        }

        /// <summary>
        ///     Устанавливает значение элементов свойства-списка
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <param name="newValue">Новые элементы</param>
        /// <exception cref="ArgumentException">В случае, если данное свойство не может быть найдено</exception>
        /// <exception cref="InvalidCastException">В случае, если элементы <paramref name="newValue" /> неверного типа</exception>
        public virtual void SetListProperty(string propertyName, IEnumerable<object> newValue)
        {
            throw new ArgumentException($"Cannot set property '{propertyName}'");
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Базовый класс бонуса у плагинов
    /// </summary>
    public abstract class BonusBase : Accessable
    {
        /// <summary>
        ///     Проверяет, все ли необходимые плагины были загружены
        /// </summary>
        /// <param name="plugins">Список уже загруженых плагинов</param>
        /// <returns>Список необходимых плагинов, которые не были загружены</returns>
        public abstract IEnumerable<string> CheckDependincies(IReadOnlyDictionary<string, BonusBase> plugins);

        /// <summary>
        ///     Соответствует <see cref="Game.LoadContent()" />
        /// </summary>
        /// <param name="gd">См. <see cref="Game.GraphicsDevice" /></param>
        public abstract void LoadContent(GraphicsDevice gd);

        /// <summary>
        ///     Соответствует <see cref="Game.Update(GameTime)" />
        /// </summary>
        /// <param name="time">См. параметр <c>gameTime</c> в <see cref="Game.Update(GameTime)" /></param>
        /// <param name="fullTime">Суммарное время игры в миллисекундах</param>
        /// <param name="keyboard">Состояние клавиатуры в данный тик</param>
        /// <param name="plugins">Все загруженные плагины</param>
        /// <param name="size">Размер экрана</param>
        /// <param name="events">Все события игры за предыдущий тик</param>
        /// <returns>Событие данного плагина. Может быть null</returns>
        public abstract Accessable Update(GameTime time, int fullTime, KeyboardState keyboard,
            IReadOnlyDictionary<string, BonusBase> plugins, Rectangle size,
            IReadOnlyDictionary<string, Accessable> events);

        /// <summary>
        ///     Соответствует <see cref="Game.Draw(GameTime)" />
        /// </summary>
        /// <param name="sb">Уже открытый SpriteBatch</param>
        public abstract void Draw(SpriteBatch sb);
    }

    /// <summary>
    ///     Данный интерфейс должны реализовать настройки каждого плагина
    /// </summary>
    public interface IPluginConfig
    {
        /// <summary>
        ///     Указывает, активирован ли данный плагин
        /// </summary>
        // ReSharper disable once UnusedMemberInSuper.Global
        bool IsEnabled { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Дополнительные функции, которые должны реализовывать плагины на IronPython
    /// </summary>
    public interface IPythonPluginConfig : IPluginConfig
    {
        /// <summary>
        ///     Сериализует настройки плагина
        /// </summary>
        /// <returns>Сериализованные настройки</returns>
        /// ///
        /// <seealso cref="Deserialize(Dictionary{string,string})" />
        Dictionary<string, string> Serialize();

        /// <summary>
        ///     Десериализовывает настройки плагина
        /// </summary>
        /// <param name="data">Сериализованные настройки</param>
        /// <seealso cref="Serialize()" />
        void Deserialize(Dictionary<string, string> data);
    }

    /// <summary>
    ///     Данный интерфейс должен реализовывать каждый плагин
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        ///     Имя плагина. Должно быть уникально среди всех других загруженных плагинов
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Настройки плагина
        /// </summary>
        IPluginConfig Config { get; }

        /// <summary>
        ///     Возвращает класс бонуса
        /// </summary>
        /// <param name="config">Настройки плагина (<see cref="Config" />)</param>
        /// <param name="random">Источник случайных чисел. Рекомендуется использовать только его</param>
        /// <param name="game">Экземпляр ядра игры</param>
        /// <returns>Бонус</returns>
        BonusBase GetBonus(IPluginConfig config, Random random, MainGame.MainGame game);

        /// <summary>
        ///     Возвращает вкладку настроек плагина
        /// </summary>
        /// <param name="config">Настройки плагина (<see cref="Config" />)</param>
        /// <returns>Вкладка настроек</returns>
        IConfigPage GetPage(IPluginConfig config);
    }

    /// <summary>
    ///     Предназначен для редактирования настроек плагина пользователем
    /// </summary>
    public interface IConfigPage
    {
        /// <summary>
        ///     Возвращает настройки, отредактированные пользователем
        /// </summary>
        /// <returns>Настройки плагина</returns>
        IPluginConfig GetConfig();

        /// <summary>
        ///     Возвращает вкладку, которая будет отображаться пользователю
        /// </summary>
        /// <returns></returns>
        TabPage GetPage();
    }
}