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

namespace DataChest {
    /// <summary>
    /// 고유 식별자를 가져오는 함수를 노출합니다.<br />
    /// Exposes function that receiving unique identifier.
    /// </summary>
    static class IDHelper {
        /// <summary>
        /// 컴퓨터의 NetBIOS 이름과 사용자 이름을 가져옵니다.<br />
        /// Get a computer's NetBIOS name and user name.
        /// </summary>
        public static string MachineUserName() {
            string s;
            try { s = Environment.UserName + "@" + Environment.MachineName; }
            catch { s =  Environment.UserName + "@UNKNOWN_MACHINE"; }
            return s;
        }
    }
}