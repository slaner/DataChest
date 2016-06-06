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
    /// ChestAPI 헤더를 구현합니다.
    /// </summary>
    interface IHeader {
        /// <summary>
        /// 시그니쳐를 가져옵니다.
        /// </summary>
        ushort Signature { get; }

        /// <summary>
        /// 버전을 가져옵니다.
        /// </summary>
        ushort Version { get; }

        /// <summary>
        /// 사용되지 않습니다.
        /// </summary>
        [Obsolete()]
        uint Unused { get; }

        /// <summary>
        /// 헤더의 체크섬을 가져옵니다.
        /// </summary>
        uint HChecksum { get; }

        /// <summary>
        /// 암호화된 데이터의 체크섬을 가져옵니다.
        /// </summary>
        uint EChecksum { get; }

        /// <summary>
        /// 원본 데이터의 체크섬을 가져옵니다.
        /// </summary>
        uint RChecksum { get; }

        /// <summary>
        /// 헤더의 크기를 가져옵니다.
        /// </summary>
        ushort HSize { get; }

        /// <summary>
        /// 암호화된 데이터의 크기를 가져옵니다.
        /// </summary>
        ulong ESize { get; }

        /// <summary>
        /// 원본 데이터의 크기를 가져옵니다.
        /// </summary>
        ulong RSize { get; }
    }
}