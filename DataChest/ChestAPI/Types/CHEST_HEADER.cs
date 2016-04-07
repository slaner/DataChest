﻿/*
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

using System.IO;

/// <summary>
/// CHEST_HEADER version 1. (36 Bytes)
/// </summary>
struct CHEST_HEADER {
    /// <summary>
    /// Size of CHEST_HEADER structure in bytes.
    /// </summary>
    public const int HEADER_SIZE = 36;
    /*
       offset   0   1   2   3   4   5   6   7   8   9   A   B   C   D   E   F
                +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
                |  sig  |  ver  |     unused    | e_checksum    | r_checksum    |
                +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
                | h_checksum    | e_size                        | r_size >>>>>> |
                +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
                | >>>>>>>>>>>>> |
                +---+---+---+---+
                
    */

    public CHEST_HEADER(ushort headerVersion) {
        signature = 22344;
        version = headerVersion;
        unused = 0;
        e_checksum = 0;
        r_checksum = 0;
        h_checksum = 0;
        e_size = 0;
        r_size = 0;
    }

    /// <summary>
    /// Signature of CHEST_HEADER structure. It must be "HW" (0x5748)
    /// </summary>
    public ushort signature;

    /// <summary>
    /// Version of CHEST_HEADER structure. At 2016-01-04, version is '1'.
    /// </summary>
    public ushort version;

    // 2016 03 05 REMOVED
    // SECURITY VULNERABILITY DETECTED. 
    // /// <summary>
    // /// Method of cryption which used.
    // /// </summary>
    // public Algorithms alg;

    /// <summary>
    /// Reserved.
    /// </summary>
    public uint unused;

    /// <summary>
    /// Checksum of encrypted data.
    /// </summary>
    public uint e_checksum;

    /// <summary>
    /// Checksum of raw data.
    /// </summary>
    public uint r_checksum;

    /// <summary>
    /// Checksum of header.
    /// </summary>
    public uint h_checksum;

    /// <summary>
    /// Size of encrypted data.
    /// </summary>
    public ulong e_size;

    /// <summary>
    /// Size of raw data.
    /// </summary>
    public ulong r_size;


    public static CHEST_HEADER CreateHeader(ushort version, FileStream fs, byte[] b) {
        CHEST_HEADER hdr = new CHEST_HEADER(version);
        hdr.r_size = (ulong) fs.Length;
        hdr.e_size = (ulong) b.LongLength;

        fs.Seek(0, SeekOrigin.Begin);
        hdr.r_checksum = HashAPI.ComputeHashUInt32(fs);
        hdr.e_checksum = HashAPI.ComputeHashUInt32(b);
        byte[] temp = hdr.ToArray();
        hdr.h_checksum = HashAPI.ComputeHashUInt32(temp);

        return hdr;
    }
    public static TaskResult FromFile(string file, out CHEST_HEADER hdr) {
        hdr = default(CHEST_HEADER);
        FileStream fs;
        TaskResult r = FileHelper.OpenFileStream(file, out fs);
        if (r != TaskResult.Success) return r;
        return FromStream(fs, out hdr);
    }
    public static TaskResult FromStream(Stream s, out CHEST_HEADER hdr) {
        hdr = default(CHEST_HEADER);
        BinaryReader br;
        try {
            br = new BinaryReader(s);
        } catch {
            return TaskResult.InvalidParameter;
        }

        hdr.signature = br.ReadUInt16();
        if (hdr.signature != 22344)
            return TaskResult.InvalidSignature;

        hdr.version = br.ReadUInt16();
        if (hdr.version > ChestAPI.Version)
            return TaskResult.NotSupportedVersion;

        hdr.unused = br.ReadUInt32();
        if (hdr.unused != 0)
            return TaskResult.InvalidHeaderFieldValue;

        hdr.e_checksum = br.ReadUInt32();
        hdr.r_checksum = br.ReadUInt32();
        uint chkHeader = br.ReadUInt32();
        hdr.e_size = br.ReadUInt64();
        hdr.r_size = br.ReadUInt64();

        byte[] hb = hdr.ToArray();
        if (HashAPI.ComputeHashUInt32(hb) != chkHeader)
            return TaskResult.IncorrectHeaderChecksum;

        hdr.h_checksum = chkHeader;
        return TaskResult.Success;
    }

    public byte[] ToArray() {
        MemoryStream ms = new MemoryStream(HEADER_SIZE);
        using (BinaryWriter bw = new BinaryWriter(ms)) {
            bw.Write(signature);
            bw.Write(version);
            bw.Write(unused);
            bw.Write(e_checksum);
            bw.Write(r_checksum);
            bw.Write(h_checksum);
            bw.Write(e_size);
            bw.Write(r_size);
        }

        byte[] arr = ms.ToArray();
        ms.Dispose();
        return arr;
    }
}
