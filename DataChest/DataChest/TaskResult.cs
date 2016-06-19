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

namespace DataChest {
    /// <summary>
    /// 작업의 결과를 나타내는 값을 열거합니다.<br />
    /// Enumerate result of operation.
    /// </summary>
    public enum TaskResult : ushort {
        Success = 0,

        // System compatible code
        InvalidParameter = 87,

        CleanupFailedSucceed = 1,

        NoData = 10,
        NoIV = 11,
        NoInputFile = 12,
        
        InvalidAlgorithm = 40,
        InvalidSignature,
        InvalidHeaderFieldValue,
        InvalidHeader,
        InvalidVersion,
        InvalidPassword,
        InvalidIV,
        InvalidBufferSize,
        InvalidHeaderClass,
        InvalidPhrasePrefix,
        
        AlgorithmInitiateFailure = 70,
        IOError,
        EncodingError,
        FileOpenError,
        StreamError,
        StreamReadError,
        StreamWriteError,
        OutOfMemory,
        PathTooLong,
        AccessDenied,
        
        FileNotFound = 90,
        FileAlreadyExists,
        DirectoryNotFound,

        NotSupportedVersion = 100,
        AmbiguousOption,
        
        IncorrectHeaderChecksum = 110,
        IncorrectRawDataChecksum,
        IncorrectEncryptedDataChecksum,
        
        CannotCreateHeaderInstance = 120,
        HeaderVersionNotMatch,
        
        ErrorCausedUDPR = 130,
    }
}