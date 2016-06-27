using System;
using System.Collections.Generic;
using System.IO;

namespace DataChest {
    /// <summary>
    /// 헤더를 생성하고 스트림에서 헤더를 읽어오는 작업을 정의하는 클래스입니다.<br />
    /// Defining functions that performing create header and read header from stream.
    /// </summary>
    static class HeaderManager {
        public static ushort NewerVersion = 0;
        static readonly Dictionary<ushort, Type> m_headers = new Dictionary<ushort, Type>();

        /// <summary>
        /// 헤더를 등록합니다.<br />
        /// Register a header.
        /// </summary>
        /// <typeparam name="T">
        /// <see cref="HeaderBase"/> 클래스를 상속하는 헤더의 형식입니다.
        /// A type of that inherits a <see cref="HeaderBase"/> class.
        /// </typeparam>
        /// <param name="version">
        /// 헤더의 버전입니다.<br />
        /// A version of header.
        /// </param>
        public static void RegisterHeader<T>(ushort version) where T : HeaderBase {
            Type t = typeof(T);
            if (m_headers.ContainsKey(version)) return;
            if (m_headers.ContainsValue(t)) return;
            m_headers.Add(version, t);
            if (version > NewerVersion) NewerVersion = version;
        }

        /// <summary>
        /// 초기 버전인 <see cref="DC_HEADER_1"/> 클래스의 개체를 만듭니다.<br />
        /// Create a instance of <see cref="DC_HEADER_1"/> which is original version.
        /// </summary>
        public static HeaderBase CreateDefault() {
            return new DC_HEADER_1();
        }
        /// <summary>
        /// 지정한 버전을 갖는 헤더 클래스의 개체를 만듭니다.<br />
        /// Create a header class instance that having specified version.
        /// </summary>
        /// <param name="version">
        /// 헤더의 버전입니다.
        /// Version of header.
        /// </param>
        /// <param name="result">
        /// 작업 결과가 저장될 변수입니다.<br />
        /// A variable to store task result.
        /// </param>
        public static HeaderBase Create(ushort version, out TaskResult result) {
            var checkpoint = DataChest.Logger?.OpenCheckpoint(nameof(Create));
            result = TaskResult.Success;
            if (!m_headers.ContainsKey(version)) {
                result = TaskResult.NotSupportedVersion;
                return null;
            }
            HeaderBase hdr;
            try { hdr = (HeaderBase)Activator.CreateInstance(m_headers[version]); }
            catch (Exception e) {
                if (DataChest.Logger == null) result = TaskResult.InvalidHeaderClass;
                result = (TaskResult) DataChest.Logger?.Abort(TaskResult.InvalidHeaderClass, e);
                return null;
            }
            DataChest.Logger?.CloseCheckpoint(checkpoint, 0);
            return hdr;
        }

        /// <summary>
        /// 지정한 버전을 갖는 헤더를 <see cref="Stream"/> 클래스의 개체로부터 만듭니다.<br />
        /// Create header that having specified version from <see cref="Stream"/> instance.
        /// </summary>
        /// <param name="s">
        /// 헤더를 만드는데 사용할 <see cref="Stream"/> 클래스의 개체입니다.<br />
        /// A instance of <see cref="Stream"/> class that used to create a header.
        /// </param>
        /// <param name="result">
        /// 작업 결과가 저장될 변수입니다.<br />
        /// A variable to store task result.
        /// </param>
        public static HeaderBase FromStream(Stream s, out TaskResult result) {
            return FromStream(s, ushort.MaxValue, out result);
        }
        /// <summary>
        /// 지정한 버전을 갖는 헤더를 <see cref="Stream"/> 클래스의 개체로부터 만듭니다.<br />
        /// Create header that having specified version from <see cref="Stream"/> instance.
        /// </summary>
        /// <param name="s">
        /// 헤더를 만드는데 사용할 <see cref="Stream"/> 클래스의 개체입니다.<br />
        /// A instance of <see cref="Stream"/> class that used to create a header.
        /// </param>
        /// <param name="version">
        /// 헤더의 버전입니다.<br />
        /// A version of header.
        /// </param>
        /// <param name="result">
        /// 작업 결과가 저장될 변수입니다.<br />
        /// A variable to store task result.
        /// </param>
        public static HeaderBase FromStream(Stream s, ushort version, out TaskResult result) {
            var checkpoint = DataChest.Logger?.OpenCheckpoint(nameof(FromStream));
            result = TaskResult.Success;
            HeaderBase hdr;
            if (version == ushort.MaxValue) {
                result = GetVersionFromStream(s, out version);
                if (result != TaskResult.Success) return null;
            }

            hdr = Create(version, out result);
            if (result != TaskResult.Success) return null;

            BinaryReader br = new BinaryReader(s);
            hdr.ProcessFields(br);

            DataChest.Logger?.CloseCheckpoint(checkpoint, 0);
            return hdr;
        }

        /// <summary>
        /// 파일로부터 <see cref="HeaderBase"/> 클래스의 개체를 만듭니다.<br />
        /// Create instance of <see cref="HeaderBase"/> class from file.
        /// </summary>
        /// <param name="fileName">
        /// 파일 이름입니다.<br />
        /// A filename.
        /// </param>
        /// <param name="result">
        /// 작업 결과가 저장될 변수입니다.<br />
        /// A variable to store task result.
        /// </param>
        public static HeaderBase FromFile(string fileName, out TaskResult result) {
            return FromFile(fileName, ushort.MaxValue, out result);
        }
        /// <summary>
        /// 파일로부터 <see cref="HeaderBase"/> 클래스의 개체를 만듭니다.<br />
        /// Create instance of <see cref="HeaderBase"/> class from file.
        /// </summary>
        /// <param name="fileName">
        /// 파일 이름입니다.<br />
        /// A filename.
        /// </param>
        /// <param name="version">
        /// 헤더의 버전입니다.
        /// Version of header.
        /// </param>
        /// <param name="result">
        /// 작업 결과가 저장될 변수입니다.<br />
        /// A variable to store task result.
        /// </param>
        public static HeaderBase FromFile(string fileName, ushort version, out TaskResult result) {
            HeaderBase hdr;
            FileStream fs;
            result = FileHelper.OpenFileStream(fileName, out fs);
            if (result != TaskResult.Success) return null;
            hdr = FromStream(fs, version, out result);
            fs.Close();
            return hdr;
        }
        
        /// <summary>
        /// 파일로부터 헤더의 버전을 가져옵니다.<br />
        /// Get a version of header from file.
        /// </summary>
        /// <param name="fileName">
        /// 파일 이름입니다.<br />
        /// A filename.
        /// </param>
        /// <param name="version">
        /// 버전 정보가 저장될 변수입니다.
        /// A variable to store version information.
        /// </param>
        public static TaskResult GetVersionFromFile(string fileName, out ushort version) {
            version = ushort.MaxValue;
            FileStream fs;
            TaskResult r = FileHelper.OpenFileStream(fileName, out fs);
            if (r != TaskResult.Success) return r;
            return GetVersionFromStream(fs, out version);
        }

        /// <summary>
        /// <see cref="Stream"/> 클래스의 개체로부터 헤더의 버전을 가져옵니다.<br />
        /// Get a version of header from file.
        /// </summary>
        /// <param name="s">
        /// 헤더를 만드는데 사용할 <see cref="Stream"/> 클래스의 개체입니다.<br />
        /// A instance of <see cref="Stream"/> class that used to create a header.
        /// </param>
        /// <param name="version">
        /// 버전 정보가 저장될 변수입니다.
        /// A variable to store version information.
        /// </param>
        public static TaskResult GetVersionFromStream(Stream s, out ushort version) {
            var checkpoint = DataChest.Logger?.OpenCheckpoint(nameof(GetVersionFromStream));
            version = ushort.MaxValue;

            // Signature & Version check
            BinaryReader br = new BinaryReader(s);
            ushort sig = br.ReadUInt16();
            if (sig != HeaderBase.HeaderSignature) {
                if (DataChest.Logger == null) return TaskResult.InvalidSignature;
                else return (TaskResult)DataChest.Logger?.Abort(TaskResult.InvalidSignature, null);
            }

            version = br.ReadUInt16();
            if (s.CanSeek) s.Seek(0, SeekOrigin.Begin);
            if (m_headers.ContainsKey(version)) {
                DataChest.Logger?.CloseCheckpoint(checkpoint, 0);
                return TaskResult.Success;
            }
            if (DataChest.Logger == null) return TaskResult.NotSupportedVersion;
            else return (TaskResult)DataChest.Logger?.Abort(TaskResult.NotSupportedVersion, null);
        }
    }
}