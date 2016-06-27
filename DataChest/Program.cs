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

namespace DataChest {
    class Program {
        internal static readonly string Modifier = "USER";
        internal static readonly string AdtVersion = "alpha-development";
        internal static readonly string AssemblyVersion;
        static Program() {
            AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        static int Main(string[] args) {
            var option = new Option();
            if (args.Length == 0) {
                Console.WriteLine(option.GetUsage());
                return 0;
            }

            if (!Parser.Default.ParseArguments(args, option)) return (int)TaskResult.InvalidParameter;
            if (option.ShowVersion) return ShowVersionInfo();
            if (option.ShowAlgorithmList) return ShowAlgorithmList();
            
            DataChest dc = new DataChest(option);
            TaskResult result = dc.Process();
            dc.Dispose();
            return (int) result;
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
            Console.WriteLine(SR.GetString("DC_Application_Info"));
            Console.WriteLine("  " + header);
            Console.WriteLine("  " + copyright);
            Console.WriteLine();
            Console.WriteLine(SR.GetString("DC_License_Info"));
            Console.WriteLine("  GPLv2");
            Console.WriteLine();
            Console.WriteLine(SR.GetString("DC_Used_Library"));
            Console.WriteLine("  CommandLine");
            Console.WriteLine("  + Author  : gsscoder");
            Console.WriteLine("  + Homepage: https://commandline.codeplex.com");
            Console.WriteLine("  + License : MIT License");

            return (int)TaskResult.Success;
        }
        static int ShowAlgorithmList() {
            Console.WriteLine(SR.GetString("DC_Available_Algorithm_List"));
            string[] algorithms = Enum.GetNames(typeof(Algorithms));
            for (int i = 0; i < algorithms.Length - 1; i++)
                Console.WriteLine("  " + i + " " + algorithms[i]);

            return (int)TaskResult.Success;
        }
    }
}