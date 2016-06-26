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

using System.Reflection;
using System.Resources;

namespace DataChest {
    static class SR {
        static readonly string[] m_translations = {
            "DC_Available_Algorithm_List",
            "DC_Err_AccessDenied",
            "DC_Err_AmbiguousOption",
            "DC_Err_DirectoryNotFound",
            "DC_Err_EncodingError",
            "DC_Err_ErrorCausedUDPR",
            "DC_Err_FileAlreadyExists",
            "DC_Err_FileNotFound",
            "DC_Err_HeaderVersionNotMatch",
            "DC_Err_IncorrectEncryptedDataChecksum",
            "DC_Err_IncorrectHeaderChecksum",
            "DC_Err_IncorrectRawDataChecksum",
            "DC_Err_InvalidAlgorithm",
            "DC_Err_InvalidBufferSize",
            "DC_Err_InvalidHeaderClass",
            "DC_Err_InvalidHeaderFieldValue",
            "DC_Err_InvalidParameter",
            "DC_Err_InvalidPasswordOrDataCorrupted",
            "DC_Err_InvalidSignature",
            "DC_Err_IOError",
            "DC_Err_NoIputFile",
            "DC_Err_NotSupportedVersion",
            "DC_Err_OutOfMemory",
            "DC_Err_PathTooLong",
            "DC_Err_StreamReadError",
            "DC_Err_StreamWriteError",
            "DC_Err_SucceedButCleanupFailed",
            "DC_Err_Success",
            "DC_Error_Code",
            "DC_Error_Description",
            "DC_File",
            "DC_License_Info",
            "DC_Option",
            "DC_Option_Algorithm",
            "DC_Option_BufferSize",
            "DC_Option_Cleanup",
            "DC_Option_Comment",
            "DC_Option_Decrypt",
            "DC_Option_DisableVerification",
            "DC_Option_HeaderVersion",
            "DC_Option_IV",
            "DC_Option_Out",
            "DC_Option_Overwrite",
            "DC_Option_Password",
            "DC_Option_RunTest",
            "DC_Option_ShowAlgorithmList",
            "DC_Option_ShowHeaderInfo",
            "DC_Option_ShowVersion",
            "DC_Option_Usage",
            "DC_Option_Verbose",
            "DC_PerformanceLogging_Start",
            "DC_PerformanceLogging_Ended",
            "DC_PerformanceLogging_Aborted",
            "DC_Usage",
            "DC_Used_Library",
            "DC_PerformanceLogging_Checkpoint_Created",
            "DC_PerformanceLogging_Checkpoint_Finished",
            "DC_PerformanceLogging_Speed",
            "DC_PerformanceLogging_ElapsedTime",
            "DC_PerformanceLogging_None",
            "DC_PerformanceLogging_Error",
        };

        readonly static ResourceManager m_res;

        static SR() {
            m_res = new ResourceManager("DataChest.language", Assembly.GetExecutingAssembly());
        }

        public static string GetString(string s) {
            if (string.IsNullOrEmpty(s)) return null;
            try { return m_res.GetString(s); }
            catch { return s; }
        }
    }
}