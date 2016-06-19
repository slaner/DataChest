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

namespace DataChest {
    static class UnitHelper {
        static string[] dataUnit = { "B", "KB", "MB", "GB", "TB", "PB" };
        static string[] spdUnit = { "B/s", "Kb/s", "Mb/s", "Gb/s", "Tb/s", "Pb/s" };

        public const long PB = 1125899906842624L;
        public const long TB = 1099511627776L;
        public const int GB = 1073741824;
        public const int MB = 1048576;
        public const int KB = 1024;

        static byte GetDataDelimiter(double size) {
            if (size >= PB)
                return 5;
            else if (size >= TB)
                return 4;
            else if (size >= GB)
                return 3;
            else if (size >= MB)
                return 2;
            else if (size >= KB)
                return 1;
            else return 0;
        }
        public static string ToReadableSize(double size, bool speed) {
            byte unit = GetDataDelimiter(size);
            double measured;
            switch (unit) {
                case 5:
                    measured = size / PB;
                    break;
                case 4:
                    measured = size / TB;
                    break;
                case 3:
                    measured = size / GB;
                    break;
                case 2:
                    measured = size / MB;
                    break;
                case 1:
                    measured = size / KB;
                    break;
                default:
                    measured = size;
                    break;
            }

            if (speed) return string.Format("{0:F} {1}", measured, spdUnit[unit]);
            return string.Format("{0:F} {1}", measured, dataUnit[unit]);
        }
    }
}