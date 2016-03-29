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

// 추가 버전을 공지하려면 아래 주석을 제거하세요
// 연관 변수: Program.AdtVersion
#define ADT_VERSION_NOTIFY

// 원본 프로젝트를 수정하여 컴파일하는 경우 아래 주석을 제거하세요
// 연관 변수: Program.Modifier
// #define PROJECT_MODIFIED

using System;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

class Program {
    internal static readonly string Modifier = "USER";
    internal static readonly string AdtVersion = "alpha-development";
    internal static readonly string AssemblyVersion;
    static Program() {
        AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }


    static int Main(string[] args) {
        var opt = new ChestAPIOptions();
        TaskResult r;
        if (args.Length == 0) {
            Console.Write(opt.GetUsage());
            return (int) TaskResult.Success;
        } else {
            // 실패하거나 도움말을 표시하는 경우, 기본 종료 코드로 종료한다.
            if (!Parser.Default.ParseArguments(args, opt))
                return SetError(TaskResult.InvalidParameter);

            // 알고리즘 목록 표시
            if (opt.ShowAlgorithmList)
                return ShowAlgorithmList();
                
            // 버전 정보 표시
            if (opt.ShowVersionInfo)
                return ShowVersionInfo();

            // 독립적인 명령어 끝
            // 입력 파일이 없는 경우
            if (opt.In.Count == 0)
                return SetError(TaskResult.NoInputFiles);

            // 헤더 정보 표시
            if(opt.ShowHeaderInfo) {
                CHEST_HEADER hdr;
                r = CHEST_HEADER.FromFile(opt.In[0], out hdr);
                if (r != TaskResult.Success)
                    return SetError(r);
                
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

            // 알고리즘 검사
            if (opt.Algorithm >= Algorithms.LastMethod)
                return SetError(TaskResult.InvalidAlgorithm);

            // API 버전이 설정되지 않은 경우 ChestAPI.Version 으로 설정한다.
            if (opt.APIVersion == 0)
                opt.APIVersion = ChestAPI.Version;
            else {
                // 지원하는 버전보다 높은 버전일 경우
                if (opt.APIVersion > ChestAPI.Version)
                    return SetError(TaskResult.NotSupportedVersion);
            }

            // 테스트 옵션이 켜진 경우
            if (opt.RunTest) {
                // 정리 옵션도 켜진 경우
                // 두 옵션은 같이 사용될 수 없다.
                if (opt.Cleanup)
                    return SetError(TaskResult.AmbiguousOption, "-t 옵션과 -c 옵션은 같이 사용될 수 없습니다.");
            }

            ChestParams cp = new ChestParams();
            cp.Cleanup = opt.Cleanup;
            cp.Overwrite = opt.Overwrite;
            cp.Algorithm = opt.Algorithm;
            cp.Encrypt = !opt.Decrypt;
            cp.Verify = !opt.DisableVerification;
            cp.OutputFile = opt.Out;
            cp.RunTest = opt.RunTest;
            cp.Version = opt.APIVersion;
            cp.SetIV(opt.IV);
            cp.SetPassword(opt.Password);
            cp.InputFile = opt.In[0];

            r = ChestAPI.Invoke(cp);
            return SetError(r);
        }
    }
        
    static int ShowVersionInfo() {
        HeadingInfo header = new HeadingInfo(
            "Data Chest",
            Program.AssemblyVersion
#if ADT_VERSION_NOTIFY
                + "-" + Program.AdtVersion
#endif
            );
        CopyrightInfo copyright = new CopyrightInfo(
            "HYE WON, HWANG"
#if PROJECT_MODIFIED
                + " and Modified by " + Program.Modifier
#endif
                + ".", 2016
        );
        Console.WriteLine("프로그램 정보:");
        Console.WriteLine("  " + header);
        Console.WriteLine("  " + copyright);
        Console.WriteLine();
        Console.WriteLine("라이센스 정보:");
        Console.WriteLine("  MIT License");
        Console.WriteLine();
        Console.WriteLine("사용 라이브러리:");
        Console.WriteLine("  CommandLine");
        Console.WriteLine("  + Author  : gsscoder");
        Console.WriteLine("  + Homepage: https://commandline.codeplex.com");
        Console.WriteLine("  + License : MIT License");

        return (int) TaskResult.Success;
    }

    static int ShowAlgorithmList() {
        Console.WriteLine("사용 가능한 알고리즘 목록:");
        string[] algorithms = Enum.GetNames(typeof(Algorithms));
        for (int i = 0; i < algorithms.Length - 1; i++)
            Console.WriteLine("  " + i + " " + algorithms[i]);

        return (int)TaskResult.Success;
    }

    static int SetError(TaskResult result) {
        return SetError(result, ErrorFormatter.GetErrorMessageFromTaskResult(result));
    }
    static int SetError(TaskResult result, string message) {
        Console.WriteLine("오류 코드: " + result + " (0x{0:X4})", (int) result);
        Console.WriteLine("오류 내용: " + message);
        return (int) result;
    }
}