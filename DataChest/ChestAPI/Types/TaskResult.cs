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

namespace DataChest.Types {
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
        /// 작업이 성공했지만 정리에 실패했습니다.
        /// </summary>
        CleanupFailedSucceed = 2,

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
        /// 유효하지 않은 버퍼 크기입니다.
        /// </summary>
        InvalidBufferSize,

        /// <summary>
        /// 유효하지 않은 헤더 클래스입니다.
        /// </summary>
        InvalidHeaderClass,



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
        /// 스트림 오류입니다.
        /// </summary>
        StreamError,

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

        /// <summary>
        /// 원본 데이터의 체크섬이 올바르지 않습니다.
        /// </summary>
        IncorrectRawDataChecksum,

        /// <summary>
        /// 암호화된 데이터의 체크섬이 올바르지 않습니다.
        /// </summary>
        IncorrectEncryptedDataChecksum,


        /// <summary>
        /// 헤더의 개체를 만들지 못했습니다.
        /// </summary>
        CannotCreateHeaderInstance = 90,

        /// <summary>
        /// 헤더 버전이 맞지 않습니다.
        /// </summary>
        HeaderVersionNotMatch,




        /// <summary>
        /// 사용자 정의 처리 루틴(UDPR, User Defined Processing Routine)에서 오류가 발생했습니다.
        /// </summary>
        ErrorCausedUDPR = 130,
    }
}