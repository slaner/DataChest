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

/// <summary>
/// ChestAPI 파일 형식에서 사용하는 버전 1 헤더입니다.
/// </summary>
class ChestHeader1 : HeaderBase {
    public ChestHeader1() : base(1) { }

    /// <summary>
    /// 필드 버전이 아닌 헤더의 버전을 가져옵니다.
    /// 상위 버전의 헤더를 만든 경우 이 속성이 정확한 헤더 버전을 반환하도록 재정의해야 합니다.
    /// </summary>
    public override ushort HeaderVersion {
        get { return 1; }
    }
}