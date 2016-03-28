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
}