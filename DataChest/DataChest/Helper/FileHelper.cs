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

namespace DataChest {
    /// <summary>
    /// 파일 작업을 간편화해놓은 함수를 노출하는 클래스입니다.<br />
    /// Exposes function that simplify file tasks.
    /// </summary>
    static class FileHelper {
        /// <summary>
        /// 지정된 이름을 가진 파일을 만듭니다.<br />
        /// Create a file for specified filename.
        /// </summary>
        /// <param name="fileName">
        /// 만들 파일입니다.<br />
        /// Filename to be created.
        /// </param>
        /// <param name="fs">
        /// 만들어진 파일의 스트림이 저장될 변수입니다.<br />
        /// A variable to store file stream which created.
        /// </param>
        public static TaskResult CreateFileStream(string fileName, bool overwrite, out FileStream fs) {
            var checkpoint = DataChest.Logger?.OpenCheckpoint(nameof(CreateFileStream));
            fs = null;
            try {
                fs = new FileStream(fileName, overwrite ? FileMode.Create : FileMode.CreateNew);
            } catch (Exception e) {
                if (DataChest.Logger == null) return TaskResult.IOError;
                else return (TaskResult) DataChest.Logger?.Abort(TaskResult.IOError, e);
            }
            DataChest.Logger?.CloseCheckpoint(checkpoint, 0);
            return TaskResult.Success;
        }
        /// <summary>
        /// 지정한 파일에 대한 파일 스트림을 엽니다.<br />
        /// Open a file stream for specified file.
        /// </summary>
        /// <param name="fileName">
        /// 열 파일입니다.<br />
        /// File name to open.
        /// </param>
        /// <param name="fs">
        /// 열린 파일의 스트림이 저장될 변수입니다.<br />
        /// A variable to store file stream.
        /// </param>
        public static TaskResult OpenFileStream(string fileName, out FileStream fs) {
            var checkpoint = DataChest.Logger?.OpenCheckpoint(nameof(OpenFileStream));
            fs = null;
            try {
                fs = File.OpenRead(fileName);
            } catch (FileNotFoundException e1) {
                if (DataChest.Logger == null) return TaskResult.FileNotFound;
                return (TaskResult)DataChest.Logger?.Abort(TaskResult.FileNotFound, e1);
            } catch (UnauthorizedAccessException e2) {
                if (DataChest.Logger == null) return TaskResult.AccessDenied;
                return (TaskResult)DataChest.Logger?.Abort(TaskResult.AccessDenied, e2);
            } catch (DirectoryNotFoundException e3) {
                if (DataChest.Logger == null) return TaskResult.DirectoryNotFound;
                return (TaskResult)DataChest.Logger?.Abort(TaskResult.DirectoryNotFound, e3);
            } catch (PathTooLongException e4) {
                if (DataChest.Logger == null) return TaskResult.PathTooLong;
                return (TaskResult)DataChest.Logger?.Abort(TaskResult.PathTooLong, e4);
            } catch (ArgumentException e5) {
                if (DataChest.Logger == null) return TaskResult.InvalidParameter;
                return (TaskResult)DataChest.Logger?.Abort(TaskResult.InvalidParameter, e5);
            } catch (Exception e) {
                if (DataChest.Logger == null) return TaskResult.IOError;
                return (TaskResult)DataChest.Logger?.Abort(TaskResult.IOError, e);
            }
            DataChest.Logger?.CloseCheckpoint(checkpoint, fs.Length);
            return TaskResult.Success;
        }
        /// <summary>
        /// 출력 경로를 만듭니다.<br />
        /// Build an output path.
        /// </summary>
        /// <param name="option">
        /// 출력 경로를 만드는데 참고할 <see cref="Option"/> 개체입니다.<br />
        /// <see cref="Option"/> instance to reference when build output path.
        /// </param>
        /// <param name="output">
        /// 만들어진 출력 경로가 저장될 변수입니다.<br />
        /// A variable to store output path.
        /// </param>
        public static TaskResult BuildOutput(Option option, out string output) {
            var checkpoint = DataChest.Logger?.OpenCheckpoint(nameof(BuildOutput));
            output = null;
            string extension;
            extension = option.Decrypt ? string.Empty : ".dcf";
            if (string.IsNullOrEmpty(option.Out))
                output = Environment.CurrentDirectory + "/" + Path.GetFileNameWithoutExtension(option.In[0]) + extension;
            else {
                // 확장자가 있는지 없는지 확인한다.
                string filename = Path.GetFileName(option.Out);
                string ext = Path.GetExtension(filename);

                // 파일명이 없는 경우
                if (string.IsNullOrEmpty(filename))
                    filename = Path.GetFileNameWithoutExtension(option.In[0]) + extension;

                // 파일명은 있는데 확장자가 dcf 가 아니거나 없는 경우
                else
                    if (!option.Decrypt && (string.IsNullOrEmpty(ext) || !ext.Equals(".dcf", StringComparison.InvariantCultureIgnoreCase)))
                    filename += extension;

                // 디렉터리 정보를 가져온다.
                string dir = Path.GetDirectoryName(option.Out);

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
                        if (!Directory.Exists(output)) {
                            if (DataChest.Logger == null) return TaskResult.DirectoryNotFound;
                            else return (TaskResult)DataChest.Logger?.Abort(TaskResult.DirectoryNotFound, null);
                        }

                        output += filename;
                    }

                    // 루트 경로의 정보가 있는 경우
                    else {

                        // 디렉터리가 있는지 확인한다.
                        if (!Directory.Exists(dir)) {
                            if (DataChest.Logger == null) return TaskResult.DirectoryNotFound;
                            else return (TaskResult)DataChest.Logger?.Abort(TaskResult.DirectoryNotFound, null);
                        }

                        output = dir + "/" + filename;
                    }
                }
            }

            DataChest.Logger?.CloseCheckpoint(checkpoint, 0);
            return TaskResult.Success;
        }
        /// <summary>
        /// 파일을 삭제합니다. 파일 삭제 도중 발생하는 오류를 무시합니다.<br />
        /// Delete a file. Ignore all exceptions thrown during file deletion.
        /// </summary>
        /// <param name="fileName">
        /// 삭제할 파일입니다.<br />
        /// File to delete.
        /// </param>
        public static void DeleteFileIgnoreErrors(string fileName) {
            try { File.Delete(fileName); } catch { }
        }
    }
}