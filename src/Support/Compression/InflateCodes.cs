// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InflateCodes.cs" company="XamlNinja">
//   2011 Richard Griffin and Ollie Riches
// </copyright>
// <summary>
// http://www.sharpgis.net/post/2011/08/28/GZIP-Compressed-Web-Requests-in-WP7-Take-2.aspx
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WP7Contrib.Communications.Compression
{
    using System;

    internal sealed class InflateCodes
    {
        static readonly int[] inflate_mask = new int[17]
                                                         {
															 0,
															 1,
															 3,
															 7,
															 15,
															 31,
															 63,
															 (int) sbyte.MaxValue,
															 (int) byte.MaxValue,
															 511,
															 1023,
															 2047,
															 4095,
															 8191,
															 16383,
															 (int) short.MaxValue,
															 (int) ushort.MaxValue
														};

        const int START = 0;
        const int LEN = 1;
        const int LENEXT = 2;
        const int DIST = 3;
        const int DISTEXT = 4;
        const int COPY = 5;
        const int LIT = 6;
        const int WASH = 7;
        const int END = 8;
        const int BADCODE = 9;

		internal byte Lbits;
		internal byte Dbits;
		internal int Tree_index = 0;
		internal int Ltree_index;
		internal int Dtree_index;
		internal int Mode;
		internal int Len;
		internal int Need;
		internal int Lit;
		internal int Get_Renamed;
		internal int Dist;

		internal int[] Tree;
		internal int[] Ltree;
		internal int[] Dtree;


		#region Constructors

        static InflateCodes()
        {
        }

        internal InflateCodes()
        {
        }


        internal void Init(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index)
        {
            this.Mode = 0;
            this.Lbits = (byte)bl;
            this.Dbits = (byte)bd;
            this.Ltree = tl;
            this.Ltree_index = tl_index;
            this.Dtree = td;
            this.Dtree_index = td_index;
            this.Tree = (int[])null;
        }

		#endregion Construtors


        internal int Process(InflateBlocks blocks, int r)
        {
            ZlibCodec z = blocks.Codec;
            int num1 = z.NextIn;
            int num2 = z.AvailableBytesIn;
            int number = blocks.Bitb;
            int num3 = blocks.Bitk;
            int num4 = blocks.Write;
            int num5 = num4 < blocks.Read ? blocks.Read - num4 - 1 : blocks.End - num4;
            while (true)
            {
                switch (this.Mode)
                {
                    case 0:
                        if (num5 >= 258 && num2 >= 10)
                        {
                            blocks.Bitb = number;
                            blocks.Bitk = num3;
                            z.AvailableBytesIn = num2;
                            z.TotalBytesIn += (long)(num1 - z.NextIn);
                            z.NextIn = num1;
                            blocks.Write = num4;
                            r = this.InflateFast((int)this.Lbits, (int)this.Dbits, this.Ltree, this.Ltree_index, this.Dtree, this.Dtree_index, blocks, z);
                            num1 = z.NextIn;
                            num2 = z.AvailableBytesIn;
                            number = blocks.Bitb;
                            num3 = blocks.Bitk;
                            num4 = blocks.Write;
                            num5 = num4 < blocks.Read ? blocks.Read - num4 - 1 : blocks.End - num4;
                            if (r != 0)
                            {
                                this.Mode = r == 1 ? 7 : 9;
                                break;
                            }
                        }
                        this.Need = (int)this.Lbits;
                        this.Tree = this.Ltree;
                        this.Tree_index = this.Ltree_index;
                        this.Mode = 1;
                        goto case 1;
                    case 1:
                        int index1 = this.Need;
                        while (num3 < index1)
                        {
                            if (num2 != 0)
                            {
                                r = 0;
                                --num2;
                                number |= ((int)z.InputBuffer[num1++] & (int)byte.MaxValue) << num3;
                                num3 += 8;
                            }
                            else
                            {
                                blocks.Bitb = number;
                                blocks.Bitk = num3;
                                z.AvailableBytesIn = num2;
                                z.TotalBytesIn += (long)(num1 - z.NextIn);
                                z.NextIn = num1;
                                blocks.Write = num4;
                                return blocks.Flush(r);
                            }
                        }
                        int index2 = (this.Tree_index + (number & InflateCodes.inflate_mask[index1])) * 3;
                        number = SharedUtils.URShift(number, this.Tree[index2 + 1]);
                        num3 -= this.Tree[index2 + 1];
                        int num6 = this.Tree[index2];
                        if (num6 == 0)
                        {
                            this.Lit = this.Tree[index2 + 2];
                            this.Mode = 6;
                            break;
                        }
                        else if ((num6 & 16) != 0)
                        {
                            this.Get_Renamed = num6 & 15;
                            this.Len = this.Tree[index2 + 2];
                            this.Mode = 2;
                            break;
                        }
                        else if ((num6 & 64) == 0)
                        {
                            this.Need = num6;
                            this.Tree_index = index2 / 3 + this.Tree[index2 + 2];
                            break;
                        }
                        else if ((num6 & 32) != 0)
                        {
                            this.Mode = 7;
                            break;
                        }
                        else
                            goto label_18;
                    case 2:
                        int index3 = this.Get_Renamed;
                        while (num3 < index3)
                        {
                            if (num2 != 0)
                            {
                                r = 0;
                                --num2;
                                number |= ((int)z.InputBuffer[num1++] & (int)byte.MaxValue) << num3;
                                num3 += 8;
                            }
                            else
                            {
                                blocks.Bitb = number;
                                blocks.Bitk = num3;
                                z.AvailableBytesIn = num2;
                                z.TotalBytesIn += (long)(num1 - z.NextIn);
                                z.NextIn = num1;
                                blocks.Write = num4;
                                return blocks.Flush(r);
                            }
                        }
                        this.Len += number & InflateCodes.inflate_mask[index3];
                        number >>= index3;
                        num3 -= index3;
                        this.Need = (int)this.Dbits;
                        this.Tree = this.Dtree;
                        this.Tree_index = this.Dtree_index;
                        this.Mode = 3;
                        goto case 3;
                    case 3:
                        int index4 = this.Need;
                        while (num3 < index4)
                        {
                            if (num2 != 0)
                            {
                                r = 0;
                                --num2;
                                number |= ((int)z.InputBuffer[num1++] & (int)byte.MaxValue) << num3;
                                num3 += 8;
                            }
                            else
                            {
                                blocks.Bitb = number;
                                blocks.Bitk = num3;
                                z.AvailableBytesIn = num2;
                                z.TotalBytesIn += (long)(num1 - z.NextIn);
                                z.NextIn = num1;
                                blocks.Write = num4;
                                return blocks.Flush(r);
                            }
                        }
                        int index5 = (this.Tree_index + (number & InflateCodes.inflate_mask[index4])) * 3;
                        number >>= this.Tree[index5 + 1];
                        num3 -= this.Tree[index5 + 1];
                        int num7 = this.Tree[index5];
                        if ((num7 & 16) != 0)
                        {
                            this.Get_Renamed = num7 & 15;
                            this.Dist = this.Tree[index5 + 2];
                            this.Mode = 4;
                            break;
                        }
                        else if ((num7 & 64) == 0)
                        {
                            this.Need = num7;
                            this.Tree_index = index5 / 3 + this.Tree[index5 + 2];
                            break;
                        }
                        else
                            goto label_34;
                    case 4:
                        int index6 = this.Get_Renamed;
                        while (num3 < index6)
                        {
                            if (num2 != 0)
                            {
                                r = 0;
                                --num2;
                                number |= ((int)z.InputBuffer[num1++] & (int)byte.MaxValue) << num3;
                                num3 += 8;
                            }
                            else
                            {
                                blocks.Bitb = number;
                                blocks.Bitk = num3;
                                z.AvailableBytesIn = num2;
                                z.TotalBytesIn += (long)(num1 - z.NextIn);
                                z.NextIn = num1;
                                blocks.Write = num4;
                                return blocks.Flush(r);
                            }
                        }
                        this.Dist += number & InflateCodes.inflate_mask[index6];
                        number >>= index6;
                        num3 -= index6;
                        this.Mode = 5;
                        goto case 5;
                    case 5:
                        int num8 = num4 - this.Dist;
                        while (num8 < 0)
                            num8 += blocks.End;
                        for (; this.Len != 0; --this.Len)
                        {
                            if (num5 == 0)
                            {
                                if (num4 == blocks.End && blocks.Read != 0)
                                {
                                    num4 = 0;
                                    num5 = num4 < blocks.Read ? blocks.Read - num4 - 1 : blocks.End - num4;
                                }
                                if (num5 == 0)
                                {
                                    blocks.Write = num4;
                                    r = blocks.Flush(r);
                                    num4 = blocks.Write;
                                    num5 = num4 < blocks.Read ? blocks.Read - num4 - 1 : blocks.End - num4;
                                    if (num4 == blocks.End && blocks.Read != 0)
                                    {
                                        num4 = 0;
                                        num5 = num4 < blocks.Read ? blocks.Read - num4 - 1 : blocks.End - num4;
                                    }
                                    if (num5 == 0)
                                    {
                                        blocks.Bitb = number;
                                        blocks.Bitk = num3;
                                        z.AvailableBytesIn = num2;
                                        z.TotalBytesIn += (long)(num1 - z.NextIn);
                                        z.NextIn = num1;
                                        blocks.Write = num4;
                                        return blocks.Flush(r);
                                    }
                                }
                            }
                            blocks.Window[num4++] = blocks.Window[num8++];
                            --num5;
                            if (num8 == blocks.End)
                                num8 = 0;
                        }
                        this.Mode = 0;
                        break;
                    case 6:
                        if (num5 == 0)
                        {
                            if (num4 == blocks.End && blocks.Read != 0)
                            {
                                num4 = 0;
                                num5 = num4 < blocks.Read ? blocks.Read - num4 - 1 : blocks.End - num4;
                            }
                            if (num5 == 0)
                            {
                                blocks.Write = num4;
                                r = blocks.Flush(r);
                                num4 = blocks.Write;
                                num5 = num4 < blocks.Read ? blocks.Read - num4 - 1 : blocks.End - num4;
                                if (num4 == blocks.End && blocks.Read != 0)
                                {
                                    num4 = 0;
                                    num5 = num4 < blocks.Read ? blocks.Read - num4 - 1 : blocks.End - num4;
                                }
                                if (num5 == 0)
                                    goto label_65;
                            }
                        }
                        r = 0;
                        blocks.Window[num4++] = (byte)this.Lit;
                        --num5;
                        this.Mode = 0;
                        break;
                    case 7:
                        goto label_68;
                    case 8:
                        goto label_73;
                    case 9:
                        goto label_74;
                    default:
                        goto label_75;
                }
            }
            label_18:
            this.Mode = 9;
            z.Message = "invalid literal/length code";
            r = -3;
            blocks.Bitb = number;
            blocks.Bitk = num3;
            z.AvailableBytesIn = num2;
            z.TotalBytesIn += (long)(num1 - z.NextIn);
            z.NextIn = num1;
            blocks.Write = num4;
            return blocks.Flush(r);
            label_34:
            this.Mode = 9;
            z.Message = "invalid distance code";
            r = -3;
            blocks.Bitb = number;
            blocks.Bitk = num3;
            z.AvailableBytesIn = num2;
            z.TotalBytesIn += (long)(num1 - z.NextIn);
            z.NextIn = num1;
            blocks.Write = num4;
            return blocks.Flush(r);
            label_65:
            blocks.Bitb = number;
            blocks.Bitk = num3;
            z.AvailableBytesIn = num2;
            z.TotalBytesIn += (long)(num1 - z.NextIn);
            z.NextIn = num1;
            blocks.Write = num4;
            return blocks.Flush(r);
            label_68:
            if (num3 > 7)
            {
                num3 -= 8;
                ++num2;
                --num1;
            }
            blocks.Write = num4;
            r = blocks.Flush(r);
            num4 = blocks.Write;
            int num9 = num4 < blocks.Read ? blocks.Read - num4 - 1 : blocks.End - num4;
            if (blocks.Read != blocks.Write)
            {
                blocks.Bitb = number;
                blocks.Bitk = num3;
                z.AvailableBytesIn = num2;
                z.TotalBytesIn += (long)(num1 - z.NextIn);
                z.NextIn = num1;
                blocks.Write = num4;
                return blocks.Flush(r);
            }
            else
                this.Mode = 8;
            label_73:
            r = 1;
            blocks.Bitb = number;
            blocks.Bitk = num3;
            z.AvailableBytesIn = num2;
            z.TotalBytesIn += (long)(num1 - z.NextIn);
            z.NextIn = num1;
            blocks.Write = num4;
            return blocks.Flush(r);
            label_74:
            r = -3;
            blocks.Bitb = number;
            blocks.Bitk = num3;
            z.AvailableBytesIn = num2;
            z.TotalBytesIn += (long)(num1 - z.NextIn);
            z.NextIn = num1;
            blocks.Write = num4;
            return blocks.Flush(r);
            label_75:
            r = -2;
            blocks.Bitb = number;
            blocks.Bitk = num3;
            z.AvailableBytesIn = num2;
            z.TotalBytesIn += (long)(num1 - z.NextIn);
            z.NextIn = num1;
            blocks.Write = num4;
            return blocks.Flush(r);
        }

        internal int InflateFast(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index, InflateBlocks s, ZlibCodec z)
        {
            int num1 = z.NextIn;
            int num2 = z.AvailableBytesIn;
            int num3 = s.Bitb;
            int num4 = s.Bitk;
            int destinationIndex = s.Write;
            int num5 = destinationIndex < s.Read ? s.Read - destinationIndex - 1 : s.End - destinationIndex;
            int num6 = InflateCodes.inflate_mask[bl];
            int num7 = InflateCodes.inflate_mask[bd];
            do
            {
                while (num4 < 20)
                {
                    --num2;
                    num3 |= ((int)z.InputBuffer[num1++] & (int)byte.MaxValue) << num4;
                    num4 += 8;
                }
                int num8 = num3 & num6;
                int[] numArray1 = tl;
                int num9 = tl_index;
                int index1 = (num9 + num8) * 3;
                int index2;
                if ((index2 = numArray1[index1]) == 0)
                {
                    num3 >>= numArray1[index1 + 1];
                    num4 -= numArray1[index1 + 1];
                    s.Window[destinationIndex++] = (byte)numArray1[index1 + 2];
                    --num5;
                }
                else
                {
                    while (true)
                    {
                        num3 >>= numArray1[index1 + 1];
                        num4 -= numArray1[index1 + 1];
                        if ((index2 & 16) == 0)
                        {
                            if ((index2 & 64) == 0)
                            {
                                num8 = num8 + numArray1[index1 + 2] + (num3 & InflateCodes.inflate_mask[index2]);
                                index1 = (num9 + num8) * 3;
                                if ((index2 = numArray1[index1]) != 0)
                                {}
                                else
                                    goto label_34;
                            }
                            else
                                goto label_35;
                        }
                        else
                            break;
                    }
                    int index3 = index2 & 15;
                    int length1 = numArray1[index1 + 2] + (num3 & InflateCodes.inflate_mask[index3]);
                    int num10 = num3 >> index3;
                    int num11 = num4 - index3;
                    while (num11 < 15)
                    {
                        --num2;
                        num10 |= ((int)z.InputBuffer[num1++] & (int)byte.MaxValue) << num11;
                        num11 += 8;
                    }
                    int num12 = num10 & num7;
                    int[] numArray2 = td;
                    int num13 = td_index;
                    int index4 = (num13 + num12) * 3;
                    int index5 = numArray2[index4];
                    while (true)
                    {
                        num10 >>= numArray2[index4 + 1];
                        num11 -= numArray2[index4 + 1];
                        if ((index5 & 16) == 0)
                        {
                            if ((index5 & 64) == 0)
                            {
                                num12 = num12 + numArray2[index4 + 2] + (num10 & InflateCodes.inflate_mask[index5]);
                                index4 = (num13 + num12) * 3;
                                index5 = numArray2[index4];
                            }
                            else
                                goto label_31;
                        }
                        else
                            break;
                    }
                    int index6 = index5 & 15;
                    while (num11 < index6)
                    {
                        --num2;
                        num10 |= ((int)z.InputBuffer[num1++] & (int)byte.MaxValue) << num11;
                        num11 += 8;
                    }
                    int num14 = numArray2[index4 + 2] + (num10 & InflateCodes.inflate_mask[index6]);
                    num3 = num10 >> index6;
                    num4 = num11 - index6;
                    num5 -= length1;
                    int sourceIndex1;
                    int num15;
                    if (destinationIndex >= num14)
                    {
                        int sourceIndex2 = destinationIndex - num14;
                        if (destinationIndex - sourceIndex2 > 0 && 2 > destinationIndex - sourceIndex2)
                        {
                            byte[] numArray3 = s.Window;
                            int index7 = destinationIndex;
                            int num16 = 1;
                            int num17 = index7 + num16;
                            byte[] numArray4 = s.Window;
                            int index8 = sourceIndex2;
                            int num18 = 1;
                            int num19 = index8 + num18;
                            int num20 = (int)numArray4[index8];
                            numArray3[index7] = (byte)num20;
                            byte[] numArray5 = s.Window;
                            int index9 = num17;
                            int num21 = 1;
                            destinationIndex = index9 + num21;
                            byte[] numArray6 = s.Window;
                            int index10 = num19;
                            int num22 = 1;
                            sourceIndex1 = index10 + num22;
                            int num23 = (int)numArray6[index10];
                            numArray5[index9] = (byte)num23;
                            length1 -= 2;
                        }
                        else
                        {
                            Array.Copy((Array)s.Window, sourceIndex2, (Array)s.Window, destinationIndex, 2);
                            destinationIndex += 2;
                            sourceIndex1 = sourceIndex2 + 2;
                            length1 -= 2;
                        }
                    }
                    else
                    {
                        sourceIndex1 = destinationIndex - num14;
                        do
                        {
                            sourceIndex1 += s.End;
                        }
                        while (sourceIndex1 < 0);
                        int length2 = s.End - sourceIndex1;
                        if (length1 > length2)
                        {
                            length1 -= length2;
                            if (destinationIndex - sourceIndex1 > 0 && length2 > destinationIndex - sourceIndex1)
                            {
                                do
                                {
                                    s.Window[destinationIndex++] = s.Window[sourceIndex1++];
                                }
                                while (--length2 != 0);
                            }
                            else
                            {
                                Array.Copy((Array)s.Window, sourceIndex1, (Array)s.Window, destinationIndex, length2);
                                destinationIndex += length2;
                                num15 = sourceIndex1 + length2;
                            }
                            sourceIndex1 = 0;
                        }
                    }
                    if (destinationIndex - sourceIndex1 > 0 && length1 > destinationIndex - sourceIndex1)
                    {
                        do
                        {
                            s.Window[destinationIndex++] = s.Window[sourceIndex1++];
                        }
                        while (--length1 != 0);
                        goto label_39;
                    }
                    else
                    {
                        Array.Copy((Array)s.Window, sourceIndex1, (Array)s.Window, destinationIndex, length1);
                        destinationIndex += length1;
                        num15 = sourceIndex1 + length1;
                        goto label_39;
                    }
                    label_31:
                    z.Message = "invalid distance code";
                    int num24 = z.AvailableBytesIn - num2;
                    int num25 = num11 >> 3 < num24 ? num11 >> 3 : num24;
                    int num26 = num2 + num25;
                    int num27 = num1 - num25;
                    int num28 = num11 - (num25 << 3);
                    s.Bitb = num10;
                    s.Bitk = num28;
                    z.AvailableBytesIn = num26;
                    z.TotalBytesIn += (long)(num27 - z.NextIn);
                    z.NextIn = num27;
                    s.Write = destinationIndex;
                    return -3;
                    label_34:
                    num3 >>= numArray1[index1 + 1];
                    num4 -= numArray1[index1 + 1];
                    s.Window[destinationIndex++] = (byte)numArray1[index1 + 2];
                    --num5;
                    goto label_39;
                    label_35:
                    if ((index2 & 32) != 0)
                    {
                        int num16 = z.AvailableBytesIn - num2;
                        int num17 = num4 >> 3 < num16 ? num4 >> 3 : num16;
                        int num18 = num2 + num17;
                        int num19 = num1 - num17;
                        int num20 = num4 - (num17 << 3);
                        s.Bitb = num3;
                        s.Bitk = num20;
                        z.AvailableBytesIn = num18;
                        z.TotalBytesIn += (long)(num19 - z.NextIn);
                        z.NextIn = num19;
                        s.Write = destinationIndex;
                        return 1;
                    }
                    else
                    {
                        z.Message = "invalid literal/length code";
                        int num16 = z.AvailableBytesIn - num2;
                        int num17 = num4 >> 3 < num16 ? num4 >> 3 : num16;
                        int num18 = num2 + num17;
                        int num19 = num1 - num17;
                        int num20 = num4 - (num17 << 3);
                        s.Bitb = num3;
                        s.Bitk = num20;
                        z.AvailableBytesIn = num18;
                        z.TotalBytesIn += (long)(num19 - z.NextIn);
                        z.NextIn = num19;
                        s.Write = destinationIndex;
                        return -3;
                    }
                    label_39: ;
                }
            }
            while (num5 >= 258 && num2 >= 10);
            int num29 = z.AvailableBytesIn - num2;
            int num30 = num4 >> 3 < num29 ? num4 >> 3 : num29;
            int num31 = num2 + num30;
            int num32 = num1 - num30;
            int num33 = num4 - (num30 << 3);
            s.Bitb = num3;
            s.Bitk = num33;
            z.AvailableBytesIn = num31;
            z.TotalBytesIn += (long)(num32 - z.NextIn);
            z.NextIn = num32;
            s.Write = destinationIndex;
            return 0;
        }
    }
}