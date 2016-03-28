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

/// <summary>
/// ChestAPI 호출에 필요한 정보를 저장하는 클래스입니다.
/// </summary>
sealed class ChestParams {
    const string PhraseFileLookupPrefix = "FF:";
    const string PhrasePlainTextPrefix = "FT:";


    string g_password;          // Password for file
    string g_output;            // Output filename (To be created)
    string g_input;             // Input filename
    string g_iv;                // Initial Vector
    ushort g_version;           // Version for CHEST_HEADER.
    Algorithms g_algorithm;     // Algorithm used for Encryption and Decryption.
    bool g_verify;              // Determine uses checksum verify.
    bool g_ivFromFile;          // True if read IV from file.
    bool g_passwordFromFile;    // True if read Password from file.
    bool g_overwriteFile;       // Determine overwrite existing output file.
    bool g_runTest;             // True if run the test from input file.
    bool g_encrypt;             // Determine do encrypt or decrypt.


    public ChestParams() {
        g_version = ChestAPI.Version;
        g_verify = true;
        g_runTest = false;
        g_encrypt = true;
        g_iv = "";
        ResetPassword();
    }


    /// <summary>
    /// 암호를 기본 값으로 되돌립니다.
    /// </summary>
    public void ResetPassword() {
        // Basic password combination
        // PhrasePlainTextPrefix + USERNAME + "@" + MACHINENAME
        g_password = PhrasePlainTextPrefix + Environment.UserName + "@" + Environment.MachineName;
        g_passwordFromFile = false;
    }

    /// <summary>
    /// 암호를 설정합니다. 암호가 null일 경우 암호를 기본 값으로 되돌립니다.
    /// </summary>
    /// <param name="value">암호입니다.</param>
    public void SetPassword(string value) {
        if (string.IsNullOrEmpty(value)) {
            ResetPassword();
            return;
        }

        // PhraseFileLookupPrefix로 시작하는 경우 (파일 찾기)
        if (value.StartsWith(PhraseFileLookupPrefix)) {
            // 길이가 2인 경우 (파일 경로가 명시되지 않음)
            if ( value.Length == 2) {
                ResetPassword();
                return;
            }

            string path = value.Substring(2);
            if(File.Exists(path)) {
                g_password = PhraseFileLookupPrefix + path;
                g_passwordFromFile = true;
            } else {
                g_password = PhrasePlainTextPrefix + value;
                g_passwordFromFile = false;
            }
        }
        // PhrasePlainTextPrefix로 시작하는 경우 (문자열 그 자체)
        else if (value.StartsWith(PhrasePlainTextPrefix)) {
            g_password = value;
            g_passwordFromFile = false;
        }
        // 자동 검사
        else {
            if (File.Exists(value)) {
                g_password = PhraseFileLookupPrefix + value;
                g_passwordFromFile = true;
            } else {
                g_password = PhrasePlainTextPrefix + value;
                g_passwordFromFile = false;
            }
        }
    }

    /// <summary>
    /// 초기 벡터를 기본 값으로 되돌립니다.
    /// </summary>
    public void ResetIV() {
        g_iv = null;
        g_ivFromFile = false;
    }

    /// <summary>
    /// 초기 벡터를 설정합니다.
    /// </summary>
    /// <param name="value">초기 벡터입니다.</param>
    public void SetIV(string value) {
        if (string.IsNullOrEmpty(value)) {
            ResetIV();
            return;
        }

        // PhraseFileLookupPrefix로 시작하는 경우 (파일 찾기)
        if (value.StartsWith(PhraseFileLookupPrefix)) {
            // 길이가 2인 경우 (파일 경로가 명시되지 않음)
            if (value.Length == 2) {
                ResetIV();
                return;
            }

            string path = value.Substring(2);
            if (File.Exists(path)) {
                g_iv = PhraseFileLookupPrefix + path;
                g_ivFromFile = true;
            } else {
                g_iv = PhrasePlainTextPrefix + value;
                g_ivFromFile = false;
            }
        }
        // PhrasePlainTextPrefix로 시작하는 경우 (문자열 그 자체)
        else if (value.StartsWith(PhrasePlainTextPrefix)) {
            g_iv = value;
            g_ivFromFile = false;
        }
        // 자동 검사
        else {
            if (File.Exists(value)) {
                g_iv = PhraseFileLookupPrefix + value;
                g_ivFromFile = true;
            } else {
                g_iv = PhrasePlainTextPrefix + value;
                g_ivFromFile = false;
            }
        }
    }
    
    /// <summary>
    /// 암호 데이터에 대한 스트림을 가져옵니다.
    /// </summary>
    /// <param name="s">암호 데이터의 스트림입니다.</param>
    internal TaskResult GetPasswordDataStream(out Stream s) {
        s = null;
        if (g_passwordFromFile) {
            FileStream fs;
            TaskResult r = FileHelper.OpenFileStream(g_password.Substring(PhraseFileLookupPrefix.Length), out fs);
            if (r != TaskResult.Success) return r;
            s = fs;
            return TaskResult.Success;
        } else {
            try {
                byte[] b = ChestAPI.SystemUnicodeEncoding.GetBytes(g_password.Substring(PhraseFileLookupPrefix.Length));
                s = new MemoryStream(b);
                return TaskResult.Success;
            } catch (System.Text.EncoderFallbackException efe) {
                return TaskResult.EncodingError;
            }
        }
    }

    /// <summary>
    /// 초기 벡터(IV) 데이터에 대한 스트림을 가져옵니다.
    /// </summary>
    /// <param name="s">초기 벡터(IV) 데이터의 스트림입니다.</param>
    internal TaskResult GetIVDataStream(out Stream s) {
        s = null;
        if (string.IsNullOrEmpty(g_iv)) return TaskResult.NoIV;

        if (g_ivFromFile) {
            FileStream fs;
            TaskResult r = FileHelper.OpenFileStream(g_iv.Substring(PhrasePlainTextPrefix.Length), out fs);
            if (r != TaskResult.Success) return r;
            s = fs;
            return TaskResult.Success;
        } else {
            try {
                byte[] b = ChestAPI.SystemUnicodeEncoding.GetBytes(g_iv.Substring(PhrasePlainTextPrefix.Length));
                s = new MemoryStream(b);
                return TaskResult.Success;
            } catch (System.Text.EncoderFallbackException efe) {
                return TaskResult.EncodingError;
            }
        }
    }


    /// <summary>
    /// 작업할 파일의 경로입니다.
    /// </summary>
    public string InputFile {
        get { return g_input; }
        set { g_input = value; }
    }

    /// <summary>
    /// 작업 후 생성될 파일의 경로입니다.
    /// </summary>
    public string OutputFile {
        get { return g_output; }
        set { g_output = value; }
    }

    /// <summary>
    /// ChestAPI 버전입니다.
    /// </summary>
    public ushort Version {
        get { return g_version; }
        set { g_version = value; }
    }

    /// <summary>
    /// 체크섬 검증 여부입니다.
    /// </summary>
    public bool Verify {
        get { return g_verify; }
        set { g_verify = value; }
    }

    /// <summary>
    /// 출력 파일이 존재할 경우 덮어쓸 것인지에 대한 여부입니다.
    /// </summary>
    public bool Overwrite {
        get { return g_overwriteFile; }
        set { g_overwriteFile = value; }
    }
    
    /// <summary>
    /// 작업에 사용할 알고리즘입니다.
    /// </summary>
    public Algorithms Algorithm {
        get { return g_algorithm; }
        set { g_algorithm = value; }
    }

    /// <summary>
    /// 설정된 암호입니다.
    /// </summary>
    public string Password {
        get { return g_password; }
    }

    /// <summary>
    /// 설정된 초기 벡터입니다.
    /// </summary>
    public string IV {
        get { return g_iv; }
    }

    /// <summary>
    /// 암호를 파일로부터 읽어오는지에 대한 여부입니다.
    /// </summary>
    public bool PasswordFromFile {
        get { return g_passwordFromFile; }
    }

    /// <summary>
    /// 초기 벡터(IV)를 파일로부터 읽어오는지에 대한 여부입니다.
    /// </summary>
    public bool IVFromFile {
        get { return g_ivFromFile; }
    }

    /// <summary>
    /// 출력 파일을 생성하지 않고 암복호화 기능을 검사하는지에 대한 여부입니다.
    /// </summary>
    public bool RunTest {
        get { return g_runTest; }
        set { g_runTest = value; }
    }

    /// <summary>
    /// 파일을 암호화할 것인지 복호화할 것인지에 대한 여부입니다.
    /// </summary>
    public bool Encrypt {
        get { return g_encrypt; }
        set { g_encrypt = value; }
    }
}