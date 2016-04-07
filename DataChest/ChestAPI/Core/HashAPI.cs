﻿using System;
using System.IO;
using System.Security.Cryptography;

/// <summary>
/// 해시를 계산하는 작업을 노출하는 클래스입니다.
/// </summary>
static class HashAPI {
    static readonly SHA256 SHA = SHA256.Create();
    
    /// <summary>
    /// <see cref="byte"/> 배열에 대한 해시를 계산합니다.
    /// </summary>
    /// <param name="b">해시를 계산할 <see cref="byte" /> 배열입니다.</param>
    public static byte[] ComputeHash(byte[] b) {
        return SHA.ComputeHash(b);
    }
    /// <summary>
    /// <see cref="Stream"/> 개체에 대한 해시를 계산합니다.
    /// </summary>
    /// <param name="s">해시를 계산할 <see cref="Stream" /> 개체입니다.</param>
    public static byte[] ComputeHash(Stream s) {
        return SHA.ComputeHash(s);
    }
    /// <summary>
    /// <see cref="byte"/> 배열에 대한 해시를 계산하고 결과값을 부호 없는 4바이트 정수로 변환합니다.
    /// </summary>
    /// <param name="b">해시를 계산할 <see cref="byte" /> 배열입니다.</param>
    public static uint ComputeHashUInt32(byte[] b) {
        return BitConverter.ToUInt32(ComputeHash(b), 0);
    }
    /// <summary>
    /// <see cref="Stream"/> 개체에 대한 해시를 계산하고 결과값을 부호 없는 4바이트 정수로 변환합니다.
    /// </summary>
    /// <param name="s">해시를 계산할 <see cref="Stream"/> 개체입니다.</param>
    public static uint ComputeHashUInt32(Stream s) {
        return BitConverter.ToUInt32(ComputeHash(s), 0);
    }
}