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

/// <summary>
/// ChestAPI 파일 형식에서 사용하는 헤더의 기본 모델입니다.
/// </summary>
class HeaderBase : IHeader {
    /// <summary>
    /// 초기 버전(1) 헤더의 크기입니다.
    /// </summary>
    public const int HeaderBaseSize = 38;
    
    ushort g_signature;
    ushort g_version;
    uint g_unused;
    uint g_hchksum;
    uint g_echksum;
    uint g_rchksum;
    ushort g_hsize;
    ulong g_esize;
    ulong g_rsize;
    
    HeaderBase() { }
    /// <summary>
    /// 지정된 헤더 버전을 사용하여 <see cref="HeaderBase" /> 클래스의 새 개체를 만듭니다.
    /// </summary>
    /// <param name="headerVersion">헤더 버전입니다.</param>
    protected HeaderBase(ushort headerVersion) {
        g_signature = 22344;
        g_version = headerVersion;
    }
    
    /// <summary>
    /// 기본 필드가 아닌 상위 버전의 헤더에서 추가된 필드를 배열로 변환하는 작업을 처리하는 함수입니다.
    /// 추가된 필드는 <see cref="ProcessArray(BinaryWriter)" /> 메서드의 매개변수를 이용하여 <see cref="BinaryWriter" />.Write() 메서드를 호출하여야 합니다.
    /// </summary>
    /// <param name="bw">필드의 내용을 기록할 <see cref="BinaryWriter" /> 개체입니다.</param>
    protected virtual void ProcessArray(BinaryWriter bw) { }
    /// <summary>
    /// 기본 필드가 아닌 상위 버전의 헤더에서 추가된 필드를 가진 헤더를 스트림에서 읽어오는 작업을 처리하는 함수입니다.
    /// 추가된 필드는 <see cref="ProcessStream(BinaryReader)" /> 메서드의 매개변수를 이용하여 <see cref="BinaryReader" />.Read...() 메서드를 호출하여야 합니다.
    /// </summary>
    /// <param name="br">필드의 내용을 읽어올 <see cref="BinaryReader" /> 개체입니다.</param>
    protected virtual void ProcessStream(BinaryReader br) { }
    
    /// <summary>
    /// 버전에 맞는 빈 헤더를 만듭니다.
    /// </summary>
    /// <param name="version">만들 헤더의 버전입니다.</param>
    public static HeaderBase CreateEmptyHeader(ushort version) {
        switch (version) {
            case 1:
                return new ChestHeader1();

            case 2:
                return new ChestHeader2();

            default:
                return null;
        }
    }
    /// <summary>
    /// 지정된 파일로부터 헤더를 불러옵니다.
    /// </summary>
    /// <typeparam name="T"><see cref="HeaderBase" /> 클래스를 상속하는 개체입니다.</typeparam>
    /// <param name="s">헤더 데이터가 저장된 파일의 경로입니다.</param>
    /// <param name="hdr">만들어진 헤더가 저장될 변수입니다.</param>
    public static TaskResult FromFile<T>(string fileName, out T hdr) where T : HeaderBase {
        hdr = default(T);
        FileStream fs;
        TaskResult r = FileHelper.OpenFileStream(fileName, out fs);
        if (r != TaskResult.Success) return r;
        return FromStream(fs, out hdr);
    }
    /// <summary>
    /// 지정된 파일과 버전을 이용하여 헤더를 불러옵니다.
    /// </summary>
    /// <param name="s">헤더 데이터가 저장된 파일의 경로입니다.</param>
    /// <param name="version">헤더의 버전입니다.</param>
    /// <param name="hdr">만들어진 헤더가 저장될 변수입니다.</param>
    public static TaskResult FromFileWithVersion(string fileName, ushort version, out HeaderBase hdr) {
        hdr = null;
        FileStream fs;
        TaskResult r = FileHelper.OpenFileStream(fileName, out fs);
        if (r != TaskResult.Success) return r;
        return FromStreamWithVersion(fs, version, out hdr);
    }
    /// <summary>
    /// 지정된 <see cref="Stream" /> 개체로부터 헤더를 불러옵니다.
    /// </summary>
    /// <typeparam name="T"><see cref="HeaderBase" /> 클래스를 상속하는 개체입니다.</typeparam>
    /// <param name="s">헤더 데이터가 저장된 <see cref="Stream" /> 개체입니다.</param>
    /// <param name="hdr">만들어진 헤더가 저장될 변수입니다.</param>
    public static TaskResult FromStream<T>(Stream s, out T hdr) where T : HeaderBase {
        // out 변수를 초기화한다.
        hdr = null;

        // 매개변수가 없는 생성자를 찾는다.
        ConstructorInfo ctor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);

        // 생성자를 찾지 못한 경우 
        if (ctor == null) return TaskResult.InvalidHeaderClass;

        // 이제, 입력된 형식 매개변수에 맞는 개체를 만든다.
        T temp = (T) ctor.Invoke(null);

        // 개체를 만들지 못한 경우
        if ( temp == null ) return TaskResult.CannotCreateHeaderInstance;

        // 스트림의 데이터를 읽어올 개체를 만든다.
        BinaryReader br;
        try {
            br = new BinaryReader(s);
            s.Seek(0, SeekOrigin.Begin);
        }
        catch { return TaskResult.StreamError; }

        // 시그니쳐를 읽어오고 값을 검사한다.
        temp.g_signature = br.ReadUInt16();
        if (temp.Signature != 22344) return TaskResult.InvalidSignature;
        
        // 버전을 읽어오고 값을 검사한다.
        // 버전은 현재 사용중인 어셈블리의 ChestAPI 버전보다 크거나, 헤더의 버전과 다를 경우 오류가 발생한다.
        temp.g_version = br.ReadUInt16();
        if (temp.Version > ChestAPI.Version) return TaskResult.NotSupportedVersion;
        if (temp.Version != temp.HeaderVersion) return TaskResult.HeaderVersionNotMatch;
        
        // 이 값은 반드시 0이여야 한다.
        temp.g_unused = br.ReadUInt32();
        if (temp.Unused != 0) return TaskResult.InvalidHeaderFieldValue;

        // 기본 헤더의 구조에 맞게 순서대로 값을 읽어온다.
        uint chkHeader = br.ReadUInt32();
        temp.g_echksum = br.ReadUInt32();
        temp.g_rchksum = br.ReadUInt32();
        temp.g_hsize = br.ReadUInt16();
        temp.g_esize = br.ReadUInt64();
        temp.g_rsize = br.ReadUInt64();

        // 기본 헤더의 값 읽기가 끝났다.
        // 여기서부터는 추가된 필드에 대한 값 읽기가 진행되는 부분이다.
        try { temp.ProcessStream(br); }
        catch { return TaskResult.ErrorCausedUDPR; }
        
        // 헤더를 배열로 변환한다.
        byte[] bHdr;
        TaskResult r = temp.ToArray(out bHdr);
        if (r != TaskResult.Success) return r;

        // 체크섬 검증
        if (HashAPI.ComputeHashUInt32(bHdr) != chkHeader)
            return TaskResult.IncorrectHeaderChecksum;

        // 끝!
        temp.g_hchksum = chkHeader;
        hdr = temp;
        return TaskResult.Success;
    }
    /// <summary>
    /// 지정된 <see cref="Stream" /> 개체와 버전을 이용하여 헤더를 불러옵니다.
    /// </summary>
    /// <param name="s">헤더 데이터가 저장된 <see cref="Stream" /> 개체입니다.</param>
    /// <param name="version">헤더의 버전입니다.</param>
    /// <param name="hdr">만들어진 헤더가 저장될 변수입니다.</param>
    public static TaskResult FromStreamWithVersion(Stream s, ushort version, out HeaderBase hdr) {
        hdr = null;
        TaskResult r;
        switch (version) {
            case 1:
                ChestHeader1 hdr1;
                r = FromStream(s, out hdr1);
                if (r != TaskResult.Success) return r;
                hdr = hdr1;
                return TaskResult.Success;

            case 2:
                ChestHeader2 hdr2;
                r = FromStream(s, out hdr2);
                if (r != TaskResult.Success) return r;
                hdr = hdr2;
                return TaskResult.Success;

            default:
                return TaskResult.NotSupportedVersion;
        }
    }
    /// <summary>
    /// 헤더의 내용을 바이트 배열로 변환합니다.
    /// </summary>
    public TaskResult ToArray(out byte[] arr) {
        arr = null;
        MemoryStream ms = new MemoryStream(HSize);
        using (BinaryWriter bw = new BinaryWriter(ms)) {
            bw.Write(g_signature);
            bw.Write(g_version);
            bw.Write(g_unused);
            bw.Write(g_hchksum);
            bw.Write(g_echksum);
            bw.Write(g_rchksum);
            bw.Write(g_hsize);
            bw.Write(g_esize);
            bw.Write(g_rsize);

            // Higher version support
            try { ProcessArray(bw); }
            catch { return TaskResult.ErrorCausedUDPR; }
        }

        arr = ms.ToArray();
        return TaskResult.Success;
    }
    /// <summary>
    /// 암호화 작업에 대한 필드의 값을 설정합니다.
    /// </summary>
    /// <param name="s">암호화할 데이터가 저장되어 있는 <see cref="Stream" /> 개체입니다.</param>
    /// <param name="b">암호화된 데이터가 저장된 바이트 배열입니다.</param>
    public TaskResult AssignBasicInformationEncrypt(Stream s, byte[] b) {
        g_rsize = (ulong) s.Length;
        g_esize = (ulong)b.LongLength;

        s.Seek(0, SeekOrigin.Begin);
        g_rchksum = HashAPI.ComputeHashUInt32(s);
        g_echksum = HashAPI.ComputeHashUInt32(b);
        byte[] bTemp;
        TaskResult r = ToArray(out bTemp);
        if (r != TaskResult.Success) return r;

        g_hchksum = HashAPI.ComputeHashUInt32(bTemp);

        return TaskResult.Success;
    }

    /// <summary>
    /// 필드 버전이 아닌 헤더의 버전을 가져옵니다.
    /// 상위 버전의 헤더를 만든 경우 이 속성이 정확한 헤더 버전을 반환하도록 재정의해야 합니다.
    /// </summary>
    public virtual ushort HeaderVersion {
        get { return 0; }
    }
    /// <summary>
    /// 시그니쳐를 가져옵니다.
    /// </summary>
    public ushort Signature {
        get { return g_signature; }
    }
    /// <summary>
    /// 버전을 가져옵니다.
    /// </summary>
    public ushort Version {
        get { return g_version; }
    }
    /// <summary>
    /// 사용되지 않습니다.
    /// </summary>
    public uint Unused {
        get { return g_unused; }
    }
    /// <summary>
    /// 헤더의 체크섬을 가져옵니다.
    /// </summary>
    public uint HChecksum {
        get { return g_hchksum; }
    }
    /// <summary>
    /// 암호화된 데이터의 체크섬을 가져옵니다.
    /// </summary>
    public uint EChecksum {
        get { return g_echksum; }
    }
    /// <summary>
    /// 원본 데이터의 체크섬을 가져옵니다.
    /// </summary>
    public uint RChecksum {
        get { return g_rchksum; }
    }
    /// <summary>
    /// 헤더의 크기를 가져옵니다.
    /// </summary>
    public ushort HSize {
        get { return g_hsize; }
        protected set { g_hsize = value; }
    }
    /// <summary>
    /// 암호화된 데이터의 크기를 가져옵니다.
    /// </summary>
    public ulong ESize {
        get { return g_esize; }
    }
    /// <summary>
    /// 원본 데이터의 크기를 가져옵니다.
    /// </summary>
    public ulong RSize {
        get { return g_rsize; }
    }
}