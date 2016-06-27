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
using System.Diagnostics;
using System.IO;

namespace DataChest {
    /// <summary>
    /// 성능 기록기입니다.<br />
    /// A performance logger.
    /// </summary>
    sealed class PerformanceLogger : IDisposable {
        /// <summary>
        /// 성능 기록에 사용되는 체크포인트입니다.<br />
        /// Checkpoint used by performance logging.
        /// </summary>
        public sealed class Checkpoint {
            string g_name;
            TimeSpan g_time;
            TimeSpan g_elapsed;

            /// <summary>
            /// 이름과 시간을 이용해 체크포인트를 만듭니다.<br />
            /// Create checkpoint using name and time.
            /// </summary>
            /// <param name="name">
            /// 체크포인트의 이름입니다.<br />
            /// Name of checkpoint.
            /// </param>
            /// <param name="time">
            /// 체크포인트 도달 시간입니다.<br />
            /// Reached time of checkpoint.
            /// </param>
            public Checkpoint(string name, TimeSpan time) {
                g_name = name;
                g_time = time;
            }

            /// <summary>
            /// 체크포인트 이름을 가져옵니다.<br />
            /// Get a name of checkpoint.
            /// </summary>
            public string Name {
                get { return g_name; }
            }
            /// <summary>
            /// 체크포인트 도달 시간을 가져옵니다.<br />
            /// Get a reached time of checkpoint.
            /// </summary>
            public TimeSpan Time {
                get { return g_time; }
            }
            /// <summary>
            /// 체크포인트가 열린 후부터 진행된 시간을 가져옵니다.<br />
            /// Get a elapsed time of checkpoint after opened.
            /// </summary>
            public TimeSpan ElapsedTime {
                get { return g_elapsed; }
            }

            /// <summary>
            /// 체크포인트를 마칩니다.<br />
            /// Finish a checkpoint.
            /// </summary>
            /// <param name="endTime">
            /// 체크포인트 종료 시간입니다.<br />
            /// Time of checkpoint ended.
            /// </param>
            public void Finish(TimeSpan endTime) {
                g_elapsed = endTime - g_time;
            }

            /// <summary>
            /// 체크포인트를 문자열로 나타냅니다.<br />
            /// Represents the checkpoint as a string.
            /// </summary>
            public override string ToString() {
                return string.Format("{0} - {1}", Name, Time);
            }
        }

        readonly Stopwatch m_sw;
        readonly bool g_enabled;
        readonly StreamWriter m_writer;
        int m_checkpointLevels = 0;
        
        public PerformanceLogger(DataChest dc) {
            g_enabled = dc.Option.Verbose;
            if (g_enabled) {
                m_sw = new Stopwatch();
                m_writer = new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding);
                m_writer.AutoFlush = true;
            }
        }
        
        /// <summary>
        /// 성능 기록을 시작합니다.<br />
        /// Start a performance logging.
        /// </summary>
        /// <param name="s">
        /// 성능 기록에 참조할 <see cref="Stream"/> 입니다.<br />
        /// <see cref="Stream"/> to reference performance logging.
        /// </param>
        public void Start() {
            if (!Enabled) return;
            
            m_writer.WriteLine(
                string.Format(
                    SR.GetString("DC_PerformanceLogging_Start"),
                    m_sw.Elapsed.ToString()));
            m_sw.Start();
        }

        /// <summary>
        /// 체크포인트를 엽니다.<br />
        /// Open a checkpoint.
        /// </summary>
        /// <param name="name">
        /// 체크포인트의 이름입니다.<br />
        /// Name of checkpoint.
        /// </param>
        public Checkpoint OpenCheckpoint(string name) {
            if (!Enabled) return null;
            if (!m_sw.IsRunning) return null;

            m_writer.Write(new string(' ', m_checkpointLevels * 2));
            m_writer.WriteLine(
                string.Format(SR.GetString("DC_PerformanceLogging_Checkpoint_Created"),
                m_sw.Elapsed,
                name));
            m_checkpointLevels++;
            return new Checkpoint(name, m_sw.Elapsed);
        }
        /// <summary>
        /// 체크포인트를 닫습니다.<br />
        /// Close a checkpoint.
        /// </summary>
        /// <param name="checkpoint">
        /// <see cref="OpenCheckpoint(string)"/> 메서드에 의해 열린 체크포인트입니다.<br />
        /// A checkpoint that is opened by <see cref="OpenCheckpoint(string)"/> method.
        /// </param>
        /// <param name="size">
        /// 처리된 데이터의 크기입니다.<br />
        /// Size of data handled.
        /// </param>
        public void CloseCheckpoint(Checkpoint checkpoint, long size) {
            if (!Enabled) return;
            if (!m_sw.IsRunning) return;
            if (checkpoint == null) return;
            if (m_checkpointLevels <= 0) return;

            checkpoint.Finish(m_sw.Elapsed);
            m_sw.Stop();
            string s;
            if (size == 0) s = string.Format("<{0}>", SR.GetString("DC_PerformanceLogging_None"));
            else s = UnitHelper.ComputeSpeed(size, checkpoint.ElapsedTime);
            m_writer.Write(new string(' ', (m_checkpointLevels - 1) * 2));
            m_writer.WriteLine(
                string.Format(
                    SR.GetString("DC_PerformanceLogging_Checkpoint_Finished"),
                    m_sw.Elapsed,
                    checkpoint.Name,
                    SR.GetString("DC_PerformanceLogging_Speed"),
                    s,
                    SR.GetString("DC_PerformanceLogging_ElapsedTime"),
                    checkpoint.ElapsedTime));
            m_checkpointLevels--;
            m_sw.Start();

        }

        /// <summary>
        /// 지정된 문자열을 성능 기록기에 줄 바꿈 문자와 같이 씁니다.<br />
        /// Write given string into performance logger with new-line character.
        /// </summary>
        /// <param name="s">
        /// 기록할 문자열입니다.<br />
        /// A string to be written.
        /// </param>
        public void WriteLine(string s) {
            if (!Enabled) return;
            if (!m_sw.IsRunning) return;

            m_sw.Stop();
            m_writer.WriteLine(
                string.Format(
                    SR.GetString("DC_PerformanceLogging_WriteLine"),
                    m_sw.Elapsed,
                    s));
        }
        /// <summary>
        /// 지정된 문자열에 있는 서식 문자열을 형식화한 후 성능 기록기에 줄 바꿈 문자와 같이 씁니다.<br />
        /// Formalize format string in given string and write into performance logger with new-line character.
        /// </summary>
        /// <param name="s">
        /// 서식을 포함한 문자열입니다.<br />
        /// A string that contains format.
        /// </param>
        /// <param name="args">
        /// 치환될 값을 담고 있는 배열입니다.<br />
        /// An array that contains a value to be replaced(=formatted).
        /// </param>
        public void WriteLine(string s, params object[] args) {
            WriteLine(string.Format(s, args));
        }

        /// <summary>
        /// 성능 기록을 중단합니다.<br />
        /// Abort a performance logging.
        /// </summary>
        /// <param name="r">
        /// 작업 결과입니다.<br />
        /// Task result.
        /// </param>
        public TaskResult Abort(TaskResult r, Exception e) {
            if (!Enabled) return r;
            if (!m_sw.IsRunning) return r;

            m_sw.Stop();
            m_writer.WriteLine(
                string.Format(
                    SR.GetString("DC_PerformanceLogging_Aborted"),
                    m_sw.Elapsed,
                    SR.GetString("DC_PerformanceLogging_Exception"),
                    e.ToString() ?? string.Format("<{0}>", SR.GetString("DC_PerformanceLogging_None")),
                    SR.GetString("DC_PerformanceLogging_Error"),
                    r.ToString()));
            return r;
        }
        /// <summary>
        /// 성능 기록을 마칩니다.
        /// End a performance logging.
        /// </summary>
        public void End() {
            if (!Enabled) return;
            m_sw.Stop();

            m_writer.WriteLine(SR.GetString("DC_PerformanceLogging_Ended"));
        }

        /// <summary>
        /// 성능 기록기 사용 가능 여부를 가져옵니다.<br />
        /// Get an availability of performance logger.
        /// </summary>
        public bool Enabled {
            get { return g_enabled; }
        }

        public void Dispose() {
            if (!Enabled) return;

            m_writer.Dispose();
        }
    }
}