using System;
using System.IO;
using System.Reflection;

class ChestHeader2 : HeaderBase {
    string g_comment;
    public ChestHeader2() : base(2) { }

    protected override void ProcessArray(BinaryWriter bw) {
        if ( string.IsNullOrEmpty(g_comment) )
            bw.Write((ushort) 0);
        else {
            byte[] bComment = ChestAPI.SystemUnicodeEncoding.GetBytes(g_comment);
            bw.Write(CommentLength);
            bw.Write(bComment);
        }
    }
    protected override TaskResult ProcessStream(BinaryReader br) {
        return TaskResult.InvalidParameter;
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
            return (ushort) (g_comment.Length * 2);
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
            HSize = (ushort) (HeaderBaseSize + 2 + (g_comment.Length * 2));
        }
    }
}

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




    public static CHEST_HEADER CreateHeader(ushort version, FileStream fs, byte[] b) {
        CHEST_HEADER hdr = new CHEST_HEADER(version);
        hdr.r_size = (ulong)fs.Length;
        hdr.e_size = (ulong)b.LongLength;

        fs.Seek(0, SeekOrigin.Begin);
        hdr.r_checksum = HashAPI.ComputeHashUInt32(fs);
        hdr.e_checksum = HashAPI.ComputeHashUInt32(b);
        byte[] temp = hdr.ToArray();
        hdr.h_checksum = HashAPI.ComputeHashUInt32(temp);

        return hdr;
    }
    /*
    public static TaskResult FromFile(string file, out CHEST_HEADER hdr) {
        hdr = default(CHEST_HEADER);
        FileStream fs;
        TaskResult r = FileHelper.OpenFileStream(file, out fs);
        if (r != TaskResult.Success) return r;
        return FromStream(fs, out hdr);
    }
    */
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
    protected virtual TaskResult ProcessStream(BinaryReader br) {
        return TaskResult.Success;
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
        try { br = new BinaryReader(s); }
        catch { return TaskResult.InvalidParameter; }

        // 시그니쳐를 읽어오고 값을 검사한다.
        temp.g_signature = br.ReadUInt16();
        if (temp.Signature != 22344) return TaskResult.InvalidSignature;
        
        // 버전을 읽어오고 값을 검사한다.
        // 버전은 현재 사용중인 어셈블리의 ChestAPI 버전보다 크거나, 헤더의 버전과 다를 경우 오류가 발생한다.
        temp.g_version = br.ReadUInt16();
        if (temp.Version > ChestAPI.Version) return TaskResult.NotSupportedVersion;
        if (temp.Version != temp.HeaderVersion) return TaskResult.HeaderVersionDamagedOrCorrupted;
        
        temp.g_unused = br.ReadUInt32();
        // Obsolete 특성때문에 발생하는 경고는 무시한다.
#pragma warning disable CS0612
        if (temp.Unused != 0) return TaskResult.InvalidHeaderFieldValue;
#pragma warning restore CS0612

        // 기본 헤더의 구조에 맞게 순서대로 값을 읽어온다.
        uint chkHeader = br.ReadUInt32();
        temp.g_echksum = br.ReadUInt32();
        temp.g_rchksum = br.ReadUInt32();
        temp.g_hsize = br.ReadUInt16();
        temp.g_esize = br.ReadUInt64();
        temp.g_rsize = br.ReadUInt64();

        // 기본 헤더의 값 읽기가 끝났다.
        // 여기서부터는 추가된 필드에 대한 값 읽기가 진행되는 부분이다.
        TaskResult r = temp.ProcessStream(br);

        // 실패한 경우 
        if (r != TaskResult.Success) return r;

        // 헤더를 배열로 변환하고 체크섬을 검증한다.
        byte[] bHdr = temp.ToArray();
        if (HashAPI.ComputeHashUInt32(bHdr) != chkHeader)
            return TaskResult.IncorrectHeaderChecksum;

        // 끝!
        temp.g_hchksum = chkHeader;
        hdr = temp;
        return TaskResult.Success;
    }
    /// <summary>
    /// 헤더의 내용을 바이트 배열로 변환합니다.
    /// </summary>
    public byte[] ToArray() {
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
            ProcessArray(bw);
        }
        return ms.ToArray();
    }
    

    /// <summary>
    /// 필드 버전이 아닌 헤더의 버전을 가져옵니다.
    /// 상위 버전의 헤더를 만든 경우 이 속성이 정확한 헤더 버전을 반환하도록 재정의해야 합니다.
    /// </summary>
    public virtual ushort HeaderVersion {
        get { return 1; }
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
    [Obsolete()]
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