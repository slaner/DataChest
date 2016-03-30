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
using System.Collections.Generic;

static class ErrorFormatter {
    static readonly Dictionary<int, string> m_errorDict = new Dictionary<int, string>() {
        {0, "작업이 성공했습니다."},
        {1, "파일이 이미 존재합니다."},
        
        {10, "데이터가 없습니다."},
        {11, "초기 벡터(IV)가 없습니다."},
        {12, "입력 파일이 없습니다."},

        {40, "유효하지 않은 알고리즘입니다."},
        {41, "유효하지 않은 시그니쳐입니다."},
        {42, "유효하지 않은 헤더 필드 값입니다."},
        {43, "유효하지 않은 헤더입니다."},
        {44, "유효하지 않은 매개 변수입니다."},
        {45, "유효하지 않은 버전입니다."},
        {46, "유효하지 않은 암호입니다."},
        {47, "유효하지 않은 초기 벡터(IV)입니다."},

        {50, "알고리즘 초기화에 실패했습니다."},
        {51, "입출력 오류입니다."},
        {52, "인코딩 오류입니다."},
        {53, "파일 열기 오류입니다."},
        {54, "스트림 읽기 오류입니다."},
        {55, "스트림 쓰기 오류입니다."},
        {56, "메모리가 부족합니다."},
        {57, "경로가 너무 깁니다."},

        {60, "파일을 찾을 수 없습니다."},
        {61, "디렉터리를 찾을 수 없습니다."},
        {62, "접근이 거부되었습니다."},
        
        {70, "지원되지 않는 버전입니다."},
        {71, "모호한 옵션입니다."},

        {80, "헤더 체크섬이 올바르지 않습니다."},
        {81, "원본 데이터의 체크섬이 올바르지 않습니다."},
        {82, "암호화된 데이터의 체크섬이 올바르지 않습니다."},
    };

    public static string GetErrorMessageFromTaskResult(TaskResult r) {
        int code = (int)r;
        if (m_errorDict.ContainsKey(code))
            return m_errorDict[code];
        else
            return "알려지지 않은 오류입니다.";
    }
}