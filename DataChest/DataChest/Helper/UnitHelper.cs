/*
  Copyright (C) 2016. HYE WON, HWANG

  This file is part of DataChest.

  DataChest is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  DataChest is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with DataChest.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;

namespace DataChest {
    /// <summary>
    /// 크기 및 진행 시간을 이용하여 처리 속도를 계산하는 함수를 노출합니다.<br />
    /// Exposes function that computing process speed using size and progress time.
    /// </summary>
    static class UnitHelper {
        enum DataSize : byte {
            B,
            KiB,
            MiB,
            GiB,
            TiB,
            PiB,
        }
        
        public const long PiB = 1125899906842624L;
        public const long TiB = 1099511627776L;
        public const int GiB = 1073741824;
        public const int MiB = 1048576;
        public const int KiB = 1024;
        static readonly double TickFrequency = 10000000.0 / Stopwatch.Frequency;

        static DataSize GetDataSize(double size) {
            if (size >= PiB) return DataSize.PiB;
            if (size >= TiB) return DataSize.TiB;
            if (size >= GiB) return DataSize.GiB;
            if (size >= MiB) return DataSize.MiB;
            if (size >= KiB) return DataSize.KiB;
            return DataSize.B;
        }
        static double GetElapsedTimeFromTick(long tick) {
            return (tick * TickFrequency) / 10000000.0;
        }

        /// <summary>
        /// 처리 속도를 계산합니다.<br />
        /// Compute a process speed.
        /// </summary>
        /// <param name="size">
        /// 처리한 데이터 크기입니다.<br />
        /// Size of proceed data.
        /// </param>
        /// <param name="elapsed">
        /// 처리 시간입니다.<br />
        /// Time of proceed.
        /// </param>
        public static string ComputeSpeed(long size, TimeSpan elapsed) {
            double measured = size / GetElapsedTimeFromTick(elapsed.Ticks);
            DataSize ds = GetDataSize(measured);

            switch (ds) {
                case DataSize.PiB:
                    measured /= PiB;
                    break;
                case DataSize.TiB:
                    measured /= TiB;
                    break;
                case DataSize.GiB:
                    measured /= GiB;
                    break;
                case DataSize.MiB:
                    measured /= MiB;
                    break;
                case DataSize.KiB:
                    measured /= KiB;
                    break;
                default:
                    break;
            }

            return string.Format("{0:F} {1}/s", measured, ds.ToString());
        }

        /// <summary>
        /// 읽을 수 있는 크기 표현으로 바꿉니다.<br />
        /// Change to readable size expression.
        /// </summary>
        /// <param name="size">
        /// 바꿀 크기입니다.<br />
        /// A size to be changed.
        /// </param>
        public static string ToReadableSize(long size) {
            DataSize ds = GetDataSize(size);
            double measured = size;

            switch (ds) {
                case DataSize.PiB:
                    measured = PiB;
                    break;
                case DataSize.TiB:
                    measured /= TiB;
                    break;
                case DataSize.GiB:
                    measured /= GiB;
                    break;
                case DataSize.MiB:
                    measured /= MiB;
                    break;
                case DataSize.KiB:
                    measured /= KiB;
                    break;
                default:
                    break;
            }

            return string.Format("{0:F} {1}", measured, ds.ToString());
        }
    }
}