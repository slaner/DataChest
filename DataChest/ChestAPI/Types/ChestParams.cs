using System;
using System.IO;

/// <summary>
/// ChestAPI 호출에 필요한 정보를 저장하는 클래스입니다.
/// </summary>
sealed class ChestParams {
    // Password and IV parsing
    // K for Key (Plain-text)
    // F for File (Read from file)

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


    public ChestParams() {
        g_version = ChestAPI.Version;
        g_verify = true;
        g_runTest = false;
        g_iv = "";
        ResetPassword();
    }


    /// <summary>
    /// 암호를 기본 값으로 되돌립니다.
    /// </summary>
    public void ResetPassword() {
        // Basic password combination
        // K:USERNAME@MACHINENAME
        g_password = "K:" + Environment.UserName + "@" + Environment.MachineName;
        g_passwordFromFile = false;
    }

    /// <summary>
    /// 암호를 설정합니다. 암호가 null일 경우 암호를 기본 값으로 되돌립니다.
    /// </summary>
    /// <param name="value">암호입니다.</param>
    /// <param name="fromFile">파일 경로일 경우 true를 입력합니다.</param>
    public void SetPassword(string value, bool fromFile) {
        if (string.IsNullOrEmpty(value)) {
            ResetPassword();
            return;
        }

        g_passwordFromFile = fromFile;
        if (fromFile)
            g_password = "F:" + value;
        else
            g_password = "K:" + value;
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
    /// <param name="fromFile">파일 경로일 경우 true를 입력합니다.</param>
    public void SetIV(string value, bool fromFile) {
        if (string.IsNullOrEmpty(value)) {
            ResetIV();
            return;
        }

        g_ivFromFile = fromFile;
        if (fromFile)
            g_iv = "F:" + value;
        else
            g_iv = "K:" + value;
    }

    /// <summary>
    /// IV를 검증합니다.
    /// </summary>
    internal bool VerifyIV() {
        if (string.IsNullOrEmpty(g_iv)) return true;

        if (g_iv.StartsWith("F:"))
            return File.Exists(g_iv.Substring(2));
        else if (g_iv.StartsWith("K:"))
            return true;
        return false;
    }

    /// <summary>
    /// 암호를 검증합니다.
    /// </summary>
    internal bool VerifyPassword() {
        if (g_password.StartsWith("F:"))
            return File.Exists(g_password.Substring(2));
        else if (g_password.StartsWith("K:"))
            return true;
        return false;
    }

    /// <summary>
    /// 암호 데이터에 대한 스트림을 가져옵니다.
    /// </summary>
    /// <param name="s">암호 데이터의 스트림입니다.</param>
    internal TaskResult GetPasswordDataStream(out Stream s) {
        s = null;
        if (g_passwordFromFile) {
            try {
                s = File.OpenRead(g_password.Substring(2));
                return TaskResult.Success;
            } catch (UnauthorizedAccessException uae) {
                return TaskResult.AccessDenied;
            } catch {
                return TaskResult.IOError;
            }
        } else {
            try {
                byte[] b = ChestAPI.SystemUnicodeEncoding.GetBytes(g_password.Substring(2));
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
            TaskResult r = FileHelper.OpenFileStream(g_iv.Substring(2), out fs);
            if (r != TaskResult.Success) return r;
            s = fs;
            return TaskResult.Success;
        } else {
            try {
                byte[] b = ChestAPI.SystemUnicodeEncoding.GetBytes(g_iv.Substring(2));
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
}