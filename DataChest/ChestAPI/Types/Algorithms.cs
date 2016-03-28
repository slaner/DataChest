/// <summary>
/// 암,복호화에 사용될 알고리즘을 열거합니다.<br />
/// SEED 알고리즘은 성능으로 인해 제외되었습니다.
/// </summary>
public enum Algorithms : ushort {
    Aes,
    Des,
    TripleDes,
    // 2016 03 06 REMOVED
    // PERFORMANCE ISSUE
    // Seed128,
    // Seed256,
    Rc2,







    LastMethod,
}