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
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace DataChest {
    /// <summary>
    /// DataChest CLI 파싱 클래스입니다.<br />
    /// A DataChest CLI parsing class.
    /// </summary>
    public sealed class Option {

        [Option('a', "algorithm", HelpText = "DC_Option_Algorithm", Required = false, MetaValue = "<alg>")]
        public Algorithms Algorithm { get; set; }

        [Option('A', "listalg", HelpText = "DC_Option_ShowAlgorithmList", Required = false)]
        public bool ShowAlgorithmList { get; set; }

        [Option('b', "bufsize", HelpText = "DC_Option_BufferSize", Required = false, MetaValue = "<size>")]
        public int? BufferSize { get; set; }

        [Option('c', "cleanup", HelpText = "DC_Option_Cleanup", Required = false)]
        public bool Cleanup { get; set; }

        [Option('d', "decrypt", HelpText = "DC_Option_Decrypt", Required = false)]
        public bool Decrypt { get; set; }

        [Option('D', "disableverify", HelpText = "DC_Option_DisableVerification", Required = false)]
        public bool DisableVerification { get; set; }

        [Option('h', "headerversion", HelpText = "DC_Option_HeaderVersion", Required = false, MetaValue = "<ver>")]
        public ushort HeaderVersion { get; set; }

        [Option('i', "iv", HelpText = "DC_Option_IV", Required = false, MetaValue = "<iv>")]
        public string IV { get; set; }

        [Option('I', "infoheader", HelpText = "DC_Option_ShowHeaderInfo", Required = false)]
        public bool ShowHeaderInfo { get; set; }

        [Option('m', "comment", HelpText = "DC_Option_Comment", Required = false, MetaValue = "<text>")]
        public string Comment { get; set; }

        [Option('o', "out", HelpText = "DC_Option_Out", Required = false, MetaValue = "<file>")]
        public string Out { get; set; }

        [Option('v', "verbose", HelpText = "DC_Option_Verbose", Required = false)]
        public bool Verbose { get; set; }

        [Option('V', "version", HelpText = "DC_Option_ShowVersion", Required = false)]
        public bool ShowVersion { get; set; }

        [Option('t', "test", HelpText = "DC_Option_RunTest", Required = false)]
        public bool RunTest { get; set; }
        
        [Option('w', "overwrite", HelpText = "DC_Option_Overwrite", Required = false)]
        public bool Overwrite { get; set; }

        [Option('p', "password", HelpText = "DC_Option_Password", Required = false, MetaValue = "<pass>")]
        public string Password { get; set; }

        [ValueList(typeof(List<string>))]
        public List<string> In { get; set; }

        [HelpOption('?', HelpText = "DC_Option_Usage")]
        public string GetUsage() {
            HelpText help = new HelpText();
            help.AdditionalNewLineAfterOption = false;
            HeadingInfo header = new HeadingInfo(
            "DataChest",
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
            help.AdditionalNewLineAfterOption = true;
            help.AddDashesToOption = true;
            help.AddPreOptionsLine(string.Format("{0}: dc [{1}] <{2}>", SR.GetString("DC_Usage"), SR.GetString("DC_Option"), SR.GetString("DC_File")));
            help.AddOptions(this);
            help.AddPostOptionsLine(string.Format(SR.GetString("DC_Keys_Description"), DataChest.CipherPlainText, DataChest.CipherFromFile));
            return help;
        }
    }
}