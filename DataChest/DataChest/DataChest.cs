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


            return TaskResult.Success;
        }

        public void Dispose() {

        }
        
        /// <summary>
        /// 옵션을 검사합니다.<br />
        /// Check an option.
        /// </summary>
        bool CheckOptions() {
            List<bool> chkopt = new List<bool>();
            chkopt.Add(m_option.Cleanup || m_option.Overwrite);
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
                s = fs;
                return r;
            } else if (source.StartsWith(CipherPlainText)) {
                FileStream fs;
                TaskResult r = FileHelper.OpenFileStream(source.Substring(CipherFromFile.Length), out fs);
                s = fs;
                return r;
            } else return TaskResult.InvalidPhrasePrefix;
        }
        /// <summary>
        /// 헤더의 정보를 출력합니다.<br />
        /// Print a information of header.
        /// </summary>
        TaskResult ShowHeaderInfo() {
            TaskResult r;
            HeaderBase hdr = HeaderManager.FromFile(m_option.In[0], out r);
            if (r == TaskResult.Success) Console.WriteLine(hdr.TraverseProperties().ToString());
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
            int size = m_option.BufferSize.Value;

            ICryptoTransform ct = alg.CreateEncryptor();
            using (CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write)) {
                byte[] temp;
                try { temp = new byte[size]; }
                catch {
                    ct.Dispose();
                    ms.Dispose();
                    return TaskResult.OutOfMemory;
                }
                
                int nRead;
                try { nRead = s.Read(temp, 0, size); }
                catch {
                    ct.Dispose();
                    return TaskResult.StreamReadError;
                }
                
                while (nRead > 0) {
                    try { cs.Write(temp, 0, nRead); } catch {
                        ct.Dispose();
                        return TaskResult.StreamWriteError;
                    }

                    try { nRead = s.Read(temp, 0, size); } catch {
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
                    return TaskResult.StreamReadError;
                }

                while (nRead > 0) {
                    try {
                        ms.Write(temp, 0, nRead);
                        nRead = cs.Read(temp, 0, size);
                    } catch {
                        SafeDispose(cs);
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