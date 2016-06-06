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
using DataChest.Types;

namespace DataChest {
    /// <summary>
    /// 파일 작업을 간편화해놓은 함수를 노출하는 클래스입니다.
    /// </summary>
    static class FileHelper {
        /// <summary>
        /// 지정한 파일에 대한 파일 스트림을 엽니다.
        /// </summary>
        /// <param name="fileName">열 파일입니다.</param>
        /// <param name="fs">열린 파일의 스트림이 저장될 변수입니다.</param>
        public static TaskResult OpenFileStream(string fileName, out FileStream fs) {
            fs = null;
            try {
                fs = File.OpenRead(fileName);
            } catch (FileNotFoundException) {
                return TaskResult.FileNotFound;
            } catch (UnauthorizedAccessException) {
                return TaskResult.AccessDenied;
            } catch (DirectoryNotFoundException) {
                return TaskResult.DirectoryNotFound;
            } catch (PathTooLongException) {
                return TaskResult.PathTooLong;
            } catch (ArgumentException) {
                return TaskResult.InvalidParameter;
            } catch {
                return TaskResult.IOError;
            }
            return TaskResult.Success;
        }
        /// <summary>
        /// 출력 경로를 만듭니다.
        /// </summary>
        /// <param name="cp">출력 경로를 만드는데 참고할 <see cref="ChestParams" /> 개체입니다.</param>
        /// <param name="output">만들어진 출력 경로가 저장될 변수입니다.</param>
        public static TaskResult BuildOutput(ChestParams cp, out string output) {
            output = null;
            string extension;
            extension = cp.Encrypt ? ".dcf" : "";
            if (string.IsNullOrEmpty(cp.OutputFile))
                output = Environment.CurrentDirectory + "/" + Path.GetFileNameWithoutExtension(cp.InputFile) + extension;
            else {
                // 확장자가 있는지 없는지 확인한다.
                string filename = Path.GetFileName(cp.OutputFile);
                string ext = Path.GetExtension(filename);

                // 파일명이 없는 경우
                if (string.IsNullOrEmpty(filename))
                    filename = Path.GetFileNameWithoutExtension(cp.InputFile) + extension;

                // 파일명은 있는데 확장자가 dcf 가 아니거나 없는 경우
                else
                    if (cp.Encrypt && (string.IsNullOrEmpty(ext) || !ext.Equals(".dcf", StringComparison.InvariantCultureIgnoreCase)))
                    filename += extension;

                // 디렉터리 정보를 가져온다.
                string dir = Path.GetDirectoryName(cp.OutputFile);

                // 디렉터리 정보가 없음 (파일명만 있는 경우)
                if (string.IsNullOrEmpty(dir))

                    // 출력 파일을 조합한다.
                    output = Environment.CurrentDirectory + "/" + filename;

                // 디렉터리 정보가 있는 경우
                else {

                    // 루트 경로의 정보가 없는 경우 (루트 드라이브가 지정되지 않음)
                    if (string.IsNullOrEmpty(Path.GetPathRoot(dir))) {

                        // 현재 디렉터리와 출력 디렉터리를 조합한다.
                        output = Environment.CurrentDirectory + "/" + dir;

                        // 디렉터리가 있는지 확인한다.
                        if (!Directory.Exists(output))
                            return TaskResult.DirectoryNotFound;

                        output += filename;
                    }

                    // 루트 경로의 정보가 있는 경우
                    else {

                        // 디렉터리가 있는지 확인한다.
                        if (!Directory.Exists(dir))
                            return TaskResult.DirectoryNotFound;

                        output = dir + "/" + filename;
                    }
                }
            }
            return TaskResult.Success;
        }
        /// <summary>
        /// 파일을 삭제합니다. 파일 삭제 도중 발생하는 오류를 무시합니다.
        /// </summary>
        /// <param name="fileName">삭제할 파일입니다.</param>
        public static void DeleteFileIgnoreErrors(string fileName) {
            try { File.Delete(fileName); } catch { }
        }
    }
}