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

            // API 버전이 설정되지 않은 경우 ChestAPI.Version 으로 설정한다.
            if (opt.APIVersion == 0)
                opt.APIVersion = ChestAPI.Version;
            else {
                // 지원하는 버전보다 높은 버전일 경우
                if (opt.APIVersion > ChestAPI.Version)
                    return SetError(TaskResult.NotSupportedVersion);
            }

            // 헤더 정보 표시
            if (opt.ShowHeaderInfo) {
                HeaderBase hdr;
                r = HeaderBase.FromFileWithVersion(opt.In[0], opt.APIVersion, out hdr);
                if (r != TaskResult.Success)
                    return SetError(r);
                
                Console.WriteLine("CHEST_HEADER information");
                Console.WriteLine("File              : " + opt.In[0]);
                Console.WriteLine("Version           : 0x{0:x4} ({0})", hdr.Version);
                Console.WriteLine("Header Checksum   : 0x{0:x8} ({0})", hdr.HChecksum);
                Console.WriteLine("Enc.Data Checksum : 0x{0:x8} ({0})", hdr.EChecksum);
                Console.WriteLine("Raw Data Checksum : 0x{0:x8} ({0})", hdr.RChecksum);
                Console.WriteLine("Header Size       : 0x{0:x4} ({0})", hdr.HSize);
                Console.WriteLine("Enc.Data Size     : 0x{0:x16} ({0})", hdr.ESize);
                Console.WriteLine("Raw Data Size     : 0x{0:x16} ({0})", hdr.RSize);

                if ( opt.APIVersion == 2)
                    Console.WriteLine("Comment           : {0}", ((ChestHeader2)hdr).Comment);

                return (int) TaskResult.Success;
            }

            // 알고리즘 검사
            if (opt.Algorithm >= Algorithms.LastMethod)
                return SetError(TaskResult.InvalidAlgorithm);

            // 테스트 옵션이 켜진 경우
            if (opt.RunTest) {
                // 정리 옵션도 켜진 경우
                // 두 옵션은 같이 사용될 수 없다.
                if (opt.Cleanup)
                    return SetError(TaskResult.AmbiguousOption, "-t 옵션과 -c 옵션은 같이 사용될 수 없습니다.");
            }

            // 버퍼가 설정된 경우
            if ( opt.BufferSize.HasValue ) {
                if (opt.BufferSize.Value > 0)
                    ChestAPI.BufferSize = opt.BufferSize.Value;
                else
                    return SetError(TaskResult.InvalidBufferSize);
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
            cp.Comment = opt.Comment;

            r = ChestAPI.Invoke(cp);
            return SetError(r);
        }
    }
        
    static int ShowVersionInfo() {
        HeadingInfo header = new HeadingInfo(
            "Data Chest",
            AssemblyVersion
#if ADT_VERSION_NOTIFY
                + "-" + AdtVersion
#endif
            );
        CopyrightInfo copyright = new CopyrightInfo(
            "HYE WON, HWANG"
#if PROJECT_MODIFIED
                + " and Modified by " + Modifier
#endif
                + ".", 2016
        );
        Console.WriteLine("프로그램 정보:");
        Console.WriteLine("  " + header);
        Console.WriteLine("  " + copyright);
        Console.WriteLine("  API Version: " + ChestAPI.Version);
        Console.WriteLine();
        Console.WriteLine("라이센스 정보:");
        Console.WriteLine("  GPLv2");
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