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
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace DataChest {
    /// <summary>
    /// DataChest CLI 파싱 클래스입니다.
    /// </summary>
    public sealed class Option {
        
        [Option('a', "algorithm", HelpText = "사용할 알고리즘을 설정합니다.", Required = false, MetaValue = "<alg>")]
        public Algorithms Algorithm { get; set; }

        [Option('A', "listalg", HelpText = "사용할 수 있는 알고리즘 목록을 표시합니다.", Required = false)]
        public bool ShowAlgorithmList { get; set; }
        
        [Option('b', "bufsize", HelpText = "작업에 사용될 버퍼의 크기를 설정합니다. 기본 값은 4096 입니다.", Required = false, MetaValue = "<size>")]
        public int? BufferSize { get; set; }

        [Option('c', "cleanup", HelpText = "작업이 성공하면 입력 파일을 삭제합니다.", Required = false)]
        public bool Cleanup { get; set; }

        [Option('d', "decrypt", HelpText = "암호화된 파일을 복호화합니다.", Required = false)]
        public bool Decrypt { get; set; }

        [Option('D', "disableverify", HelpText = "체크섬 검증을 비활성화합니다.", Required = false)]
        public bool DisableVerification { get; set; }

        [Option('i', "iv", HelpText = "** 초기 벡터(IV)를 설정합니다.", Required = false, MetaValue = "<iv>")]
        public string IV { get; set; }

        [Option('I', "infoheader", HelpText = "* 파일의 CHEST_HEADER 정보를 표시합니다.", Required = false)]
        public bool ShowHeaderInfo { get; set; }

        [Option('m', "comment", HelpText = "파일에 대한 코멘트를 설정합니다.", Required = false, MetaValue = "<text>")]
        public string Comment { get; set; }

        [Option('o', "out", HelpText = "출력 파일의 이름을 설정합니다.", Required = false, MetaValue = "<file>")]
        public string Out { get; set; }

        [Option('v', "headerversion", HelpText = "헤더의 버전을 설정합니다.", Required = false, MetaValue = "<ver>")]
        public ushort HeaderVersion { get; set; }

        [Option('V', "version", HelpText = "버전 및 라이센스 정보를 표시합니다.", Required = false)]
        public bool ShowVersion { get; set; }

        [Option('t', "test", HelpText = "파일에 대한 기능 시험을 수행합니다.", Required = false)]
        public bool RunTest { get; set; }
        
        [Option('w', "overwrite", HelpText = "출력 파일을 덮어쓰도록 설정합니다.", Required = false)]
        public bool Overwrite { get; set; }

        [Option('p', "password", HelpText = "** 암호를 설정합니다.", Required = false, MetaValue = "<pass>")]
        public string Password { get; set; }

        [ValueList(typeof(List<string>))]
        public List<string> In { get; set; }

        [HelpOption('?', HelpText = "도움말을 표시합니다.")]
        public string GetUsage() {
            #region Displaying help message via HelpText object(automatically generated)
            HelpText help = new HelpText();
            help.AdditionalNewLineAfterOption = false;
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

            help.Heading = header;
            help.Copyright = copyright;
            help.AddDashesToOption = true;
            // License
            help.AddPreOptionsLine("사용법: dc [옵션] <파일>");
            help.AddOptions(this);
            help.AddPostOptionsLine("*\t-V, --apiversion 옵션을 제외한 모든 옵션은 무시됩니다.");
            help.AddPostOptionsLine("**\t기본적으로 파일을 먼저 찾고, 파일이 존재하지 않는 경우 문자열 자체를 암호로 사용합니다.");
            help.AddPostOptionsLine("\t단, " + DataChest.CipherPlainText + " 또는 " + DataChest.CipherFromFile + "로 시작하는 경우 해당 규칙에 맞게 설정합니다.");
            help.AddPostOptionsLine("\t" + DataChest.CipherPlainText + " 는 문자열을 암호로 사용하고, " + DataChest.CipherFromFile + " 는 파일을 찾습니다.");
            help.AddPostOptionsLine("\t" + DataChest.CipherFromFile + " 를 사용할 때 파일이 없으면 오류가 발생합니다.");
            help.AddPostOptionsLine("\t예)");
            help.AddPostOptionsLine("\t" + DataChest.CipherPlainText + "PassPhrase = PassPhrase 라는 문자열을 암호로 사용");
            help.AddPostOptionsLine("\t" + DataChest.CipherFromFile + "C:\\phrase.bin = C:\\phrase.bin 이라는 파일의 내용을 암호로 사용" + Environment.NewLine);
            return help;
            #endregion
            #region via StringBuilder, hand work(manual)
            /*
            StringBuilder sbUsage = new StringBuilder();
    #region Version & Information Displaying
            sbUsage.AppendFormat("Data Chest  v{0}", Program.AssemblyVersion);
    #endregion

    #region Default Usage
            sbUsage.AppendLine("사용법:");
            sbUsage.AppendLine("    dc [옵션] <파일>\n");
    #endregion

    #region Available Options
            sbUsage.AppendLine("사용 가능한 옵션:");
            sbUsage.AppendLine(FormatOptions('a', "algorithm <alg>", "사용할 알고리즘을 설정합니다."));
            sbUsage.AppendLine(FormatOptions('A', "listalg", "사용 가능한 알고리즘 목록울 표시합니다."));
            sbUsage.AppendLine(FormatOptions('c', "cleanup", "작업이 성공하면 입력 파일을 삭제합니다."));
            sbUsage.AppendLine(FormatOptions('d', "decrypt", "암호화된 파일을 복호화합니다."));
            sbUsage.AppendLine(FormatOptions('D', "disableverify", "체크섬 검증을 비활성화합니다."));
            sbUsage.AppendLine(FormatOptions('e', "encrypt", "파일을 암호화합니다. 암호화된 파일에 이 옵션을 사용하면 이중으로 암호화됩니다."));
            sbUsage.AppendLine(FormatOptions('i', "infoheader", "* 암호화된 파일에 대한 CHEST_HEADER 정보를 표시합니다."));
            sbUsage.AppendLine(FormatOptions('I', "iv", "** 초기 벡터(IV)를 설정합니다."));
            sbUsage.AppendLine(FormatOptions('o', "out", "출력 파일의 이름을 설정합니다. 설정되지 않은 경우 입력 파일의 이름과 같게 설정됩니다."));
            sbUsage.AppendLine(FormatOptions('p', "password <password>", "** 암호를 설정합니다."));
            sbUsage.AppendLine(FormatOptions('t', "test", "파일에 대한 암호화 또는 복호화 기능을 시험합니다."));
            sbUsage.AppendLine(FormatOptions('v', "version", "버전 정보를 표시합니다."));
            sbUsage.AppendLine(FormatOptions('V', "apiversion <version>", "ChestAPI 버전을 설정합니다. 현재 버전에 대한 기본 값은 " + 1 + " 입니다."));
            sbUsage.AppendLine(FormatOptions('w', "overwrite", "출력 파일을 덮어쓰도록 설정합니다."));
            sbUsage.AppendLine(FormatOptions('?', "help", "도움말을 표시합니다."));
    #endregion

    #region Remarks
            sbUsage.AppendLine();
            sbUsage.AppendLine("*\t-V, --apiversion 옵션을 제외한 모든 옵션은 무시됩니다.");
            sbUsage.AppendLine("**\t기본적으로 파일을 먼저 찾고, 파일이 존재하지 않는 경우 문자열 자체를 암호로 사용합니다.");
            sbUsage.AppendLine("\t단, K: 또는 F:로 시작하는 경우 해당 규칙에 맞게 설정합니다.");
            sbUsage.AppendLine("\tK: 는 문자열을 암호로 사용하고, F: 는 파일을 찾습니다.");
            sbUsage.AppendLine("\tF: 를 사용할 때 파일이 없으면 오류가 발생합니다.");
            sbUsage.AppendLine("\t예)");
            sbUsage.AppendLine("\tK:PassPhrase = PassPhrase 라는 문자열을 암호로 사용");
            sbUsage.AppendLine("\tF:C:\\phrase.bin = C:\\phrase.bin 이라는 파일의 내용을 암호로 사용");
    #endregion


            return sbUsage.ToString();
            */
            #endregion
        }

        static string FormatOptions(char shortName, string description) {
            return string.Format("    -{0,-26} {1}", shortName, description);
        }
        static string FormatOptions(string longName, string description) {
            return string.Format("    -{0,-26} {1}", "-" + longName, description);
        }
        static string FormatOptions(char shortName, string longName, string description) {
            return string.Format("    -{0,-26} {1}", shortName + ", --" + longName, description);
        }
    }
}