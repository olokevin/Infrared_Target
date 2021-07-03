using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// AES(128)加密算法：
/// 说明：  
///     1、该算法是RM内部使用的非标准性AES加密算法
///     2、该加密算法是从对应的c++语言移植过来
///     3、
/// </summary>

public class AES2
{
    public const byte BPOLY = 0x1b;
    public const int BLOCKSIZE = 16; //!< Block size in number of bytes.
    public const int READ_NUMBER = 256;
    public const int KEY_COUNT = 1;
    public const int KEYBITS = 128; //!< Use AES128.
    public const int ROUNDS = 10; //!< Number of rounds.
    public const int KEYLENGTH = 16; //!< Key length in number of bytes.

    public byte[] block1 = new byte[256]; //!< Workspace 1.
    public byte[] block2 = new byte[256]; //!< Worksapce 2.
    public byte[] tempbuf = new byte[256];
    public byte[] chainBlock = new byte[16];

    public byte[] powTbl;  //!< Final location of exponentiation lookup table.
    public byte[] logTbl;  //!< Final location of logarithm lookup table.
    public byte[] sBox;    //!< Final location of s-box.
    public byte[] sBoxInv; //!< Final location of inverse s-box.
    public byte[] expandedKey; //!< Final location of expanded key.
    public byte[] curKey;

    //AES key mc
    byte[] kTable = new byte[32]
    {
        0x91, 0x70, 0x9a, 0xD3, 0x26, 0x67, 0x2A, 0xC3,
        0x82, 0xB6, 0x69, 0x27, 0xE6, 0xd8, 0x84, 0x21,
        0x25, 0xF8, 0xA8, 0x8E, 0x29, 0x2A, 0x15, 0x93,
        0xD5, 0xC5, 0xA3, 0xB2, 0x7B, 0x91, 0x28, 0x67,
    };

    byte[] row = new byte[8]
    {
        0x02, 0x03, 0x01, 0x01,
        0x02, 0x03, 0x01, 0x01
    };

    void XORBytes(byte[] bytes1, int offset1, byte[] bytes2, int offset2, byte count)
    {
        for (int i = 0; i < count; ++i)
        {
            bytes1[i + offset1] ^= bytes2[i + offset2];
        }
    }

    void CopyBytes(byte[] to, int tooffset, byte[] from, int fromoffset, byte count)
    {
        for (int i = 0; i < count; ++i)
        {
            to[i + tooffset] = from[i + fromoffset];
        }
    }

    byte Multiply(byte num, byte factor)
    {
        byte mask = 1;
        byte result = 0;

        while (mask != 0)
        {
            // Check bit of factor given by mask.
            if ((mask & factor) != 0)
            {
                // Add current multiple of num in GF(2).
                result ^= num;
            }

            // Shift mask to indicate next bit.
            mask <<= 1;

            // Double num.
            byte tmp = (byte)(num & 0x80);
            num = (byte)((num << 1) ^ ((tmp != 0) ? BPOLY : 0));
        }

        return result;
    }

    void ShiftRows(byte[] state, int offset)
    {
        byte temp;

        // Note: State is arranged column by column.

        // Cycle second row left one time.
        temp = state[offset + 1 + 0 * 4];
        state[offset + 1 + 0 * 4] = state[offset + 1 + 1 * 4];
        state[offset + 1 + 1 * 4] = state[offset + 1 + 2 * 4];
        state[offset + 1 + 2 * 4] = state[offset + 1 + 3 * 4];
        state[offset + 1 + 3 * 4] = temp;

        // Cycle third row left two times.
        temp = state[offset + 2 + 0 * 4];
        state[offset + 2 + 0 * 4] = state[offset + 2 + 2 * 4];
        state[offset + 2 + 2 * 4] = temp;
        temp = state[offset + 2 + 1 * 4];
        state[offset + 2 + 1 * 4] = state[offset + 2 + 3 * 4];
        state[offset + 2 + 3 * 4] = temp;

        // Cycle fourth row left three times, ie. right once.
        temp = state[offset + 3 + 3 * 4];
        state[offset + 3 + 3 * 4] = state[offset + 3 + 2 * 4];
        state[offset + 3 + 2 * 4] = state[offset + 3 + 1 * 4];
        state[offset + 3 + 1 * 4] = state[offset + 3 + 0 * 4];
        state[offset + 3 + 0 * 4] = temp;
    }

    void CalcSBox(byte[] sBox)
    {
        byte i, rot;
        byte temp;
        byte result;

        // Fill all entries of sBox[].
        i = 0;
        do
        {
            //Inverse in GF(2^8).
            if (i > 0)
            {
                temp = powTbl[255 - logTbl[i]];
            }
            else
            {
                temp = 0;
            }

            // Affine transformation in GF(2).
            result = (byte)(temp ^ 0x63); // Start with adding a vector in GF(2).
            for (rot = 4; rot > 0; rot--)
            {
                // Rotate left.
                temp = (byte)((temp << 1) | (temp >> 7));
                // Add rotated byte in GF(2).
                result ^= temp;
            }
            // Put result in table.
            sBox[i] = result;
        } while (++i != 0);
    }

    void CalcPowLog(byte[] powTbl, byte[] logTbl)
    {
        byte i = 0;
        byte t = 1;
        do
        {
            // Use 0x03 as root for exponentiation and logarithms.
            powTbl[i] = t;
            logTbl[t] = i;
            i++;
            // Multiply t by 3 in GF(2^8).
            byte tmp = (byte)((t << 1) ^ (((t & 0x80) != 0) ? BPOLY : 0));
            t ^= tmp;
        } while (t != 1); // Cyclic properties ensure that i < 255.
        powTbl[255] = powTbl[0]; // 255 = '-0', 254 = -1, etc.
    }

    void CalcSBoxInv(byte[] sBox, byte[] sBoxInv)
    {
        byte i = 0;
        byte j = 0;

        // Iterate through all elements in sBoxInv using  i.
        do
        {
            // Search through sBox using j.
            do
            {
                // Check if current j is the inverse of current i.
                if (sBox[j] == i)
                {
                    // If so, set sBoxInc and indicate search finished.
                    sBoxInv[i] = j;
                    j = 255;
                }
            } while (++j != 0);
        } while (++i != 0);
    }

    void CycleLeft(byte[] row)
    {
        // Cycle 4 bytes in an array left once.
        byte temp = row[0];
        row[0] = row[1];
        row[1] = row[2];
        row[2] = row[3];
        row[3] = temp;
    }

    void CalcCols(byte[] col, int offset)
    {
        for (int i = 0; i < 4; ++i)
        {
            col[i + offset] = (byte)((col[i + offset] << 1) ^ (((col[i + offset] & 0x80) != 0) ? BPOLY : 0));
        }
    }

    void InvMixColumn(byte[] column, int offset)
    {
        byte[] r = new byte[4];

        r[0] = (byte)(column[1 + offset] ^ column[2 + offset] ^ column[3 + offset]);
        r[1] = (byte)(column[0 + offset] ^ column[2 + offset] ^ column[3 + offset]);
        r[2] = (byte)(column[0 + offset] ^ column[1 + offset] ^ column[3 + offset]);
        r[3] = (byte)(column[0 + offset] ^ column[1 + offset] ^ column[2 + offset]);

        /*column[0] = (column[0] << 1) ^ (column[0] & 0x80 ? BPOLY : 0);
	    column[1] = (column[1] << 1) ^ (column[1] & 0x80 ? BPOLY : 0);
	    column[2] = (column[2] << 1) ^ (column[2] & 0x80 ? BPOLY : 0);
	    column[3] = (column[3] << 1) ^ (column[3] & 0x80 ? BPOLY : 0);*/
        CalcCols(column, offset);

        r[0] ^= (byte)(column[0 + offset] ^ column[1 + offset]);
        r[1] ^= (byte)(column[1 + offset] ^ column[2 + offset]);
        r[2] ^= (byte)(column[2 + offset] ^ column[3 + offset]);
        r[3] ^= (byte)(column[0 + offset] ^ column[3 + offset]);

        /*column[0] = (column[0] << 1) ^ (column[0] & 0x80 ? BPOLY : 0);
	    column[1] = (column[1] << 1) ^ (column[1] & 0x80 ? BPOLY : 0);
	    column[2] = (column[2] << 1) ^ (column[2] & 0x80 ? BPOLY : 0);
	    column[3] = (column[3] << 1) ^ (column[3] & 0x80 ? BPOLY : 0);*/
        CalcCols(column, offset);

        r[0] ^= (byte)(column[0 + offset] ^ column[2 + offset]);
        r[1] ^= (byte)(column[1 + offset] ^ column[3 + offset]);
        r[2] ^= (byte)(column[0 + offset] ^ column[2 + offset]);
        r[3] ^= (byte)(column[1 + offset] ^ column[3 + offset]);

        /*column[0] = (column[0] << 1) ^ (column[0] & 0x80 ? BPOLY : 0);
	    column[1] = (column[1] << 1) ^ (column[1] & 0x80 ? BPOLY : 0);
	    column[2] = (column[2] << 1) ^ (column[2] & 0x80 ? BPOLY : 0);
	    column[3] = (column[3] << 1) ^ (column[3] & 0x80 ? BPOLY : 0);*/
        CalcCols(column, offset);

        column[0 + offset] ^= (byte)(column[1 + offset] ^ column[2 + offset] ^ column[3 + offset]);
        r[0] ^= column[0 + offset];
        r[1] ^= column[0 + offset];
        r[2] ^= column[0 + offset];
        r[3] ^= column[0 + offset];

        column[0 + offset] = r[0];
        column[1 + offset] = r[1];
        column[2 + offset] = r[2];
        column[3 + offset] = r[3];
    }

    void SubBytes(byte[] bytes, int offset, byte count)
    {
        for (int i = 0; i < count; ++i)
        {
            bytes[i + offset] = sBox[bytes[i + offset]];
        }
    }

    void InvSubBytesAndXOR(byte[] bytes, int byteoffset, byte[] key, int keyoffset, byte count)
    {
        for (int i = 0; i < count; ++i)
        {
            bytes[i + byteoffset] = (byte)(block2[bytes[i + byteoffset]] ^ key[i + keyoffset]);
        }
    }

    void InvShiftRows(byte[] state, int offset)
    {
        byte temp;

        // Note: State is arranged column by column.

        // Cycle second row right one time.
        temp = state[offset + 1 + 3 * 4];
        state[offset + 1 + 3 * 4] = state[offset + 1 + 2 * 4];
        state[offset + 1 + 2 * 4] = state[offset + 1 + 1 * 4];
        state[offset + 1 + 1 * 4] = state[offset + 1 + 0 * 4];
        state[offset + 1 + 0 * 4] = temp;

        // Cycle third row right two times.
        temp = state[offset + 2 + 0 * 4];
        state[offset + 2 + 0 * 4] = state[offset + 2 + 2 * 4];
        state[offset + 2 + 2 * 4] = temp;
        temp = state[offset + 2 + 1 * 4];
        state[offset + 2 + 1 * 4] = state[offset + 2 + 3 * 4];
        state[offset + 2 + 3 * 4] = temp;

        // Cycle fourth row right three times, ie. left once.
        temp = state[offset + 3 + 0 * 4];
        state[offset + 3 + 0 * 4] = state[offset + 3 + 1 * 4];
        state[offset + 3 + 1 * 4] = state[offset + 3 + 2 * 4];
        state[offset + 3 + 2 * 4] = state[offset + 3 + 3 * 4];
        state[offset + 3 + 3 * 4] = temp;
    }

    void KeyExpansion(byte[] expandedKey, int exp_offset)
    {
        byte[] temp = new byte[4];
        byte[] Rcon = new byte[4] { 0x01, 0x00, 0x00, 0x00 }; // Round constant.

        byte[] key = curKey;

        // Copy key to start of expanded key.
        for (int k = 0; k < KEYLENGTH; ++k)
        {
            expandedKey[exp_offset] = key[k];
            exp_offset++;
        }

        CopyBytes(temp, 0, expandedKey, exp_offset - 4, 4);

        // Expand key.
        int i = KEYLENGTH;

        while (i < BLOCKSIZE * (ROUNDS + 1))
        {
            // Are we at the start of a multiple of the key size?
            if ((i % KEYLENGTH) == 0)
            {
                CycleLeft(temp); // Cycle left once.
                SubBytes(temp, 0, 4); // Substitute each byte.
                XORBytes(temp, 0, Rcon, 0, 4); // Add constant in GF(2).
                Rcon[0] = (byte)((Rcon[0] << 1) ^ (((Rcon[0] & 0x80) != 0) ? BPOLY : 0));
            }

            // Add bytes in GF(2) one KEYLENGTH away.
            XORBytes(temp, 0, expandedKey, exp_offset - KEYLENGTH, 4);

            expandedKey[exp_offset++] = temp[0];
            expandedKey[exp_offset++] = temp[1];
            expandedKey[exp_offset++] = temp[2];
            expandedKey[exp_offset++] = temp[3];

            i += 4; // Next 4 bytes.
        }
    }

    void InvCipher(byte[] block, int block_offset, byte[] expandedKey, int exp_offset)
    {
        byte i, j;
        byte round = ROUNDS - 1;

        //expandedKey += BLOCKSIZE * ROUNDS;
        exp_offset += BLOCKSIZE * ROUNDS;
        XORBytes(block, block_offset, expandedKey, exp_offset, 16);
        //expandedKey -= BLOCKSIZE;
        exp_offset -= BLOCKSIZE;

        for (int k = 0; k < round; ++k)
        {
            InvShiftRows(block, block_offset);
            InvSubBytesAndXOR(block, block_offset, expandedKey, exp_offset, 16);
            exp_offset -= BLOCKSIZE;
            //InvMixColumns( block );
            for (i = 4, j = 0; i > 0; i--, j += 4)
            {
                InvMixColumn(block, block_offset + j);
            }
        }

        InvShiftRows(block, block_offset);
        InvSubBytesAndXOR(block, block_offset, expandedKey, exp_offset, 16);
    }

    public void aesDecInit()
    {
        powTbl = block1;
        logTbl = block2;
        CalcPowLog(powTbl, logTbl);

        sBox = tempbuf;
        CalcSBox(sBox);

        expandedKey = block1;
        KeyExpansion(expandedKey, 0);

        sBoxInv = block2; // Must be block2.
        CalcSBoxInv(sBox, sBoxInv);
    }

    public void aesDecrypt(byte[] buffer, byte[] chainBlock)
    {
        CopyBytes(tempbuf, 0, buffer, 0, BLOCKSIZE);
        InvCipher(buffer, 0, expandedKey, 0);
        XORBytes(buffer, 0, chainBlock, 0, BLOCKSIZE);
        CopyBytes(chainBlock, 0, tempbuf, 0, BLOCKSIZE);
    }

    public void aesEncrypt(byte[] buffer, byte[] chainBlock)
    {
        XORBytes(buffer, 0, chainBlock, 0, BLOCKSIZE);
        Cipher(buffer, 0, expandedKey, 0);
        CopyBytes(chainBlock, 0, buffer, 0, BLOCKSIZE);
    }

    public void aesDecryptBlock(byte[] buffer, int buffer_offset, uint nSize)
    {
        uint n;

        //add 2010-07-27
        for (n = 0; n < 16; n++)
            chainBlock[n] = 0;

        for (n = nSize; n > 0; n -= 16)
        {
            Array.Copy(buffer, buffer_offset, tempbuf, 0, BLOCKSIZE);
            InvCipher(buffer, buffer_offset, expandedKey, 0);
            // 在aes加密的基础上，增加了一异或
            XORBytes(buffer, buffer_offset, chainBlock, 0, BLOCKSIZE);
            CopyBytes(chainBlock, 0, tempbuf, 0, BLOCKSIZE);
            buffer_offset += 16;
        }
    }

    public void aesEncryptBlock(byte[] buffer, int buffer_offset, uint nSize)
    {
        uint n;

        //add 2010-07-27
        for (n = 0; n < 16; n++)
            chainBlock[n] = 0;

        for (n = nSize; n > 0; n -= 16)
        {
            XORBytes(buffer, buffer_offset, chainBlock, 0, BLOCKSIZE);
            Cipher(buffer, buffer_offset, expandedKey, 0);
            CopyBytes(chainBlock, 0, buffer, buffer_offset, BLOCKSIZE);
            buffer_offset += 16;
        }
    }

    byte DotProduct(byte[] vector1, int offset1, byte[] vector2, int offset2)
    {
        byte result = 0;

        //result ^= Multiply( *vector1++, *vector2++ );
        //result ^= Multiply( *vector1++, *vector2++ );
        //result ^= Multiply( *vector1++, *vector2++ );
        //result ^= Multiply( *vector1  , *vector2   );

        for (int i = 0; i < 4; ++i)
        {
            result ^= Multiply(vector1[i + offset1], vector2[i + offset2]);
        }

        return result;
    }

    void Cipher(byte[] block, int block_offset1, byte[] expandedKey, int exp_offset)
    {
        byte i, j;
        byte round = ROUNDS - 1;

        int expoffset = exp_offset;
        XORBytes(block, block_offset1, expandedKey, expoffset, 16);
        //expandedKey += BLOCKSIZE;
        expoffset += BLOCKSIZE;

        for (int k = 0; k < round; ++k)
        {
            SubBytes(block, block_offset1, 16);
            ShiftRows(block, block_offset1);
            //MixColumns( block );
            for (i = 4, j = 0; i > 0; i--, j += 4)
            {
                MixColumn(block, block_offset1 + j);
            }

            XORBytes(block, block_offset1, expandedKey, expoffset, 16);
            expoffset += BLOCKSIZE;
        }

        SubBytes(block, block_offset1, 16);
        ShiftRows(block, block_offset1);
        XORBytes(block, block_offset1, expandedKey, expoffset, 16);
    }

    public void aesEncInit()
    {
        powTbl = block1;
        logTbl = block2;
        CalcPowLog(powTbl, logTbl);

        sBox = block2;
        CalcSBox(sBox);

        expandedKey = block1;
        KeyExpansion(expandedKey, 0);
    }

    void MixColumn(byte[] column, int offset)
    {
        byte[] result = new byte[4];

        // Take dot products of each matrix row and the column vector.
        result[0] = DotProduct(row, 0, column, offset);
        result[1] = DotProduct(row, 3, column, offset);
        result[2] = DotProduct(row, 2, column, offset);
        result[3] = DotProduct(row, 1, column, offset);

        // Copy temporary result to original column.
        //column[0] = result[0];
        //column[1] = result[1];
        //column[2] = result[2];
        //column[3] = result[3];
        CopyBytes(column, offset, result, 0, 4);
    }

    public bool initKey(byte receiver_type, byte receiver_ID /*= 0*/ )
    {
        curKey = kTable;
        return true;
    }
}

