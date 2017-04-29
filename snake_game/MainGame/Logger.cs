using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DamienG.Security.Cryptography;

namespace snake_game.MainGame
{
    public class Logger
    {
        /// <summary>
        /// Format:
        /// 1. Random seed, 4 bytes
        /// 2. CRC32 hash of config string, 4 bytes
        /// 3. User actions, 8 bytes:
        /// 3.1. time, 4 bytes
        /// 3.2. action, 4 bytes:
        /// 3.2.1. Bools, 1 byte.
        /// 3.2.2. Turn degrees, 2 bytes.
        /// 3.2.3. Separator: 0x255
        /// </summary>
        readonly Stream _logFile;

        public Logger(int seed, Config config)
        {
            _logFile = File.Create($"snakeGame.log.{seed}.binlog");
            Write(seed);

            var crc = Crc32.Compute(Encoding.UTF8.GetBytes(ConfigLoad.Save(config)));
            Write(crc);
        }

        public Logger(int seed, Config config, Stream logFile)
        {
            _logFile = logFile;
            Write(seed);

            var crc = Crc32.Compute(Encoding.UTF8.GetBytes(ConfigLoad.Save(config)));
            Write(crc);
        }

        public void LogAction(int time, ControlResult.Result action)
        {
            Write(time);

            var bools = new BoolArray
            {
                [0] = action.Debug,
                [1] = action.IsExit,
                [2] = action.Turn.ReplaceTurn,
                [3] = action.Turn.ToTurn
            };
            _logFile.WriteByte(bools.Data);

            var turnDegrees = (short) (action.Turn.TurnDegrees % 360);
            Write(turnDegrees);
            _logFile.WriteByte(255);
        }

        void Write(short data)
        {
            var dataBytes = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian) Array.Reverse(dataBytes);
            foreach (var b in dataBytes)
            {
                _logFile.WriteByte(b);
            }
        }

        void Write(int data)
        {
            var dataBytes = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian) Array.Reverse(dataBytes);
            foreach (var b in dataBytes)
            {
                _logFile.WriteByte(b);
            }
        }

        void Write(uint data)
        {
            var dataBytes = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian) Array.Reverse(dataBytes);
            foreach (var b in dataBytes)
            {
                _logFile.WriteByte(b);
            }
        }
    }

    public class LogReader
    {
        readonly Stream _logFile;

        public LogReader(string logPath)
        {
            _logFile = File.OpenRead(logPath);
        }

        public LogReader(Stream logFile)
        {
            _logFile = logFile;
        }

        public int GetSeed()
        {
            _logFile.Seek(0, SeekOrigin.Begin);

            return ReadInt();
        }

        public uint GetCRC()
        {
            _logFile.Seek(4, SeekOrigin.Begin);

            return ReadUInt();
        }

        public ControlResult.Result GetResult(int n)
        {
            var start = 8 + 8 * n + 4; // meta + offset + time
            _logFile.Seek(start, SeekOrigin.Begin);

            var bools = new BoolArray((byte) _logFile.ReadByte());
            var turn = (int) ReadShort();

            return new ControlResult.Result
            {
                Debug = bools[0],
                IsExit = bools[1],
                Turn = new ControlResult.Turn
                {
                    ReplaceTurn = bools[2],
                    ToTurn = bools[3],
                    TurnDegrees = turn
                }
            };
        }

        public int GetTime(int n)
        {
            var start = 8 + 8 * n + 4; // meta + offset
            _logFile.Seek(start, SeekOrigin.Begin);

            return ReadInt();
        }

        short ReadShort()
        {
            var dataBytes = new byte[2];
            for (var i = 0; i < dataBytes.Length; i++)
            {
                dataBytes[i] = (byte) _logFile.ReadByte();
            }
            if (BitConverter.IsLittleEndian) Array.Reverse(dataBytes);
            return BitConverter.ToInt16(dataBytes, 0);
        }

        int ReadInt()
        {
            var dataBytes = new byte[4];
            for (var i = 0; i < dataBytes.Length; i++)
            {
                dataBytes[i] = (byte) _logFile.ReadByte();
            }
            if (BitConverter.IsLittleEndian) Array.Reverse(dataBytes);
            return BitConverter.ToInt32(dataBytes, 0);
        }

        uint ReadUInt()
        {
            var dataBytes = new byte[4];
            for (var i = 0; i < dataBytes.Length; i++)
            {
                dataBytes[i] = (byte) _logFile.ReadByte();
            }
            if (BitConverter.IsLittleEndian) Array.Reverse(dataBytes);
            return BitConverter.ToUInt32(dataBytes, 0);
        }
    }

    class BoolArray
    {
        byte _data;
        public byte Data => _data;

        public BoolArray()
        {
            _data = 0;
        }

        public BoolArray(byte data)
        {
            _data = data;
        }

        public BoolArray(IReadOnlyList<bool> boolArray)
        {
            for (var i = 0; i < boolArray.Count; i++)
            {
                this[i] = boolArray[i];
            }
        }

        public bool this[int index]
        {
            get
            {
                if (index < 0 || index >= 8)
                    throw new ArgumentOutOfRangeException();

                return ((_data >> index) & 1) != 0;
            }
            set
            {
                if (index < 0 || index >= 8)
                    throw new ArgumentOutOfRangeException();

                if (value)
                    _data = (byte) (_data | (1 << index));
                else
                    _data = (byte) (_data & ~(1 << index));
            }
        }
    }
}