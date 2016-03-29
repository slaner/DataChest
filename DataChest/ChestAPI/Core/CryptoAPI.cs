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
using System.IO;
using System.Security.Cryptography;

/// <summary>
/// 암호화 및 복호화하는 함수가 정의되어 있는 클래스입니다.
/// </summary>
static class CryptoAPI {
    /// <summary>
    /// 데이터를 암복호화할 때 사용할 버퍼의 크기입니다.
    /// </summary>
    public static int BufferSize = 4096;

    /// <summary>
    /// IV가 지정되지 않은 경우에 기본 값으로 사용되는 값입니다.
    /// </summary>
    public static readonly byte[] DefaultIV = {
        0x45, 0xFC, 0x50, 0x57, 0x57, 0x6A, 0x03, 0x8D,
        0x45, 0xEC, 0x50, 0xFF, 0x75, 0x08, 0xC7, 0x45,
        0xEC, 0x01, 0x00, 0x00, 0x00, 0x57, 0x89, 0x7D,
        0xF0, 0xFF, 0xD6, 0x8B, 0xD8, 0x81, 0xFB, 0x23
    };
    
    
    public static TaskResult Invoke(ChestParams cp, bool encrypt) {
        // 암호화의 경우:
        //  파일 읽기 -> 데이터 암호화 -> 헤더 생성 -> 헤더 + 데이터(암호화) 합침 -> 파일로 생성
        //
        // 복호화의 경우:
        //  파일 읽기 -> 헤더/데이터 분리 -> 데이터 복호화 -> 파일로 생성
        // 암호 및 초기 벡터에 대한 데이터 스트림을 가져온다.

        // 알고리즘을 준비한다.
        TaskResult r;
        SymmetricAlgorithm sa;
        byte[] result;
        PrepareAlgorithm(cp, out sa);

        // 파일 스트림을 읽어온다.
        FileStream fs;
        r = FileHelper.OpenFileStream(cp.InputFile, out fs);
        if (r != TaskResult.Success) {
            sa.Dispose();
            return r;
        }

        // 출력 파일의 경로를 가져온다.
        string output;
        r = FileHelper.GetOutputPath(cp, out output, encrypt);
        if (r != TaskResult.Success) {
            fs.Dispose();
            sa.Dispose();
            return r;
        }

        // 출력 파일이 저장될 스트림 생성
        FileStream ofs = null;
        try {
            // 테스트를 수행하는 경우는 개체를 생성하지 않는다.
            if ( !cp.RunTest)
                ofs = new FileStream(output, cp.Overwrite ? FileMode.Create : FileMode.CreateNew);

        } catch (IOException) {
            fs.Dispose();
            sa.Dispose();
            return TaskResult.FileAlreadyExists;
        } catch {
            // Add More specific exception handler here
            bool AddMoreSpecificExceptionHandlerHere;
            fs.Dispose();
            sa.Dispose();
            return TaskResult.IOError;
        }

        // 오류 검출용 변수
        bool error = false;

        // 암호화를 수행
        if (encrypt) {

            // 실질적인 암호화는 Encrypt 함수 내에서 수행된다.
            r = Encrypt(fs, sa, out result);

            // 오류가 발생한 경우 오류를 리턴한다.
            if (r != TaskResult.Success) {
                fs.Dispose();
                sa.Dispose();
                return r;
            }

            // 검사를 수행하는 경우
            if ( cp.RunTest ) {
                // 암호화가 성공적으로 수행됬으므로 Success 를 반환한다.
                fs.Dispose();
                sa.Dispose();
                return TaskResult.Success;
            }

            // 헤더를 생성하고, 바이트 배열로 변환한다. (파일에 쓰기 위해서)
            CHEST_HEADER hdr = CHEST_HEADER.CreateHeader(cp.Version, fs, result);
            byte[] bHdr = hdr.ToArray();

            // 파일에 데이터를 쓰기 위해 BinaryWriter 개체를 만든다.
            using (BinaryWriter bw = new BinaryWriter(ofs)) {
                try {
                    // 헤더를 기록하고
                    bw.Write(bHdr);

                    // 버퍼 크기만큼 분할하여 파일 스트림에 기록한다.
                    int loop_count = result.Length / BufferSize;
                    int remain = result.Length % BufferSize;
                    for (int i = 0; i < loop_count; i++)
                        bw.Write(result, i * BufferSize, BufferSize);

                    if (remain > 0)
                        bw.Write(result, loop_count * BufferSize, remain);
                } catch {
                    // 위의 코드를 보면 ArgumentException, ArgumentNullException 및 ArgumentOutOfRangeException 예외는 뜰 가능성이 전혀 없다.
                    // IOException 예외만 처리해주면 될듯.
                    error = true;
                }
            }

            // 스트림 기록 중 오류가 발생한 경우
            if ( error ) {
                sa.Dispose();
                fs.Dispose();
                return TaskResult.StreamWriteError;
            }

            // 정리 옵션이 켜진 경우
            if (cp.Cleanup) {
                try {
                    File.Delete(cp.InputFile);
                } catch {
                    sa.Dispose();
                    fs.Dispose();
                    return TaskResult.CleanupFailedSucceed;
                }
            }
        } else {

            // 파일로부터 헤더를 구성한다.
            CHEST_HEADER hdr;
            r = CHEST_HEADER.FromStream(fs, out hdr);

            // 실패!
            if (r != TaskResult.Success) {
                fs.Dispose();
                sa.Dispose();
                return r;
            }

            // 검사를 수행하는 경우
            if (cp.RunTest) {
                // 암호화가 성공적으로 수행됬으므로 Success 를 반환한다.
                fs.Dispose();
                sa.Dispose();
                return TaskResult.Success;
            }

            // 실질적인 복호화는 Decrypt 함수 내에서 수행된다.
            r = Decrypt(fs, sa, out result);
            if (r != TaskResult.Success) {
                fs.Dispose();
                sa.Dispose();
                return r;
            }

            // 파일에 데이터를 쓰기 위해 BinaryWriter 개체를 만든다.
            // 여기서 쓰는 데이터는 복호화된 데이터가 쓰여지게 된다.
            using (BinaryWriter bw = new BinaryWriter(ofs)) {
                try {
                    // 버퍼 크기만큼 분할하여 파일 스트림에 기록한다.
                    int loop_count = result.Length / BufferSize;
                    int remain = result.Length % BufferSize;
                    for (int i = 0; i < loop_count; i++)
                        bw.Write(result, i * BufferSize, BufferSize);

                    if (remain > 0)
                        bw.Write(result, loop_count * BufferSize, remain);
                } catch {
                    // 위의 코드를 보면 ArgumentException, ArgumentNullException 및 ArgumentOutOfRangeException 예외는 뜰 가능성이 전혀 없다.
                    // IOException 예외만 처리해주면 될듯.
                    error = true;
                }
            }

            // 스트림 기록 중 오류가 발생한 경우
            if (error) {
                sa.Dispose();
                fs.Dispose();
                return TaskResult.StreamWriteError;
            }

            // 정리 옵션이 켜진 경우
            if (cp.Cleanup) {
                try {
                    File.Delete(cp.InputFile);
                } catch {
                    sa.Dispose();
                    fs.Dispose();
                    return TaskResult.CleanupFailedSucceed;
                }
            }
        }
        
        sa.Dispose();
        fs.Dispose();
        return TaskResult.Success;
    }

    static byte[] GetCompatibleHashForSize(Stream s, int size) {
        SHA256 hasher = SHA256.Create();
        byte[] hash = hasher.ComputeHash(s);
        byte[] result = new byte[size];
        Array.Copy(hash, result, size);
        return result;
    }
    static SymmetricAlgorithm CreateAlgorithm(Algorithms alg, out int keySize) {
        switch (alg) {
            case Algorithms.Aes:
                keySize = 128;
                return Aes.Create();

            case Algorithms.Des:
                keySize = 64;
                return DES.Create();

            case Algorithms.TripleDes:
                keySize = 128;
                return TripleDES.Create();

            case Algorithms.Rc2:
                keySize = 40;
                return RC2.Create();

            default:
                keySize = 0;
                return null;
        }
    }
    static TaskResult Encrypt(Stream s, SymmetricAlgorithm sa, out byte[] result) {
        result = null;
        MemoryStream ms = new MemoryStream();

        ICryptoTransform trans = sa.CreateEncryptor();
        using (CryptoStream cs = new CryptoStream(ms, trans, CryptoStreamMode.Write)) {
            byte[] temp;
            try {
                temp = new byte[BufferSize];
            } catch {
                cs.Dispose();
                return TaskResult.OutOfMemory;
            }
            int nRead;
            try {
                nRead = s.Read(temp, 0, BufferSize);
            } catch {
                cs.Dispose();
                return TaskResult.StreamReadError;
            }
            while (nRead > 0) {
                try {
                    cs.Write(temp, 0, nRead);
                    nRead = s.Read(temp, 0, BufferSize);
                } catch {
                    cs.Dispose();
                    return TaskResult.StreamReadError;
                }
            }
        }

        result = ms.ToArray();
        return TaskResult.Success;
    }
    static TaskResult Decrypt(Stream s, SymmetricAlgorithm sa, out byte[] result) {
        result = null;
        MemoryStream ms = new MemoryStream();

        ICryptoTransform trans = sa.CreateDecryptor();
        using (CryptoStream cs = new CryptoStream(s, trans, CryptoStreamMode.Read)) {
            byte[] temp;
            try {
                temp = new byte[BufferSize];
            } catch {
                cs.Dispose();
                return TaskResult.OutOfMemory;
            }
            int nRead;
            try {
                nRead = cs.Read(temp, 0, BufferSize);
            } catch {
                cs.Dispose();
                return TaskResult.StreamReadError;
            }
            while (nRead > 0) {
                try {
                    ms.Write(temp, 0, nRead);
                    nRead = cs.Read(temp, 0, BufferSize);
                } catch {
                    cs.Dispose();
                    return TaskResult.StreamReadError;
                }
            }
        }

        result = ms.ToArray();
        ms.Dispose();
        return TaskResult.Success;
    }
    static TaskResult PrepareAlgorithm(ChestParams cp, out SymmetricAlgorithm sa) {
        int keySize;
        sa = CreateAlgorithm(cp.Algorithm, out keySize);
        if (sa == null)
            return TaskResult.InvalidAlgorithm;

        TaskResult r;
        Stream pw, iv;
        bool skipIV = false;

        r = cp.GetPasswordDataStream(out pw);
        if (r != TaskResult.Success) return r;

        r = cp.GetIVDataStream(out iv);
        if (r == TaskResult.NoIV) skipIV = true;
        else if (r != TaskResult.Success) return r;

        // 해시로 변형한 후 키로 사용하는 경우:
        byte[] bpw, biv = null;
        int ks = sa.KeySize / 8;

        bpw = GetCompatibleHashForSize(pw, ks);
        pw.Dispose();
        sa.Key = bpw;
        if (!skipIV) {
            biv = GetCompatibleHashForSize(iv, ks);
            iv.Dispose();
        } else {
            biv = new byte[sa.IV.Length];
            Buffer.BlockCopy(DefaultIV, 0, biv, 0, biv.Length);
        }
        sa.IV = biv;

        return TaskResult.Success;
    }

}