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
using System.Text;

namespace DataChest {
    /// <summary>
    /// DataChest 파일에서 사용하는 헤더 기본 모델입니다.<br />
    /// A base header model used in DCF(DataChest File).
    /// </summary>\
    abstract class HeaderBase : IHeader {
        /// <summary>
        /// 기본 모델 헤더의 바이트 크기입니다.<br />
        /// Size of base header model in bytes.
        /// </summary>
        public const int BaseHeaderModelSize = 38;
        /// <summary>
        /// DCF 헤더를 식별하는 시그니쳐 값입니다.<br />
        /// Signature value to identifying DCF header.
        /// </summary>
        public const ushort HeaderSignature = 22344;

        ushort g_signature;
        ushort g_version;
        uint g_unused;
        uint g_hchksum;
        uint g_echksum;
        uint g_rchksum;
        ushort g_hsize;
        ulong g_esize;
        ulong g_rsize;
        
        /// <summary>
        /// 지정된 헤더 버전을 갖는 <see cref="HeaderBase"/> 클래스의 새 개체를 만듭니다.<br />
        /// Create a new instance of <see cref="HeaderBase"/> class having specified header version.
        /// </summary>
        /// <param name="headerVersion">
        /// 헤더 버전입니다.<br />
        /// Version of the header.
        /// </param>
        protected HeaderBase(ushort headerVersion) {
            g_signature = HeaderSignature;
            g_version = headerVersion;
        }
        
        internal StringBuilder TraverseProperties() {
            Type t = GetType();
            var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (properties.Length == 0) return null;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("+ [{0}]", t.Name));
            foreach (var property in properties) {
                TraverseProperty(2, sb, this, null, property);
            }

            return sb;
        }
        void TraverseProperty(int depth, StringBuilder sb, HeaderBase root, object parent, PropertyInfo property) {
            Type baseType = property.PropertyType;
            object value = parent != null ? property.GetValue(parent, null) : property.GetValue(root, null);
            if (parent != null && (value == root || value == parent)) return;

            if (value != null) {
                Type valueType = value.GetType();
                if (baseType != valueType) baseType = valueType;
            }

            if (!baseType.Module.ScopeName.Equals("CommonLanguageRuntimeLibrary")) {
                if (baseType.IsEnum) {
                    sb.Append(new string(' ', depth * 2 - 2));
                    sb.AppendLine("- " + property.Name + ": " + value + " (" + Enum.GetName(baseType, value) + ")");
                    return;
                }

                var properties = baseType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                if (properties.Length > 0) {
                    sb.Append(new string(' ', depth * 2 - 2));
                    sb.AppendLine("+ " + property.Name + " [" + baseType.Name + "]");
                }

                foreach (var p in properties) {
                    TraverseProperty(depth + 1, sb, root, value, p);
                }
            } else {
                sb.Append(new string(' ', depth * 2 - 2));
                sb.AppendLine("- " + property.Name + ": " + ((value == null) ? "<null>" : value));
            }
        }



        /// <summary>
        /// 기본 필드가 아닌 상위 버전의 헤더에서 추가된 필드를 배열로 변환하는 작업을 처리하는 함수입니다.<br />
        /// Convert fields that added by higher version header to byte array.
        /// </summary>
        /// <param name="bw">
        /// 필드의 내용을 기록할 <see cref="BinaryWriter"/> 개체입니다.<br />
        /// A <see cref="BinaryWriter"/> instance to write fields data.
        /// </param>
        protected virtual void ProcessArray(BinaryWriter bw) { }
        /// <summary>
        /// 기본 필드가 아닌 상위 버전의 헤더에서 추가된 필드를 가진 헤더를 스트림에서 읽어오는 작업을 처리하는 함수입니다.<br />
        /// Read fields that added by higher version header from stream.
        /// </summary>
        /// <param name="br">
        /// 필드의 내용을 읽어올 <see cref="BinaryReader"/> 개체입니다.<br />
        /// A <see cref="BinaryReader"/> instance to read fields data.
        /// </param>
        protected internal virtual void ProcessStream(BinaryReader br) { }
        /// <summary>
        /// 기본 필드의 내용을 읽어오는 작업을 수행하는 함수입니다.<br />
        /// A function that performing read a basic fields information.
        /// </summary>
        /// <param name="br">
        /// 기본 필드의 내용이 저장되어 있는 <see cref="Stream"/> 클래스 개체를 읽기 위한 <see cref="BinaryReader"/> 클래스의 개체입니다.
        /// A instance of <see cref="BinaryReader"/> class to read instance <see cref="Stream"/> class that containing basic fields information.
        /// </param>
        protected internal TaskResult ProcessFields(BinaryReader br) {
            var checkpoint = DataChest.Logger?.OpenCheckpoint(nameof(ProcessFields));
            g_signature = br.ReadUInt16();
            if (g_signature != HeaderSignature) {
                if (DataChest.Logger == null) return TaskResult.InvalidSignature;
                else return (TaskResult)DataChest.Logger?.Abort(TaskResult.InvalidSignature, null);
            }
            
            g_version = br.ReadUInt16();
            if (g_version != HeaderVersion) {
                if (DataChest.Logger == null) return TaskResult.HeaderVersionNotMatch;
                else return (TaskResult)DataChest.Logger?.Abort(TaskResult.HeaderVersionNotMatch, null);
            }

            g_unused = br.ReadUInt32();
            if (Unused != 0) {
                if (DataChest.Logger == null) return TaskResult.InvalidHeaderFieldValue;
                else return (TaskResult)DataChest.Logger?.Abort(TaskResult.InvalidHeaderFieldValue, null);
            }

            uint chkHeader = br.ReadUInt32();
            g_echksum = br.ReadUInt32();
            g_rchksum = br.ReadUInt32();
            g_hsize = br.ReadUInt16();
            g_esize = br.ReadUInt64();
            g_rsize = br.ReadUInt64();
            
            try { ProcessStream(br); }
            catch (Exception e) {
                if (DataChest.Logger == null) return TaskResult.ErrorCausedUDPR;
                else return (TaskResult)DataChest.Logger?.Abort(TaskResult.ErrorCausedUDPR, e);
            }
            
            byte[] bHdr;
            TaskResult r = ToArray(out bHdr);
            if (r != TaskResult.Success) return r;
            
            if (HashHelper.ComputeUInt32(bHdr) != chkHeader) {
                if (DataChest.Logger == null) return TaskResult.IncorrectHeaderChecksum;
                else return (TaskResult)DataChest.Logger?.Abort(TaskResult.IncorrectHeaderChecksum, null);
            }

            g_hchksum = chkHeader;
            DataChest.Logger?.CloseCheckpoint(checkpoint, 0);
            return TaskResult.Success;
        }




        /// <summary>
        /// 헤더의 내용을 바이트 배열로 변환합니다.<br />
        /// Convert header to byte array.
        /// </summary>
        public TaskResult ToArray(out byte[] arr) {
            var checkpoint = DataChest.Logger?.OpenCheckpoint(nameof(ToArray));
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
                catch (Exception e) {
                    if (DataChest.Logger == null) return TaskResult.ErrorCausedUDPR;
                    else return (TaskResult)DataChest.Logger?.Abort(TaskResult.ErrorCausedUDPR, e);
                }
            }

            arr = ms.ToArray();
            DataChest.Logger?.CloseCheckpoint(checkpoint, arr.Length);
            return TaskResult.Success;
        }
        /// <summary>
        /// 암호화 데이터를 채웁니다.<br />
        /// Fills an encryption information.
        /// </summary>
        /// <param name="s">
        /// 암호화할 데이터가 저장되어 있는 <see cref="Stream"/> 개체입니다.<br />
        /// A <see cref="Stream"/> instance that containing data to be encrypted.
        /// </param>
        /// <param name="b">
        /// 암호화된 데이터가 저장된 바이트 배열입니다.<br />
        /// A <see cref="byte"/> array which encrypted data stored.
        /// </param>
        public TaskResult FillEncryptionInfo(Stream s, byte[] b) {
            var checkpoint = DataChest.Logger?.OpenCheckpoint(nameof(FillEncryptionInfo));
            g_rsize = (ulong)s.Length;
            g_esize = (ulong)b.LongLength;

            s.Seek(0, SeekOrigin.Begin);
            g_rchksum = HashHelper.ComputeUInt32(s);
            g_echksum = HashHelper.ComputeUInt32(b);
            byte[] bTemp;
            TaskResult r = ToArray(out bTemp);
            if (r != TaskResult.Success) return r;

            g_hchksum = HashHelper.ComputeUInt32(bTemp);

            DataChest.Logger?.CloseCheckpoint(checkpoint, 0);
            return TaskResult.Success;
        }

        /// <summary>
        /// 필드 버전이 아닌 헤더의 버전을 가져옵니다. (이 속성은 반드시 맞는 헤더 버전을 반환하도록 재정의되어야 합니다)<br />
        /// Get a header version. (This property must be overrided to return correct header version)
        /// </summary>
        public virtual ushort HeaderVersion {
            get { return 0; }
        }
        /// <summary>
        /// 시그니쳐를 가져옵니다.<br />
        /// Get a signature.
        /// </summary>
        public ushort Signature {
            get { return g_signature; }
        }
        /// <summary>
        /// 버전을 가져옵니다.<br />
        /// Get a version.
        /// </summary>
        public ushort Version {
            get { return g_version; }
        }
        /// <summary>
        /// 사용되지 않습니다.<br />
        /// Unused field.
        /// </summary>
        public uint Unused {
            get { return g_unused; }
        }
        /// <summary>
        /// 헤더의 체크섬을 가져옵니다.<br />
        /// Get a checksum of header.
        /// </summary>
        public uint HChecksum {
            get { return g_hchksum; }
        }
        /// <summary>
        /// 암호화된 데이터의 체크섬을 가져옵니다.<br />
        /// Get a checksum of encrypted data.
        /// </summary>
        public uint EChecksum {
            get { return g_echksum; }
        }
        /// <summary>
        /// 원본 데이터의 체크섬을 가져옵니다.<br />
        /// Get a checksum of raw data.
        /// </summary>
        public uint RChecksum {
            get { return g_rchksum; }
        }
        /// <summary>
        /// 헤더의 크기를 가져옵니다.<br />
        /// Get a size of header.
        /// </summary>
        public ushort HSize {
            get { return g_hsize; }
            protected set { g_hsize = value; }
        }
        /// <summary>
        /// 암호화된 데이터의 크기를 가져옵니다.<br />
        /// Get a size of encrypted data.
        /// </summary>
        public ulong ESize {
            get { return g_esize; }
        }
        /// <summary>
        /// 원본 데이터의 크기를 가져옵니다.<br />
        /// Get a size of raw data.
        /// </summary>
        public ulong RSize {
            get { return g_rsize; }
        }
    }
}