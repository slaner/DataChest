using System;
using System.Security.Cryptography;

/// <summary>
/// CRC-32 해시 계산 함수가 정의되어 있는 클래스입니다.
/// </summary>
public sealed class Crc32 : HashAlgorithm {
    // CRC32 based Wikipedia and Stephan Brumme blog
    // https://en.wikipedia.org/wiki/Cyclic_redundancy_check
    // http://create.stephan-brumme.com/crc32

    /// <summary>CRC-32 해시 알고리즘의 기본 다항식 상수(MSB 우선)입니다.</summary>
    public const uint DefaultPolynomial = 0x04C11BD7;

    uint[] m_crcTable = new uint[256];
    uint g_polynomial;
    uint g_hash;


    /// <summary>
    /// 기본 다항식 상수 값(0x04C11BD7)을 사용하여 CRC-32 해시 알고리즘 개체를 초기화합니다.
    /// </summary>
    public Crc32() : this(DefaultPolynomial) { }
    /// <summary>
    /// 지정된 다항식 값을 사용하여 CRC-32 해시 알고리즘 개체를 초기화합니다.
    /// </summary>
    /// <param name="polynomial">CRC-32 해시 계산에 사용할 다항식 값을 입력합니다.</param>
    public Crc32(uint polynomial) {
        g_polynomial = polynomial;
        CreateCrcTable();
    }


    public override void Initialize() {
        // Do Nothing:
    }

    // Core Functions:
    //    HashCore, HashFinal, CreateCrcTable
    protected override void HashCore(byte[] array, int ibStart, int cbSize) {
        uint crc = 0;
        for (int i = ibStart; i < ibStart + cbSize; i++)
            crc = (crc >> 8) ^ m_crcTable[(crc & 0xFF) ^ array[i]];
        g_hash = ~crc;
    }
    protected override byte[] HashFinal() {
        return BitConverter.GetBytes(g_hash);
    }
    void CreateCrcTable() {
        for (int i = 0; i < 256; i++) {
            uint temp = (uint)i;
            for (int j = 0; j < 8; j++) {
                temp = (temp & 1) != 0 ? g_polynomial ^ (temp >> 1) : temp >> 1;
            }

            m_crcTable[i] = temp;
        }
    }


    /// <summary>CRC-32 해시를 계산한 결과 값을 가져옵니다.</summary>
    public uint HashResult {
        get { return g_hash; }
    }

    /// <summary>CRC-32 해시 계산에 사용되는 다항식 값을 가져옵니다.</summary>
    public uint Polynomial {
        get { return g_polynomial; }
    }

    /// <summary>계산된 해시 코드의 크기(비트 단위)를 가져옵니다.</summary>
    public override int HashSize {
        get { return 32; }
    }
}