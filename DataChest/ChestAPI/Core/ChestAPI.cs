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
    /// <summary>
    /// CRC-32 해시 계산에 사용되는 CRC32 개체를 나타냅니다.
    /// </summary>
    public static Crc32 Crc = new Crc32();


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