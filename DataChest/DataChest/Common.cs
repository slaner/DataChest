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
using System.Text;

namespace DataChest {
    /// <summary>
    /// 공통적으로 사용되는 값들을 정의한 클래스입니다.<br />
    /// A class defining common used values.
    /// </summary>
    static class Common {
        /// <summary>
        /// 운영체제의 유니코드 인코딩을 가져옵니다.<br />
        /// Gets unicode encoding for current Operating System.
        /// </summary>
        public static readonly Encoding SystemEncoding = BitConverter.IsLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode;
    }
}