
/// <summary>
/// 작업의 결과를 나타내는 값을 열거합니다.
/// </summary>
public enum TaskResult : ushort {
    /// <summary>
    /// 작업이 성공했습니다.
    /// </summary>
    Success = 0,

    /// <summary>
    /// 파일이 존재합니다.
    /// </summary>
    FileExists = 1,


    /// <summary>
    /// 데이터가 없습니다.
    /// </summary>
    NoData = 10,

    /// <summary>
    /// 초기 벡터(IV)가 없습니다.
    /// </summary>
    NoIV = 11,

    /// <summary>
    /// 입력 파일이 없습니다.
    /// </summary>
    NoInputFiles = 12,


    /// <summary>
    /// 유효하지 않은 알고리즘입니다.
    /// </summary>
    InvalidAlgorithm = 40,

    /// <summary>
    /// 유효하지 않은 키 길이입니다.
    /// </summary>
    InvalidKeySize = 41,

    /// <summary>
    /// 유효하지 않은 시그니쳐입니다.
    /// </summary>
    InvalidSignature = 43,

    /// <summary>
    /// 유효하지 않은 헤더 필드 값입니다.
    /// </summary>
    InvalidHeaderFieldValue = 44,

    /// <summary>
    /// 유효하지 않은 헤더입니다.
    /// </summary>
    InvalidHeader = 45,

    /// <summary>
    /// 유효하지 않은 매개 변수입니다.
    /// </summary>
    InvalidParameter = 46,

    /// <summary>
    /// 유효하지 않은 버전입니다.
    /// </summary>
    InvalidVersion = 47,

    /// <summary>
    /// 유효하지 않은 암호입니다.
    /// </summary>
    InvalidPassword = 48,

    /// <summary>
    /// 유효하지 않은 초기 벡터(IV)입니다.
    /// </summary>
    InvalidIV = 49,


    /// <summary>
    /// 알고리즘 개체 초기화에 실패했습니다.
    /// </summary>
    AlgorithmInitiateFailure = 50,

    /// <summary>
    /// 입출력 오류입니다.
    /// </summary>
    IOError = 51,

    /// <summary>
    /// 인코딩 오류입니다.
    /// </summary>
    EncodingError = 52,

    /// <summary>
    /// 파일 열기 오류입니다.
    /// </summary>
    FileOpenError = 53,

    /// <summary>
    /// 스트림 오류입니다.
    /// </summary>
    StreamError = 54,

    /// <summary>
    /// 메모리가 부족합니다.
    /// </summary>
    OutOfMemory = 55,

    /// <summary>
    /// 경로가 너무 깁니다.
    /// </summary>
    PathTooLong = 56,



    /// <summary>
    /// 파일을 찾을 수 없습니다.
    /// </summary>
    FileNotFound = 60,

    /// <summary>
    /// 디렉터리를 찾을 수 없습니다.
    /// </summary>
    DirectoryNotFound = 61,

    /// <summary>
    /// 접근이 거부되었습니다.
    /// </summary>
    AccessDenied = 62,


    /// <summary>
    /// 지원되지 않는 버전입니다.
    /// </summary>
    NotSupportedVersion = 70,


    /// <summary>
    /// 헤더 체크섬이 올바르지 않습니다.
    /// </summary>
    IncorrectHeaderChecksum = 80,
}