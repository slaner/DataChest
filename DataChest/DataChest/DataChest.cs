using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace DataChest {
    public sealed class DataChest : IDisposable {
        /// <summary>
        /// 암호 또는 IV 데이터를 파일로부터 읽어옵니다.<br />
        /// Read password or IV data from file.
        /// </summary>
        public const string CipherFromFile = "FF:";
        /// <summary>
        /// 암호 또는 IV 데이터를 평문 그대로 사용합니다<br />
        /// Use plain text as password or IV data.
        /// </summary>
        public const string CipherPlainText = "FT:";
        /// <summary>
        /// 기본 버퍼 크기입니다.<br />
        /// Default buffer size.
        /// </summary>
        public const int DefaultBufferSize = 4096;
        /// <summary>
        /// IV가 지정되지 않은 경우에 기본 값으로 사용되는 값입니다.<br />
        /// Default used value when IV is not specified.
        /// </summary>
        /// <remarks>
        /// 이 바이트 배열은 2부터 시작하여 총 32개의 소수로 만들어졌습니다.<br />
        /// This byte array begins 2 and created with 32 prime numbers.
        /// </remarks>
        public static readonly byte[] DefaultIV = {
            0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11, 0x13,
            0x17, 0x1D, 0x1F, 0x25, 0x29, 0x2B, 0x2F, 0x35,
            0x3B, 0x3D, 0x43, 0x47, 0x49, 0x4F, 0x53, 0x59,
            0x61, 0x65, 0x67, 0x6B, 0x6D, 0x71, 0x7F, 0x83,
        };

        Option                  m_option    = null;
        SymmetricAlgorithm      m_alg       = null;

        static DataChest() {
            // TODO: 이 곳에서 상위 버전 헤더에 대한 등록을 수행합니다.
            // TODO: Perform registration of higher version header in here.
            // 등록 수행 방법(Performing registration): 
            //   HeaderManager.RegisterHeader<DC_HEADER_1>(1);
            //   HeaderManager.RegisterHeader<YOUR_HEADER_TYPE_NAME>(VERSION_OF_HEADER_TYPE);
            HeaderManager.RegisterHeader<DC_HEADER_1>(1);
            HeaderManager.RegisterHeader<DC_HEADER_2>(2);
        }

        public DataChest(Option option) {
            m_option = option;
        }


        /// <summary>
        /// 주어진 작업에 대한 테스트를 수행합니다.<br />
        /// Perform a test for given task.
        /// </summary>
        /// <param name="s">
        /// 데이터를 저장하고 있는 <see cref="Stream"/> 개체입니다.<br />
        /// A instance of <see cref="Stream"/> that contains data.
        /// </param>
        TaskResult TestProcess(Stream s) {
            TaskResult r = 0;
            byte[] result = null;
            HeaderBase hdr = null;
            uint hash = 0;
            
            if (m_option.Decrypt) {
                hdr = HeaderManager.FromStream(s, out r);
                if (r != TaskResult.Success) {
                    s.Dispose();
                    return r;
                }
                
                if (!m_option.DisableVerification) {
                    long lnPrevPos = s.Position;
                    hash = HashAPI.ComputeHashUInt32(s);
                    
                    if (hdr.EChecksum != hash) {
                        s.Dispose();
                        return TaskResult.IncorrectEncryptedDataChecksum;
                    }
                    
                    s.Seek(lnPrevPos, SeekOrigin.Begin);
                }
            }

            r = m_option.Decrypt ? Decrypt(s, m_alg, out result) : Encrypt(s, m_alg, out result);
            if (r != TaskResult.Success) {
                s.Dispose();
                return r;
            }

            if (m_option.DisableVerification || !m_option.Decrypt) {
                s.Dispose();
                return r;
            }
            
            hash = HashAPI.ComputeHashUInt32(result);
            if (hdr.RChecksum != hash) {
                s.Dispose();
                return TaskResult.IncorrectRawDataChecksum;
            }

            s.Dispose();
            return TaskResult.Success;
        }
        /// <summary>
        /// 암호화 작업을 수행합니다.<br />
        /// Perform encryption process.
        /// </summary>
        /// <param name="sin">
        /// 암호화될 데이터가 저장된 <see cref="Stream"/> 개체입니다.<br />
        /// A instance of <see cref="Stream"/> class that contains data to be encrypted.
        /// </param>
        /// <param name="sout">
        /// 암호화된 데이터가 저장될 <see cref="Stream"/> 개체입니다.<br />
        /// A instance of <see cref="Stream"/> class to encrypted data be stored.
        /// </param>
        /// <param name="output">
        /// 출력 파일의 이름입니다.<br />
        /// A name of output file.
        /// </param>
        TaskResult EncryptProcess(Stream sin, Stream sout, string output) {
            byte[] result;
            TaskResult r = Encrypt(sin, m_alg, out result);
            if (r != TaskResult.Success) {
                sin.Dispose();
                sout.Dispose();
                return r;
            }

            ushort v = m_option.HeaderVersion;
            if (v == 0) v = HeaderManager.NewerVersion;

            HeaderBase hdr = HeaderManager.Create(v, out r);
            if (r != TaskResult.Success) {
                sin.Dispose();
                sout.Dispose();
                return r;
            }
            r = hdr.AssignBasicInformationEncrypt(sin, result);

            byte[] header;
            r = hdr.ToArray(out header);
            if (r != TaskResult.Success) {
                sin.Dispose();
                sout.Dispose();
                return r;
            }
            
            try {
                using (BinaryWriter bw = new BinaryWriter(sout)) {
                    bw.Write(header);
                    
                    int loop_count = result.Length / m_option.BufferSize.Value;
                    int remain = result.Length % m_option.BufferSize.Value;
                    for (int i = 0; i < loop_count; i++)
                        bw.Write(result, i * m_option.BufferSize.Value, m_option.BufferSize.Value);

                    if (remain > 0)
                        bw.Write(result, loop_count * m_option.BufferSize.Value, remain);
                }
            }
            catch {
                sin.Dispose();
                FileHelper.DeleteFileIgnoreErrors(output);
                return TaskResult.StreamWriteError;
            }

            if (m_option.Cleanup) {
                // TODO: Multiple file support?
                try { File.Delete(m_option.In[0]); } catch {
                    sin.Dispose();
                    return TaskResult.CleanupFailedSucceed;
                }
            }
            
            return TaskResult.Success;
        }
        /// <summary>
        /// 복호화 작업을 수행합니다.<br />
        /// Perform encryption process.
        /// </summary>
        /// <param name="sin">
        /// 복호화될 데이터가 저장된 <see cref="Stream"/> 개체입니다.<br />
        /// A instance of <see cref="Stream"/> class that contains data to be decrypted.
        /// </param>
        /// <param name="sout">
        /// 복호화된 데이터가 저장될 <see cref="Stream"/> 개체입니다.<br />
        /// A instance of <see cref="Stream"/> class to decrypted data be stored.
        /// </param>
        /// <param name="output">
        /// 출력 파일의 이름입니다.<br />
        /// A name of output file.
        /// </param>
        TaskResult DecryptProcess(Stream sin, Stream sout, string output) {
            byte[] result;
            TaskResult r;

            HeaderBase hdr = HeaderManager.FromStream(sin, out r);
            if (r != TaskResult.Success) {
                sin.Dispose();
                sout.Dispose();
                FileHelper.DeleteFileIgnoreErrors(output);
                return r;
            }

            if (!m_option.DisableVerification) {
                long lnPrevPos = sin.Position;
                uint prevHash = HashAPI.ComputeHashUInt32(sin);

                if (hdr.EChecksum != prevHash) {
                    sin.Dispose();
                    sout.Dispose();
                    FileHelper.DeleteFileIgnoreErrors(output);
                    return TaskResult.IncorrectEncryptedDataChecksum;
                }

                sin.Seek(lnPrevPos, SeekOrigin.Begin);
            }

            r = Decrypt(sin, m_alg, out result);
            if (r != TaskResult.Success) {
                sin.Dispose();
                sout.Dispose();
                return r;
            }

            if (!m_option.DisableVerification) {
                if (HashAPI.ComputeHashUInt32(result) != hdr.RChecksum) {
                    sin.Dispose();
                    sout.Dispose();
                    FileHelper.DeleteFileIgnoreErrors(output);
                    return TaskResult.IncorrectEncryptedDataChecksum;
                }
            }

            try {
                using (BinaryWriter bw = new BinaryWriter(sout)) {
                    int loop_count = result.Length / m_option.BufferSize.Value;
                    int remain = result.Length % m_option.BufferSize.Value;
                    for (int i = 0; i < loop_count; i++)
                        bw.Write(result, i * m_option.BufferSize.Value, m_option.BufferSize.Value);

                    if (remain > 0)
                        bw.Write(result, loop_count * m_option.BufferSize.Value, remain);
                }
            } catch {
                sin.Dispose();
                FileHelper.DeleteFileIgnoreErrors(output);
                return TaskResult.StreamWriteError;
            }

            if (m_option.Cleanup) {
                try { File.Delete(m_option.In[0]); } catch {
                    sin.Dispose();
                    return TaskResult.CleanupFailedSucceed;
                }
            }

            return TaskResult.Success;
        }

        public TaskResult Process() {
            if (m_option.In.Count == 0) return TaskResult.NoInputFile;
            if (m_option.ShowHeaderInfo) {
                return ShowHeaderInfo();
            }
            if (!CheckOptions()) return TaskResult.AmbiguousOption;
            if (m_option.BufferSize.HasValue) {
                if (m_option.BufferSize.Value < 128) return TaskResult.InvalidBufferSize;
            } else m_option.BufferSize = DefaultBufferSize;

            m_alg = AlgorithmManager.CreateAlgorithm((int)m_option.Algorithm);
            if (m_alg == null) return TaskResult.InvalidAlgorithm;

            TaskResult r;
            r = SetupAlgorithm();
            if (r != TaskResult.Success) return r;

            FileStream fs;
            r = FileHelper.OpenFileStream(m_option.In[0], out fs);
            if (r != TaskResult.Success) return r;

            if (m_option.RunTest) return TestProcess(fs);

            string output;
            r = FileHelper.BuildOutput(m_option, out output);
            
            if (r != TaskResult.Success) {
                fs.Dispose();
                return r;
            }

            FileStream ofs;
            try { ofs = new FileStream(output, m_option.Overwrite ? FileMode.Create : FileMode.CreateNew); }
            catch (IOException) {
                fs.Dispose();
                return TaskResult.FileAlreadyExists;
            }
            catch {
                fs.Dispose();
                return TaskResult.IOError;
            }

            r = m_option.Decrypt ? DecryptProcess(fs, ofs, output) : EncryptProcess(fs, ofs, output);
            if (r != TaskResult.Success) FileHelper.DeleteFileIgnoreErrors(output);
            return r;
        }

        public void Dispose() {

        }

        /// <summary>
        /// 작업에 사용되는 암호 및 IV를 설정합니다.<br />
        /// Set a password and IV used by cryptographic function.
        /// </summary>
        TaskResult SetupAlgorithm() {
            TaskResult r;
            string pw, iv = null;
            if (string.IsNullOrEmpty(m_option.Password)) pw = IDHelper.MachineUserName();
            else pw = m_option.Password;

            Stream spw, siv;

            r = GetDataStream(pw, out spw);
            if (r != TaskResult.Success) return r;

            byte[] temp = new byte[m_alg.Key.Length];
            Buffer.BlockCopy(HashAPI.ComputeHash(spw), 0, temp, 0, temp.Length);
            m_alg.Key = temp;
            spw.Dispose();

            if (string.IsNullOrEmpty(m_option.IV)) {
                temp = new byte[m_alg.IV.Length];
                Buffer.BlockCopy(DefaultIV, 0, temp, 0, temp.Length);
                m_alg.IV = temp;
            } else {
                r = GetDataStream(iv, out siv);
                if (r != TaskResult.Success) return r;
                temp = new byte[m_alg.IV.Length];
                Buffer.BlockCopy(HashAPI.ComputeHash(siv), 0, temp, 0, temp.Length);
                m_alg.IV = temp;
                spw.Dispose();
            }

            return TaskResult.Success;
        }
        /// <summary>
        /// 옵션을 검사합니다.<br />
        /// Check an option.
        /// </summary>
        bool CheckOptions() {
            List<bool> chkopt = new List<bool>();
            chkopt.Add(m_option.Cleanup && m_option.Overwrite);
            chkopt.Add(m_option.RunTest && (!string.IsNullOrEmpty(m_option.Out)));
            foreach (bool statement in chkopt) {
                if (statement) return false;
            }

            return true;
        }
        /// <summary>
        /// 소스로부터 데이터 스트림을 가져옵니다.<br />
        /// Get a data stream for source.
        /// </summary>
        /// <param name="source">
        /// 데이터 스트림을 가져올 소스 텍스트입니다.<br />
        /// A source text to get data stream.
        /// </param>
        /// <param name="s">
        /// 데이터 스트림이 저장될 변수입니다.<br />
        /// A variable to store data stream.
        /// </param>
        TaskResult GetDataStream(string source, out Stream s) {
            s = null;
            if (source.StartsWith(CipherFromFile)) {
                FileStream fs;
                TaskResult r = FileHelper.OpenFileStream(source.Substring(CipherFromFile.Length), out fs);
                if (r == TaskResult.FileNotFound) {
                    byte[] b = Common.SystemEncoding.GetBytes(source.Substring(CipherFromFile.Length));
                    s = new MemoryStream(b);
                } else if (r == TaskResult.Success) s = fs;
                return r;
            } else if (source.StartsWith(CipherPlainText)) {
                try {
                    byte[] b = Common.SystemEncoding.GetBytes(source.Substring(CipherPlainText.Length));
                    s = new MemoryStream(b);
                    return TaskResult.Success;
                } catch (EncoderFallbackException) {
                    return TaskResult.EncodingError;
                }
            } else {
                try {
                    byte[] b = Common.SystemEncoding.GetBytes(source);
                    s = new MemoryStream(b);
                    return TaskResult.Success;
                } catch (EncoderFallbackException) {
                    return TaskResult.EncodingError;
                }
            }
        }
        /// <summary>
        /// 헤더의 정보를 출력합니다.<br />
        /// Print a information of header.
        /// </summary>
        TaskResult ShowHeaderInfo() {
            TaskResult r;
            HeaderBase hdr = HeaderManager.FromFile(m_option.In[0], out r);
            if (r == TaskResult.Success)
                Console.WriteLine(hdr.TraverseProperties().ToString());
            else {
                Console.WriteLine("ERROR: INVALID_HEADER_OR_NOT_VALID_FILE ({0})", r);
            }
            return r;
        }

        /// <summary>
        /// <see cref="Stream"/> 개체의 데이터를 암호화합니다.<br />
        /// Encrypt a data of <see cref="Stream"/> instance.
        /// </summary>
        /// <param name="s">
        /// 암호화될 <see cref="Stream"/> 개체입니다.
        /// <see cref="Stream"/> instance to be encrypted.
        /// </param>
        /// <param name="alg">
        /// 암호화에 사용되는 <see cref="SymmetricAlgorithm"/> 개체입니다.<br />
        /// <see cref="SymmetricAlgorithm"/> instance used by encryption.
        /// </param>
        /// <param name="result">
        /// 결과 데이터가 저장되는 <see cref="byte"/> 배열입니다.<br />
        /// A variable of <see cref="byte"/> array to store result data.
        /// </param>
        TaskResult Encrypt(Stream s, SymmetricAlgorithm alg, out byte[] result) {
            result = null;
            MemoryStream ms = new MemoryStream();

            ICryptoTransform ct = alg.CreateEncryptor();
            using (CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write)) {
                byte[] temp;
                try { temp = new byte[m_option.BufferSize.Value]; }
                catch {
                    ct.Dispose();
                    ms.Dispose();
                    return TaskResult.OutOfMemory;
                }
                
                int nRead;
                try { nRead = s.Read(temp, 0, m_option.BufferSize.Value); }
                catch {
                    ct.Dispose();
                    return TaskResult.StreamReadError;
                }
                
                while (nRead > 0) {
                    try { cs.Write(temp, 0, nRead); } catch {
                        ct.Dispose();
                        return TaskResult.StreamWriteError;
                    }

                    try { nRead = s.Read(temp, 0, m_option.BufferSize.Value); } catch {
                        ct.Dispose();
                        return TaskResult.StreamReadError;
                    }
                }
            }

            result = ms.ToArray();
            return TaskResult.Success;
        }
        /// <summary>
        /// <see cref="Stream"/> 개체의 데이터를 복호화합니다.<br />
        /// Decrypt a data of <see cref="Stream"/> instance.
        /// </summary>
        /// <param name="s">
        /// 복호화될 <see cref="Stream"/> 개체입니다.
        /// <see cref="Stream"/> instance to be decrypted.
        /// </param>
        /// <param name="alg">
        /// 복호화에 사용되는 <see cref="SymmetricAlgorithm"/> 개체입니다.<br />
        /// <see cref="SymmetricAlgorithm"/> instance used by decryption.
        /// </param>
        /// <param name="result">
        /// 결과 데이터가 저장되는 <see cref="byte"/> 배열입니다.<br />
        /// A variable of <see cref="byte"/> array to store result data.
        /// </param>
        TaskResult Decrypt(Stream s, SymmetricAlgorithm alg, out byte[] result) {
            result = null;
            MemoryStream ms = new MemoryStream();
            ICryptoTransform ct = alg.CreateDecryptor();
            int size = m_option.BufferSize.Value;

            using (CryptoStream cs = new CryptoStream(s, ct, CryptoStreamMode.Read)) {
                byte[] temp;
                try { temp = new byte[size]; }
                catch {
                    SafeDispose(cs);
                    ct.Dispose();
                    return TaskResult.OutOfMemory;
                }
                
                int nRead;
                try { nRead = cs.Read(temp, 0, size); }
                catch {
                    SafeDispose(cs);
                    ct.Dispose();
                    return TaskResult.InvalidPasswordOrDataCorrupted;
                }

                while (nRead > 0) {
                    try {
                        ms.Write(temp, 0, nRead);
                        nRead = cs.Read(temp, 0, size);
                    } catch {
                        SafeDispose(cs);
                        ct.Dispose();
                        return TaskResult.InvalidPasswordOrDataCorrupted;
                    }
                }
            }

            result = ms.ToArray();
            ct.Dispose();
            ms.Dispose();
            return TaskResult.Success;
        }

        /// <summary>
        /// 안전하게 <see cref="CryptoStream"/> 개체에서 사용하는 모든 리소스를 해제합니다.<br />
        /// Safely disposes <see cref="CryptoStream"/> instance.
        /// </summary>
        /// <param name="cs">
        /// 리소스를 해제할 <see cref="CryptoStream"/> 개체입니다.<br />
        /// A instance of <see cref="CryptoStream"/> to disposed.
        /// </param>
        void SafeDispose(CryptoStream cs) {
            try { cs.Dispose(); }
            catch (CryptographicException) {
                Type tcs = typeof(CryptoStream);
                FieldInfo _finalBlockTransformed = tcs.GetField("_finalBlockTransformed", BindingFlags.NonPublic | BindingFlags.Instance);
                if (_finalBlockTransformed != null) {
                    try { _finalBlockTransformed.SetValue(cs, true); } catch { }
                }
                else return;
                cs.Dispose();
            }
        }
    }
}