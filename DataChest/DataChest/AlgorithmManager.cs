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
using System.Security.Cryptography;

namespace DataChest {
    /// <summary>
    /// 암,복호화에 사용될 알고리즘을 열거합니다.<br />
    /// Enumerate algorithms that used in cryptographic function.
    /// </summary>
    public enum Algorithms : ushort {
        /// <summary>
        /// 고급 암호화 표준(AES, Advanced Encryption Standard) 알고리즘입니다.<br />
        /// Advanced Encryption Standard(AES) algorithm.
        /// </summary>
        Aes,

        /// <summary>
        /// 데이터 암호화 표준(DES, Data Encryption Standard) 알고리즘입니다.<br />
        /// Data Encryption Standard(DES) algorithm.
        /// </summary>
        Des,

        /// <summary>
        /// 데이터 암호화 표준(DES) 알고리즘을 이용하여 3중으로 수행하는 알고리즘입니다.<br />
        /// A algorithm that perform a cryptographic function with DES triple times.
        /// </summary>
        TripleDes,

        /// <summary>
        /// RC2 알고리즘입니다.<br />
        /// RC2 algorithm.
        /// </summary>
        Rc2,
        




        /// <summary>
        /// 이 열거형의 끝을 나타내기 위한 상수입니다.<br />
        /// Indicate last member of this enumeration.
        /// </summary>
        LastMethod,
    }

    /// <summary>
    /// DataChest 프로그램에서 지원하는 알고리즘을 노출하는 클래스입니다.<br />
    /// A class exposes algorithms that supported DataChest application.
    /// </summary>
    static class AlgorithmManager {
        static readonly Dictionary<int, Type> g_algorithms = new Dictionary<int, Type>();
        static readonly Type BaseAlgorithm = typeof(SymmetricAlgorithm);

        /// <summary>
        /// 사용할 수 있는 알고리즘을 정의합니다.<br />
        /// Define algorithms available.
        /// </summary>
        /// <param name="id">
        /// 알고리즘의 식별 번호입니다.<br />
        /// Identifier number of algorithm.
        /// </param>
        /// <param name="t">
        /// 알고리즘의 형식입니다. (현 버전의 DataChest 프로그램에서는 대칭 키 알고리즘만 지원합니다)<br />
        /// Type of algorithm. (Only symmetric algorithms supported in current version of DataChest application)
        /// </param>
        static void DefineAlgorithm(int id, Type t) {
            if (id < 0) return;
            if (t == null) return;
            if (g_algorithms.ContainsKey(id)) return;
            if (!t.IsSubclassOf(BaseAlgorithm)) return;

            g_algorithms.Add(id, t);
        }
        static AlgorithmManager() {
            // 이 곳에서 DefineAlgorithm(int, Type) 함수를 이용하여
            // DataChest 프로그램에서 지원하는 대칭 키 암호화 알고리즘을 추가합니다.
            // Add algorithms that supported DataChest application by DefineAlgorithm(int, Type) function.

            DefineAlgorithm((int)Algorithms.Aes, typeof(AesCryptoServiceProvider));
            DefineAlgorithm((int)Algorithms.Des, typeof(DESCryptoServiceProvider));
            DefineAlgorithm((int)Algorithms.TripleDes, typeof(TripleDESCryptoServiceProvider));
            DefineAlgorithm((int)Algorithms.Rc2, typeof(RC2CryptoServiceProvider));
        }

        /// <summary>
        /// 식별 번호에 해당하는 알고리즘 개체를 생성합니다.<br />
        /// Create a instance of algorithm that matches identifier number.
        /// </summary>
        /// <param name="id">
        /// 알고리즘의 식별 번호입니다.<br />
        /// Identifier number of algorithm.
        /// </param>
        public static SymmetricAlgorithm CreateAlgorithm(int id) {
            if (!g_algorithms.ContainsKey(id)) return null;
            SymmetricAlgorithm sa;
            try { sa = (SymmetricAlgorithm) Activator.CreateInstance(g_algorithms[id]); }
            catch { return null; }

            return sa;
        }
    }
}