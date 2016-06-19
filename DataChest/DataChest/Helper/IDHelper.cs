using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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