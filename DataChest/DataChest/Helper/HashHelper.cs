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
using System.IO;
using System.Security.Cryptography;

namespace DataChest {
    /// <summary>
    /// 해시 계산 함수를 정의합니다.<br />
    /// Define a hash computing functions.
    /// </summary>
    static class HashHelper {
        static readonly bool m_initialized = false;
        static readonly SHA256Managed m_sha = null;
        static HashHelper() {
            m_sha = new SHA256Managed();
            m_initialized = true;
        }

        /// <summary>
        /// <see cref="byte"/> 배열에 대한 SHA-256 해시를 계산합니다.<br />
        /// Compute SHA-256 hash for <see cref="byte"/> array.
        /// </summary>
        /// <param name="b">
        /// 해시를 계산할 <see cref="byte"/> 배열입니다.<br />
        /// A <see cref="byte"/> array to be hash computed.
        /// </param>
        public static byte[] Compute(byte[] b) {
            return m_sha.ComputeHash(b);
        }
        /// <summary>
        /// <see cref="Stream"/> 에 대한 SHA-256 해시를 계산합니다.<br />
        /// Compute SHA-256 hash for <see cref="Stream"/>.
        /// </summary>
        /// <param name="s">
        /// 해시를 계산할 <see cref="Stream"/> 입니다.<br />
        /// A <see cref="Stream"/> to be hash computed.
        /// </param>
        public static byte[] Compute(Stream s) {
            return m_sha.ComputeHash(s);
        }
        /// <summary>
        /// <see cref="byte"/> 배열에 대한 SHA-256 해시를 계산하고 부호 없는 32비트 정수로 변환합니다.<br />
        /// Compute SHA-256 hash for <see cref="byte"/> array and convert result to unsigned 32-bit integer.
        /// </summary>
        /// <param name="b">
        /// 해시를 계산할 <see cref="byte"/> 배열입니다.<br />
        /// A <see cref="byte"/> array to be hash computed.
        /// </param>
        public static uint ComputeUInt32(byte[] b) {
            return BitConverter.ToUInt32(Compute(b), 0);
        }
        /// <summary>
        /// <see cref="Stream"/> 에 대한 SHA-256 해시를 계산하고 부호 없는 32비트 정수로 변환합니다.<br />
        /// Compute SHA-256 hash for <see cref="Stream"/> and convert result to unsigned 32-bit integer.
        /// </summary>
        /// <param name="b">
        /// 해시를 계산할 <see cref="Stream"/> 입니다.<br />
        /// A <see cref="Stream"/> to be hash computed.
        /// </param>
        public static uint ComputeUInt32(Stream s) {
            return BitConverter.ToUInt32(Compute(s), 0);
        }

        /// <summary>
        /// 할당한 리소스를 해제합니다.<br />
        /// Release resources allocated.
        /// </summary>
        public static void Cleanup() {
            if (m_initialized && (m_sha != null)) m_sha.Dispose();
        }
    }
}