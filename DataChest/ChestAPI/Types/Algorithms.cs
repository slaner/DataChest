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

namespace DataChest.Types {
    /// <summary>
    /// 암,복호화에 사용될 알고리즘을 열거합니다.
    /// SEED 알고리즘은 성능으로 인해 제외되었습니다.
    /// </summary>
    public enum Algorithms : ushort {
        Aes,
        Des,
        TripleDes,
        // 2016 03 06 REMOVED
        // PERFORMANCE ISSUE
        // Seed128,
        // Seed256,
        Rc2,







        LastMethod,
    }
}