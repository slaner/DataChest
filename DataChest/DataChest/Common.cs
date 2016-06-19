using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataChest {
    static class Common {
        /// <summary>
        /// 운영체제의 유니코드 인코딩을 가져옵니다.<br />
        /// Gets unicode encoding for current Operating System.
        /// </summary>
        public static readonly Encoding SystemEncoding = BitConverter.IsLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode;


    }
}