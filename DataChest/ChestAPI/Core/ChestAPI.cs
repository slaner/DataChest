/*
  Copyright (c) 2016 HYE WON, HWANG

  Permission is hereby granted, free of charge, to any person
  obtaining a copy of this software and associated documentation
  files (the "Software"), to deal in the Software without
  restriction, including without limitation the rights to use,
  copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the
  Software is furnished to do so, subject to the following
  conditions:

  The above copyright notice and this permission notice shall be
  included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
  OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
  HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
  OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

static class ChestAPI {
    /// <summary>
    /// 시스템에 대한 유니코드 인코딩을 나타냅니다.
    /// </summary>
    public static Encoding SystemUnicodeEncoding = BitConverter.IsLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode;
    /// <summary>
    /// ChestAPI 버전을 나타냅니다.
    /// </summary>
    public static ushort Version = 1;
    static SHA256 SHA = SHA256.Create();


    /// <summary>
    /// 파일에 대한 암호화 또는 복호화 작업을 수행합니다.
    /// </summary>
    /// <param name="cp">API 호출에 필요한 옵션 정보를 담고 있는 ChestParams 클래스의 개체입니다.</param>
    public static TaskResult Invoke(ChestParams cp) {
        // 옵션이 유효한지 확인한다.
        TaskResult r = VerifyParameters(cp);

        // 유효하지 않다면 종료한다.
        if (r != TaskResult.Success) return r;

        // 호출!
        return CryptoAPI.Invoke(cp, cp.Encrypt);
    }

    /// <summary>
    /// 스트림에 대한 해시를 계산합니다.
    /// </summary>
    /// <param name="s">해시를 계산할 스트림입니다.</param>
    public static uint ComputeHash(Stream s) {
        byte[] r = SHA.ComputeHash(s);
        return BitConverter.ToUInt32(r, 0);
    }

    /// <summary>
    /// 바이트 배열에 대한 해시를 계산합니다.
    /// </summary>
    /// <param name="buffer">해시를 계산할 바이트 배열입니다.</param>
    public static uint ComputeHash(byte[] buffer) {
        byte[] r = SHA.ComputeHash(buffer);
        return BitConverter.ToUInt32(r, 0);
    }
    
    static TaskResult VerifyParameters(ChestParams cp) {
        // 입력 파일이 없는 경우
        if (string.IsNullOrEmpty(cp.InputFile)) return TaskResult.InvalidParameter;

        // 잘못된 버전이 입력된 경우
        if (cp.Version > Version) return TaskResult.InvalidVersion;

        // 정의되지 않은 알고리즘을 사용한 경우
        if (cp.Algorithm >= Algorithms.LastMethod) return TaskResult.InvalidAlgorithm;
        
        // 검증 끝
        return TaskResult.Success;
    }
    static TaskResult VerifyChestHeaderForFile(ChestParams cp) {
        FileStream fs;
        TaskResult r = FileHelper.OpenFileStream(cp.InputFile, out fs);
        if (r != TaskResult.Success) return r;
        CHEST_HEADER hdr;
        r = CHEST_HEADER.FromStream(fs, out hdr);
        fs.Dispose();
        return r;
    }
}