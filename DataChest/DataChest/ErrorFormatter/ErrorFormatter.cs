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

using System.Collections.Generic;

namespace DataChest {
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
        {48, "유효하지 않은 버퍼 크기입니다."},
        {49, "유효하지 않은 헤더 클래스입니다."},
        {50, "알고리즘 초기화에 실패했습니다."},
        {51, "입출력 오류입니다."},
        {52, "인코딩 오류입니다."},
        {53, "파일 열기 오류입니다."},
        {54, "스트림 오류입니다."},
        {55, "스트림 읽기 오류입니다."},
        {56, "스트림 쓰기 오류입니다."},
        {57, "메모리가 부족합니다."},
        {58, "경로가 너무 깁니다."},

        {60, "파일을 찾을 수 없습니다."},
        {61, "디렉터리를 찾을 수 없습니다."},
        {62, "접근이 거부되었습니다."},

        {70, "지원되지 않는 버전입니다."},
        {71, "모호한 옵션입니다."},

        {80, "헤더 체크섬이 올바르지 않습니다."},
        {81, "원본 데이터의 체크섬이 올바르지 않습니다."},
        {82, "암호화된 데이터의 체크섬이 올바르지 않습니다."},

        {90, "헤더의 개체를 만들지 못했습니다."},
        {91, "헤더 버전이 맞지 않습니다."},

        {130, "사용자 정의 처리 루틴(UDPR, User Defined Processing Routine)에서 오류가 발생했습니다."},
    };

        public static string GetErrorMessageFromTaskResult(TaskResult r) {
            int code = (int)r;
            if (m_errorDict.ContainsKey(code))
                return m_errorDict[code];
            else
                return "알려지지 않은 오류입니다.";
        }
    }
}