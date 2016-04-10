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

using System;

/// <summary>
/// ChestAPI 헤더를 구현합니다.
/// </summary>
interface IHeader {
    /// <summary>
    /// 시그니쳐를 가져옵니다.
    /// </summary>
    ushort Signature { get; }

    /// <summary>
    /// 버전을 가져옵니다.
    /// </summary>
    ushort Version { get; }
    
    /// <summary>
    /// 사용되지 않습니다.
    /// </summary>
    [Obsolete()]
    uint Unused { get; }

    /// <summary>
    /// 헤더의 체크섬을 가져옵니다.
    /// </summary>
    uint HChecksum { get; }

    /// <summary>
    /// 암호화된 데이터의 체크섬을 가져옵니다.
    /// </summary>
    uint EChecksum { get; }

    /// <summary>
    /// 원본 데이터의 체크섬을 가져옵니다.
    /// </summary>
    uint RChecksum { get; }

    /// <summary>
    /// 헤더의 크기를 가져옵니다.
    /// </summary>
    ushort HSize { get; }

    /// <summary>
    /// 암호화된 데이터의 크기를 가져옵니다.
    /// </summary>
    ulong ESize { get; }

    /// <summary>
    /// 원본 데이터의 크기를 가져옵니다.
    /// </summary>
    ulong RSize { get; }
}