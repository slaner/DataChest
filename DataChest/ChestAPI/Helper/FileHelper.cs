using System;
using System.IO;

static class FileHelper {
    public static TaskResult OpenFileStream(string fileName, out FileStream fs) {
        fs = null;
        try {
            fs = File.OpenRead(fileName);
        } catch (FileNotFoundException fnfe) {
            return TaskResult.FileNotFound;
        } catch (UnauthorizedAccessException uae) {
            return TaskResult.AccessDenied;
        } catch (DirectoryNotFoundException dnfe) {
            return TaskResult.DirectoryNotFound;
        } catch (PathTooLongException ptle) {
            return TaskResult.PathTooLong;
        } catch (ArgumentException ae) {
            return TaskResult.InvalidParameter;
        } catch {
            return TaskResult.IOError;
        }
        return TaskResult.Success;
    }
    public static TaskResult GetOutputPath(ChestParams cp, out string output, bool encrypt) {
        output = null;
        string extension;
        extension = encrypt ? ".dcf" : "";
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
                if (encrypt && (string.IsNullOrEmpty(ext) || !ext.Equals(".dcf", StringComparison.InvariantCultureIgnoreCase)))
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
}