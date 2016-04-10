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
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// 파일을 암호화하고 복호화하는 작업을 노출하는 클래스입니다.
/// </summary>
static class ChestAPI {
    /// <summary>
    /// 시스템에 대한 유니코드 인코딩을 나타냅니다.
    /// </summary>
    public static readonly Encoding SystemUnicodeEncoding = BitConverter.IsLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode;
    /// <summary>
    /// IV가 지정되지 않은 경우에 기본 값으로 사용되는 값입니다.
    /// 이 DefaultIV 바이트 배열은 "DataChest v0.1-alpha Default IV Seed" 라는 내용을 가진 파일(바이너리 편집기로 작성됨)을
    /// "DataChest v0.1-alpha Seed for Password" 라는 문자열로 암호화한 후 암호화된 내용의 상위 32바이트입니다.
    /// 이 때 사용된 IV는 0으로 채워진 바이트 배열이 사용되었습니다.
    /// </summary>
    public static readonly byte[] DefaultIV = {
        0xBF, 0x67, 0x8B, 0x27, 0xDC, 0xDE, 0x98, 0xB9,
        0xDD, 0x44, 0x77, 0x13, 0x85, 0x08, 0xB5, 0x58,
        0x20, 0x58, 0xE6, 0x6F, 0x5E, 0x0F, 0x66, 0x90,
        0x3E, 0x9E, 0x11, 0xB3, 0xC1, 0x04, 0x11, 0x2E,
    };
    /// <summary>
    /// ChestAPI 버전을 나타냅니다.
    /// </summary>
    public const ushort Version = 2;
    /// <summary>
    /// 데이터를 암복호화할 때 사용할 버퍼의 크기입니다.
    /// </summary>
    public static int BufferSize = 4096;


    /// <summary>
    /// 암복호화 API 호출에 필요한 정보를 저장하고 있는 <see cref="ChestParams" /> 개체를 이용하여 암복호화 API를 호출합니다.
    /// </summary>
    /// <param name="cp">암복호화 API 호출에 필요한 정보를 저장하고 있는 <see cref="ChestParams" /> 개체입니다.</param>
    public static TaskResult Invoke(ChestParams cp) {
        // 입력 파일이 없는 경우
        // 잘못된 버전이 입력된 경우
        // 정의되지 않은 알고리즘을 사용한 경우
        if (string.IsNullOrEmpty(cp.InputFile)) return TaskResult.InvalidParameter;
        if (cp.Version > Version) return TaskResult.InvalidVersion;
        if (cp.Algorithm >= Algorithms.LastMethod) return TaskResult.InvalidAlgorithm;

        SymmetricAlgorithm sa;
        TaskResult r = InitAlgorithm(cp, out sa);

        // 알고리즘 초기화에 실패한 경우
        if (r != TaskResult.Success) return r;

        // 파일을 읽어온다.
        FileStream fs;
        r = FileHelper.OpenFileStream(cp.InputFile, out fs);

        // 파일을 읽어오지 못한 경우
        if (r != TaskResult.Success) {
            sa.Dispose();
            return r;
        }

        // 파일에 대한 테스트를 수행할 경우
        #region Run Test Handler
        if (cp.RunTest) {
            // 체크섬 및 암.복호화 결과를 저장하기 위한 변수를 선언한다.
            byte[] rtResult;

            HeaderBase rtHdr = null;
            uint hash;

            // 복호화일 경우 헤더를 먼저 가져온다.
            if (!cp.Encrypt) {
                r = HeaderBase.FromStreamWithVersion(fs, cp.Version, out rtHdr);

                // 헤더를 가져오지 못한 경우
                if (r != TaskResult.Success) {
                    Disposes(sa, fs);
                    return r;
                }

                // 복호화하기 전 암호화된 데이터에 대한 체크섬을 검증한다.
                if (cp.Verify) {
                    long lnPrevPos = fs.Position;
                    hash = HashAPI.ComputeHashUInt32(fs);

                    // 체크섬 값을 비교한다.
                    if (rtHdr.EChecksum != hash) {
                        Disposes(sa, fs);
                        return TaskResult.IncorrectEncryptedDataChecksum;
                    }

                    // 스트림의 위치를 원래 위치로 되돌린다.
                    fs.Seek(lnPrevPos, SeekOrigin.Begin);
                }
            }

            // 암호화/복호화 선택하여 호출한다.
            r = cp.Encrypt ? Encrypt(fs, sa, out rtResult) : Decrypt(fs, sa, out rtResult);

            // 실패한 경우 사용하는 리소스를 메모리에서 해제한 후 실패 코드를 반환한다.
            if (r != TaskResult.Success) {
                Disposes(sa, fs);
                return r;
            }

            // 체크섬 검증을 하지 않거나 파일을 암호화한 경우 체크섬 검증을 건너뛴다.
            if (!cp.Verify || cp.Encrypt) {
                Disposes(sa, fs);
                return r;
            }

            // 복호화된 데이터에 대한 체크섬을 계산한다.
            hash = HashAPI.ComputeHashUInt32(rtResult);

            // 체크섬 비교
            if (rtHdr.RChecksum != hash) {
                Disposes(sa, fs);
                return TaskResult.IncorrectRawDataChecksum;
            }

            // 사용한 개체 제거
            Disposes(sa, fs);

            // 성공!
            return TaskResult.Success;
        }
        #endregion

        // 출력 파일이 생성될 경로를 만든다.
        string output;
        r = FileHelper.BuildOutput(cp, out output);

        // 경로를 만들지 못한 경우
        if (r != TaskResult.Success) {
            Disposes(sa, fs);
            return r;
        }

        FileStream ofs;
        try { ofs = new FileStream(output, cp.Overwrite ? FileMode.Create : FileMode.CreateNew); } catch (IOException) {
            Disposes(sa, fs);
            return TaskResult.FileAlreadyExists;
        }
        // 처리되지 않은 예외
        catch {
            Disposes(sa, fs);
            return TaskResult.IOError;
        }

        // 복호화를 하고 체크섬 검증을 하는 경우
        if (!cp.Encrypt && cp.Verify) {
            long lnPrevPos = fs.Position;
            uint hash = HashAPI.ComputeHashUInt32(fs);

            // 암호화된 
        }

        // 공용적으로 사용되는 변수 선언
        byte[] result;

        #region Encrypt handler
        // 암호화의 경우
        if (cp.Encrypt) {
            // 암호화 수행 및 실패 시 처리
            r = Encrypt(fs, sa, out result);
            if (r != TaskResult.Success) {
                Disposes(sa, fs, ofs);
                FileHelper.DeleteFileIgnoreErrors(output);
                return r;
            }

            // 헤더를 만든다.
            HeaderBase hdr = HeaderBase.CreateEmptyHeader(cp.Version);

            // 커멘트가 있고 버전이 2 이상인 경우
            if (!string.IsNullOrEmpty(cp.Comment) && cp.Version >= 2) {
                ((ChestHeader2)hdr).Comment = cp.Comment;
            }

            // 헤더의 정보를 기록합니다.
            r = hdr.AssignBasicInformationEncrypt(fs, result);
            if (r != TaskResult.Success) return r;

            // 헤더를 바이트 배열로 변환합니다.
            byte[] bHeader;
            r = hdr.ToArray(out bHeader);
            if (r != TaskResult.Success) return r;

            // 파일에 쓰기 위해 BinaryWriter 개체를 생성한다.
            try {
                using (BinaryWriter bw = new BinaryWriter(ofs)) {
                    // 헤더를 먼저 기록한다.
                    bw.Write(bHeader);

                    // TODO: 64-bit integer support
                    // 암호화된 데이터를 기록한다.
                    int loop_count = result.Length / BufferSize;
                    int remain = result.Length % BufferSize;
                    for (int i = 0; i < loop_count; i++)
                        bw.Write(result, i * BufferSize, BufferSize);

                    if (remain > 0)
                        bw.Write(result, loop_count * BufferSize, remain);
                }
            }
            // TODO: additional exception handler
            // 예외가 발생하는 이유는 IOException 뿐
            catch {
                Disposes(sa, fs);
                FileHelper.DeleteFileIgnoreErrors(output);
                return TaskResult.StreamWriteError;
            }

            // 파일에 모두 기록 성공.
            // 정리(Cleanup) 옵션이 설정되어 있다면 입력 파일을 삭제한다.
            if (cp.Cleanup) {
                // TODO: Multiple file support?
                try { File.Delete(cp.InputFile); } catch {
                    Disposes(sa, fs);
                    return TaskResult.CleanupFailedSucceed;
                }
            }
        }
        #endregion

        #region Decrypt handler
        // 복호화의 경우
        else {
            // 파일에서 헤더 정보를 가져온다.
            HeaderBase hdr;
            r = HeaderBase.FromStreamWithVersion(fs, cp.Version, out hdr);

            // 헤더 정보를 가져오지 못한 경우
            if (r != TaskResult.Success) {
                Disposes(fs, sa, ofs);
                FileHelper.DeleteFileIgnoreErrors(output);
                return r;
            }

            // 체크섬 검증을 하는 경우
            if (cp.Verify) {
                // 암호화된 데이터의 체크섬 값과 헤더의 e_checksum 값이 일치하는지 검사한다.
                long lnPrevPos = fs.Position;
                uint prevHash = HashAPI.ComputeHashUInt32(fs);

                // 일치하지 않는 경우
                if (hdr.EChecksum != prevHash) {
                    Disposes(fs, sa, ofs);
                    FileHelper.DeleteFileIgnoreErrors(output);
                    return TaskResult.IncorrectEncryptedDataChecksum;
                }

                // 파일 스트림의 위치를 원래대로 돌려놓는다.
                fs.Seek(lnPrevPos, SeekOrigin.Begin);
            }

            // 복호화 수행 및 실패 시 처리
            r = Decrypt(fs, sa, out result);
            if (r != TaskResult.Success) {
                Disposes(sa, fs, ofs);
                FileHelper.DeleteFileIgnoreErrors(output);
                return r;
            }

            // 복호화가 완료된 후 데이터의 체크섬 검증을 수행한다.
            if (cp.Verify) {
                if (HashAPI.ComputeHashUInt32(result) != hdr.RChecksum) {
                    Disposes(fs, sa, ofs);
                    FileHelper.DeleteFileIgnoreErrors(output);
                    return TaskResult.IncorrectRawDataChecksum;
                }
            }

            // 파일에 데이터를 쓰기 위해 BinaryWriter 개체를 만든다.
            // 여기서 쓰는 데이터는 복호화된 데이터가 쓰여지게 된다.
            try {
                using (BinaryWriter bw = new BinaryWriter(ofs)) {
                    // 버퍼 크기만큼 분할하여 파일 스트림에 기록한다.
                    int loop_count = result.Length / BufferSize;
                    int remain = result.Length % BufferSize;
                    for (int i = 0; i < loop_count; i++)
                        bw.Write(result, i * BufferSize, BufferSize);

                    if (remain > 0)
                        bw.Write(result, loop_count * BufferSize, remain);
                }
            }
            // TODO: additional exception handler
            // 예외가 발생하는 이유는 IOException 뿐
            catch {
                Disposes(sa, fs);
                FileHelper.DeleteFileIgnoreErrors(output);
                return TaskResult.StreamWriteError;
            }

            // 파일에 모두 기록 성공.
            // 정리(Cleanup) 옵션이 설정되어 있다면 입력 파일을 삭제한다.
            if (cp.Cleanup) {
                // TODO: Multiple file support?
                try { File.Delete(cp.InputFile); } catch {
                    Disposes(sa, fs);
                    return TaskResult.CleanupFailedSucceed;
                }
            }
        }
        #endregion

        // 작업 끝
        return r;
    }

    /// <summary>
    /// <see cref="ChestParams" /> 개체로부터 <see cref="SymmetricAlgorithm" /> 개체를 생성하고 초기화합니다.
    /// </summary>
    /// <param name="cp">필요한 정보를 저장하고 있는 <see cref="ChestParams" /> 개체입니다.</param>
    /// <param name="sa">생성된 <see cref="SymmetricAlgorithm" /> 개체가 저장될 변수입니다.</param>
    static TaskResult InitAlgorithm(ChestParams cp, out SymmetricAlgorithm sa) {
        // 알고리즘 생성
        sa = CreateAlgorithm(cp.Algorithm);

        // 유효하지 않은 알고리즘인 경우 
        if (sa == null) return TaskResult.InvalidAlgorithm;

        TaskResult r;
        Stream pw, iv;

        // 암호에 대한 스트림을 가져온다.
        r = cp.GetPasswordDataStream(out pw);
        // 실패한 경우 종료한다.
        if (r != TaskResult.Success) return r;
        // 성공한 경우, 체크섬를 계산하고 키로 사용한다.
        else sa.Key = HashAPI.ComputeHash(pw);

        // 암호 스트림을 해제한다.
        pw.Dispose();

        // IV에 대한 스트림을 가져온다.
        r = cp.GetIVDataStream(out iv);
        // IV가 없는 경우 DefaultIV 를 사용한다.
        if (r == TaskResult.NoIV) {
            byte[] temp = new byte[sa.IV.Length];
            Buffer.BlockCopy(DefaultIV, 0, temp, 0, temp.Length);
            sa.IV = temp;
        }
        // 실패한 경우 종료한다.
        else if (r != TaskResult.Success) return r;
        // 성공한 경우, 체크섬를 계산하고 IV로 사용한다.
        else {
            sa.IV = HashAPI.ComputeHash(iv);
            // IV 스트림을 해제한다.
            iv.Dispose();
        }

        return TaskResult.Success;
    }
    /// <summary>
    /// <see cref="Algorithms" /> 상수 값에 맞는 <see cref="SymmetricAlgorithm" /> 개체를 생성합니다.
    /// </summary>
    /// <param name="alg">생성할 <see cref="SymmetricAlgorithm" /> 개체를 결정하는 <see cref="Algorithms" /> 상수입니다.</param>
    static SymmetricAlgorithm CreateAlgorithm(Algorithms alg) {
        switch (alg) {
            case Algorithms.Aes:
                return Aes.Create();

            case Algorithms.Des:
                return DES.Create();

            case Algorithms.TripleDes:
                return TripleDES.Create();

            case Algorithms.Rc2:
                return RC2.Create();

            default:
                return null;
        }
    }
    /// <summary>
    /// <see cref="Stream"/> 개체의 데이터를 암호화합니다.
    /// </summary>
    /// <param name="s">데이터를 암호화할 <see cref="Stream" /> 개체입니다.</param>
    /// <param name="sa">데이터 암호화에 사용되는 <see cref="SymmetricAlgorithm" /> 개체입니다.</param>
    /// <param name="result">암호화된 데이터가 저장되는 <see cref="byte" /> 배열입니다.</param>
    static TaskResult Encrypt(Stream s, SymmetricAlgorithm sa, out byte[] result) {
        result = null;
        MemoryStream ms = new MemoryStream();

        ICryptoTransform ct = sa.CreateEncryptor();
        using (CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write)) {
            byte[] temp;

            // 배열 선언
            try { temp = new byte[BufferSize]; }

            // 예외가 발생하면 100% 메모리 부족 예외다.
            catch {
                ct.Dispose();
                ms.Dispose();
                return TaskResult.OutOfMemory;
            }

            // 읽은 데이터의 크기를 저장할 변수 선언
            int nRead;

            // 버퍼만큼 읽어온다.
            try { nRead = s.Read(temp, 0, BufferSize); }

            // 예외가 발생한 경우, CryptoStream 및 ICryptoTransform 개체를 해제하고 종료한다.
            catch {
                ct.Dispose();
                return TaskResult.StreamReadError;
            }

            // 읽은 데이터의 크기가 1 이상 (데이터가 있음)
            while (nRead > 0) {
                // 상황별 예외처리
                try { cs.Write(temp, 0, nRead); } catch {
                    ct.Dispose();
                    return TaskResult.StreamWriteError;
                }

                try { nRead = s.Read(temp, 0, BufferSize); } catch {
                    ct.Dispose();
                    return TaskResult.StreamReadError;
                }
            }
        }

        result = ms.ToArray();
        return TaskResult.Success;
    }
    /// <summary>
    /// <see cref="Stream"/> 개체의 데이터를 복호화합니다.
    /// </summary>
    /// <param name="s">데이터를 복호화할 <see cref="Stream" /> 개체입니다.</param>
    /// <param name="sa">데이터 복호화에 사용되는 <see cref="SymmetricAlgorithm" /> 개체입니다.</param>
    /// <param name="result">복호화된 데이터가 저장되는 <see cref="byte" /> 배열입니다.</param>
    static TaskResult Decrypt(Stream s, SymmetricAlgorithm sa, out byte[] result) {
        result = null;

        // 복호화된 데이터를 저장할 스트림 생성
        MemoryStream ms = new MemoryStream();

        // 복호화기 생성
        ICryptoTransform ct = sa.CreateDecryptor();

        using (CryptoStream cs = new CryptoStream(s, ct, CryptoStreamMode.Read)) {
            byte[] temp;

            // 배열 선언
            try { temp = new byte[BufferSize]; }

            // 예외가 발생하면 100% 메모리 부족 예외다.
            catch {
                SafeDisposeCryptoStream(cs);
                ct.Dispose();
                return TaskResult.OutOfMemory;
            }

            // 읽은 데이터의 크기를 저장할 변수 선언
            int nRead;

            // 버퍼만큼 읽어온다.
            try { nRead = cs.Read(temp, 0, BufferSize); }

            // 예외가 발생한 경우, CryptoStream 및 ICryptoTransform 개체를 해제하고 종료한다.
            catch {
                SafeDisposeCryptoStream(cs);
                ct.Dispose();
                return TaskResult.StreamReadError;
            }

            // 읽은 데이터의 크기가 1 이상 (데이터가 있음)
            while (nRead > 0) {
                try {
                    ms.Write(temp, 0, nRead);
                    nRead = cs.Read(temp, 0, BufferSize);
                } catch {
                    SafeDisposeCryptoStream(cs);
                    ct.Dispose();
                    return TaskResult.StreamReadError;
                }
            }
        }

        result = ms.ToArray();
        ct.Dispose();
        ms.Dispose();
        return TaskResult.Success;
    }
    /// <summary>
    /// <see cref="CryptographicException" /> 예외가 발생하지 않도록 안전하게 <see cref="CryptoStream"/> 개체에서 사용하는 모든 리소스를 해제합니다.
    /// </summary>
    /// <param name="cs">리소스를 해제할 <see cref="CryptoStream" /> 개체입니다.</param>
    static void SafeDisposeCryptoStream(CryptoStream cs) {
        // 예외가 발생하지 않고 해제되면 가장 좋은 상황
        try { cs.Dispose(); }

        // 예외 발생 - 원인은 두 가지로 볼 수 있다.
        // 1. 키가 손상되어 스트림에 대한 잘못된 패딩이 발생할 수 있는 경우
        // 2. 현재 스트림에 쓸 수 없는 경우- 또는 - 최종 블록이 이미 변환된 경우
        catch (CryptographicException) {

            // CryptoStream 개체에 대한 형식을 가져온다.
            Type tcs = typeof(CryptoStream);

            // 최종 블록 변환 여부를 저장하는 변수 정보를 가져온다.
            FieldInfo _finalBlockTransformed = tcs.GetField("_finalBlockTransformed", BindingFlags.NonPublic | BindingFlags.Instance);

            // 변수 정보를 가져온 경우
            if (_finalBlockTransformed != null) {
                // 값을 설정한다. 여기서 예외가 발생하는 경우 그냥 종료한다.
                try { _finalBlockTransformed.SetValue(cs, true); } catch { }
            }
            // 변수 정보를 가져오지 못한 경우 그냥 종료한다.
            else return;

            // 값을 설정했으니 다시 Dispose를 호출한다.
            cs.Dispose();
        }
    }
    /// <summary>
    /// <see cref="IDisposable" /> 인터페이스를 구현하는 개체를 메모리에서 해제합니다.
    /// </summary>
    /// <param name="obj">해제할 <see cref="IDisposable" /> 인터페이스를 구현하는 개체 배열입니다.</param>
    static void Disposes(params IDisposable[] obj) {
        foreach (IDisposable dis in obj) {
            dis.Dispose();
        }
    }
}