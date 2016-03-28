// 추가 버전을 공지하려면 아래 주석을 제거하세요
// 연관 변수: Program.AdtVersion
// #define ADT_VERSION_NOTIFY

// 원본 프로젝트를 수정하여 컴파일하는 경우 아래 주석을 제거하세요
// 연관 변수: Program.Modifier
// #define PROJECT_MODIFIED

using System;
using System.Reflection;
using CommandLine;

class Program {
    internal static readonly string Modifier = "USER";
    internal static readonly string AdtVersion = "development";
    internal static readonly string AssemblyVersion;
    static Program() {
        AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }


    static int Main(string[] args) {
        var opt = new ChestAPIOptions();
        if (args.Length == 0) {
            Console.Write(opt.GetUsage());
            return (int) TaskResult.Success;
        } else {
            // 실패하거나 도움말을 표시하는 경우, 기본 종료 코드로 종료한다.
            if (!Parser.Default.ParseArguments(args, opt))
                return (int) TaskResult.InvalidParameter;

            // 알고리즘 목록 표시
            if (opt.ShowAlgorithmList)
                return ShowAlgorithmList();
                
            // 버전 정보 표시
            if (opt.ShowVersionInfo)
                return ShowVersionInfo();

            // 독립적인 명령어 끝
            // 입력 파일이 없는 경우
            if (opt.In.Count == 0)
                return SetError(TaskResult.NoInputFiles, "입력 파일이 없습니다.");

            // 헤더 정보 표시
            if(opt.ShowHeaderInfo) {
                CHEST_HEADER hdr;
                TaskResult r = CHEST_HEADER.FromFile(opt.In[0], out hdr);
                if ( r!= TaskResult.Success) 
                    return SetError(r, "헤더 정보를 가져올 수 없습니다.");
                
                Console.WriteLine("CHEST_HEADER information");
                Console.WriteLine("File                   : " + opt.In[0]);
                Console.WriteLine("Version                : 0x{0:x4} ({0})", hdr.version);
                Console.WriteLine("Encrypted Data Checksum: 0x{0:x8} ({0})", hdr.e_checksum);
                Console.WriteLine("Raw Data Checksum      : 0x{0:x8} ({0})", hdr.r_checksum);
                Console.WriteLine("Header Checksum        : 0x{0:x8} ({0})", hdr.h_checksum);
                Console.WriteLine("Encrypted Data Size    : 0x{0:x16} ({0})", hdr.e_size);
                Console.WriteLine("Raw Data Size          : 0x{0:x16} ({0})", hdr.r_size);
                return (int) TaskResult.Success;
            }

            //how to get unbound options ?;
            // Successful
            opt.ToString();
        }

        return (int)TaskResult.Success;
    }
        
    static int ShowVersionInfo() {
        Console.WriteLine("CommandLine - MIT License");
        return (int) TaskResult.Success;
    }

    static int ShowAlgorithmList() {
        Console.WriteLine("사용 가능한 알고리즘 목록:");
        string[] algorithms = Enum.GetNames(typeof(Algorithms));
        for (int i = 0; i < algorithms.Length - 1; i++)
            Console.WriteLine("  " + algorithms[i]);

        return (int)TaskResult.Success;
    }
    
    static int SetError(TaskResult result, string message) {
        Console.WriteLine("오류 코드: " + result + " (0x{0:X4})", (int) result);
        Console.WriteLine("오류 내용: " + message);
        return (int) result;
    }
}