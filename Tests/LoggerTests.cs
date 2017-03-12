using System;
using System.IO;
using Microsoft.Xna.Framework;
using snake_game.MainGame;
using Xunit;

namespace Tests
{
    public class LoggerTests
    {
        [Fact]
        public void LoggerTest1()
        {
            var config = new Config
            {
                GameConfig = new Config.GameConfigClass
                {
                    FogEnabled = true,
                    BackgroundColor = new Color(1, 36, 85)
                },
                SnakeConfig = new Config.SnakeConfigClass
                {
                    Speed =  36,
                    CircleOffset = 15
                }
            };
            Console.WriteLine(ConfigLoad.Save(config));
            var actions = new[]
            {
                new ControlResult.Result
                {
                    Debug = true,
                    IsExit = false,
                    Turn = new ControlResult.Turn
                    {
                        ReplaceTurn = true,
                        ToTurn = false,
                        TurnDegrees = 31
                    }
                },
                new ControlResult.Result
                {
                    Debug = false,
                    IsExit = false,
                    Turn = new ControlResult.Turn
                    {
                        ReplaceTurn = false,
                        ToTurn = true,
                        TurnDegrees = 71
                    }
                },
                new ControlResult.Result
                {
                    Debug = true,
                    IsExit = true,
                    Turn = new ControlResult.Turn
                    {
                        ReplaceTurn = false,
                        ToTurn = false,
                        TurnDegrees = 0
                    }
                }
            };
            var times = new[] {245, 6820, 8603};
            var file = new MemoryStream();
            var seed = 583902;

            var logger = new Logger(seed, config, file);
            for (var i = 0; i < actions.Length; i++)
            {
                logger.LogAction(times[i], actions[i]);
            }

            var logReader = new LogReader(file);
            Assert.Equal(logReader.GetSeed(), seed);
            for (var i = 0; i < actions.Length; i++)
            {
                Assert.Equal(logReader.GetTime(i), times[i]);
                Assert.Equal(logReader.GetResult(i), actions[i]);
            }
        }
    }
}