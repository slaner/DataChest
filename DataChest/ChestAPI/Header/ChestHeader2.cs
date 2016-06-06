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
using System.IO;

namespace DataChest {
    /// <summary>
    /// ChestAPI 파일 형식에서 사용하는 버전 2 헤더입니다.
    /// </summary>
    class ChestHeader2 : HeaderBase {
        string g_comment;
        public ChestHeader2() : base(2) { }

        protected override void ProcessArray(BinaryWriter bw) {
            if (string.IsNullOrEmpty(g_comment))
                bw.Write((ushort)0);
            else {
                byte[] bComment = ChestAPI.SystemUnicodeEncoding.GetBytes(g_comment);
                bw.Write(CommentLength);
                bw.Write(bComment);
            }
        }
        protected override void ProcessStream(BinaryReader br) {
            ushort commentLength = br.ReadUInt16();
            if (commentLength == 0) return;

            byte[] bComment = br.ReadBytes(commentLength);
            g_comment = ChestAPI.SystemUnicodeEncoding.GetString(bComment);
        }

        /// <summary>
        /// 필드 버전이 아닌 헤더의 버전을 가져옵니다.
        /// 상위 버전의 헤더를 만든 경우 이 속성이 정확한 헤더 버전을 반환하도록 재정의해야 합니다.
        /// </summary>
        public override ushort HeaderVersion {
            get { return 2; }
        }
        /// <summary>
        /// 코멘트의 길이를 가져옵니다. 코멘트는 유니코드로 저장되기 때문에 이 값은 원래 길이의 배수입니다.
        /// </summary>
        public ushort CommentLength {
            get {
                if (g_comment == null) return 0;
                return (ushort)(g_comment.Length * 2);
            }
        }
        /// <summary>
        /// 코멘트를 가져오거나 설정합니다.
        /// </summary>
        public string Comment {
            get { return g_comment; }
            set {
                if (value == null) return;

                // 32727 = short.MaxValue - 40(HeaderBaseSize + 2 for comment length)
                if (value.Length > 32727) return;
                g_comment = value;
                HSize = (ushort)(HeaderBaseSize + 2 + (g_comment.Length * 2));
            }
        }
    }
}