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

/// <summary>
/// 작업의 결과를 나타내는 값을 열거합니다.
/// </summary>
public enum TaskResult : ushort {
    /// <summary>
    /// 작업이 성공했습니다.
    /// </summary>
    Success = 0,

    /// <summary>
    /// 파일이 이미 존재합니다.
    /// </summary>
    FileAlreadyExists = 1,


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
    /// 유효하지 않은 시그니쳐입니다.
    /// </summary>
    InvalidSignature,

    /// <summary>
    /// 유효하지 않은 헤더 필드 값입니다.
    /// </summary>
    InvalidHeaderFieldValue,

    /// <summary>
    /// 유효하지 않은 헤더입니다.
    /// </summary>
    InvalidHeader,

    /// <summary>
    /// 유효하지 않은 매개 변수입니다.
    /// </summary>
    InvalidParameter,

    /// <summary>
    /// 유효하지 않은 버전입니다.
    /// </summary>
    InvalidVersion,

    /// <summary>
    /// 유효하지 않은 암호입니다.
    /// </summary>
    InvalidPassword,

    /// <summary>
    /// 유효하지 않은 초기 벡터(IV)입니다.
    /// </summary>
    InvalidIV,


    /// <summary>
    /// 알고리즘 초기화에 실패했습니다.
    /// </summary>
    AlgorithmInitiateFailure = 50,

    /// <summary>
    /// 입출력 오류입니다.
    /// </summary>
    IOError,

    /// <summary>
    /// 인코딩 오류입니다.
    /// </summary>
    EncodingError,

    /// <summary>
    /// 파일 열기 오류입니다.
    /// </summary>
    FileOpenError,

    /// <summary>
    /// 스트림 읽기 오류입니다.
    /// </summary>
    StreamReadError,

    /// <summary>
    /// 스트림 쓰기 오류입니다.
    /// </summary>
    StreamWriteError,
    
    /// <summary>
    /// 메모리가 부족합니다.
    /// </summary>
    OutOfMemory,

    /// <summary>
    /// 경로가 너무 깁니다.
    /// </summary>
    PathTooLong,



    /// <summary>
    /// 파일을 찾을 수 없습니다.
    /// </summary>
    FileNotFound = 60,

    /// <summary>
    /// 디렉터리를 찾을 수 없습니다.
    /// </summary>
    DirectoryNotFound,

    /// <summary>
    /// 접근이 거부되었습니다.
    /// </summary>
    AccessDenied,


    /// <summary>
    /// 지원되지 않는 버전입니다.
    /// </summary>
    NotSupportedVersion = 70,

    /// <summary>
    /// 모호한 옵션입니다.
    /// </summary>
    AmbiguousOption = 71,


    /// <summary>
    /// 헤더 체크섬이 올바르지 않습니다.
    /// </summary>
    IncorrectHeaderChecksum = 80,
}