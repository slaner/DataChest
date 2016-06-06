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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DataChest {
    static class PerformanceLogger {
        static Stopwatch m_sw = new Stopwatch();
        static ulong m;

        static string LastCryptographyPerformance = "<결과 없음>";
        static string LastFileIOPerformance = "<결과 없음>";
        static ulong LastFileSize = 0;
        static TimeSpan LastEntireTaskTime = TimeSpan.MinValue;



        /// <summary>
        /// 성능 기록기를 시작합니다.
        /// </summary>
        public static void Begin() {
            m_sw.Restart();
        }

        /// <summary>
        /// 성능 기록기를 종료합니다.
        /// </summary>
        public static void End() {
            m_sw.Stop();
        }



        static void CalculateSpeed() {

        }
    }
}