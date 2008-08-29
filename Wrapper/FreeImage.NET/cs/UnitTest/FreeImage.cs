// ==========================================================
// FreeImage 3 .NET wrapper
// Original FreeImage 3 functions and .NET compatible derived functions
//
// Design and implementation by
// - Jean-Philippe Goerke (jpgoerke@users.sourceforge.net)
// - Carsten Klein (cklein05@users.sourceforge.net)
//
// Contributors:
// - David Boland (davidboland@vodafone.ie)
//
// Main reference : MSDN Knowlede Base
//
// This file is part of FreeImage 3
//
// COVERED CODE IS PROVIDED UNDER THIS LICENSE ON AN "AS IS" BASIS, WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, WITHOUT LIMITATION, WARRANTIES
// THAT THE COVERED CODE IS FREE OF DEFECTS, MERCHANTABLE, FIT FOR A PARTICULAR PURPOSE
// OR NON-INFRINGING. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE COVERED
// CODE IS WITH YOU. SHOULD ANY COVERED CODE PROVE DEFECTIVE IN ANY RESPECT, YOU (NOT
// THE INITIAL DEVELOPER OR ANY OTHER CONTRIBUTOR) ASSUME THE COST OF ANY NECESSARY
// SERVICING, REPAIR OR CORRECTION. THIS DISCLAIMER OF WARRANTY CONSTITUTES AN ESSENTIAL
// PART OF THIS LICENSE. NO USE OF ANY COVERED CODE IS AUTHORIZED HEREUNDER EXCEPT UNDER
// THIS DISCLAIMER.
//
// Use at your own risk!
// ==========================================================

// ==========================================================
// To build the project without VS use the following commandline:
// "csc.exe /out:FreeImageNET.dll /target:library /doc:FreeImageNET.XML /debug- /o /unsafe+ /filealign:512 FreeImage.cs"
// ==========================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using FreeImageAPI;
using FreeImageAPI.IO;
using FreeImageAPI.Metadata;
using FreeImageAPI.Plugins;

/////////////////////////////////////////////////////
//                                                 //
//              FreeImage.h import                 //
//                                                 //
/////////////////////////////////////////////////////

	#region Structs

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>BITMAP</b> structure defines the type, width, height, color format, and bit values of a bitmap.
	/// </summary>
	/// <remarks>
	/// The bitmap formats currently used are monochrome and color. The monochrome bitmap uses a one-bit,
	/// one-plane format. Each scan is a multiple of 32 bits.
	/// <para/>
	/// Scans are organized as follows for a monochrome bitmap of height n:
	/// <para/>
	/// <code>
	/// Scan 0
	/// Scan 1
	/// .
	/// .
	/// .
	/// Scan n-2
	/// Scan n-1
	/// </code>
	/// <para/>
	/// The pixels on a monochrome device are either black or white. If the corresponding bit in the
	/// bitmap is 1, the pixel is set to the foreground color; if the corresponding bit in the bitmap
	/// is zero, the pixel is set to the background color.
	/// <para/>
	/// All devices that have the RC_BITBLT device capability support bitmaps. For more information,
	/// see <b>GetDeviceCaps</b>.
	/// <para/>
	/// Each device has a unique color format. To transfer a bitmap from one device to another,
	/// use the <b>GetDIBits</b> and <b>SetDIBits</b> functions.
	/// </remarks>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct BITMAP
	{
		/// <summary>
		/// Specifies the bitmap type. This member must be zero.
		/// </summary>
		public int bmType;
		/// <summary>
		/// Specifies the width, in pixels, of the bitmap. The width must be greater than zero.
		/// </summary>
		public int bmWidth;
		/// <summary>
		/// Specifies the height, in pixels, of the bitmap. The height must be greater than zero.
		/// </summary>
		public int bmHeight;
		/// <summary>
		/// Specifies the number of bytes in each scan line. This value must be divisible by 2,
		/// because the system assumes that the bit values of a bitmap form an array that is word aligned.
		/// </summary>
		public int bmWidthBytes;
		/// <summary>
		/// Specifies the count of color planes.
		/// </summary>
		public ushort bmPlanes;
		/// <summary>
		/// Specifies the number of bits required to indicate the color of a pixel.
		/// </summary>
		public ushort bmBitsPixel;
		/// <summary>
		/// Pointer to the location of the bit values for the bitmap.
		/// The <b>bmBits</b> member must be a long pointer to an array of character (1-byte) values.
		/// </summary>
		public IntPtr bmBits;
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// This structure contains information about the dimensions and color format
	/// of a device-independent bitmap (DIB).
	/// </summary>
	/// <remarks>
	/// The <see cref="FreeImageAPI.BITMAPINFO"/> structure combines the
	/// <b>BITMAPINFOHEADER</b> structure and a color table to provide a complete
	/// definition of the dimensions and colors of a DIB.
	/// </remarks>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct BITMAPINFOHEADER : IEquatable<BITMAPINFOHEADER>
	{
		/// <summary>
		/// Specifies the size of the structure, in bytes.
		/// </summary>
		public uint biSize;
		/// <summary>
		/// Specifies the width of the bitmap, in pixels.
		/// <para/>
		/// <b>Windows 98/Me, Windows 2000/XP:</b> If <b>biCompression</b> is BI_JPEG or BI_PNG,
		/// the <b>biWidth</b> member specifies the width of the decompressed JPEG or PNG image file,
		/// respectively.
		/// </summary>
		public int biWidth;
		/// <summary>
		/// Specifies the height of the bitmap, in pixels. If <b>biHeight</b> is positive, the bitmap
		/// is a bottom-up DIB and its origin is the lower-left corner. If <b>biHeight</b> is negative,
		/// the bitmap is a top-down DIB and its origin is the upper-left corner. 
		/// <para/>
		/// If <b>biHeight</b> is negative, indicating a top-down DIB, <b>biCompression</b> must be
		/// either BI_RGB or BI_BITFIELDS. Top-down DIBs cannot be compressed.
		/// <para/>
		/// <b>Windows 98/Me, Windows 2000/XP:</b> If <b>biCompression</b> is BI_JPEG or BI_PNG,
		/// the <b>biHeight</b> member specifies the height of the decompressed JPEG or PNG image file,
		/// respectively.
		/// </summary>
		public int biHeight;
		/// <summary>
		/// Specifies the number of planes for the target device. This value must be set to 1.
		/// </summary>
		public ushort biPlanes;
		/// <summary>
		/// Specifies the number of bits per pixel.The biBitCount member of the <b>BITMAPINFOHEADER</b>
		/// structure determines the number of bits that define each pixel and the maximum number of
		/// colors in the bitmap. This member must be one of the following values.
		/// <para/>
		/// 
		/// <list type="table">
		/// <listheader>
		/// <term>Value</term>
		/// <description>Meaning</description>
		/// </listheader>
		/// 
		/// <item>
		/// <term>0</term>
		/// <description>
		/// <b>Windows 98/Me, Windows 2000/XP:</b> The number of bits-per-pixel is specified
		/// or is implied by the JPEG or PNG format.
		/// </description>
		/// </item>
		/// 
		/// <item>
		/// <term>1</term>
		/// <description>
		/// The bitmap is monochrome, and the bmiColors member of <see cref="FreeImageAPI.BITMAPINFO"/>
		/// contains two entries. Each bit in the bitmap array represents a pixel. If the bit is clear,
		/// the pixel is displayed with the color of the first entry in the bmiColors table; if the bit
		/// is set, the pixel has the color of the second entry in the table.
		/// </description>
		/// </item>
		/// 
		/// <item>
		/// <term>4</term>
		/// <description>
		/// The bitmap has a maximum of 16 colors, and the <b>bmiColors</b> member of <b>BITMAPINFO</b>
		/// contains up to 16 entries. Each pixel in the bitmap is represented by a 4-bit index into the
		/// color table. For example, if the first byte in the bitmap is 0x1F, the byte represents two
		/// pixels. The first pixel contains the color in the second table entry, and the second pixel
		/// contains the color in the sixteenth table entry.</description>
		/// </item>
		/// 
		/// <item>
		/// <term>8</term>
		/// <description>
		/// The bitmap has a maximum of 256 colors, and the <b>bmiColors</b> member of <b>BITMAPINFO</b>
		/// contains up to 256 entries. In this case, each byte in the array represents a single pixel.
		/// </description>
		/// </item>
		/// 
		/// <item>
		/// <term>16</term>
		/// <description>
		/// The bitmap has a maximum of 2^16 colors. If the <b>biCompression</b> member of the
		/// <b>BITMAPINFOHEADER</b> is BI_RGB, the <b>bmiColors</b> member of <b>BITMAPINFO</b> is NULL.
		/// Each <b>WORD</b> in the bitmap array represents a single pixel. The relative intensities
		/// of red, green, and blue are represented with five bits for each color component.
		/// The value for blue is in the least significant five bits, followed by five bits each for
		/// green and red. The most significant bit is not used. The <b>bmiColors</b> color table is used
		/// for optimizing colors used on palette-based devices, and must contain the number of entries
		/// specified by the <b>biClrUsed</b> member of the <b>BITMAPINFOHEADER</b>.
		/// <para/>
		/// If the <b>biCompression</b> member of the <b>BITMAPINFOHEADER</b> is BI_BITFIELDS, the
		/// <b>bmiColors</b> member contains three <b>DWORD</b> color masks that specify the red, green,
		/// and blue components, respectively, of each pixel. Each <b>WORD</b> in the bitmap array represents
		/// a single pixel.
		/// <para/>
		/// <b>Windows NT/Windows 2000/XP:</b> When the <b>biCompression</b> member is BI_BITFIELDS,
		/// bits set in each <b>DWORD</b> mask must be contiguous and should not overlap the bits
		/// of another mask. All the bits in the pixel do not have to be used.
		/// <para/>
		/// <b>Windows 95/98/Me:</b> When the <b>biCompression</b> member is BI_BITFIELDS, the system
		/// supports only the following 16bpp color masks: A 5-5-5 16-bit image, where the blue mask is
		/// 0x001F, the green mask is 0x03E0, and the red mask is 0x7C00; and a 5-6-5 16-bit image,
		/// where the blue mask is 0x001F, the green mask is 0x07E0, and the red mask is 0xF800.
		/// </description>
		/// </item>
		/// 
		/// <item>
		/// <term>24</term>
		/// <description>
		/// The bitmap has a maximum of 2^24 colors, and the <b>bmiColors</b> member of <b>BITMAPINFO</b>
		/// is NULL. Each 3-byte triplet in the bitmap array represents the relative intensities of blue,
		/// green, and red, respectively, for a pixel. The <b>bmiColors</b> color table is used for
		/// optimizing colors used on palette-based devices, and must contain the number of entries
		/// specified by the <b>biClrUsed</b> member of the <b>BITMAPINFOHEADER</b>.
		/// </description>
		/// </item>
		/// 
		/// <item>
		/// <term>32</term>
		/// <description>
		/// The bitmap has a maximum of 2^32 colors. If the <b>biCompression</b> member of the
		/// <b>BITMAPINFOHEADER</b> is BI_RGB, the <b>bmiColors</b> member of <b>BITMAPINFO</b> is NULL.
		/// Each <b>DWORD</b> in the bitmap array represents the relative intensities of blue, green, and red,
		/// respectively, for a pixel. The high byte in each <b>DWORD</b> is not used. The <b>bmiColors</b>
		/// color table is used for optimizing colors used on palette-based devices, and must contain the 
		/// number of entries specified by the <b>biClrUsed</b> member of the <b>BITMAPINFOHEADER</b>.
		/// <para/>
		/// If the <b>biCompression</b> member of the <b>BITMAPINFOHEADER</b> is BI_BITFIELDS,
		/// the <b>bmiColors</b> member contains three <b>DWORD</b> color masks that specify the red, green,
		/// and blue components, respectively, of each pixel. Each <b>DWORD</b> in the bitmap array represents
		/// a single pixel.
		/// <para/>
		/// <b>Windows NT/ 2000:</b> When the <b>biCompression</b> member is BI_BITFIELDS, bits set in each
		/// <b>DWORD</b> mask must be contiguous and should not overlap the bits of another mask. All the
		/// bits in the pixel do not need to be used.
		/// <para/>
		/// <b>Windows 95/98/Me:</b> When the <b>biCompression</b> member is BI_BITFIELDS, the system
		/// supports only the following 32-bpp color mask: The blue mask is 0x000000FF, the green mask is
		/// 0x0000FF00, and the red mask is 0x00FF0000.
		/// </description>
		/// </item>
		/// </list>
		/// </summary>
		public ushort biBitCount;
		/// <summary>
		/// Specifies the type of compression for a compressed bottom-up bitmap (top-down DIBs cannot be
		/// compressed).
		/// <list type="table">
		/// <listheader>
		/// <term>Value</term>
		/// <description>Meaning</description>
		/// </listheader>
		/// 
		/// <item>
		/// <term>BI_RGB</term>
		/// <description>An uncompressed format.</description>
		/// </item>
		/// 
		/// <item>
		/// <term>BI_RLE8</term>
		/// <description>A run-length encoded (RLE) format for bitmaps with 8 bpp. The compression format
		/// is a 2-byte format consisting of a count byte followed by a byte containing a color index.
		/// </description>
		/// </item>
		/// 
		/// <item>
		/// <term>BI_RLE4</term>
		/// <description>An RLE format for bitmaps with 4 bpp. The compression format is a 2-byte format
		/// consisting of a count byte followed by two word-length color indexes.</description>
		/// </item>
		/// 
		/// <item>
		/// <term>BI_BITFIELDS</term>
		/// <description>Specifies that the bitmap is not compressed and that the color table consists
		/// of three <b>DWORD</b> color masks that specify the red, green, and blue components, respectively,
		/// of each pixel. This is valid when used with 16- and 32-bpp bitmaps.</description>
		/// </item>
		/// 
		/// <item>
		/// <term>BI_JPEG</term>
		/// <description><b>Windows 98/Me, Windows 2000/XP:</b> Indicates that the image is a JPEG image.
		/// </description>
		/// </item>
		/// 
		/// <item>
		/// <term>BI_PNG</term>
		/// <description><b>Windows 98/Me, Windows 2000/XP:</b> Indicates that the image is a PNG image.
		/// </description>
		/// </item>
		/// 
		/// </list>
		/// </summary>
		public uint biCompression;
		/// <summary>
		/// Specifies the size, in bytes, of the image. This may be set to zero for BI_RGB bitmaps.
		/// <para/>
		/// <b>Windows 98/Me, Windows 2000/XP:</b> If <b>biCompression</b> is BI_JPEG or BI_PNG,
		/// <b>biSizeImage</b> indicates the size of the JPEG or PNG image buffer, respectively.
		/// </summary>
		public uint biSizeImage;
		/// <summary>
		/// Specifies the horizontal resolution, in pixels-per-meter, of the target device for the bitmap.
		/// An application can use this value to select a bitmap from a resource group that best matches
		/// the characteristics of the current device.
		/// </summary>
		public int biXPelsPerMeter;
		/// <summary>
		/// Specifies the vertical resolution, in pixels-per-meter, of the target device for the bitmap.
		/// </summary>
		public int biYPelsPerMeter;
		/// <summary>
		/// Specifies the number of color indexes in the color table that are actually used by the bitmap.
		/// If this value is zero, the bitmap uses the maximum number of colors corresponding to the value
		/// of the biBitCount member for the compression mode specified by <b>biCompression</b>.
		/// <para/>
		/// If <b>iClrUsed</b> is nonzero and the <b>biBitCount</b> member is less than 16, the <b>biClrUsed</b>
		/// member specifies the actual number of colors the graphics engine or device driver accesses.
		/// If <b>biBitCount</b> is 16 or greater, the <b>biClrUsed</b> member specifies the size of the color
		/// table used to optimize performance of the system color palettes. If <b>biBitCount</b> equals 16 or 32,
		/// the optimal color palette starts immediately following the three <b>DWORD</b> masks.
		/// <para/>
		/// When the bitmap array immediately follows the <see cref="BITMAPINFO"/> structure, it is a packed bitmap.
		/// Packed bitmaps are referenced by a single pointer. Packed bitmaps require that the
		/// <b>biClrUsed</b> member must be either zero or the actual size of the color table.
		/// </summary>
		public uint biClrUsed;
		/// <summary>
		/// Specifies the number of color indexes that are required for displaying the bitmap. If this value
		/// is zero, all colors are required.
		/// </summary>
		public uint biClrImportant;

		/// <summary>
		/// Tests whether two specified <see cref="BITMAPINFOHEADER"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="BITMAPINFOHEADER"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="BITMAPINFOHEADER"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="BITMAPINFOHEADER"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(BITMAPINFOHEADER left, BITMAPINFOHEADER right)
		{
			return ((left.biSize == right.biSize) &&
					(left.biWidth == right.biWidth) &&
					(left.biHeight == right.biHeight) &&
					(left.biPlanes == right.biPlanes) &&
					(left.biBitCount == right.biBitCount) &&
					(left.biCompression == right.biCompression) &&
					(left.biSizeImage == right.biSizeImage) &&
					(left.biXPelsPerMeter == right.biXPelsPerMeter) &&
					(left.biYPelsPerMeter == right.biYPelsPerMeter) &&
					(left.biClrUsed == right.biClrUsed) &&
					(left.biClrImportant == right.biClrImportant));
		}

		/// <summary>
		/// Tests whether two specified <see cref="BITMAPINFOHEADER"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="BITMAPINFOHEADER"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="BITMAPINFOHEADER"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="BITMAPINFOHEADER"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(BITMAPINFOHEADER left, BITMAPINFOHEADER right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Tests whether the specified <see cref="BITMAPINFOHEADER"/> structure is equivalent to this <see cref="BITMAPINFOHEADER"/> structure.
		/// </summary>
		/// <param name="other">A <see cref="BITMAPINFOHEADER"/> structure to compare to this instance.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="BITMAPINFOHEADER"/> structure
		/// equivalent to this <see cref="BITMAPINFOHEADER"/> structure; otherwise, <b>false</b>.</returns>
		public bool Equals(BITMAPINFOHEADER other)
		{
			return (this == other);
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="BITMAPINFOHEADER"/> structure
		/// and is equivalent to this <see cref="BITMAPINFOHEADER"/> structure.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="BITMAPINFOHEADER"/> structure
		/// equivalent to this <see cref="BITMAPINFOHEADER"/> structure; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is BITMAPINFOHEADER) && (this == (BITMAPINFOHEADER)obj));
		}

		/// <summary>
		/// Returns a hash code for this <see cref="BITMAPINFOHEADER"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="BITMAPINFOHEADER"/>.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>BITMAPINFO</b> structure defines the dimensions and color information for a DIB.
	/// </summary>
	/// <remarks>
	/// A DIB consists of two distinct parts: a <b>BITMAPINFO</b> structure describing the dimensions
	/// and colors of the bitmap, and an array of bytes defining the pixels of the bitmap. The bits in
	/// the array are packed together, but each scan line must be padded with zeroes to end on a
	/// <b>LONG</b> data-type boundary. If the height of the bitmap is positive, the bitmap is a
	/// bottom-up DIB and its origin is the lower-left corner. If the height is negative, the bitmap is
	/// a top-down DIB and its origin is the upper left corner.
	/// <para/>
	/// A bitmap is packed when the bitmap array immediately follows the <b>BITMAPINFO</b> header.
	/// Packed bitmaps are referenced by a single pointer. For packed bitmaps, the <b>biClrUsed</b>
	/// member must be set to an even number when using the DIB_PAL_COLORS mode so that the DIB bitmap
	/// array starts on a <b>DWORD</b> boundary.
	/// <para/>
	/// <b>Note</b>  The <b>bmiColors</b> member should not contain palette indexes if the bitmap is to
	/// be stored in a file or transferred to another application.
	/// <para/>
	/// Unless the application has exclusive use and control of the bitmap, the bitmap color table
	/// should contain explicit RGB values.
	/// </remarks>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct BITMAPINFO : IEquatable<BITMAPINFO>
	{
		/// <summary>
		/// Specifies a <see cref="FreeImageAPI.BITMAPINFOHEADER"/> structure that contains information
		/// about the dimensions of color format.
		/// </summary>
		public BITMAPINFOHEADER bmiHeader;
		/// <summary>
		/// The <b>bmiColors</b> member contains one of the following:
		/// <list type="bullets">
		/// 
		/// <item>
		/// <term>
		/// An array of <see cref="FreeImageAPI.RGBQUAD"/>. The elements of the array that make up the
		/// color table.
		/// </term>
		/// </item>
		/// 
		/// <item>
		/// <term>
		/// An array of 16-bit unsigned integers that specifies indexes into the currently realized
		/// logical palette. This use of <b>bmiColors</b> is allowed for functions that use DIBs.
		/// When <b>bmiColors</b> elements contain indexes to a realized logical palette, they must
		/// also call the following bitmap functions:
		/// </term>
		/// </item>
		/// 
		/// </list>
		/// <b>CreateDIBitmap</b>
		/// <para/>
		/// <b>CreateDIBPatternBrush</b>
		/// <para/>
		/// <b>CreateDIBSection</b>
		/// <para/>
		/// The <i>iUsage</i> parameter of CreateDIBSection must be set to DIB_PAL_COLORS.
		/// <para/>
		/// The number of entries in the array depends on the values of the <b>biBitCount</b> and
		/// <b>biClrUsed</b> members of the <see cref="FreeImageAPI.BITMAPINFOHEADER"/> structure.
		/// <para/>
		/// The colors in the <b>bmiColors</b> table appear in order of importance. For more information,
		/// see the Remarks section.
		/// </summary>
		public RGBQUAD[] bmiColors;

		/// <summary>
		/// Tests whether two specified <see cref="BITMAPINFO"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="BITMAPINFO"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="BITMAPINFO"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="BITMAPINFO"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(BITMAPINFO left, BITMAPINFO right)
		{
			if (left.bmiHeader != right.bmiHeader)
			{
				return false;
			}
			if ((left.bmiColors == null) && (right.bmiColors == null))
			{
				return true;
			}
			if ((left.bmiColors == null) || (right.bmiColors == null))
			{
				return false;
			}
			if (left.bmiColors.Length != right.bmiColors.Length)
			{
				return false;
			}
			for (int i = 0; i < left.bmiColors.Length; i++)
			{
				if (left.bmiColors[i] != right.bmiColors[i])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Tests whether two specified <see cref="BITMAPINFO"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="BITMAPINFO"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="BITMAPINFO"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="BITMAPINFO"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(BITMAPINFO left, BITMAPINFO right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Tests whether the specified <see cref="BITMAPINFO"/> structure is equivalent to this <see cref="BITMAPINFO"/> structure.
		/// </summary>
		/// <param name="other">A <see cref="BITMAPINFO"/> structure to compare to this instance.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="BITMAPINFO"/> structure
		/// equivalent to this <see cref="BITMAPINFO"/> structure; otherwise, <b>false</b>.</returns>
		public bool Equals(BITMAPINFO other)
		{
			return (this == other);
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="BITMAPINFO"/> structure
		/// and is equivalent to this <see cref="BITMAPINFO"/> structure.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="BITMAPINFO"/> structure
		/// equivalent to this <see cref="BITMAPINFO"/> structure; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is BITMAPINFO) && (this == ((BITMAPINFO)obj)));
		}

		/// <summary>
		/// Returns a hash code for this <see cref="BITMAPINFO"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="BITMAPINFO"/>.</returns>
		public override int GetHashCode()
		{
			int hash = bmiHeader.GetHashCode();
			if (bmiColors != null)
			{
				for (int c = 0; c < bmiColors.Length; c++)
				{
					hash ^= bmiColors[c].GetHashCode();
					hash <<= 1;
				}
				hash <<= 1;
			}
			else
			{
				hash >>= 1;
			}
			return hash;
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FIBITMAP</b> structure is a handle to a FreeImage bimtap.
	/// </summary>
	/// <remarks>
	/// The handle represented by a <b>FIBITBAP</b> structure provides
	/// access to either a singlepage bitmap or exactly one page of
	/// a multipage bitmap.
	/// </remarks>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIBITMAP : IComparable, IComparable<FIBITMAP>, IEquatable<FIBITMAP>
	{
		private IntPtr data;

		/// <summary>
		/// A read-only field that represents a handle that has been initialized to zero.
		/// </summary>
		public static readonly FIBITMAP Zero;

		/// <summary>
		/// Tests whether two specified <see cref="FIBITMAP"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="FIBITMAP"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="FIBITMAP"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIBITMAP"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(FIBITMAP left, FIBITMAP right)
		{
			return (left.data == right.data);
		}

		/// <summary>
		/// Tests whether two specified <see cref="FIBITMAP"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="FIBITMAP"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="FIBITMAP"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIBITMAP"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(FIBITMAP left, FIBITMAP right)
		{
			return (left.data != right.data);
		}

		/// <summary>
		/// Gets whether the handle is a null or not.
		/// </summary>
		/// <value><b>true</b> if this <see cref="FIBITMAP"/> handle is a null;
		/// otherwise, <b>false</b>.</value>		
		public bool IsNull
		{
			get
			{
				return (data == IntPtr.Zero);
			}
		}

		/// <summary>
		/// Sets the handle to <i>null</i>.
		/// </summary>
		public void SetNull()
		{
			data = IntPtr.Zero;
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="FIBITMAP"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return data.ToString();
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FIBITMAP"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FIBITMAP"/>.</returns>
		public override int GetHashCode()
		{
			return data.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
		/// <returns><b>true</b> if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is FIBITMAP) && (this == ((FIBITMAP)obj)));
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns><b>true</b> if the current object is equal to the other parameter; otherwise, <b>false</b>.</returns>
		public bool Equals(FIBITMAP other)
		{
			return (this == other);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="FIBITMAP"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is FIBITMAP))
			{
				throw new ArgumentException();
			}
			return CompareTo((FIBITMAP)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="FIBITMAP"/> object.
		/// </summary>
		/// <param name="other">A <see cref="FIBITMAP"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(FIBITMAP other)
		{
			return this.data.ToInt64().CompareTo(other.data.ToInt64());
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FIMULTIBITMAP</b> structure is a handle to a FreeImage multipaged bimtap.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIMULTIBITMAP : IComparable, IComparable<FIMULTIBITMAP>, IEquatable<FIMULTIBITMAP>
	{
		private IntPtr data;

		/// <summary>
		/// A read-only field that represents a handle that has been initialized to zero.
		/// </summary>
		public static readonly FIMULTIBITMAP Zero;

		/// <summary>
		/// Tests whether two specified <see cref="FIMULTIBITMAP"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="FIMULTIBITMAP"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="FIMULTIBITMAP"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIMULTIBITMAP"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(FIMULTIBITMAP left, FIMULTIBITMAP right)
		{
			return (left.data == right.data);
		}

		/// <summary>
		/// Tests whether two specified <see cref="FIMULTIBITMAP"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="FIMULTIBITMAP"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="FIMULTIBITMAP"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIMULTIBITMAP"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(FIMULTIBITMAP left, FIMULTIBITMAP right)
		{
			return (left.data != right.data);
		}

		/// <summary>
		/// Gets whether the handle is a null or not.
		/// </summary>
		/// <value><b>true</b> if this <see cref="FIMULTIBITMAP"/> handle is a null;
		/// otherwise, <b>false</b>.</value>		
		public bool IsNull
		{
			get
			{
				return (data == IntPtr.Zero);
			}
		}

		/// <summary>
		/// Sets the handle to <i>null</i>.
		/// </summary>
		public void SetNull()
		{
			data = IntPtr.Zero;
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="FIMULTIBITMAP"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return data.ToString();
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FIMULTIBITMAP"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FIMULTIBITMAP"/>.</returns>
		public override int GetHashCode()
		{
			return data.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
		/// <returns><b>true</b> if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is FIMULTIBITMAP) && (this == ((FIMULTIBITMAP)obj)));
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns><b>true</b> if the current object is equal to the other parameter; otherwise, <b>false</b>.</returns>
		public bool Equals(FIMULTIBITMAP other)
		{
			return (this == other);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="FIMULTIBITMAP"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is FIMULTIBITMAP))
			{
				throw new ArgumentException();
			}
			return CompareTo((FIMULTIBITMAP)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="FIMULTIBITMAP"/> object.
		/// </summary>
		/// <param name="other">A <see cref="FIMULTIBITMAP"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(FIMULTIBITMAP other)
		{
			return this.data.ToInt64().CompareTo(other.data.ToInt64());
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FIMEMORY</b> structure is a handle to an opened memory stream.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIMEMORY : IComparable, IComparable<FIMEMORY>, IEquatable<FIMEMORY>
	{
		private IntPtr data;

		/// <summary>
		/// A read-only field that represents a handle that has been initialized to zero.
		/// </summary>
		public static readonly FIMEMORY Zero;

		/// <summary>
		/// Tests whether two specified <see cref="FIMEMORY"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="FIMEMORY"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="FIMEMORY"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIMEMORY"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(FIMEMORY left, FIMEMORY right)
		{
			return (left.data == right.data);
		}

		/// <summary>
		/// Tests whether two specified <see cref="FIMEMORY"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="FIMEMORY"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="FIMEMORY"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIMEMORY"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(FIMEMORY left, FIMEMORY right)
		{
			return (left.data != right.data);
		}

		/// <summary>
		/// Gets whether the pointer is a null pointer or not.
		/// </summary>
		/// <value><b>true</b> if this <see cref="FIMEMORY"/> is a null pointer;
		/// otherwise, <b>false</b>.</value>		
		public bool IsNull
		{
			get
			{
				return (data == IntPtr.Zero);
			}
		}

		/// <summary>
		/// Sets the handle to <i>null</i>.
		/// </summary>
		public void SetNull()
		{
			data = IntPtr.Zero;
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="FIMEMORY"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return data.ToString();
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FIMEMORY"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FIMEMORY"/>.</returns>
		public override int GetHashCode()
		{
			return data.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
		/// <returns><b>true</b> if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is FIMEMORY) && (this == ((FIMEMORY)obj)));
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns><b>true</b> if the current object is equal to the other parameter; otherwise, <b>false</b>.</returns>
		public bool Equals(FIMEMORY other)
		{
			return (this == other);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="FIMEMORY"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is FIMEMORY))
			{
				throw new ArgumentException();
			}
			return CompareTo((FIMEMORY)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="FIMEMORY"/> object.
		/// </summary>
		/// <param name="other">A <see cref="FIMEMORY"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(FIMEMORY other)
		{
			return this.data.ToInt64().CompareTo(other.data.ToInt64());
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FIMETADATA</b> structure is an unique search handle for metadata search operations.
	/// </summary>
	/// <remarks>
	/// The <b>FIMETADATA</b> structure is usually returned by the
	/// <see cref="FreeImageAPI.FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL, FIBITMAP, out FITAG)"/>
	/// function and then used on subsequent calls to
	/// <see cref="FreeImageAPI.FreeImage.FindNextMetadata(FIMETADATA, out FITAG)"/>.
	/// When the <b>FIMETADATA</b> handle is no longer used, it needs to be freed by the
	/// <see cref="FreeImageAPI.FreeImage.FindCloseMetadata(FIMETADATA)"/> function.
	/// </remarks>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIMETADATA : IComparable, IComparable<FIMETADATA>, IEquatable<FIMETADATA>
	{
		private IntPtr data;

		/// <summary>
		/// A read-only field that represents a handle that has been initialized to zero.
		/// </summary>
		public static readonly FIMETADATA Zero;

		/// <summary>
		/// Tests whether two specified <see cref="FIMETADATA"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="FIMETADATA"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="FIMETADATA"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIMETADATA"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(FIMETADATA left, FIMETADATA right)
		{
			return (left.data == right.data);
		}

		/// <summary>
		/// Tests whether two specified <see cref="FIMETADATA"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="FIMETADATA"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="FIMETADATA"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIMETADATA"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(FIMETADATA left, FIMETADATA right)
		{
			return (left.data != right.data);
		}

		/// <summary>
		/// Gets whether the pointer is a null pointer or not.
		/// </summary>
		/// <value><b>true</b> if this <see cref="FIMETADATA"/> is a null pointer;
		/// otherwise, <b>false</b>.</value>		
		public bool IsNull
		{
			get
			{
				return (data == IntPtr.Zero);
			}
		}

		/// <summary>
		/// Sets the handle to <i>null</i>.
		/// </summary>
		public void SetNull()
		{
			data = IntPtr.Zero;
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="FIMETADATA"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return data.ToString();
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FIMETADATA"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FIMETADATA"/>.</returns>
		public override int GetHashCode()
		{
			return data.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
		/// <returns><b>true</b> if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is FIMETADATA) && (this == ((FIMETADATA)obj)));
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns><b>true</b> if the current object is equal to the other parameter; otherwise, <b>false</b>.</returns>
		public bool Equals(FIMETADATA other)
		{
			return (this == other);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="FIMETADATA"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is FIMETADATA))
			{
				throw new ArgumentException();
			}
			return CompareTo((FIMETADATA)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="FIMETADATA"/> object.
		/// </summary>
		/// <param name="other">A <see cref="FIMETADATA"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(FIMETADATA other)
		{
			return this.data.ToInt64().CompareTo(other.data.ToInt64());
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FITAG</b> structure is a handle to a FreeImage metadata tag.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FITAG : IComparable, IComparable<FITAG>, IEquatable<FITAG>
	{
		private IntPtr data;

		/// <summary>
		/// A read-only field that represents a handle that has been initialized to zero.
		/// </summary>
		public static readonly FITAG Zero;

		/// <summary>
		/// Tests whether two specified <see cref="FITAG"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="FITAG"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="FITAG"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FITAG"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(FITAG left, FITAG right)
		{
			return (left.data == right.data);
		}

		/// <summary>
		/// Tests whether two specified <see cref="FITAG"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="FITAG"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="FITAG"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FITAG"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(FITAG left, FITAG right)
		{
			return (left.data != right.data);
		}

		/// <summary>
		/// Gets whether the pointer is a null pointer or not.
		/// </summary>
		/// <value><b>true</b> if this <see cref="FITAG"/> is a null pointer;
		/// otherwise, <b>false</b>.</value>		
		public bool IsNull
		{
			get
			{
				return (data == IntPtr.Zero);
			}
		}

		/// <summary>
		/// Sets the handle to <i>null</i>.
		/// </summary>
		public void SetNull()
		{
			data = IntPtr.Zero;
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="FITAG"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return data.ToString();
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FITAG"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FITAG"/>.</returns>
		public override int GetHashCode()
		{
			return data.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
		/// <returns><b>true</b> if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is FITAG) && (this == ((FITAG)obj)));
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns><b>true</b> if the current object is equal to the other parameter; otherwise, <b>false</b>.</returns>
		public bool Equals(FITAG other)
		{
			return (this == other);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="FITAG"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is FITAG))
			{
				throw new ArgumentException();
			}
			return CompareTo((FITAG)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="FITAG"/> object.
		/// </summary>
		/// <param name="other">A <see cref="FITAG"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(FITAG other)
		{
			return this.data.ToInt64().CompareTo(other.data.ToInt64());
		}
	}
}

namespace FreeImageAPI.IO
{
	/// <summary>
	/// Structure for implementing access to custom handles.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct FreeImageIO
	{
		/// <summary>
		/// Delegate to the C++ function <b>fread</b>.
		/// </summary>
		public ReadProc readProc;

		/// <summary>
		/// Delegate to the C++ function <b>fwrite</b>.
		/// </summary>
		public WriteProc writeProc;

		/// <summary>
		/// Delegate to the C++ function <b>fseek</b>.
		/// </summary>
		public SeekProc seekProc;

		/// <summary>
		/// Delegate to the C++ function <b>ftell</b>.
		/// </summary>
		public TellProc tellProc;
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>RGBQUAD</b> structure describes a color consisting of relative
	/// intensities of red, green, blue and alpha value. Each single color
	/// component consumes 8 bits and so, takes values in the range from 0 to 255.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <b>RGBQUAD</b> structure provides access to an underlying Win32 <b>RGBQUAD</b>
	/// structure. To determine the alpha, red, green or blue component of a color,
	/// use the rgbReserved, rgbRed, rgbGreen or rgbBlue fields, respectively.
	/// </para>
	/// <para>For easy integration of the underlying structure into the .NET framework,
	/// the <b>RGBQUAD</b> structure implements implicit conversion operators to 
	/// convert the represented color to and from the <see cref="System.Drawing.Color"/>
	/// type. This makes the <see cref="System.Drawing.Color"/> type a real replacement
	/// for the <b>RGBQUAD</b> structure and my be used in all situations which require
	/// an <b>RGBQUAD</b> type.
	/// </para>
	/// <para>
	/// Each color component rgbReserved, rgbRed, rgbGreen or rgbBlue of <b>RGBQUAD</b>
	/// is translated into it's corresponding color component A, R, G or B of
	/// <see cref="System.Drawing.Color"/> by an one-to-one manner and vice versa.
	/// </para>
	/// <para>
	/// <b>Conversion from System.Drawing.Color to RGBQUAD</b>
	/// </para>
	/// <c>RGBQUAD.component = Color.component</c>
	/// <para>
	/// <b>Conversion from RGBQUAD to System.Drawing.Color</b>
	/// </para>
	/// <c>Color.component = RGBQUAD.component</c>
	/// <para>
	/// The same conversion is also applied when the <see cref="FreeImageAPI.RGBQUAD.Color"/>
	/// property or the <see cref="FreeImageAPI.RGBQUAD(System.Drawing.Color)"/> constructor
	/// is invoked.
	/// </para>
	/// </remarks>
	/// <example>
	/// The following code example demonstrates the various conversions between the
	/// <b>RGBQUAD</b> structure and the <see cref="System.Drawing.Color"/> structure.
	/// <code>
	/// RGBQUAD rgbq;
	/// // Initialize the structure using a native .NET Color structure.
	///	rgbq = new RGBQUAD(Color.Indigo);
	/// // Initialize the structure using the implicit operator.
	///	rgbq = Color.DarkSeaGreen;
	/// // Convert the RGBQUAD instance into a native .NET Color
	/// // using its implicit operator.
	///	Color color = rgbq;
	/// // Using the structure's Color property for converting it
	/// // into a native .NET Color.
	///	Color another = rgbq.Color;
	/// </code>
	/// </example>
	[Serializable, StructLayout(LayoutKind.Explicit)]
	public struct RGBQUAD : IComparable, IComparable<RGBQUAD>, IEquatable<RGBQUAD>
	{
		/// <summary>
		/// The blue color component.
		/// </summary>
		[FieldOffset(0)]
		public byte rgbBlue;

		/// <summary>
		/// The green color component.
		/// </summary>
		[FieldOffset(1)]
		public byte rgbGreen;

		/// <summary>
		/// The red color component.
		/// </summary>
		[FieldOffset(2)]
		public byte rgbRed;

		/// <summary>
		/// The alpha color component.
		/// </summary>
		[FieldOffset(3)]
		public byte rgbReserved;

		/// <summary>
		/// The color's value.
		/// </summary>
		[FieldOffset(0)]
		public uint uintValue;

		/// <summary>
		/// Initializes a new instance based on the specified <see cref="System.Drawing.Color"/>.
		/// </summary>
		/// <param name="color"><see cref="System.Drawing.Color"/> to initialize with.</param>
		public RGBQUAD(Color color)
		{
			uintValue = 0u;
			rgbBlue = color.B;
			rgbGreen = color.G;
			rgbRed = color.R;
			rgbReserved = color.A;
		}

		/// <summary>
		/// Tests whether two specified <see cref="RGBQUAD"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="RGBQUAD"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="RGBQUAD"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="RGBQUAD"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(RGBQUAD left, RGBQUAD right)
		{
			return (left.uintValue == right.uintValue);
		}

		/// <summary>
		/// Tests whether two specified <see cref="RGBQUAD"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="RGBQUAD"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="RGBQUAD"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="RGBQUAD"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(RGBQUAD left, RGBQUAD right)
		{
			return (left.uintValue != right.uintValue);
		}

		/// <summary>
		/// Converts the value of a <see cref="System.Drawing.Color"/> structure to a <see cref="RGBQUAD"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="System.Drawing.Color"/> structure.</param>
		/// <returns>A new instance of <see cref="RGBQUAD"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator RGBQUAD(Color value)
		{
			return new RGBQUAD(value);
		}

		/// <summary>
		/// Converts the value of a <see cref="RGBQUAD"/> structure to a Color structure.
		/// </summary>
		/// <param name="value">A <see cref="RGBQUAD"/> structure.</param>
		/// <returns>A new instance of <see cref="System.Drawing.Color"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator Color(RGBQUAD value)
		{
			return value.Color;
		}

		/// <summary>
		/// Converts the value of an <see cref="UInt32"/> structure to a <see cref="RGBQUAD"/> structure.
		/// </summary>
		/// <param name="value">An <see cref="UInt32"/> structure.</param>
		/// <returns>A new instance of <see cref="RGBQUAD"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator RGBQUAD(uint value)
		{
			RGBQUAD result = new RGBQUAD();
			result.uintValue = value;
			return result;
		}

		/// <summary>
		/// Converts the value of a <see cref="RGBQUAD"/> structure to an <see cref="UInt32"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="RGBQUAD"/> structure.</param>
		/// <returns>A new instance of <see cref="RGBQUAD"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator uint(RGBQUAD value)
		{
			return value.uintValue;
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Drawing.Color"/> of the structure.
		/// </summary>
		public Color Color
		{
			get
			{
				return Color.FromArgb(
					rgbReserved,
					rgbRed,
					rgbGreen,
					rgbBlue);
			}
			set
			{
				rgbRed = value.R;
				rgbGreen = value.G;
				rgbBlue = value.B;
				rgbReserved = value.A;
			}
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="RGBQUAD"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is RGBQUAD))
			{
				throw new ArgumentException();
			}
			return CompareTo((RGBQUAD)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="RGBQUAD"/> object.
		/// </summary>
		/// <param name="other">A <see cref="RGBQUAD"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(RGBQUAD other)
		{
			return this.Color.ToArgb().CompareTo(other.Color.ToArgb());
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="RGBQUAD"/> structure
		/// and is equivalent to this <see cref="RGBQUAD"/> structure.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="RGBQUAD"/> structure
		/// equivalent to this <see cref="RGBQUAD"/> structure; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is RGBQUAD) && (this == ((RGBQUAD)obj)));
		}

		/// <summary>
		/// Tests whether the specified <see cref="RGBQUAD"/> structure is equivalent to this <see cref="RGBQUAD"/> structure.
		/// </summary>
		/// <param name="other">A <see cref="RGBQUAD"/> structure to compare to this instance.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="RGBQUAD"/> structure
		/// equivalent to this <see cref="RGBQUAD"/> structure; otherwise, <b>false</b>.</returns>
		public bool Equals(RGBQUAD other)
		{
			return (this == other);
		}

		/// <summary>
		/// Returns a hash code for this <see cref="RGBQUAD"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="RGBQUAD"/>.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="RGBQUAD"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return FreeImage.ColorToString(Color);
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>RGBTRIPLE</b> structure describes a color consisting of relative
	/// intensities of red, green and blue value. Each single color component
	/// consumes 8 bits and so, takes values in the range from 0 to 255.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <b>RGBTRIPLE</b> structure provides access to an underlying Win32 <b>RGBTRIPLE</b>
	/// structure. To determine the red, green or blue component of a color, use the
	/// rgbtRed, rgbtGreen or rgbtBlue fields, respectively.
	/// </para>
	/// <para>For easy integration of the underlying structure into the .NET framework,
	/// the <b>RGBTRIPLE</b> structure implements implicit conversion operators to 
	/// convert the represented color to and from the <see cref="System.Drawing.Color"/>
	/// type. This makes the <see cref="System.Drawing.Color"/> type a real replacement
	/// for the <b>RGBTRIPLE</b> structure and my be used in all situations which require
	/// an <b>RGBTRIPLE</b> type.
	/// </para>
	/// <para>
	/// Each of the color components rgbtRed, rgbtGreen or rgbtBlue of <b>RGBTRIPLE</b> is
	/// translated into it's corresponding color component R, G or B of
	/// <see cref="System.Drawing.Color"/> by an one-to-one manner and vice versa.
	/// When converting from <see cref="System.Drawing.Color"/> into <b>RGBTRIPLE</b>, the
	/// color's alpha value is ignored and assumed to be 255 when converting from
	/// <b>RGBTRIPLE</b> into <see cref="System.Drawing.Color"/>, creating a fully
	/// opaque color.
	/// </para>
	/// <para>
	/// <b>Conversion from System.Drawing.Color to RGBTRIPLE</b>
	/// </para>
	/// <c>RGBTRIPLE.component = Color.component</c>
	/// <para>
	/// <b>Conversion from RGBTRIPLE to System.Drawing.Color</b>
	/// </para>
	/// <c>Color.component = RGBTRIPLE.component</c>
	/// <para>
	/// The same conversion is also applied when the <see cref="FreeImageAPI.RGBTRIPLE.Color"/>
	/// property or the <see cref="FreeImageAPI.RGBTRIPLE(System.Drawing.Color)"/> constructor
	/// is invoked.
	/// </para>
	/// </remarks>
	/// <example>
	/// The following code example demonstrates the various conversions between the
	/// <b>RGBTRIPLE</b> structure and the <see cref="System.Drawing.Color"/> structure.
	/// <code>
	/// RGBTRIPLE rgbt;
	/// // Initialize the structure using a native .NET Color structure.
	///	rgbt = new RGBTRIPLE(Color.Indigo);
	/// // Initialize the structure using the implicit operator.
	///	rgbt = Color.DarkSeaGreen;
	/// // Convert the RGBTRIPLE instance into a native .NET Color
	/// // using its implicit operator.
	///	Color color = rgbt;
	/// // Using the structure's Color property for converting it
	/// // into a native .NET Color.
	///	Color another = rgbt.Color;
	/// </code>
	/// </example>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct RGBTRIPLE : IComparable, IComparable<RGBTRIPLE>, IEquatable<RGBTRIPLE>
	{
		/// <summary>
		/// The blue color component.
		/// </summary>
		public byte rgbtBlue;

		/// <summary>
		/// The green color component.
		/// </summary>
		public byte rgbtGreen;

		/// <summary>
		/// The red color component.
		/// </summary>
		public byte rgbtRed;

		/// <summary>
		/// Initializes a new instance based on the specified <see cref="System.Drawing.Color"/>.
		/// </summary>
		/// <param name="color"><see cref="System.Drawing.Color"/> to initialize with.</param>
		public RGBTRIPLE(Color color)
		{
			rgbtBlue = color.B;
			rgbtGreen = color.G;
			rgbtRed = color.R;
		}

		/// <summary>
		/// Tests whether two specified <see cref="RGBTRIPLE"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="RGBTRIPLE"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="RGBTRIPLE"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="RGBTRIPLE"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(RGBTRIPLE left, RGBTRIPLE right)
		{
			return
				left.rgbtBlue == right.rgbtBlue &&
				left.rgbtGreen == right.rgbtGreen &&
				left.rgbtRed == right.rgbtRed;
		}

		/// <summary>
		/// Tests whether two specified <see cref="RGBTRIPLE"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="RGBTRIPLE"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="RGBTRIPLE"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="RGBTRIPLE"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(RGBTRIPLE left, RGBTRIPLE right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Converts the value of a <see cref="System.Drawing.Color"/> structure to a <see cref="RGBTRIPLE"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="System.Drawing.Color"/> structure.</param>
		/// <returns>A new instance of <see cref="RGBTRIPLE"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator RGBTRIPLE(Color value)
		{
			return new RGBTRIPLE(value);
		}

		/// <summary>
		/// Converts the value of a <see cref="RGBTRIPLE"/> structure to a <see cref="System.Drawing.Color"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="RGBTRIPLE"/> structure.</param>
		/// <returns>A new instance of <see cref="System.Drawing.Color"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator Color(RGBTRIPLE value)
		{
			return value.Color;
		}

		/// <summary>
		/// Converts the value of an <see cref="UInt32"/> structure to a <see cref="RGBTRIPLE"/> structure.
		/// </summary>
		/// <param name="value">An <see cref="UInt32"/> structure.</param>
		/// <returns>A new instance of <see cref="RGBTRIPLE"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator RGBTRIPLE(uint value)
		{
			RGBTRIPLE result = new RGBTRIPLE();
			result.rgbtBlue = (byte)(value & 0xFF);
			result.rgbtGreen = (byte)((value >> 8) & 0xFF);
			result.rgbtRed = (byte)((value >> 16) & 0xFF);
			return result;
		}

		/// <summary>
		/// Converts the value of a <see cref="RGBTRIPLE"/> structure to an <see cref="UInt32"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="RGBTRIPLE"/> structure.</param>
		/// <returns>A new instance of <see cref="RGBTRIPLE"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator uint(RGBTRIPLE value)
		{
			return (uint)((value.rgbtRed << 16) | (value.rgbtGreen << 8) | (value.rgbtBlue));
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Drawing.Color"/> of the structure.
		/// </summary>
		public Color Color
		{
			get
			{
				return Color.FromArgb(
					rgbtRed,
					rgbtGreen,
					rgbtBlue);
			}
			set
			{
				rgbtBlue = value.B;
				rgbtGreen = value.G;
				rgbtRed = value.R;
			}
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="RGBTRIPLE"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is RGBTRIPLE))
			{
				throw new ArgumentException();
			}
			return CompareTo((RGBTRIPLE)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="RGBTRIPLE"/> object.
		/// </summary>
		/// <param name="other">A <see cref="RGBTRIPLE"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(RGBTRIPLE other)
		{
			return this.Color.ToArgb().CompareTo(other.Color.ToArgb());
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="RGBTRIPLE"/> structure
		/// and is equivalent to this <see cref="RGBTRIPLE"/> structure.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="RGBTRIPLE"/> structure
		/// equivalent to this <see cref="RGBTRIPLE"/> structure; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is RGBTRIPLE) && (this == ((RGBTRIPLE)obj)));
		}

		/// <summary>
		/// Tests whether the specified <see cref="RGBTRIPLE"/> structure is equivalent to this
		/// <see cref="RGBTRIPLE"/> structure.
		/// </summary>
		/// <param name="other">A <see cref="RGBTRIPLE"/> structure to compare to this instance.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="RGBTRIPLE"/> structure
		/// equivalent to this <see cref="RGBTRIPLE"/> structure; otherwise, <b>false</b>.</returns>
		public bool Equals(RGBTRIPLE other)
		{
			return (this == other);
		}

		/// <summary>
		/// Returns a hash code for this <see cref="RGBTRIPLE"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="RGBTRIPLE"/>.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="RGBTRIPLE"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return FreeImage.ColorToString(Color);
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FIRGBA16</b> structure describes a color consisting of relative
	/// intensities of red, green, blue and alpha value. Each single color
	/// component consumes 16 bits and so, takes values in the range from 0 to 65535.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <b>FIRGBA16</b> structure provides access to an underlying FreeImage <b>FIRGBA16</b>
	/// structure. To determine the alpha, red, green or blue component of a color,
	/// use the alpha, red, green or blue fields, respectively.
	/// </para>
	/// <para>For easy integration of the underlying structure into the .NET framework,
	/// the <b>FIRGBA16</b> structure implements implicit conversion operators to 
	/// convert the represented color to and from the <see cref="System.Drawing.Color"/>
	/// type. This makes the <see cref="System.Drawing.Color"/> type a real replacement
	/// for the <b>FIRGBA16</b> structure and my be used in all situations which require
	/// an <b>FIRGBA16</b> type.
	/// </para>
	/// <para>
	/// Each color component alpha, red, green or blue of <b>FIRGBA16</b>
	/// is translated into it's corresponding color component A, R, G or B of
	/// <see cref="System.Drawing.Color"/> by an 8 bit right shift and vice versa.
	/// </para>
	/// <para>
	/// <b>Conversion from System.Drawing.Color to FIRGBA16</b>
	/// </para>
	/// <c>FIRGBA16.component = Color.component &lt;&lt; 8</c>
	/// <para>
	/// <b>Conversion from FIRGBA16 to System.Drawing.Color</b>
	/// </para>
	/// <c>Color.component = FIRGBA16.component &gt;&gt; 8</c>
	/// <para>
	/// The same conversion is also applied when the <see cref="FreeImageAPI.FIRGBA16.Color"/>
	/// property or the <see cref="FreeImageAPI.FIRGBA16(System.Drawing.Color)"/> constructor
	/// is invoked.
	/// </para>
	/// </remarks>
	/// <example>
	/// The following code example demonstrates the various conversions between the
	/// <b>FIRGBA16</b> structure and the <see cref="System.Drawing.Color"/> structure.
	/// <code>
	/// FIRGBA16 firgba16;
	/// // Initialize the structure using a native .NET Color structure.
	///	firgba16 = new FIRGBA16(Color.Indigo);
	/// // Initialize the structure using the implicit operator.
	///	firgba16 = Color.DarkSeaGreen;
	/// // Convert the FIRGBA16 instance into a native .NET Color
	/// // using its implicit operator.
	///	Color color = firgba16;
	/// // Using the structure's Color property for converting it
	/// // into a native .NET Color.
	///	Color another = firgba16.Color;
	/// </code>
	/// </example>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIRGBA16 : IComparable, IComparable<FIRGBA16>, IEquatable<FIRGBA16>
	{
		/// <summary>
		/// The red color component.
		/// </summary>
		public ushort red;

		/// <summary>
		/// The green color component.
		/// </summary>
		public ushort green;

		/// <summary>
		/// The blue color component.
		/// </summary>
		public ushort blue;

		/// <summary>
		/// The alpha color component.
		/// </summary>
		public ushort alpha;

		/// <summary>
		/// Initializes a new instance based on the specified <see cref="System.Drawing.Color"/>.
		/// </summary>
		/// <param name="color"><see cref="System.Drawing.Color"/> to initialize with.</param>
		public FIRGBA16(Color color)
		{
			red = (ushort)(color.R << 8);
			green = (ushort)(color.G << 8);
			blue = (ushort)(color.B << 8);
			alpha = (ushort)(color.A << 8);
		}

		/// <summary>
		/// Tests whether two specified <see cref="FIRGBA16"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="FIRGBA16"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="FIRGBA16"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIRGBA16"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(FIRGBA16 left, FIRGBA16 right)
		{
			return
				((left.alpha == right.alpha) &&
				(left.blue == right.blue) &&
				(left.green == right.green) &&
				(left.red == right.red));
		}

		/// <summary>
		/// Tests whether two specified <see cref="FIRGBA16"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="FIRGBA16"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="FIRGBA16"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIRGBA16"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(FIRGBA16 left, FIRGBA16 right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Converts the value of a <see cref="System.Drawing.Color"/> structure to a <see cref="FIRGBA16"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="System.Drawing.Color"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRGBA16"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIRGBA16(Color value)
		{
			return new FIRGBA16(value);
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRGBA16"/> structure to a <see cref="System.Drawing.Color"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRGBA16"/> structure.</param>
		/// <returns>A new instance of <see cref="System.Drawing.Color"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator Color(FIRGBA16 value)
		{
			return value.Color;
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Drawing.Color"/> of the structure.
		/// </summary>
		public Color Color
		{
			get
			{
				return Color.FromArgb((alpha >> 8), (red >> 8), (green >> 8), (blue >> 8));
			}
			set
			{
				red = (ushort)(value.R << 8);
				green = (ushort)(value.G << 8);
				blue = (ushort)(value.B << 8);
				alpha = (ushort)(value.A << 8);
			}
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="FIRGBA16"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is FIRGBA16))
			{
				throw new ArgumentException();
			}
			return CompareTo((FIRGBA16)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="FIRGBA16"/> object.
		/// </summary>
		/// <param name="other">A <see cref="FIRGBA16"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(FIRGBA16 other)
		{
			return this.Color.ToArgb().CompareTo(other.Color.ToArgb());
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="FIRGBA16"/> structure
		/// and is equivalent to this <see cref="FIRGBA16"/> structure.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FIRGBA16"/> structure
		/// equivalent to this <see cref="FIRGBA16"/> structure; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is FIRGBA16) && (this == ((FIRGBA16)obj)));
		}

		/// <summary>
		/// Tests whether the specified <see cref="FIRGBA16"/> structure is equivalent to this <see cref="FIRGBA16"/> structure.
		/// </summary>
		/// <param name="other">A <see cref="FIRGBA16"/> structure to compare to this instance.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FIRGBA16"/> structure
		/// equivalent to this <see cref="FIRGBA16"/> structure; otherwise, <b>false</b>.</returns>
		public bool Equals(FIRGBA16 other)
		{
			return (this == other);
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FIRGBA16"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FIRGBA16"/>.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="FIRGBA16"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return FreeImage.ColorToString(Color);
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FIRGB16</b> structure describes a color consisting of relative
	/// intensities of red, green, blue and alpha value. Each single color
	/// component consumes 16 bits and so, takes values in the range from 0 to 65535.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <b>FIRGB16</b> structure provides access to an underlying FreeImage <b>FIRGB16</b>
	/// structure. To determine the red, green or blue component of a color,
	/// use the red, green or blue fields, respectively.
	/// </para>
	/// <para>For easy integration of the underlying structure into the .NET framework,
	/// the <b>FIRGB16</b> structure implements implicit conversion operators to 
	/// convert the represented color to and from the <see cref="System.Drawing.Color"/>
	/// type. This makes the <see cref="System.Drawing.Color"/> type a real replacement
	/// for the <b>FIRGB16</b> structure and my be used in all situations which require
	/// an <b>FIRGB16</b> type.
	/// </para>
	/// <para>
	/// Each color component red, green or blue of <b>FIRGB16</b> is translated into
	/// it's corresponding color component R, G or B of
	/// <see cref="System.Drawing.Color"/> by right shifting 8 bits and shifting left 8 bits for the reverse conversion.
	/// When converting from <see cref="System.Drawing.Color"/> into <b>FIRGB16</b>, the
	/// color's alpha value is ignored and assumed to be 255 when converting from
	/// <b>FIRGB16</b> into <see cref="System.Drawing.Color"/>, creating a fully
	/// opaque color.
	/// </para>
	/// <para>
	/// <b>Conversion from System.Drawing.Color to FIRGB16</b>
	/// </para>
	/// <c>FIRGB16.component = Color.component &lt;&lt; 8</c>
	/// <para>
	/// <b>Conversion from FIRGB16 to System.Drawing.Color</b>
	/// </para>
	/// <c>Color.component = FIRGB16.component &gt;&gt; 8</c>
	/// <para>
	/// The same conversion is also applied when the <see cref="FreeImageAPI.FIRGB16.Color"/>
	/// property or the <see cref="FreeImageAPI.FIRGB16(System.Drawing.Color)"/> constructor
	/// is invoked.
	/// </para>
	/// </remarks>
	/// <example>
	/// The following code example demonstrates the various conversions between the
	/// <b>FIRGB16</b> structure and the <see cref="System.Drawing.Color"/> structure.
	/// <code>
	/// FIRGB16 firgb16;
	/// // Initialize the structure using a native .NET Color structure.
	///	firgb16 = new FIRGBA16(Color.Indigo);
	/// // Initialize the structure using the implicit operator.
	///	firgb16 = Color.DarkSeaGreen;
	/// // Convert the FIRGB16 instance into a native .NET Color
	/// // using its implicit operator.
	///	Color color = firgb16;
	/// // Using the structure's Color property for converting it
	/// // into a native .NET Color.
	///	Color another = firgb16.Color;
	/// </code>
	/// </example>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIRGB16 : IComparable, IComparable<FIRGB16>, IEquatable<FIRGB16>
	{
		/// <summary>
		/// The red color component.
		/// </summary>
		public ushort red;

		/// <summary>
		/// The green color component.
		/// </summary>
		public ushort green;

		/// <summary>
		/// The blue color component.
		/// </summary>
		public ushort blue;

		/// <summary>
		/// Initializes a new instance based on the specified <see cref="System.Drawing.Color"/>.
		/// </summary>
		/// <param name="color"><see cref="System.Drawing.Color"/> to initialize with.</param>
		public FIRGB16(Color color)
		{
			red = (ushort)(color.R << 8);
			green = (ushort)(color.G << 8);
			blue = (ushort)(color.B << 8);
		}

		/// <summary>
		/// Tests whether two specified <see cref="FIRGB16"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="FIRGB16"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="FIRGB16"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIRGB16"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(FIRGB16 left, FIRGB16 right)
		{
			return
				((left.blue == right.blue) &&
				(left.green == right.green) &&
				(left.red == right.red));
		}

		/// <summary>
		/// Tests whether two specified <see cref="FIRGB16"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="FIRGB16"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="FIRGB16"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIRGB16"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(FIRGB16 left, FIRGB16 right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Converts the value of a <see cref="System.Drawing.Color"/> structure to a <see cref="FIRGB16"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="System.Drawing.Color"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRGB16"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIRGB16(Color value)
		{
			return new FIRGB16(value);
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRGB16"/> structure to a <see cref="System.Drawing.Color"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRGB16"/> structure.</param>
		/// <returns>A new instance of <see cref="System.Drawing.Color"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator Color(FIRGB16 value)
		{
			return value.Color;
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Drawing.Color"/> of the structure.
		/// </summary>
		public Color Color
		{
			get
			{
				return Color.FromArgb((red >> 8), (green >> 8), (blue >> 8));
			}
			set
			{
				red = (ushort)(value.R << 8);
				green = (ushort)(value.G << 8);
				blue = (ushort)(value.B << 8);
			}
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="FIRGB16"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is FIRGB16))
			{
				throw new ArgumentException();
			}
			return CompareTo((FIRGB16)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="FIRGB16"/> object.
		/// </summary>
		/// <param name="other">A <see cref="FIRGB16"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(FIRGB16 other)
		{
			return this.Color.ToArgb().CompareTo(other.Color.ToArgb());
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="FIRGB16"/> structure
		/// and is equivalent to this <see cref="FIRGB16"/> structure.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FIRGB16"/> structure
		/// equivalent to this <see cref="FIRGB16"/> structure; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is FIRGB16) && (this == ((FIRGB16)obj)));
		}

		/// <summary>
		/// Tests whether the specified <see cref="FIRGB16"/> structure is equivalent to this <see cref="FIRGB16"/> structure.
		/// </summary>
		/// <param name="other">A <see cref="FIRGB16"/> structure to compare to this instance.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FIRGB16"/> structure
		/// equivalent to this <see cref="FIRGB16"/> structure; otherwise, <b>false</b>.</returns>
		public bool Equals(FIRGB16 other)
		{
			return (this == other);
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FIRGB16"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FIRGB16"/>.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="FIRGB16"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return FreeImage.ColorToString(Color);
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FIRGBAF</b> structure describes a color consisting of relative
	/// intensities of red, green, blue and alpha value. Each single color
	/// component consumes 32 bits and takes values in the range from 0 to 1.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <b>FIRGBAF</b> structure provides access to an underlying FreeImage <b>FIRGBAF</b>
	/// structure. To determine the alpha, red, green or blue component of a color,
	/// use the alpha, red, green or blue fields, respectively.
	/// </para>
	/// <para>For easy integration of the underlying structure into the .NET framework,
	/// the <b>FIRGBAF</b> structure implements implicit conversion operators to 
	/// convert the represented color to and from the <see cref="System.Drawing.Color"/>
	/// type. This makes the <see cref="System.Drawing.Color"/> type a real replacement
	/// for the <b>FIRGBAF</b> structure and my be used in all situations which require
	/// an <b>FIRGBAF</b> type.
	/// </para>
	/// <para>
	/// Each color component alpha, red, green or blue of <b>FIRGBAF</b> is translated
	/// into it's corresponding color component A, R, G or B of
	/// <see cref="System.Drawing.Color"/> by linearly mapping the values of one range
	/// into the other range and vice versa.
	/// </para>
	/// <para>
	/// <b>Conversion from System.Drawing.Color to FIRGBAF</b>
	/// </para>
	/// <c>FIRGBAF.component = (float)Color.component / 255f</c>
	/// <para>
	/// <b>Conversion from FIRGBAF to System.Drawing.Color</b>
	/// </para>
	/// <c>Color.component = (int)(FIRGBAF.component * 255f)</c>
	/// <para>
	/// The same conversion is also applied when the <see cref="FreeImageAPI.FIRGBAF.Color"/>
	/// property or the <see cref="FreeImageAPI.FIRGBAF(System.Drawing.Color)"/> constructor
	/// is invoked.
	/// </para>
	/// </remarks>
	/// <example>
	/// The following code example demonstrates the various conversions between the
	/// <b>FIRGBAF</b> structure and the <see cref="System.Drawing.Color"/> structure.
	/// <code>
	/// FIRGBAF firgbaf;
	/// // Initialize the structure using a native .NET Color structure.
	///	firgbaf = new FIRGBAF(Color.Indigo);
	/// // Initialize the structure using the implicit operator.
	///	firgbaf = Color.DarkSeaGreen;
	/// // Convert the FIRGBAF instance into a native .NET Color
	/// // using its implicit operator.
	///	Color color = firgbaf;
	/// // Using the structure's Color property for converting it
	/// // into a native .NET Color.
	///	Color another = firgbaf.Color;
	/// </code>
	/// </example>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIRGBAF : IComparable, IComparable<FIRGBAF>, IEquatable<FIRGBAF>
	{
		/// <summary>
		/// The red color component.
		/// </summary>
		public float red;

		/// <summary>
		/// The green color component.
		/// </summary>
		public float green;

		/// <summary>
		/// The blue color component.
		/// </summary>
		public float blue;

		/// <summary>
		/// The alpha color component.
		/// </summary>
		public float alpha;

		/// <summary>
		/// Initializes a new instance based on the specified <see cref="System.Drawing.Color"/>.
		/// </summary>
		/// <param name="color"><see cref="System.Drawing.Color"/> to initialize with.</param>
		public FIRGBAF(Color color)
		{
			red = (float)color.R / 255f;
			green = (float)color.G / 255f;
			blue = (float)color.B / 255f;
			alpha = (float)color.A / 255f;
		}

		/// <summary>
		/// Tests whether two specified <see cref="FIRGBAF"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="FIRGBAF"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="FIRGBAF"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIRGBAF"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(FIRGBAF left, FIRGBAF right)
		{
			return
				((left.alpha == right.alpha) &&
				(left.blue == right.blue) &&
				(left.green == right.green) &&
				(left.red == right.red));
		}

		/// <summary>
		/// Tests whether two specified <see cref="FIRGBAF"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="FIRGBAF"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="FIRGBAF"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIRGBAF"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(FIRGBAF left, FIRGBAF right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Converts the value of a <see cref="System.Drawing.Color"/> structure to a <see cref="FIRGBAF"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="System.Drawing.Color"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRGBAF"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIRGBAF(Color value)
		{
			return new FIRGBAF(value);
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRGBAF"/> structure to a <see cref="System.Drawing.Color"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRGBAF"/> structure.</param>
		/// <returns>A new instance of <see cref="System.Drawing.Color"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator Color(FIRGBAF value)
		{
			return value.Color;
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Drawing.Color"/> of the structure.
		/// </summary>
		public Color Color
		{
			get
			{
				return Color.FromArgb(
					(int)(alpha * 255f),
					(int)(red * 255f),
					(int)(green * 255f),
					(int)(blue * 255f));
			}
			set
			{
				red = (float)value.R / 255f;
				green = (float)value.G / 255f;
				blue = (float)value.B / 255f;
				alpha = (float)value.A / 255f;
			}
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="FIRGBAF"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is FIRGBAF))
			{
				throw new ArgumentException();
			}
			return CompareTo((FIRGBAF)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="FIRGBAF"/> object.
		/// </summary>
		/// <param name="other">A <see cref="FIRGBAF"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(FIRGBAF other)
		{
			return this.Color.ToArgb().CompareTo(other.Color.ToArgb());
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="FIRGBAF"/> structure
		/// and is equivalent to this <see cref="FIRGBAF"/> structure.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FIRGBAF"/> structure
		/// equivalent to this <see cref="FIRGBAF"/> structure; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is FIRGBAF) && (this == ((FIRGBAF)obj)));
		}

		/// <summary>
		/// Tests whether the specified <see cref="FIRGBAF"/> structure is equivalent to this <see cref="FIRGBAF"/> structure.
		/// </summary>
		/// <param name="other">A <see cref="FIRGBAF"/> structure to compare to this instance.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FIRGBAF"/> structure
		/// equivalent to this <see cref="FIRGBAF"/> structure; otherwise, <b>false</b>.</returns>
		public bool Equals(FIRGBAF other)
		{
			return (this == other);
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FIRGBAF"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FIRGBAF"/>.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="FIRGBAF"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return FreeImage.ColorToString(Color);
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FIRGBF</b> structure describes a color consisting of relative
	/// intensities of red, green, blue and alpha value. Each single color
	/// component consumes 32 bits and takes values in the range from 0 to 1.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <b>FIRGBF</b> structure provides access to an underlying FreeImage <b>FIRGBF</b>
	/// structure. To determine the red, green or blue component of a color, use the
	/// red, green or blue fields, respectively.
	/// </para>
	/// <para>For easy integration of the underlying structure into the .NET framework,
	/// the <b>FIRGBF</b> structure implements implicit conversion operators to 
	/// convert the represented color to and from the <see cref="System.Drawing.Color"/>
	/// type. This makes the <see cref="System.Drawing.Color"/> type a real replacement
	/// for the <b>FIRGBF</b> structure and my be used in all situations which require
	/// an <b>FIRGBF</b> type.
	/// </para>
	/// <para>
	/// Each color component alpha, red, green or blue of <b>FIRGBF</b> is translated
	/// into it's corresponding color component A, R, G or B of
	/// <see cref="System.Drawing.Color"/> by linearly mapping the values of one range
	/// into the other range and vice versa.
	/// When converting from <see cref="System.Drawing.Color"/> into <b>FIRGBF</b>, the
	/// color's alpha value is ignored and assumed to be 255 when converting from
	/// <b>FIRGBF</b> into <see cref="System.Drawing.Color"/>, creating a fully
	/// opaque color.
	/// </para>
	/// <para>
	/// <b>Conversion from System.Drawing.Color to FIRGBF</b>
	/// </para>
	/// <c>FIRGBF.component = (float)Color.component / 255f</c>
	/// <para>
	/// <b>Conversion from FIRGBF to System.Drawing.Color</b>
	/// </para>
	/// <c>Color.component = (int)(FIRGBF.component * 255f)</c>
	/// <para>
	/// The same conversion is also applied when the <see cref="FreeImageAPI.FIRGBF.Color"/>
	/// property or the <see cref="FreeImageAPI.FIRGBF(System.Drawing.Color)"/> constructor
	/// is invoked.
	/// </para>
	/// </remarks>
	/// <example>
	/// The following code example demonstrates the various conversions between the
	/// <b>FIRGBF</b> structure and the <see cref="System.Drawing.Color"/> structure.
	/// <code>
	/// FIRGBF firgbf;
	/// // Initialize the structure using a native .NET Color structure.
	///	firgbf = new FIRGBF(Color.Indigo);
	/// // Initialize the structure using the implicit operator.
	///	firgbf = Color.DarkSeaGreen;
	/// // Convert the FIRGBF instance into a native .NET Color
	/// // using its implicit operator.
	///	Color color = firgbf;
	/// // Using the structure's Color property for converting it
	/// // into a native .NET Color.
	///	Color another = firgbf.Color;
	/// </code>
	/// </example>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIRGBF : IComparable, IComparable<FIRGBF>, IEquatable<FIRGBF>
	{
		/// <summary>
		/// The red color component.
		/// </summary>
		public float red;

		/// <summary>
		/// The green color component.
		/// </summary>
		public float green;

		/// <summary>
		/// The blue color component.
		/// </summary>
		public float blue;

		/// <summary>
		/// Initializes a new instance based on the specified <see cref="System.Drawing.Color"/>.
		/// </summary>
		/// <param name="color"><see cref="System.Drawing.Color"/> to initialize with.</param>
		public FIRGBF(Color color)
		{
			red = (float)color.R / 255f;
			green = (float)color.G / 255f;
			blue = (float)color.B / 255f;
		}

		/// <summary>
		/// Tests whether two specified <see cref="FIRGBF"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="FIRGBF"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="FIRGBF"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIRGBF"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(FIRGBF left, FIRGBF right)
		{
			return
				((left.blue == right.blue) &&
				(left.green == right.green) &&
				(left.red == right.red));
		}

		/// <summary>
		/// Tests whether two specified <see cref="FIRGBF"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="FIRGBF"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="FIRGBF"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FIRGBF"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(FIRGBF left, FIRGBF right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Converts the value of a <see cref="System.Drawing.Color"/> structure to a <see cref="FIRGBF"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="System.Drawing.Color"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRGBF"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIRGBF(Color value)
		{
			return new FIRGBF(value);
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRGBF"/> structure to a <see cref="System.Drawing.Color"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRGBF"/> structure.</param>
		/// <returns>A new instance of <see cref="System.Drawing.Color"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator Color(FIRGBF value)
		{
			return value.Color;
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Drawing.Color"/> of the structure.
		/// </summary>
		public Color Color
		{
			get
			{
				return Color.FromArgb(
					(int)(red * 255f),
					(int)(green * 255f),
					(int)(blue * 255f));
			}
			set
			{
				red = (float)value.R / 255f;
				green = (float)value.G / 255f;
				blue = (float)value.B / 255f;
			}
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="FIRGBF"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is FIRGBF))
			{
				throw new ArgumentException();
			}
			return CompareTo((FIRGBF)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="FIRGBF"/> object.
		/// </summary>
		/// <param name="other">A <see cref="FIRGBF"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(FIRGBF other)
		{
			return this.Color.ToArgb().CompareTo(other.Color.ToArgb());
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="FIRGBF"/> structure
		/// and is equivalent to this <see cref="FIRGBF"/> structure.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FIRGBF"/> structure
		/// equivalent to this <see cref="FIRGBF"/> structure; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is FIRGBF) && (this == ((FIRGBF)obj)));
		}

		/// <summary>
		/// Tests whether the specified <see cref="FIRGBF"/> structure is equivalent to this <see cref="FIRGBF"/> structure.
		/// </summary>
		/// <param name="other">A <see cref="FIRGBF"/> structure to compare to this instance.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FIRGBF"/> structure
		/// equivalent to this <see cref="FIRGBF"/> structure; otherwise, <b>false</b>.</returns>
		public bool Equals(FIRGBF other)
		{
			return (this == other);
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FIRGBF"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FIRGBF"/>.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}


		/// <summary>
		/// Converts the numeric value of the <see cref="FIRGBF"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return FreeImage.ColorToString(Color);
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FICOMPLEX</b> structure describes a color consisting of a real and an imaginary part.
	/// Each part is using 4 bytes of data.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FICOMPLEX : IComparable, IComparable<FICOMPLEX>, IEquatable<FICOMPLEX>
	{
		/// <summary>
		/// Real part of the color.
		/// </summary>
		public double real;

		/// <summary>
		/// Imaginary part of the color.
		/// </summary>
		public double imag;

		/// <summary>
		/// Tests whether two specified <see cref="FICOMPLEX"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="FICOMPLEX"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="FICOMPLEX"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FICOMPLEX"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(FICOMPLEX left, FICOMPLEX right)
		{
			return ((left.real == right.real) && (left.imag == right.imag));
		}

		/// <summary>
		/// Tests whether two specified <see cref="FICOMPLEX"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="FICOMPLEX"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="FICOMPLEX"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FICOMPLEX"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(FICOMPLEX left, FICOMPLEX right)
		{
			return ((left.real != right.real) || (left.imag == right.imag));
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="FICOMPLEX"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is FICOMPLEX))
			{
				throw new ArgumentException();
			}
			return CompareTo((FICOMPLEX)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="FICOMPLEX"/> object.
		/// </summary>
		/// <param name="other">A <see cref="FICOMPLEX"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(FICOMPLEX other)
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="FICOMPLEX"/> structure
		/// and is equivalent to this <see cref="FICOMPLEX"/> structure.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FICOMPLEX"/> structure
		/// equivalent to this <see cref="FICOMPLEX"/> structure; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is FICOMPLEX) && (this == ((FICOMPLEX)obj)));
		}

		/// <summary>
		/// Tests whether the specified <see cref="FICOMPLEX"/> structure is equivalent to this <see cref="FICOMPLEX"/> structure.
		/// </summary>
		/// <param name="other">A <see cref="FICOMPLEX"/> structure to compare to this instance.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FICOMPLEX"/> structure
		/// equivalent to this <see cref="FICOMPLEX"/> structure; otherwise, <b>false</b>.</returns>
		public bool Equals(FICOMPLEX other)
		{
			return (this == other);
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FICOMPLEX"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FICOMPLEX"/>.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// This Structure contains ICC-Profile data.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIICCPROFILE
	{
		private ICC_FLAGS flags;
		private uint size;
		private IntPtr data;

		/// <summary>
		/// Creates a new ICC-Profile for <paramref name="dib"/>.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="data">The ICC-Profile data.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public FIICCPROFILE(FIBITMAP dib, byte[] data)
			: this(dib, data, (int)data.Length)
		{
		}

		/// <summary>
		/// Creates a new ICC-Profile for <paramref name="dib"/>.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="data">The ICC-Profile data.</param>
		/// <param name="size">Number of bytes to use from data.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public unsafe FIICCPROFILE(FIBITMAP dib, byte[] data, int size)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			FIICCPROFILE prof;
			size = Math.Min(size, (int)data.Length);
			prof = *(FIICCPROFILE*)FreeImage.CreateICCProfile(dib, data, size);
			this.flags = prof.flags;
			this.size = prof.size;
			this.data = prof.data;
		}

		/// <summary>
		/// Info flag of the profile.
		/// </summary>
		public ICC_FLAGS Flags
		{
			get { return flags; }
		}

		/// <summary>
		/// Profile's size measured in bytes.
		/// </summary>
		public uint Size
		{
			get { return size; }
		}

		/// <summary>
		/// Points to a block of contiguous memory containing the profile.
		/// </summary>
		public IntPtr DataPointer
		{
			get { return data; }
		}

		/// <summary>
		/// Copy of the ICC-Profiles data.
		/// </summary>
		public unsafe byte[] Data
		{
			get
			{
				byte[] result = new byte[size];
				fixed (byte* dst = result)
				{
					FreeImage.CopyMemory(dst, data.ToPointer(), size);
				}
				return result;
			}
		}

		/// <summary>
		/// Indicates whether the profile is CMYK.
		/// </summary>
		public bool IsCMYK
		{
			get
			{
				return ((flags & ICC_FLAGS.FIICC_COLOR_IS_CMYK) != 0);
			}
		}
	}
}

namespace FreeImageAPI.Plugins
{
	/// <summary>
	/// The structure contains functionpointers that make up a FreeImage plugin.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct Plugin
	{
		/// <summary>
		/// Delegate to a function that returns a string which describes
		/// the plugins format.
		/// </summary>
		public FormatProc formatProc;

		/// <summary>
		/// Delegate to a function that returns a string which contains
		/// a more detailed description.
		/// </summary>
		public DescriptionProc descriptionProc;

		/// <summary>
		/// Delegate to a function that returns a comma seperated list
		/// of file extensions the plugin can read or write.
		/// </summary>
		public ExtensionListProc extensionListProc;

		/// <summary>
		/// Delegate to a function that returns a regular expression that
		/// can be used to idientify whether a file can be handled by the plugin.
		/// </summary>
		public RegExprProc regExprProc;

		/// <summary>
		/// Delegate to a function that opens a file.
		/// </summary>
		public OpenProc openProc;

		/// <summary>
		/// Delegate to a function that closes a previosly opened file.
		/// </summary>
		public CloseProc closeProc;

		/// <summary>
		/// Delegate to a function that returns the number of pages of a multipage
		/// bitmap if the plugin is capable of handling multipage bitmaps.
		/// </summary>
		public PageCountProc pageCountProc;

		/// <summary>
		/// UNKNOWN
		/// </summary>
		public PageCapabilityProc pageCapabilityProc;

		/// <summary>
		/// Delegate to a function that loads and decodes a bitmap into memory.
		/// </summary>
		public LoadProc loadProc;

		/// <summary>
		///  Delegate to a function that saves a bitmap.
		/// </summary>
		public SaveProc saveProc;

		/// <summary>
		/// Delegate to a function that determines whether the source is a valid image.
		/// </summary>
		public ValidateProc validateProc;

		/// <summary>
		/// Delegate to a function that returns a string which contains
		/// the plugin's mime type.
		/// </summary>
		public MimeProc mimeProc;

		/// <summary>
		/// Delegate to a function that returns whether the plugin can handle the
		/// specified color depth.
		/// </summary>
		public SupportsExportBPPProc supportsExportBPPProc;

		/// <summary>
		/// Delegate to a function that returns whether the plugin can handle the
		/// specified image type.
		/// </summary>
		public SupportsExportTypeProc supportsExportTypeProc;

		/// <summary>
		/// Delegate to a function that returns whether the plugin can handle
		/// ICC-Profiles.
		/// </summary>
		public SupportsICCProfilesProc supportsICCProfilesProc;
	}
}

	#endregion

	#region Enums

namespace FreeImageAPI
{
	/// <summary>
	/// I/O image format identifiers.
	/// </summary>
	public enum FREE_IMAGE_FORMAT
	{
		/// <summary>
		/// Unknown format (returned value only, never use it as input value)
		/// </summary>
		FIF_UNKNOWN = -1,
		/// <summary>
		/// Windows or OS/2 Bitmap File (*.BMP)
		/// </summary>
		FIF_BMP = 0,
		/// <summary>
		/// Windows Icon (*.ICO)
		/// </summary>
		FIF_ICO = 1,
		/// <summary>
		/// Independent JPEG Group (*.JPG, *.JIF, *.JPEG, *.JPE)
		/// </summary>
		FIF_JPEG = 2,
		/// <summary>
		/// JPEG Network Graphics (*.JNG)
		/// </summary>
		FIF_JNG = 3,
		/// <summary>
		/// Commodore 64 Koala format (*.KOA)
		/// </summary>
		FIF_KOALA = 4,
		/// <summary>
		/// Amiga IFF (*.IFF, *.LBM)
		/// </summary>
		FIF_LBM = 5,
		/// <summary>
		/// Amiga IFF (*.IFF, *.LBM)
		/// </summary>
		FIF_IFF = 5,
		/// <summary>
		/// Multiple Network Graphics (*.MNG)
		/// </summary>
		FIF_MNG = 6,
		/// <summary>
		/// Portable Bitmap (ASCII) (*.PBM)
		/// </summary>
		FIF_PBM = 7,
		/// <summary>
		/// Portable Bitmap (BINARY) (*.PBM)
		/// </summary>
		FIF_PBMRAW = 8,
		/// <summary>
		/// Kodak PhotoCD (*.PCD)
		/// </summary>
		FIF_PCD = 9,
		/// <summary>
		/// Zsoft Paintbrush PCX bitmap format (*.PCX)
		/// </summary>
		FIF_PCX = 10,
		/// <summary>
		/// Portable Graymap (ASCII) (*.PGM)
		/// </summary>
		FIF_PGM = 11,
		/// <summary>
		/// Portable Graymap (BINARY) (*.PGM)
		/// </summary>
		FIF_PGMRAW = 12,
		/// <summary>
		/// Portable Network Graphics (*.PNG)
		/// </summary>
		FIF_PNG = 13,
		/// <summary>
		/// Portable Pixelmap (ASCII) (*.PPM)
		/// </summary>
		FIF_PPM = 14,
		/// <summary>
		/// Portable Pixelmap (BINARY) (*.PPM)
		/// </summary>
		FIF_PPMRAW = 15,
		/// <summary>
		/// Sun Rasterfile (*.RAS)
		/// </summary>
		FIF_RAS = 16,
		/// <summary>
		/// truevision Targa files (*.TGA, *.TARGA)
		/// </summary>
		FIF_TARGA = 17,
		/// <summary>
		/// Tagged Image File Format (*.TIF, *.TIFF)
		/// </summary>
		FIF_TIFF = 18,
		/// <summary>
		/// Wireless Bitmap (*.WBMP)
		/// </summary>
		FIF_WBMP = 19,
		/// <summary>
		/// Adobe Photoshop (*.PSD)
		/// </summary>
		FIF_PSD = 20,
		/// <summary>
		/// Dr. Halo (*.CUT)
		/// </summary>
		FIF_CUT = 21,
		/// <summary>
		/// X11 Bitmap Format (*.XBM)
		/// </summary>
		FIF_XBM = 22,
		/// <summary>
		/// X11 Pixmap Format (*.XPM)
		/// </summary>
		FIF_XPM = 23,
		/// <summary>
		/// DirectDraw Surface (*.DDS)
		/// </summary>
		FIF_DDS = 24,
		/// <summary>
		/// Graphics Interchange Format (*.GIF)
		/// </summary>
		FIF_GIF = 25,
		/// <summary>
		/// High Dynamic Range (*.HDR)
		/// </summary>
		FIF_HDR = 26,
		/// <summary>
		/// Raw Fax format CCITT G3 (*.G3)
		/// </summary>
		FIF_FAXG3 = 27,
		/// <summary>
		/// Silicon Graphics SGI image format (*.SGI)
		/// </summary>
		FIF_SGI = 28,
		/// <summary>
		/// OpenEXR format (*.EXR)
		/// </summary>
		FIF_EXR = 29,
		/// <summary>
		/// JPEG-2000 format (*.J2K, *.J2C)
		/// </summary>
		FIF_J2K = 30,
		/// <summary>
		/// JPEG-2000 format (*.JP2)
		/// </summary>
		FIF_JP2 = 31
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Image types used in FreeImage.
	/// </summary>
	public enum FREE_IMAGE_TYPE
	{
		/// <summary>
		/// unknown type
		/// </summary>
		FIT_UNKNOWN = 0,
		/// <summary>
		/// standard image : 1-, 4-, 8-, 16-, 24-, 32-bit
		/// </summary>
		FIT_BITMAP = 1,
		/// <summary>
		/// array of unsigned short : unsigned 16-bit
		/// </summary>
		FIT_UINT16 = 2,
		/// <summary>
		/// array of short : signed 16-bit
		/// </summary>
		FIT_INT16 = 3,
		/// <summary>
		/// array of unsigned long : unsigned 32-bit
		/// </summary>
		FIT_UINT32 = 4,
		/// <summary>
		/// array of long : signed 32-bit
		/// </summary>
		FIT_INT32 = 5,
		/// <summary>
		/// array of float : 32-bit IEEE floating point
		/// </summary>
		FIT_FLOAT = 6,
		/// <summary>
		/// array of double : 64-bit IEEE floating point
		/// </summary>
		FIT_DOUBLE = 7,
		/// <summary>
		/// array of FICOMPLEX : 2 x 64-bit IEEE floating point
		/// </summary>
		FIT_COMPLEX = 8,
		/// <summary>
		/// 48-bit RGB image : 3 x 16-bit
		/// </summary>
		FIT_RGB16 = 9,
		/// <summary>
		/// 64-bit RGBA image : 4 x 16-bit
		/// </summary>
		FIT_RGBA16 = 10,
		/// <summary>
		/// 96-bit RGB float image : 3 x 32-bit IEEE floating point
		/// </summary>
		FIT_RGBF = 11,
		/// <summary>
		/// 128-bit RGBA float image : 4 x 32-bit IEEE floating point
		/// </summary>
		FIT_RGBAF = 12
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Image color types used in FreeImage.
	/// </summary>
	public enum FREE_IMAGE_COLOR_TYPE
	{
		/// <summary>
		/// min value is white
		/// </summary>
		FIC_MINISWHITE = 0,
		/// <summary>
		/// min value is black
		/// </summary>
		FIC_MINISBLACK = 1,
		/// <summary>
		/// RGB color model
		/// </summary>
		FIC_RGB = 2,
		/// <summary>
		/// color map indexed
		/// </summary>
		FIC_PALETTE = 3,
		/// <summary>
		/// RGB color model with alpha channel
		/// </summary>
		FIC_RGBALPHA = 4,
		/// <summary>
		/// CMYK color model
		/// </summary>
		FIC_CMYK = 5
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Color quantization algorithms.
	/// Constants used in FreeImage_ColorQuantize.
	/// </summary>
	public enum FREE_IMAGE_QUANTIZE
	{
		/// <summary>
		/// Xiaolin Wu color quantization algorithm
		/// </summary>
		FIQ_WUQUANT = 0,
		/// <summary>
		/// NeuQuant neural-net quantization algorithm by Anthony Dekker
		/// </summary>
		FIQ_NNQUANT = 1
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Dithering algorithms.
	/// Constants used in FreeImage_Dither.
	/// </summary>
	public enum FREE_IMAGE_DITHER
	{
		/// <summary>
		/// Floyd and Steinberg error diffusion
		/// </summary>
		FID_FS = 0,
		/// <summary>
		/// Bayer ordered dispersed dot dithering (order 2 dithering matrix)
		/// </summary>
		FID_BAYER4x4 = 1,
		/// <summary>
		/// Bayer ordered dispersed dot dithering (order 3 dithering matrix)
		/// </summary>
		FID_BAYER8x8 = 2,
		/// <summary>
		/// Ordered clustered dot dithering (order 3 - 6x6 matrix)
		/// </summary>
		FID_CLUSTER6x6 = 3,
		/// <summary>
		/// Ordered clustered dot dithering (order 4 - 8x8 matrix)
		/// </summary>
		FID_CLUSTER8x8 = 4,
		/// <summary>
		/// Ordered clustered dot dithering (order 8 - 16x16 matrix)
		/// </summary>
		FID_CLUSTER16x16 = 5,
		/// <summary>
		/// Bayer ordered dispersed dot dithering (order 4 dithering matrix)
		/// </summary>
		FID_BAYER16x16 = 6
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Lossless JPEG transformations constants used in FreeImage_JPEGTransform.
	/// </summary>
	public enum FREE_IMAGE_JPEG_OPERATION
	{
		/// <summary>
		/// no transformation
		/// </summary>
		FIJPEG_OP_NONE = 0,
		/// <summary>
		/// horizontal flip
		/// </summary>
		FIJPEG_OP_FLIP_H = 1,
		/// <summary>
		/// vertical flip
		/// </summary>
		FIJPEG_OP_FLIP_V = 2,
		/// <summary>
		/// transpose across UL-to-LR axis
		/// </summary>
		FIJPEG_OP_TRANSPOSE = 3,
		/// <summary>
		/// transpose across UR-to-LL axis
		/// </summary>
		FIJPEG_OP_TRANSVERSE = 4,
		/// <summary>
		/// 90-degree clockwise rotation
		/// </summary>
		FIJPEG_OP_ROTATE_90 = 5,
		/// <summary>
		/// 180-degree rotation
		/// </summary>
		FIJPEG_OP_ROTATE_180 = 6,
		/// <summary>
		/// 270-degree clockwise (or 90 ccw)
		/// </summary>
		FIJPEG_OP_ROTATE_270 = 7
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Tone mapping operators. Constants used in FreeImage_ToneMapping.
	/// </summary>
	public enum FREE_IMAGE_TMO
	{
		/// <summary>
		/// Adaptive logarithmic mapping (F. Drago, 2003)
		/// </summary>
		FITMO_DRAGO03 = 0,
		/// <summary>
		/// Dynamic range reduction inspired by photoreceptor physiology (E. Reinhard, 2005)
		/// </summary>
		FITMO_REINHARD05 = 1,
		/// <summary>
		/// Gradient domain high dynamic range compression (R. Fattal, 2002)
		/// </summary>
		FITMO_FATTAL02
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Upsampling / downsampling filters. Constants used in FreeImage_Rescale.
	/// </summary>
	public enum FREE_IMAGE_FILTER
	{
		/// <summary>
		/// Box, pulse, Fourier window, 1st order (constant) b-spline
		/// </summary>
		FILTER_BOX = 0,
		/// <summary>
		/// Mitchell and Netravali's two-param cubic filter
		/// </summary>
		FILTER_BICUBIC = 1,
		/// <summary>
		/// Bilinear filter
		/// </summary>
		FILTER_BILINEAR = 2,
		/// <summary>
		/// 4th order (cubic) b-spline
		/// </summary>
		FILTER_BSPLINE = 3,
		/// <summary>
		/// Catmull-Rom spline, Overhauser spline
		/// </summary>
		FILTER_CATMULLROM = 4,
		/// <summary>
		/// Lanczos3 filter
		/// </summary>
		FILTER_LANCZOS3 = 5
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Color channels. Constants used in color manipulation routines.
	/// </summary>
	public enum FREE_IMAGE_COLOR_CHANNEL
	{
		/// <summary>
		/// Use red, green and blue channels
		/// </summary>
		FICC_RGB = 0,
		/// <summary>
		/// Use red channel
		/// </summary>
		FICC_RED = 1,
		/// <summary>
		/// Use green channel
		/// </summary>
		FICC_GREEN = 2,
		/// <summary>
		/// Use blue channel
		/// </summary>
		FICC_BLUE = 3,
		/// <summary>
		/// Use alpha channel
		/// </summary>
		FICC_ALPHA = 4,
		/// <summary>
		/// Use black channel
		/// </summary>
		FICC_BLACK = 5,
		/// <summary>
		/// Complex images: use real part
		/// </summary>
		FICC_REAL = 6,
		/// <summary>
		/// Complex images: use imaginary part
		/// </summary>
		FICC_IMAG = 7,
		/// <summary>
		/// Complex images: use magnitude
		/// </summary>
		FICC_MAG = 8,
		/// <summary>
		/// Complex images: use phase
		/// </summary>
		FICC_PHASE = 9
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Tag data type information (based on TIFF specifications)
	/// Note: RATIONALs are the ratio of two 32-bit integer values.
	/// </summary>
	public enum FREE_IMAGE_MDTYPE
	{
		/// <summary>
		/// placeholder
		/// </summary>
		FIDT_NOTYPE = 0,
		/// <summary>
		/// 8-bit unsigned integer
		/// </summary>
		FIDT_BYTE = 1,
		/// <summary>
		/// 8-bit bytes w/ last byte null
		/// </summary>
		FIDT_ASCII = 2,
		/// <summary>
		/// 16-bit unsigned integer
		/// </summary>
		FIDT_SHORT = 3,
		/// <summary>
		/// 32-bit unsigned integer
		/// </summary>
		FIDT_LONG = 4,
		/// <summary>
		/// 64-bit unsigned fraction
		/// </summary>
		FIDT_RATIONAL = 5,
		/// <summary>
		/// 8-bit signed integer
		/// </summary>
		FIDT_SBYTE = 6,
		/// <summary>
		/// 8-bit untyped data
		/// </summary>
		FIDT_UNDEFINED = 7,
		/// <summary>
		/// 16-bit signed integer
		/// </summary>
		FIDT_SSHORT = 8,
		/// <summary>
		/// 32-bit signed integer
		/// </summary>
		FIDT_SLONG = 9,
		/// <summary>
		/// 64-bit signed fraction
		/// </summary>
		FIDT_SRATIONAL = 10,
		/// <summary>
		/// 32-bit IEEE floating point
		/// </summary>
		FIDT_FLOAT = 11,
		/// <summary>
		/// 64-bit IEEE floating point
		/// </summary>
		FIDT_DOUBLE = 12,
		/// <summary>
		/// 32-bit unsigned integer (offset)
		/// </summary>
		FIDT_IFD = 13,
		/// <summary>
		/// 32-bit RGBQUAD
		/// </summary>
		FIDT_PALETTE = 14
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Metadata models supported by FreeImage.
	/// </summary>
	public enum FREE_IMAGE_MDMODEL
	{
		/// <summary>
		/// No data
		/// </summary>
		FIMD_NODATA = -1,
		/// <summary>
		/// single comment or keywords
		/// </summary>
		FIMD_COMMENTS = 0,
		/// <summary>
		/// Exif-TIFF metadata
		/// </summary>
		FIMD_EXIF_MAIN = 1,
		/// <summary>
		/// Exif-specific metadata
		/// </summary>
		FIMD_EXIF_EXIF = 2,
		/// <summary>
		/// Exif GPS metadata
		/// </summary>
		FIMD_EXIF_GPS = 3,
		/// <summary>
		/// Exif maker note metadata
		/// </summary>
		FIMD_EXIF_MAKERNOTE = 4,
		/// <summary>
		/// Exif interoperability metadata
		/// </summary>
		FIMD_EXIF_INTEROP = 5,
		/// <summary>
		/// IPTC/NAA metadata
		/// </summary>
		FIMD_IPTC = 6,
		/// <summary>
		/// Abobe XMP metadata
		/// </summary>
		FIMD_XMP = 7,
		/// <summary>
		/// GeoTIFF metadata
		/// </summary>
		FIMD_GEOTIFF = 8,
		/// <summary>
		/// Animation metadata
		/// </summary>
		FIMD_ANIMATION = 9,
		/// <summary>
		/// Used to attach other metadata types to a dib
		/// </summary>
		FIMD_CUSTOM = 10
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Flags used in load functions.
	/// </summary>
	[System.Flags]
	public enum FREE_IMAGE_LOAD_FLAGS
	{
		/// <summary>
		/// Default option for all types.
		/// </summary>
		DEFAULT = 0,
		/// <summary>
		/// Load the image as a 256 color image with ununsed palette entries, if it's 16 or 2 color.
		/// </summary>
		GIF_LOAD256 = 1,
		/// <summary>
		/// 'Play' the GIF to generate each frame (as 32bpp) instead of returning raw frame data when loading.
		/// </summary>
		GIF_PLAYBACK = 2,
		/// <summary>
		/// Convert to 32bpp and create an alpha channel from the AND-mask when loading.
		/// </summary>
		ICO_MAKEALPHA = 1,
		/// <summary>
		/// Load the file as fast as possible, sacrificing some quality.
		/// </summary>
		JPEG_FAST = 0x0001,
		/// <summary>
		/// Load the file with the best quality, sacrificing some speed.
		/// </summary>
		JPEG_ACCURATE = 0x0002,
		/// <summary>
		/// load separated CMYK "as is" (use | to combine with other load flags).
		/// </summary>
		JPEG_CMYK = 0x0004,
		/// <summary>
		/// Load the bitmap sized 768 x 512.
		/// </summary>
		PCD_BASE = 1,
		/// <summary>
		/// Load the bitmap sized 384 x 256.
		/// </summary>
		PCD_BASEDIV4 = 2,
		/// <summary>
		/// Load the bitmap sized 192 x 128.
		/// </summary>
		PCD_BASEDIV16 = 3,
		/// <summary>
		/// Avoid gamma correction.
		/// </summary>
		PNG_IGNOREGAMMA = 1,
		/// <summary>
		/// If set the loader converts RGB555 and ARGB8888 -> RGB888.
		/// </summary>
		TARGA_LOAD_RGB888 = 1,
		/// <summary>
		/// Reads tags for separated CMYK.
		/// </summary>
		TIFF_CMYK = 0x0001
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Flags used in save functions.
	/// </summary>
	[System.Flags]
	public enum FREE_IMAGE_SAVE_FLAGS
	{
		/// <summary>
		/// Default option for all types.
		/// </summary>
		DEFAULT = 0,
		/// <summary>
		/// Save with run length encoding.
		/// </summary>
		BMP_SAVE_RLE = 1,
		/// <summary>
		/// Save data as float instead of as half (not recommended).
		/// </summary>
		EXR_FLOAT = 0x0001,
		/// <summary>
		/// Save with no compression.
		/// </summary>
		EXR_NONE = 0x0002,
		/// <summary>
		/// Save with zlib compression, in blocks of 16 scan lines.
		/// </summary>
		EXR_ZIP = 0x0004,
		/// <summary>
		/// Save with piz-based wavelet compression.
		/// </summary>
		EXR_PIZ = 0x0008,
		/// <summary>
		/// Save with lossy 24-bit float compression.
		/// </summary>
		EXR_PXR24 = 0x0010,
		/// <summary>
		/// Save with lossy 44% float compression - goes to 22% when combined with EXR_LC.
		/// </summary>
		EXR_B44 = 0x0020,
		/// <summary>
		/// Save images with one luminance and two chroma channels, rather than as RGB (lossy compression).
		/// </summary>
		EXR_LC = 0x0040,
		/// <summary>
		/// Save with superb quality (100:1).
		/// </summary>
		JPEG_QUALITYSUPERB = 0x80,
		/// <summary>
		/// Save with good quality (75:1).
		/// </summary>
		JPEG_QUALITYGOOD = 0x0100,
		/// <summary>
		/// Save with normal quality (50:1).
		/// </summary>
		JPEG_QUALITYNORMAL = 0x0200,
		/// <summary>
		/// Save with average quality (25:1).
		/// </summary>
		JPEG_QUALITYAVERAGE = 0x0400,
		/// <summary>
		/// Save with bad quality (10:1).
		/// </summary>
		JPEG_QUALITYBAD = 0x0800,
		/// <summary>
		/// Save as a progressive-JPEG (use | to combine with other save flags).
		/// </summary>
		JPEG_PROGRESSIVE = 0x2000,
		/// <summary>
		/// Save with high 4x1 chroma subsampling (4:1:1).
		/// </summary>
		JPEG_SUBSAMPLING_411 = 0x1000,
		/// <summary>
		/// Save with medium 2x2 medium chroma (4:2:0).
		/// </summary>
		JPEG_SUBSAMPLING_420 = 0x4000,
		/// <summary>
		/// Save with low 2x1 chroma subsampling (4:2:2).
		/// </summary>
		JPEG_SUBSAMPLING_422 = 0x8000,
		/// <summary>
		/// Save with no chroma subsampling (4:4:4).
		/// </summary>
		JPEG_SUBSAMPLING_444 = 0x10000,
		/// <summary>
		/// Save using ZLib level 1 compression flag
		/// (default value is <see cref="PNG_Z_DEFAULT_COMPRESSION"/>).
		/// </summary>
		PNG_Z_BEST_SPEED = 0x0001,
		/// <summary>
		/// Save using ZLib level 6 compression flag (default recommended value).
		/// </summary>
		PNG_Z_DEFAULT_COMPRESSION = 0x0006,
		/// <summary>
		/// save using ZLib level 9 compression flag
		/// (default value is <see cref="PNG_Z_DEFAULT_COMPRESSION"/>).
		/// </summary>
		PNG_Z_BEST_COMPRESSION = 0x0009,
		/// <summary>
		/// Save without ZLib compression.
		/// </summary>
		PNG_Z_NO_COMPRESSION = 0x0100,
		/// <summary>
		/// Save using Adam7 interlacing (use | to combine with other save flags).
		/// </summary>
		PNG_INTERLACED = 0x0200,
		/// <summary>
		/// If set the writer saves in ASCII format (i.e. P1, P2 or P3).
		/// </summary>
		PNM_SAVE_ASCII = 1,
		/// <summary>
		/// Stores tags for separated CMYK (use | to combine with compression flags).
		/// </summary>
		TIFF_CMYK = 0x0001,
		/// <summary>
		/// Save using PACKBITS compression.
		/// </summary>
		TIFF_PACKBITS = 0x0100,
		/// <summary>
		/// Save using DEFLATE compression (a.k.a. ZLIB compression).
		/// </summary>
		TIFF_DEFLATE = 0x0200,
		/// <summary>
		/// Save using ADOBE DEFLATE compression.
		/// </summary>
		TIFF_ADOBE_DEFLATE = 0x0400,
		/// <summary>
		/// Save without any compression.
		/// </summary>
		TIFF_NONE = 0x0800,
		/// <summary>
		/// Save using CCITT Group 3 fax encoding.
		/// </summary>
		TIFF_CCITTFAX3 = 0x1000,
		/// <summary>
		/// Save using CCITT Group 4 fax encoding.
		/// </summary>
		TIFF_CCITTFAX4 = 0x2000,
		/// <summary>
		/// Save using LZW compression.
		/// </summary>
		TIFF_LZW = 0x4000,
		/// <summary>
		/// Save using JPEG compression.
		/// </summary>
		TIFF_JPEG = 0x8000
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Flags for ICC profiles.
	/// </summary>
	[System.Flags]
	public enum ICC_FLAGS : ushort
	{
		/// <summary>
		/// Default value.
		/// </summary>
		FIICC_DEFAULT = 0x00,
		/// <summary>
		/// The color is CMYK.
		/// </summary>
		FIICC_COLOR_IS_CMYK = 0x01
	}
}

	#endregion

	#region Delegates

namespace FreeImageAPI
{
	// Delegates used by the FreeImageIO structure

	/// <summary>
	/// Delegate for capturing FreeImage error messages.
	/// </summary>
	/// <param name="fif">The format of the image.</param>
	/// <param name="message">The errormessage.</param>
	// DLL_API is missing in the definition of the callbackfuntion.
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void OutputMessageFunction(FREE_IMAGE_FORMAT fif, string message);
}

namespace FreeImageAPI.IO
{
	/// <summary>
	/// Delegate to the C++ function <b>fread</b>.
	/// </summary>
	/// <param name="buffer">Pointer to read from.</param>
	/// <param name="size">Item size in bytes.</param>
	/// <param name="count">Maximum number of items to be read.</param>
	/// <param name="handle">Handle/stream to read from.</param>
	/// <returns>Number of full items actually read,
	/// which may be less than count if an error occurs or
	/// if the end of the file is encountered before reaching count.</returns>
	public delegate uint ReadProc(IntPtr buffer, uint size, uint count, fi_handle handle);

	/// <summary>
	/// Delegate to the C++ function <b>fwrite</b>.
	/// </summary>
	/// <param name="buffer">Pointer to data to be written.</param>
	/// <param name="size">Item size in bytes.</param>
	/// <param name="count">Maximum number of items to be written.</param>
	/// <param name="handle">Handle/stream to write to.</param>
	/// <returns>Number of full items actually written,
	/// which may be less than count if an error occurs.
	/// Also, if an error occurs, the file-position indicator cannot be determined.</returns>
	public delegate uint WriteProc(IntPtr buffer, uint size, uint count, fi_handle handle);

	/// <summary>
	/// Delegate to the C++ function <b>fseek</b>.
	/// </summary>
	/// <param name="handle">Handle/stream to seek in.</param>
	/// <param name="offset">Number of bytes from origin.</param>
	/// <param name="origin">Initial position.</param>
	/// <returns>If successful 0 is returned; otherwise a nonzero value. </returns>
	public delegate int SeekProc(fi_handle handle, int offset, SeekOrigin origin);

	/// <summary>
	/// Delegate to the C++ function <b>ftell</b>.
	/// </summary>
	/// <param name="handle">Handle/stream to retrieve its currents position from.</param>
	/// <returns>The current position.</returns>
	public delegate int TellProc(fi_handle handle);

	// Delegates used by 'Plugin' structure
}

namespace FreeImageAPI.Plugins
{
	/// <summary>
	/// Delegate to a function that returns a string which describes
	/// the plugins format.
	/// </summary>
	public delegate string FormatProc();

	/// <summary>
	/// Delegate to a function that returns a string which contains
	/// a more detailed description.
	/// </summary>
	public delegate string DescriptionProc();

	/// <summary>
	/// Delegate to a function that returns a comma seperated list
	/// of file extensions the plugin can read or write.
	/// </summary>
	public delegate string ExtensionListProc();

	/// <summary>
	/// Delegate to a function that returns a regular expression that
	/// can be used to idientify whether a file can be handled by the plugin.
	/// </summary>
	public delegate string RegExprProc();

	/// <summary>
	/// Delegate to a function that opens a file.
	/// </summary>
	public delegate IntPtr OpenProc(ref FreeImageIO io, fi_handle handle, bool read);

	/// <summary>
	/// Delegate to a function that closes a previosly opened file.
	/// </summary>
	public delegate void CloseProc(ref FreeImageIO io, fi_handle handle, IntPtr data);

	/// <summary>
	/// Delegate to a function that returns the number of pages of a multipage
	/// bitmap if the plugin is capable of handling multipage bitmaps.
	/// </summary>
	public delegate int PageCountProc(ref FreeImageIO io, fi_handle handle, IntPtr data);

	/// <summary>
	/// UNKNOWN
	/// </summary>
	public delegate int PageCapabilityProc(ref FreeImageIO io, fi_handle handle, IntPtr data);

	/// <summary>
	/// Delegate to a function that loads and decodes a bitmap into memory.
	/// </summary>
	public delegate FIBITMAP LoadProc(ref FreeImageIO io, fi_handle handle, int page, int flags, IntPtr data);

	/// <summary>
	///  Delegate to a function that saves a bitmap.
	/// </summary>
	public delegate bool SaveProc(ref FreeImageIO io, FIBITMAP dib, fi_handle handle, int page, int flags, IntPtr data);

	/// <summary>
	/// Delegate to a function that determines whether the source defined
	/// by <param name="io"/> and <param name="handle"/> is a valid image.
	/// </summary>
	public delegate bool ValidateProc(ref FreeImageIO io, fi_handle handle);

	/// <summary>
	/// Delegate to a function that returns a string which contains
	/// the plugin's mime type.
	/// </summary>
	public delegate string MimeProc();

	/// <summary>
	/// Delegate to a function that returns whether the plugin can handle the
	/// specified color depth.
	/// </summary>
	public delegate bool SupportsExportBPPProc(int bpp);

	/// <summary>
	/// Delegate to a function that returns whether the plugin can handle the
	/// specified image type.
	/// </summary>
	public delegate bool SupportsExportTypeProc(FREE_IMAGE_TYPE type);

	/// <summary>
	/// Delegate to a function that returns whether the plugin can handle
	/// ICC-Profiles.
	/// </summary>
	public delegate bool SupportsICCProfilesProc();

	/// <summary>
	/// Callback function used by FreeImage to register plugins.
	/// </summary>
	public delegate void InitProc(ref Plugin plugin, int format_id);
}

	#endregion

namespace FreeImageAPI
{
	public static partial class FreeImage
	{
		#region Constants

		/// <summary>
		/// Filename of the FreeImage library.
		/// </summary>
		private const string FreeImageLibrary = "FreeImage";

		/// <summary>
		/// Major version of the library.
		/// </summary>
		public const int FREEIMAGE_MAJOR_VERSION = 3;
		/// <summary>
		/// Minor version of the library.
		/// </summary>
		public const int FREEIMAGE_MINOR_VERSION = 11;
		/// <summary>
		/// Release version of the library.
		/// </summary>
		public const int FREEIMAGE_RELEASE_SERIAL = 0;

		/// <summary>
		/// Number of bytes to shift left within a 4 byte block.
		/// </summary>
		public const int FI_RGBA_RED = 2;
		/// <summary>
		/// Number of bytes to shift left within a 4 byte block.
		/// </summary>
		public const int FI_RGBA_GREEN = 1;
		/// <summary>
		/// Number of bytes to shift left within a 4 byte block.
		/// </summary>
		public const int FI_RGBA_BLUE = 0;
		/// <summary>
		/// Number of bytes to shift left within a 4 byte block.
		/// </summary>
		public const int FI_RGBA_ALPHA = 3;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const uint FI_RGBA_RED_MASK = 0x00FF0000;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const uint FI_RGBA_GREEN_MASK = 0x0000FF00;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const uint FI_RGBA_BLUE_MASK = 0x000000FF;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const uint FI_RGBA_ALPHA_MASK = 0xFF000000;
		/// <summary>
		/// Number of bits to shift left within a 32 bit block.
		/// </summary>
		public const int FI_RGBA_RED_SHIFT = 16;
		/// <summary>
		/// Number of bits to shift left within a 32 bit block.
		/// </summary>
		public const int FI_RGBA_GREEN_SHIFT = 8;
		/// <summary>
		/// Number of bits to shift left within a 32 bit block.
		/// </summary>
		public const int FI_RGBA_BLUE_SHIFT = 0;
		/// <summary>
		/// Number of bits to shift left within a 32 bit block.
		/// </summary>
		public const int FI_RGBA_ALPHA_SHIFT = 24;
		/// <summary>
		/// Mask indicating the position of color components of a 32 bit color.
		/// </summary>
		public const uint FI_RGBA_RGB_MASK = (FI_RGBA_RED_MASK | FI_RGBA_GREEN_MASK | FI_RGBA_BLUE_MASK);

		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const int FI16_555_RED_MASK = 0x7C00;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const int FI16_555_GREEN_MASK = 0x03E0;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const int FI16_555_BLUE_MASK = 0x001F;
		/// <summary>
		/// Number of bits to shift left within a 16 bit block.
		/// </summary>
		public const int FI16_555_RED_SHIFT = 10;
		/// <summary>
		/// Number of bits to shift left within a 16 bit block.
		/// </summary>
		public const int FI16_555_GREEN_SHIFT = 5;
		/// <summary>
		/// Number of bits to shift left within a 16 bit block.
		/// </summary>
		public const int FI16_555_BLUE_SHIFT = 0;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const int FI16_565_RED_MASK = 0xF800;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const int FI16_565_GREEN_MASK = 0x07E0;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const int FI16_565_BLUE_MASK = 0x001F;
		/// <summary>
		/// Number of bits to shift left within a 16 bit block.
		/// </summary>
		public const int FI16_565_RED_SHIFT = 11;
		/// <summary>
		/// Number of bits to shift left within a 16 bit block.
		/// </summary>
		public const int FI16_565_GREEN_SHIFT = 5;
		/// <summary>
		/// Number of bits to shift left within a 16 bit block.
		/// </summary>
		public const int FI16_565_BLUE_SHIFT = 0;

		#endregion

		#region General functions

		/// <summary>
		/// Initialises the library.
		/// </summary>
		/// <param name="load_local_plugins_only">
		/// When the <paramref name="load_local_plugins_only"/> is true, FreeImage won't make use of external plugins.
		/// </param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Initialise")]
		private static extern void Initialise(bool load_local_plugins_only);

		/// <summary>
		/// Deinitialises the library.
		/// </summary>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_DeInitialise")]
		private static extern void DeInitialise();

		/// <summary>
		/// Returns a string containing the current version of the library.
		/// </summary>
		/// <returns>The current version of the library.</returns>
		public static unsafe string GetVersion() { return PtrToStr(GetVersion_()); }
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_GetVersion")]
		private static unsafe extern byte* GetVersion_();

		/// <summary>
		/// Returns a string containing a standard copyright message.
		/// </summary>
		/// <returns>A standard copyright message.</returns>
		public static unsafe string GetCopyrightMessage() { return PtrToStr(GetCopyrightMessage_()); }
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_GetCopyrightMessage")]
		private static unsafe extern byte* GetCopyrightMessage_();

		/// <summary>
		/// Calls the set error message function in FreeImage.
		/// </summary>
		/// <param name="fif">Format of the bitmaps.</param>
		/// <param name="message">The error message.</param>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_OutputMessageProc")]
		public static extern void OutputMessageProc(FREE_IMAGE_FORMAT fif, string message);

		/// <summary>
		/// You use the function FreeImage_SetOutputMessage to capture the log string
		/// so that you can show it to the user of the program.
		/// The callback is implemented in the <see cref="FreeImageEngine.Message"/> event of this class.
		/// </summary>
		/// <remarks>The function is private because FreeImage can only have a single
		/// callback function. To use the callback use the <see cref="FreeImageEngine.Message"/>
		/// event of this class.</remarks>
		/// <param name="omf">Handler to the callback function.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetOutputMessage")]
		internal static extern void SetOutputMessage(OutputMessageFunction omf);

		#endregion

		#region Bitmap management functions

		/// <summary>
		/// Creates a new bitmap in memory.
		/// </summary>
		/// <param name="width">Width of the new bitmap.</param>
		/// <param name="height">Height of the new bitmap.</param>
		/// <param name="bpp">Bit depth of the new Bitmap.
		/// Supported pixel depth: 1-, 4-, 8-, 16-, 24-, 32-bit per pixel for standard bitmap</param>
		/// <param name="red_mask">Red part of the color layout.
		/// eg: 0xFF0000</param>
		/// <param name="green_mask">Green part of the color layout.
		/// eg: 0x00FF00</param>
		/// <param name="blue_mask">Blue part of the color layout.
		/// eg: 0x0000FF</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Allocate")]
		public static extern FIBITMAP Allocate(int width, int height, int bpp,
				uint red_mask, uint green_mask, uint blue_mask);

		/// <summary>
		/// Creates a new bitmap in memory.
		/// </summary>
		/// <param name="type">Type of the image.</param>
		/// <param name="width">Width of the new bitmap.</param>
		/// <param name="height">Height of the new bitmap.</param>
		/// <param name="bpp">Bit depth of the new Bitmap.
		/// Supported pixel depth: 1-, 4-, 8-, 16-, 24-, 32-bit per pixel for standard bitmap</param>
		/// <param name="red_mask">Red part of the color layout.
		/// eg: 0xFF0000</param>
		/// <param name="green_mask">Green part of the color layout.
		/// eg: 0x00FF00</param>
		/// <param name="blue_mask">Blue part of the color layout.
		/// eg: 0x0000FF</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AllocateT")]
		public static extern FIBITMAP AllocateT(FREE_IMAGE_TYPE type, int width, int height, int bpp,
				uint red_mask, uint green_mask, uint blue_mask);

		/// <summary>
		/// Makes an exact reproduction of an existing bitmap, including metadata and attached profile if any.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Clone")]
		public static extern FIBITMAP Clone(FIBITMAP dib);

		/// <summary>
		/// Deletes a previously loaded FIBITMAP from memory.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Unload")]
		public static extern void Unload(FIBITMAP dib);

		/// <summary>
		/// Decodes a bitmap, allocates memory for it and returns it as a FIBITMAP.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="filename">Name of the file to decode.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_LoadU")]
		public static extern FIBITMAP Load(FREE_IMAGE_FORMAT fif, string filename, FREE_IMAGE_LOAD_FLAGS flags);

		/// <summary>
		/// Decodes a bitmap, allocates memory for it and returns it as a FIBITMAP.
		/// The filename supports UNICODE.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="filename">Name of the file to decode.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_LoadU")]
		private static extern FIBITMAP LoadU(FREE_IMAGE_FORMAT fif, string filename, FREE_IMAGE_LOAD_FLAGS flags);

		/// <summary>
		/// Loads a bitmap from an arbitrary source.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="io">A FreeImageIO structure with functionpointers to handle the source.</param>
		/// <param name="handle">A handle to the source.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_LoadFromHandle")]
		public static extern FIBITMAP LoadFromHandle(FREE_IMAGE_FORMAT fif, ref FreeImageIO io, fi_handle handle, FREE_IMAGE_LOAD_FLAGS flags);

		/// <summary>
		/// Saves a previosly loaded FIBITMAP to a file.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">Name of the file to save to.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_SaveU")]
		public static extern bool Save(FREE_IMAGE_FORMAT fif, FIBITMAP dib, string filename, FREE_IMAGE_SAVE_FLAGS flags);

		/// <summary>
		/// Saves a previosly loaded FIBITMAP to a file.
		/// The filename supports UNICODE.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">Name of the file to save to.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_SaveU")]
		private static extern bool SaveU(FREE_IMAGE_FORMAT fif, FIBITMAP dib, string filename, FREE_IMAGE_SAVE_FLAGS flags);

		/// <summary>
		/// Saves a bitmap to an arbitrary source.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="io">A FreeImageIO structure with functionpointers to handle the source.</param>
		/// <param name="handle">A handle to the source.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SaveToHandle")]
		public static extern bool SaveToHandle(FREE_IMAGE_FORMAT fif, FIBITMAP dib, ref FreeImageIO io, fi_handle handle,
				FREE_IMAGE_SAVE_FLAGS flags);

		#endregion

		#region Memory I/O streams

		/// <summary>
		/// Open a memory stream.
		/// </summary>
		/// <param name="data">Pointer to the data in memory.</param>
		/// <param name="size_in_bytes">Length of the data in byte.</param>
		/// <returns>Handle to a memory stream.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_OpenMemory")]
		public static extern FIMEMORY OpenMemory(IntPtr data, uint size_in_bytes);

		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_OpenMemory")]
		internal static extern FIMEMORY OpenMemoryEx(byte[] data, uint size_in_bytes);

		/// <summary>
		/// Close and free a memory stream.
		/// </summary>
		/// <param name="stream">Handle to a memory stream.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_CloseMemory")]
		public static extern void CloseMemory(FIMEMORY stream);

		/// <summary>
		/// Decodes a bitmap from a stream, allocates memory for it and returns it as a FIBITMAP.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="stream">Handle to a memory stream.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_LoadFromMemory")]
		public static extern FIBITMAP LoadFromMemory(FREE_IMAGE_FORMAT fif, FIMEMORY stream, FREE_IMAGE_LOAD_FLAGS flags);

		/// <summary>
		/// Saves a previosly loaded FIBITMAP to a stream.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="stream">Handle to a memory stream.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SaveToMemory")]
		public static extern bool SaveToMemory(FREE_IMAGE_FORMAT fif, FIBITMAP dib, FIMEMORY stream, FREE_IMAGE_SAVE_FLAGS flags);

		/// <summary>
		/// Gets the current position of a memory handle.
		/// </summary>
		/// <param name="stream">Handle to a memory stream.</param>
		/// <returns>The current file position if successful, -1 otherwise.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_TellMemory")]
		public static extern int TellMemory(FIMEMORY stream);

		/// <summary>
		/// Moves the memory handle to a specified location.
		/// </summary>
		/// <param name="stream">Handle to a memory stream.</param>
		/// <param name="offset">Number of bytes from origin.</param>
		/// <param name="origin">Initial position.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SeekMemory")]
		public static extern bool SeekMemory(FIMEMORY stream, int offset, System.IO.SeekOrigin origin);

		/// <summary>
		/// Provides a direct buffer access to a memory stream.
		/// </summary>
		/// <param name="stream">The target memory stream.</param>
		/// <param name="data">Pointer to the data in memory.</param>
		/// <param name="size_in_bytes">Size of the data in bytes.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AcquireMemory")]
		public static extern bool AcquireMemory(FIMEMORY stream, ref IntPtr data, ref uint size_in_bytes);

		/// <summary>
		/// Reads data from a memory stream.
		/// </summary>
		/// <param name="buffer">The buffer to store the data in.</param>
		/// <param name="size">Size in bytes of the items.</param>
		/// <param name="count">Number of items to read.</param>
		/// <param name="stream">The stream to read from.
		/// The memory pointer associated with stream is increased by the number of bytes actually read.</param>
		/// <returns>The number of full items actually read.
		/// May be less than count on error or stream-end.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ReadMemory")]
		public static extern uint ReadMemory(byte[] buffer, uint size, uint count, FIMEMORY stream);

		/// <summary>
		/// Writes data to a memory stream.
		/// </summary>
		/// <param name="buffer">The buffer to read the data from.</param>
		/// <param name="size">Size in bytes of the items.</param>
		/// <param name="count">Number of items to write.</param>
		/// <param name="stream">The stream to write to.
		/// The memory pointer associated with stream is increased by the number of bytes actually written.</param>
		/// <returns>The number of full items actually written.
		/// May be less than count on error or stream-end.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_WriteMemory")]
		public static extern uint WriteMemory(byte[] buffer, uint size, uint count, FIMEMORY stream);

		/// <summary>
		/// Open a multi-page bitmap from a memory stream.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="stream">The stream to decode.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_LoadMultiBitmapFromMemory")]
		public static extern FIMULTIBITMAP LoadMultiBitmapFromMemory(FREE_IMAGE_FORMAT fif, FIMEMORY stream, FREE_IMAGE_LOAD_FLAGS flags);

		#endregion

		#region Plugin functions

		/// <summary>
		/// Registers a new plugin to be used in FreeImage.
		/// </summary>
		/// <param name="proc_address">Pointer to the function that initialises the plugin.</param>
		/// <param name="format">A string describing the format of the plugin.</param>
		/// <param name="description">A string describing the plugin.</param>
		/// <param name="extension">A string witha comma sperated list of extensions. f.e: "pl,pl2,pl4"</param>
		/// <param name="regexpr">A regular expression used to identify the bitmap.</param>
		/// <returns>The format idientifier assigned by FreeImage.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_RegisterLocalPlugin")]
		public static extern FREE_IMAGE_FORMAT RegisterLocalPlugin(InitProc proc_address,
			string format, string description, string extension, string regexpr);

		/// <summary>
		/// Registers a new plugin to be used in FreeImage. The plugin is residing in a DLL.
		/// The Init function must be called �Init� and must use the stdcall calling convention.
		/// </summary>
		/// <param name="path">Complete path to the dll file hosting the plugin.</param>
		/// <param name="format">A string describing the format of the plugin.</param>
		/// <param name="description">A string describing the plugin.</param>
		/// <param name="extension">A string witha comma sperated list of extensions. f.e: "pl,pl2,pl4"</param>
		/// <param name="regexpr">A regular expression used to identify the bitmap.</param>
		/// <returns>The format idientifier assigned by FreeImage.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_RegisterExternalPlugin")]
		public static extern FREE_IMAGE_FORMAT RegisterExternalPlugin(string path,
			string format, string description, string extension, string regexpr);

		/// <summary>
		/// Retrieves the number of FREE_IMAGE_FORMAT identifiers being currently registered.
		/// </summary>
		/// <returns>The number of registered formats.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFIFCount")]
		public static extern int GetFIFCount();

		/// <summary>
		/// Enables or disables a plugin.
		/// </summary>
		/// <param name="fif">The plugin to enable or disable.</param>
		/// <param name="enable">True: enable the plugin. false: disable the plugin.</param>
		/// <returns>The previous state of the plugin.
		/// 1 - enabled. 0 - disables. -1 plugin does not exist.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetPluginEnabled")]
		public static extern int SetPluginEnabled(FREE_IMAGE_FORMAT fif, bool enable);

		/// <summary>
		/// Retrieves the state of a plugin.
		/// </summary>
		/// <param name="fif">The plugin to check.</param>
		/// <returns>1 - enabled. 0 - disables. -1 plugin does not exist.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_IsPluginEnabled")]
		public static extern int IsPluginEnabled(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// Returns a <see cref="FREE_IMAGE_FORMAT"/> identifier from the format string that was used to register the FIF.
		/// </summary>
		/// <param name="format">The string that was used to register the plugin.</param>
		/// <returns>A <see cref="FREE_IMAGE_FORMAT"/> identifier from the format.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_GetFIFFromFormat")]
		public static extern FREE_IMAGE_FORMAT GetFIFFromFormat(string format);

		/// <summary>
		/// Returns a <see cref="FREE_IMAGE_FORMAT"/> identifier from a MIME content type string
		/// (MIME stands for Multipurpose Internet Mail Extension).
		/// </summary>
		/// <param name="mime">A MIME content type.</param>
		/// <returns>A <see cref="FREE_IMAGE_FORMAT"/> identifier from the MIME.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_GetFIFFromMime")]
		public static extern FREE_IMAGE_FORMAT GetFIFFromMime(string mime);

		/// <summary>
		/// Returns the string that was used to register a plugin from the system assigned <see cref="FREE_IMAGE_FORMAT"/>.
		/// </summary>
		/// <param name="fif">The assigned <see cref="FREE_IMAGE_FORMAT"/>.</param>
		/// <returns>The string that was used to register the plugin.</returns>
		public static unsafe string GetFormatFromFIF(FREE_IMAGE_FORMAT fif) { return PtrToStr(GetFormatFromFIF_(fif)); }
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFormatFromFIF")]
		private static unsafe extern byte* GetFormatFromFIF_(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// Returns a comma-delimited file extension list describing the bitmap formats the given plugin can read and/or write.
		/// </summary>
		/// <param name="fif">The desired <see cref="FREE_IMAGE_FORMAT"/>.</param>
		/// <returns>A comma-delimited file extension list.</returns>
		public static unsafe string GetFIFExtensionList(FREE_IMAGE_FORMAT fif) { return PtrToStr(GetFIFExtensionList_(fif)); }
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFIFExtensionList")]
		private static unsafe extern byte* GetFIFExtensionList_(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// Returns a descriptive string that describes the bitmap formats the given plugin can read and/or write.
		/// </summary>
		/// <param name="fif">The desired <see cref="FREE_IMAGE_FORMAT"/>.</param>
		/// <returns>A descriptive string that describes the bitmap formats.</returns>
		public static unsafe string GetFIFDescription(FREE_IMAGE_FORMAT fif) { return PtrToStr(GetFIFDescription_(fif)); }
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFIFDescription")]
		private static unsafe extern byte* GetFIFDescription_(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// Returns a regular expression string that can be used by a regular expression engine to identify the bitmap.
		/// FreeImageQt makes use of this function.
		/// </summary>
		/// <param name="fif">The desired <see cref="FREE_IMAGE_FORMAT"/>.</param>
		/// <returns>A regular expression string.</returns>
		public static unsafe string GetFIFRegExpr(FREE_IMAGE_FORMAT fif) { return PtrToStr(GetFIFRegExpr_(fif)); }
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFIFRegExpr")]
		private static unsafe extern byte* GetFIFRegExpr_(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// Given a <see cref="FREE_IMAGE_FORMAT"/> identifier, returns a MIME content type string (MIME stands for Multipurpose Internet Mail Extension).
		/// </summary>
		/// <param name="fif">The desired <see cref="FREE_IMAGE_FORMAT"/>.</param>
		/// <returns>A MIME content type string.</returns>
		public static unsafe string GetFIFMimeType(FREE_IMAGE_FORMAT fif) { return PtrToStr(GetFIFMimeType_(fif)); }
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFIFMimeType")]
		private static unsafe extern byte* GetFIFMimeType_(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// This function takes a filename or a file-extension and returns the plugin that can
		/// read/write files with that extension in the form of a <see cref="FREE_IMAGE_FORMAT"/> identifier.
		/// </summary>
		/// <param name="filename">The filename or -extension.</param>
		/// <returns>The <see cref="FREE_IMAGE_FORMAT"/> of the plugin.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_GetFIFFromFilenameU")]
		public static extern FREE_IMAGE_FORMAT GetFIFFromFilename(string filename);

		/// <summary>
		/// This function takes a filename or a file-extension and returns the plugin that can
		/// read/write files with that extension in the form of a <see cref="FREE_IMAGE_FORMAT"/> identifier.
		/// Supports UNICODE filenames.
		/// </summary>
		/// <param name="filename">The filename or -extension.</param>
		/// <returns>The <see cref="FREE_IMAGE_FORMAT"/> of the plugin.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_GetFIFFromFilenameU")]
		private static extern FREE_IMAGE_FORMAT GetFIFFromFilenameU(string filename);

		/// <summary>
		/// Checks if a plugin can load bitmaps.
		/// </summary>
		/// <param name="fif">The <see cref="FREE_IMAGE_FORMAT"/> of the plugin.</param>
		/// <returns>True if the plugin can load bitmaps, else false.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FIFSupportsReading")]
		public static extern bool FIFSupportsReading(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// Checks if a plugin can save bitmaps.
		/// </summary>
		/// <param name="fif">The <see cref="FREE_IMAGE_FORMAT"/> of the plugin.</param>
		/// <returns>True if the plugin can save bitmaps, else false.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FIFSupportsWriting")]
		public static extern bool FIFSupportsWriting(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// Checks if a plugin can save bitmaps in the desired bit depth.
		/// </summary>
		/// <param name="fif">The <see cref="FREE_IMAGE_FORMAT"/> of the plugin.</param>
		/// <param name="bpp">The desired bit depth.</param>
		/// <returns>True if the plugin can save bitmaps in the desired bit depth, else false.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FIFSupportsExportBPP")]
		public static extern bool FIFSupportsExportBPP(FREE_IMAGE_FORMAT fif, int bpp);

		/// <summary>
		/// Checks if a plugin can save a bitmap in the desired data type.
		/// </summary>
		/// <param name="fif">The <see cref="FREE_IMAGE_FORMAT"/> of the plugin.</param>
		/// <param name="type">The desired image type.</param>
		/// <returns>True if the plugin can save bitmaps as the desired type, else false.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FIFSupportsExportType")]
		public static extern bool FIFSupportsExportType(FREE_IMAGE_FORMAT fif, FREE_IMAGE_TYPE type);

		/// <summary>
		/// Checks if a plugin can load or save an ICC profile.
		/// </summary>
		/// <param name="fif">The <see cref="FREE_IMAGE_FORMAT"/> of the plugin.</param>
		/// <returns>True if the plugin can load or save an ICC profile, else false.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FIFSupportsICCProfiles")]
		public static extern bool FIFSupportsICCProfiles(FREE_IMAGE_FORMAT fif);

		#endregion

		#region Multipage functions

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap.
		/// Load flags can be provided by the flags parameter.
		/// </summary>
		/// <param name="fif">Format of the image.</param>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="create_new">When true a new bitmap is created.</param>
		/// <param name="read_only">When true the bitmap will be loaded read only.</param>
		/// <param name="keep_cache_in_memory">When true performance is increased at the cost of memory.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_OpenMultiBitmap")]
		public static extern FIMULTIBITMAP OpenMultiBitmap(FREE_IMAGE_FORMAT fif, string filename, bool create_new,
				bool read_only, bool keep_cache_in_memory, FREE_IMAGE_LOAD_FLAGS flags);

		/// <summary>
		/// Closes a previously opened multi-page bitmap and, when the bitmap was not opened read-only, applies any changes made to it.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_CloseMultiBitmap")]
		public static extern bool CloseMultiBitmap(FIMULTIBITMAP bitmap, FREE_IMAGE_SAVE_FLAGS flags);

		/// <summary>
		/// Returns the number of pages currently available in the multi-paged bitmap.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <returns>Number of pages.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetPageCount")]
		public static extern int GetPageCount(FIMULTIBITMAP bitmap);

		/// <summary>
		/// Appends a new page to the end of the bitmap.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="data">Handle to a FreeImage bitmap.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AppendPage")]
		public static extern void AppendPage(FIMULTIBITMAP bitmap, FIBITMAP data);

		/// <summary>
		/// Inserts a new page before the given position in the bitmap.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="page">Page has to be a number smaller than the current number of pages available in the bitmap.</param>
		/// <param name="data">Handle to a FreeImage bitmap.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_InsertPage")]
		public static extern void InsertPage(FIMULTIBITMAP bitmap, int page, FIBITMAP data);

		/// <summary>
		/// Deletes the page on the given position.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="page">Number of the page to delete.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_DeletePage")]
		public static extern void DeletePage(FIMULTIBITMAP bitmap, int page);

		/// <summary>
		/// Locks a page in memory for editing.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="page">Number of the page to lock.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_LockPage")]
		public static extern FIBITMAP LockPage(FIMULTIBITMAP bitmap, int page);

		/// <summary>
		/// Unlocks a previously locked page and gives it back to the multi-page engine.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="data">Handle to a FreeImage bitmap.</param>
		/// <param name="changed">If true, the page is applied to the multi-page bitmap.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_UnlockPage")]
		public static extern void UnlockPage(FIMULTIBITMAP bitmap, FIBITMAP data, bool changed);

		/// <summary>
		/// Moves the source page to the position of the target page.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="target">New position of the page.</param>
		/// <param name="source">Old position of the page.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_MovePage")]
		public static extern bool MovePage(FIMULTIBITMAP bitmap, int target, int source);

		/// <summary>
		/// Returns an array of page-numbers that are currently locked in memory.
		/// When the pages parameter is null, the size of the array is returned in the count variable.
		/// </summary>
		/// <example>
		/// <code>
		/// int[] lockedPages = null;
		/// int count = 0;
		/// GetLockedPageNumbers(dib, lockedPages, ref count);
		/// lockedPages = new int[count];
		/// GetLockedPageNumbers(dib, lockedPages, ref count);
		/// </code>
		/// </example>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="pages">The list of locked pages in the multi-pages bitmap.
		/// If set to null, count will contain the number of pages.</param>
		/// <param name="count">If <paramref name="pages"/> is set to null count will contain the number of locked pages.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetLockedPageNumbers")]
		public static extern bool GetLockedPageNumbers(FIMULTIBITMAP bitmap, int[] pages, ref int count);

		#endregion

		#region Filetype functions

		/// <summary>
		/// Orders FreeImage to analyze the bitmap signature.
		/// </summary>
		/// <param name="filename">Name of the file to analyze.</param>
		/// <param name="size">Reserved parameter - use 0.</param>
		/// <returns>Type of the bitmap.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_GetFileTypeU")]
		public static extern FREE_IMAGE_FORMAT GetFileType(string filename, int size);


		/// <summary>
		/// Orders FreeImage to analyze the bitmap signature.
		/// Supports UNICODE filenames.
		/// </summary>
		/// <param name="filename">Name of the file to analyze.</param>
		/// <param name="size">Reserved parameter - use 0.</param>
		/// <returns>Type of the bitmap.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_GetFileTypeU")]
		private static extern FREE_IMAGE_FORMAT GetFileTypeU(string filename, int size);

		/// <summary>
		/// Uses the <see cref="FreeImageIO"/> structure as described in the topic bitmap management functions
		/// to identify a bitmap type.
		/// </summary>
		/// <param name="io">A <see cref="FreeImageIO"/> structure with functionpointers to handle the source.</param>
		/// <param name="handle">A handle to the source.</param>
		/// <param name="size">Size in bytes of the source.</param>
		/// <returns>Type of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFileTypeFromHandle")]
		public static extern FREE_IMAGE_FORMAT GetFileTypeFromHandle(ref FreeImageIO io, fi_handle handle, int size);

		/// <summary>
		/// Uses a memory handle to identify a bitmap type.
		/// </summary>
		/// <param name="stream">Pointer to the stream.</param>
		/// <param name="size">Size in bytes of the source.</param>
		/// <returns>Type of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFileTypeFromMemory")]
		public static extern FREE_IMAGE_FORMAT GetFileTypeFromMemory(FIMEMORY stream, int size);

		#endregion

		#region Helper functions

		/// <summary>
		/// Returns whether the platform is using Little Endian.
		/// </summary>
		/// <returns>Returns true if the platform is using Litte Endian, else false.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_IsLittleEndian")]
		public static extern bool IsLittleEndian();

		/// <summary>
		/// Converts a X11 color name into a corresponding RGB value.
		/// </summary>
		/// <param name="szColor">Name of the color to convert.</param>
		/// <param name="nRed">Red component.</param>
		/// <param name="nGreen">Green component.</param>
		/// <param name="nBlue">Blue component.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_LookupX11Color")]
		public static extern bool LookupX11Color(string szColor, out byte nRed, out byte nGreen, out byte nBlue);

		/// <summary>
		/// Converts a SVG color name into a corresponding RGB value.
		/// </summary>
		/// <param name="szColor">Name of the color to convert.</param>
		/// <param name="nRed">Red component.</param>
		/// <param name="nGreen">Green component.</param>
		/// <param name="nBlue">Blue component.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_LookupSVGColor")]
		public static extern bool LookupSVGColor(string szColor, out byte nRed, out byte nGreen, out byte nBlue);

		#endregion

		#region Pixel access functions

		/// <summary>
		/// Returns a pointer to the data-bits of the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Pointer to the data-bits.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetBits")]
		public static extern IntPtr GetBits(FIBITMAP dib);

		/// <summary>
		/// Returns a pointer to the start of the given scanline in the bitmap's data-bits.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline.</param>
		/// <returns>Pointer to the scanline.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetScanLine")]
		public static extern IntPtr GetScanLine(FIBITMAP dib, int scanline);

		/// <summary>
		/// Get the pixel index of a palettized image at position (x, y), including range check (slow access).
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="x">Pixel position in horizontal direction.</param>
		/// <param name="y">Pixel position in vertical direction.</param>
		/// <param name="value">The pixel index.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetPixelIndex")]
		public static extern bool GetPixelIndex(FIBITMAP dib, uint x, uint y, out byte value);

		/// <summary>
		/// Get the pixel color of a 16-, 24- or 32-bit image at position (x, y), including range check (slow access).
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="x">Pixel position in horizontal direction.</param>
		/// <param name="y">Pixel position in vertical direction.</param>
		/// <param name="value">The pixel color.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetPixelColor")]
		public static extern bool GetPixelColor(FIBITMAP dib, uint x, uint y, out RGBQUAD value);

		/// <summary>
		/// Set the pixel index of a palettized image at position (x, y), including range check (slow access).
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="x">Pixel position in horizontal direction.</param>
		/// <param name="y">Pixel position in vertical direction.</param>
		/// <param name="value">The new pixel index.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetPixelIndex")]
		public static extern bool SetPixelIndex(FIBITMAP dib, uint x, uint y, ref byte value);

		/// <summary>
		/// Set the pixel color of a 16-, 24- or 32-bit image at position (x, y), including range check (slow access).
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="x">Pixel position in horizontal direction.</param>
		/// <param name="y">Pixel position in vertical direction.</param>
		/// <param name="value">The new pixel color.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetPixelColor")]
		public static extern bool SetPixelColor(FIBITMAP dib, uint x, uint y, ref RGBQUAD value);

		#endregion

		#region Bitmap information functions

		/// <summary>
		/// Retrieves the type of the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Type of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetImageType")]
		public static extern FREE_IMAGE_TYPE GetImageType(FIBITMAP dib);

		/// <summary>
		/// Returns the number of colors used in a bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Palette-size for palletised bitmaps, and 0 for high-colour bitmaps.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetColorsUsed")]
		public static extern uint GetColorsUsed(FIBITMAP dib);

		/// <summary>
		/// Returns the size of one pixel in the bitmap in bits.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Size of one pixel in the bitmap in bits.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetBPP")]
		public static extern uint GetBPP(FIBITMAP dib);

		/// <summary>
		/// Returns the width of the bitmap in pixel units.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>With of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetWidth")]
		public static extern uint GetWidth(FIBITMAP dib);

		/// <summary>
		/// Returns the height of the bitmap in pixel units.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Height of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetHeight")]
		public static extern uint GetHeight(FIBITMAP dib);

		/// <summary>
		/// Returns the width of the bitmap in bytes.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>With of the bitmap in bytes.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetLine")]
		public static extern uint GetLine(FIBITMAP dib);

		/// <summary>
		/// Returns the width of the bitmap in bytes, rounded to the next 32-bit boundary,
		/// also known as pitch or stride or scan width.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>With of the bitmap in bytes.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetPitch")]
		public static extern uint GetPitch(FIBITMAP dib);

		/// <summary>
		/// Returns the size of the DIB-element of a FIBITMAP in memory.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Size of the DIB-element</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetDIBSize")]
		public static extern uint GetDIBSize(FIBITMAP dib);

		/// <summary>
		/// Returns a pointer to the bitmap's palette.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Pointer to the bitmap's palette.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetPalette")]
		public static extern IntPtr GetPalette(FIBITMAP dib);

		/// <summary>
		/// Returns the horizontal resolution, in pixels-per-meter, of the target device for the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The horizontal resolution, in pixels-per-meter.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetDotsPerMeterX")]
		public static extern uint GetDotsPerMeterX(FIBITMAP dib);

		/// <summary>
		/// Returns the vertical resolution, in pixels-per-meter, of the target device for the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The vertical resolution, in pixels-per-meter.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetDotsPerMeterY")]
		public static extern uint GetDotsPerMeterY(FIBITMAP dib);

		/// <summary>
		/// Set the horizontal resolution, in pixels-per-meter, of the target device for the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="res">The new horizontal resolution.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetDotsPerMeterX")]
		public static extern void SetDotsPerMeterX(FIBITMAP dib, uint res);

		/// <summary>
		/// Set the vertical resolution, in pixels-per-meter, of the target device for the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="res">The new vertical resolution.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetDotsPerMeterY")]
		public static extern void SetDotsPerMeterY(FIBITMAP dib, uint res);

		/// <summary>
		/// Returns a pointer to the <see cref="BITMAPINFOHEADER"/> of the DIB-element in a FIBITMAP.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Poiter to the header of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetInfoHeader")]
		public static extern IntPtr GetInfoHeader(FIBITMAP dib);

		/// <summary>
		/// Alias for FreeImage_GetInfoHeader that returns a pointer to a <see cref="BITMAPINFO"/>
		/// rather than to a <see cref="BITMAPINFOHEADER"/>.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Pointer to the <see cref="BITMAPINFO"/> structure for the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetInfo")]
		public static extern IntPtr GetInfo(FIBITMAP dib);

		/// <summary>
		/// Investigates the color type of the bitmap by reading the bitmap's pixel bits and analysing them.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The color type of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetColorType")]
		public static extern FREE_IMAGE_COLOR_TYPE GetColorType(FIBITMAP dib);

		/// <summary>
		/// Returns a bit pattern describing the red color component of a pixel in a FreeImage bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The bit pattern for RED.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetRedMask")]
		public static extern uint GetRedMask(FIBITMAP dib);

		/// <summary>
		/// Returns a bit pattern describing the green color component of a pixel in a FreeImage bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The bit pattern for green.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetGreenMask")]
		public static extern uint GetGreenMask(FIBITMAP dib);

		/// <summary>
		/// Returns a bit pattern describing the blue color component of a pixel in a FreeImage bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The bit pattern for blue.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetBlueMask")]
		public static extern uint GetBlueMask(FIBITMAP dib);

		/// <summary>
		/// Returns the number of transparent colors in a palletised bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The number of transparent colors in a palletised bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTransparencyCount")]
		public static extern uint GetTransparencyCount(FIBITMAP dib);

		/// <summary>
		/// Returns a pointer to the bitmap's transparency table.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Pointer to the bitmap's transparency table.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTransparencyTable")]
		public static extern IntPtr GetTransparencyTable(FIBITMAP dib);

		/// <summary>
		/// Tells FreeImage if it should make use of the transparency table
		/// or the alpha channel that may accompany a bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="enabled">True to enable the transparency, false to disable.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTransparent")]
		public static extern void SetTransparent(FIBITMAP dib, bool enabled);

		/// <summary>
		/// Set the bitmap's transparency table. Only affects palletised bitmaps.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="table">Pointer to the bitmap's new transparency table.</param>
		/// <param name="count">The number of transparent colors in the new transparency table.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTransparencyTable")]
		internal static extern void SetTransparencyTable_(FIBITMAP dib, byte[] table, int count);

		/// <summary>
		/// Returns whether the transparency table is enabled.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Returns true when the transparency table is enabled (1-, 4- or 8-bit images)
		/// or when the input dib contains alpha values (32-bit images). Returns false otherwise.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_IsTransparent")]
		public static extern bool IsTransparent(FIBITMAP dib);

		/// <summary>
		/// Returns whether the bitmap has a file background color.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Returns true when the image has a file background color, false otherwise.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_HasBackgroundColor")]
		public static extern bool HasBackgroundColor(FIBITMAP dib);

		/// <summary>
		/// Returns the file background color of an image.
		/// For 8-bit images, the color index in the palette is returned in the
		/// rgbReserved member of the bkcolor parameter.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="bkcolor">The background color.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetBackgroundColor")]
		public static extern bool GetBackgroundColor(FIBITMAP dib, out RGBQUAD bkcolor);

		/// <summary>
		/// Set the file background color of an image.
		/// When saving an image to PNG, this background color is transparently saved to the PNG file.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="bkcolor">The new background color.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetBackgroundColor")]
		public static unsafe extern bool SetBackgroundColor(FIBITMAP dib, ref RGBQUAD bkcolor);

		/// <summary>
		/// Set the file background color of an image.
		/// When saving an image to PNG, this background color is transparently saved to the PNG file.
		/// When the bkcolor parameter is null, the background color is removed from the image.
		/// <para>
		/// This overloaded version of the function with an array parameter is provided to allow
		/// passing <c>null</c> in the <paramref name="bkcolor"/> parameter. This is similar to the
		/// original C/C++ function. Passing <c>null</c> as <paramref name="bkcolor"/> parameter will
		/// unset the dib's previously set background color.
		/// </para> 
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="bkcolor">The new background color.
		/// The first entry in the array is used.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <example>
		/// <code>
		/// // create a RGBQUAD color
		/// RGBQUAD color = new RGBQUAD(Color.Green);
		/// 
		/// // set the dib's background color (using the other version of the function)
		/// FreeImage.SetBackgroundColor(dib, ref color);
		/// 
		/// // remove it again (this only works due to the array parameter RGBQUAD[])
		/// FreeImage.SetBackgroundColor(dib, null);
		/// </code>
		/// </example>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetBackgroundColor")]
		public static unsafe extern bool SetBackgroundColor(FIBITMAP dib, RGBQUAD[] bkcolor);

		/// <summary>
		/// Sets the index of the palette entry to be used as transparent color
		/// for the image specified. Does nothing on high color images.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="index">The index of the palette entry to be set as transparent color.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTransparentIndex")]
		public static extern void SetTransparentIndex(FIBITMAP dib, int index);

		/// <summary>
		/// Returns the palette entry used as transparent color for the image specified.
		/// Works for palletised images only and returns -1 for high color
		/// images or if the image has no color set to be transparent.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>the index of the palette entry used as transparent color for
		/// the image specified or -1 if there is no transparent color found
		/// (e.g. the image is a high color image).</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTransparentIndex")]
		public static extern int GetTransparentIndex(FIBITMAP dib);

		#endregion

		#region ICC profile functions

		/// <summary>
		/// Retrieves the <see cref="FIICCPROFILE"/> data of the bitmap.
		/// This function can also be called safely, when the original format does not support profiles.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The <see cref="FIICCPROFILE"/> data of the bitmap.</returns>
		public static FIICCPROFILE GetICCProfileEx(FIBITMAP dib) { unsafe { return *(FIICCPROFILE*)FreeImage.GetICCProfile(dib); } }

		/// <summary>
		/// Retrieves a pointer to the <see cref="FIICCPROFILE"/> data of the bitmap.
		/// This function can also be called safely, when the original format does not support profiles.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Pointer to the <see cref="FIICCPROFILE"/> data of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetICCProfile")]
		public static extern IntPtr GetICCProfile(FIBITMAP dib);

		/// <summary>
		/// Creates a new <see cref="FIICCPROFILE"/> block from ICC profile data previously read from a file
		/// or built by a color management system. The profile data is attached to the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="data">Pointer to the new <see cref="FIICCPROFILE"/> data.</param>
		/// <param name="size">Size of the <see cref="FIICCPROFILE"/> data.</param>
		/// <returns>Pointer to the created <see cref="FIICCPROFILE"/> structure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_CreateICCProfile")]
		public static extern IntPtr CreateICCProfile(FIBITMAP dib, byte[] data, int size);

		/// <summary>
		/// This function destroys an <see cref="FIICCPROFILE"/> previously created by <see cref="CreateICCProfile(FIBITMAP,byte[],int)"/>.
		/// After this call the bitmap will contain no profile information.
		/// This function should be called to ensure that a stored bitmap will not contain any profile information.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_DestroyICCProfile")]
		public static extern void DestroyICCProfile(FIBITMAP dib);

		#endregion

		#region Internal Functions

		/*
		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine1To4")]
		public static extern void ConvertLine1To4(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine8To4")]
		public static extern void ConvertLine8To4(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine16To4_555")]
		public static extern void ConvertLine16To4_555(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine16To4_565")]
		public static extern void ConvertLine16To4_565(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine24To4")]
		public static extern void ConvertLine24To4(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine32To4")]
		public static extern void ConvertLine32To4(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine1To8")]
		public static extern void ConvertLine1To8(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine4To8")]
		public static extern void ConvertLine4To8(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine16To8_555")]
		public static extern void ConvertLine16To8_555(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine16To8_565")]
		public static extern void ConvertLine16To8_565(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine24To8")]
		public static extern void ConvertLine24To8(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine32To8")]
		public static extern void ConvertLine32To8(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine1To16_555")]
		public static extern void ConvertLine1To16_555(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine4To16_555")]
		public static extern void ConvertLine4To16_555(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine8To16_555")]
		public static extern void ConvertLine8To16_555(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine16_565_To16_555")]
		public static extern void ConvertLine16_565_To16_555(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine24To16_555")]
		public static extern void ConvertLine24To16_555(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine32To16_555")]
		public static extern void ConvertLine32To16_555(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine1To16_565")]
		public static extern void ConvertLine1To16_565(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine4To16_565")]
		public static extern void ConvertLine4To16_565(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine8To16_565")]
		public static extern void ConvertLine8To16_565(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine16_555_To16_565")]
		public static extern void ConvertLine16_555_To16_565(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine24To16_565")]
		public static extern void ConvertLine24To16_565(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine32To16_565")]
		public static extern void ConvertLine32To16_565(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine1To24")]
		public static extern void ConvertLine1To24(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine4To24")]
		public static extern void ConvertLine4To24(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine8To24")]
		public static extern void ConvertLine8To24(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine16To24_555")]
		public static extern void ConvertLine16To24_555(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine16To24_565")]
		public static extern void ConvertLine16To24_565(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine32To24")]
		public static extern void ConvertLine32To24(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine1To32")]
		public static extern void ConvertLine1To32(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine4To32")]
		public static extern void ConvertLine4To32(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine8To32")]
		public static extern void ConvertLine8To32(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine16To32_555")]
		public static extern void ConvertLine16To32_555(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine16To32_565")]
		public static extern void ConvertLine16To32_565(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine24To32")]
		public static extern void ConvertLine24To32(ref byte target, ref byte source, int width_in_pixels);

		*/

		#endregion

		#region Conversion functions

		/// <summary>
		/// Converts a bitmap to 4 bits.
		/// If the bitmap was a high-color bitmap (16, 24 or 32-bit) or if it was a
		/// monochrome or greyscale bitmap (1 or 8-bit), the end result will be a
		/// greyscale bitmap, otherwise (1-bit palletised bitmaps) it will be a palletised bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertTo4Bits")]
		public static extern FIBITMAP ConvertTo4Bits(FIBITMAP dib);

		/// <summary>
		/// Converts a bitmap to 8 bits. If the bitmap was a high-color bitmap (16, 24 or 32-bit)
		/// or if it was a monochrome or greyscale bitmap (1 or 4-bit), the end result will be a
		/// greyscale bitmap, otherwise (1 or 4-bit palletised bitmaps) it will be a palletised bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertTo8Bits")]
		public static extern FIBITMAP ConvertTo8Bits(FIBITMAP dib);

		/// <summary>
		/// Converts a bitmap to a 8-bit greyscale image with a linear ramp.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertToGreyscale")]
		public static extern FIBITMAP ConvertToGreyscale(FIBITMAP dib);

		/// <summary>
		/// Converts a bitmap to 16 bits, where each pixel has a color pattern of
		/// 5 bits red, 5 bits green and 5 bits blue. One bit in each pixel is unused.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertTo16Bits555")]
		public static extern FIBITMAP ConvertTo16Bits555(FIBITMAP dib);

		/// <summary>
		/// Converts a bitmap to 16 bits, where each pixel has a color pattern of
		/// 5 bits red, 6 bits green and 5 bits blue.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertTo16Bits565")]
		public static extern FIBITMAP ConvertTo16Bits565(FIBITMAP dib);

		/// <summary>
		/// Converts a bitmap to 24 bits. A clone of the input bitmap is returned for 24-bit bitmaps.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertTo24Bits")]
		public static extern FIBITMAP ConvertTo24Bits(FIBITMAP dib);

		/// <summary>
		/// Converts a bitmap to 32 bits. A clone of the input bitmap is returned for 32-bit bitmaps.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertTo32Bits")]
		public static extern FIBITMAP ConvertTo32Bits(FIBITMAP dib);

		/// <summary>
		/// Quantizes a high-color 24-bit bitmap to an 8-bit palette color bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="quantize">Specifies the color reduction algorithm to be used.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ColorQuantize")]
		public static extern FIBITMAP ColorQuantize(FIBITMAP dib, FREE_IMAGE_QUANTIZE quantize);

		/// <summary>
		/// ColorQuantizeEx is an extension to the <see cref="ColorQuantize(FIBITMAP, FREE_IMAGE_QUANTIZE)"/> method that
		/// provides additional options used to quantize a 24-bit image to any
		/// number of colors (up to 256), as well as quantize a 24-bit image using a
		/// partial or full provided palette.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="quantize">Specifies the color reduction algorithm to be used.</param>
		/// <param name="PaletteSize">Size of the desired output palette.</param>
		/// <param name="ReserveSize">Size of the provided palette of ReservePalette.</param>
		/// <param name="ReservePalette">The provided palette.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ColorQuantizeEx")]
		public static extern FIBITMAP ColorQuantizeEx(FIBITMAP dib, FREE_IMAGE_QUANTIZE quantize, int PaletteSize, int ReserveSize, RGBQUAD[] ReservePalette);

		/// <summary>
		/// Converts a bitmap to 1-bit monochrome bitmap using a threshold T between [0..255].
		/// The function first converts the bitmap to a 8-bit greyscale bitmap.
		/// Then, any brightness level that is less than T is set to zero, otherwise to 1.
		/// For 1-bit input bitmaps, the function clones the input bitmap and builds a monochrome palette.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="t">The threshold.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Threshold")]
		public static extern FIBITMAP Threshold(FIBITMAP dib, byte t);

		/// <summary>
		/// Converts a bitmap to 1-bit monochrome bitmap using a dithering algorithm.
		/// For 1-bit input bitmaps, the function clones the input bitmap and builds a monochrome palette.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="algorithm">The dithering algorithm to use.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Dither")]
		public static extern FIBITMAP Dither(FIBITMAP dib, FREE_IMAGE_DITHER algorithm);

		/// <summary>
		/// Converts a raw bitmap somewhere in memory to a FreeImage bitmap.
		/// The parameters in this function are used to describe the raw bitmap.
		/// </summary>
		/// <param name="bits">Pointer to start of the raw bits.</param>
		/// <param name="width">Width of the bitmap.</param>
		/// <param name="height">Height of the bitmap.</param>
		/// <param name="pitch">Defines the total width of a scanline in the source bitmap,
		/// including padding bytes that may be applied.</param>
		/// <param name="bpp">The bit depth of the bitmap.</param>
		/// <param name="red_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="green_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="blue_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="topdown">Stores the bitmap top-left pixel first when it is true
		/// or bottom-left pixel first when it is false</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertFromRawBits")]
		public static extern FIBITMAP ConvertFromRawBits(IntPtr bits, int width, int height, int pitch,
				uint bpp, uint red_mask, uint green_mask, uint blue_mask, bool topdown);

		/// <summary>
		/// Converts a FreeImage bitmap to a raw piece of memory.
		/// </summary>
		/// <param name="bits">Pointer to the start of the raw bits.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="pitch">Defines the total width of a scanline in the source bitmap,
		/// including padding bytes that may be applied.</param>
		/// <param name="bpp">The bit depth of the bitmap.</param>
		/// <param name="red_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="green_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="blue_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="topdown">Store the bitmap top-left pixel first when it is true
		/// or bottom-left pixel first when it is false.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertToRawBits")]
		public static extern void ConvertToRawBits(IntPtr bits, FIBITMAP dib, int pitch, uint bpp,
				uint red_mask, uint green_mask, uint blue_mask, bool topdown);

		/// <summary>
		/// Converts a 24- or 32-bit RGB(A) standard image or a 48-bit RGB image to a FIT_RGBF type image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertToRGBF")]
		public static extern FIBITMAP ConvertToRGBF(FIBITMAP dib);

		/// <summary>
		/// Converts a non standard image whose color type is FIC_MINISBLACK
		/// to a standard 8-bit greyscale image.
		/// </summary>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="scale_linear">When true the conversion is done by scaling linearly
		/// each pixel value from [min, max] to an integer value between [0..255],
		/// where min and max are the minimum and maximum pixel values in the image.
		/// When false the conversion is done by rounding each pixel value to an integer between [0..255].
		///
		/// Rounding is done using the following formula:
		///
		/// dst_pixel = (BYTE) MIN(255, MAX(0, q)) where int q = int(src_pixel + 0.5);</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertToStandardType")]
		public static extern FIBITMAP ConvertToStandardType(FIBITMAP src, bool scale_linear);

		/// <summary>
		/// Converts an image of any type to type dst_type.
		/// </summary>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="dst_type">Destination type.</param>
		/// <param name="scale_linear">True to scale linear, else false.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertToType")]
		public static extern FIBITMAP ConvertToType(FIBITMAP src, FREE_IMAGE_TYPE dst_type, bool scale_linear);

		#endregion

		#region Tone mapping operators

		/// <summary>
		/// Converts a High Dynamic Range image (48-bit RGB or 96-bit RGBF) to a 24-bit RGB image, suitable for display.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="tmo">The tone mapping operator to be used.</param>
		/// <param name="first_param">Parmeter depending on the used algorithm</param>
		/// <param name="second_param">Parmeter depending on the used algorithm</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ToneMapping")]
		public static extern FIBITMAP ToneMapping(FIBITMAP dib, FREE_IMAGE_TMO tmo, double first_param, double second_param);

		/// <summary>
		/// Converts a High Dynamic Range image to a 24-bit RGB image using a global
		/// operator based on logarithmic compression of luminance values, imitating the human response to light.
		/// </summary>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="gamma">A gamma correction that is applied after the tone mapping.
		/// A value of 1 means no correction.</param>
		/// <param name="exposure">Scale factor allowing to adjust the brightness of the output image.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_TmoDrago03")]
		public static extern FIBITMAP TmoDrago03(FIBITMAP src, double gamma, double exposure);

		/// <summary>
		/// Converts a High Dynamic Range image to a 24-bit RGB image using a global operator inspired
		/// by photoreceptor physiology of the human visual system.
		/// </summary>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="intensity">Controls the overall image intensity in the range [-8, 8].</param>
		/// <param name="contrast">Controls the overall image contrast in the range [0.3, 1.0[.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_TmoReinhard05")]
		public static extern FIBITMAP TmoReinhard05(FIBITMAP src, double intensity, double contrast);

		/// <summary>
		/// Apply the Gradient Domain High Dynamic Range Compression to a RGBF image and convert to 24-bit RGB.
		/// </summary>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="color_saturation">Color saturation (s parameter in the paper) in [0.4..0.6]</param>
		/// <param name="attenuation">Atenuation factor (beta parameter in the paper) in [0.8..0.9]</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_TmoFattal02")]
		public static extern FIBITMAP TmoFattal02(FIBITMAP src, double color_saturation, double attenuation);

		#endregion

		#region Compression functions

		/// <summary>
		/// Compresses a source buffer into a target buffer, using the ZLib library.
		/// </summary>
		/// <param name="target">Pointer to the target buffer.</param>
		/// <param name="target_size">Size of the target buffer.
		/// Must be at least 0.1% larger than source_size plus 12 bytes.</param>
		/// <param name="source">Pointer to the source buffer.</param>
		/// <param name="source_size">Size of the source buffer.</param>
		/// <returns>The actual size of the compressed buffer, or 0 if an error occurred.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ZLibCompress")]
		public static extern uint ZLibCompress(byte[] target, uint target_size, byte[] source, uint source_size);

		/// <summary>
		/// Decompresses a source buffer into a target buffer, using the ZLib library.
		/// </summary>
		/// <param name="target">Pointer to the target buffer.</param>
		/// <param name="target_size">Size of the target buffer.
		/// Must have been saved outlide of zlib.</param>
		/// <param name="source">Pointer to the source buffer.</param>
		/// <param name="source_size">Size of the source buffer.</param>
		/// <returns>The actual size of the uncompressed buffer, or 0 if an error occurred.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ZLibUncompress")]
		public static extern uint ZLibUncompress(byte[] target, uint target_size, byte[] source, uint source_size);

		/// <summary>
		/// Compresses a source buffer into a target buffer, using the ZLib library.
		/// </summary>
		/// <param name="target">Pointer to the target buffer.</param>
		/// <param name="target_size">Size of the target buffer.
		/// Must be at least 0.1% larger than source_size plus 24 bytes.</param>
		/// <param name="source">Pointer to the source buffer.</param>
		/// <param name="source_size">Size of the source buffer.</param>
		/// <returns>The actual size of the compressed buffer, or 0 if an error occurred.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ZLibGZip")]
		public static extern uint ZLibGZip(byte[] target, uint target_size, byte[] source, uint source_size);

		/// <summary>
		/// Decompresses a source buffer into a target buffer, using the ZLib library.
		/// </summary>
		/// <param name="target">Pointer to the target buffer.</param>
		/// <param name="target_size">Size of the target buffer.
		/// Must have been saved outlide of zlib.</param>
		/// <param name="source">Pointer to the source buffer.</param>
		/// <param name="source_size">Size of the source buffer.</param>
		/// <returns>The actual size of the uncompressed buffer, or 0 if an error occurred.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ZLibGUnzip")]
		public static extern uint ZLibGUnzip(byte[] target, uint target_size, byte[] source, uint source_size);

		/// <summary>
		/// Generates a CRC32 checksum.
		/// </summary>
		/// <param name="crc">The CRC32 checksum to begin with.</param>
		/// <param name="source">Pointer to the source buffer.
		/// If the value is 0, the function returns the required initial value for the crc.</param>
		/// <param name="source_size">Size of the source buffer.</param>
		/// <returns></returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ZLibCRC32")]
		public static extern uint ZLibCRC32(uint crc, byte[] source, uint source_size);

		#endregion

		#region Tag creation and destruction

		/// <summary>
		/// Allocates a new <see cref="FITAG"/> object.
		/// This object must be destroyed with a call to
		/// <see cref="FreeImageAPI.FreeImage.DeleteTag(FITAG)"/> when no longer in use.
		/// </summary>
		/// <returns>The new <see cref="FITAG"/>.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_CreateTag")]
		public static extern FITAG CreateTag();

		/// <summary>
		/// Delete a previously allocated <see cref="FITAG"/> object.
		/// </summary>
		/// <param name="tag">The <see cref="FITAG"/> to destroy.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_DeleteTag")]
		public static extern void DeleteTag(FITAG tag);

		/// <summary>
		/// Creates and returns a copy of a <see cref="FITAG"/> object.
		/// </summary>
		/// <param name="tag">The <see cref="FITAG"/> to clone.</param>
		/// <returns>The new <see cref="FITAG"/>.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_CloneTag")]
		public static extern FITAG CloneTag(FITAG tag);

		#endregion

		#region Tag accessors

		/// <summary>
		/// Returns the tag field name (unique inside a metadata model).
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <returns>The field name.</returns>
		public static unsafe string GetTagKey(FITAG tag) { return PtrToStr(GetTagKey_(tag)); }
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_GetTagKey")]
		private static unsafe extern byte* GetTagKey_(FITAG tag);

		/// <summary>
		/// Returns the tag description.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <returns>The description or NULL if unavailable.</returns>
		public static unsafe string GetTagDescription(FITAG tag) { return PtrToStr(GetTagDescription_(tag)); }
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_GetTagDescription")]
		private static unsafe extern byte* GetTagDescription_(FITAG tag);

		/// <summary>
		/// Returns the tag ID.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <returns>The ID or 0 if unavailable.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTagID")]
		public static extern ushort GetTagID(FITAG tag);

		/// <summary>
		/// Returns the tag data type.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <returns>The tag type.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTagType")]
		public static extern FREE_IMAGE_MDTYPE GetTagType(FITAG tag);

		/// <summary>
		/// Returns the number of components in the tag (in tag type units).
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <returns>The number of components.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTagCount")]
		public static extern uint GetTagCount(FITAG tag);

		/// <summary>
		/// Returns the length of the tag value in bytes.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <returns>The length of the tag value.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTagLength")]
		public static extern uint GetTagLength(FITAG tag);

		/// <summary>
		/// Returns the tag value.
		/// It is up to the programmer to interpret the returned pointer correctly,
		/// according to the results of GetTagType and GetTagCount.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <returns>Pointer to the value.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTagValue")]
		public static extern IntPtr GetTagValue(FITAG tag);

		/// <summary>
		/// Sets the tag field name.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <param name="key">The new name.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_SetTagKey")]
		public static extern bool SetTagKey(FITAG tag, string key);

		/// <summary>
		/// Sets the tag description.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <param name="description">The new description.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_SetTagDescription")]
		public static extern bool SetTagDescription(FITAG tag, string description);

		/// <summary>
		/// Sets the tag ID.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <param name="id">The new ID.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTagID")]
		public static extern bool SetTagID(FITAG tag, ushort id);

		/// <summary>
		/// Sets the tag data type.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <param name="type">The new type.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTagType")]
		public static extern bool SetTagType(FITAG tag, FREE_IMAGE_MDTYPE type);

		/// <summary>
		/// Sets the number of data in the tag.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <param name="count">New number of data.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTagCount")]
		public static extern bool SetTagCount(FITAG tag, uint count);

		/// <summary>
		/// Sets the length of the tag value in bytes.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <param name="length">The new length.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTagLength")]
		public static extern bool SetTagLength(FITAG tag, uint length);

		/// <summary>
		/// Sets the tag value.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <param name="value">Pointer to the new value.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTagValue")]
		public static extern bool SetTagValue(FITAG tag, byte[] value);
		//public static extern bool SetTagValue(FITAG tag, IntPtr value);

		#endregion

		#region Metadata iterator

		/// <summary>
		/// Provides information about the first instance of a tag that matches the metadata model.
		/// </summary>
		/// <param name="model">The model to match.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="tag">Tag that matches the metadata model.</param>
		/// <returns>Unique search handle that can be used to call FindNextMetadata or FindCloseMetadata.
		/// Null if the metadata model does not exist.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FindFirstMetadata")]
		public static extern FIMETADATA FindFirstMetadata(FREE_IMAGE_MDMODEL model, FIBITMAP dib, out FITAG tag);

		/// <summary>
		/// Find the next tag, if any, that matches the metadata model argument in a previous call
		/// to FindFirstMetadata, and then alters the tag object contents accordingly.
		/// </summary>
		/// <param name="mdhandle">Unique search handle provided by FindFirstMetadata.</param>
		/// <param name="tag">Tag that matches the metadata model.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FindNextMetadata")]
		public static extern bool FindNextMetadata(FIMETADATA mdhandle, out FITAG tag);

		/// <summary>
		/// Closes the specified metadata search handle and releases associated resources.
		/// </summary>
		/// <param name="mdhandle">The handle to close.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FindCloseMetadata")]
		private static extern void FindCloseMetadata_(FIMETADATA mdhandle);

		#endregion

		#region Metadata setter and getter

		/// <summary>
		/// Retrieve a metadata attached to a dib.
		/// </summary>
		/// <param name="model">The metadata model to look for.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="key">The metadata field name.</param>
		/// <param name="tag">A FITAG structure returned by the function.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_GetMetadata")]
		public static extern bool GetMetadata(FREE_IMAGE_MDMODEL model, FIBITMAP dib, string key, out FITAG tag);

		/// <summary>
		/// Attach a new FreeImage tag to a dib.
		/// </summary>
		/// <param name="model">The metadata model used to store the tag.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="key">The tag field name.</param>
		/// <param name="tag">The FreeImage tag to be attached.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_SetMetadata")]
		public static extern bool SetMetadata(FREE_IMAGE_MDMODEL model, FIBITMAP dib, string key, FITAG tag);

		#endregion

		#region Metadata helper functions

		/// <summary>
		/// Returns the number of tags contained in the model metadata model attached to the input dib.
		/// </summary>
		/// <param name="model">The metadata model.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Number of tags contained in the metadata model.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetMetadataCount")]
		public static extern uint GetMetadataCount(FREE_IMAGE_MDMODEL model, FIBITMAP dib);

		/// <summary>
		/// Copies the metadata of FreeImage bitmap to another.
		/// </summary>
		/// <param name="dst">The FreeImage bitmap to copy the metadata to.</param>
		/// <param name="src">The FreeImage bitmap to copy the metadata from.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_CloneMetadata")]
		public static extern bool CloneMetadata(FIBITMAP dst, FIBITMAP src);

		/// <summary>
		/// Converts a FreeImage tag structure to a string that represents the interpreted tag value.
		/// The function is not thread safe.
		/// </summary>
		/// <param name="model">The metadata model.</param>
		/// <param name="tag">The interpreted tag value.</param>
		/// <param name="Make">Reserved.</param>
		/// <returns>The representing string.</returns>
		public static unsafe string TagToString(FREE_IMAGE_MDMODEL model, FITAG tag, uint Make) { return PtrToStr(TagToString_(model, tag, Make)); }
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_TagToString")]
		private static unsafe extern byte* TagToString_(FREE_IMAGE_MDMODEL model, FITAG tag, uint Make);

		#endregion

		#region Rotation and flipping

		/// <summary>
		/// This function rotates a 1-, 8-bit greyscale or a 24-, 32-bit color image by means of 3 shears.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="angle">The angle of rotation.</param>
		/// <returns>Handle to a FreeImage bitmap.
		/// 1-bit images rotation is limited to integer multiple of 90�.
		/// Null is returned for other values.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_RotateClassic")]
		public static extern FIBITMAP RotateClassic(FIBITMAP dib, double angle);

		/// <summary>
		/// This function performs a rotation and / or translation of an 8-bit greyscale,
		/// 24- or 32-bit image, using a 3rd order (cubic) B-Spline.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="angle">The angle of rotation.</param>
		/// <param name="x_shift">Horizontal image translation.</param>
		/// <param name="y_shift">Vertical image translation.</param>
		/// <param name="x_origin">Rotation center x-coordinate.</param>
		/// <param name="y_origin">Rotation center y-coordinate.</param>
		/// <param name="use_mask">When true the irrelevant part of the image is set to a black color,
		/// otherwise, a mirroring technique is used to fill irrelevant pixels.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_RotateEx")]
		public static extern FIBITMAP RotateEx(FIBITMAP dib, double angle,
			double x_shift, double y_shift, double x_origin, double y_origin, bool use_mask);

		/// <summary>
		/// Flip the input dib horizontally along the vertical axis.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FlipHorizontal")]
		public static extern bool FlipHorizontal(FIBITMAP dib);

		/// <summary>
		/// Flip the input dib vertically along the horizontal axis.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FlipVertical")]
		public static extern bool FlipVertical(FIBITMAP dib);

		/// <summary>
		/// Performs a lossless rotation or flipping on a JPEG file.
		/// </summary>
		/// <param name="src_file">Source file.</param>
		/// <param name="dst_file">Destination file; can be the source file; will be overwritten.</param>
		/// <param name="operation">The operation to apply.</param>
		/// <param name="perfect">To avoid lossy transformation, you can set the perfect parameter to true.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_JPEGTransform")]
		public static extern bool JPEGTransform(string src_file, string dst_file,
			FREE_IMAGE_JPEG_OPERATION operation, bool perfect);

		#endregion

		#region Upsampling / downsampling

		/// <summary>
		/// Performs resampling (or scaling, zooming) of a greyscale or RGB(A) image
		/// to the desired destination width and height.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="dst_width">Destination width.</param>
		/// <param name="dst_height">Destination height.</param>
		/// <param name="filter">The filter to apply.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Rescale")]
		public static extern FIBITMAP Rescale(FIBITMAP dib, int dst_width, int dst_height, FREE_IMAGE_FILTER filter);

		/// <summary>
		/// Creates a thumbnail from a greyscale or RGB(A) image, keeping aspect ratio.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="max_pixel_size">Thumbnail square size.</param>
		/// <param name="convert">When true HDR images are transperantly converted to standard images.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_MakeThumbnail")]
		public static extern FIBITMAP MakeThumbnail(FIBITMAP dib, int max_pixel_size, bool convert);

		#endregion

		#region Color manipulation

		/// <summary>
		/// Perfoms an histogram transformation on a 8-, 24- or 32-bit image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="LUT">The lookup table (LUT).
		/// It's size is assumed to be 256 in length.</param>
		/// <param name="channel">The color channel to be transformed.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AdjustCurve")]
		public static extern bool AdjustCurve(FIBITMAP dib, byte[] LUT, FREE_IMAGE_COLOR_CHANNEL channel);

		/// <summary>
		/// Performs gamma correction on a 8-, 24- or 32-bit image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="gamma">The parameter represents the gamma value to use (gamma > 0).
		/// A value of 1.0 leaves the image alone, less than one darkens it, and greater than one lightens it.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AdjustGamma")]
		public static extern bool AdjustGamma(FIBITMAP dib, double gamma);

		/// <summary>
		/// Adjusts the brightness of a 8-, 24- or 32-bit image by a certain amount.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="percentage">A value 0 means no change,
		/// less than 0 will make the image darker and greater than 0 will make the image brighter.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AdjustBrightness")]
		public static extern bool AdjustBrightness(FIBITMAP dib, double percentage);

		/// <summary>
		/// Adjusts the contrast of a 8-, 24- or 32-bit image by a certain amount.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="percentage">A value 0 means no change,
		/// less than 0 will decrease the contrast and greater than 0 will increase the contrast of the image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AdjustContrast")]
		public static extern bool AdjustContrast(FIBITMAP dib, double percentage);

		/// <summary>
		/// Inverts each pixel data.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Invert")]
		public static extern bool Invert(FIBITMAP dib);

		/// <summary>
		/// Computes the image histogram.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="histo">Array of integers with a size of 256.</param>
		/// <param name="channel">Channel to compute from.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetHistogram")]
		public static extern bool GetHistogram(FIBITMAP dib, int[] histo, FREE_IMAGE_COLOR_CHANNEL channel);

		#endregion

		#region Channel processing

		/// <summary>
		/// Retrieves the red, green, blue or alpha channel of a 24- or 32-bit image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="channel">The color channel to extract.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetChannel")]
		public static extern FIBITMAP GetChannel(FIBITMAP dib, FREE_IMAGE_COLOR_CHANNEL channel);

		/// <summary>
		/// Insert a 8-bit dib into a 24- or 32-bit image.
		/// Both images must have to same width and height.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="dib8">Handle to the bitmap to insert.</param>
		/// <param name="channel">The color channel to replace.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetChannel")]
		public static extern bool SetChannel(FIBITMAP dib, FIBITMAP dib8, FREE_IMAGE_COLOR_CHANNEL channel);

		/// <summary>
		/// Retrieves the real part, imaginary part, magnitude or phase of a complex image.
		/// </summary>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="channel">The color channel to extract.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetComplexChannel")]
		public static extern FIBITMAP GetComplexChannel(FIBITMAP src, FREE_IMAGE_COLOR_CHANNEL channel);

		/// <summary>
		/// Set the real or imaginary part of a complex image.
		/// Both images must have to same width and height.
		/// </summary>
		/// <param name="dst">Handle to a FreeImage bitmap.</param>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="channel">The color channel to replace.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetComplexChannel")]
		public static extern bool SetComplexChannel(FIBITMAP dst, FIBITMAP src, FREE_IMAGE_COLOR_CHANNEL channel);

		#endregion

		#region Copy / Paste / Composite routines

		/// <summary>
		/// Copy a sub part of the current dib image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="left">Specifies the left position of the cropped rectangle.</param>
		/// <param name="top">Specifies the top position of the cropped rectangle.</param>
		/// <param name="right">Specifies the right position of the cropped rectangle.</param>
		/// <param name="bottom">Specifies the bottom position of the cropped rectangle.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Copy")]
		public static extern FIBITMAP Copy(FIBITMAP dib, int left, int top, int right, int bottom);

		/// <summary>
		/// Alpha blend or combine a sub part image with the current dib image.
		/// The bit depth of the dst bitmap must be greater than or equal to the bit depth of the src.
		/// </summary>
		/// <param name="dst">Handle to a FreeImage bitmap.</param>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="left">Specifies the left position of the sub image.</param>
		/// <param name="top">Specifies the top position of the sub image.</param>
		/// <param name="alpha">alpha blend factor.
		/// The source and destination images are alpha blended if alpha=0..255.
		/// If alpha > 255, then the source image is combined to the destination image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Paste")]
		public static extern bool Paste(FIBITMAP dst, FIBITMAP src, int left, int top, int alpha);

		/// <summary>
		/// This function composite a transparent foreground image against a single background color or
		/// against a background image.
		/// </summary>
		/// <param name="fg">Handle to a FreeImage bitmap.</param>
		/// <param name="useFileBkg">When true the background of fg is used if it contains one.</param>
		/// <param name="appBkColor">The application background is used if useFileBkg is false.</param>
		/// <param name="bg">Image used as background when useFileBkg is false or fg has no background
		/// and appBkColor is null.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Composite")]
		public static extern FIBITMAP Composite(FIBITMAP fg, bool useFileBkg, ref RGBQUAD appBkColor, FIBITMAP bg);

		/// <summary>
		/// This function composite a transparent foreground image against a single background color or
		/// against a background image.
		/// </summary>
		/// <param name="fg">Handle to a FreeImage bitmap.</param>
		/// <param name="useFileBkg">When true the background of fg is used if it contains one.</param>
		/// <param name="appBkColor">The application background is used if useFileBkg is false
		/// and 'appBkColor' is not null.</param>
		/// <param name="bg">Image used as background when useFileBkg is false or fg has no background
		/// and appBkColor is null.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Composite")]
		public static extern FIBITMAP Composite(FIBITMAP fg, bool useFileBkg, RGBQUAD[] appBkColor, FIBITMAP bg);

		/// <summary>
		/// Performs a lossless crop on a JPEG file.
		/// </summary>
		/// <param name="src_file">Source filename.</param>
		/// <param name="dst_file">Destination filename.</param>
		/// <param name="left">Specifies the left position of the cropped rectangle.</param>
		/// <param name="top">Specifies the top position of the cropped rectangle.</param>
		/// <param name="right">Specifies the right position of the cropped rectangle.</param>
		/// <param name="bottom">Specifies the bottom position of the cropped rectangle.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_JPEGCrop")]
		public static extern bool JPEGCrop(string src_file, string dst_file, int left, int top, int right, int bottom);

		/// <summary>
		/// Applies the alpha value of each pixel to its color components.
		/// The aplha value stays unchanged.
		/// Only works with 32-bits color depth.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_PreMultiplyWithAlpha")]
		public static extern bool PreMultiplyWithAlpha(FIBITMAP dib);

		#endregion

		#region Miscellaneous algorithms

		/// <summary>
		/// Solves a Poisson equation, remap result pixels to [0..1] and returns the solution.
		/// </summary>
		/// <param name="Laplacian">Handle to a FreeImage bitmap.</param>
		/// <param name="ncycle">Number of cycles in the multigrid algorithm (usually 2 or 3)</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_MultigridPoissonSolver")]
		public static extern FIBITMAP MultigridPoissonSolver(FIBITMAP Laplacian, int ncycle);

		#endregion

		#region Colors

		/// <summary>
		/// Creates a lookup table to be used with FreeImage_AdjustCurve() which
		/// may adjusts brightness and contrast, correct gamma and invert the image with a
		/// single call to FreeImage_AdjustCurve().
		/// </summary>
		/// <param name="LUT">Output lookup table to be used with FreeImage_AdjustCurve().
		/// The size of 'LUT' is assumed to be 256.</param>
		/// <param name="brightness">Percentage brightness value where -100 &lt;= brightness &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will make the image darker and greater
		/// than 0 will make the image brighter.</para></param>
		/// <param name="contrast">Percentage contrast value where -100 &lt;= contrast &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will decrease the contrast
		/// and greater than 0 will increase the contrast of the image.</para></param>
		/// <param name="gamma">Gamma value to be used for gamma correction.
		/// <para>A value of 1.0 leaves the image alone, less than one darkens it,
		/// and greater than one lightens it.</para></param>
		/// <param name="invert">If set to true, the image will be inverted.</param>
		/// <returns>The number of adjustments applied to the resulting lookup table
		/// compared to a blind lookup table.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetAdjustColorsLookupTable")]
		public static extern int GetAdjustColorsLookupTable(byte[] LUT, double brightness, double contrast, double gamma, bool invert);

		/// <summary>
		/// Adjusts an image's brightness, contrast and gamma as well as it may
		/// optionally invert the image within a single operation.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="brightness">Percentage brightness value where -100 &lt;= brightness &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will make the image darker and greater
		/// than 0 will make the image brighter.</para></param>
		/// <param name="contrast">Percentage contrast value where -100 &lt;= contrast &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will decrease the contrast
		/// and greater than 0 will increase the contrast of the image.</para></param>
		/// <param name="gamma">Gamma value to be used for gamma correction.
		/// <para>A value of 1.0 leaves the image alone, less than one darkens it,
		/// and greater than one lightens it.</para>
		/// This parameter must not be zero or smaller than zero.
		/// If so, it will be ignored and no gamma correction will be performed on the image.</param>
		/// <param name="invert">If set to true, the image will be inverted.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AdjustColors")]
		public static extern bool AdjustColors(FIBITMAP dib, double brightness, double contrast, double gamma, bool invert);

		/// <summary>
		/// Applies color mapping for one or several colors on a 1-, 4- or 8-bit
		/// palletized or a 16-, 24- or 32-bit high color image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="srccolors">Array of colors to be used as the mapping source.</param>
		/// <param name="dstcolors">Array of colors to be used as the mapping destination.</param>
		/// <param name="count">The number of colors to be mapped. This is the size of both
		/// srccolors and dstcolors.</param>
		/// <param name="ignore_alpha">If true, 32-bit images and colors are treated as 24-bit.</param>
		/// <param name="swap">If true, source and destination colors are swapped, that is,
		/// each destination color is also mapped to the corresponding source color.</param>
		/// <returns>The total number of pixels changed.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ApplyColorMapping")]
		public static extern uint ApplyColorMapping(FIBITMAP dib, RGBQUAD[] srccolors, RGBQUAD[] dstcolors, uint count, bool ignore_alpha, bool swap);

		/// <summary>
		/// Swaps two specified colors on a 1-, 4- or 8-bit palletized
		/// or a 16-, 24- or 32-bit high color image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="color_a">One of the two colors to be swapped.</param>
		/// <param name="color_b">The other of the two colors to be swapped.</param>
		/// <param name="ignore_alpha">If true, 32-bit images and colors are treated as 24-bit.</param>
		/// <returns>The total number of pixels changed.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SwapColors")]
		public static extern uint SwapColors(FIBITMAP dib, ref RGBQUAD color_a, ref RGBQUAD color_b, bool ignore_alpha);

		/// <summary>
		/// Applies palette index mapping for one or several indices
		/// on a 1-, 4- or 8-bit palletized image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="srcindices">Array of palette indices to be used as the mapping source.</param>
		/// <param name="dstindices">Array of palette indices to be used as the mapping destination.</param>
		/// <param name="count">The number of palette indices to be mapped. This is the size of both
		/// srcindices and dstindices</param>
		/// <param name="swap">If true, source and destination palette indices are swapped, that is,
		/// each destination index is also mapped to the corresponding source index.</param>
		/// <returns>The total number of pixels changed.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ApplyPaletteIndexMapping")]
		public static extern uint ApplyPaletteIndexMapping(FIBITMAP dib, byte[] srcindices, byte[] dstindices, uint count, bool swap);

		/// <summary>
		/// Swaps two specified palette indices on a 1-, 4- or 8-bit palletized image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="index_a">One of the two palette indices to be swapped.</param>
		/// <param name="index_b">The other of the two palette indices to be swapped.</param>
		/// <returns>The total number of pixels changed.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SwapPaletteIndices")]
		public static extern uint SwapPaletteIndices(FIBITMAP dib, ref byte index_a, ref byte index_b);

		#endregion
	}
}

/////////////////////////////////////////////////////
//                                                 //
//               Wrapper functions                 //
//                                                 //
/////////////////////////////////////////////////////

	#region Structs

namespace FreeImageAPI.IO
{
	/// <summary>
	/// Wrapper for a custom handle.
	/// </summary>
	/// <remarks>
	/// The <b>fi_handle</b> of FreeImage in C++ is a simple pointer, but in .NET
	/// it's not that simple. This wrapper uses fi_handle in two different ways.
	///
	/// We implement a new plugin and FreeImage gives us a handle (pointer) that
	/// we can simply pass through to the given functions in a 'FreeImageIO'
	/// structure.
	/// But when we want to use LoadFromhandle or SaveToHandle we need
	/// a fi_handle (that we recieve again in our own functions).
	/// This handle is for example a stream (see LoadFromStream / SaveToStream)
	/// that we want to work with. To know which stream a read/write is meant for
	/// we could use a hash value that the wrapper itself handles or we can
	/// go the unmanaged way of using a handle.
	/// Therefor we use a <see cref="GCHandle"/> to recieve a unique pointer that we can
	/// convert back into a .NET object.
	/// When the <b>fi_handle</b> instance is no longer needed the instance must be disposed
	/// by the creater manually! It is recommended to use the <c>using</c> statement to
	/// be sure the instance is always disposed:
	/// 
	/// <code>
	/// using (fi_handle handle = new fi_handle(object))
	/// {
	///     callSomeFunctions(handle);
	/// }
	/// </code>
	/// 
	/// What does that mean?
	/// If we get a <b>fi_handle</b> from unmanaged code we get a pointer to unmanaged
	/// memory that we do not have to care about, and just pass ist back to FreeImage.
	/// If we have to create a handle our own we use the standard constructur
	/// that fills the <see cref="IntPtr"/> with an pointer that represents the given object.
	/// With calling <see cref="GetObject"/> the <see cref="IntPtr"/> is used to retrieve the original
	/// object we passed through the constructor.
	///
	/// This way we can implement a <b>fi_handle</b> that works with managed an unmanaged
	/// code.
	/// </remarks>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct fi_handle : IComparable, IComparable<fi_handle>, IEquatable<fi_handle>, IDisposable
	{
		/// <summary>
		/// The handle to wrap.
		/// </summary>
		public IntPtr handle;

		/// <summary>
		/// Initializes a new instance wrapping a managed object.
		/// </summary>
		/// <param name="obj">The object to wrap.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="obj"/> is null.</exception>
		public fi_handle(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			GCHandle gch = GCHandle.Alloc(obj, GCHandleType.Normal);
			handle = GCHandle.ToIntPtr(gch);
		}

		/// <summary>
		/// Tests whether two specified <see cref="fi_handle"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="fi_handle"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="fi_handle"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="fi_handle"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(fi_handle left, fi_handle right)
		{
			return (left.handle == right.handle);
		}

		/// <summary>
		/// Tests whether two specified <see cref="fi_handle"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="fi_handle"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="fi_handle"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="fi_handle"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(fi_handle left, fi_handle right)
		{
			return (left.handle != right.handle);
		}

		/// <summary>
		/// Gets whether the pointer is a null pointer.
		/// </summary>
		public bool IsNull
		{
			get
			{
				return (handle == IntPtr.Zero);
			}
		}

		/// <summary>
		/// Returns the object assigned to the handle in case this instance
		/// was created by managed code.
		/// </summary>
		/// <returns><see cref="Object"/> assigned to this handle or null on failure.</returns>
		internal object GetObject()
		{
			object result = null;
			if (handle != IntPtr.Zero)
			{
				try
				{
					result = GCHandle.FromIntPtr(handle).Target;
				}
				catch
				{
				}
			}
			return result;
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="fi_handle"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return handle.ToString();
		}

		/// <summary>
		/// Returns a hash code for this <see cref="fi_handle"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="fi_handle"/>.</returns>
		public override int GetHashCode()
		{
			return handle.GetHashCode();
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="fi_handle"/> structure
		/// and is equivalent to this <see cref="fi_handle"/> structure.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="fi_handle"/> structure
		/// equivalent to this <see cref="fi_handle"/> structure; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is fi_handle) && (this == ((fi_handle)obj)));
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, <b>false</b>.</returns>
		public bool Equals(fi_handle other)
		{
			return (this == other);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="fi_handle"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is fi_handle))
			{
				throw new ArgumentException();
			}
			return CompareTo((fi_handle)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="fi_handle"/> object.
		/// </summary>
		/// <param name="other">A <see cref="fi_handle"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(fi_handle other)
		{
			return handle.ToInt64().CompareTo(other.handle.ToInt64());
		}

		/// <summary>
		/// Releases all resources used by the instance.
		/// </summary>
		public void Dispose()
		{
			if (this.handle != IntPtr.Zero)
			{
				try
				{
					GCHandle.FromIntPtr(handle).Free();
				}
				catch
				{
				}
				finally
				{
					this.handle = IntPtr.Zero;
				}
			}
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FI1BIT</b> structure represents a single bit.
	/// It's value can be <i>0</i> or <i>1</i>.
	/// </summary>
	[DebuggerDisplay("{value}"),
	Serializable]
	public struct FI1BIT
	{
		/// <summary>
		/// Represents the largest possible value of <see cref="FI1BIT"/>. This field is constant.
		/// </summary>
		public const byte MaxValue = 0x01;

		/// <summary>
		/// Represents the smallest possible value of <see cref="FI1BIT"/>. This field is constant.
		/// </summary>
		public const byte MinValue = 0x00;

		/// <summary>
		/// The value of the structure.
		/// </summary>
		private byte value;

		/// <summary>
		/// Initializes a new instance based on the specified value.
		/// </summary>
		/// <param name="value">The value to initialize with.</param>
		private FI1BIT(byte value)
		{
			this.value = (byte)(value & MaxValue);
		}

		/// <summary>
		/// Converts the value of a <see cref="FI1BIT"/> structure to a <see cref="Byte"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FI1BIT"/> structure.</param>
		/// <returns>A new instance of <see cref="FI1BIT"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator byte(FI1BIT value)
		{
			return value.value;
		}

		/// <summary>
		/// Converts the value of a <see cref="Byte"/> structure to a <see cref="FI1BIT"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="Byte"/> structure.</param>
		/// <returns>A new instance of <see cref="FI1BIT"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FI1BIT(byte value)
		{
			return new FI1BIT(value);
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="FI1BIT"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return value.ToString();
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FI4BIT</b> structure represents the half of a <see cref="Byte"/>.
	/// It's valuerange is between <i>0</i> and <i>15</i>.
	/// </summary>
	[DebuggerDisplay("{value}"),
	Serializable]
	public struct FI4BIT
	{
		/// <summary>
		/// Represents the largest possible value of <see cref="FI4BIT"/>. This field is constant.
		/// </summary>
		public const byte MaxValue = 0x0F;

		/// <summary>
		/// Represents the smallest possible value of <see cref="FI4BIT"/>. This field is constant.
		/// </summary>
		public const byte MinValue = 0x00;

		/// <summary>
		/// The value of the structure.
		/// </summary>
		private byte value;

		/// <summary>
		/// Initializes a new instance based on the specified value.
		/// </summary>
		/// <param name="value">The value to initialize with.</param>
		private FI4BIT(byte value)
		{
			this.value = (byte)(value & MaxValue);
		}

		/// <summary>
		/// Converts the value of a <see cref="FI4BIT"/> structure to a <see cref="Byte"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FI4BIT"/> structure.</param>
		/// <returns>A new instance of <see cref="FI4BIT"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator byte(FI4BIT value)
		{
			return value.value;
		}

		/// <summary>
		/// Converts the value of a <see cref="Byte"/> structure to a <see cref="FI4BIT"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="Byte"/> structure.</param>
		/// <returns>A new instance of <see cref="FI4BIT"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FI4BIT(byte value)
		{
			return new FI4BIT(value);
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="FI4BIT"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return value.ToString();
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FI16RGB555</b> structure describes a color consisting of relative
	/// intensities of red, green, blue and alpha value. Each single color
	/// component consumes 5 bits and so, takes values in the range from 0 to 31.
	/// </summary>
	/// <remarks>
	/// <para>For easy integration of the underlying structure into the .NET framework,
	/// the <b>FI16RGB555</b> structure implements implicit conversion operators to 
	/// convert the represented color to and from the <see cref="System.Drawing.Color"/>
	/// type. This makes the <see cref="System.Drawing.Color"/> type a real replacement
	/// for the <b>FI16RGB555</b> structure and my be used in all situations which require
	/// an <b>FI16RGB555</b> type.
	/// </para>
	/// </remarks>
	/// <example>
	/// The following code example demonstrates the various conversions between the
	/// <b>FI16RGB555</b> structure and the <see cref="System.Drawing.Color"/> structure.
	/// <code>
	/// FI16RGB555 fi16rgb;
	/// // Initialize the structure using a native .NET Color structure.
	///	fi16rgb = new FI16RGB555(Color.Indigo);
	/// // Initialize the structure using the implicit operator.
	///	fi16rgb = Color.DarkSeaGreen;
	/// // Convert the FI16RGB555 instance into a native .NET Color
	/// // using its implicit operator.
	///	Color color = fi16rgb;
	/// // Using the structure's Color property for converting it
	/// // into a native .NET Color.
	///	Color another = fi16rgb.Color;
	/// </code>
	/// </example>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FI16RGB555 : IComparable, IComparable<FI16RGB555>, IEquatable<FI16RGB555>
	{
		/// <summary>
		/// The value of the color.
		/// </summary>
		private ushort value;

		/// <summary>
		/// Initializes a new instance based on the specified <see cref="System.Drawing.Color"/>.
		/// </summary>
		/// <param name="color"><see cref="System.Drawing.Color"/> to initialize with.</param>
		public FI16RGB555(Color color)
		{
			value = (ushort)(
				(((color.R * 31) / 255) << FreeImage.FI16_555_RED_SHIFT) +
				(((color.G * 31) / 255) << FreeImage.FI16_555_GREEN_SHIFT) +
				(((color.B * 31) / 255) << FreeImage.FI16_555_BLUE_SHIFT));
		}

		/// <summary>
		/// Tests whether two specified <see cref="FI16RGB555"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="FI16RGB555"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="FI16RGB555"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FI16RGB555"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(FI16RGB555 left, FI16RGB555 right)
		{
			return (left.value == right.value);
		}

		/// <summary>
		/// Tests whether two specified <see cref="FI16RGB555"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="FI16RGB555"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="FI16RGB555"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FI16RGB555"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(FI16RGB555 left, FI16RGB555 right)
		{
			return (!(left == right));
		}

		/// <summary>
		/// Converts the value of a <see cref="System.Drawing.Color"/> structure to a <see cref="FI16RGB555"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="System.Drawing.Color"/> structure.</param>
		/// <returns>A new instance of <see cref="FI16RGB555"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FI16RGB555(Color value)
		{
			return new FI16RGB555(value);
		}

		/// <summary>
		/// Converts the value of a <see cref="FI16RGB555"/> structure to a <see cref="System.Drawing.Color"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FI16RGB555"/> structure.</param>
		/// <returns>A new instance of <see cref="System.Drawing.Color"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator Color(FI16RGB555 value)
		{
			return value.Color;
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Drawing.Color"/> of the structure.
		/// </summary>
		public Color Color
		{
			get
			{
				return Color.FromArgb(
					((value & FreeImage.FI16_555_RED_MASK) >> FreeImage.FI16_555_RED_SHIFT) * 255 / 31,
					((value & FreeImage.FI16_555_GREEN_MASK) >> FreeImage.FI16_555_GREEN_SHIFT) * 255 / 31,
					((value & FreeImage.FI16_555_BLUE_MASK) >> FreeImage.FI16_555_BLUE_SHIFT) * 255 / 31);
			}
			set
			{
				this.value = (ushort)(
					(((value.R * 31) / 255) << FreeImage.FI16_555_RED_SHIFT) +
					(((value.G * 31) / 255) << FreeImage.FI16_555_GREEN_SHIFT) +
					(((value.B * 31) / 255) << FreeImage.FI16_555_BLUE_SHIFT));
			}
		}

		/// <summary>
		/// Gets or sets the red color component.
		/// </summary>
		public byte Red
		{
			get
			{
				return (byte)(((value & FreeImage.FI16_555_RED_MASK) >> FreeImage.FI16_555_RED_SHIFT) * 255 / 31);
			}
			set
			{
				this.value = (ushort)((this.value & (~FreeImage.FI16_555_RED_MASK)) | (((value * 31) / 255) << FreeImage.FI16_555_RED_SHIFT));
			}
		}

		/// <summary>
		/// Gets or sets the green color component.
		/// </summary>
		public byte Green
		{
			get
			{
				return (byte)(((value & FreeImage.FI16_555_GREEN_MASK) >> FreeImage.FI16_555_GREEN_SHIFT) * 255 / 31);
			}
			set
			{
				this.value = (ushort)((this.value & (~FreeImage.FI16_555_GREEN_MASK)) | (((value * 31) / 255) << FreeImage.FI16_555_GREEN_SHIFT));
			}
		}

		/// <summary>
		/// Gets or sets the blue color component.
		/// </summary>
		public byte Blue
		{
			get
			{
				return (byte)(((value & FreeImage.FI16_555_BLUE_MASK) >> FreeImage.FI16_555_BLUE_SHIFT) * 255 / 31);
			}
			set
			{
				this.value = (ushort)((this.value & (~FreeImage.FI16_555_BLUE_MASK)) | (((value * 31) / 255) << FreeImage.FI16_555_BLUE_SHIFT));
			}
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="FI16RGB555"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is FI16RGB555))
			{
				throw new ArgumentException();
			}
			return CompareTo((FI16RGB555)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="FI16RGB555"/> object.
		/// </summary>
		/// <param name="other">A <see cref="FI16RGB555"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(FI16RGB555 other)
		{
			return this.Color.ToArgb().CompareTo(other.Color.ToArgb());
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="FI16RGB555"/> structure
		/// and is equivalent to this <see cref="FI16RGB555"/> structure.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FI16RGB555"/> structure
		/// equivalent to this <see cref="FI16RGB555"/> structure; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// Tests whether the specified <see cref="FI16RGB555"/> structure is equivalent to this <see cref="FI16RGB555"/> structure.
		/// </summary>
		/// <param name="other">A <see cref="FI16RGB555"/> structure to compare to this instance.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FI16RGB555"/> structure
		/// equivalent to this <see cref="FI16RGB555"/> structure; otherwise, <b>false</b>.</returns>
		public bool Equals(FI16RGB555 other)
		{
			return (this == other);
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FI16RGB555"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FI16RGB555"/>.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="FI16RGB555"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return FreeImage.ColorToString(Color);
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FI16RGB565</b> structure describes a color consisting of relative
	/// intensities of red, green, blue and alpha value. Each single color
	/// component consumes 5 bits and so, takes values in the range from 0 to 31.
	/// </summary>
	/// <remarks>
	/// <para>For easy integration of the underlying structure into the .NET framework,
	/// the <b>FI16RGB565</b> structure implements implicit conversion operators to 
	/// convert the represented color to and from the <see cref="System.Drawing.Color"/>
	/// type. This makes the <see cref="System.Drawing.Color"/> type a real replacement
	/// for the <b>FI16RGB565</b> structure and my be used in all situations which require
	/// an <b>FI16RGB565</b> type.
	/// </para>
	/// </remarks>
	/// <example>
	/// The following code example demonstrates the various conversions between the
	/// <b>FI16RGB565</b> structure and the <see cref="System.Drawing.Color"/> structure.
	/// <code>
	/// FI16RGB565 fi16rgb;
	/// // Initialize the structure using a native .NET Color structure.
	///	fi16rgb = new FI16RGB565(Color.Indigo);
	/// // Initialize the structure using the implicit operator.
	///	fi16rgb = Color.DarkSeaGreen;
	/// // Convert the FI16RGB565 instance into a native .NET Color
	/// // using its implicit operator.
	///	Color color = fi16rgb;
	/// // Using the structure's Color property for converting it
	/// // into a native .NET Color.
	///	Color another = fi16rgb.Color;
	/// </code>
	/// </example>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FI16RGB565 : IComparable, IComparable<FI16RGB565>, IEquatable<FI16RGB565>
	{
		/// <summary>
		/// The value of the color.
		/// </summary>
		private ushort value;

		/// <summary>
		/// Initializes a new instance based on the specified <see cref="System.Drawing.Color"/>.
		/// </summary>
		/// <param name="color"><see cref="System.Drawing.Color"/> to initialize with.</param>
		public FI16RGB565(Color color)
		{
			value = (ushort)(
				(((color.R * 31) / 255) << FreeImage.FI16_565_RED_SHIFT) +
				(((color.G * 63) / 255) << FreeImage.FI16_565_GREEN_SHIFT) +
				(((color.B * 31) / 255) << FreeImage.FI16_565_BLUE_SHIFT));
		}

		/// <summary>
		/// Tests whether two specified <see cref="FI16RGB565"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="FI16RGB565"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="FI16RGB565"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FI16RGB565"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(FI16RGB565 left, FI16RGB565 right)
		{
			return (left.value == right.value);
		}

		/// <summary>
		/// Tests whether two specified <see cref="FI16RGB565"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="FI16RGB565"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="FI16RGB565"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="FI16RGB565"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(FI16RGB565 left, FI16RGB565 right)
		{
			return (!(left == right));
		}

		/// <summary>
		/// Converts the value of a <see cref="System.Drawing.Color"/> structure to a <see cref="FI16RGB565"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="System.Drawing.Color"/> structure.</param>
		/// <returns>A new instance of <see cref="FI16RGB565"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FI16RGB565(Color value)
		{
			return new FI16RGB565(value);
		}

		/// <summary>
		/// Converts the value of a <see cref="FI16RGB565"/> structure to a <see cref="System.Drawing.Color"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FI16RGB565"/> structure.</param>
		/// <returns>A new instance of <see cref="System.Drawing.Color"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator Color(FI16RGB565 value)
		{
			return value.Color;
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Drawing.Color"/> of the structure.
		/// </summary>
		public Color Color
		{
			get
			{
				return Color.FromArgb(
					((value & FreeImage.FI16_565_RED_MASK) >> FreeImage.FI16_565_RED_SHIFT) * 255 / 31,
					((value & FreeImage.FI16_565_GREEN_MASK) >> FreeImage.FI16_565_GREEN_SHIFT) * 255 / 63,
					((value & FreeImage.FI16_565_BLUE_MASK) >> FreeImage.FI16_565_BLUE_SHIFT) * 255 / 31);
			}
			set
			{
				this.value = (ushort)(
					(((value.R * 31) / 255) << FreeImage.FI16_565_RED_SHIFT) +
					(((value.G * 63) / 255) << FreeImage.FI16_565_GREEN_SHIFT) +
					(((value.B * 31) / 255) << FreeImage.FI16_565_BLUE_SHIFT));
			}
		}

		/// <summary>
		/// Gets or sets the red color component.
		/// </summary>
		public byte Red
		{
			get
			{
				return (byte)(((value & FreeImage.FI16_565_RED_MASK) >> FreeImage.FI16_565_RED_SHIFT) * 255 / 31);
			}
			set
			{
				this.value = (ushort)((this.value & (~FreeImage.FI16_565_RED_MASK)) | (((value * 31) / 255) << FreeImage.FI16_565_RED_SHIFT));
			}
		}

		/// <summary>
		/// Gets or sets the green color component.
		/// </summary>
		public byte Green
		{
			get
			{
				return (byte)(((value & FreeImage.FI16_565_GREEN_MASK) >> FreeImage.FI16_565_GREEN_SHIFT) * 255 / 63);
			}
			set
			{
				this.value = (ushort)((this.value & (~FreeImage.FI16_565_GREEN_MASK)) | (((value * 63) / 255) << FreeImage.FI16_565_GREEN_SHIFT));
			}
		}

		/// <summary>
		/// Gets or sets the blue color component.
		/// </summary>
		public byte Blue
		{
			get
			{
				return (byte)(((value & FreeImage.FI16_565_BLUE_MASK) >> FreeImage.FI16_565_BLUE_SHIFT) * 255 / 31);
			}
			set
			{
				this.value = (ushort)((this.value & (~FreeImage.FI16_565_BLUE_MASK)) | (((value * 31) / 255) << FreeImage.FI16_565_BLUE_SHIFT));
			}
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="FI16RGB565"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is FI16RGB565))
			{
				throw new ArgumentException();
			}
			return CompareTo((FI16RGB565)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="FI16RGB565"/> object.
		/// </summary>
		/// <param name="other">A <see cref="FI16RGB565"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(FI16RGB565 other)
		{
			return this.Color.ToArgb().CompareTo(other.Color.ToArgb());
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="FI16RGB565"/> structure
		/// and is equivalent to this <see cref="FI16RGB565"/> structure.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FI16RGB565"/> structure
		/// equivalent to this <see cref="FI16RGB565"/> structure; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// Tests whether the specified <see cref="FI16RGB565"/> structure is equivalent to this <see cref="FI16RGB565"/> structure.
		/// </summary>
		/// <param name="other">A <see cref="FI16RGB565"/> structure to compare to this instance.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FI16RGB565"/> structure
		/// equivalent to this <see cref="FI16RGB565"/> structure; otherwise, <b>false</b>.</returns>
		public bool Equals(FI16RGB565 other)
		{
			return (this == other);
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FI16RGB565"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FI16RGB565"/>.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="FI16RGB565"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return FreeImage.ColorToString(Color);
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FIRational</b> structure represents a fraction via two <see cref="Int32"/>
	/// instances which are interpreted as numerator and denominator.
	/// </summary>
	/// <remarks>
	/// The structure tries to approximate the value of <see cref="FreeImageAPI.FIRational(decimal)"/>
	/// when creating a new instance by using a better algorithm than FreeImage does.
	/// <para/>
	/// The structure implements the following operators:
	/// +, -, ++, --, ==, != , >, >==, &lt;, &lt;== and ~ (which switches nominator and denomiator).
	/// <para/>
	/// The structure can be converted into all .NET standard types either implicit or
	/// explicit.
	/// </remarks>
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
	public struct FIRational : IConvertible, IComparable, IFormattable, IComparable<FIRational>, IEquatable<FIRational>
	{
		private int numerator;
		private int denominator;

		/// <summary>
		/// Represents the largest possible value of <see cref="FIRational"/>. This field is constant.
		/// </summary>
		public static readonly FIRational MaxValue = new FIRational(Int32.MaxValue, 1);

		/// <summary>
		/// Represents the smallest possible value of <see cref="FIRational"/>. This field is constant.
		/// </summary>
		public static readonly FIRational MinValue = new FIRational(Int32.MinValue, 1);

		/// <summary>
		/// Represents the smallest positive <see cref="FIRational"/> value greater than zero. This field is constant.
		/// </summary>
		public static readonly FIRational Epsilon = new FIRational(1, Int32.MaxValue);

		/// <summary>
		/// Initializes a new instance based on the specified parameters.
		/// </summary>
		/// <param name="n">The numerator.</param>
		/// <param name="d">The denominator.</param>
		public FIRational(int n, int d)
		{
			numerator = n;
			denominator = d;
			Normalize();
		}

		/// <summary>
		/// Initializes a new instance based on the specified parameters.
		/// </summary>
		/// <param name="tag">The tag to read the data from.</param>
		public unsafe FIRational(FITAG tag)
		{
			switch (FreeImage.GetTagType(tag))
			{
				case FREE_IMAGE_MDTYPE.FIDT_SRATIONAL:
					int* value = (int*)FreeImage.GetTagValue(tag);
					numerator = (int)value[0];
					denominator = (int)value[1];
					Normalize();
					return;
				default:
					throw new ArgumentException("tag");
			}
		}

		/// <summary>
		/// Initializes a new instance based on the specified parameters.
		/// </summary>
		/// <param name="value">The value to convert into a fraction.</param>
		/// <exception cref="OverflowException">
		/// <paramref name="value"/> cannot be converted into a fraction
		/// represented by two integer values.</exception>
		public FIRational(decimal value)
		{
			try
			{
				int sign = value < 0 ? -1 : 1;
				value = Math.Abs(value);
				try
				{
					int[] contFract = CreateContinuedFraction(value);
					CreateFraction(contFract, out numerator, out denominator);
					Normalize();
				}
				catch
				{
					numerator = 0;
					denominator = 1;
				}
				if (Math.Abs(((decimal)numerator / (decimal)denominator) - value) > 0.0001m)
				{
					int maxDen = (Int32.MaxValue / (int)value) - 2;
					maxDen = maxDen < 10000 ? maxDen : 10000;
					ApproximateFraction(value, maxDen, out numerator, out denominator);
					Normalize();
					if (Math.Abs(((decimal)numerator / (decimal)denominator) - value) > 0.0001m)
					{
						throw new OverflowException();
					}
				}
				numerator *= sign;
				Normalize();
			}
			catch (Exception ex)
			{
				throw new OverflowException("Unable to calculate fraction.", ex);
			}
		}

		/// <summary>
		/// Initializes a new instance based on the specified parameters.
		/// </summary>
		/// <param name="r">The structure to clone from.</param>
		public FIRational(FIRational r)
		{
			numerator = r.numerator;
			denominator = r.denominator;
			Normalize();
		}

		/// <summary>
		/// The numerator of the fraction.
		/// </summary>
		public int Numerator
		{
			get { return numerator; }
		}

		/// <summary>
		/// The denominator of the fraction.
		/// </summary>
		public int Denominator
		{
			get { return denominator; }
		}

		/// <summary>
		/// Returns the truncated value of the fraction.
		/// </summary>
		/// <returns></returns>
		public int Truncate()
		{
			return denominator > 0 ? (int)(numerator / denominator) : 0;
		}

		/// <summary>
		/// Returns whether the fraction is representing an integer value.
		/// </summary>
		public bool IsInteger
		{
			get
			{
				return (denominator == 1 ||
					(denominator != 0 && (numerator % denominator == 0)) ||
					(denominator == 0 && numerator == 0));
			}
		}

		/// <summary>
		/// Calculated the greatest common divisor of 'a' and 'b'.
		/// </summary>
		private static long Gcd(long a, long b)
		{
			a = Math.Abs(a);
			b = Math.Abs(b);
			long r;
			while (b > 0)
			{
				r = a % b;
				a = b;
				b = r;
			}
			return a;
		}

		/// <summary>
		/// Calculated the smallest common multiple of 'a' and 'b'.
		/// </summary>
		private static long Scm(int n, int m)
		{
			return Math.Abs((long)n * (long)m) / Gcd(n, m);
		}

		/// <summary>
		/// Normalizes the fraction.
		/// </summary>
		private void Normalize()
		{
			if (denominator == 0)
			{
				numerator = 0;
				denominator = 1;
				return;
			}

			if (numerator != 1 && denominator != 1)
			{
				int common = (int)Gcd(numerator, denominator);
				if (common != 1 && common != 0)
				{
					numerator /= common;
					denominator /= common;
				}
			}

			if (denominator < 0)
			{
				numerator *= -1;
				denominator *= -1;
			}
		}

		/// <summary>
		/// Normalizes a fraction.
		/// </summary>
		private static void Normalize(ref long numerator, ref long denominator)
		{
			if (denominator == 0)
			{
				numerator = 0;
				denominator = 1;
			}
			else if (numerator != 1 && denominator != 1)
			{
				long common = Gcd(numerator, denominator);
				if (common != 1)
				{
					numerator /= common;
					denominator /= common;
				}
			}
			if (denominator < 0)
			{
				numerator *= -1;
				denominator *= -1;
			}
		}

		/// <summary>
		/// Returns the digits after the point.
		/// </summary>
		private static int GetDigits(decimal value)
		{
			int result = 0;
			value -= decimal.Truncate(value);
			while (value != 0)
			{
				value *= 10;
				value -= decimal.Truncate(value);
				result++;
			}
			return result;
		}

		/// <summary>
		/// Creates a continued fraction of a decimal value.
		/// </summary>
		private static int[] CreateContinuedFraction(decimal value)
		{
			int precision = GetDigits(value);
			decimal epsilon = 0.0000001m;
			List<int> list = new List<int>();
			value = Math.Abs(value);

			byte b = 0;

			list.Add((int)value);
			value -= ((int)value);

			while (value != 0m)
			{
				if (++b == byte.MaxValue || value < epsilon)
				{
					break;
				}
				value = 1m / value;
				if (Math.Abs((Math.Round(value, precision - 1) - value)) < epsilon)
				{
					value = Math.Round(value, precision - 1);
				}
				list.Add((int)value);
				value -= ((int)value);
			}
			return list.ToArray();
		}

		/// <summary>
		/// Creates a fraction from a continued fraction.
		/// </summary>
		private static void CreateFraction(int[] continuedFraction, out int numerator, out int denominator)
		{
			numerator = 1;
			denominator = 0;
			int temp;

			for (int i = continuedFraction.Length - 1; i > -1; i--)
			{
				temp = numerator;
				numerator = continuedFraction[i] * numerator + denominator;
				denominator = temp;
			}
		}

		/// <summary>
		/// Tries 'brute force' to approximate <paramref name="value"/> with a fraction.
		/// </summary>
		private static void ApproximateFraction(decimal value, int maxDen, out int num, out int den)
		{
			num = 0;
			den = 0;
			decimal bestDifference = 1m;
			decimal currentDifference = -1m;
			int digits = GetDigits(value);

			if (digits <= 9)
			{
				int mul = 1;
				for (int i = 1; i <= digits; i++)
				{
					mul *= 10;
				}
				if (mul <= maxDen)
				{
					num = (int)(value * mul);
					den = mul;
					return;
				}
			}

			for (int i = 1; i <= maxDen; i++)
			{
				int numerator = (int)Math.Floor(value * (decimal)i + 0.5m);
				currentDifference = Math.Abs(value - (decimal)numerator / (decimal)i);
				if (currentDifference < bestDifference)
				{
					num = numerator;
					den = i;
					bestDifference = currentDifference;
				}
			}
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="FIRational"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return ((IConvertible)this).ToDouble(null).ToString();
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="FIRational"/> structure
		/// and is equivalent to this <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FIRational"/> structure
		/// equivalent to this <see cref="FIRational"/> structure; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is FIRational) && (this == ((FIRational)obj)));
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FIRational"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FIRational"/>.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#region Operators

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIRational operator +(FIRational r1)
		{
			return r1;
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIRational operator -(FIRational r1)
		{
			r1.numerator *= -1;
			return r1;
		}

		/// <summary>
		/// Returns the reciprocal value of this instance.
		/// </summary>
		public static FIRational operator ~(FIRational r1)
		{
			int temp = r1.denominator;
			r1.denominator = r1.numerator;
			r1.numerator = temp;
			r1.Normalize();
			return r1;
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIRational operator ++(FIRational r1)
		{
			checked
			{
				r1.numerator += r1.denominator;
			}
			return r1;
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIRational operator --(FIRational r1)
		{
			checked
			{
				r1.numerator -= r1.denominator;
			}
			return r1;
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIRational operator +(FIRational r1, FIRational r2)
		{
			long numerator = 0;
			long denominator = Scm(r1.denominator, r2.denominator);
			numerator = (r1.numerator * (denominator / r1.denominator)) + (r2.numerator * (denominator / r2.denominator));
			Normalize(ref numerator, ref denominator);
			checked
			{
				return new FIRational((int)numerator, (int)denominator);
			}
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIRational operator -(FIRational r1, FIRational r2)
		{
			return r1 + (-r2);
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIRational operator *(FIRational r1, FIRational r2)
		{
			long numerator = r1.numerator * r2.numerator;
			long denominator = r1.denominator * r2.denominator;
			Normalize(ref numerator, ref denominator);
			checked
			{
				return new FIRational((int)numerator, (int)denominator);
			}
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIRational operator /(FIRational r1, FIRational r2)
		{
			int temp = r2.denominator;
			r2.denominator = r2.numerator;
			r2.numerator = temp;
			return r1 * r2;
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIRational operator %(FIRational r1, FIRational r2)
		{
			r2.Normalize();
			if (Math.Abs(r2.numerator) < r2.denominator)
				return new FIRational(0, 0);
			int div = (int)(r1 / r2);
			return r1 - (r2 * div);
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static bool operator ==(FIRational r1, FIRational r2)
		{
			r1.Normalize();
			r2.Normalize();
			return (r1.numerator == r2.numerator) && (r1.denominator == r2.denominator);
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static bool operator !=(FIRational r1, FIRational r2)
		{
			return !(r1 == r2);
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static bool operator >(FIRational r1, FIRational r2)
		{
			long denominator = Scm(r1.denominator, r2.denominator);
			return (r1.numerator * (denominator / r1.denominator)) > (r2.numerator * (denominator / r2.denominator));
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static bool operator <(FIRational r1, FIRational r2)
		{
			long denominator = Scm(r1.denominator, r2.denominator);
			return (r1.numerator * (denominator / r1.denominator)) < (r2.numerator * (denominator / r2.denominator));
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static bool operator >=(FIRational r1, FIRational r2)
		{
			long denominator = Scm(r1.denominator, r2.denominator);
			return (r1.numerator * (denominator / r1.denominator)) >= (r2.numerator * (denominator / r2.denominator));
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static bool operator <=(FIRational r1, FIRational r2)
		{
			long denominator = Scm(r1.denominator, r2.denominator);
			return (r1.numerator * (denominator / r1.denominator)) <= (r2.numerator * (denominator / r2.denominator));
		}

		#endregion

		#region Conversions

		/// <summary>
		/// Converts the value of a <see cref="FIRational"/> structure to a <see cref="Boolean"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRational"/> structure.</param>
		/// <returns>A new instance of <see cref="Boolean"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator bool(FIRational value)
		{
			return (value.numerator != 0);
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRational"/> structure to a <see cref="Byte"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRational"/> structure.</param>
		/// <returns>A new instance of <see cref="Byte"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator byte(FIRational value)
		{
			return (byte)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRational"/> structure to a <see cref="Char"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRational"/> structure.</param>
		/// <returns>A new instance of <see cref="Char"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator char(FIRational value)
		{
			return (char)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRational"/> structure to a <see cref="Decimal"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRational"/> structure.</param>
		/// <returns>A new instance of <see cref="Decimal"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator decimal(FIRational value)
		{
			return value.denominator == 0 ? 0m : (decimal)value.numerator / (decimal)value.denominator;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRational"/> structure to a <see cref="Double"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRational"/> structure.</param>
		/// <returns>A new instance of <see cref="Double"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator double(FIRational value)
		{
			return value.denominator == 0 ? 0d : (double)value.numerator / (double)value.denominator;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRational"/> structure to an <see cref="Int16"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRational"/> structure.</param>
		/// <returns>A new instance of <see cref="Int16"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator short(FIRational value)
		{
			return (short)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRational"/> structure to an <see cref="Int32"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRational"/> structure.</param>
		/// <returns>A new instance of <see cref="Int32"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator int(FIRational value)
		{
			return (int)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRational"/> structure to an <see cref="Int64"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRational"/> structure.</param>
		/// <returns>A new instance of <see cref="Int64"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator long(FIRational value)
		{
			return (byte)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRational"/> structure to a <see cref="Single"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRational"/> structure.</param>
		/// <returns>A new instance of <see cref="Single"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator float(FIRational value)
		{
			return value.denominator == 0 ? 0f : (float)value.numerator / (float)value.denominator;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRational"/> structure to a <see cref="SByte"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRational"/> structure.</param>
		/// <returns>A new instance of <see cref="SByte"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator sbyte(FIRational value)
		{
			return (sbyte)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRational"/> structure to an <see cref="UInt16"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRational"/> structure.</param>
		/// <returns>A new instance of <see cref="UInt16"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator ushort(FIRational value)
		{
			return (ushort)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRational"/> structure to an <see cref="UInt32"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRational"/> structure.</param>
		/// <returns>A new instance of <see cref="UInt32"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator uint(FIRational value)
		{
			return (uint)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIRational"/> structure to an <see cref="UInt64"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIRational"/> structure.</param>
		/// <returns>A new instance of <see cref="UInt64"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator ulong(FIRational value)
		{
			return (ulong)(double)value;
		}

		//

		/// <summary>
		/// Converts the value of a <see cref="Boolean"/> structure to a <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="Boolean"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRational"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator FIRational(bool value)
		{
			return new FIRational(value ? 1 : 0, 1);
		}

		/// <summary>
		/// Converts the value of a <see cref="Byte"/> structure to a <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="Byte"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRational"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIRational(byte value)
		{
			return new FIRational(value, 1);
		}

		/// <summary>
		/// Converts the value of a <see cref="Char"/> structure to a <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="Char"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRational"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIRational(char value)
		{
			return new FIRational(value, 1);
		}

		/// <summary>
		/// Converts the value of a <see cref="Decimal"/> structure to a <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="Decimal"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRational"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator FIRational(decimal value)
		{
			return new FIRational(value);
		}

		/// <summary>
		/// Converts the value of a <see cref="Double"/> structure to a <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="Double"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRational"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator FIRational(double value)
		{
			return new FIRational((decimal)value);
		}

		/// <summary>
		/// Converts the value of an <see cref="Int16"/> structure to a <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="value">An <see cref="Int16"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRational"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIRational(short value)
		{
			return new FIRational(value, 1);
		}

		/// <summary>
		/// Converts the value of an <see cref="Int32"/> structure to a <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="value">An <see cref="Int32"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRational"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIRational(int value)
		{
			return new FIRational(value, 1);
		}

		/// <summary>
		/// Converts the value of an <see cref="Int64"/> structure to a <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="value">An <see cref="Int64"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRational"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator FIRational(long value)
		{
			return new FIRational((int)value, 1);
		}

		/// <summary>
		/// Converts the value of a <see cref="SByte"/> structure to a <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="SByte"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRational"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIRational(sbyte value)
		{
			return new FIRational(value, 1);
		}

		/// <summary>
		/// Converts the value of a <see cref="Single"/> structure to a <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="Single"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRational"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator FIRational(float value)
		{
			return new FIRational((decimal)value);
		}

		/// <summary>
		/// Converts the value of an <see cref="UInt16"/> structure to a <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="value">An <see cref="UInt16"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRational"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIRational(ushort value)
		{
			return new FIRational(value, 1);
		}

		/// <summary>
		/// Converts the value of an <see cref="UInt32"/> structure to a <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="value">An <see cref="UInt32"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRational"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator FIRational(uint value)
		{
			return new FIRational((int)value, 1);
		}

		/// <summary>
		/// Converts the value of an <see cref="UInt64"/> structure to a <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="value">An <see cref="UInt64"/> structure.</param>
		/// <returns>A new instance of <see cref="FIRational"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator FIRational(ulong value)
		{
			return new FIRational((int)value, 1);
		}

		#endregion

		#region IConvertible Member

		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Double;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return (bool)this;
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return (byte)this;
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return (char)this;
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(((IConvertible)this).ToDouble(provider));
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return this;
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return this;
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return (short)this;
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return (int)this;
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return (long)this;
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return (sbyte)this;
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return this;
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return ToString(((double)this).ToString(), provider);
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return Convert.ChangeType(((IConvertible)this).ToDouble(provider), conversionType, provider);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return (ushort)this;
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return (uint)this;
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return (ulong)this;
		}

		#endregion

		#region IComparable Member

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="FIRational"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is FIRational))
			{
				throw new ArgumentException();
			}
			return CompareTo((FIRational)obj);
		}

		#endregion

		#region IFormattable Member

		/// <summary>
		/// Formats the value of the current instance using the specified format.
		/// </summary>
		/// <param name="format">The String specifying the format to use.</param>
		/// <param name="formatProvider">The IFormatProvider to use to format the value.</param>
		/// <returns>A String containing the value of the current instance in the specified format.</returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null)
			{
				format = "";
			}
			return String.Format(formatProvider, format, ((IConvertible)this).ToDouble(formatProvider));
		}

		#endregion

		#region IEquatable<FIRational> Member

		/// <summary>
		/// Tests whether the specified <see cref="FIRational"/> structure is equivalent to this <see cref="FIRational"/> structure.
		/// </summary>
		/// <param name="other">A <see cref="FIRational"/> structure to compare to this instance.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FIRational"/> structure
		/// equivalent to this <see cref="FIRational"/> structure; otherwise, <b>false</b>.</returns>
		public bool Equals(FIRational other)
		{
			return (this == other);
		}

		#endregion

		#region IComparable<FIRational> Member

		/// <summary>
		/// Compares this instance with a specified <see cref="FIRational"/> object.
		/// </summary>
		/// <param name="other">A <see cref="FIRational"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(FIRational other)
		{
			FIRational difference = this - other;
			difference.Normalize();
			if (difference.numerator > 0) return 1;
			if (difference.numerator < 0) return -1;
			else return 0;
		}

		#endregion
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// The <b>FIURational</b> structure represents a fraction via two <see cref="UInt32"/>
	/// instances which are interpreted as numerator and denominator.
	/// </summary>
	/// <remarks>
	/// The structure tries to approximate the value of <see cref="FreeImageAPI.FIURational(decimal)"/>
	/// when creating a new instance by using a better algorithm than FreeImage does.
	/// <para/>
	/// The structure implements the following operators:
	/// +, ++, --, ==, != , >, >==, &lt;, &lt;== and ~ (which switches nominator and denomiator).
	/// <para/>
	/// The structure can be converted into all .NET standard types either implicit or
	/// explicit.
	/// </remarks>
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
	public struct FIURational : IConvertible, IComparable, IFormattable, IComparable<FIURational>, IEquatable<FIURational>
	{
		private uint numerator;
		private uint denominator;

		/// <summary>
		/// Represents the largest possible value of <see cref="FIURational"/>. This field is constant.
		/// </summary>
		public static readonly FIURational MaxValue = new FIURational(UInt32.MaxValue, 1u);

		/// <summary>
		/// Represents the smallest possible value of <see cref="FIURational"/>. This field is constant.
		/// </summary>
		public static readonly FIURational MinValue = new FIURational(0u, 1u);

		/// <summary>
		/// Represents the smallest positive <see cref="FIURational"/> value greater than zero. This field is constant.
		/// </summary>
		public static readonly FIURational Epsilon = new FIURational(1, Int32.MaxValue);

		/// <summary>
		/// Initializes a new instance based on the specified parameters.
		/// </summary>
		/// <param name="n">The numerator.</param>
		/// <param name="d">The denominator.</param>
		public FIURational(uint n, uint d)
		{
			numerator = n;
			denominator = d;
			Normalize();
		}

		/// <summary>
		/// Initializes a new instance based on the specified parameters.
		/// </summary>
		/// <param name="tag">The tag to read the data from.</param>
		public unsafe FIURational(FITAG tag)
		{
			switch (FreeImage.GetTagType(tag))
			{
				case FREE_IMAGE_MDTYPE.FIDT_RATIONAL:
					uint* pvalue = (uint*)FreeImage.GetTagValue(tag);
					numerator = pvalue[0];
					denominator = pvalue[1];
					Normalize();
					return;
				default:
					throw new ArgumentException("tag");
			}
		}

		/// <summary>
		///Initializes a new instance based on the specified parameters.
		/// </summary>
		/// <param name="value">The value to convert into a fraction.</param>
		/// <exception cref="OverflowException">
		/// <paramref name="value"/> cannot be converted into a fraction
		/// represented by two integer values.</exception>
		public FIURational(decimal value)
		{
			try
			{
				if (value < 0) throw new ArgumentOutOfRangeException("value");
				try
				{
					int[] contFract = CreateContinuedFraction(value);
					CreateFraction(contFract, out numerator, out denominator);
					Normalize();
				}
				catch
				{
					numerator = 0;
					denominator = 1;
				}
				if (Math.Abs(((decimal)numerator / (decimal)denominator) - value) > 0.0001m)
				{
					int maxDen = (Int32.MaxValue / (int)value) - 2;
					maxDen = maxDen < 10000 ? maxDen : 10000;
					ApproximateFraction(value, maxDen, out numerator, out denominator);
					Normalize();
					if (Math.Abs(((decimal)numerator / (decimal)denominator) - value) > 0.0001m)
					{
						throw new OverflowException();
					}
				}
				Normalize();
			}
			catch (Exception ex)
			{
				throw new OverflowException("Unable to calculate fraction.", ex);
			}
		}

		/// <summary>
		/// Initializes a new instance based on the specified parameters.
		/// </summary>
		/// <param name="r">The structure to clone from.</param>
		public FIURational(FIURational r)
		{
			numerator = r.numerator;
			denominator = r.denominator;
			Normalize();
		}

		/// <summary>
		/// The numerator of the fraction.
		/// </summary>
		public uint Numerator
		{
			get { return numerator; }
		}

		/// <summary>
		/// The denominator of the fraction.
		/// </summary>
		public uint Denominator
		{
			get { return denominator; }
		}

		/// <summary>
		/// Returns the truncated value of the fraction.
		/// </summary>
		/// <returns></returns>
		public int Truncate()
		{
			return denominator > 0 ? (int)(numerator / denominator) : 0;
		}

		/// <summary>
		/// Returns whether the fraction is representing an integer value.
		/// </summary>
		public bool IsInteger
		{
			get
			{
				return (denominator == 1 ||
					(denominator != 0 && (numerator % denominator == 0)) ||
					(denominator == 0 && numerator == 0));
			}
		}

		/// <summary>
		/// Calculated the greatest common divisor of 'a' and 'b'.
		/// </summary>
		private static ulong Gcd(ulong a, ulong b)
		{
			ulong r;
			while (b > 0)
			{
				r = a % b;
				a = b;
				b = r;
			}
			return a;
		}

		/// <summary>
		/// Calculated the smallest common multiple of 'a' and 'b'.
		/// </summary>
		private static ulong Scm(uint n, uint m)
		{
			return (ulong)n * (ulong)m / Gcd(n, m);
		}

		/// <summary>
		/// Normalizes the fraction.
		/// </summary>
		private void Normalize()
		{
			if (denominator == 0)
			{
				numerator = 0;
				denominator = 1;
				return;
			}

			if (numerator != 1 && denominator != 1)
			{
				uint common = (uint)Gcd(numerator, denominator);
				if (common != 1 && common != 0)
				{
					numerator /= common;
					denominator /= common;
				}
			}
		}

		/// <summary>
		/// Normalizes a fraction.
		/// </summary>
		private static void Normalize(ref ulong numerator, ref ulong denominator)
		{
			if (denominator == 0)
			{
				numerator = 0;
				denominator = 1;
			}
			else if (numerator != 1 && denominator != 1)
			{
				ulong common = Gcd(numerator, denominator);
				if (common != 1)
				{
					numerator /= common;
					denominator /= common;
				}
			}
		}

		/// <summary>
		/// Returns the digits after the point.
		/// </summary>
		private static int GetDigits(decimal value)
		{
			int result = 0;
			value -= decimal.Truncate(value);
			while (value != 0)
			{
				value *= 10;
				value -= decimal.Truncate(value);
				result++;
			}
			return result;
		}

		/// <summary>
		/// Creates a continued fraction of a decimal value.
		/// </summary>
		private static int[] CreateContinuedFraction(decimal value)
		{
			int precision = GetDigits(value);
			decimal epsilon = 0.0000001m;
			List<int> list = new List<int>();
			value = Math.Abs(value);

			byte b = 0;

			list.Add((int)value);
			value -= ((int)value);

			while (value != 0m)
			{
				if (++b == byte.MaxValue || value < epsilon)
				{
					break;
				}
				value = 1m / value;
				if (Math.Abs((Math.Round(value, precision - 1) - value)) < epsilon)
				{
					value = Math.Round(value, precision - 1);
				}
				list.Add((int)value);
				value -= ((int)value);
			}
			return list.ToArray();
		}

		/// <summary>
		/// Creates a fraction from a continued fraction.
		/// </summary>
		private static void CreateFraction(int[] continuedFraction, out uint numerator, out uint denominator)
		{
			numerator = 1;
			denominator = 0;
			uint temp;

			for (int i = continuedFraction.Length - 1; i > -1; i--)
			{
				temp = numerator;
				numerator = (uint)(continuedFraction[i] * numerator + denominator);
				denominator = temp;
			}
		}

		/// <summary>
		/// Tries 'brute force' to approximate <paramref name="value"/> with a fraction.
		/// </summary>
		private static void ApproximateFraction(decimal value, int maxDen, out uint num, out uint den)
		{
			num = 0;
			den = 0;
			decimal bestDifference = 1m;
			decimal currentDifference = -1m;
			int digits = GetDigits(value);

			if (digits <= 9)
			{
				uint mul = 1;
				for (int i = 1; i <= digits; i++)
				{
					mul *= 10;
				}
				if (mul <= maxDen)
				{
					num = (uint)(value * mul);
					den = mul;
					return;
				}
			}

			for (uint u = 1; u <= maxDen; u++)
			{
				uint numerator = (uint)Math.Floor(value * (decimal)u + 0.5m);
				currentDifference = Math.Abs(value - (decimal)numerator / (decimal)u);
				if (currentDifference < bestDifference)
				{
					num = numerator;
					den = u;
					bestDifference = currentDifference;
				}
			}
		}

		/// <summary>
		/// Converts the numeric value of the <see cref="FIURational"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return ((IConvertible)this).ToDouble(null).ToString();
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="FIURational"/> structure
		/// and is equivalent to this <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FIURational"/> structure
		/// equivalent to this <see cref="FIURational"/> structure; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is FIURational) && (this == ((FIURational)obj)));
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FIURational"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FIURational"/>.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#region Operators

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIURational operator +(FIURational value)
		{
			return value;
		}

		/// <summary>
		/// Returns the reciprocal value of this instance.
		/// </summary>
		public static FIURational operator ~(FIURational value)
		{
			uint temp = value.denominator;
			value.denominator = value.numerator;
			value.numerator = temp;
			value.Normalize();
			return value;
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIURational operator ++(FIURational value)
		{
			checked
			{
				value.numerator += value.denominator;
			}
			return value;
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIURational operator --(FIURational value)
		{
			checked
			{
				value.numerator -= value.denominator;
			}
			return value;
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIURational operator +(FIURational left, FIURational right)
		{
			ulong numerator = 0;
			ulong denominator = Scm(left.denominator, right.denominator);
			numerator = (left.numerator * (denominator / left.denominator)) +
						(right.numerator * (denominator / right.denominator));
			Normalize(ref numerator, ref denominator);
			checked
			{
				return new FIURational((uint)numerator, (uint)denominator);
			}
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIURational operator -(FIURational left, FIURational right)
		{
			checked
			{
				if (left.denominator != right.denominator)
				{
					uint denom = left.denominator;
					left.numerator *= right.denominator;
					left.denominator *= right.denominator;
					right.numerator *= denom;
					right.denominator *= denom;
				}
				left.numerator -= right.numerator;
				left.Normalize();
				return left;
			}
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIURational operator *(FIURational left, FIURational r2)
		{
			ulong numerator = left.numerator * r2.numerator;
			ulong denominator = left.denominator * r2.denominator;
			Normalize(ref numerator, ref denominator);
			checked
			{
				return new FIURational((uint)numerator, (uint)denominator);
			}
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIURational operator /(FIURational left, FIURational right)
		{
			uint temp = right.denominator;
			right.denominator = right.numerator;
			right.numerator = temp;
			return left * right;
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static FIURational operator %(FIURational left, FIURational right)
		{
			right.Normalize();
			if (Math.Abs(right.numerator) < right.denominator)
				return new FIURational(0, 0);
			int div = (int)(left / right);
			return left - (right * div);
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static bool operator ==(FIURational left, FIURational right)
		{
			left.Normalize();
			right.Normalize();
			return (left.numerator == right.numerator) && (left.denominator == right.denominator);
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static bool operator !=(FIURational left, FIURational right)
		{
			left.Normalize();
			right.Normalize();
			return (left.numerator != right.numerator) || (left.denominator != right.denominator);
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static bool operator >(FIURational left, FIURational right)
		{
			ulong denominator = Scm(left.denominator, right.denominator);
			return (left.numerator * (denominator / left.denominator)) >
				(right.numerator * (denominator / right.denominator));
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static bool operator <(FIURational left, FIURational right)
		{
			ulong denominator = Scm(left.denominator, right.denominator);
			return (left.numerator * (denominator / left.denominator)) <
				(right.numerator * (denominator / right.denominator));
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static bool operator >=(FIURational left, FIURational right)
		{
			ulong denominator = Scm(left.denominator, right.denominator);
			return (left.numerator * (denominator / left.denominator)) >=
				(right.numerator * (denominator / right.denominator));
		}

		/// <summary>
		/// Standard implementation of the operator.
		/// </summary>
		public static bool operator <=(FIURational left, FIURational right)
		{
			ulong denominator = Scm(left.denominator, right.denominator);
			return (left.numerator * (denominator / left.denominator)) <=
				(right.numerator * (denominator / right.denominator));
		}

		#endregion

		#region Conversions

		/// <summary>
		/// Converts the value of a <see cref="FIURational"/> structure to a <see cref="Boolean"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIURational"/> structure.</param>
		/// <returns>A new instance of <see cref="Boolean"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator bool(FIURational value)
		{
			return (value.numerator != 0);
		}

		/// <summary>
		/// Converts the value of a <see cref="FIURational"/> structure to a <see cref="Byte"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIURational"/> structure.</param>
		/// <returns>A new instance of <see cref="Byte"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator byte(FIURational value)
		{
			return (byte)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIURational"/> structure to a <see cref="Char"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIURational"/> structure.</param>
		/// <returns>A new instance of <see cref="Char"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator char(FIURational value)
		{
			return (char)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIURational"/> structure to a <see cref="Decimal"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIURational"/> structure.</param>
		/// <returns>A new instance of <see cref="Decimal"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator decimal(FIURational value)
		{
			return value.denominator == 0 ? 0m : (decimal)value.numerator / (decimal)value.denominator;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIURational"/> structure to a <see cref="Double"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIURational"/> structure.</param>
		/// <returns>A new instance of <see cref="Double"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator double(FIURational value)
		{
			return value.denominator == 0 ? 0d : (double)value.numerator / (double)value.denominator;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIURational"/> structure to an <see cref="Int16"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIURational"/> structure.</param>
		/// <returns>A new instance of <see cref="Int16"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator short(FIURational value)
		{
			return (short)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIURational"/> structure to an <see cref="Int32"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIURational"/> structure.</param>
		/// <returns>A new instance of <see cref="Int32"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator int(FIURational value)
		{
			return (int)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIURational"/> structure to an <see cref="Int64"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIURational"/> structure.</param>
		/// <returns>A new instance of <see cref="Int64"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator long(FIURational value)
		{
			return (byte)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIURational"/> structure to a <see cref="Single"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIURational"/> structure.</param>
		/// <returns>A new instance of <see cref="Single"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator float(FIURational value)
		{
			return value.denominator == 0 ? 0f : (float)value.numerator / (float)value.denominator;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIURational"/> structure to a <see cref="SByte"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIURational"/> structure.</param>
		/// <returns>A new instance of <see cref="SByte"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator sbyte(FIURational value)
		{
			return (sbyte)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIURational"/> structure to an <see cref="UInt16"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIURational"/> structure.</param>
		/// <returns>A new instance of <see cref="UInt16"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator ushort(FIURational value)
		{
			return (ushort)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIURational"/> structure to an <see cref="UInt32"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIURational"/> structure.</param>
		/// <returns>A new instance of <see cref="UInt32"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator uint(FIURational value)
		{
			return (uint)(double)value;
		}

		/// <summary>
		/// Converts the value of a <see cref="FIURational"/> structure to an <see cref="UInt32"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="FIURational"/> structure.</param>
		/// <returns>A new instance of <see cref="UInt32"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator ulong(FIURational value)
		{
			return (ulong)(double)value;
		}

		//

		/// <summary>
		/// Converts the value of a <see cref="Boolean"/> structure to a <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="Boolean"/> structure.</param>
		/// <returns>A new instance of <see cref="FIURational"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator FIURational(bool value)
		{
			return new FIURational(value ? 1u : 0u, 1u);
		}

		/// <summary>
		/// Converts the value of a <see cref="Byte"/> structure to a <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="Byte"/> structure.</param>
		/// <returns>A new instance of <see cref="FIURational"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIURational(byte value)
		{
			return new FIURational(value, 1);
		}

		/// <summary>
		/// Converts the value of a <see cref="Char"/> structure to a <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="Char"/> structure.</param>
		/// <returns>A new instance of <see cref="FIURational"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIURational(char value)
		{
			return new FIURational(value, 1);
		}

		/// <summary>
		/// Converts the value of a <see cref="Decimal"/> structure to a <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="Decimal"/> structure.</param>
		/// <returns>A new instance of <see cref="FIURational"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator FIURational(decimal value)
		{
			return new FIURational(value);
		}

		/// <summary>
		/// Converts the value of a <see cref="Double"/> structure to a <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="Double"/> structure.</param>
		/// <returns>A new instance of <see cref="FIURational"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator FIURational(double value)
		{
			return new FIURational((decimal)value);
		}

		/// <summary>
		/// Converts the value of an <see cref="Int16"/> structure to a <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="value">An <see cref="Int16"/> structure.</param>
		/// <returns>A new instance of <see cref="FIURational"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIURational(short value)
		{
			return new FIURational((uint)value, 1u);
		}

		/// <summary>
		/// Converts the value of an <see cref="Int32"/> structure to a <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="value">An <see cref="Int32"/> structure.</param>
		/// <returns>A new instance of <see cref="FIURational"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIURational(int value)
		{
			return new FIURational((uint)value, 1u);
		}

		/// <summary>
		/// Converts the value of an <see cref="Int64"/> structure to a <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="value">An <see cref="Int64"/> structure.</param>
		/// <returns>A new instance of <see cref="FIURational"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator FIURational(long value)
		{
			return new FIURational((uint)value, 1u);
		}

		/// <summary>
		/// Converts the value of a <see cref="SByte"/> structure to a <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="SByte"/> structure.</param>
		/// <returns>A new instance of <see cref="FIURational"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIURational(sbyte value)
		{
			return new FIURational((uint)value, 1u);
		}

		/// <summary>
		/// Converts the value of a <see cref="Single"/> structure to a <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="value">A <see cref="Single"/> structure.</param>
		/// <returns>A new instance of <see cref="FIURational"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator FIURational(float value)
		{
			return new FIURational((decimal)value);
		}

		/// <summary>
		/// Converts the value of an <see cref="UInt16"/> structure to a <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="value">An <see cref="UInt16"/> structure.</param>
		/// <returns>A new instance of <see cref="FIURational"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FIURational(ushort value)
		{
			return new FIURational(value, 1);
		}

		/// <summary>
		/// Converts the value of an <see cref="UInt32"/> structure to a <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="value">An <see cref="UInt32"/> structure.</param>
		/// <returns>A new instance of <see cref="FIURational"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator FIURational(uint value)
		{
			return new FIURational(value, 1u);
		}

		/// <summary>
		/// Converts the value of an <see cref="UInt64"/> structure to a <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="value">An <see cref="UInt64"/> structure.</param>
		/// <returns>A new instance of <see cref="FIURational"/> initialized to <paramref name="value"/>.</returns>
		public static explicit operator FIURational(ulong value)
		{
			return new FIURational((uint)value, 1u);
		}

		#endregion

		#region IConvertible Member

		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Double;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return (bool)this;
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return (byte)this;
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return (char)this;
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(((IConvertible)this).ToDouble(provider));
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return this;
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return this;
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return (short)this;
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return (int)this;
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return (long)this;
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return (sbyte)this;
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return this;
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return ToString(((double)this).ToString(), provider);
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return Convert.ChangeType(((IConvertible)this).ToDouble(provider), conversionType, provider);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return (ushort)this;
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return (uint)this;
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return (ulong)this;
		}

		#endregion

		#region IComparable Member

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="FIURational"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is FIURational))
			{
				throw new ArgumentException();
			}
			return CompareTo((FIURational)obj);
		}

		#endregion

		#region IFormattable Member

		/// <summary>
		/// Formats the value of the current instance using the specified format.
		/// </summary>
		/// <param name="format">The String specifying the format to use.</param>
		/// <param name="formatProvider">The IFormatProvider to use to format the value.</param>
		/// <returns>A String containing the value of the current instance in the specified format.</returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null)
			{
				format = "";
			}
			return String.Format(formatProvider, format, ((IConvertible)this).ToDouble(formatProvider));
		}

		#endregion

		#region IEquatable<FIURational> Member

		/// <summary>
		/// Tests whether the specified <see cref="FIURational"/> structure is equivalent to this <see cref="FIURational"/> structure.
		/// </summary>
		/// <param name="other">A <see cref="FIURational"/> structure to compare to this instance.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="FIURational"/> structure
		/// equivalent to this <see cref="FIURational"/> structure; otherwise, <b>false</b>.</returns>
		public bool Equals(FIURational other)
		{
			return (this == other);
		}

		#endregion

		#region IComparable<FIURational> Member

		/// <summary>
		/// Compares this instance with a specified <see cref="FIURational"/> object.
		/// </summary>
		/// <param name="other">A <see cref="FIURational"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(FIURational other)
		{
			FIURational difference = this - other;
			difference.Normalize();
			if (difference.numerator > 0) return 1;
			if (difference.numerator < 0) return -1;
			else return 0;
		}

		#endregion
	}
}

	#endregion

	#region Classes

namespace FreeImageAPI
{
	/// <summary>
	/// Encapsulates a FreeImage-bitmap.
	/// </summary>
	[Serializable, Guid("64a4c935-b757-499c-ab8c-6110316a9e51")]
	public class FreeImageBitmap : ICloneable, IDisposable, IEnumerable, ISerializable
	{
		#region Fields

		private bool disposed;
		private object tag;
		private object lockObject = new object();
		private SaveInformation saveInformation = new SaveInformation();

		/// <summary>
		/// Format of the sourceimage.
		/// </summary>
		private FREE_IMAGE_FORMAT originalFormat = FREE_IMAGE_FORMAT.FIF_UNKNOWN;

		/// <summary>
		/// Handle to the encapsulated FreeImage-bitmap.
		/// </summary>
		private FIBITMAP dib;

		/// <summary>
		/// Handle to the encapsulated FreeImage-multipagebitmap.
		/// </summary>
		private FIMULTIBITMAP mdib;

		#endregion

		#region Constructors and Destructor

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class.
		/// </summary>
		protected FreeImageBitmap()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class.
		/// For internal use only.
		/// </summary>
		/// <exception cref="Exception">The operation failed.</exception>
		internal protected FreeImageBitmap(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new Exception();
			}
			this.dib = dib;
			AddMemoryPressure();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified image.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(FreeImageBitmap original)
		{
			original.EnsureNotDisposed();
			dib = FreeImage.Clone(original.dib);
			if (dib.IsNull)
			{
				throw new Exception();
			}
			AddMemoryPressure();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new <see cref="FreeImageBitmap"/>.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="newSize.Width"/> or <paramref name="newSize.Height"/> are less or equal zero.
		/// </exception>
		public FreeImageBitmap(FreeImageBitmap original, Size newSize)
			: this(original, newSize.Width, newSize.Height)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="width">Width of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="height">Height of the new <see cref="FreeImageBitmap"/>.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="width"/> or <paramref name="height"/> are less or equal zero.</exception>
		public FreeImageBitmap(FreeImageBitmap original, int width, int height)
		{
			if (original == null)
			{
				throw new ArgumentNullException("original");
			}
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			original.EnsureNotDisposed();
			dib = FreeImage.Rescale(original.dib, width, height, FREE_IMAGE_FILTER.FILTER_BICUBIC);
			if (dib.IsNull)
			{
				throw new Exception();
			}
			AddMemoryPressure();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified image.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <remarks>
		/// Although this constructor supports creating images in both formats
		/// <see cref="System.Drawing.Imaging.PixelFormat.Format32bppPArgb"/>
		/// and <see cref="System.Drawing.Imaging.PixelFormat.Format64bppPArgb"/>, bitmaps
		/// created in these formats are treated like any normal 32-bit RGBA and 64-bit RGBA
		/// images respectively. Currently, there is no  support for automatic premultiplying images in
		/// <see cref="FreeImageBitmap"/>.
		/// </remarks>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(Image original)
			: this(original as Bitmap)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new <see cref="FreeImageBitmap"/>.</param>
		/// <remarks>
		/// Although this constructor supports creating images in both formats
		/// <see cref="System.Drawing.Imaging.PixelFormat.Format32bppPArgb"/>
		/// and <see cref="System.Drawing.Imaging.PixelFormat.Format64bppPArgb"/>, bitmaps
		/// created in these formats are treated like any normal 32-bit RGBA and 64-bit RGBA
		/// images respectively. Currently, there is no  support for automatic premultiplying images in
		/// <see cref="FreeImageBitmap"/>.
		/// </remarks>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="newSize.Width"/> or <paramref name="newSize.Height"/> are less or equal zero.
		/// </exception>
		public FreeImageBitmap(Image original, Size newSize)
			: this(original as Bitmap, newSize.Width, newSize.Height)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="width">The width, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="height">The height, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <remarks>
		/// Although this constructor supports creating images in both formats
		/// <see cref="System.Drawing.Imaging.PixelFormat.Format32bppPArgb"/>
		/// and <see cref="System.Drawing.Imaging.PixelFormat.Format64bppPArgb"/>, bitmaps
		/// created in these formats are treated like any normal 32-bit RGBA and 64-bit RGBA
		/// images respectively. Currently, there is no  support for automatic premultiplying images in
		/// <see cref="FreeImageBitmap"/>.
		/// </remarks>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="width"/> or <paramref name="height"/> are less or equal zero.</exception>
		public FreeImageBitmap(Image original, int width, int height)
			: this(original as Bitmap, width, height)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified image.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <remarks>
		/// Although this constructor supports creating images in both formats
		/// <see cref="System.Drawing.Imaging.PixelFormat.Format32bppPArgb"/>
		/// and <see cref="System.Drawing.Imaging.PixelFormat.Format64bppPArgb"/>, bitmaps
		/// created in these formats are treated like any normal 32-bit RGBA and 64-bit RGBA
		/// images respectively. Currently, there is no  support for automatic premultiplying images in
		/// <see cref="FreeImageBitmap"/>.
		/// </remarks>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(Bitmap original)
		{
			if (original == null)
			{
				throw new ArgumentNullException("original");
			}
			dib = FreeImage.CreateFromBitmap(original, true);
			if (dib.IsNull)
			{
				throw new Exception();
			}
			AddMemoryPressure();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new <see cref="FreeImageBitmap"/>.</param>
		/// <remarks>
		/// Although this constructor supports creating images in both formats
		/// <see cref="System.Drawing.Imaging.PixelFormat.Format32bppPArgb"/>
		/// and <see cref="System.Drawing.Imaging.PixelFormat.Format64bppPArgb"/>, bitmaps
		/// created in these formats are treated like any normal 32-bit RGBA and 64-bit RGBA
		/// images respectively. Currently, there is no  support for automatic premultiplying images in
		/// <see cref="FreeImageBitmap"/>.
		/// </remarks>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="newSize.Width"/> or <paramref name="newSize.Height"/> are less or equal zero.
		/// </exception>
		public FreeImageBitmap(Bitmap original, Size newSize)
			: this(original, newSize.Width, newSize.Height)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="width">The width, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="height">The height, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <remarks>
		/// Although this constructor supports creating images in both formats
		/// <see cref="System.Drawing.Imaging.PixelFormat.Format32bppPArgb"/>
		/// and <see cref="System.Drawing.Imaging.PixelFormat.Format64bppPArgb"/>, bitmaps
		/// created in these formats are treated like any normal 32-bit RGBA and 64-bit RGBA
		/// images respectively. Currently, there is no  support for automatic premultiplying images in
		/// <see cref="FreeImageBitmap"/>.
		/// </remarks>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="width"/> or <paramref name="height"/> are less or equal zero.</exception>
		public FreeImageBitmap(Bitmap original, int width, int height)
		{
			if (original == null)
			{
				throw new ArgumentNullException("original");
			}
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			FIBITMAP temp = FreeImage.CreateFromBitmap(original, true);
			if (temp.IsNull)
			{
				throw new Exception();
			}
			dib = FreeImage.Rescale(temp, width, height, FREE_IMAGE_FILTER.FILTER_BICUBIC);
			FreeImage.Unload(temp);
			if (dib.IsNull)
			{
				throw new Exception();
			}
			AddMemoryPressure();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified stream.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <param name="useIcm">Ignored.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream, bool useIcm)
			: this(stream)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified stream.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream)
			: this(stream, FREE_IMAGE_FORMAT.FIF_UNKNOWN, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified stream in the specified format.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <param name="format">Format of the image.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream, FREE_IMAGE_FORMAT format)
			: this(stream, format, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified stream with the specified loading flags.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream, FREE_IMAGE_LOAD_FLAGS flags)
			: this(stream, FREE_IMAGE_FORMAT.FIF_UNKNOWN, flags)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified stream in the specified format
		/// with the specified loading flags.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream, FREE_IMAGE_FORMAT format, FREE_IMAGE_LOAD_FLAGS flags)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			saveInformation.loadFlags = flags;

			dib = FreeImage.LoadFromStream(stream, flags, ref format);

			if (dib.IsNull)
			{
				throw new Exception();
			}

			AddMemoryPressure();
			originalFormat = format;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class bases on the specified file.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename)
			: this(filename, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class bases on the specified file.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="useIcm">Ignored.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename, bool useIcm)
			: this(filename)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class bases on the specified file
		/// with the specified loading flags.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename, FREE_IMAGE_LOAD_FLAGS flags)
			: this(filename, FREE_IMAGE_FORMAT.FIF_UNKNOWN, flags)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class bases on the specified file
		/// in the specified format.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="format">Format of the image.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename, FREE_IMAGE_FORMAT format)
			: this(filename, format, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class bases on the specified file
		/// in the specified format with the specified loading flags.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename, FREE_IMAGE_FORMAT format, FREE_IMAGE_LOAD_FLAGS flags)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException("filename");
			}
			saveInformation.loadFlags = flags;

			mdib = FreeImage.OpenMultiBitmapEx(filename, ref format, flags, false, true, false);

			if (mdib.IsNull)
			{
				throw new Exception();
			}

			originalFormat = format;

			if (FreeImage.GetPageCount(mdib) != 0)
			{
				if (FreeImage.GetPageCount(mdib) == 1)
				{
					if (!FreeImage.CloseMultiBitmapEx(ref mdib, FREE_IMAGE_SAVE_FLAGS.DEFAULT))
					{
						throw new Exception("Unable to unload temporary FIMULTIBITMAP.");
					}

					dib = FreeImage.LoadEx(filename, flags, ref format);
					if (dib.IsNull)
					{
						throw new Exception();
					}

					AddMemoryPressure();
					return;
				}
				else
				{
					dib = FreeImage.LockPage(mdib, 0);
					if (!dib.IsNull)
					{
						AddMemoryPressure();
						return;
					}
				}
			}

			FreeImage.CloseMultiBitmap(mdib, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
			throw new Exception();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class
		/// bases on the specified size.
		/// </summary>
		/// <param name="width">The width, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="height">The height, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(int width, int height)
		{
			dib = FreeImage.Allocate(
				width,
				height,
				24,
				FreeImage.FI_RGBA_RED_MASK,
				FreeImage.FI_RGBA_GREEN_MASK,
				FreeImage.FI_RGBA_BLUE_MASK);
			if (dib.IsNull)
			{
				throw new Exception();
			}
			AddMemoryPressure();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class bases on the specified resource.
		/// </summary>
		/// <param name="type">The class used to extract the resource.</param>
		/// <param name="resource">The name of the resource.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(Type type, string resource)
			: this(type.Module.Assembly.GetManifestResourceStream(type, resource))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class bases on the specified size
		/// and with the resolution of the specified <see cref="System.Drawing.Graphics"/> object.
		/// </summary>
		/// <param name="width">The width, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="height">The height, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="g">The Graphics object that specifies the resolution for the new <see cref="FreeImageBitmap"/>.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="g"/> is null.</exception>
		public FreeImageBitmap(int width, int height, Graphics g)
			: this(width, height)
		{
			FreeImage.SetResolutionX(dib, (uint)g.DpiX);
			FreeImage.SetResolutionX(dib, (uint)g.DpiY);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class bases on the specified size and format.
		/// </summary>
		/// <param name="width">The width, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="height">The height, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="format">The PixelFormat enumeration for the new <see cref="FreeImageBitmap"/>.</param>
		/// <remarks>
		/// Although this constructor supports creating images in both formats
		/// <see cref="System.Drawing.Imaging.PixelFormat.Format32bppPArgb"/>
		/// and <see cref="System.Drawing.Imaging.PixelFormat.Format64bppPArgb"/>, bitmaps
		/// created in these formats are treated like any normal 32-bit RGBA and 64-bit RGBA
		/// images respectively. Currently, there is no  support for automatic premultiplying images in
		/// <see cref="FreeImageBitmap"/>.
		/// </remarks>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentException"><paramref name="format"/> is invalid.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="width"/> or <paramref name="height"/> are less or equal zero.</exception>
		public FreeImageBitmap(int width, int height, PixelFormat format)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			uint bpp, redMask, greenMask, blueMask;
			FREE_IMAGE_TYPE type;
			if (!FreeImage.GetFormatParameters(format, out type, out bpp, out redMask, out greenMask, out blueMask))
			{
				throw new ArgumentException("format is invalid");
			}
			dib = FreeImage.AllocateT(type, width, height, (int)bpp, redMask, greenMask, blueMask);
			if (dib.IsNull)
			{
				throw new Exception();
			}
			AddMemoryPressure();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class bases on the specified size and type.
		/// Only non standard bitmaps are supported.
		/// </summary>	
		/// <param name="width">The width, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="height">The height, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="type">The type of the bitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="type"/> is FIT_BITMAP or FIT_UNKNOWN.</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> is invalid.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="width"/> or <paramref name="height"/> are less or equal zero.</exception>
		public FreeImageBitmap(int width, int height, FREE_IMAGE_TYPE type)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			if ((type == FREE_IMAGE_TYPE.FIT_BITMAP) || (type == FREE_IMAGE_TYPE.FIT_UNKNOWN))
			{
				throw new ArgumentException("type is invalid.");
			}
			dib = FreeImage.AllocateT(type, width, height, 0, 0u, 0u, 0u);
			if (dib.IsNull)
			{
				throw new Exception();
			}
			AddMemoryPressure();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class bases on the specified size,
		/// pixel format and pixel data.
		/// </summary>
		/// <param name="width">The width, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="height">The height, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="stride">Integer that specifies the byte offset between the beginning
		/// of one scan line and the next. This is usually (but not necessarily)
		/// the number of bytes in the pixel format (for example, 2 for 16 bits per pixel)
		/// multiplied by the width of the bitmap. The value passed to this parameter must
		/// be a multiple of four..</param>
		/// <param name="format">The PixelFormat enumeration for the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="scan0">Pointer to an array of bytes that contains the pixel data.</param>
		/// <remarks>
		/// Although this constructor supports creating images in both formats
		/// <see cref="System.Drawing.Imaging.PixelFormat.Format32bppPArgb"/>
		/// and <see cref="System.Drawing.Imaging.PixelFormat.Format64bppPArgb"/>, bitmaps
		/// created in these formats are treated like any normal 32-bit RGBA and 64-bit RGBA
		/// images respectively. Currently, there is no  support for automatic premultiplying images in
		/// <see cref="FreeImageBitmap"/>.
		/// </remarks>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentException"><paramref name="format"/> is invalid.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="width"/> or <paramref name="height"/> are less or equal zero.</exception>
		public FreeImageBitmap(int width, int height, int stride, PixelFormat format, IntPtr scan0)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			uint bpp, redMask, greenMask, blueMask;
			FREE_IMAGE_TYPE type;
			bool topDown = (stride > 0);
			stride = (stride > 0) ? stride : (stride * -1);

			if (!FreeImage.GetFormatParameters(format, out type, out bpp, out redMask, out greenMask, out blueMask))
			{
				throw new ArgumentException("format is invalid.");
			}

			dib = FreeImage.ConvertFromRawBits(
				scan0, type, width, height, stride, bpp, redMask, greenMask, blueMask, topDown);

			if (dib.IsNull)
			{
				throw new Exception();
			}
			AddMemoryPressure();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class bases on the specified size,
		/// pixel format and pixel data.
		/// </summary>
		/// <param name="width">The width, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="height">The height, in pixels, of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="stride">Integer that specifies the byte offset between the beginning
		/// of one scan line and the next. This is usually (but not necessarily)
		/// the number of bytes in the pixel format (for example, 2 for 16 bits per pixel)
		/// multiplied by the width of the bitmap. The value passed to this parameter must
		/// be a multiple of four..</param>
		/// <param name="bpp">The color depth of the new <see cref="FreeImageBitmap"/></param>
		/// <param name="type">The type for the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="scan0">Pointer to an array of bytes that contains the pixel data.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentException"><paramref name="format"/> is invalid.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="width"/> or <paramref name="height"/> are less or equal zero.</exception>
		public FreeImageBitmap(int width, int height, int stride, int bpp, FREE_IMAGE_TYPE type, IntPtr scan0)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			uint redMask, greenMask, blueMask;
			bool topDown = (stride > 0);
			stride = (stride > 0) ? stride : (stride * -1);

			if (!FreeImage.GetTypeParameters(type, bpp, out redMask, out greenMask, out blueMask))
			{
				throw new ArgumentException("bpp and type are invalid or not supported.");
			}

			dib = FreeImage.ConvertFromRawBits(
				scan0, type, width, height, stride, (uint)bpp, redMask, greenMask, blueMask, topDown);

			if (dib.IsNull)
			{
				throw new Exception();
			}
			AddMemoryPressure();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreeImageBitmap"/> class.
		/// </summary>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="SerializationException">The operation failed.</exception>
		public FreeImageBitmap(SerializationInfo info, StreamingContext context)
		{
			try
			{
				byte[] data = (byte[])info.GetValue("Bitmap Data", typeof(byte[]));
				if ((data != null) && (data.Length > 0))
				{
					MemoryStream memory = new MemoryStream(data);
					FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_TIFF;
					dib = FreeImage.LoadFromStream(memory, ref format);

					if (dib.IsNull)
					{
						throw new Exception();
					}

					AddMemoryPressure();
				}
			}
			catch
			{
				throw new SerializationException();
			}
		}

		/// <summary>
		/// Frees all managed and unmanaged ressources.
		/// </summary>
		~FreeImageBitmap()
		{
			Dispose(false);
		}

		#endregion

		#region Operators

		/// <summary>
		/// Converts a <see cref="FreeImageBitmap"/> instance to a <see cref="Bitmap"/> instance.
		/// </summary>
		/// <param name="value">A <see cref="FreeImageBitmap"/> instance.</param>
		/// <returns>A new instance of <see cref="Bitmap"/> initialized to <paramref name="value"/>.</returns>
		/// <remarks>
		/// The explicit conversion from <see cref="FreeImageBitmap"/> into Bitmap
		/// allows to create an instance on the fly and use it as if
		/// was a Bitmap. This way it can be directly used with a
		/// PixtureBox for example without having to call any
		/// conversion operations.
		/// </remarks>
		public static explicit operator Bitmap(FreeImageBitmap value)
		{
			return value.ToBitmap();
		}

		/// <summary>
		/// Converts a <see cref="Bitmap"/> instance to a <see cref="FreeImageBitmap"/> instance.
		/// </summary>
		/// <param name="value">A <see cref="Bitmap"/> instance.</param>
		/// <returns>A new instance of <see cref="FreeImageBitmap"/> initialized to <paramref name="value"/>.</returns>
		/// <remarks>
		/// The explicit conversion from <see cref="Bitmap"/> into <see cref="FreeImageBitmap"/>
		/// allows to create an instance on the fly to perform
		/// image processing operations and converting it back.
		/// </remarks>
		public static explicit operator FreeImageBitmap(Bitmap value)
		{
			return new FreeImageBitmap(value);
		}

		/// <summary>
		/// Determines whether two specified <see cref="FreeImageBitmap"/> objects have the same value.
		/// </summary>
		/// <param name="left">A <see cref="FreeImageBitmap"/> or a null reference (<b>Nothing</b> in Visual Basic).</param>
		/// <param name="right">A <see cref="FreeImageBitmap"/> or a null reference (<b>Nothing</b> in Visual Basic).</param>
		/// <returns>
		/// <b>true</b> if the value of left is the same as the value of right; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(FreeImageBitmap left, FreeImageBitmap right)
		{
			if (object.ReferenceEquals(left, right))
			{
				return true;
			}
			else if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
			{
				return false;
			}
			else
			{
				left.EnsureNotDisposed();
				right.EnsureNotDisposed();
				return FreeImage.Compare(left.dib, right.dib, FREE_IMAGE_COMPARE_FLAGS.COMPLETE);
			}
		}

		/// <summary>
		/// Determines whether two specified <see cref="FreeImageBitmap"/> objects have different values.
		/// </summary>
		/// <param name="left">A <see cref="FreeImageBitmap"/> or a null reference (<b>Nothing</b> in Visual Basic).</param>
		/// <param name="right">A <see cref="FreeImageBitmap"/> or a null reference (<b>Nothing</b> in Visual Basic).</param>
		/// <returns>
		/// true if the value of left is different from the value of right; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(FreeImageBitmap left, FreeImageBitmap right)
		{
			return !(left == right);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Type of the bitmap.
		/// </summary>
		public FREE_IMAGE_TYPE ImageType
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetImageType(dib);
			}
		}

		/// <summary>
		/// Number of palette entries.
		/// </summary>
		public int ColorsUsed
		{
			get
			{
				EnsureNotDisposed();
				return (int)FreeImage.GetColorsUsed(dib);
			}
		}

		/// <summary>
		/// The number of unique colors actually used by the bitmap. This might be different from
		/// what ColorsUsed returns, which actually returns the palette size for palletised images.
		/// Works for FIT_BITMAP type bitmaps only.
		/// </summary>
		public int UniqueColors
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetUniqueColors(dib);
			}
		}

		/// <summary>
		/// The size of one pixel in the bitmap in bits.
		/// </summary>
		public int ColorDepth
		{
			get
			{
				EnsureNotDisposed();
				return (int)FreeImage.GetBPP(dib);
			}
		}

		/// <summary>
		/// Width of the bitmap in pixel units.
		/// </summary>
		public int Width
		{
			get
			{
				EnsureNotDisposed();
				return (int)FreeImage.GetWidth(dib);
			}
		}

		/// <summary>
		/// Height of the bitmap in pixel units.
		/// </summary>
		public int Height
		{
			get
			{
				EnsureNotDisposed();
				return (int)FreeImage.GetHeight(dib);
			}
		}

		/// <summary>
		/// Returns the width of the bitmap in bytes, rounded to the next 32-bit boundary.
		/// </summary>
		public int Pitch
		{
			get
			{
				EnsureNotDisposed();
				return (int)FreeImage.GetPitch(dib);
			}
		}

		/// <summary>
		/// Size of the bitmap in memory.
		/// </summary>
		public int DataSize
		{
			get
			{
				EnsureNotDisposed();
				return (int)FreeImage.GetDIBSize(dib);
			}
		}

		/// <summary>
		/// Returns a structure that represents the palette of a FreeImage bitmap.
		/// </summary>
		/// <exception cref="Exception"><see cref="HasPalette"/> is false.</exception>
		public Palette Palette
		{
			get
			{
				EnsureNotDisposed();
				if (HasPalette)
				{
					return new Palette(dib);
				}
				throw new Exception();
			}
		}

		/// <summary>
		/// Gets whether the bitmap is RGB 555.
		/// </summary>
		public bool IsRGB555
		{
			get
			{
				return FreeImage.IsRGB555(dib);
			}
		}

		/// <summary>
		/// Gets whether the bitmap is RGB 565.
		/// </summary>
		public bool IsRGB565
		{
			get
			{
				return FreeImage.IsRGB565(dib);
			}
		}

		/// <summary>
		/// Gets the horizontal resolution, in pixels per inch, of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public float HorizontalResolution
		{
			get
			{
				EnsureNotDisposed();
				return (float)FreeImage.GetResolutionX(dib);
			}
			private set
			{
				EnsureNotDisposed();
				FreeImage.SetResolutionX(dib, (uint)value);
			}
		}

		/// <summary>
		/// Gets the vertical resolution, in pixels per inch, of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public float VerticalResolution
		{
			get
			{
				EnsureNotDisposed();
				return (float)FreeImage.GetResolutionY(dib);
			}
			private set
			{
				EnsureNotDisposed();
				FreeImage.SetResolutionY(dib, (uint)value);
			}
		}

		/// <summary>
		/// Returns the <see cref="BITMAPINFOHEADER"/> structure of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public BITMAPINFOHEADER InfoHeader
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetInfoHeaderEx(dib);
			}
		}

		/// <summary>
		/// Returns the <see cref="BITMAPINFO"/> structure of a this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public BITMAPINFO Info
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetInfoEx(dib);
			}
		}

		/// <summary>
		/// Investigates the color type of this <see cref="FreeImageBitmap"/>
		/// by reading the bitmaps pixel bits and analysing them.
		/// </summary>
		public FREE_IMAGE_COLOR_TYPE ColorType
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetColorType(dib);
			}
		}

		/// <summary>
		/// Bit pattern describing the red color component of a pixel in this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public uint RedMask
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetRedMask(dib);
			}
		}

		/// <summary>
		/// Bit pattern describing the green color component of a pixel in this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public uint GreenMask
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetGreenMask(dib);
			}
		}

		/// <summary>
		/// Bit pattern describing the blue color component of a pixel in this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public uint BlueMask
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetBlueMask(dib);
			}
		}

		/// <summary>
		/// Number of transparent colors in a palletised <see cref="FreeImageBitmap"/>.
		/// </summary>
		public int TransparencyCount
		{
			get
			{
				EnsureNotDisposed();
				return (int)FreeImage.GetTransparencyCount(dib);
			}
		}

		/// <summary>
		/// Get or sets transparency table of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public byte[] TransparencyTable
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetTransparencyTableEx(dib);
			}
			set
			{
				EnsureNotDisposed();
				FreeImage.SetTransparencyTable(dib, value);
			}
		}

		/// <summary>
		/// Gets or sets whether this <see cref="FreeImageBitmap"/> is transparent.
		/// </summary>
		public bool IsTransparent
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.IsTransparent(dib);
			}
			set
			{
				EnsureNotDisposed();
				FreeImage.SetTransparent(dib, value);
			}
		}

		/// <summary>
		/// Gets whether this <see cref="FreeImageBitmap"/> has a file background color.
		/// </summary>
		public bool HasBackgroundColor
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.HasBackgroundColor(dib);
			}
		}

		/// <summary>
		/// Gets or sets the background color of this <see cref="FreeImageBitmap"/>.
		/// In case the value is null, the background color is removed.
		/// </summary>
		/// <exception cref="InvalidOperationException">Get: There is no background color available.</exception>
		/// <exception cref="Exception">Set: Setting background color failed.</exception>
		public Color? BackgroundColor
		{
			get
			{
				EnsureNotDisposed();
				if (!FreeImage.HasBackgroundColor(dib))
				{
					throw new InvalidOperationException("No background color available.");
				}
				RGBQUAD rgbq;
				FreeImage.GetBackgroundColor(dib, out rgbq);
				return rgbq.Color;
			}
			set
			{
				EnsureNotDisposed();
				if (!FreeImage.SetBackgroundColor(dib, (value.HasValue ? new RGBQUAD[] { value.Value } : null)))
				{
					throw new Exception("Setting background color failed.");
				}
			}
		}

		/// <summary>
		/// Pointer to the data-bits of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public IntPtr Bits
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetBits(dib);
			}
		}

		/// <summary>
		/// Width, in bytes, of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public int Line
		{
			get
			{
				EnsureNotDisposed();
				return (int)FreeImage.GetLine(dib);
			}
		}

		/// <summary>
		/// Pointer to the scanline of the top most pixel row of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public IntPtr Scan0
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetScanLine(dib, (int)(FreeImage.GetHeight(dib) - 1));
			}
		}

		/// <summary>
		/// Width, in bytes, of this <see cref="FreeImageBitmap"/>.
		/// In case this <see cref="FreeImageBitmap"/> is top down <b>Stride</b> will be positive, else negative.
		/// </summary>
		public int Stride
		{
			get
			{
				return -Line;
			}
		}

		/// <summary>
		/// Gets attribute flags for the pixel data of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public unsafe int Flags
		{
			get
			{
				EnsureNotDisposed();
				int result = 0;
				byte alpha;
				int cd = ColorDepth;

				if ((cd == 32) || (FreeImage.GetTransparencyCount(dib) != 0))
				{
					result += (int)ImageFlags.HasAlpha;
				}

				if (cd == 32)
				{
					uint width = FreeImage.GetWidth(dib);
					uint height = FreeImage.GetHeight(dib);
					for (int y = 0; y < height; y++)
					{
						RGBQUAD* scanline = (RGBQUAD*)FreeImage.GetScanLine(dib, y);
						for (int x = 0; x < width; x++)
						{
							alpha = scanline[x].Color.A;
							if (alpha != byte.MinValue && alpha != byte.MaxValue)
							{
								result += (int)ImageFlags.HasTranslucent;
								y = (int)height;
								break;
							}
						}
					}
				}
				else if (FreeImage.GetTransparencyCount(dib) != 0)
				{
					byte[] transTable = FreeImage.GetTransparencyTableEx(dib);
					for (int i = 0; i < transTable.Length; i++)
					{
						if (transTable[i] != byte.MinValue && transTable[i] != byte.MaxValue)
						{
							result += (int)ImageFlags.HasTranslucent;
							break;
						}
					}
				}

				if (FreeImage.GetICCProfileEx(dib).IsCMYK)
				{
					result += (int)ImageFlags.ColorSpaceCmyk;
				}
				else
				{
					result += (int)ImageFlags.ColorSpaceRgb;
				}

				if (FreeImage.GetColorType(dib) == FREE_IMAGE_COLOR_TYPE.FIC_MINISBLACK ||
					FreeImage.GetColorType(dib) == FREE_IMAGE_COLOR_TYPE.FIC_MINISWHITE)
				{
					result += (int)ImageFlags.ColorSpaceGray;
				}

				if (originalFormat == FREE_IMAGE_FORMAT.FIF_BMP ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_FAXG3 ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_ICO ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_JPEG ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_PCX ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_PNG ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_PSD ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_TIFF)
				{
					result += (int)ImageFlags.HasRealDpi;
				}

				return result;
			}
		}

		/// <summary>
		/// Gets the width and height of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public SizeF PhysicalDimension
		{
			get
			{
				EnsureNotDisposed();
				return new SizeF((float)FreeImage.GetWidth(dib), (float)FreeImage.GetHeight(dib));
			}
		}

		/// <summary>
		/// Gets the pixel format for this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public PixelFormat PixelFormat
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetPixelFormat(dib);
			}
		}

		/// <summary>
		/// Gets IDs of the property items stored in this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public int[] PropertyIdList
		{
			get
			{
				EnsureNotDisposed();
				List<int> list = new List<int>();
				ImageMetadata metaData = new ImageMetadata(dib, true);

				foreach (MetadataModel metadataModel in metaData)
				{
					foreach (MetadataTag metadataTag in metadataModel)
					{
						list.Add(metadataTag.ID);
					}
				}

				return list.ToArray();
			}
		}

		/// <summary>
		/// Gets all the property items (pieces of metadata) stored in this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public PropertyItem[] PropertyItems
		{
			get
			{
				EnsureNotDisposed();
				List<PropertyItem> list = new List<PropertyItem>();
				ImageMetadata metaData = new ImageMetadata(dib, true);

				foreach (MetadataModel metadataModel in metaData)
				{
					foreach (MetadataTag metadataTag in metadataModel)
					{
						list.Add(metadataTag.GetPropertyItem());
					}
				}

				return list.ToArray();
			}
		}

		/// <summary>
		/// Gets the format of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public ImageFormat RawFormat
		{
			get
			{
				EnsureNotDisposed();
				Attribute guidAttribute =
					Attribute.GetCustomAttribute(
						typeof(FreeImageBitmap), typeof(System.Runtime.InteropServices.GuidAttribute)
					);
				return (guidAttribute == null) ?
					null :
					new ImageFormat(new Guid(((GuidAttribute)guidAttribute).Value));
			}
		}

		/// <summary>
		/// Gets the width and height, in pixels, of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public Size Size
		{
			get
			{
				EnsureNotDisposed();
				return new Size(Width, Height);
			}
		}

		/// <summary>
		/// Gets or sets an object that provides additional data about the <see cref="FreeImageBitmap"/>.
		/// </summary>
		public Object Tag
		{
			get
			{
				EnsureNotDisposed();
				return tag;
			}
			set
			{
				EnsureNotDisposed();
				tag = value;
			}
		}

		/// <summary>
		/// Gets whether this <see cref="FreeImageBitmap"/> has been disposed.
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				return disposed;
			}
		}

		/// <summary>
		/// Gets a new instance of a metadata representing class.
		/// </summary>
		public ImageMetadata Metadata
		{
			get
			{
				EnsureNotDisposed();
				return new ImageMetadata(dib, true);
			}
		}

		/// <summary>
		/// Gets or sets the comment of this <see cref="FreeImageBitmap"/>.
		/// Supported formats are JPEG, PNG and GIF.
		/// </summary>
		public string Comment
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetImageComment(dib);
			}
			set
			{
				EnsureNotDisposed();
				FreeImage.SetImageComment(dib, value);
			}
		}

		/// <summary>
		/// Returns whether this <see cref="FreeImageBitmap"/> has a palette.
		/// </summary>
		public bool HasPalette
		{
			get
			{
				EnsureNotDisposed();
				return (FreeImage.GetPalette(dib) != IntPtr.Zero);
			}
		}

		/// <summary>
		/// Gets or sets the entry used as transparent color in this <see cref="FreeImageBitmap"/>.
		/// Only works for 1-, 4- and 8-bpp.
		/// </summary>
		public int TransparentIndex
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetTransparentIndex(dib);
			}
			set
			{
				EnsureNotDisposed();
				FreeImage.SetTransparentIndex(dib, value);
			}
		}

		/// <summary>
		/// Gets the number of frames in this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public int FrameCount
		{
			get
			{
				EnsureNotDisposed();
				int result = 1;
				if (!mdib.IsNull)
				{
					result = FreeImage.GetPageCount(mdib);
				}
				return result;
			}
		}

		/// <summary>
		/// Gets the ICCProfile structure of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public FIICCPROFILE ICCProfile
		{
			get
			{
				EnsureNotDisposed();
				return FreeImage.GetICCProfileEx(dib);
			}
		}

		/// <summary>
		/// Gets the format of the original image in case
		/// this <see cref="FreeImageBitmap"/> was loaded from a file or stream.
		/// </summary>
		public FREE_IMAGE_FORMAT ImageFormat
		{
			get
			{
				EnsureNotDisposed();
				return originalFormat;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the bounds of this <see cref="FreeImageBitmap"/> in the specified unit.
		/// </summary>
		/// <param name="pageUnit">One of the <see cref="System.Drawing.GraphicsUnit"/> values indicating
		/// the unit of measure for the bounding rectangle.</param>
		/// <returns>The <see cref="System.Drawing.RectangleF"/> that represents the bounds of this
		/// <see cref="FreeImageBitmap"/>, in the specified unit.</returns>
		public RectangleF GetBounds(ref GraphicsUnit pageUnit)
		{
			EnsureNotDisposed();
			pageUnit = GraphicsUnit.Pixel;
			return new RectangleF(
					0f,
					0f,
					(float)FreeImage.GetWidth(dib),
					(float)FreeImage.GetHeight(dib));
		}

		/// <summary>
		/// Gets the specified property item from this <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <param name="propid">The ID of the property item to get.</param>
		/// <returns>The <see cref="PropertyItem"/> this method gets.</returns>
		public PropertyItem GetPropertyItem(int propid)
		{
			EnsureNotDisposed();
			ImageMetadata metadata = new ImageMetadata(dib, true);
			foreach (MetadataModel metadataModel in metadata)
			{
				foreach (MetadataTag tag in metadataModel)
				{
					if (tag.ID == propid)
					{
						return tag.GetPropertyItem();
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Returns a thumbnail for this <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <param name="thumbWidth">The width, in pixels, of the requested thumbnail image.</param>
		/// <param name="thumbHeight">The height, in pixels, of the requested thumbnail image.</param>
		/// <param name="callback">Ignored.</param>
		/// <param name="callBackData">Ignored.</param>
		/// <returns>A <see cref="FreeImageBitmap"/> that represents the thumbnail.</returns>
		public FreeImageBitmap GetThumbnailImage(int thumbWidth, int thumbHeight,
			Image.GetThumbnailImageAbort callback, IntPtr callBackData)
		{
			EnsureNotDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.Rescale(
				dib, thumbWidth, thumbHeight, FREE_IMAGE_FILTER.FILTER_BICUBIC);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap(newDib);
			}
			return result;
		}

		/// <summary>
		/// Returns a thumbnail for this <see cref="FreeImageBitmap"/>, keeping aspect ratio.
		/// <paramref name="maxPixelSize"/> defines the maximum width or height
		/// of the thumbnail.
		/// </summary>
		/// <param name="maxPixelSize">Thumbnail square size.</param>
		/// <param name="convert">When true HDR images are transperantly
		/// converted to standard images.</param>
		/// <returns>The thumbnail in a new instance.</returns>
		public FreeImageBitmap GetThumbnailImage(int maxPixelSize, bool convert)
		{
			EnsureNotDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.MakeThumbnail(dib, maxPixelSize, convert);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap(newDib);
			}
			return result;
		}

		/// <summary>
		/// Converts this <see cref="FreeImageBitmap"/> instance to a <see cref="Bitmap"/> instance.
		/// </summary>
		/// <returns>A new instance of <see cref="Bitmap"/> initialized this instance.</returns>
		public Bitmap ToBitmap()
		{
			EnsureNotDisposed();
			return FreeImage.GetBitmap(dib, true);
		}

		/// <summary>
		/// Returns an instance of <see cref="Scanline&lt;T&gt;"/>, representing the scanline
		/// specified by <paramref name="scanline"/> of this <see cref="FreeImageBitmap"/>.
		/// Since FreeImage bitmaps are always bottum up aligned, keep in mind that scanline 0 is the
		/// bottom-most line of the image.
		/// </summary>
		/// <param name="scanline">Number of the scanline to retrieve.</param>
		/// <returns>An instance of <see cref="Scanline&lt;T&gt;"/> representing the
		/// <paramref name="scanline"/>th scanline.</returns>
		/// <remarks>
		/// List of return-types of <b>T</b>:<para/>
		/// <list type="table">
		/// <listheader><term>Color Depth / Type</term><description><see cref="Type">Result Type</see></description></listheader>
		/// <item><term>1 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="FI1BIT"/></description></item>
		/// <item><term>4 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="FI4BIT"/></description></item>
		/// <item><term>8 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="Byte"/></description></item>
		/// <item><term>16 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="UInt16"/></description></item>
		/// <item><term>16 - 555 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="FI16RGB555"/></description></item>
		/// <item><term>16 - 565 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="FI16RGB565"/></description></item>
		/// <item><term>24 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="RGBTRIPLE"/></description></item>
		/// <item><term>32 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="RGBQUAD"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_COMPLEX"/></term><description><see cref="FICOMPLEX"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_DOUBLE"/></term><description><see cref="Double"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_FLOAT"/></term><description><see cref="Single"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_INT16"/></term><description><see cref="Int16"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_INT32"/></term><description><see cref="Int32"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_RGB16"/></term><description><see cref="FIRGB16"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_RGBA16"/></term><description><see cref="FIRGBA16"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_RGBAF"/></term><description><see cref="FIRGBAF"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_RGBF"/></term><description><see cref="FIRGBF"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_UINT16"/></term><description><see cref="UInt16"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_UINT32"/></term><description><see cref="UInt32"/></description></item>
		/// </list>
		/// </remarks>
		/// <example>
		/// <code>
		/// FreeImageBitmap bitmap = new FreeImageBitmap(@"C:\Pictures\picture.bmp");
		/// if (bitmap.ColorDepth == 32)
		/// {
		/// 	Scanline&lt;RGBQUAD&gt; scanline = (Scanline&lt;RGBQUAD&gt;)bitmap.GetScanline(0);
		/// 	foreach (RGBQUAD pixel in scanline)
		/// 	{
		///			Console.WriteLine(pixel);
		/// 	}
		///	}
		/// </code>
		/// </example>
		/// <exception cref="ArgumentException">
		/// The bitmap's type or color depth are not supported.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="scanline"/> is no valid value.
		/// </exception>
		public Scanline<T> GetScanline<T>(int scanline) where T : struct
		{
			EnsureNotDisposed();
			return new Scanline<T>(dib, scanline);
		}

		/// <summary>
		/// Returns an instance of <see cref="Scanline&lt;T&gt;"/>, representing the scanline
		/// specified by <paramref name="scanline"/> of this <see cref="FreeImageBitmap"/>.
		/// Since FreeImage bitmaps are always bottum up aligned, keep in mind that scanline 0 is the
		/// bottom-most line of the image.
		/// </summary>
		/// <param name="scanline">Number of the scanline to retrieve.</param>
		/// <returns>An instance of <see cref="Scanline&lt;T&gt;"/> representing the
		/// <paramref name="scanline"/>th scanline.</returns>
		/// <remarks>
		/// List of return-types of <b>T</b>:<para/>
		/// <list type="table">
		/// <listheader><term>Color Depth / Type</term><description><see cref="Type">Result Type</see></description></listheader>
		/// <item><term>1 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="FI1BIT"/></description></item>
		/// <item><term>4 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="FI4BIT"/></description></item>
		/// <item><term>8 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="Byte"/></description></item>
		/// <item><term>16 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="UInt16"/></description></item>
		/// <item><term>16 - 555 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="FI16RGB555"/></description></item>
		/// <item><term>16 - 565 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="FI16RGB565"/></description></item>
		/// <item><term>24 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="RGBTRIPLE"/></description></item>
		/// <item><term>32 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="RGBQUAD"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_COMPLEX"/></term><description><see cref="FICOMPLEX"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_DOUBLE"/></term><description><see cref="Double"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_FLOAT"/></term><description><see cref="Single"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_INT16"/></term><description><see cref="Int16"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_INT32"/></term><description><see cref="Int32"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_RGB16"/></term><description><see cref="FIRGB16"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_RGBA16"/></term><description><see cref="FIRGBA16"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_RGBAF"/></term><description><see cref="FIRGBAF"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_RGBF"/></term><description><see cref="FIRGBF"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_UINT16"/></term><description><see cref="UInt16"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_UINT32"/></term><description><see cref="UInt32"/></description></item>
		/// </list>
		/// </remarks>
		/// <example>
		/// <code>
		/// FreeImageBitmap bitmap = new FreeImageBitmap(@"C:\Pictures\picture.bmp");
		/// if (bitmap.ColorDepth == 32)
		/// {
		/// 	Scanline&lt;RGBQUAD&gt; scanline = (Scanline&lt;RGBQUAD&gt;)bitmap.GetScanline(0);
		/// 	foreach (RGBQUAD pixel in scanline)
		/// 	{
		///			Console.WriteLine(pixel);
		/// 	}
		///	}
		/// </code>
		/// </example>
		/// <exception cref="ArgumentException">
		/// The type of the bitmap or color depth are not supported.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="scanline"/> is no valid value.
		/// </exception>
		public object GetScanline(int scanline)
		{
			EnsureNotDisposed();
			object result = null;
			int width = (int)FreeImage.GetWidth(dib);

			switch (FreeImage.GetImageType(dib))
			{
				case FREE_IMAGE_TYPE.FIT_BITMAP:

					switch (FreeImage.GetBPP(dib))
					{
						case 1u: result = new Scanline<FI1BIT>(dib, scanline, width); break;
						case 4u: result = new Scanline<FI4BIT>(dib, scanline, width); break;
						case 8u: result = new Scanline<Byte>(dib, scanline, width); break;
						case 16u:
							if ((RedMask == FreeImage.FI16_555_RED_MASK) &&
								(GreenMask == FreeImage.FI16_555_GREEN_MASK) &&
								(BlueMask == FreeImage.FI16_555_BLUE_MASK))
							{
								result = new Scanline<FI16RGB555>(dib, scanline, width);
							}
							else if ((RedMask == FreeImage.FI16_565_RED_MASK) &&
								(GreenMask == FreeImage.FI16_565_GREEN_MASK) &&
								(BlueMask == FreeImage.FI16_565_BLUE_MASK))
							{
								result = new Scanline<FI16RGB565>(dib, scanline, width);
							}
							else
							{
								result = new Scanline<UInt16>(dib, scanline, width);
							}
							break;
						case 24u: result = new Scanline<RGBTRIPLE>(dib, scanline, width); break;
						case 32u: result = new Scanline<RGBQUAD>(dib, scanline, width); break;
						default: throw new ArgumentException("Color depth is not supported.");
					}
					break;

				case FREE_IMAGE_TYPE.FIT_COMPLEX: result = new Scanline<FICOMPLEX>(dib, scanline, width); break;
				case FREE_IMAGE_TYPE.FIT_DOUBLE: result = new Scanline<Double>(dib, scanline, width); break;
				case FREE_IMAGE_TYPE.FIT_FLOAT: result = new Scanline<Single>(dib, scanline, width); break;
				case FREE_IMAGE_TYPE.FIT_INT16: result = new Scanline<Int16>(dib, scanline, width); break;
				case FREE_IMAGE_TYPE.FIT_INT32: result = new Scanline<Int32>(dib, scanline, width); break;
				case FREE_IMAGE_TYPE.FIT_RGB16: result = new Scanline<FIRGB16>(dib, scanline, width); break;
				case FREE_IMAGE_TYPE.FIT_RGBA16: result = new Scanline<FIRGBA16>(dib, scanline, width); break;
				case FREE_IMAGE_TYPE.FIT_RGBAF: result = new Scanline<FIRGBAF>(dib, scanline, width); break;
				case FREE_IMAGE_TYPE.FIT_RGBF: result = new Scanline<FIRGBF>(dib, scanline, width); break;
				case FREE_IMAGE_TYPE.FIT_UINT16: result = new Scanline<UInt16>(dib, scanline, width); break;
				case FREE_IMAGE_TYPE.FIT_UINT32: result = new Scanline<UInt32>(dib, scanline, width); break;
				case FREE_IMAGE_TYPE.FIT_UNKNOWN:
				default: throw new ArgumentException("Type is not supported.");
			}

			return result;
		}

		/// <summary>
		/// Returns a pointer to the specified scanline.
		/// Due to FreeImage bitmaps are bottum up,
		/// scanline 0 is the most bottom line of the image.
		/// </summary>
		/// <param name="scanline">Number of the scanline.</param>
		/// <returns>Pointer to the scanline.</returns>
		public IntPtr GetScanlinePointer(int scanline)
		{
			EnsureNotDisposed();
			return FreeImage.GetScanLine(dib, scanline);
		}

		/// <summary>
		/// Returns a list of structures, representing the scanlines of this <see cref="FreeImageBitmap"/>.
		/// Due to FreeImage bitmaps are bottum up, scanline 0 is the
		/// bottom-most line of the image.
		/// Each color depth has a different representing structure due to different memory layouts.
		/// </summary>
		/// <remarks>
		/// List of return-types of <b>T</b>:<para/>
		/// <list type="table">
		/// <listheader><term>Color Depth / Type</term><description><see cref="Type">Result Type of IEnmuerable&lt;Scanline&lt;T&gt;&gt;</see></description></listheader>
		/// <item><term>1 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="FI1BIT"/></description></item>
		/// <item><term>4 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="FI4BIT"/></description></item>
		/// <item><term>8 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="Byte"/></description></item>
		/// <item><term>16 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="UInt16"/></description></item>
		/// <item><term>16 - 555 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="FI16RGB555"/></description></item>
		/// <item><term>16 - 565 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="FI16RGB565"/></description></item>
		/// <item><term>24 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="RGBTRIPLE"/></description></item>
		/// <item><term>32 (<see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>)</term><description><see cref="RGBQUAD"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_COMPLEX"/></term><description><see cref="FICOMPLEX"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_DOUBLE"/></term><description><see cref="Double"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_FLOAT"/></term><description><see cref="Single"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_INT16"/></term><description><see cref="Int16"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_INT32"/></term><description><see cref="Int32"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_RGB16"/></term><description><see cref="FIRGB16"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_RGBA16"/></term><description><see cref="FIRGBA16"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_RGBAF"/></term><description><see cref="FIRGBAF"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_RGBF"/></term><description><see cref="FIRGBF"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_UINT16"/></term><description><see cref="UInt16"/></description></item>
		/// <item><term><see cref="FREE_IMAGE_TYPE.FIT_UINT32"/></term><description><see cref="UInt32"/></description></item>
		/// </list>
		/// </remarks>
		public IList GetScanlines()
		{
			EnsureNotDisposed();

			int height = (int)FreeImage.GetHeight(dib);
			IList list;

			switch (FreeImage.GetImageType(dib))
			{
				case FREE_IMAGE_TYPE.FIT_BITMAP:

					switch (FreeImage.GetBPP(dib))
					{
						case 1u: list = new List<Scanline<FI1BIT>>(height); break;
						case 4u: list = new List<Scanline<FI4BIT>>(height); break;
						case 8u: list = new List<Scanline<Byte>>(height); break;
						case 16u:
							if (FreeImage.IsRGB555(dib))
							{
								list = new List<Scanline<FI16RGB555>>(height);
							}
							else if (FreeImage.IsRGB565(dib))
							{
								list = new List<Scanline<FI16RGB565>>(height);
							}
							else
							{
								list = new List<Scanline<UInt16>>(height);
							}
							break;
						case 24u: list = new List<Scanline<RGBTRIPLE>>(height); break;
						case 32u: list = new List<Scanline<RGBQUAD>>(height); break;
						default: throw new ArgumentException("Color depth is not supported.");
					}
					break;

				case FREE_IMAGE_TYPE.FIT_COMPLEX: list = new List<Scanline<FICOMPLEX>>(height); break;
				case FREE_IMAGE_TYPE.FIT_DOUBLE: list = new List<Scanline<Double>>(height); break;
				case FREE_IMAGE_TYPE.FIT_FLOAT: list = new List<Scanline<Single>>(height); break;
				case FREE_IMAGE_TYPE.FIT_INT16: list = new List<Scanline<Int16>>(height); break;
				case FREE_IMAGE_TYPE.FIT_INT32: list = new List<Scanline<Int32>>(height); break;
				case FREE_IMAGE_TYPE.FIT_RGB16: list = new List<Scanline<FIRGB16>>(height); break;
				case FREE_IMAGE_TYPE.FIT_RGBA16: list = new List<Scanline<FIRGBA16>>(height); break;
				case FREE_IMAGE_TYPE.FIT_RGBAF: list = new List<Scanline<FIRGBAF>>(height); break;
				case FREE_IMAGE_TYPE.FIT_RGBF: list = new List<Scanline<FIRGBF>>(height); break;
				case FREE_IMAGE_TYPE.FIT_UINT16: list = new List<Scanline<UInt16>>(height); break;
				case FREE_IMAGE_TYPE.FIT_UINT32: list = new List<Scanline<UInt32>>(height); break;
				case FREE_IMAGE_TYPE.FIT_UNKNOWN:
				default: throw new ArgumentException("Type is not supported.");
			}

			for (int i = 0; i < height; i++)
			{
				list.Add(GetScanline(i));
			}

			return list;
		}

		/// <summary>
		/// Removes the specified property item from this <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <param name="propid">The ID of the property item to remove.</param>
		public void RemovePropertyItem(int propid)
		{
			EnsureNotDisposed();
			ImageMetadata mdata = new ImageMetadata(dib, true);
			foreach (MetadataModel model in mdata)
			{
				foreach (MetadataTag tag in model)
				{
					if (tag.ID == propid)
					{
						model.RemoveTag(tag.Key);
						return;
					}
				}
			}
		}

		/// <summary>
		/// This method rotates, flips, or rotates and flips this <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <param name="rotateFlipType">A RotateFlipType member
		/// that specifies the type of rotation and flip to apply to this <see cref="FreeImageBitmap"/>.</param>
		public void RotateFlip(RotateFlipType rotateFlipType)
		{
			EnsureNotDisposed();

			FIBITMAP newDib = new FIBITMAP();
			uint bpp = FreeImage.GetBPP(dib);

			switch (rotateFlipType)
			{
				case RotateFlipType.RotateNoneFlipX:

					FreeImage.FlipHorizontal(dib);
					break;

				case RotateFlipType.RotateNoneFlipY:

					FreeImage.FlipVertical(dib);
					break;

				case RotateFlipType.RotateNoneFlipXY:

					FreeImage.FlipHorizontal(dib);
					FreeImage.FlipVertical(dib);
					break;

				case RotateFlipType.Rotate90FlipNone:

					newDib = (bpp == 4u) ? FreeImage.Rotate4bit(dib, 90d) : FreeImage.RotateClassic(dib, 90d);
					break;

				case RotateFlipType.Rotate90FlipX:

					newDib = (bpp == 4u) ? FreeImage.Rotate4bit(dib, 90d) : FreeImage.RotateClassic(dib, 90d);
					FreeImage.FlipHorizontal(newDib);
					break;

				case RotateFlipType.Rotate90FlipY:

					newDib = (bpp == 4u) ? FreeImage.Rotate4bit(dib, 90d) : FreeImage.RotateClassic(dib, 90d);
					FreeImage.FlipVertical(newDib);
					break;

				case RotateFlipType.Rotate90FlipXY:

					newDib = (bpp == 4u) ? FreeImage.Rotate4bit(dib, 90d) : FreeImage.RotateClassic(dib, 90d);
					FreeImage.FlipHorizontal(newDib);
					FreeImage.FlipVertical(newDib);
					break;

				case RotateFlipType.Rotate180FlipXY:
					newDib = FreeImage.Clone(dib);
					break;
			}
			ReplaceDib(newDib);
		}

		/// <summary>
		/// Copies the metadata from another <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <param name="bitmap">The bitmap to read the metadata from.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="bitmap"/> is null.
		/// </exception>
		public void CloneMetadataFrom(FreeImageBitmap bitmap)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			EnsureNotDisposed();
			bitmap.EnsureNotDisposed();
			FreeImage.CloneMetadata(dib, bitmap.dib);
		}

		/// <summary>
		/// Copies the metadata from another <see cref="FreeImageBitmap"/> using
		/// the provided options.
		/// </summary>
		/// <param name="bitmap">The bitmap to read the metadata from.</param>
		/// <param name="flags">Specifies the way the metadata is copied.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="bitmap"/> is null.
		/// </exception>
		public void CloneMetadataFrom(FreeImageBitmap bitmap, FREE_IMAGE_METADATA_COPY flags)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			EnsureNotDisposed();
			bitmap.EnsureNotDisposed();
			FreeImage.CloneMetadataEx(bitmap.dib, dib, flags);
		}

		/// <summary>
		/// Saves this <see cref="FreeImageBitmap"/> to the specified file.
		/// </summary>
		/// <param name="filename">A string that contains the name of the file to which
		/// to save this <see cref="FreeImageBitmap"/>.</param>
		/// <exception cref="ArgumentException"><paramref name="filename"/> is null or empty.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(string filename)
		{
			Save(filename, FREE_IMAGE_FORMAT.FIF_UNKNOWN, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Saves this <see cref="FreeImageBitmap"/> to the specified file in the specified format.
		/// </summary>
		/// <param name="filename">A string that contains the name of the file to which
		/// to save this <see cref="FreeImageBitmap"/>.</param>
		/// <param name="format">An <see cref="FREE_IMAGE_FORMAT"/> that specifies the format of the saved image.</param>
		/// <exception cref="ArgumentException"><paramref name="filename"/> is null or empty.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(string filename, FREE_IMAGE_FORMAT format)
		{
			Save(filename, format, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Saves this <see cref="FreeImageBitmap"/> to the specified file in the specified format
		/// using the specified saving flags.
		/// </summary>
		/// <param name="filename">A string that contains the name of the file to which
		/// to save this <see cref="FreeImageBitmap"/>.</param>
		/// <param name="format">An <see cref="FREE_IMAGE_FORMAT"/> that specifies the format of the saved image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="ArgumentException"><paramref name="filename"/> is null or empty.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(string filename, FREE_IMAGE_FORMAT format, FREE_IMAGE_SAVE_FLAGS flags)
		{
			EnsureNotDisposed();
			if (string.IsNullOrEmpty(filename))
			{
				throw new ArgumentException("filename");
			}
			if (!FreeImage.SaveEx(dib, filename, format, flags))
			{
				throw new Exception();
			}

			saveInformation.filename = filename;
			saveInformation.format = format;
			saveInformation.saveFlags = flags;
		}

		/// <summary>
		/// Saves this <see cref="FreeImageBitmap"/> to the specified stream in the specified format.
		/// </summary>
		/// <param name="stream">The stream where this <see cref="FreeImageBitmap"/> will be saved.</param>
		/// <param name="format">An <see cref="FREE_IMAGE_FORMAT"/> that specifies the format of the saved image.</param>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(Stream stream, FREE_IMAGE_FORMAT format)
		{
			Save(stream, format, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Saves this <see cref="FreeImageBitmap"/> to the specified stream in the specified format
		/// using the specified saving flags.
		/// </summary>
		/// <param name="stream">The stream where this <see cref="FreeImageBitmap"/> will be saved.</param>
		/// <param name="format">An <see cref="FREE_IMAGE_FORMAT"/> that specifies the format of the saved image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(Stream stream, FREE_IMAGE_FORMAT format, FREE_IMAGE_SAVE_FLAGS flags)
		{
			EnsureNotDisposed();
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!FreeImage.SaveToStream(dib, stream, format, flags))
			{
				throw new Exception();
			}

			saveInformation.filename = null;
		}

		/// <summary>
		/// Adds a frame to the file specified in a previous call to the <see cref="Save(String)"/> method.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// This instance has not been saved to a file using Save(...) before.</exception>
		public void SaveAdd()
		{
			SaveAdd(this);
		}

		/// <summary>
		/// Adds a frame to the file specified in a previous call to the <see cref="Save(String)"/> method.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <param name="bitmap">A <see cref="FreeImageBitmap"/> that contains the frame to add.</param>
		/// <exception cref="InvalidOperationException">
		/// This instance has not been saved to a file using Save(...) before.</exception>
		public void SaveAdd(FreeImageBitmap bitmap)
		{
			if (saveInformation.filename == null)
			{
				throw new InvalidOperationException();
			}

			SaveAdd(
				saveInformation.filename,
				bitmap,
				saveInformation.format,
				saveInformation.loadFlags,
				saveInformation.saveFlags);
		}

		/// <summary>
		/// Adds a frame to the file specified.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <param name="filename">File to add this frame to.</param>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void SaveAdd(string filename)
		{
			SaveAdd(
				filename,
				this,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				FREE_IMAGE_LOAD_FLAGS.DEFAULT,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Adds a frame to the file specified using the specified parameters.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <param name="filename">File to add this frame to.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="loadFlags">Flags to enable or disable plugin-features.</param>
		/// <param name="saveFlags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void SaveAdd(
			string filename,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_LOAD_FLAGS loadFlags,
			FREE_IMAGE_SAVE_FLAGS saveFlags)
		{
			SaveAdd(
				filename,
				this,
				format,
				loadFlags,
				saveFlags);
		}

		/// <summary>
		/// Selects the frame specified by the index.
		/// </summary>
		/// <param name="frameIndex">The index of the active frame.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="frameIndex"/> is out of range.</exception>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="InvalidOperationException">The loaded bitmap is not multipaged.</exception>
		public void SelectActiveFrame(int frameIndex)
		{
			EnsureNotDisposed();
			if (frameIndex < 0)
			{
				throw new ArgumentOutOfRangeException("frameIndex");
			}
			if (mdib.IsNull)
			{
				throw new InvalidOperationException("No multipaged bitmap loaded.");
			}
			if (frameIndex >= FrameCount)
			{
				throw new ArgumentOutOfRangeException("frameIndex");
			}

			if (FreeImage.GetLockedPages(mdib)[0] != frameIndex)
			{
				long size = DataSize;
				FreeImage.UnlockPage(mdib, dib, false);
				GC.RemoveMemoryPressure(size);

				dib = FreeImage.LockPage(mdib, frameIndex);
				if (dib.IsNull)
				{
					throw new Exception();
				}
				AddMemoryPressure();
			}
		}

		/// <summary>
		/// Creates a GDI bitmap object from this <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <returns>A handle to the GDI bitmap object that this method creates.</returns>
		public IntPtr GetHbitmap()
		{
			EnsureNotDisposed();
			return FreeImage.GetHbitmap(dib, IntPtr.Zero, false);
		}

		/// <summary>
		/// Creates a GDI bitmap object from this <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <param name="background">A <see cref="System.Drawing.Color"/> structure that specifies the background color.
		/// This parameter is ignored if the bitmap is totally opaque.</param>
		/// <returns>A handle to the GDI bitmap object that this method creates.</returns>
		public IntPtr GetHbitmap(Color background)
		{
			EnsureNotDisposed();
			using (FreeImageBitmap temp = new FreeImageBitmap(this))
			{
				temp.BackgroundColor = background;
				return temp.GetHbitmap();
			}
		}

		/// <summary>
		/// Returns the handle to an icon.
		/// </summary>
		/// <returns>A Windows handle to an icon with the same image as this <see cref="FreeImageBitmap"/>.</returns>
		public IntPtr GetHicon()
		{
			EnsureNotDisposed();
			using (Bitmap bitmap = FreeImage.GetBitmap(dib, true))
			{
				return bitmap.GetHicon();
			}
		}

		/// <summary>
		/// Creates a GDI bitmap object from this <see cref="FreeImageBitmap"/> with the same
		/// color depth as the primary device.
		/// </summary>
		/// <returns>A handle to the GDI bitmap object that this method creates.</returns>
		public IntPtr GetHbitmapForDevice()
		{
			EnsureNotDisposed();
			return FreeImage.GetBitmapForDevice(dib, IntPtr.Zero, false);
		}

		/// <summary>
		/// Gets the <see cref="Color"/> of the specified pixel in this <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <param name="x">The x-coordinate of the pixel to retrieve.</param>
		/// <param name="y">The y-coordinate of the pixel to retrieve.</param>
		/// <returns>A <see cref="System.Drawing.Color"/> structure that represents the color of the specified pixel.</returns>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="NotSupportedException">The type of this bitmap is not supported.</exception>
		public unsafe Color GetPixel(int x, int y)
		{
			EnsureNotDisposed();
			if (FreeImage.GetImageType(dib) == FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				if (ColorDepth == 16 || ColorDepth == 24 || ColorDepth == 32)
				{
					RGBQUAD rgbq;
					if (!FreeImage.GetPixelColor(dib, (uint)x, (uint)y, out rgbq))
					{
						throw new Exception();
					}
					return rgbq.Color;
				}
				else if (ColorDepth == 1 || ColorDepth == 4 || ColorDepth == 8)
				{
					byte index;
					if (!FreeImage.GetPixelIndex(dib, (uint)x, (uint)y, out index))
					{
						throw new Exception();
					}
					RGBQUAD* palette = (RGBQUAD*)FreeImage.GetPalette(dib);
					return palette[index].Color;
				}
			}
			throw new NotSupportedException();
		}

		/// <summary>
		/// Makes the default transparent color transparent for this <see cref="FreeImageBitmap"/>.
		/// </summary>
		public void MakeTransparent()
		{
			EnsureNotDisposed();
			MakeTransparent(Color.Transparent);
		}

		/// <summary>
		/// Makes the specified color transparent for this <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <param name="transparentColor">The <see cref="System.Drawing.Color"/> structure that represents
		/// the color to make transparent.</param>
		/// <exception cref="NotImplementedException">
		/// This method is not implemented.</exception>
		public void MakeTransparent(Color transparentColor)
		{
			EnsureNotDisposed();
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Sets the <see cref="System.Drawing.Color"/> of the specified pixel in this <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <param name="x">The x-coordinate of the pixel to set.</param>
		/// <param name="y">The y-coordinate of the pixel to set.</param>
		/// <param name="color">A <see cref="System.Drawing.Color"/> structure that represents the color
		/// to assign to the specified pixel.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="NotSupportedException">The type of this bitmap is not supported.</exception>
		public unsafe void SetPixel(int x, int y, Color color)
		{
			EnsureNotDisposed();
			if (FreeImage.GetImageType(dib) == FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				if (ColorDepth == 16 || ColorDepth == 24 || ColorDepth == 32)
				{
					RGBQUAD rgbq = color;
					if (!FreeImage.SetPixelColor(dib, (uint)x, (uint)y, ref rgbq))
					{
						throw new Exception();
					}
					return;
				}
				else if (ColorDepth == 1 || ColorDepth == 4 || ColorDepth == 8)
				{
					uint colorsUsed = FreeImage.GetColorsUsed(dib);
					RGBQUAD* palette = (RGBQUAD*)FreeImage.GetPalette(dib);
					for (int i = 0; i < colorsUsed; i++)
					{
						if (palette[i].Color == color)
						{
							byte index = (byte)i;
							if (!FreeImage.SetPixelIndex(dib, (uint)x, (uint)y, ref index))
							{
								throw new Exception();
							}
							return;
						}
					}
					throw new ArgumentOutOfRangeException("color");
				}
			}
			throw new NotSupportedException();
		}

		/// <summary>
		/// Sets the resolution for this <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <param name="xDpi">The horizontal resolution, in dots per inch, of this <see cref="FreeImageBitmap"/>.</param>
		/// <param name="yDpi">The vertical resolution, in dots per inch, of this <see cref="FreeImageBitmap"/>.</param>
		public void SetResolution(float xDpi, float yDpi)
		{
			EnsureNotDisposed();
			FreeImage.SetResolutionX(dib, (uint)xDpi);
			FreeImage.SetResolutionY(dib, (uint)yDpi);
		}

		/// <summary>
		/// This function is not yet implemented.
		/// </summary>
		/// <exception cref="NotImplementedException">
		/// This method is not implemented.</exception>
		public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// This function is not yet implemented.
		/// </summary>
		/// <exception cref="NotImplementedException">
		/// This method is not implemented.</exception>
		public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format, BitmapData bitmapData)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// This function is not yet implemented.
		/// </summary>
		/// <exception cref="NotImplementedException">
		/// This method is not implemented.</exception>
		public void UnlockBits(BitmapData bitmapdata)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Converts this <see cref="FreeImageBitmap"/> into a different color depth.
		/// The parameter <paramref name="bpp"/> specifies color depth, greyscale conversion
		/// and palette reorder.
		/// <para>Adding the <see cref="FREE_IMAGE_COLOR_DEPTH.FICD_FORCE_GREYSCALE"/> flag
		/// will first perform a convesion to greyscale. This can be done with any target
		/// color depth.</para>
		/// <para>Adding the <see cref="FREE_IMAGE_COLOR_DEPTH.FICD_REORDER_PALETTE"/> flag
		/// will allow the algorithm to reorder the palette. This operation will not be performed to
		/// non-greyscale images to prevent data lost by mistake.</para>
		/// </summary>
		/// <param name="bpp">A bitfield containing information about the conversion
		/// to perform.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH bpp)
		{
			EnsureNotDisposed();
			return ReplaceDib(FreeImage.ConvertColorDepth(dib, bpp, false));
		}

		/// <summary>
		/// Converts this <see cref="FreeImageBitmap"/> <see cref="FREE_IMAGE_TYPE"/> to
		/// <paramref name="type"/> initializing a new instance.
		/// In case source and destination type are the same, the operation fails.
		/// An error message can be catched using the 'Message' event.
		/// </summary>
		/// <param name="type">Destination type.</param>
		/// <param name="scaleLinear">True to scale linear, else false.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool ConvertType(FREE_IMAGE_TYPE type, bool scaleLinear)
		{
			EnsureNotDisposed();
			return (ImageType == type) ? false : ReplaceDib(FreeImage.ConvertToType(dib, type, scaleLinear));
		}

		/// <summary>
		/// Converts this <see cref="FreeImageBitmap"/> <see cref="FreeImageBitmap"/> to <paramref name="type"/>.
		/// In case source and destination type are the same, the operation fails.
		/// An error message can be catched using the 'Message' event.
		/// </summary>
		/// <param name="type">Destination type.</param>
		/// <param name="scaleLinear">True to scale linear, else false.</param>
		/// <returns>The converted instance.</returns>
		public FreeImageBitmap GetTypeConvertedInstance(FREE_IMAGE_TYPE type, bool scaleLinear)
		{
			EnsureNotDisposed();
			FreeImageBitmap result = null;
			if (ImageType != type)
			{
				FIBITMAP newDib = FreeImage.ConvertToType(dib, type, scaleLinear);
				if (!newDib.IsNull)
				{
					result = new FreeImageBitmap(newDib);
				}
			}
			return result;
		}

		/// <summary>
		/// Converts this <see cref="FreeImageBitmap"/> into a different color depth initializing
		/// a new instance.
		/// The parameter <paramref name="bpp"/> specifies color depth, greyscale conversion
		/// and palette reorder.
		/// <para>Adding the <see cref="FREE_IMAGE_COLOR_DEPTH.FICD_FORCE_GREYSCALE"/> flag will
		/// first perform a convesion to greyscale. This can be done with any target color depth.</para>
		/// <para>Adding the <see cref="FREE_IMAGE_COLOR_DEPTH.FICD_REORDER_PALETTE"/> flag will
		/// allow the algorithm to reorder the palette. This operation will not be performed to
		/// non-greyscale images to prevent data lost by mistake.</para>
		/// </summary>
		/// <param name="bpp">A bitfield containing information about the conversion
		/// to perform.</param>
		/// <returns>The converted instance.</returns>
		public FreeImageBitmap GetColorConvertedInstance(FREE_IMAGE_COLOR_DEPTH bpp)
		{
			EnsureNotDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.ConvertColorDepth(dib, bpp, false);
			if (newDib == dib)
			{
				newDib = FreeImage.Clone(dib);
			}
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap(newDib);
			}
			return result;
		}

		/// <summary>
		/// Rescales this <see cref="FreeImageBitmap"/> to the specified size using the
		/// specified filter.
		/// </summary>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="filter">Filter to use for resizing.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Rescale(Size newSize, FREE_IMAGE_FILTER filter)
		{
			return Rescale(newSize.Width, newSize.Height, filter);
		}

		/// <summary>
		/// Rescales this <see cref="FreeImageBitmap"/> to the specified size using the
		/// specified filter.
		/// </summary>
		/// <param name="width">Width of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="height">Height of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="filter">Filter to use for resizing.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Rescale(int width, int height, FREE_IMAGE_FILTER filter)
		{
			EnsureNotDisposed();
			return ReplaceDib(FreeImage.Rescale(dib, width, height, filter));
		}

		/// <summary>
		/// Rescales this <see cref="FreeImageBitmap"/> to the specified size using the
		/// specified filter initializing a new instance.
		/// </summary>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="filter">Filter to use for resizing.</param>
		/// <returns>The rescaled instance.</returns>
		public FreeImageBitmap GetScaledInstance(Size newSize, FREE_IMAGE_FILTER filter)
		{
			return GetScaledInstance(newSize.Width, newSize.Height, filter);
		}

		/// <summary>
		/// Rescales this <see cref="FreeImageBitmap"/> to the specified size using the
		/// specified filter initializing a new instance.
		/// </summary>
		/// <param name="width">Width of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="height">Height of the new <see cref="FreeImageBitmap"/>.</param>
		/// <param name="filter">Filter to use for resizing.</param>
		/// <returns>The rescaled instance.</returns>
		public FreeImageBitmap GetScaledInstance(int width, int height, FREE_IMAGE_FILTER filter)
		{
			EnsureNotDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.Rescale(dib, width, height, filter);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap(newDib);
			}
			return result;
		}

		/// <summary>
		/// Quantizes this <see cref="FreeImageBitmap"/> from 24 bit to 8bit creating a new
		/// palette with the specified <paramref name="paletteSize"/> using the specified
		/// <paramref name="algorithm"/>.
		/// </summary>
		/// <param name="algorithm">The color reduction algorithm to be used.</param>
		/// <param name="paletteSize">Size of the desired output palette.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Quantize(FREE_IMAGE_QUANTIZE algorithm, int paletteSize)
		{
			return Quantize(algorithm, paletteSize, 0, (RGBQUAD[])null);
		}

		/// <summary>
		/// Quantizes this <see cref="FreeImageBitmap"/> from 24 bit to 8bit creating a new
		/// palette with the specified <paramref name="paletteSize"/> using the specified
		/// <paramref name="algorithm"/> and the specified
		/// <paramref name="reservePalette">palette</paramref> up to the
		/// specified <paramref name="paletteSize">length</paramref>.
		/// </summary>
		/// <param name="algorithm">The color reduction algorithm to be used.</param>
		/// <param name="paletteSize">Size of the desired output palette.</param>
		/// <param name="reservePalette">The provided palette.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Quantize(FREE_IMAGE_QUANTIZE algorithm, int paletteSize, Palette reservePalette)
		{
			return Quantize(algorithm, paletteSize, reservePalette.Length, reservePalette.Data);
		}

		/// <summary>
		/// Quantizes this <see cref="FreeImageBitmap"/> from 24 bit to 8bit creating a new
		/// palette with the specified <paramref name="paletteSize"/> using the specified
		/// <paramref name="algorithm"/> and the specified
		/// <paramref name="reservePalette">palette</paramref> up to the
		/// specified <paramref name="paletteSize">length</paramref>.
		/// </summary>
		/// <param name="algorithm">The color reduction algorithm to be used.</param>
		/// <param name="paletteSize">Size of the desired output palette.</param>
		/// <param name="reserveSize">Size of the provided palette of ReservePalette.</param>
		/// <param name="reservePalette">The provided palette.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Quantize(FREE_IMAGE_QUANTIZE algorithm, int paletteSize, int reserveSize, Palette reservePalette)
		{
			return Quantize(algorithm, paletteSize, reserveSize, reservePalette.Data);
		}

		/// <summary>
		/// Quantizes this <see cref="FreeImageBitmap"/> from 24 bit to 8bit creating a new
		/// palette with the specified <paramref name="paletteSize"/> using the specified
		/// <paramref name="algorithm"/> and the specified
		/// <paramref name="reservePalette">palette</paramref> up to the
		/// specified <paramref name="paletteSize">length</paramref>.
		/// </summary>
		/// <param name="algorithm">The color reduction algorithm to be used.</param>
		/// <param name="paletteSize">Size of the desired output palette.</param>
		/// <param name="reserveSize">Size of the provided palette of ReservePalette.</param>
		/// <param name="reservePalette">The provided palette.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Quantize(FREE_IMAGE_QUANTIZE algorithm, int paletteSize, int reserveSize, RGBQUAD[] reservePalette)
		{
			EnsureNotDisposed();
			return ReplaceDib(FreeImage.ColorQuantizeEx(dib, algorithm, paletteSize, reserveSize, reservePalette));
		}

		/// <summary>
		/// Quantizes this <see cref="FreeImageBitmap"/> from 24 bit, using the specified
		/// <paramref name="algorithm"/> initializing a new 8 bit instance with the
		/// specified <paramref name="paletteSize"/>.
		/// </summary>
		/// <param name="algorithm">The color reduction algorithm to be used.</param>
		/// <param name="paletteSize">Size of the desired output palette.</param>
		/// <returns>The quantized instance.</returns>
		public FreeImageBitmap GetQuantizedInstance(FREE_IMAGE_QUANTIZE algorithm, int paletteSize)
		{
			return GetQuantizedInstance(algorithm, paletteSize, 0, (RGBQUAD[])null);
		}

		/// <summary>
		/// Quantizes this <see cref="FreeImageBitmap"/> from 24 bit, using the specified
		/// <paramref name="algorithm"/> and <paramref name="reservePalette">palette</paramref>
		/// initializing a new 8 bit instance with the specified <paramref name="paletteSize"/>.
		/// </summary>
		/// <param name="algorithm">The color reduction algorithm to be used.</param>
		/// <param name="paletteSize">Size of the desired output palette.</param>
		/// <param name="reservePalette">The provided palette.</param>
		/// <returns>The quantized instance.</returns>
		public FreeImageBitmap GetQuantizedInstance(FREE_IMAGE_QUANTIZE algorithm, int paletteSize, Palette reservePalette)
		{
			return GetQuantizedInstance(algorithm, paletteSize, reservePalette.Length, reservePalette);
		}

		/// <summary>
		/// Quantizes this <see cref="FreeImageBitmap"/> from 24 bit, using the specified
		/// <paramref name="algorithm"/> and up to <paramref name="reserveSize"/>
		/// entries from <paramref name="reservePalette">palette</paramref> initializing
		/// a new 8 bit instance with the specified <paramref name="paletteSize"/>.
		/// </summary>
		/// <param name="algorithm">The color reduction algorithm to be used.</param>
		/// <param name="paletteSize">Size of the desired output palette.</param>
		/// <param name="reserveSize">Size of the provided palette.</param>
		/// <param name="reservePalette">The provided palette.</param>
		/// <returns>The quantized instance.</returns>
		public FreeImageBitmap GetQuantizedInstance(FREE_IMAGE_QUANTIZE algorithm, int paletteSize, int reserveSize, Palette reservePalette)
		{
			return GetQuantizedInstance(algorithm, paletteSize, reserveSize, reservePalette.Data);
		}

		/// <summary>
		/// Quantizes this <see cref="FreeImageBitmap"/> from 24 bit, using the specified
		/// <paramref name="algorithm"/> and up to <paramref name="reserveSize"/>
		/// entries from <paramref name="reservePalette">palette</paramref> initializing
		/// a new 8 bit instance with the specified <paramref name="paletteSize"/>.
		/// </summary>
		/// <param name="algorithm">The color reduction algorithm to be used.</param>
		/// <param name="paletteSize">Size of the desired output palette.</param>
		/// <param name="reserveSize">Size of the provided palette.</param>
		/// <param name="reservePalette">The provided palette.</param>
		/// <returns>The quantized instance.</returns>
		public FreeImageBitmap GetQuantizedInstance(FREE_IMAGE_QUANTIZE algorithm, int paletteSize, int reserveSize, RGBQUAD[] reservePalette)
		{
			EnsureNotDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.ColorQuantizeEx(dib, algorithm, paletteSize, reserveSize, reservePalette);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap(newDib);
			}
			return result;
		}

		/// <summary>
		/// Converts a High Dynamic Range image to a 24-bit RGB image using a global
		/// operator based on logarithmic compression of luminance values, imitating
		/// the human response to light.
		/// </summary>
		/// <param name="gamma">A gamma correction that is applied after the tone mapping.
		/// A value of 1 means no correction.</param>
		/// <param name="exposure">Scale factor allowing to adjust the brightness of the output image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool TmoDrago03(double gamma, double exposure)
		{
			EnsureNotDisposed();
			return ReplaceDib(FreeImage.TmoDrago03(dib, gamma, exposure));
		}

		/// <summary>
		/// Converts a High Dynamic Range image to a 24-bit RGB image using a global operator inspired
		/// by photoreceptor physiology of the human visual system.
		/// </summary>
		/// <param name="intensity">Controls the overall image intensity in the range [-8, 8].</param>
		/// <param name="contrast">Controls the overall image contrast in the range [0.3, 1.0[.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool TmoReinhard05(double intensity, double contrast)
		{
			EnsureNotDisposed();
			return ReplaceDib(FreeImage.TmoReinhard05(dib, intensity, contrast));
		}

		/// <summary>
		/// Apply the Gradient Domain High Dynamic Range Compression to a RGBF image and convert to 24-bit RGB.
		/// </summary>
		/// <param name="color_saturation">Color saturation (s parameter in the paper) in [0.4..0.6]</param>
		/// <param name="attenuation">Atenuation factor (beta parameter in the paper) in [0.8..0.9]</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool TmoFattal02(double color_saturation, double attenuation)
		{
			EnsureNotDisposed();
			return ReplaceDib(FreeImage.TmoFattal02(dib, color_saturation, attenuation));
		}

		/// <summary>
		/// This method rotates a 1-, 4-, 8-bit greyscale or a 24-, 32-bit color image by means of 3 shears.
		/// For 1- and 4-bit images, rotation is limited to angles whose value is an integer
		/// multiple of 90.
		/// </summary>
		/// <param name="angle">The angle of rotation.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Rotate(double angle)
		{
			EnsureNotDisposed();
			bool result = false;
			if (ColorDepth == 4)
			{
				result = ReplaceDib(FreeImage.Rotate4bit(dib, angle));
			}
			else
			{
				result = ReplaceDib(FreeImage.RotateClassic(dib, angle));
			}
			return result;
		}

		/// <summary>
		/// Rotates this <see cref="FreeImageBitmap"/> by the specified angle initializing a new instance.
		/// For 1- and 4-bit images, rotation is limited to angles whose value is an integer
		/// multiple of 90.
		/// </summary>
		/// <param name="angle">The angle of rotation.</param>
		/// <returns>The rotated instance.</returns>
		public FreeImageBitmap GetRotatedInstance(double angle)
		{
			EnsureNotDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib;
			if (ColorDepth == 4)
			{
				newDib = FreeImage.Rotate4bit(dib, angle);
			}
			else
			{
				newDib = FreeImage.RotateClassic(dib, angle);
			}
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap(newDib);
			}
			return result;
		}

		/// <summary>
		/// This method performs a rotation and / or translation of an 8-bit greyscale,
		/// 24- or 32-bit image, using a 3rd order (cubic) B-Spline.
		/// </summary>
		/// <param name="angle">The angle of rotation.</param>
		/// <param name="xShift">Horizontal image translation.</param>
		/// <param name="yShift">Vertical image translation.</param>
		/// <param name="xOrigin">Rotation center x-coordinate.</param>
		/// <param name="yOrigin">Rotation center y-coordinate.</param>
		/// <param name="useMask">When true the irrelevant part of the image is set to a black color,
		/// otherwise, a mirroring technique is used to fill irrelevant pixels.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Rotate(double angle, double xShift, double yShift,
			double xOrigin, double yOrigin, bool useMask)
		{
			EnsureNotDisposed();
			return ReplaceDib(FreeImage.RotateEx(dib, angle, xShift, yShift, xOrigin, yOrigin, useMask));
		}

		/// <summary>
		/// This method performs a rotation and / or translation of an 8-bit greyscale,
		/// 24- or 32-bit image, using a 3rd order (cubic) B-Spline initializing a new instance.
		/// </summary>
		/// <param name="angle">The angle of rotation.</param>
		/// <param name="xShift">Horizontal image translation.</param>
		/// <param name="yShift">Vertical image translation.</param>
		/// <param name="xOrigin">Rotation center x-coordinate.</param>
		/// <param name="yOrigin">Rotation center y-coordinate.</param>
		/// <param name="useMask">When true the irrelevant part of the image is set to a black color,
		/// otherwise, a mirroring technique is used to fill irrelevant pixels.</param>
		/// <returns>The rotated instance.</returns>
		public FreeImageBitmap GetRotatedInstance(double angle, double xShift, double yShift,
			double xOrigin, double yOrigin, bool useMask)
		{
			EnsureNotDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.RotateEx(
				dib, angle, xShift, yShift, xOrigin, yOrigin, useMask);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap(newDib);
			}
			return result;
		}

		/// <summary>
		/// Perfoms an histogram transformation on a 8-, 24- or 32-bit image.
		/// </summary>
		/// <param name="lookUpTable">The lookup table (LUT).
		/// It's size is assumed to be 256 in length.</param>
		/// <param name="channel">The color channel to be transformed.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustCurve(byte[] lookUpTable, FREE_IMAGE_COLOR_CHANNEL channel)
		{
			EnsureNotDisposed();
			return FreeImage.AdjustCurve(dib, lookUpTable, channel);
		}

		/// <summary>
		/// Performs gamma correction on a 8-, 24- or 32-bit image.
		/// </summary>
		/// <param name="gamma">The parameter represents the gamma value to use (gamma > 0).
		/// A value of 1.0 leaves the image alone, less than one darkens it, and greater than one lightens it.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustGamma(double gamma)
		{
			EnsureNotDisposed();
			return FreeImage.AdjustGamma(dib, gamma);
		}

		/// <summary>
		/// Adjusts the brightness of a 8-, 24- or 32-bit image by a certain amount.
		/// </summary>
		/// <param name="percentage">A value 0 means no change,
		/// less than 0 will make the image darker and greater than 0 will make the image brighter.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustBrightness(double percentage)
		{
			EnsureNotDisposed();
			return FreeImage.AdjustBrightness(dib, percentage);
		}

		/// <summary>
		/// Adjusts the contrast of a 8-, 24- or 32-bit image by a certain amount.
		/// </summary>
		/// <param name="percentage">A value 0 means no change,
		/// less than 0 will decrease the contrast and greater than 0 will increase the contrast of the image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustContrast(double percentage)
		{
			EnsureNotDisposed();
			return FreeImage.AdjustContrast(dib, percentage);
		}

		/// <summary>
		/// Inverts each pixel data.
		/// </summary>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Invert()
		{
			EnsureNotDisposed();
			return FreeImage.Invert(dib);
		}

		/// <summary>
		/// Computes the image histogram.
		/// </summary>
		/// <param name="channel">Channel to compute from.</param>
		/// <param name="histogram">Array of integers containing the histogram.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool GetHistogram(FREE_IMAGE_COLOR_CHANNEL channel, out int[] histogram)
		{
			EnsureNotDisposed();
			histogram = new int[256];
			return FreeImage.GetHistogram(dib, histogram, channel);
		}

		/// <summary>
		/// Retrieves the red, green, blue or alpha channel of a 24- or 32-bit image.
		/// </summary>
		/// <param name="channel">The color channel to extract.</param>
		/// <returns>The color channel in a new instance.</returns>
		public FreeImageBitmap GetChannel(FREE_IMAGE_COLOR_CHANNEL channel)
		{
			EnsureNotDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.GetChannel(dib, channel);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap(newDib);
			}
			return result;
		}

		/// <summary>
		/// Insert a 8-bit dib into a 24- or 32-bit image.
		/// Both images must have to same width and height.
		/// </summary>
		/// <param name="bitmap">The <see cref="FreeImageBitmap"/> to insert.</param>
		/// <param name="channel">The color channel to replace.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool SetChannel(FreeImageBitmap bitmap, FREE_IMAGE_COLOR_CHANNEL channel)
		{
			EnsureNotDisposed();
			bitmap.EnsureNotDisposed();
			return FreeImage.SetChannel(dib, bitmap.dib, channel);
		}

		/// <summary>
		/// Retrieves the real part, imaginary part, magnitude or phase of a complex image.
		/// </summary>
		/// <param name="channel">The color channel to extract.</param>
		/// <returns>The color channel in a new instance.</returns>
		public FreeImageBitmap GetComplexChannel(FREE_IMAGE_COLOR_CHANNEL channel)
		{
			EnsureNotDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.GetComplexChannel(dib, channel);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap(newDib);
			}
			return result;
		}

		/// <summary>
		/// Set the real or imaginary part of a complex image.
		/// Both images must have to same width and height.
		/// </summary>
		/// <param name="bitmap">The <see cref="FreeImageBitmap"/> to insert.</param>
		/// <param name="channel">The color channel to replace.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool SetComplexChannel(FreeImageBitmap bitmap, FREE_IMAGE_COLOR_CHANNEL channel)
		{
			EnsureNotDisposed();
			bitmap.EnsureNotDisposed();
			return FreeImage.SetComplexChannel(dib, bitmap.dib, channel);
		}

		/// <summary>
		/// Copy a sub part of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <param name="rect">The subpart to copy.</param>
		/// <returns>The sub part in a new instance.</returns>
		public FreeImageBitmap Copy(Rectangle rect)
		{
			EnsureNotDisposed();
			return Copy(rect.Left, rect.Top, rect.Right, rect.Bottom);
		}

		/// <summary>
		/// Copy a sub part of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <param name="left">Specifies the left position of the cropped rectangle.</param>
		/// <param name="top">Specifies the top position of the cropped rectangle.</param>
		/// <param name="right">Specifies the right position of the cropped rectangle.</param>
		/// <param name="bottom">Specifies the bottom position of the cropped rectangle.</param>
		/// <returns>The sub part in a new instance.</returns>
		public FreeImageBitmap Copy(int left, int top, int right, int bottom)
		{
			EnsureNotDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.Copy(dib, left, top, right, bottom);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap(newDib);
			}
			return result;
		}

		/// <summary>
		/// Alpha blend or combine a sub part image with this <see cref="FreeImageBitmap"/>.
		/// The bit depth of <paramref name="bitmap"/> must be greater than or equal to the bit depth this instance.
		/// </summary>
		/// <param name="bitmap">The <see cref="FreeImageBitmap"/> to paste into this instance.</param>
		/// <param name="left">Specifies the left position of the sub image.</param>
		/// <param name="top">Specifies the top position of the sub image.</param>
		/// <param name="alpha">alpha blend factor.
		/// The source and destination images are alpha blended if alpha=0..255.
		/// If alpha > 255, then the source image is combined to the destination image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Paste(FreeImageBitmap bitmap, int left, int top, int alpha)
		{
			EnsureNotDisposed();
			bitmap.EnsureNotDisposed();
			return FreeImage.Paste(dib, bitmap.dib, left, top, alpha);
		}

		/// <summary>
		/// Alpha blend or combine a sub part image with tthis <see cref="FreeImageBitmap"/>.
		/// The bit depth of <paramref name="bitmap"/> must be greater than or equal to the bit depth this instance.
		/// </summary>
		/// <param name="bitmap">The <see cref="FreeImageBitmap"/> to paste into this instance.</param>
		/// <param name="point">Specifies the position of the sub image.</param>
		/// <param name="alpha">alpha blend factor.
		/// The source and destination images are alpha blended if alpha=0..255.
		/// If alpha > 255, then the source image is combined to the destination image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Paste(FreeImageBitmap bitmap, Point point, int alpha)
		{
			EnsureNotDisposed();
			return Paste(bitmap, point.X, point.Y, alpha);
		}

		/// <summary>
		/// This method composite a transparent foreground image against a single background color or
		/// against a background image.
		/// In case <paramref name="useBitmapBackground"/> is false and <paramref name="applicationBackground"/>
		/// and <paramref name="bitmapBackGround"/>
		/// are null, a checkerboard will be used as background.
		/// </summary>
		/// <param name="useBitmapBackground">When true the background of this instance is used
		/// if it contains one.</param>
		/// <param name="applicationBackground">Backgroundcolor used in case <paramref name="useBitmapBackground"/> is false
		/// and <paramref name="applicationBackground"/> is not null.</param>
		/// <param name="bitmapBackGround">Background used in case <paramref name="useBitmapBackground"/>
		/// is false and <paramref name="applicationBackground"/> is null.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Composite(bool useBitmapBackground, Color? applicationBackground, FreeImageBitmap bitmapBackGround)
		{
			EnsureNotDisposed();
			bitmapBackGround.EnsureNotDisposed();
			RGBQUAD? rgb = applicationBackground;
			return ReplaceDib(
				FreeImage.Composite(
					dib,
					useBitmapBackground,
					rgb.HasValue ? new RGBQUAD[] { rgb.Value } : null,
					bitmapBackGround.dib));
		}

		/// <summary>
		/// Applies the alpha value of each pixel to its color components.
		/// The aplha value stays unchanged.
		/// Only works with 32-bits color depth.
		/// </summary>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool PreMultiplyWithAlpha()
		{
			EnsureNotDisposed();
			return FreeImage.PreMultiplyWithAlpha(dib);
		}

		/// <summary>
		/// Solves a Poisson equation, remap result pixels to [0..1] and returns the solution.
		/// </summary>
		/// <param name="ncycle">Number of cycles in the multigrid algorithm (usually 2 or 3)</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool MultigridPoissonSolver(int ncycle)
		{
			EnsureNotDisposed();
			return ReplaceDib(FreeImage.MultigridPoissonSolver(dib, ncycle));
		}

		/// <summary>
		/// Adjusts an image's brightness, contrast and gamma as well as it may
		/// optionally invert the image within a single operation.
		/// </summary>
		/// <param name="brightness">Percentage brightness value where -100 &lt;= brightness &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will make the image darker and greater
		/// than 0 will make the image brighter.</para></param>
		/// <param name="contrast">Percentage contrast value where -100 &lt;= contrast &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will decrease the contrast
		/// and greater than 0 will increase the contrast of the image.</para></param>
		/// <param name="gamma">Gamma value to be used for gamma correction.
		/// <para>A value of 1.0 leaves the image alone, less than one darkens it,
		/// and greater than one lightens it.</para>
		/// This parameter must not be zero or smaller than zero.
		/// If so, it will be ignored and no gamma correction will be performed on the image.</param>
		/// <param name="invert">If set to true, the image will be inverted.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustColors(double brightness, double contrast, double gamma, bool invert)
		{
			EnsureNotDisposed();
			return FreeImage.AdjustColors(dib, brightness, contrast, gamma, invert);
		}

		/// <summary>
		/// Applies color mapping for one or several colors on a 1-, 4- or 8-bit
		/// palletized or a 16-, 24- or 32-bit high color image.
		/// </summary>
		/// <param name="srccolors">Array of colors to be used as the mapping source.</param>
		/// <param name="dstcolors">Array of colors to be used as the mapping destination.</param>
		/// <param name="ignore_alpha">If true, 32-bit images and colors are treated as 24-bit.</param>
		/// <param name="swap">If true, source and destination colors are swapped, that is,
		/// each destination color is also mapped to the corresponding source color.</param>
		/// <returns>The total number of pixels changed.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="srccolors"/> or <paramref name="dstcolors"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="srccolors"/> has a different length than <paramref name="dstcolors"/>.
		/// </exception>
		public uint ApplyColorMapping(RGBQUAD[] srccolors, RGBQUAD[] dstcolors, bool ignore_alpha, bool swap)
		{
			EnsureNotDisposed();
			if (srccolors == null)
			{
				throw new ArgumentNullException("srccolors");
			}
			if (dstcolors == null)
			{
				throw new ArgumentNullException("dstcolors");
			}
			if (srccolors.Length != dstcolors.Length)
			{
				throw new ArgumentException("srccolors and dstcolors must have the same length.");
			}
			return FreeImage.ApplyColorMapping(dib, srccolors, dstcolors, (uint)srccolors.Length, ignore_alpha, swap);
		}

		/// <summary>
		/// Swaps two specified colors on a 1-, 4- or 8-bit palletized
		/// or a 16-, 24- or 32-bit high color image.
		/// </summary>
		/// <param name="color_a">One of the two colors to be swapped.</param>
		/// <param name="color_b">The other of the two colors to be swapped.</param>
		/// <param name="ignore_alpha">If true, 32-bit images and colors are treated as 24-bit.</param>
		/// <returns>The total number of pixels changed.</returns>
		public uint SwapColors(RGBQUAD color_a, RGBQUAD color_b, bool ignore_alpha)
		{
			EnsureNotDisposed();
			return FreeImage.SwapColors(dib, ref color_a, ref color_b, ignore_alpha);
		}

		/// <summary>
		/// Applies palette index mapping for one or several indices
		/// on a 1-, 4- or 8-bit palletized image.
		/// </summary>
		/// <param name="srcindices">Array of palette indices to be used as the mapping source.</param>
		/// <param name="dstindices">Array of palette indices to be used as the mapping destination.</param>
		/// <param name="count">The number of palette indices to be mapped. This is the size of both
		/// srcindices and dstindices</param>
		/// <param name="swap">If true, source and destination palette indices are swapped, that is,
		/// each destination index is also mapped to the corresponding source index.</param>
		/// <returns>The total number of pixels changed.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="srccolors"/> or <paramref name="dstcolors"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="srccolors"/> has a different length than <paramref name="dstcolors"/>.
		/// </exception>
		public uint ApplyPaletteIndexMapping(byte[] srcindices, byte[] dstindices, uint count, bool swap)
		{
			EnsureNotDisposed();
			if (srcindices == null)
			{
				throw new ArgumentNullException("srcindices");
			}
			if (dstindices == null)
			{
				throw new ArgumentNullException("dstindices");
			}
			if (srcindices.Length != dstindices.Length)
			{
				throw new ArgumentException("srcindices and dstindices must have the same length.");
			}
			return FreeImage.ApplyPaletteIndexMapping(dib, srcindices, dstindices, (uint)srcindices.Length, swap);
		}

		/// <summary>
		/// Swaps two specified palette indices on a 1-, 4- or 8-bit palletized image.
		/// </summary>
		/// <param name="index_a">One of the two palette indices to be swapped.</param>
		/// <param name="index_b">The other of the two palette indices to be swapped.</param>
		/// <returns>The total number of pixels changed.</returns>
		public uint SwapPaletteIndices(byte index_a, byte index_b)
		{
			EnsureNotDisposed();
			return FreeImage.SwapPaletteIndices(dib, ref index_a, ref index_b);
		}

		/// <summary>
		/// Creates a new ICC-Profile.
		/// </summary>
		/// <param name="data">The data of the new ICC-Profile.</param>
		/// <returns>The new ICC-Profile of the bitmap.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
		public FIICCPROFILE CreateICCProfile(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return CreateICCProfile(data, data.Length);
		}

		/// <summary>
		/// Creates a new ICC-Profile.
		/// </summary>
		/// <param name="data">The data of the new ICC-Profile.</param>
		/// <param name="size">The number of bytes of <paramref name="data"/> to use.</param>
		/// <returns>The new ICC-Profile of the bitmap.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
		public FIICCPROFILE CreateICCProfile(byte[] data, int size)
		{
			EnsureNotDisposed();
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return FreeImage.CreateICCProfileEx(dib, data, size);
		}

		private void AddMemoryPressure()
		{
			GC.AddMemoryPressure(DataSize);
		}

		/// <summary>
		/// Determines whether this and the specified instances are the same.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if this instance is the same <paramref name="obj"/>
		/// or if both are null references; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj);
		}

		/// <summary>
		/// Returns a hash code for this <see cref="FreeImageBitmap"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="FreeImageBitmap"/>.</returns>
		public override int GetHashCode()
		{
			return dib.GetHashCode();
		}

		#endregion

		#region Static functions

		/// <summary>
		/// Returns a value that indicates whether the pixel format for this <see cref="FreeImageBitmap"/> contains alpha information.
		/// </summary>
		/// <param name="pixfmt">The <see cref="System.Drawing.Imaging.PixelFormat"/> to test.</param>
		/// <returns><b>true</b> if pixfmt contains alpha information; otherwise, <b>false</b>.</returns>
		public static bool IsAlphaPixelFormat(PixelFormat pixfmt)
		{
			return Bitmap.IsAlphaPixelFormat(pixfmt);
		}

		/// <summary>
		/// Returns a value that indicates whether the pixel format is 32 bits per pixel.
		/// </summary>
		/// <param name="pixfmt">The <see cref="System.Drawing.Imaging.PixelFormat"/> to test.</param>
		/// <returns>true if pixfmt is canonical; otherwise, false.</returns>
		public static bool IsCanonicalPixelFormat(PixelFormat pixfmt)
		{
			return Bitmap.IsCanonicalPixelFormat(pixfmt);
		}

		/// <summary>
		/// Returns a value that indicates whether the pixel format is 64 bits per pixel.
		/// </summary>
		/// <param name="pixfmt">The <see cref="System.Drawing.Imaging.PixelFormat"/> enumeration to test.</param>
		/// <returns>true if pixfmt is extended; otherwise, false.</returns>
		public static bool IsExtendedPixelFormat(PixelFormat pixfmt)
		{
			return Bitmap.IsExtendedPixelFormat(pixfmt);
		}

		/// <summary>
		/// Creates a <see cref="FreeImageBitmap"/> from a Windows handle to an icon.
		/// </summary>
		/// <param name="hicon">A handle to an icon.</param>
		/// <returns>The <see cref="FreeImageBitmap"/> that this method creates.</returns>
		public static FreeImageBitmap FromHicon(IntPtr hicon)
		{
			using (Bitmap bitmap = Bitmap.FromHicon(hicon))
			{
				return new FreeImageBitmap(bitmap);
			}
		}

		/// <summary>
		/// Creates a <see cref="FreeImageBitmap"/> from the specified Windows resource.
		/// </summary>
		/// <param name="hinstance">A handle to an instance of the executable
		/// file that contains the resource.</param>
		/// <param name="bitmapName">A string containing the name of the resource bitmap.</param>
		/// <returns>The <see cref="FreeImageBitmap"/> that this method creates.</returns>
		public static FreeImageBitmap FromResource(IntPtr hinstance, string bitmapName)
		{
			using (Bitmap bitmap = Bitmap.FromResource(hinstance, bitmapName))
			{
				return new FreeImageBitmap(bitmap);
			}
		}

		/// <summary>
		/// Creates a <see cref="FreeImageBitmap"/> from the specified file.
		/// </summary>
		/// <param name="filename">A string that contains the name of the file
		/// from which to create the <see cref="FreeImageBitmap"/>.</param>
		/// <returns>The <see cref="FreeImageBitmap"/> this method creates.</returns>
		public static FreeImageBitmap FromFile(string filename)
		{
			return new FreeImageBitmap(filename);
		}

		/// <summary>
		/// Creates a <see cref="FreeImageBitmap"/> from the specified file
		/// using embedded color management information in that file.
		/// </summary>
		/// <param name="filename">A string that contains the
		/// name of the file from which to create the <see cref="FreeImageBitmap"/>.</param>
		/// <param name="useEmbeddedColorManagement">Ignored.</param>
		/// <returns>The <see cref="FreeImageBitmap"/> this method creates.</returns>
		public static FreeImageBitmap FromFile(string filename, bool useEmbeddedColorManagement)
		{
			return new FreeImageBitmap(filename);
		}

		/// <summary>
		/// Creates a <see cref="FreeImageBitmap"/> from a handle to a GDI bitmap.
		/// </summary>
		/// <param name="hbitmap">The GDI bitmap handle from which to create the <see cref="FreeImageBitmap"/>.</param>
		/// <returns>The <see cref="FreeImageBitmap"/> this method creates.</returns>
		public static FreeImageBitmap FromHbitmap(IntPtr hbitmap)
		{
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.CreateFromHbitmap(hbitmap, IntPtr.Zero);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap(newDib);
			}
			return result;
		}

		/// <summary>
		/// Creates a <see cref="FreeImageBitmap"/> from a handle to a GDI bitmap and a handle to a GDI palette.
		/// </summary>
		/// <param name="hbitmap">The GDI bitmap handle from which to create the <see cref="FreeImageBitmap"/>.</param>
		/// <param name="hpalette">Ignored.</param>
		/// <returns>The <see cref="FreeImageBitmap"/> this method creates.</returns>
		public static FreeImageBitmap FromHbitmap(IntPtr hbitmap, IntPtr hpalette)
		{
			return FromHbitmap(hbitmap);
		}

		/// <summary>
		/// Frees a bitmap handle.
		/// </summary>
		/// <param name="hbitmap">Handle to a bitmap.</param>
		/// <returns><b>true</b> on success, <b>false</b> on failure.</returns>
		public static bool FreeHbitmap(IntPtr hbitmap)
		{
			return FreeImage.FreeHbitmap(hbitmap);
		}

		/// <summary>
		/// Creates a <see cref="FreeImageBitmap"/> from the specified data stream.
		/// </summary>
		/// <param name="stream">A <see cref="Stream"/> that contains the data for this <see cref="FreeImageBitmap"/>.</param>
		/// <returns>The <see cref="FreeImageBitmap"/> this method creates.</returns>
		public static FreeImageBitmap FromStream(Stream stream)
		{
			return new FreeImageBitmap(stream);
		}

		/// <summary>
		/// Creates a <see cref="FreeImageBitmap"/> from the specified data stream.
		/// </summary>
		/// <param name="stream">A <see cref="Stream"/> that contains the data for this <see cref="FreeImageBitmap"/>.</param>
		/// <param name="useEmbeddedColorManagement">Ignored.</param>
		/// <returns>The <see cref="FreeImageBitmap"/> this method creates.</returns>
		public static FreeImageBitmap FromStream(Stream stream, bool useEmbeddedColorManagement)
		{
			return new FreeImageBitmap(stream);
		}

		/// <summary>
		/// Creates a <see cref="FreeImageBitmap"/> from the specified data stream.
		/// </summary>
		/// <param name="stream">A <see cref="Stream"/> that contains the data for this <see cref="FreeImageBitmap"/>.</param>
		/// <param name="useEmbeddedColorManagement">Ignored.</param>
		/// <param name="validateImageData">Ignored.</param>
		/// <returns>The <see cref="FreeImageBitmap"/> this method creates.</returns>
		public static FreeImageBitmap FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
		{
			return new FreeImageBitmap(stream);
		}

		/// <summary>
		/// Returns the color depth, in number of bits per pixel,
		/// of the specified pixel format.
		/// </summary>
		/// <param name="pixfmt">The <see cref="System.Drawing.Imaging.PixelFormat"/> member that specifies
		/// the format for which to find the size.</param>
		/// <returns>The color depth of the specified pixel format.</returns>
		public static int GetPixelFormatSize(PixelFormat pixfmt)
		{
			return Bitmap.GetPixelFormatSize(pixfmt);
		}

		/// <summary>
		/// Performs a lossless rotation or flipping on a JPEG file.
		/// </summary>
		/// <param name="source">Source file.</param>
		/// <param name="destination">Destination file; can be the source file; will be overwritten.</param>
		/// <param name="operation">The operation to apply.</param>
		/// <param name="perfect">To avoid lossy transformation, you can set the perfect parameter to true.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool JPEGTransform(string source, string destination, FREE_IMAGE_JPEG_OPERATION operation, bool perfect)
		{
			return FreeImage.JPEGTransform(source, destination, operation, perfect);
		}

		/// <summary>
		/// Performs a lossless crop on a JPEG file.
		/// </summary>
		/// <param name="source">Source filename.</param>
		/// <param name="destination">Destination filename.</param>
		/// <param name="rect">Specifies the cropped rectangle.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="source"/> or <paramref name="destination"/> is null.
		/// </exception>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="source"/> does not exist.
		/// </exception>
		public static bool JPEGCrop(string source, string destination, Rectangle rect)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!File.Exists(source))
			{
				throw new FileNotFoundException("source");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			return JPEGCrop(source, destination, rect.Left, rect.Top, rect.Right, rect.Bottom);
		}

		/// <summary>
		/// Performs a lossless crop on a JPEG file.
		/// </summary>
		/// <param name="source">Source filename.</param>
		/// <param name="destination">Destination filename.</param>
		/// <param name="left">Specifies the left position of the cropped rectangle.</param>
		/// <param name="top">Specifies the top position of the cropped rectangle.</param>
		/// <param name="right">Specifies the right position of the cropped rectangle.</param>
		/// <param name="bottom">Specifies the bottom position of the cropped rectangle.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="source"/> or <paramref name="destination"/> is null.
		/// </exception>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="source"/> does not exist.
		/// </exception>
		public static bool JPEGCrop(string source, string destination, int left, int top, int right, int bottom)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!File.Exists(source))
			{
				throw new FileNotFoundException("source");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			return FreeImage.JPEGCrop(source, destination, left, top, right, bottom);
		}

		/// <summary>
		/// Converts a X11 color name into a corresponding RGB value.
		/// </summary>
		/// <param name="color">Name of the color to convert.</param>
		/// <param name="red">Red component.</param>
		/// <param name="green">Green component.</param>
		/// <param name="blue">Blue component.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="color"/> is null.</exception>
		public static bool LookupX11Color(string color, out byte red, out byte green, out byte blue)
		{
			if (color == null)
			{
				throw new ArgumentNullException("color");
			}
			return FreeImage.LookupX11Color(color, out red, out green, out blue);
		}

		/// <summary>
		/// Converts a SVG color name into a corresponding RGB value.
		/// </summary>
		/// <param name="color">Name of the color to convert.</param>
		/// <param name="red">Red component.</param>
		/// <param name="green">Green component.</param>
		/// <param name="blue">Blue component.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="color"/> is null.</exception>
		public static bool LookupSVGColor(string color, out byte red, out byte green, out byte blue)
		{
			if (color == null)
			{
				throw new ArgumentNullException("color");
			}
			return FreeImage.LookupSVGColor(color, out red, out green, out blue);
		}

		/// <summary>
		/// Creates a lookup table to be used with AdjustCurve() which
		/// may adjusts brightness and contrast, correct gamma and invert the image with a
		/// single call to AdjustCurve().
		/// </summary>
		/// <param name="lookUpTable">Output lookup table to be used with AdjustCurve().
		/// The size of <paramref name="lookUpTable"/> is assumed to be 256.</param>
		/// <param name="brightness">Percentage brightness value where -100 &lt;= brightness &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will make the image darker and greater
		/// than 0 will make the image brighter.</para></param>
		/// <param name="contrast">Percentage contrast value where -100 &lt;= contrast &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will decrease the contrast
		/// and greater than 0 will increase the contrast of the image.</para></param>
		/// <param name="gamma">Gamma value to be used for gamma correction.
		/// <para>A value of 1.0 leaves the image alone, less than one darkens it,
		/// and greater than one lightens it.</para></param>
		/// <param name="invert">If set to true, the image will be inverted.</param>
		/// <returns>The number of adjustments applied to the resulting lookup table
		/// compared to a blind lookup table.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="lookUpTable"/> is null.</exception>
		/// <exception cref="ArgumentException"><paramref name="lookUpTable.Length"/> is not 256.</exception>
		public static int GetAdjustColorsLookupTable(byte[] lookUpTable, double brightness, double contrast, double gamma, bool invert)
		{
			if (lookUpTable == null)
			{
				throw new ArgumentNullException("lookUpTable");
			}
			if (lookUpTable.Length != 256)
			{
				throw new ArgumentException("lookUpTable");
			}
			return FreeImage.GetAdjustColorsLookupTable(lookUpTable, brightness, contrast, gamma, invert);
		}

		/// <summary>
		/// Adds a specified frame to the file specified using the specified parameters.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <param name="filename">File to add this frame to.</param>
		/// <param name="bitmap">A <see cref="FreeImageBitmap"/> that contains the frame to add.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="loadFlags">Flags to enable or disable plugin-features.</param>
		/// <param name="saveFlags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="filename"/> or <paramref name="bitmap"/> is null.
		/// </exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public static void SaveAdd(
			string filename,
			FreeImageBitmap bitmap,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_LOAD_FLAGS loadFlags,
			FREE_IMAGE_SAVE_FLAGS saveFlags)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException("filename");
			}
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			bitmap.EnsureNotDisposed();
			if (bitmap.dib.IsNull)
			{
				throw new ArgumentNullException("bitmap");
			}

			FIMULTIBITMAP mpBitmap =
				FreeImage.OpenMultiBitmapEx(filename, ref format, loadFlags, false, false, true);

			if (mpBitmap.IsNull)
			{
				throw new Exception();
			}

			FreeImage.AppendPage(mpBitmap, bitmap.dib);

			if (!FreeImage.CloseMultiBitmap(mpBitmap, saveFlags))
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Returns a new instance of the <see cref="PropertyItem"/> class which
		/// has no public accessible constructor.
		/// </summary>
		/// <returns>A new instace of <see cref="PropertyItem"/>.</returns>
		public static PropertyItem CreateNewPropertyItem()
		{
			return FreeImage.CreatePropertyItem();
		}

		#endregion

		#region Helper functions

		/// <summary>
		/// Throws an exception in case the instance has already been disposed.
		/// </summary>
		private void EnsureNotDisposed()
		{
			lock (lockObject)
			{
				if (!this.disposed)
				{
					return;
				}
			}
			throw new ObjectDisposedException(ToString());
		}

		/// <summary>
		/// Tries to replace the wrapped <see cref="FIBITMAP"/> with a new one.
		/// In case the new dib is null or the same as the already
		/// wrapped one, nothing will be changed and the result will
		/// be false.
		/// Otherwise the wrapped <see cref="FIBITMAP"/> will be unloaded and replaced.
		/// </summary>
		/// <param name="newDib">The new dib.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		private bool ReplaceDib(FIBITMAP newDib)
		{
			bool result = false;
			if ((dib != newDib) && (!newDib.IsNull))
			{
				if (mdib.IsNull)
				{
					UnloadDib();
					dib = newDib;
					AddMemoryPressure();
				}
				else
				{
					UnloadDib();
					FreeImage.CloseMultiBitmapEx(ref mdib);

					dib = newDib;
					AddMemoryPressure();
				}
				result = true;
			}
			return result;
		}

		/// <summary>
		/// Unloads currently wrapped <see cref="FIBITMAP"/> or unlocks the locked page
		/// in case it came from a multipaged bitmap.
		/// </summary>
		private void UnloadDib()
		{
			if (mdib.IsNull)
			{
				long size = FreeImage.GetDIBSize(dib);
				FreeImage.UnloadEx(ref dib);
				GC.RemoveMemoryPressure(size);
			}
			else if (!dib.IsNull)
			{
				long size = FreeImage.GetDIBSize(dib);
				FreeImage.UnlockPage(mdib, dib, false);
				GC.RemoveMemoryPressure(size);
				dib.SetNull();
			}
		}

		#endregion

		#region Interfaces

		/// <summary>
		/// Helper class to store informations for <see cref="FreeImageBitmap.SaveAdd()"/>.
		/// </summary>
		private class SaveInformation : ICloneable
		{
			public string filename = null;
			public FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			public FREE_IMAGE_LOAD_FLAGS loadFlags = FREE_IMAGE_LOAD_FLAGS.DEFAULT;
			public FREE_IMAGE_SAVE_FLAGS saveFlags = FREE_IMAGE_SAVE_FLAGS.DEFAULT;

			public object Clone()
			{
				return base.MemberwiseClone();
			}
		}

		/// <summary>
		/// Creates a deep copy of this <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <returns>A deep copy of this <see cref="FreeImageBitmap"/>.</returns>
		public object Clone()
		{
			EnsureNotDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.Clone(dib);
			if (!dib.IsNull)
			{
				result = new FreeImageBitmap(newDib);
				result.saveInformation = (SaveInformation)saveInformation.Clone();
				result.tag = tag;
				result.originalFormat = originalFormat;
			}
			return result;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing,
		/// releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing,
		/// releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">If true managed ressources are released.</param>
		protected virtual void Dispose(bool disposing)
		{
			// Only clean up once
			lock (lockObject)
			{
				if (disposed)
				{
					return;
				}
				disposed = true;
			}

			// Clean up managed resources
			if (disposing)
			{
				tag = null;
			}

			// Clean up unmanaged resources
			UnloadDib();
			FreeImage.CloseMultiBitmapEx(ref mdib);
		}

		/// <summary>
		/// Retrieves an object that can iterate through the individual scanlines in this <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <returns>An <see cref="IEnumerator"/> for the <see cref="FreeImageBitmap"/>.</returns>
		/// <exception cref="ArgumentException">The bitmaps's type is not supported.</exception>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetScanlines().GetEnumerator();
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			EnsureNotDisposed();
			using (MemoryStream memory = new MemoryStream(DataSize))
			{
				if (!FreeImage.SaveToStream(ref dib, memory, FREE_IMAGE_FORMAT.FIF_TIFF, FREE_IMAGE_SAVE_FLAGS.TIFF_LZW, false))
				{
					throw new SerializationException();
				}
				memory.Capacity = (int)memory.Length;
				info.AddValue("Bitmap Data", memory.GetBuffer());
			}
		}

		#endregion
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Class handling non-bitmap related functions.
	/// </summary>
	public static class FreeImageEngine
	{
		#region Callback

		// Callback delegate
		private static OutputMessageFunction outputMessageFunction;
		// Handle to pin the functions address
		private static GCHandle outputMessageHandle;

		static FreeImageEngine()
		{
			// Check if FreeImage.dll is present and cancel setting the callbackfuntion if not
			if (!IsAvailable)
			{
				return;
			}
			// Create a delegate (function pointer) to 'OnMessage'
			outputMessageFunction = new OutputMessageFunction(OnMessage);
			// Pin the object so the garbage collector does not move it around in memory
			outputMessageHandle = GCHandle.Alloc(outputMessageFunction, GCHandleType.Normal);
			// Set the callback
			FreeImage.SetOutputMessage(outputMessageFunction);
		}

		/// <summary>
		/// Internal callback
		/// </summary>
		private static void OnMessage(FREE_IMAGE_FORMAT fif, string message)
		{
			// Invoke the message
			if (Message != null)
			{
				Message.Invoke(fif, message);
			}
		}

		/// <summary>
		/// Gets a value indicating if the FreeImage DLL is available or not.
		/// </summary>
		public static bool IsAvailable
		{
			get
			{
				return FreeImage.IsAvailable();
			}
		}

		/// <summary>
		/// Internal errors in FreeImage generate a logstring that can be
		/// captured by this event.
		/// </summary>
		public static event OutputMessageFunction Message;

		#endregion

		/// <summary>
		/// Gets a string containing the current version of the library.
		/// </summary>
		public static string Version
		{
			get
			{
				return FreeImage.GetVersion();
			}
		}

		/// <summary>
		/// Gets a string containing a standard copyright message.
		/// </summary>
		public static string CopyrightMessage
		{
			get
			{
				return FreeImage.GetCopyrightMessage();
			}
		}

		/// <summary>
		/// Gets whether the platform is using Little Endian.
		/// </summary>
		public static bool IsLittleEndian
		{
			get
			{
				return FreeImage.IsLittleEndian();
			}
		}
	}
}

namespace FreeImageAPI.Plugins
{
	/// <summary>
	/// Class representing a FreeImage format.
	/// </summary>
	public sealed class FreeImagePlugin
	{
		private readonly FREE_IMAGE_FORMAT fif;

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="fif">The FreeImage format to wrap.</param>
		internal FreeImagePlugin(FREE_IMAGE_FORMAT fif)
		{
			this.fif = fif;
		}

		/// <summary>
		/// Gets the format of this instance.
		/// </summary>
		public FREE_IMAGE_FORMAT FIFormat
		{
			get
			{
				return fif;
			}
		}

		/// <summary>
		/// Gets or sets whether this plugin is enabled.
		/// </summary>
		public bool Enabled
		{
			get
			{
				return (FreeImage.IsPluginEnabled(fif) == 1);
			}
			set
			{
				FreeImage.SetPluginEnabled(fif, value);
			}
		}

		/// <summary>
		/// Gets a string describing the format.
		/// </summary>
		public string Format
		{
			get
			{
				return FreeImage.GetFormatFromFIF(fif);
			}
		}

		/// <summary>
		/// Gets a comma-delimited file extension list describing the bitmap formats
		/// this plugin can read and/or write.
		/// </summary>
		public string ExtentsionList
		{
			get
			{
				return FreeImage.GetFIFExtensionList(fif);
			}
		}

		/// <summary>
		/// Gets a descriptive string that describes the bitmap formats
		/// this plugin can read and/or write.
		/// </summary>
		public string Description
		{
			get
			{
				return FreeImage.GetFIFDescription(fif);
			}
		}

		/// <summary>
		/// Returns a regular expression string that can be used by
		/// a regular expression engine to identify the bitmap.
		/// FreeImageQt makes use of this function.
		/// </summary>
		public string RegExpr
		{
			get
			{
				return FreeImage.GetFIFRegExpr(fif);
			}
		}

		/// <summary>
		/// Gets whether this plugin can load bitmaps.
		/// </summary>
		public bool SupportsReading
		{
			get
			{
				return FreeImage.FIFSupportsReading(fif);
			}
		}

		/// <summary>
		/// Gets whether this plugin can save bitmaps.
		/// </summary>
		public bool SupportsWriting
		{
			get
			{
				return FreeImage.FIFSupportsWriting(fif);
			}
		}

		/// <summary>
		/// Checks whether this plugin can save a bitmap in the desired data type.
		/// </summary>
		/// <param name="type">The desired image type.</param>
		/// <returns>True if this plugin can save bitmaps as the desired type, else false.</returns>
		public bool SupportsExportType(FREE_IMAGE_TYPE type)
		{
			return FreeImage.FIFSupportsExportType(fif, type);
		}

		/// <summary>
		/// Checks whether this plugin can save bitmaps in the desired bit depth.
		/// </summary>
		/// <param name="bpp">The desired bit depth.</param>
		/// <returns>True if this plugin can save bitmaps in the desired bit depth, else false.</returns>
		public bool SupportsExportBPP(int bpp)
		{
			return FreeImage.FIFSupportsExportBPP(fif, bpp);
		}

		/// <summary>
		/// Gets whether this plugin can load or save an ICC profile.
		/// </summary>
		public bool SupportsICCProfiles
		{
			get
			{
				return FreeImage.FIFSupportsICCProfiles(fif);
			}
		}

		/// <summary>
		/// Checks whether an extension is valid for this format.
		/// </summary>
		/// <param name="extension">The desired extension.</param>
		/// <returns>True if the extension is valid for this format, false otherwise.</returns>
		public bool ValidExtension(string extension)
		{
			return FreeImage.IsExtensionValidForFIF(fif, extension);
		}

		/// <summary>
		/// Checks whether an extension is valid for this format.
		/// </summary>
		/// <param name="extension">The desired extension.</param>
		/// <param name="comparisonType">The string comparison type.</param>
		/// <returns>True if the extension is valid for this format, false otherwise.</returns>
		public bool ValidExtension(string extension, StringComparison comparisonType)
		{
			return FreeImage.IsExtensionValidForFIF(fif, extension, comparisonType);
		}

		/// <summary>
		/// Checks whether a filename is valid for this format.
		/// </summary>
		/// <param name="filename">The desired filename.</param>
		/// <returns>True if the filename is valid for this format, false otherwise.</returns>
		public bool ValidFilename(string filename)
		{
			return FreeImage.IsFilenameValidForFIF(fif, filename);
		}

		/// <summary>
		/// Checks whether a filename is valid for this format.
		/// </summary>
		/// <param name="filename">The desired filename.</param>
		/// <param name="comparisonType">The string comparison type.</param>
		/// <returns>True if the filename is valid for this format, false otherwise.</returns>
		public bool ValidFilename(string filename, StringComparison comparisonType)
		{
			return FreeImage.IsFilenameValidForFIF(fif, filename, comparisonType);
		}

		/// <summary>
		/// Gets a descriptive string that describes the bitmap formats
		/// this plugin can read and/or write.
		/// </summary>
		/// <returns>A descriptive string that describes the bitmap formats.</returns>
		public override string ToString()
		{
			return Description;
		}
	}
}

namespace FreeImageAPI.IO
{
	/// <summary>
	/// Internal class wrapping stream io functions.
	/// </summary>
	/// <remarks>
	/// FreeImage can read files from a disk or a network drive but also allows the user to
	/// implement their own loading or saving functions to load them directly from an ftp or web
	/// server for example.
	/// <para/>
	/// In .NET streams are a common way to handle data. The <b>FreeImageStreamIO</b> class handles
	/// the loading and saving from and to streams. It implements the funtions FreeImage needs
	/// to load data from an an arbitrary source.
	/// <para/>
	/// FreeImage requests a <see cref="FreeImageAPI.IO.FreeImageIO"/> structure containing pointers (delegates) to these
	/// functions. <b>FreeImageStreamIO</b> implements the function creates the structure and
	/// prevents the garbage collector from moving these functions in memory.
	/// <para/>
	/// The class is for internal use only.
	/// </remarks>
	internal static class FreeImageStreamIO
	{
		private static GCHandle readHandle;
		private static GCHandle writeHandle;
		private static GCHandle seekHandle;
		private static GCHandle tellHandle;

		/// <summary>
		/// <see cref="FreeImageAPI.IO.FreeImageIO"/> structure that can be used to read from streams via
		/// <see cref="FreeImageAPI.FreeImage.LoadFromHandle(FREE_IMAGE_FORMAT, ref FreeImageIO, fi_handle, FREE_IMAGE_LOAD_FLAGS)"/>.
		/// </summary>
		public static FreeImageIO io;

		/// <summary>
		/// Initializes a new instances which can be used to
		/// create a FreeImage compatible <see cref="FreeImageAPI.IO.FreeImageIO"/> structure.
		/// </summary>
		static FreeImageStreamIO()
		{
			io.readProc = new ReadProc(streamRead);
			io.writeProc = new WriteProc(streamWrite);
			io.seekProc = new SeekProc(streamSeek);
			io.tellProc = new TellProc(streamTell);
			readHandle = GCHandle.Alloc(io.readProc, GCHandleType.Normal);
			writeHandle = GCHandle.Alloc(io.writeProc, GCHandleType.Normal);
			seekHandle = GCHandle.Alloc(io.seekProc, GCHandleType.Normal);
			tellHandle = GCHandle.Alloc(io.tellProc, GCHandleType.Normal);
		}

		/// <summary>
		/// Reads the requested data from the stream and writes it to the given address.
		/// </summary>
		static unsafe uint streamRead(IntPtr buffer, uint size, uint count, fi_handle handle)
		{
			Stream stream = handle.GetObject() as Stream;
			if ((stream == null) || (!stream.CanRead))
			{
				return 0;
			}
			uint readCount = 0;
			byte* ptr = (byte*)buffer;
			byte[] bufferTemp = new byte[size];
			int read;
			while (readCount < count)
			{
				read = stream.Read(bufferTemp, 0, (int)size);
				if (read != (int)size)
				{
					stream.Seek(-read, SeekOrigin.Current);
					break;
				}
				for (int i = 0; i < read; i++, ptr++)
				{
					*ptr = bufferTemp[i];
				}
				readCount++;
			}
			return (uint)readCount;
		}

		/// <summary>
		/// Reads the given data and writes it into the stream.
		/// </summary>
		static unsafe uint streamWrite(IntPtr buffer, uint size, uint count, fi_handle handle)
		{
			Stream stream = handle.GetObject() as Stream;
			if ((stream == null) || (!stream.CanWrite))
			{
				return 0;
			}
			uint writeCount = 0;
			byte[] bufferTemp = new byte[size];
			byte* ptr = (byte*)buffer;
			while (writeCount < count)
			{
				for (int i = 0; i < size; i++, ptr++)
				{
					bufferTemp[i] = *ptr;
				}
				try
				{
					stream.Write(bufferTemp, 0, bufferTemp.Length);
				}
				catch
				{
					return writeCount;
				}
				writeCount++;
			}
			return writeCount;
		}

		/// <summary>
		/// Moves the streams position.
		/// </summary>
		static int streamSeek(fi_handle handle, int offset, SeekOrigin origin)
		{
			Stream stream = handle.GetObject() as Stream;
			if (stream == null)
			{
				return 1;
			}
			stream.Seek((long)offset, origin);
			return 0;
		}

		/// <summary>
		/// Returns the streams current position
		/// </summary>
		static int streamTell(fi_handle handle)
		{
			Stream stream = handle.GetObject() as Stream;
			if (stream == null)
			{
				return -1;
			}
			return (int)stream.Position;
		}
	}
}

namespace FreeImageAPI.Metadata
{
	/// <summary>
	/// Class handling metadata of a FreeImage bitmap.
	/// </summary>
	public class ImageMetadata : IEnumerable, IComparable, IComparable<ImageMetadata>
	{
		private readonly List<MetadataModel> data;
		private readonly FIBITMAP dib;
		private bool hideEmptyModels;

		/// <summary>
		/// Initializes a new instance based on the specified <see cref="FIBITMAP"/>,
		/// showing all known models.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public ImageMetadata(FIBITMAP dib) : this(dib, false) { }

		/// <summary>
		/// Initializes a new instance based on the specified <see cref="FIBITMAP"/>,
		/// showing or hiding empry models.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="hideEmptyModels">When <b>true</b>, empty metadata models
		/// will be hidden until a tag to this model is added.</param>
		public ImageMetadata(FIBITMAP dib, bool hideEmptyModels)
		{
			if (dib.IsNull) throw new ArgumentNullException("dib");
			data = new List<MetadataModel>(FreeImage.FREE_IMAGE_MDMODELS.Length);
			this.dib = dib;
			this.hideEmptyModels = hideEmptyModels;

			foreach (Type exportedType in Assembly.GetAssembly(this.GetType()).GetExportedTypes())
			{
				if (exportedType.IsClass &&
					exportedType.IsPublic &&
					exportedType.BaseType != null &&
					exportedType.BaseType == typeof(MetadataModel))
				{
					ConstructorInfo constructorInfo = exportedType.GetConstructor(new Type[] { typeof(FIBITMAP) });
					if (constructorInfo != null)
					{
						MetadataModel model = (MetadataModel)constructorInfo.Invoke(new object[] { dib });
						if (model != null)
						{
							data.Add(model);
						}
					}
				}
			}
			data.Capacity = data.Count;
		}

		/// <summary>
		/// Gets or sets the <see cref="MetadataModel"/> of the specified type.
		/// <para>In case the getter returns <c>null</c> the model is not contained
		/// by the list.</para>
		/// <para><c>null</c> can be used calling the setter to destroy the model.</para>
		/// </summary>
		/// <param name="model">Type of the model.</param>
		/// <returns>The <see cref="FreeImageAPI.Metadata.MetadataModel"/> object of the specified type.</returns>
		public MetadataModel this[FREE_IMAGE_MDMODEL model]
		{
			get
			{
				for (int i = 0; i < data.Count; i++)
				{
					if (data[i].Model == model)
					{
						if (!data[i].Exists && hideEmptyModels)
						{
							return null;
						}
						return data[i];
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="FreeImageAPI.Metadata.MetadataModel"/> at the specified index.
		/// <para>In case the getter returns <c>null</c> the model is not contained
		/// by the list.</para>
		/// <para><c>null</c> can be used calling the setter to destroy the model.</para>
		/// </summary>
		/// <param name="index">Index of the <see cref="FreeImageAPI.Metadata.MetadataModel"/> within
		/// this instance.</param>
		/// <returns>The <see cref="FreeImageAPI.Metadata.MetadataModel"/>
		/// object at the specified index.</returns>
		public MetadataModel this[int index]
		{
			get
			{
				if (index < 0 || index >= data.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return (hideEmptyModels && !data[index].Exists) ? null : data[index];
			}
		}

		/// <summary>
		/// Returns a list of all visible
		/// <see cref="FreeImageAPI.Metadata.MetadataModel">MetadataModels</see>.
		/// </summary>
		public List<MetadataModel> List
		{
			get
			{
				if (hideEmptyModels)
				{
					List<MetadataModel> result = new List<MetadataModel>();
					for (int i = 0; i < data.Count; i++)
					{
						if (data[i].Exists)
						{
							result.Add(data[i]);
						}
					}
					return result;
				}
				else
				{
					return data;
				}
			}
		}

		/// <summary>
		/// Adds new tag to the bitmap or updates its value in case it already exists.
		/// <see cref="FreeImageAPI.Metadata.MetadataTag.Key"/> will be used as key.
		/// </summary>
		/// <param name="tag">The tag to add or update.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="tag"/> is null.</exception>
		public bool AddTag(MetadataTag tag)
		{
			for (int i = 0; i < data.Count; i++)
			{
				if (tag.Model == data[i].Model)
				{
					return data[i].AddTag(tag);
				}
			}
			return false;
		}

		/// <summary>
		/// Returns the number of visible
		/// <see cref="FreeImageAPI.Metadata.MetadataModel">MetadataModels</see>.
		/// </summary>
		public int Count
		{
			get
			{
				if (hideEmptyModels)
				{
					int count = 0;
					for (int i = 0; i < data.Count; i++)
					{
						if (data[i].Exists)
						{
							count++;
						}
					}
					return count;
				}
				else
				{
					return data.Count;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether empty
		/// <see cref="FreeImageAPI.Metadata.MetadataModel">MetadataModels</see> are hidden.
		/// </summary>
		public bool HideEmptyModels
		{
			get
			{
				return hideEmptyModels;
			}
			set
			{
				hideEmptyModels = value;
			}
		}

		/// <summary>
		/// Retrieves an object that can iterate through the individual
		/// <see cref="FreeImageAPI.Metadata.MetadataModel">MetadataModels</see>
		/// in this <see cref="ImageMetadata"/>.
		/// </summary>
		/// <returns>An <see cref="IEnumerator"/> for this <see cref="ImageMetadata"/>.</returns>
		public IEnumerator GetEnumerator()
		{
			if (hideEmptyModels)
			{
				List<MetadataModel> tempList = new List<MetadataModel>(data.Count);
				for (int i = 0; i < data.Count; i++)
				{
					if (data[i].Exists)
					{
						tempList.Add(data[i]);
					}
				}
				return tempList.GetEnumerator();
			}
			else
			{
				return data.GetEnumerator();
			}
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="ImageMetadata"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is ImageMetadata))
			{
				throw new ArgumentException();
			}
			return CompareTo((ImageMetadata)obj);
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="ImageMetadata"/> object.
		/// </summary>
		/// <param name="other">A <see cref="ImageMetadata"/> to compare.</param>
		/// <returns>A signed number indicating the relative values of this instance
		/// and <paramref name="other"/>.</returns>
		public int CompareTo(ImageMetadata other)
		{
			return this.dib.CompareTo(other.dib);
		}
	}
}

namespace FreeImageAPI.Plugins
{
	/// <summary>
	/// Class representing own FreeImage-Plugins.
	/// </summary>
	/// <remarks>
	/// FreeImages itself is plugin based. Each supported format is integrated by a seperat plugin,
	/// that handles loading, saving, descriptions, identifing ect.
	/// And of course the user can create own plugins and use them in FreeImage.
	/// To do that the above mentioned predefined methodes need to be implemented.
	/// <para/>
	/// The class below handles the creation of such a plugin. The class itself is abstract
	/// as well as some core functions that need to be implemented.
	/// The class can be used to enable or disable the plugin in FreeImage after regististration or
	/// retrieve the formatid, assigned by FreeImage.
	/// The class handles the callback functions, garbage collector and pointer operation to make
	/// the implementation as user friendly as possible.
	/// <para/>
	/// How to:
	/// There are two functions that need to be implemented:
	/// <see cref="FreeImageAPI.Plugins.LocalPlugin.GetImplementedMethods"/> and
	/// <see cref="FreeImageAPI.Plugins.LocalPlugin.FormatProc"/>.
	/// <see cref="FreeImageAPI.Plugins.LocalPlugin.GetImplementedMethods"/> is used by the constructor
	/// of the abstract class. FreeImage wants a list of the implemented functions. Each function is
	/// represented by a function pointer (a .NET <see cref="System.Delegate"/>). In case a function
	/// is not implemented FreeImage recieves an empty <b>delegate</b>). To tell the constructor
	/// which functions have been implemented the information is represented by a disjunction of
	/// <see cref="FreeImageAPI.Plugins.LocalPlugin.MethodFlags"/>.
	/// <para/>
	/// For example:
	///		return MethodFlags.LoadProc | MethodFlags.SaveProc;
	/// <para/>
	/// The above statement means that LoadProc and SaveProc have been implemented by the user.
	/// Keep in mind, that each function has a standard implementation that has static return
	/// values that may cause errors if listed in
	/// <see cref="FreeImageAPI.Plugins.LocalPlugin.GetImplementedMethods"/> without a real implementation.
	/// <para/>
	/// <see cref="FreeImageAPI.Plugins.LocalPlugin.FormatProc"/> is used by some checks of FreeImage and
	/// must be implemented. <see cref="FreeImageAPI.Plugins.LocalPlugin.LoadProc"/> for example can be
	/// implemented if the plugin supports reading, but it doesn't have to, the plugin could only
	/// be used to save an already loaded bitmap in a special format.
	/// </remarks>
	public abstract class LocalPlugin
	{
		/// <summary>
		/// Struct containing function pointers.
		/// </summary>
		private Plugin plugin;
		/// <summary>
		/// Delegate for register callback by FreeImage.
		/// </summary>
		private InitProc initProc;
		/// <summary>
		/// GCHandles to prevent the garbage collector from chaning function addresses.
		/// </summary>
		private GCHandle[] handles = new GCHandle[16];
		/// <summary>
		/// The format id assiged to the plugin.
		/// </summary>
		protected FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
		/// <summary>
		/// When true the plugin was registered successfully else false.
		/// </summary>
		protected readonly bool registered = false;
		/// <summary>
		/// A copy of the functions used to register.
		/// </summary>
		protected readonly MethodFlags implementedMethods;

		/// <summary>
		/// MethodFlags defines values to fill a bitfield telling which
		/// functions have been implemented by a plugin.
		/// </summary>
		[Flags]
		protected enum MethodFlags
		{
			/// <summary>
			/// No mothods implemented.
			/// </summary>
			None = 0x0,

			/// <summary>
			/// DescriptionProc has been implemented.
			/// </summary>
			DescriptionProc = 0x1,

			/// <summary>
			/// ExtensionListProc has been implemented.
			/// </summary>
			ExtensionListProc = 0x2,

			/// <summary>
			/// RegExprProc has been implemented.
			/// </summary>
			RegExprProc = 0x4,

			/// <summary>
			/// OpenProc has been implemented.
			/// </summary>
			OpenProc = 0x8,

			/// <summary>
			/// CloseProc has been implemented.
			/// </summary>
			CloseProc = 0x10,

			/// <summary>
			/// PageCountProc has been implemented.
			/// </summary>
			PageCountProc = 0x20,

			/// <summary>
			/// PageCapabilityProc has been implemented.
			/// </summary>
			PageCapabilityProc = 0x40,

			/// <summary>
			/// LoadProc has been implemented.
			/// </summary>
			LoadProc = 0x80,

			/// <summary>
			/// SaveProc has been implemented.
			/// </summary>
			SaveProc = 0x100,

			/// <summary>
			/// ValidateProc has been implemented.
			/// </summary>
			ValidateProc = 0x200,

			/// <summary>
			/// MimeProc has been implemented.
			/// </summary>
			MimeProc = 0x400,

			/// <summary>
			/// SupportsExportBPPProc has been implemented.
			/// </summary>
			SupportsExportBPPProc = 0x800,

			/// <summary>
			/// SupportsExportTypeProc has been implemented.
			/// </summary>
			SupportsExportTypeProc = 0x1000,

			/// <summary>
			/// SupportsICCProfilesProc has been implemented.
			/// </summary>
			SupportsICCProfilesProc = 0x2000
		}

		// Functions that must be implemented.

		/// <summary>
		/// Function that returns a bitfield containing the
		/// implemented methods.
		/// </summary>
		/// <returns>Bitfield of the implemented methods.</returns>
		protected abstract MethodFlags GetImplementedMethods();

		/// <summary>
		/// Implementation of <b>FormatProc</b>
		/// </summary>
		/// <returns>A string containing the plugins format.</returns>
		protected abstract string FormatProc();

		// Functions that can be implemented.

		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual string DescriptionProc() { return ""; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual string ExtensionListProc() { return ""; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual string RegExprProc() { return ""; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual IntPtr OpenProc(ref FreeImageIO io, fi_handle handle, bool read) { return IntPtr.Zero; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual void CloseProc(ref FreeImageIO io, fi_handle handle, IntPtr data) { }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual int PageCountProc(ref FreeImageIO io, fi_handle handle, IntPtr data) { return 0; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual int PageCapabilityProc(ref FreeImageIO io, fi_handle handle, IntPtr data) { return 0; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual FIBITMAP LoadProc(ref FreeImageIO io, fi_handle handle, int page, int flags, IntPtr data) { return FIBITMAP.Zero; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual bool SaveProc(ref FreeImageIO io, FIBITMAP dib, fi_handle handle, int page, int flags, IntPtr data) { return false; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual bool ValidateProc(ref FreeImageIO io, fi_handle handle) { return false; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual string MimeProc() { return ""; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual bool SupportsExportBPPProc(int bpp) { return false; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual bool SupportsExportTypeProc(FREE_IMAGE_TYPE type) { return false; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual bool SupportsICCProfilesProc() { return false; }

		/// <summary>
		/// The constructor automatically registeres the plugin in FreeImage.
		/// To do this it prepares a FreeImage defined structure with function pointers
		/// to the implemented functions or null if not implemented.
		/// Before registing the functions they are pinned in memory so the garbage collector
		/// can't move them around in memory after we passed there addresses to FreeImage.
		/// </summary>
		public LocalPlugin()
		{
			int i = 0;
			implementedMethods = GetImplementedMethods();

			if ((implementedMethods & MethodFlags.DescriptionProc) != 0)
			{
				plugin.descriptionProc = new DescriptionProc(DescriptionProc);
				handles[i++] = GetHandle(plugin.descriptionProc);
			}
			if ((implementedMethods & MethodFlags.ExtensionListProc) != 0)
			{
				plugin.extensionListProc = new ExtensionListProc(ExtensionListProc);
				handles[i++] = GetHandle(plugin.extensionListProc);
			}
			if ((implementedMethods & MethodFlags.RegExprProc) != 0)
			{
				plugin.regExprProc = new RegExprProc(RegExprProc);
				handles[i++] = GetHandle(plugin.regExprProc);
			}
			if ((implementedMethods & MethodFlags.OpenProc) != 0)
			{
				plugin.openProc = new OpenProc(OpenProc);
				handles[i++] = GetHandle(plugin.openProc);
			}
			if ((implementedMethods & MethodFlags.CloseProc) != 0)
			{
				plugin.closeProc = new CloseProc(CloseProc);
				handles[i++] = GetHandle(plugin.closeProc);
			}
			if ((implementedMethods & MethodFlags.PageCountProc) != 0)
			{
				plugin.pageCountProc = new PageCountProc(PageCountProc);
				handles[i++] = GetHandle(plugin.pageCountProc);
			}
			if ((implementedMethods & MethodFlags.PageCapabilityProc) != 0)
			{
				plugin.pageCapabilityProc = new PageCapabilityProc(PageCapabilityProc);
				handles[i++] = GetHandle(plugin.pageCapabilityProc);
			}
			if ((implementedMethods & MethodFlags.LoadProc) != 0)
			{
				plugin.loadProc = new LoadProc(LoadProc);
				handles[i++] = GetHandle(plugin.loadProc);
			}
			if ((implementedMethods & MethodFlags.SaveProc) != 0)
			{
				plugin.saveProc = new SaveProc(SaveProc);
				handles[i++] = GetHandle(plugin.saveProc);
			}
			if ((implementedMethods & MethodFlags.ValidateProc) != 0)
			{
				plugin.validateProc = new ValidateProc(ValidateProc);
				handles[i++] = GetHandle(plugin.validateProc);
			}
			if ((implementedMethods & MethodFlags.MimeProc) != 0)
			{
				plugin.mimeProc = new MimeProc(MimeProc);
				handles[i++] = GetHandle(plugin.mimeProc);
			}
			if ((implementedMethods & MethodFlags.SupportsExportBPPProc) != 0)
			{
				plugin.supportsExportBPPProc = new SupportsExportBPPProc(SupportsExportBPPProc);
				handles[i++] = GetHandle(plugin.supportsExportBPPProc);
			}
			if ((implementedMethods & MethodFlags.SupportsExportTypeProc) != 0)
			{
				plugin.supportsExportTypeProc = new SupportsExportTypeProc(SupportsExportTypeProc);
				handles[i++] = GetHandle(plugin.supportsExportTypeProc);
			}
			if ((implementedMethods & MethodFlags.SupportsICCProfilesProc) != 0)
			{
				plugin.supportsICCProfilesProc = new SupportsICCProfilesProc(SupportsICCProfilesProc);
				handles[i++] = GetHandle(plugin.supportsICCProfilesProc);
			}

			// FormatProc is always implemented
			plugin.formatProc = new FormatProc(FormatProc);
			handles[i++] = GetHandle(plugin.formatProc);

			// InitProc is the register call back.
			initProc = new InitProc(RegisterProc);
			handles[i++] = GetHandle(initProc);

			// Register the plugin. The result will be saved and can be accessed later.
			registered = FreeImage.RegisterLocalPlugin(initProc, null, null, null, null) != FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			if (registered)
			{
				PluginRepository.RegisterLocalPlugin(this);
			}
		}

		/// <summary>
		/// Releases all resources used by the instance.
		/// </summary>
		~LocalPlugin()
		{
			for (int i = 0; i < handles.Length; i++)
			{
				if (handles[i].IsAllocated)
				{
					handles[i].Free();
				}
			}
		}

		private GCHandle GetHandle(Delegate d)
		{
			return GCHandle.Alloc(d, GCHandleType.Normal);
		}

		private void RegisterProc(ref Plugin plugin, int format_id)
		{
			// Copy the function pointers
			plugin = this.plugin;
			// Retrieve the format if assigned to this plugin by FreeImage.
			format = (FREE_IMAGE_FORMAT)format_id;
		}

		/// <summary>
		/// Gets or sets if the plugin is enabled.
		/// </summary>
		public bool Enabled
		{
			get
			{
				if (registered)
				{
					return (FreeImage.IsPluginEnabled(format) > 0);
				}
				else
				{
					throw new ObjectDisposedException("plugin not registered");
				}
			}
			set
			{
				if (registered)
				{
					FreeImage.SetPluginEnabled(format, value);
				}
				else
				{
					throw new ObjectDisposedException("plugin not registered");
				}
			}
		}

		/// <summary>
		/// Gets if the plugin was registered successfully.
		/// </summary>
		public bool Registered
		{
			get { return registered; }
		}

		/// <summary>
		/// Gets the <see cref="FREE_IMAGE_FORMAT"/> FreeImage assigned to this plugin.
		/// </summary>
		public FREE_IMAGE_FORMAT Format
		{
			get
			{
				return format;
			}
		}

		/// <summary>
		/// Reads from an unmanaged stream.
		/// </summary>
		protected unsafe int Read(FreeImageIO io, fi_handle handle, uint size, uint count, ref byte[] buffer)
		{
			fixed (byte* ptr = buffer)
			{
				return (int)io.readProc(new IntPtr(ptr), size, count, handle);
			}
		}

		/// <summary>
		/// Reads a single byte from an unmanaged stream.
		/// </summary>
		protected unsafe int ReadByte(FreeImageIO io, fi_handle handle)
		{
			byte buffer = 0;
			return (int)io.readProc(new IntPtr(&buffer), 1, 1, handle) > 0 ? buffer : -1;
		}

		/// <summary>
		/// Writes to an unmanaged stream.
		/// </summary>
		protected unsafe int Write(FreeImageIO io, fi_handle handle, uint size, uint count, ref byte[] buffer)
		{
			fixed (byte* ptr = buffer)
			{
				return (int)io.writeProc(new IntPtr(ptr), size, count, handle);
			}
		}

		/// <summary>
		/// Writes a single byte to an unmanaged stream.
		/// </summary>
		protected unsafe int WriteByte(FreeImageIO io, fi_handle handle, byte value)
		{
			return (int)io.writeProc(new IntPtr(&value), 1, 1, handle);
		}

		/// <summary>
		/// Seeks in an unmanaged stream.
		/// </summary>
		protected int Seek(FreeImageIO io, fi_handle handle, int offset, SeekOrigin origin)
		{
			return io.seekProc(handle, offset, origin);
		}

		/// <summary>
		/// Retrieves the position of an unmanaged stream.
		/// </summary>
		protected int Tell(FreeImageIO io, fi_handle handle)
		{
			return io.tellProc(handle);
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Represents unmanaged memory, containing an array of a given structure.
	/// </summary>
	/// <typeparam name="T">Structuretype represented by the instance.</typeparam>
	/// <remarks>
	/// <see cref="System.Boolean"/> and <see cref="System.Char"/> can not be marshalled.
	/// <para/>
	/// Use <see cref="System.Int32"/> instead of <see cref="System.Boolean"/> and
	/// <see cref="System.Byte"/> instead of <see cref="System.Char"/>.
	/// </remarks>
	public unsafe class MemoryArray<T> : ICloneable, ICollection, IEnumerable<T>, IEquatable<MemoryArray<T>> where T : struct
	{
		/// <summary>
		/// Baseaddress of the wrapped memory.
		/// </summary>
		protected readonly byte* baseAddress;

		/// <summary>
		/// Number of elements being wrapped.
		/// </summary>
		protected readonly int length;

		/// <summary>
		/// Size, in bytes, of each element.
		/// </summary>
		protected readonly int size;

		/// <summary>
		/// Array of <b>T</b> containing a single element.
		/// The array is used as a workaround, because there are no pointer for generic types.
		/// </summary>
		protected readonly T[] buffer;

		/// <summary>
		/// Pointer to the element of <b>buffer</b>.
		/// </summary>
		protected readonly byte* ptr;

		/// <summary>
		/// Handle for pinning <b>buffer</b>.
		/// </summary>
		protected readonly GCHandle handle;

		/// <summary>
		/// Indicates whether the wrapped memory is handled like a bitfield.
		/// </summary>
		protected readonly bool isOneBit;

		/// <summary>
		/// Indicates whther the wrapped memory is handles like 4-bit blocks.
		/// </summary>
		protected readonly bool isFourBit;

		/// <summary>
		/// An object that can be used to synchronize access to the <see cref="MemoryArray&lt;T&gt;"/>.
		/// </summary>
		protected object syncRoot = null;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		protected MemoryArray()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryArray&lt;T&gt;"/> class. 
		/// </summary>
		/// <param name="baseAddress">Address of the memory block.</param>
		/// <param name="length">Length of the array.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="baseAddress"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="length"/> is less or equal zero.</exception>
		/// <exception cref="NotSupportedException">
		/// The type is not supported.</exception>
		public MemoryArray(IntPtr baseAddress, int length)
			: this(baseAddress.ToPointer(), length)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryArray&lt;T&gt;"/> class. 
		/// </summary>
		/// <param name="baseAddress">Address of the memory block.</param>
		/// <param name="length">Length of the array.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="baseAddress"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="length"/> is less or equal zero.</exception>
		/// <exception cref="NotSupportedException">
		/// The type is not supported.</exception>
		public MemoryArray(void* baseAddress, int length)
		{
			if (typeof(T) == typeof(FI1BIT))
			{
				isOneBit = true;
			}
			else if (typeof(T) == typeof(FI4BIT))
			{
				isFourBit = true;
			}
			else
			{
				T[] dummy = new T[2];
				long marshalledSize = Marshal.SizeOf(typeof(T));
				long structureSize =
					Marshal.UnsafeAddrOfPinnedArrayElement(dummy, 1).ToInt64() -
					Marshal.UnsafeAddrOfPinnedArrayElement(dummy, 0).ToInt64();
				if (marshalledSize != structureSize)
				{
					throw new NotSupportedException(
						"The desired type can not be handled, " +
						"because it's managed and unmanaged size in bytes are different.");
				}
			}

			if (baseAddress == null)
			{
				throw new ArgumentNullException("baseAddress");
			}
			if (length < 1)
			{
				throw new ArgumentOutOfRangeException("length");
			}

			this.baseAddress = (byte*)baseAddress;
			this.length = (int)length;

			if (!isOneBit && !isFourBit)
			{
				this.size = Marshal.SizeOf(typeof(T));
				// Create an array containing a single element.
				// Due to the fact, that it's not possible to create pointers
				// of generic types, an array is used to obtain the memory
				// address of an element of T.
				this.buffer = new T[1];
				// The array is pinned immediately to prevent the GC from
				// moving it to a different position in memory.
				this.handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
				// The array and its content have beed pinned, so that its address
				// can be safely requested and stored for the whole lifetime
				// of the instace.
				this.ptr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(this.buffer, 0);
			}
		}

		/// <summary>
		/// Frees the allocated <see cref="System.Runtime.InteropServices.GCHandle"/>.
		/// </summary>
		~MemoryArray()
		{
			if (handle.IsAllocated)
			{
				handle.Free();
			}
		}

		/// <summary>
		/// Tests whether two specified <see cref="MemoryArray&lt;T&gt;"/> structures are equivalent.
		/// </summary>
		/// <param name="left">The <see cref="MemoryArray&lt;T&gt;"/> that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="MemoryArray&lt;T&gt;"/> that is to the right of the equality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="MemoryArray&lt;T&gt;"/> structures are equal; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(MemoryArray<T> left, MemoryArray<T> right)
		{
			if (object.ReferenceEquals(left, right))
			{
				return true;
			}
			if (object.ReferenceEquals(right, null) ||
				object.ReferenceEquals(left, null) ||
				(left.length != right.length))
			{
				return false;
			}
			if (left.baseAddress == right.baseAddress)
			{
				return true;
			}
			return FreeImage.CompareMemory(left.baseAddress, right.baseAddress, (uint)left.length);
		}

		/// <summary>
		/// Tests whether two specified <see cref="MemoryArray&lt;T&gt;"/> structures are different.
		/// </summary>
		/// <param name="left">The <see cref="MemoryArray&lt;T&gt;"/> that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="MemoryArray&lt;T&gt;"/> that is to the right of the inequality operator.</param>
		/// <returns>
		/// <b>true</b> if the two <see cref="MemoryArray&lt;T&gt;"/> structures are different; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(MemoryArray<T> left, MemoryArray<T> right)
		{
			return (!(left == right));
		}

		/// <summary>
		/// Gets the value at the specified position.
		/// </summary>
		/// <param name="index">A 32-bit integer that represents the position
		/// of the array element to get.</param>
		/// <returns>The value at the specified position.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is outside the range of valid indexes
		/// for the unmanaged array.</exception>
		public T GetValue(int index)
		{
			if ((index >= this.length) || (index < 0))
			{
				throw new ArgumentOutOfRangeException("index");
			}

			return GetValueInternal(index);
		}

		private T GetValueInternal(int index)
		{
			if (isOneBit)
			{
				return (T)(object)(FI1BIT)(((baseAddress[index / 8] & ((1 << (7 - (index % 8))))) == 0) ? 0 : 1);
			}
			else if (isFourBit)
			{
				return (T)(object)(FI4BIT)(((index % 2) == 0) ? (baseAddress[index / 2] >> 4) : (baseAddress[index / 2] & 0x0F));
			}
			else
			{
				CopyMemory(ptr, baseAddress + (index * size), size);
				return buffer[0];
			}
		}

		/// <summary>
		/// Sets a value to the element at the specified position.
		/// </summary>
		/// <param name="value">The new value for the specified element.</param>
		/// <param name="index">A 32-bit integer that represents the
		/// position of the array element to set.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is outside the range of valid indexes
		/// for the unmanaged array.</exception>
		public void SetValue(T value, int index)
		{
			if ((index >= this.length) || (index < 0))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			SetValueInternal(value, index);
		}

		private void SetValueInternal(T value, int index)
		{
			if (isOneBit)
			{
				if ((FI1BIT)(object)value != 0)
				{
					baseAddress[index / 8] |= (byte)(1 << (7 - (index % 8)));
				}
				else
				{
					baseAddress[index / 8] &= (byte)(~(1 << (7 - (index % 8))));
				}
			}
			else if (isFourBit)
			{
				if ((index % 2) == 0)
				{
					baseAddress[index / 2] = (byte)((baseAddress[index / 2] & 0x0F) | ((FI4BIT)(object)value << 4));
				}
				else
				{
					baseAddress[index / 2] = (byte)((baseAddress[index / 2] & 0xF0) | ((FI4BIT)(object)value & 0x0F));
				}
			}
			else
			{
				buffer[0] = value;
				CopyMemory(baseAddress + (index * size), ptr, size);
			}
		}

		/// <summary>
		/// Gets the values at the specified position and length.
		/// </summary>
		/// <param name="index">A 32-bit integer that represents the position
		/// of the array elements to get.</param>
		/// <param name="length"> A 32-bit integer that represents the length
		/// of the array elements to get.</param>
		/// <returns>The values at the specified position and length.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is outside the range of valid indexes
		/// for the unmanaged array or <paramref name="length"/> is greater than the number of elements
		/// from <paramref name="index"/> to the end of the unmanaged array.</exception>
		public T[] GetValues(int index, int length)
		{
			if ((index >= this.length) || (index < 0))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (((index + length) > this.length) || (length < 1))
			{
				throw new ArgumentOutOfRangeException("length");
			}

			T[] data = new T[length];
			if (isOneBit || isFourBit)
			{
				for (int i = 0; i < length; i++)
				{
					data[i] = GetValueInternal(i);
				}
			}
			else
			{
				GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
				byte* dst = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
				CopyMemory(dst, baseAddress + (size * index), size * length);
				handle.Free();
			}
			return data;
		}

		/// <summary>
		/// Sets the values at the specified position.
		/// </summary>
		/// <param name="values">An array containing the new values for the specified elements.</param>
		/// <param name="index">A 32-bit integer that represents the position
		/// of the array elements to set.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="values"/> is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is outside the range of valid indexes
		/// for the unmanaged array or <paramref name="values.Length"/> is greater than the number of elements
		/// from <paramref name="index"/> to the end of the array.</exception>
		public void SetValues(T[] values, int index)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if ((index >= this.length) || (index < 0))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if ((index + values.Length) > this.length)
			{
				throw new ArgumentOutOfRangeException("values.Length");
			}

			if (isOneBit || isFourBit)
			{
				for (int i = 0; i != values.Length; )
				{
					SetValueInternal(values[i++], index++);
				}
			}
			else
			{
				GCHandle handle = GCHandle.Alloc(values, GCHandleType.Pinned);
				byte* src = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(values, 0);
				CopyMemory(baseAddress + (index * size), src, size * length);
				handle.Free();
			}
		}

		/// <summary>
		/// Copies the entire array to a compatible one-dimensional <see cref="System.Array"/>,
		/// starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination
		/// of the elements copied from <see cref="MemoryArray&lt;T&gt;"/>.
		/// The <see cref="System.Array"/> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array"/>
		/// at which copying begins.</param>
		public void CopyTo(Array array, int index)
		{
			if (!(array is T[]))
			{
				throw new InvalidCastException("array");
			}
			try
			{
				CopyTo((T[])array, 0, index, length);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new ArgumentException(ex.Message, ex);
			}
		}

		/// <summary>
		/// Copies a range of elements from the unmanaged array starting at the specified
		/// <typeparamref name="sourceIndex"/> and pastes them to <paramref name="array"/>
		/// starting at the specified <paramref name="destinationIndex"/>.
		/// The length and the indexes are specified as 32-bit integers.
		/// </summary>
		/// <param name="array">The array that receives the data.</param>
		/// <param name="sourceIndex">A 32-bit integer that represents the index
		/// in the unmanaged array at which copying begins.</param>
		/// <param name="destinationIndex">A 32-bit integer that represents the index in
		/// the destination array at which storing begins.</param>
		/// <param name="length">A 32-bit integer that represents the number of elements to copy.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="array"/> is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="sourceIndex"/> is outside the range of valid indexes
		/// for the unmanaged array or <paramref name="length"/> is greater than the number of elements
		/// from <paramref name="index"/> to the end of the unmanaged array
		/// <para>-or-</para>
		/// <paramref name="destinationIndex"/> is outside the range of valid indexes
		/// for the array or <paramref name="length"/> is greater than the number of elements
		/// from <paramref name="index"/> to the end of the array.
		/// </exception>
		public void CopyTo(T[] array, int sourceIndex, int destinationIndex, int length)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if ((sourceIndex >= this.length) || (sourceIndex < 0))
			{
				throw new ArgumentOutOfRangeException("sourceIndex");
			}
			if ((destinationIndex >= array.Length) || (destinationIndex < 0))
			{
				throw new ArgumentOutOfRangeException("destinationIndex");
			}
			if ((sourceIndex + length > this.length) ||
				(destinationIndex + length > array.Length) ||
				(length < 1))
			{
				throw new ArgumentOutOfRangeException("length");
			}

			if (isOneBit || isFourBit)
			{
				for (int i = 0; i != length; i++)
				{
					array[destinationIndex++] = GetValueInternal(sourceIndex++);
				}
			}
			else
			{
				GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
				byte* dst = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(array, destinationIndex);
				CopyMemory(dst, baseAddress + (size * sourceIndex), size * length);
				handle.Free();
			}
		}

		/// <summary>
		/// Copies a range of elements from the array starting at the specified
		/// <typeparamref name="sourceIndex"/> and pastes them to the unmanaged array
		/// starting at the specified <paramref name="destinationIndex"/>.
		/// The length and the indexes are specified as 32-bit integers.
		/// </summary>
		/// <param name="array">The array that holds the data.</param>
		/// <param name="sourceIndex">A 32-bit integer that represents the index
		/// in the array at which copying begins.</param>
		/// <param name="destinationIndex">A 32-bit integer that represents the index in
		/// the unmanaged array at which storing begins.</param>
		/// <param name="length">A 32-bit integer that represents the number of elements to copy.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="array"/> is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="sourceIndex"/> is outside the range of valid indexes
		/// for the array or <paramref name="length"/> is greater than the number of elements
		/// from <paramref name="index"/> to the end of the array
		/// <para>-or-</para>
		/// <paramref name="destinationIndex"/> is outside the range of valid indexes
		/// for the unmanaged array or <paramref name="length"/> is greater than the number of elements
		/// from <paramref name="index"/> to the end of the unmanaged array.
		/// </exception>
		public void CopyFrom(T[] array, int sourceIndex, int destinationIndex, int length)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if ((destinationIndex >= this.length) || (destinationIndex < 0))
			{
				throw new ArgumentOutOfRangeException("destinationIndex");
			}
			if ((sourceIndex >= array.Length) || (sourceIndex < 0))
			{
				throw new ArgumentOutOfRangeException("sourceIndex");
			}
			if ((destinationIndex + length > this.length) ||
				(sourceIndex + length > array.Length) ||
				(length < 1))
			{
				throw new ArgumentOutOfRangeException("length");
			}

			if (isOneBit || isFourBit)
			{
				for (int i = 0; i != length; i++)
				{
					SetValueInternal(array[sourceIndex++], destinationIndex++);
				}
			}
			else
			{
				GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
				byte* src = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(array, sourceIndex);
				CopyMemory(baseAddress + (size * destinationIndex), src, size * length);
				handle.Free();
			}
		}

		/// <summary>
		/// Returns the represented block of memory as an array of <see cref="Byte"/>.
		/// </summary>
		/// <returns>The represented block of memory.</returns>
		public byte[] ToByteArray()
		{
			byte[] result;
			if (isOneBit)
			{
				result = new byte[(length + 7) / 8];
			}
			else if (isFourBit)
			{
				result = new byte[(length + 3) / 4];
			}
			else
			{
				result = new byte[size * length];
			}
			fixed (byte* dst = result)
			{
				CopyMemory(dst, baseAddress, result.Length);
			}
			return result;
		}

		/// <summary>
		/// Gets or sets the value at the specified position in the array.
		/// </summary>
		/// <param name="index">A 32-bit integer that represents the position
		/// of the array element to get.</param>
		/// <returns>The value at the specified position in the array.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is outside the range of valid indexes
		/// for the unmanaged array.</exception>
		public T this[int index]
		{
			get
			{
				return GetValue(index);
			}
			set
			{
				SetValue(value, index);
			}
		}

		/// <summary>
		/// Gets or sets the values of the unmanaged array.
		/// </summary>
		public T[] Data
		{
			get
			{
				return GetValues(0, length);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length != length)
				{
					throw new ArgumentOutOfRangeException("value.Lengt");
				}
				SetValues(value, 0);
			}
		}

		/// <summary>
		/// Gets the length of the unmanaged array.
		/// </summary>
		public int Length
		{
			get
			{
				return length;
			}
		}

		/// <summary>
		/// Gets the base address of the represented memory block.
		/// </summary>
		public IntPtr BaseAddress
		{
			get
			{
				return new IntPtr(baseAddress);
			}
		}

		/// <summary>
		/// Creates a shallow copy of the <see cref="MemoryArray&lt;T&gt;"/>.
		/// </summary>
		/// <returns>A shallow copy of the <see cref="MemoryArray&lt;T&gt;"/>.</returns>
		public object Clone()
		{
			return new MemoryArray<T>(baseAddress, length);
		}

		/// <summary>
		/// Gets a 32-bit integer that represents the total number of elements
		/// in the <see cref="MemoryArray&lt;T&gt;"/>.
		/// </summary>
		public int Count
		{
			get { return length; }
		}

		/// <summary>
		/// Gets a value indicating whether access to the <see cref="MemoryArray&lt;T&gt;"/>
		/// is synchronized (thread safe).
		/// </summary>
		public bool IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="MemoryArray&lt;T&gt;"/>.
		/// </summary>
		public object SyncRoot
		{
			get
			{
				if (syncRoot == null)
				{
					System.Threading.Interlocked.CompareExchange(ref syncRoot, new object(), null);
				}
				return syncRoot;
			}
		}

		/// <summary>
		/// Retrieves an object that can iterate through the individual
		/// elements in this <see cref="MemoryArray&lt;T&gt;"/>.
		/// </summary>
		/// <returns>An <see cref="IEnumerator"/> for the <see cref="MemoryArray&lt;T&gt;"/>.</returns>
		public IEnumerator GetEnumerator()
		{
			T[] values = GetValues(0, length);
			for (int i = 0; i != values.Length; i++)
			{
				yield return values[i];
			}
		}

		/// <summary>
		/// Retrieves an object that can iterate through the individual
		/// elements in this <see cref="MemoryArray&lt;T&gt;"/>.
		/// </summary>
		/// <returns>An <see cref="IEnumerator&lt;T&gt;"/> for the <see cref="MemoryArray&lt;T&gt;"/>.</returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			T[] values = GetValues(0, length);
			for (int i = 0; i != values.Length; i++)
			{
				yield return values[i];
			}
		}

		/// <summary>
		/// Tests whether the specified <see cref="MemoryArray&lt;T&gt;"/> structure is equivalent to this
		/// <see cref="MemoryArray&lt;T&gt;"/> structure.
		/// </summary>
		/// <param name="obj">The structure to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="MemoryArray&lt;T&gt;"/>
		/// instance equivalent to this <see cref="MemoryArray&lt;T&gt;"/> structure; otherwise,
		/// <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is MemoryArray<T>) && Equals((MemoryArray<T>)obj));
		}

		/// <summary>
		/// Tests whether the specified <see cref="MemoryArray&lt;T&gt;"/> structure is equivalent to this
		/// <see cref="MemoryArray&lt;T&gt;"/> structure.
		/// </summary>
		/// <param name="other">The structure to test.</param>
		/// <returns><b>true</b> if <paramref name="other"/> is equivalent to this
		/// <see cref="MemoryArray&lt;T&gt;"/> structure; otherwise,
		/// <b>false</b>.</returns>
		public bool Equals(MemoryArray<T> other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current <see cref="MemoryArray&lt;T&gt;"/>.</returns>
		public override int GetHashCode()
		{
			return (int)baseAddress ^ length;
		}

		/// <summary>
		/// Copies a block of memory from one location to another.
		/// </summary>
		/// <param name="dest">Pointer to the starting address of the copy destination.</param>
		/// <param name="src">Pointer to the starting address of the block of memory to be copied.</param>
		/// <param name="len">Size of the block of memory to copy, in bytes.</param>
		protected static unsafe void CopyMemory(byte* dest, byte* src, int len)
		{
			if (len >= 0x10)
			{
				do
				{
					*((int*)dest) = *((int*)src);
					*((int*)(dest + 4)) = *((int*)(src + 4));
					*((int*)(dest + 8)) = *((int*)(src + 8));
					*((int*)(dest + 12)) = *((int*)(src + 12));
					dest += 0x10;
					src += 0x10;
				}
				while ((len -= 0x10) >= 0x10);
			}
			if (len > 0)
			{
				if ((len & 8) != 0)
				{
					*((int*)dest) = *((int*)src);
					*((int*)(dest + 4)) = *((int*)(src + 4));
					dest += 8;
					src += 8;
				}
				if ((len & 4) != 0)
				{
					*((int*)dest) = *((int*)src);
					dest += 4;
					src += 4;
				}
				if ((len & 2) != 0)
				{
					*((short*)dest) = *((short*)src);
					dest += 2;
					src += 2;
				}
				if ((len & 1) != 0)
				{
					*dest = *src;
				}
			}
		}
	}
}

namespace FreeImageAPI.Metadata
{
	/// <summary>
	/// Base class that represents a collection of all tags contained in a metadata model.
	/// </summary>
	/// <remarks>
	/// The <b>MetedataModel</b> class is an abstract base class, which is inherited by
	/// several derived classes, one for each existing metadata model.
	/// </remarks> 
	public abstract class MetadataModel : IEnumerable
	{
		/// <summary>
		/// Handle to a FreeImage-bitmap.
		/// </summary>
		private readonly FIBITMAP dib;

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		protected MetadataModel(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			this.dib = dib;
		}

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public abstract FREE_IMAGE_MDMODEL Model
		{
			get;
		}

		/// <summary>
		/// Adds new tag to the bitmap or updates its value in case it already exists.
		/// <see cref="FreeImageAPI.Metadata.MetadataTag.Key"/> will be used as key.
		/// </summary>
		/// <param name="tag">The tag to add or update.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="tag"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// The tags model differs from this instances model.</exception>
		public bool AddTag(MetadataTag tag)
		{
			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}
			if (tag.Model != Model)
			{
				throw new ArgumentException("tag.Model");
			}
			return tag.AddToImage(dib);
		}

		/// <summary>
		/// Adds a list of tags to the bitmap or updates their values in case they already exist.
		/// <see cref="FreeImageAPI.Metadata.MetadataTag.Key"/> will be used as key.
		/// </summary>
		/// <param name="list">A list of tags to add or update.</param>
		/// <returns>Returns the number of successfully added tags.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="list"/> is null.</exception>
		public int AddTag(IEnumerable<MetadataTag> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			int count = 0;
			foreach (MetadataTag tag in list)
			{
				if (tag.Model == Model && tag.AddToImage(dib))
				{
					count++;
				}
			}
			return count;
		}

		/// <summary>
		/// Removes the specified tag from the bitmap.
		/// </summary>
		/// <param name="key">The key of the tag.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="key"/> is null.</exception>
		public bool RemoveTag(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return FreeImage.SetMetadata(Model, dib, key, FITAG.Zero);
		}

		/// <summary>
		/// Destroys the metadata model
		/// which will remove all tags of this model from the bitmap.
		/// </summary>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool DestoryModel()
		{
			return FreeImage.SetMetadata(Model, dib, null, FITAG.Zero);
		}

		/// <summary>
		/// Returns the specified metadata tag.
		/// </summary>
		/// <param name="key">The key of the tag.</param>
		/// <returns>The metadata tag.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="key"/> is null.</exception>
		public MetadataTag GetTag(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			MetadataTag tag;
			return FreeImage.GetMetadata(Model, dib, key, out tag) ? tag : null;
		}

		/// <summary>
		/// Returns whether the specified tag exists.
		/// </summary>
		/// <param name="key">The key of the tag.</param>
		/// <returns>True in case the tag exists, else false.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="key"/> is null.</exception>
		public bool TagExists(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			MetadataTag tag;
			return FreeImage.GetMetadata(Model, dib, key, out tag);
		}

		/// <summary>
		/// Returns a list of all metadata tags this instance represents.
		/// </summary>
		public List<MetadataTag> List
		{
			get
			{
				List<MetadataTag> list = new List<MetadataTag>((int)FreeImage.GetMetadataCount(Model, dib));
				MetadataTag tag;
				FIMETADATA mdHandle = FreeImage.FindFirstMetadata(Model, dib, out tag);
				if (!mdHandle.IsNull)
				{
					do
					{
						list.Add(tag);
					}
					while (FreeImage.FindNextMetadata(mdHandle, out tag));
					FreeImage.FindCloseMetadata(mdHandle);
				}
				return list;
			}
		}

		/// <summary>
		/// Returns the tag at the given index.
		/// </summary>
		/// <param name="index">Index of the tag to return.</param>
		/// <returns>The tag at the given index.</returns>
		protected MetadataTag GetTagFromIndex(int index)
		{
			if (index >= Count || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			MetadataTag tag;
			int count = 0;
			FIMETADATA mdHandle = FreeImage.FindFirstMetadata(Model, dib, out tag);
			if (!mdHandle.IsNull)
			{
				try
				{
					do
					{
						if (count++ == index)
						{
							break;
						}
					}
					while (FreeImage.FindNextMetadata(mdHandle, out tag));
				}
				finally
				{
					FreeImage.FindCloseMetadata(mdHandle);
				}				
			}
			return tag;
		}

		/// <summary>
		/// Returns the metadata tag at the given index. This operation is slow when accessing all tags.
		/// </summary>
		/// <param name="index">Index of the tag.</param>
		/// <returns>The metadata tag.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is greater or equal <b>Count</b>
		/// or index is less than zero.</exception>
		public MetadataTag this[int index]
		{
			get
			{
				return GetTagFromIndex(index);
			}
		}

		/// <summary>
		/// Retrieves an object that can iterate through the individual MetadataTags in this MetadataModel.
		/// </summary>
		/// <returns>An <see cref="IEnumerator"/> for the
		/// <see cref="FreeImageAPI.Metadata.MetadataModel"/>.</returns>
		public IEnumerator GetEnumerator()
		{
			return List.GetEnumerator();
		}

		/// <summary>
		/// Returns the number of metadata tags this instance represents.
		/// </summary>
		public int Count
		{
			get { return (int)FreeImage.GetMetadataCount(Model, dib); }
		}

		/// <summary>
		/// Returns whether this model exists in the bitmaps metadata structure.
		/// </summary>
		public bool Exists
		{
			get
			{
				return Count > 0;
			}
		}

		/// <summary>
		/// Searches for a pattern in each metadata tag and returns the result as a list.
		/// </summary>
		/// <param name="searchPattern">The regular expression to use for the search.</param>
		/// <param name="flags">A bitfield that controls which fields should be searched in.</param>
		/// <returns>A list containing all found metadata tags.</returns>
		/// <exception cref="ArgumentNullException">
		/// <typeparamref name="searchPattern"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// <typeparamref name="searchPattern"/> is empty.</exception>
		public List<MetadataTag> RegexSearch(string searchPattern, MD_SEARCH_FLAGS flags)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchString");
			}
			if (searchPattern.Length == 0)
			{
				throw new ArgumentException("searchString is empty");
			}
			List<MetadataTag> result = new List<MetadataTag>(Count);
			Regex regex = new Regex(searchPattern);
			List<MetadataTag> list = List;
			foreach (MetadataTag tag in list)
			{
				if (((flags & MD_SEARCH_FLAGS.KEY) > 0) && regex.Match(tag.Key).Success)
				{
					result.Add(tag);
					continue;
				}
				if (((flags & MD_SEARCH_FLAGS.DESCRIPTION) > 0) && regex.Match(tag.Description).Success)
				{
					result.Add(tag);
					continue;
				}
				if (((flags & MD_SEARCH_FLAGS.TOSTRING) > 0) && regex.Match(tag.ToString()).Success)
				{
					result.Add(tag);
					continue;
				}
			}
			result.Capacity = result.Count;
			return result;
		}

		/// <summary>
		/// Converts the model of the MetadataModel object to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			return Model.ToString();
		}
	}
}

	#region Metadata Models

namespace FreeImageAPI.Metadata
{
	/// <summary>
	/// Represents a collection of all tags contained in the metadata model <see cref="FREE_IMAGE_MDMODEL.FIMD_ANIMATION"/>.
	/// </summary>
	public sealed class MDM_ANIMATION : MetadataModel
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_ANIMATION(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_ANIMATION; }
		}
	}

	/// <summary>
	/// Represents a collection of all tags contained in the metadata model <see cref="FREE_IMAGE_MDMODEL.FIMD_COMMENTS"/>.
	/// </summary>
	public sealed class MDM_COMMENTS : MetadataModel
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_COMMENTS(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_COMMENTS; }
		}
	}

	/// <summary>
	/// Represents a collection of all tags contained in the metadata model <see cref="FREE_IMAGE_MDMODEL.FIMD_CUSTOM"/>.
	/// </summary>
	public sealed class MDM_CUSTOM : MetadataModel
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_CUSTOM(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_CUSTOM; }
		}
	}

	/// <summary>
	/// Represents a collection of all tags contained in the metadata model <see cref="FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF"/>.
	/// </summary>
	public sealed class MDM_EXIF_EXIF : MetadataModel
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_EXIF_EXIF(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF; }
		}
	}

	/// <summary>
	/// Represents a collection of all tags contained in the metadata model <see cref="FREE_IMAGE_MDMODEL.FIMD_EXIF_GPS"/>.
	/// </summary>
	public sealed class MDM_EXIF_GPS : MetadataModel
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_EXIF_GPS(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_EXIF_GPS; }
		}
	}

	/// <summary>
	/// Represents a collection of all tags contained in the metadata model <see cref="FREE_IMAGE_MDMODEL.FIMD_EXIF_INTEROP"/>.
	/// </summary>
	public sealed class MDM_INTEROP : MetadataModel
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_INTEROP(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_EXIF_INTEROP; }
		}
	}

	/// <summary>
	/// Represents a collection of all tags contained in the metadata model <see cref="FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN"/>.
	/// </summary>
	public class MDM_MAIN : MetadataModel
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_MAIN(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN; }
		}
	}

	/// <summary>
	/// Represents a collection of all tags contained in the metadata model <see cref="FREE_IMAGE_MDMODEL.FIMD_EXIF_MAKERNOTE"/>.
	/// </summary>
	public sealed class MDM_MAKERNOTE : MetadataModel
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_MAKERNOTE(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_EXIF_MAKERNOTE; }
		}
	}

	/// <summary>
	/// Represents a collection of all tags contained in the metadata model <see cref="FREE_IMAGE_MDMODEL.FIMD_GEOTIFF"/>.
	/// </summary>
	public sealed class MDM_GEOTIFF : MetadataModel
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_GEOTIFF(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_GEOTIFF; }
		}
	}

	/// <summary>
	/// Represents a collection of all tags contained in the metadata model <see cref="FREE_IMAGE_MDMODEL.FIMD_IPTC"/>.
	/// </summary>
	public sealed class MDM_IPTC : MetadataModel
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_IPTC(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_IPTC; }
		}
	}

	/// <summary>
	/// Represents a collection of all tags contained in the metadata model <see cref="FREE_IMAGE_MDMODEL.FIMD_NODATA"/>.
	/// </summary>
	public sealed class MDM_NODATA : MetadataModel
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_NODATA(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_NODATA; }
		}
	}

	/// <summary>
	/// Represents a collection of all tags contained in the metadata model <see cref="FREE_IMAGE_MDMODEL.FIMD_XMP"/>.
	/// </summary>
	public sealed class MDM_XMP : MetadataModel
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_XMP(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_XMP; }
		}
	}
}

	#endregion

namespace FreeImageAPI.Metadata
{
	/// <summary>
	/// Manages metadata objects and operations.
	/// </summary>
	public sealed class MetadataTag : IComparable, IComparable<MetadataTag>, ICloneable, IEquatable<MetadataTag>, IDisposable
	{
		/// <summary>
		/// The encapsulated FreeImage-tag.
		/// </summary>
		internal FITAG tag;
		/// <summary>
		/// The metadata model of <see cref="tag"/>.
		/// </summary>
		private FREE_IMAGE_MDMODEL model;
		/// <summary>
		/// Indicates whether this instance has already been disposed.
		/// </summary>
		private bool disposed = false;
		/// <summary>
		/// Indicates whether this instance was created by FreeImage or
		/// by the user.
		/// </summary>
		private bool selfCreated;
		/// <summary>
		/// List linking metadata-model and Type.
		/// </summary>
		private static readonly Dictionary<FREE_IMAGE_MDTYPE, Type> idList;
		/// <summary>
		/// List linking Type and metadata-model.
		/// </summary>
		private static readonly Dictionary<Type, FREE_IMAGE_MDTYPE> typeList;

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		private MetadataTag()
		{
		}

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="model">The new model the tag should be of.</param>
		public MetadataTag(FREE_IMAGE_MDMODEL model)
		{
			this.model = model;
			tag = FreeImage.CreateTag();
			selfCreated = true;

			if (model == FREE_IMAGE_MDMODEL.FIMD_XMP)
			{
				Key = "XMLPacket";
			}
		}

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="tag">The <see cref="FITAG"/> to represent.</param>
		/// <param name="dib">The bitmap <paramref name="tag"/> was extracted from.</param>
		public MetadataTag(FITAG tag, FIBITMAP dib)
		{
			if (tag.IsNull)
			{
				throw new ArgumentNullException("tag");
			}
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			this.tag = tag;
			model = GetModel(dib, tag);
			selfCreated = false;

			if (model == FREE_IMAGE_MDMODEL.FIMD_XMP)
			{
				Key = "XMLPacket";
			}
		}

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="tag">The <see cref="FITAG"/> to represent.</param>
		/// <param name="model">The model of <paramref name="tag"/>.</param>
		public MetadataTag(FITAG tag, FREE_IMAGE_MDMODEL model)
		{
			if (tag.IsNull)
			{
				throw new ArgumentNullException("tag");
			}
			this.tag = tag;
			this.model = model;
			selfCreated = false;

			if (model == FREE_IMAGE_MDMODEL.FIMD_XMP)
			{
				Key = "XMLPacket";
			}
		}

		static MetadataTag()
		{
			idList = new Dictionary<FREE_IMAGE_MDTYPE, Type>();
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_BYTE, typeof(byte));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_SHORT, typeof(ushort));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_LONG, typeof(uint));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_RATIONAL, typeof(FIURational));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_SBYTE, typeof(sbyte));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_UNDEFINED, typeof(byte));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_SSHORT, typeof(short));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_SLONG, typeof(int));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_SRATIONAL, typeof(FIRational));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_FLOAT, typeof(float));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_DOUBLE, typeof(double));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_IFD, typeof(uint));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_PALETTE, typeof(RGBQUAD));

			typeList = new Dictionary<Type, FREE_IMAGE_MDTYPE>();
			typeList.Add(typeof(ushort), FREE_IMAGE_MDTYPE.FIDT_SHORT);
			typeList.Add(typeof(ushort[]), FREE_IMAGE_MDTYPE.FIDT_SHORT);
			typeList.Add(typeof(string), FREE_IMAGE_MDTYPE.FIDT_ASCII);
			typeList.Add(typeof(uint), FREE_IMAGE_MDTYPE.FIDT_LONG);
			typeList.Add(typeof(uint[]), FREE_IMAGE_MDTYPE.FIDT_LONG);
			typeList.Add(typeof(FIURational), FREE_IMAGE_MDTYPE.FIDT_RATIONAL);
			typeList.Add(typeof(FIURational[]), FREE_IMAGE_MDTYPE.FIDT_RATIONAL);
			typeList.Add(typeof(sbyte), FREE_IMAGE_MDTYPE.FIDT_SBYTE);
			typeList.Add(typeof(sbyte[]), FREE_IMAGE_MDTYPE.FIDT_SBYTE);
			typeList.Add(typeof(byte), FREE_IMAGE_MDTYPE.FIDT_UNDEFINED);
			typeList.Add(typeof(byte[]), FREE_IMAGE_MDTYPE.FIDT_UNDEFINED);
			typeList.Add(typeof(short), FREE_IMAGE_MDTYPE.FIDT_SSHORT);
			typeList.Add(typeof(short[]), FREE_IMAGE_MDTYPE.FIDT_SSHORT);
			typeList.Add(typeof(int), FREE_IMAGE_MDTYPE.FIDT_SLONG);
			typeList.Add(typeof(int[]), FREE_IMAGE_MDTYPE.FIDT_SLONG);
			typeList.Add(typeof(FIRational), FREE_IMAGE_MDTYPE.FIDT_SRATIONAL);
			typeList.Add(typeof(FIRational[]), FREE_IMAGE_MDTYPE.FIDT_SRATIONAL);
			typeList.Add(typeof(float), FREE_IMAGE_MDTYPE.FIDT_FLOAT);
			typeList.Add(typeof(float[]), FREE_IMAGE_MDTYPE.FIDT_FLOAT);
			typeList.Add(typeof(double), FREE_IMAGE_MDTYPE.FIDT_DOUBLE);
			typeList.Add(typeof(double[]), FREE_IMAGE_MDTYPE.FIDT_DOUBLE);
			typeList.Add(typeof(RGBQUAD), FREE_IMAGE_MDTYPE.FIDT_PALETTE);
			typeList.Add(typeof(RGBQUAD[]), FREE_IMAGE_MDTYPE.FIDT_PALETTE);
		}

		/// <summary>
		/// Releases all resources used by the instance.
		/// </summary>
		~MetadataTag()
		{
			Dispose();
		}

		/// <summary>
		/// Determines whether two specified <see cref="MetadataTag"/> objects have the same value.
		/// </summary>
		/// <param name="left">A <see cref="MetadataTag"/> or a null reference (<b>Nothing</b> in Visual Basic).</param>
		/// <param name="right">A <see cref="MetadataTag"/> or a null reference (<b>Nothing</b> in Visual Basic).</param>
		/// <returns>
		/// <b>true</b> if the value of left is the same as the value of right; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator ==(MetadataTag left, MetadataTag right)
		{
			// Check whether both are null
			if (Object.ReferenceEquals(left, null) && Object.ReferenceEquals(right, null))
			{
				return true;
			}
			// Check whether only one is null
			if (Object.ReferenceEquals(left, null) || Object.ReferenceEquals(right, null))
			{
				return false;
			}
			left.CheckDisposed();
			right.CheckDisposed();
			// Check all properties
			if ((left.Key != right.Key) ||
				(left.ID != right.ID) ||
				(left.Description != right.Description) ||
				(left.Count != right.Count) ||
				(left.Length != right.Length) ||
				(left.Model != right.Model) ||
				(left.Type != right.Type))
			{
				return false;
			}
			if (left.Length == 0)
			{
				return true;
			}
			IntPtr ptr1 = FreeImage.GetTagValue(left.tag);
			IntPtr ptr2 = FreeImage.GetTagValue(right.tag);
			return FreeImage.CompareMemory(ptr1, ptr2, left.Length);
		}

		/// <summary>
		/// Determines whether two specified <see cref="MetadataTag"/> objects have different values.
		/// </summary>
		/// <param name="left">A <see cref="MetadataTag"/> or a null reference (<b>Nothing</b> in Visual Basic).</param>
		/// <param name="right">A <see cref="MetadataTag"/> or a null reference (<b>Nothing</b> in Visual Basic).</param>
		/// <returns>
		/// true if the value of left is different from the value of right; otherwise, <b>false</b>.
		/// </returns>
		public static bool operator !=(MetadataTag left, MetadataTag right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Extracts the value of a <see cref="MetadataTag"/> instance to a <see cref="FITAG"/> handle.
		/// </summary>
		/// <param name="value">A <see cref="MetadataTag"/> instance.</param>
		/// <returns>A new instance of <see cref="FITAG"/> initialized to <paramref name="value"/>.</returns>
		public static implicit operator FITAG(MetadataTag value)
		{
			return value.tag;
		}

		private static FREE_IMAGE_MDMODEL GetModel(FIBITMAP dib, FITAG tag)
		{
			FITAG value;
			foreach (FREE_IMAGE_MDMODEL model in FreeImage.FREE_IMAGE_MDMODELS)
			{
				FIMETADATA mData = FreeImage.FindFirstMetadata(model, dib, out value);
				if (mData.IsNull)
				{
					continue;
				}
				try
				{
					do
					{
						if (value == tag)
						{
							return model;
						}
					}
					while (FreeImage.FindNextMetadata(mData, out value));
				}
				finally
				{
					if (!mData.IsNull)
					{
						FreeImage.FindCloseMetadata(mData);
					}
				}
			}
			throw new ArgumentException("'tag' is no metadata object of 'dib'");
		}

		/// <summary>
		/// Gets the model of the metadata.
		/// </summary>
		public FREE_IMAGE_MDMODEL Model
		{
			get { CheckDisposed(); return model; }
		}

		/// <summary>
		/// Gets or sets the key of the metadata.
		/// </summary>
		public string Key
		{
			get { CheckDisposed(); return FreeImage.GetTagKey(tag); }
			set
			{
				CheckDisposed();
				if ((model != FREE_IMAGE_MDMODEL.FIMD_XMP) || (value == "XMLPacket"))
				{
					FreeImage.SetTagKey(tag, value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the description of the metadata.
		/// </summary>
		public string Description
		{
			get { CheckDisposed(); return FreeImage.GetTagDescription(tag); }
			set { CheckDisposed(); FreeImage.SetTagDescription(tag, value); }
		}

		/// <summary>
		/// Gets or sets the ID of the metadata.
		/// </summary>
		public ushort ID
		{
			get { CheckDisposed(); return FreeImage.GetTagID(tag); }
			set { CheckDisposed(); FreeImage.SetTagID(tag, value); }
		}

		/// <summary>
		/// Gets the type of the metadata.
		/// </summary>
		public FREE_IMAGE_MDTYPE Type
		{
			get { CheckDisposed(); return FreeImage.GetTagType(tag); }
			private set { FreeImage.SetTagType(tag, value); }
		}

		/// <summary>
		/// Gets the number of elements the metadata object contains.
		/// </summary>
		public uint Count
		{
			get { CheckDisposed(); return Type == FREE_IMAGE_MDTYPE.FIDT_ASCII ? FreeImage.GetTagCount(tag) - 1 : FreeImage.GetTagCount(tag); }
			private set { FreeImage.SetTagCount(tag, value); }
		}

		/// <summary>
		/// Gets the length of the value in bytes.
		/// </summary>
		public uint Length
		{
			get { CheckDisposed(); return Type == FREE_IMAGE_MDTYPE.FIDT_ASCII ? FreeImage.GetTagLength(tag) - 1 : FreeImage.GetTagLength(tag); }
			private set { FreeImage.SetTagLength(tag, value); }
		}

		private unsafe byte[] GetData()
		{
			uint length = Length;
			byte[] value = new byte[length];
			byte* ptr = (byte*)FreeImage.GetTagValue(tag);
			for (int i = 0; i < length; i++)
			{
				value[i] = ptr[i];
			}
			return value;
		}

		/// <summary>
		/// Gets or sets the value of the metadata.
		/// <para> In case value is of byte or byte[], <see cref="FREE_IMAGE_MDTYPE.FIDT_UNDEFINED"/> is assumed.</para>
		/// <para> In case value is of uint or uint[], <see cref="FREE_IMAGE_MDTYPE.FIDT_LONG"/> is assumed.</para>
		/// </summary>
		public unsafe object Value
		{
			get
			{
				CheckDisposed();
				int cnt = (int)Count;

				if (Type == FREE_IMAGE_MDTYPE.FIDT_ASCII)
				{
					byte* value = (byte*)FreeImage.GetTagValue(tag);
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < cnt; i++)
					{
						sb.Append(Convert.ToChar(value[i]));
					}
					return sb.ToString();
				}
				else if (Type == FREE_IMAGE_MDTYPE.FIDT_NOTYPE)
				{
					return null;
				}

				Array array = Array.CreateInstance(idList[Type], Count);
				void* src = (void*)FreeImage.GetTagValue(tag);
				GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
				void* dst = (void*)Marshal.UnsafeAddrOfPinnedArrayElement(array, 0);
				FreeImage.CopyMemory(dst, src, Length);
				handle.Free();
				return array;
			}
			set
			{
				SetValue(value);
			}
		}

		/// <summary>
		/// Sets the value of the metadata.
		/// <para> In case value is of byte or byte[] <see cref="FREE_IMAGE_MDTYPE.FIDT_UNDEFINED"/> is assumed.</para>
		/// <para> In case value is of uint or uint[] <see cref="FREE_IMAGE_MDTYPE.FIDT_LONG"/> is assumed.</para>
		/// </summary>
		/// <param name="value">New data of the metadata.</param>
		/// <returns>True on success, false on failure.</returns>
		/// <exception cref="NotSupportedException">
		/// The data format is not supported.</exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="value"/> is null.</exception>
		public bool SetValue(object value)
		{
			Type type = value.GetType();
			if (!typeList.ContainsKey(type))
			{
				throw new NotSupportedException();
			}
			return SetValue(value, typeList[type]);
		}

		/// <summary>
		/// Sets the value of the metadata.
		/// </summary>
		/// <param name="value">New data of the metadata.</param>
		/// <param name="type">Type of the data.</param>
		/// <returns>True on success, false on failure.</returns>
		/// <exception cref="NotSupportedException">
		/// The data type is not supported.</exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="value"/> and <paramref name="type"/> to not fit.</exception>
		public bool SetValue(object value, FREE_IMAGE_MDTYPE type)
		{
			CheckDisposed();
			if ((!value.GetType().IsArray) && (!(value is string)))
			{
				Array array = Array.CreateInstance(value.GetType(), 1);
				array.SetValue(value, 0);
				return SetArrayValue(array, type);
			}
			return SetArrayValue(value, type);
		}

		/// <summary>
		/// Sets the value of this tag to the value of <paramref name="value"/>
		/// using the given type.
		/// </summary>
		/// <param name="value">New value of the tag.</param>
		/// <param name="type">Data-type of the tag.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="value"/> is a null reference.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="type"/> is FIDT_ASCII and
		/// <paramref name="value"/> is not String.
		/// <paramref name="type"/> is not FIDT_ASCII and
		/// <paramref name="value"/> is not Array.</exception>
		/// <exception cref="NotSupportedException">
		/// <paramref name="type"/> is FIDT_NOTYPE.</exception>
		private unsafe bool SetArrayValue(object value, FREE_IMAGE_MDTYPE type)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			byte[] data = null;

			if (type == FREE_IMAGE_MDTYPE.FIDT_ASCII)
			{
				string tempValue = value as string;
				if (tempValue == null)
				{
					throw new ArgumentException("value");
				}
				Type = type;
				Count = (uint)(tempValue.Length + 1);
				Length = (uint)((tempValue.Length * sizeof(byte)) + 1);
				data = new byte[Length + 1];

				for (int i = 0; i < tempValue.Length; i++)
				{
					data[i] = (byte)tempValue[i];
				}
				data[data.Length - 1] = 0;
			}
			else if (type == FREE_IMAGE_MDTYPE.FIDT_NOTYPE)
			{
				throw new NotSupportedException();
			}
			else
			{
				Array array = value as Array;
				if (array == null)
				{
					throw new ArgumentException("value");
				}
				Type = type;
				Count = (uint)array.Length;
				Length = (uint)(array.Length * Marshal.SizeOf(idList[type]));
				data = new byte[Length];
				GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
				void* src = (void*)Marshal.UnsafeAddrOfPinnedArrayElement(array, 0);
				fixed (byte* dst = data)
				{
					FreeImage.CopyMemory(dst, src, Length);
				}
				handle.Free();
			}

			return FreeImage.SetTagValue(tag, data);
		}

		/// <summary>
		/// Add this metadata to an image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>True on success, false on failure.</returns>
		public bool AddToImage(FIBITMAP dib)
		{
			CheckDisposed();
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			if (Key == null)
			{
				throw new ArgumentNullException("Key");
			}
			if (!selfCreated)
			{
				tag = FreeImage.CloneTag(tag);
				if (tag.IsNull)
				{
					throw new Exception();
				}
				selfCreated = true;
			}
			if (!FreeImage.SetMetadata(Model, dib, Key, tag))
			{
				return false;
			}
			FREE_IMAGE_MDMODEL _model = Model;
			string _key = Key;
			selfCreated = false;
			FreeImage.DeleteTag(tag);
			return FreeImage.GetMetadata(_model, dib, _key, out tag);
		}

		/// <summary>
		/// Gets a .NET PropertyItem for this metadata tag.
		/// </summary>
		/// <returns>The .NET PropertyItem.</returns>
		public unsafe System.Drawing.Imaging.PropertyItem GetPropertyItem()
		{
			System.Drawing.Imaging.PropertyItem item = FreeImage.CreatePropertyItem();
			item.Id = ID;
			item.Len = (int)Length;
			item.Type = (short)Type;
			byte[] data = new byte[item.Len];
			byte* ptr = (byte*)FreeImage.GetTagValue(tag);
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = ptr[i];
			}
			item.Value = data;
			return item;
		}

		/// <summary>
		/// Converts the value of the <see cref="MetadataTag"/> object
		/// to its equivalent string representation.
		/// </summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			CheckDisposed();
			string fiString = FreeImage.TagToString(model, tag, 0);

			if (String.IsNullOrEmpty(fiString))
			{
				return tag.ToString();
			}
			else
			{
				return fiString;
			}
		}

		/// <summary>
		/// Creates a deep copy of this <see cref="MetadataTag"/>.
		/// </summary>
		/// <returns>A deep copy of this <see cref="MetadataTag"/>.</returns>
		public object Clone()
		{
			CheckDisposed();
			MetadataTag clone = new MetadataTag();
			clone.model = model;
			clone.tag = FreeImage.CloneTag(tag);
			clone.selfCreated = true;
			return clone;
		}

		/// <summary>
		/// Tests whether the specified object is a <see cref="MetadataTag"/> instance
		/// and is equivalent to this <see cref="MetadataTag"/> instance.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> is a <see cref="MetadataTag"/> instance
		/// equivalent to this <see cref="MetadataTag"/> instance; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is MetadataTag) && (Equals((MetadataTag)obj)));
		}

		/// <summary>
		/// Tests whether the specified <see cref="MetadataTag"/> instance is equivalent to this <see cref="MetadataTag"/> instance.
		/// </summary>
		/// <param name="other">A <see cref="MetadataTag"/> instance to compare to this instance.</param>
		/// <returns><b>true</b> if <paramref name="obj"/> equivalent to this <see cref="MetadataTag"/> instance;
		/// otherwise, <b>false</b>.</returns>
		public bool Equals(MetadataTag other)
		{
			return (this == other);
		}

		/// <summary>
		/// Returns a hash code for this <see cref="MetadataTag"/> structure.
		/// </summary>
		/// <returns>An integer value that specifies the hash code for this <see cref="MetadataTag"/>.</returns>
		public override int GetHashCode()
		{
			return tag.GetHashCode();
		}

		/// <summary>
		/// Compares this instance with a specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="MetadataTag"/>.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is MetadataTag))
			{
				throw new ArgumentException();
			}
			return CompareTo((MetadataTag)obj);
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(MetadataTag other)
		{
			CheckDisposed();
			other.CheckDisposed();
			return tag.CompareTo(other.tag);
		}

		/// <summary>
		/// Releases all resources used by the instance.
		/// </summary>
		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;
				if (selfCreated)
				{
					FreeImage.DeleteTag(tag);
				}
			}
		}

		/// <summary>
		/// Gets whether this instance has already been disposed.
		/// </summary>
		public bool Disposed
		{
			get { return disposed; }
		}

		/// <summary>
		/// Throwns an <see cref="ObjectDisposedException"/> in case
		/// this instance has already been disposed.
		/// </summary>
		private void CheckDisposed()
		{
			if (disposed)
			{
				throw new ObjectDisposedException("The object has already been disposed.");
			}
		}
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Provides methods for working with the standard bitmap palette.
	/// </summary>
	public sealed class Palette : MemoryArray<RGBQUAD>
	{
		/// <summary>
		/// Initializes a new instance for the given FreeImage bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public Palette(FIBITMAP dib)
			: base(FreeImage.GetPalette(dib), (int)FreeImage.GetColorsUsed(dib))
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException();
			}
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				throw new ArgumentException("dib");
			}
			if (FreeImage.GetBPP(dib) > 8u)
			{
				throw new ArgumentException("dib");
			}
		}

		/// <summary>
		/// Gets or sets the palette through an array of <see cref="RGBQUAD"/>.
		/// </summary>
		public RGBQUAD[] AsArray
		{
			get
			{
				return Data;
			}
			set
			{
				Data = value;
			}
		}

		/// <summary>
		/// Get an array of <see cref="System.Drawing.Color"/> that the block of memory represents.
		/// This property is used for internal palette operations.
		/// </summary>
		internal unsafe Color[] ColorData
		{
			get
			{
				Color[] data = new Color[length];
				for (int i = 0; i < length; i++)
				{
					data[i] = Color.FromArgb((int)(((uint*)baseAddress)[i] | 0xFF000000));
				}
				return data;
			}
		}

		/// <summary>
		/// Returns the palette as an array of <see cref="RGBQUAD"/>.
		/// </summary>
		/// <returns>The palette as an array of <see cref="RGBQUAD"/>.</returns>
		public RGBQUAD[] ToArray()
		{
			return Data;
		}

		/// <summary>
		/// Creates a linear palette based on the provided <paramref name="color"/>.
		/// </summary>
		/// <param name="color">The <see cref="System.Drawing.Color"/> used to colorize the palette.</param>
		/// <remarks>
		/// Only call this method on linear palettes.
		/// </remarks>
		public void Colorize(Color color)
		{
			Colorize(color, 0.5d);
		}

		/// <summary>
		/// Creates a linear palette based on the provided <paramref name="color"/>.
		/// </summary>
		/// <param name="color">The <see cref="System.Drawing.Color"/> used to colorize the palette.</param>
		/// <param name="splitSize">The position of the color within the new palette.
		/// 0 &lt; <paramref name="splitSize"/> &lt; 1.</param>
		/// <remarks>
		/// Only call this method on linear palettes.
		/// </remarks>
		public void Colorize(Color color, double splitSize)
		{
			Colorize(color, (int)(length * splitSize));
		}

		/// <summary>
		/// Creates a linear palette based on the provided <paramref name="color"/>.
		/// </summary>
		/// <param name="color">The <see cref="System.Drawing.Color"/> used to colorize the palette.</param>
		/// <param name="splitSize">The position of the color within the new palette.
		/// 0 &lt; <paramref name="splitSize"/> &lt; <see cref="MemoryArray&lt;T&gt;.Length"/>.</param>
		/// <remarks>
		/// Only call this method on linear palettes.
		/// </remarks>
		public void Colorize(Color color, int splitSize)
		{
			if (splitSize < 1 || splitSize >= length)
			{
				throw new ArgumentOutOfRangeException("splitSize");
			}

			RGBQUAD[] pal = new RGBQUAD[length];

			double red = color.R;
			double green = color.G;
			double blue = color.B;

			int i = 0;
			double r, g, b;

			r = red / splitSize;
			g = green / splitSize;
			b = blue / splitSize;

			for (; i <= splitSize; i++)
			{
				pal[i].rgbRed = (byte)(i * r);
				pal[i].rgbGreen = (byte)(i * g);
				pal[i].rgbBlue = (byte)(i * b);
			}

			r = (255 - red) / (length - splitSize);
			g = (255 - green) / (length - splitSize);
			b = (255 - blue) / (length - splitSize);

			for (; i < length; i++)
			{
				pal[i].rgbRed = (byte)(red + ((i - splitSize) * r));
				pal[i].rgbGreen = (byte)(green + ((i - splitSize) * g));
				pal[i].rgbBlue = (byte)(blue + ((i - splitSize) * b));
			}

			Data = pal;
		}

		/// <summary>
		/// Saves this <see cref="Palette"/> to the specified file.
		/// </summary>
		/// <param name="filename">
		/// A string that contains the name of the file to which to save this <see cref="Palette"/>.
		/// </param>
		public void Save(string filename)
		{
			using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
			{
				Save(stream);
			}
		}

		/// <summary>
		/// Saves this <see cref="Palette"/> to the specified stream.
		/// </summary>
		/// <param name="stream">
		/// The <see cref="Stream"/> where the image will be saved.
		/// </param>
		public void Save(Stream stream)
		{
			Save(new BinaryWriter(stream));
		}

		/// <summary>
		/// Saves this <see cref="Palette"/> using the specified writer.
		/// </summary>
		/// <param name="writer">
		/// The <see cref="BinaryWriter"/> used to save the image.
		/// </param>
		public void Save(BinaryWriter writer)
		{
			writer.Write(ToByteArray());
		}

		/// <summary>
		/// Loads a palette from the specified file.
		/// </summary>
		/// <param name="filename">The name of the palette file.</param>
		public void Load(string filename)
		{
			using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				Load(stream);
			}
		}

		/// <summary>
		/// Loads a palette from the specified stream.
		/// </summary>
		/// <param name="stream">The stream to load the palette from.</param>
		public void Load(Stream stream)
		{
			Load(new BinaryReader(stream));
		}

		/// <summary>
		/// Loads a palette from the reader.
		/// </summary>
		/// <param name="reader">The reader to load the palette from.</param>
		public unsafe void Load(BinaryReader reader)
		{
			int size = length * sizeof(RGBQUAD);
			byte[] data = reader.ReadBytes(size);
			fixed(byte* src = data)
			{
				CopyMemory(baseAddress, src, data.Length);
			}
		}
	}
}

namespace FreeImageAPI.Plugins
{
	/// <summary>
	/// Class representing all registered <see cref="FreeImageAPI.Plugins.FreeImagePlugin"/> in FreeImage.
	/// </summary>
	public static class PluginRepository
	{
		private static readonly List<FreeImagePlugin> plugins = null;
		private static readonly List<FreeImagePlugin> localPlugins = null;

		static PluginRepository()
		{
			plugins = new List<FreeImagePlugin>(FreeImage.GetFIFCount());
			localPlugins = new List<FreeImagePlugin>(0);
			for (int i = 0; i < plugins.Capacity; i++)
			{
				plugins.Add(new FreeImagePlugin((FREE_IMAGE_FORMAT)i));
			}
		}

		/// <summary>
		/// Adds local plugin to this class.
		/// </summary>
		/// <param name="localPlugin">The registered plugin.</param>
		internal static void RegisterLocalPlugin(LocalPlugin localPlugin)
		{
			FreeImagePlugin plugin = new FreeImagePlugin(localPlugin.Format);
			plugins.Add(plugin);
			localPlugins.Add(plugin);
		}

		/// <summary>
		/// Returns an instance of <see cref="FreeImageAPI.Plugins.FreeImagePlugin"/>, representing the given format.
		/// </summary>
		/// <param name="fif">The representing format.</param>
		/// <returns>An instance of <see cref="FreeImageAPI.Plugins.FreeImagePlugin"/>.</returns>
		public static FreeImagePlugin Plugin(FREE_IMAGE_FORMAT fif)
		{
			return Plugin((int)fif);
		}

		/// <summary>
		/// Returns an instance of <see cref="FreeImageAPI.Plugins.FreeImagePlugin"/>,
		/// representing the format at the given index.
		/// </summary>
		/// <param name="index">The index of the representing format.</param>
		/// <returns>An instance of <see cref="FreeImagePlugin"/>.</returns>
		public static FreeImagePlugin Plugin(int index)
		{
			return (index >= 0) ? plugins[index] : null;
		}

		/// <summary>
		/// Returns an instance of <see cref="FreeImageAPI.Plugins.FreeImagePlugin"/>.
		/// <typeparamref name="expression"/> is searched in:
		/// <c>Format</c>, <c>RegExpr</c>,
		/// <c>ValidExtension</c> and <c>ValidFilename</c>.
		/// </summary>
		/// <param name="expression">The expression to search for.</param>
		/// <returns>An instance of <see cref="FreeImageAPI.Plugins.FreeImagePlugin"/>.</returns>
		public static FreeImagePlugin Plugin(string expression)
		{
			FreeImagePlugin result = null;
			expression = expression.ToLower();

			foreach (FreeImagePlugin plugin in plugins)
			{
				if (plugin.Format.ToLower().Contains(expression) ||
					plugin.RegExpr.ToLower().Contains(expression) ||
					plugin.ValidExtension(expression, StringComparison.CurrentCultureIgnoreCase) ||
					plugin.ValidFilename(expression, StringComparison.CurrentCultureIgnoreCase))
				{
					result = plugin;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Returns an instance of <see cref="FreeImageAPI.Plugins.FreeImagePlugin"/> for the given format.
		/// </summary>
		/// <param name="format">The format of the Plugin.</param>
		/// <returns>An instance of <see cref="FreeImageAPI.Plugins.FreeImagePlugin"/>.</returns>
		public static FreeImagePlugin PluginFromFormat(string format)
		{
			return Plugin(FreeImage.GetFIFFromFormat(format));
		}

		/// <summary>
		/// Returns an instance of <see cref="FreeImageAPI.Plugins.FreeImagePlugin"/> for the given filename.
		/// </summary>
		/// <param name="filename">The valid filename for the plugin.</param>
		/// <returns>An instance of <see cref="FreeImageAPI.Plugins.FreeImagePlugin"/>.</returns>
		public static FreeImagePlugin PluginFromFilename(string filename)
		{
			return Plugin(FreeImage.GetFIFFromFilename(filename));
		}

		/// <summary>
		/// Returns an instance of <see cref="FreeImageAPI.Plugins.FreeImagePlugin"/> for the given mime.
		/// </summary>
		/// <param name="mime">The valid mime for the plugin.</param>
		/// <returns>An instance of <see cref="FreeImageAPI.Plugins.FreeImagePlugin"/>.</returns>
		public static FreeImagePlugin PluginFromMime(string mime)
		{
			return Plugin(FreeImage.GetFIFFromMime(mime));
		}

		/// <summary>
		/// Gets the number of registered plugins.
		/// </summary>
		public static int FIFCount
		{
			get
			{
				return FreeImage.GetFIFCount();
			}
		}

		/// <summary>
		/// Gets a readonly collection of all plugins.
		/// </summary>
		public static ReadOnlyCollection<FreeImagePlugin> PluginList
		{
			get
			{
				return plugins.AsReadOnly();
			}
		}

		/// <summary>
		/// Gets a list of plugins that are only able to
		/// read but not to write.
		/// </summary>
		public static List<FreeImagePlugin> ReadOnlyPlugins
		{
			get
			{
				List<FreeImagePlugin> list = new List<FreeImagePlugin>();
				foreach (FreeImagePlugin p in plugins)
				{
					if (p.SupportsReading && !p.SupportsWriting) 
					{
						list.Add(p); 
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Gets a list of plugins that are only able to
		/// write but not to read.
		/// </summary>
		public static List<FreeImagePlugin> WriteOnlyPlugins
		{
			get
			{
				List<FreeImagePlugin> list = new List<FreeImagePlugin>();
				foreach (FreeImagePlugin p in plugins)
				{
					if (!p.SupportsReading && p.SupportsWriting)
					{
						list.Add(p);
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Gets a list of plugins that are not able to
		/// read or write.
		/// </summary>
		public static List<FreeImagePlugin> StupidPlugins
		{
			get
			{
				List<FreeImagePlugin> list = new List<FreeImagePlugin>();
				foreach (FreeImagePlugin p in plugins)
				{
					if (!p.SupportsReading && !p.SupportsWriting)
					{
						list.Add(p);
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Gets a list of plugins that are able to read.
		/// </summary>
		public static List<FreeImagePlugin> ReadablePlugins
		{
			get
			{
				List<FreeImagePlugin> list = new List<FreeImagePlugin>();
				foreach (FreeImagePlugin p in plugins)
				{
					if (p.SupportsReading)
					{
						list.Add(p);
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Gets a list of plugins that are able to write.
		/// </summary>
		public static List<FreeImagePlugin> WriteablePlugins
		{
			get
			{
				List<FreeImagePlugin> list = new List<FreeImagePlugin>();
				foreach (FreeImagePlugin p in plugins)
				{
					if (p.SupportsWriting)
					{
						list.Add(p);
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Gets a list of local plugins.
		/// </summary>
		public static ReadOnlyCollection<FreeImagePlugin> LocalPlugins
		{
			get
			{
				return localPlugins.AsReadOnly();  
			}
		}

		/// <summary>
		/// Gets a list of built-in plugins.
		/// </summary>
		public static List<FreeImagePlugin> BuiltInPlugins
		{
			get
			{
				List<FreeImagePlugin> list = new List<FreeImagePlugin>();
				foreach (FreeImagePlugin p in plugins)
				{
					if (!localPlugins.Contains(p))
					{
						list.Add(p);
					}
				}
				return list;
			}
		}
		
		/// <summary>
		/// Windows or OS/2 Bitmap File (*.BMP)
		/// </summary>
		public static FreeImagePlugin BMP { get { return plugins[0]; } }

		/// <summary>
		/// Independent JPEG Group (*.JPG, *.JIF, *.JPEG, *.JPE)
		/// </summary>
		public static FreeImagePlugin ICO { get { return plugins[1]; } }

		/// <summary>
		/// Independent JPEG Group (*.JPG, *.JIF, *.JPEG, *.JPE)
		/// </summary>
		public static FreeImagePlugin JPEG { get { return plugins[2]; } }

		/// <summary>
		/// JPEG Network Graphics (*.JNG)
		/// </summary>
		public static FreeImagePlugin JNG { get { return plugins[3]; } }

		/// <summary>
		/// Commodore 64 Koala format (*.KOA)
		/// </summary>
		public static FreeImagePlugin KOALA { get { return plugins[4]; } }

		/// <summary>
		/// Amiga IFF (*.IFF, *.LBM)
		/// </summary>
		public static FreeImagePlugin LBM { get { return plugins[5]; } }

		/// <summary>
		/// Amiga IFF (*.IFF, *.LBM)
		/// </summary>
		public static FreeImagePlugin IFF { get { return plugins[5]; } }

		/// <summary>
		/// Multiple Network Graphics (*.MNG)
		/// </summary>
		public static FreeImagePlugin MNG { get { return plugins[6]; } }

		/// <summary>
		/// Portable Bitmap (ASCII) (*.PBM)
		/// </summary>
		public static FreeImagePlugin PBM { get { return plugins[7]; } }

		/// <summary>
		/// Portable Bitmap (BINARY) (*.PBM)
		/// </summary>
		public static FreeImagePlugin PBMRAW { get { return plugins[8]; } }

		/// <summary>
		/// Kodak PhotoCD (*.PCD)
		/// </summary>
		public static FreeImagePlugin PCD { get { return plugins[9]; } }

		/// <summary>
		/// Zsoft Paintbrush PCX bitmap format (*.PCX)
		/// </summary>
		public static FreeImagePlugin PCX { get { return plugins[10]; } }

		/// <summary>
		/// Portable Graymap (ASCII) (*.PGM)
		/// </summary>
		public static FreeImagePlugin PGM { get { return plugins[11]; } }

		/// <summary>
		/// Portable Graymap (BINARY) (*.PGM)
		/// </summary>
		public static FreeImagePlugin PGMRAW { get { return plugins[12]; } }

		/// <summary>
		/// Portable Network Graphics (*.PNG)
		/// </summary>
		public static FreeImagePlugin PNG { get { return plugins[13]; } }

		/// <summary>
		/// Portable Pixelmap (ASCII) (*.PPM)
		/// </summary>
		public static FreeImagePlugin PPM { get { return plugins[14]; } }

		/// <summary>
		/// Portable Pixelmap (BINARY) (*.PPM)
		/// </summary>
		public static FreeImagePlugin PPMRAW { get { return plugins[15]; } }

		/// <summary>
		/// Sun Rasterfile (*.RAS)
		/// </summary>
		public static FreeImagePlugin RAS { get { return plugins[16]; } }

		/// <summary>
		/// truevision Targa files (*.TGA, *.TARGA)
		/// </summary>
		public static FreeImagePlugin TARGA { get { return plugins[17]; } }

		/// <summary>
		/// Tagged Image File Format (*.TIF, *.TIFF)
		/// </summary>
		public static FreeImagePlugin TIFF { get { return plugins[18]; } }

		/// <summary>
		/// Wireless Bitmap (*.WBMP)
		/// </summary>
		public static FreeImagePlugin WBMP { get { return plugins[19]; } }

		/// <summary>
		/// Adobe Photoshop (*.PSD)
		/// </summary>
		public static FreeImagePlugin PSD { get { return plugins[20]; } }

		/// <summary>
		/// Dr. Halo (*.CUT)
		/// </summary>
		public static FreeImagePlugin CUT { get { return plugins[21]; } }

		/// <summary>
		/// X11 Bitmap Format (*.XBM)
		/// </summary>
		public static FreeImagePlugin XBM { get { return plugins[22]; } }

		/// <summary>
		/// X11 Pixmap Format (*.XPM)
		/// </summary>
		public static FreeImagePlugin XPM { get { return plugins[23]; } }

		/// <summary>
		/// DirectDraw Surface (*.DDS)
		/// </summary>
		public static FreeImagePlugin DDS { get { return plugins[24]; } }

		/// <summary>
		/// Graphics Interchange Format (*.GIF)
		/// </summary>
		public static FreeImagePlugin GIF { get { return plugins[25]; } }

		/// <summary>
		/// High Dynamic Range (*.HDR)
		/// </summary>
		public static FreeImagePlugin HDR { get { return plugins[26]; } }

		/// <summary>
		/// Raw Fax format CCITT G3 (*.G3)
		/// </summary>
		public static FreeImagePlugin FAXG3 { get { return plugins[27]; } }

		/// <summary>
		/// Silicon Graphics SGI image format (*.SGI)
		/// </summary>
		public static FreeImagePlugin SGI { get { return plugins[28]; } }

		/// <summary>
		/// OpenEXR format (*.EXR)
		/// </summary>
		public static FreeImagePlugin EXR { get { return plugins[29]; } }

		/// <summary>
		/// JPEG-2000 format (*.J2K, *.J2C)
		/// </summary>
		public static FreeImagePlugin J2K { get { return plugins[30]; } }

		/// <summary>
		/// JPEG-2000 format (*.JP2)
		/// </summary>
		public static FreeImagePlugin JP2 { get { return plugins[31]; } }
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Provides mathods for working with generic bitmap scanlines.
	/// </summary>
	/// <typeparam name="T">Type of the bitmaps' scanlines.</typeparam>
	public sealed class Scanline<T> : MemoryArray<T> where T : struct
	{
		/// <summary>
		/// Initializes a new instance based on the specified FreeImage bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public Scanline(FIBITMAP dib)
			: this(dib, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance based on the specified FreeImage bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Index of the zero based scanline.</param>
		public Scanline(FIBITMAP dib, int scanline)
			: this(dib, scanline, (int)(typeof(T) == typeof(FI1BIT) ?
				FreeImage.GetBPP(dib) * FreeImage.GetWidth(dib) :
				typeof(T) == typeof(FI4BIT) ?
				FreeImage.GetBPP(dib) * FreeImage.GetWidth(dib) / 4 :
				(FreeImage.GetBPP(dib) * FreeImage.GetWidth(dib)) / (Marshal.SizeOf(typeof(T)) * 8)))
		{
		}

		internal Scanline(FIBITMAP dib, int scanline, int length)
			: base(FreeImage.GetScanLine(dib, scanline), length)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			if ((scanline < 0) || (scanline >= FreeImage.GetHeight(dib)))
			{
				throw new ArgumentOutOfRangeException("scanline");
			}
		}
	}
}

namespace FreeImageAPI.IO
{
	/// <summary>
	/// Class wrapping streams, implementing a buffer for read data,
	/// so that seek operations can be made.
	/// </summary>
	/// <remarks>
	/// FreeImage can load bitmaps from arbitrary sources.
	/// .NET works with different streams like File- or NetConnection-strams.
	/// NetConnection streams, which are used to load files from web servers,
	/// for example cannot seek.
	/// But FreeImage frequently uses the seek operation when loading bitmaps.
	/// <b>StreamWrapper</b> wrapps a stream and makes it seekable by caching all read
	/// data into an internal MemoryStream to jump back- and forward.
	/// StreamWapper is for internal use and only for loading from streams.
	/// </remarks>
	internal class StreamWrapper : Stream
	{
		/// <summary>
		/// The stream to wrap
		/// </summary>
		private readonly Stream stream;
		/// <summary>
		/// The caching stream
		/// </summary>
		private MemoryStream memoryStream = new MemoryStream();
		/// <summary>
		/// Indicates if the wrapped stream reached its end
		/// </summary>
		private bool eos = false;
		/// <summary>
		/// Tells the wrapper to block readings or not
		/// </summary>
		private bool blocking = false;
		/// <summary>
		/// Indicates if the wrapped stream is disposed or not
		/// </summary>
		private bool disposed = false;

		/// <summary>
		/// Initializes a new instance based on the specified <see cref="Stream"/>.
		/// </summary>
		/// <param name="stream">The stream to wrap.</param>
		/// <param name="blocking">When true the wrapper always tries to read the requested
		/// amount of data from the wrapped stream.</param>
		public StreamWrapper(Stream stream, bool blocking)
		{
			if (!stream.CanRead)
			{
				throw new ArgumentException("stream is not capable of reading.");
			}
			this.stream = stream;
			this.blocking = blocking;
		}

		/// <summary>
		/// Releases all resources used by the instance.
		/// </summary>
		~StreamWrapper()
		{
			Dispose(false);
		}

		// The wrapper only accepts readable streams
		public override bool CanRead
		{
			get { checkDisposed(); return true; }
		}

		// We implement that feature
		public override bool CanSeek
		{
			get { checkDisposed(); return true; }
		}

		// The wrapper is readonly
		public override bool CanWrite
		{
			get { checkDisposed(); return false; }
		}

		// Just forward it
		public override void Flush()
		{
			checkDisposed();
			stream.Flush();
		}

		// Calling this property will cause the wrapper to read the stream
		// to its end and cache it completely.
		public override long Length
		{
			get
			{
				checkDisposed();
				if (!eos)
				{
					Fill();
				}
				return memoryStream.Length;
			}
		}

		// Gets or sets the current position
		public override long Position
		{
			get
			{
				checkDisposed();
				return memoryStream.Position;
			}
			set
			{
				checkDisposed();
				Seek(value, SeekOrigin.Begin);
			}
		}

		// Implements the reading feature
		public override int Read(byte[] buffer, int offset, int count)
		{
			checkDisposed();
			// total bytes read from memory-stream
			int memoryBytes = 0;
			// total bytes read from the original stream
			int streamBytes = 0;
			memoryBytes = memoryStream.Read(buffer, offset, count);
			if ((count > memoryBytes) && (!eos))
			{
				// read the rest from the original stream (can be 0 bytes)
				do
				{
					int read = stream.Read(
						buffer,
						offset + memoryBytes + streamBytes,
						count - memoryBytes - streamBytes);
					streamBytes += read;
					if (read == 0)
					{
						eos = true;
						break;
					}
					if (!blocking)
					{
						break;
					}
				} while ((memoryBytes + streamBytes) < count);
				// copy the bytes from the original stream into the memory stream
				// if 0 bytes were read we write 0 so the memory-stream is not changed
				memoryStream.Write(buffer, offset + memoryBytes, streamBytes);
			}
			return memoryBytes + streamBytes;
		}

		// Implements the seeking feature
		public override long Seek(long offset, SeekOrigin origin)
		{
			checkDisposed();
			long newPosition = 0L;
			// get new position
			switch (origin)
			{
				case SeekOrigin.Begin:
					newPosition = offset;
					break;
				case SeekOrigin.Current:
					newPosition = memoryStream.Position + offset;
					break;
				case SeekOrigin.End:
					// to seek from the end have have to read to the end first
					if (!eos)
					{
						Fill();
					}
					newPosition = memoryStream.Length + offset;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			// in case the new position is beyond the memory-streams end
			// and the original streams end hasn't been reached
			// the original stream is read until either the stream ends or
			// enough bytes have been read
			if ((newPosition > memoryStream.Length) && (!eos))
			{
				memoryStream.Position = memoryStream.Length;
				int bytesToRead = (int)(newPosition - memoryStream.Length);
				byte[] buffer = new byte[1024];
				do
				{
					bytesToRead -= Read(buffer, 0, (bytesToRead >= buffer.Length) ? buffer.Length : bytesToRead);
				} while ((bytesToRead > 0) && (!eos));
			}
			memoryStream.Position = (newPosition <= memoryStream.Length) ? newPosition : memoryStream.Length;
			return 0;
		}

		// No write-support
		public override void SetLength(long value)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		// No write-support
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Reset()
		{
			checkDisposed();
			Position = 0;
		}

		// Reads the wrapped stream until its end.
		private void Fill()
		{
			if (!eos)
			{
				memoryStream.Position = memoryStream.Length;
				int bytesRead = 0;
				byte[] buffer = new byte[1024];
				do
				{
					bytesRead = stream.Read(buffer, 0, buffer.Length);
					memoryStream.Write(buffer, 0, bytesRead);
				} while (bytesRead != 0);
				eos = true;
			}
		}

		public new void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private new void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;
				if (disposing)
				{
					if (memoryStream != null)
					{
						memoryStream.Dispose();
					}
				}
			}
		}

		public bool Disposed
		{
			get { return disposed; }
		}

		private void checkDisposed()
		{
			if (disposed) throw new ObjectDisposedException("StreamWrapper");
		}
	}
}

	#endregion

	#region Enums

namespace FreeImageAPI
{
	/// <summary>
	/// Enumeration used for color conversions.
	/// FREE_IMAGE_COLOR_DEPTH contains several colors to convert to.
	/// The default value 'FICD_AUTO'.
	/// </summary>
	[System.Flags]
	public enum FREE_IMAGE_COLOR_DEPTH
	{
		/// <summary>
		/// Unknown.
		/// </summary>
		FICD_UNKNOWN = 0,
		/// <summary>
		/// Auto selected by the used algorithm.
		/// </summary>
		FICD_AUTO = FICD_UNKNOWN,
		/// <summary>
		/// 1-bit.
		/// </summary>
		FICD_01_BPP = 1,
		/// <summary>
		/// 1-bit using dithering.
		/// </summary>
		FICD_01_BPP_DITHER = FICD_01_BPP,
		/// <summary>
		/// 1-bit using threshold.
		/// </summary>
		FICD_01_BPP_THRESHOLD = FICD_01_BPP | 2,
		/// <summary>
		/// 4-bit.
		/// </summary>
		FICD_04_BPP = 4,
		/// <summary>
		/// 8-bit.
		/// </summary>
		FICD_08_BPP = 8,
		/// <summary>
		/// 16-bit 555 (1 bit remains unused).
		/// </summary>
		FICD_16_BPP_555 = FICD_16_BPP | 2,
		/// <summary>
		/// 16-bit 565 (all bits are used).
		/// </summary>
		FICD_16_BPP = 16,
		/// <summary>
		/// 24-bit.
		/// </summary>
		FICD_24_BPP = 24,
		/// <summary>
		/// 32-bit.
		/// </summary>
		FICD_32_BPP = 32,
		/// <summary>
		/// Reorder palette (make it linear). Only affects 1-, 4- and 8-bit images.
		/// <para>The palette is only reordered in case the image is greyscale
		/// (all palette entries have the same red, green and blue value).</para>
		/// </summary>
		FICD_REORDER_PALETTE = 1024,
		/// <summary>
		/// Converts the image to greyscale.
		/// </summary>
		FICD_FORCE_GREYSCALE = 2048,
		/// <summary>
		/// Flag to mask out all non color depth flags.
		/// </summary>
		FICD_COLOR_MASK = FICD_01_BPP | FICD_04_BPP | FICD_08_BPP | FICD_16_BPP | FICD_24_BPP | FICD_32_BPP
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// List of combinable compare modes.
	/// </summary>
	[System.Flags]
	public enum FREE_IMAGE_COMPARE_FLAGS
	{
		/// <summary>
		/// Compare headers.
		/// </summary>
		HEADER = 0x1,
		/// <summary>
		/// Compare palettes.
		/// </summary>
		PALETTE = 0x2,
		/// <summary>
		/// Compare pixel data.
		/// </summary>
		DATA = 0x4,
		/// <summary>
		/// Compare meta data.
		/// </summary>
		METADATA = 0x8,
		/// <summary>
		/// Compare everything.
		/// </summary>
		COMPLETE = (HEADER | PALETTE | DATA | METADATA)
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// Flags for copying data from a bitmap to another.
	/// </summary>
	public enum FREE_IMAGE_METADATA_COPY
	{
		/// <summary>
		/// Exisiting metadata will remain unchanged.
		/// </summary>
		KEEP_EXISITNG = 0x0,
		/// <summary>
		/// Existing metadata will be cleared.
		/// </summary>
		CLEAR_EXISTING = 0x1,
		/// <summary>
		/// Existing metadata will be overwritten.
		/// </summary>
		REPLACE_EXISTING = 0x2
	}
}

namespace FreeImageAPI
{
	/// <summary>
	/// List different search modes.
	/// </summary>
	[System.Flags]
	public enum MD_SEARCH_FLAGS
	{
		/// <summary>
		/// The key of the metadata.
		/// </summary>
		KEY = 0x1,
		/// <summary>
		/// The description of the metadata
		/// </summary>
		DESCRIPTION = 0x2,
		/// <summary>
		/// The ToString value of the metadata
		/// </summary>
		TOSTRING = 0x4,
	}
}

	#endregion

namespace FreeImageAPI
{
	/// <summary>
	/// Static class importing functions from the FreeImage library
	/// and providing additional functions.
	/// </summary>
	public static partial class FreeImage
	{
		#region Constants

		/// <summary>
		/// Array containing all 'FREE_IMAGE_MDMODEL's.
		/// </summary>
		public static readonly FREE_IMAGE_MDMODEL[] FREE_IMAGE_MDMODELS =
			(FREE_IMAGE_MDMODEL[])Enum.GetValues(typeof(FREE_IMAGE_MDMODEL));

		/// <summary>
		/// Saved instance for faster access.
		/// </summary>
		private static readonly ConstructorInfo PropertyItemConstructor =
			typeof(PropertyItem).GetConstructor(
			BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);

		private const int DIB_RGB_COLORS = 0;
		private const int DIB_PAL_COLORS = 1;
		private const int CBM_INIT = 0x4;

		/// <summary>
		/// An uncompressed format.
		/// </summary>
		public const int BI_RGB = 0;

		/// <summary>
		/// A run-length encoded (RLE) format for bitmaps with 8 bpp. The compression format is a 2-byte
		/// format consisting of a count byte followed by a byte containing a color index.
		/// </summary>
		public const int BI_RLE8 = 1;

		/// <summary>
		/// An RLE format for bitmaps with 4 bpp. The compression format is a 2-byte format consisting
		/// of a count byte followed by two word-length color indexes.
		/// </summary>
		public const int BI_RLE4 = 2;

		/// <summary>
		/// Specifies that the bitmap is not compressed and that the color table consists of three
		/// <b>DWORD</b> color masks that specify the red, green, and blue components, respectively,
		/// of each pixel. This is valid when used with 16- and 32-bpp bitmaps.
		/// </summary>
		public const int BI_BITFIELDS = 3;

		/// <summary>
		/// <b>Windows 98/Me, Windows 2000/XP:</b> Indicates that the image is a JPEG image.
		/// </summary>
		public const int BI_JPEG = 4;

		/// <summary>
		/// <b>Windows 98/Me, Windows 2000/XP:</b> Indicates that the image is a PNG image.
		/// </summary>
		public const int BI_PNG = 5;

		#endregion

		#region General functions

		/// <summary>
		/// Returns the internal version of this FreeImage 3 .NET wrapper.
		/// </summary>
		/// <returns>The internal version of this FreeImage 3 .NET wrapper.</returns>
		public static string GetWrapperVersion()
		{
			return FREEIMAGE_MAJOR_VERSION + "." + FREEIMAGE_MINOR_VERSION + "." + FREEIMAGE_RELEASE_SERIAL;
		}

		/// <summary>
		/// Returns a value indicating if the FreeImage DLL is available or not.
		/// </summary>
		/// <returns>True, if the FreeImage DLL is available, false otherwise.</returns>
		public static bool IsAvailable()
		{
			try
			{
				// Call a static fast executing function
				GetVersion();
				// No exception thrown, the dll seems to be present
				return true;
			}
			catch (DllNotFoundException)
			{
				return false;
			}
			catch (EntryPointNotFoundException)
			{
				return false;
			}
		}

		#endregion

		#region Bitmap management functions

		/// <summary>
		/// Converts a FreeImage bitmap to a .NET <see cref="System.Drawing.Bitmap"/>.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The converted .NET <see cref="System.Drawing.Bitmap"/>.</returns>
		/// <remarks>Copying metadata has been disabled until a proper way
		/// of reading and storing metadata in a .NET bitmap is found.</remarks>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// The image type of <paramref name="dib"/> is not FIT_BITMAP.</exception>
		public static Bitmap GetBitmap(FIBITMAP dib)
		{
			return GetBitmap(dib, true);
		}

		/// <summary>
		/// Converts a FreeImage bitmap to a .NET <see cref="System.Drawing.Bitmap"/>.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="copyMetadata">When true existing metadata will be copied.</param>
		/// <returns>The converted .NET <see cref="System.Drawing.Bitmap"/>.</returns>
		/// <remarks>Copying metadata has been disabled until a proper way
		/// of reading and storing metadata in a .NET bitmap is found.</remarks>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// The image type of <paramref name="dib"/> is not FIT_BITMAP.</exception>
		internal static Bitmap GetBitmap(FIBITMAP dib, bool copyMetadata)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			if (GetImageType(dib) != FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				throw new ArgumentException("Only bitmaps with type of FIT_BITMAP can be converted.");
			}

			PixelFormat format = GetPixelFormat(dib);

			if ((format == PixelFormat.Undefined) && (GetBPP(dib) == 16u))
			{
				throw new ArgumentException("Only 16bit 555 and 565 are supported.");
			}

			int height = (int)GetHeight(dib);
			int width = (int)GetWidth(dib);
			int pitch = (int)GetPitch(dib);

			Bitmap result = new Bitmap(width, height, format);
			BitmapData data;
			// Locking the complete bitmap in writeonly mode
			data = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, format);
			// Writing the bitmap data directly into the new created .NET bitmap.
			ConvertToRawBits(data.Scan0, dib, pitch, GetBPP(dib),
				GetRedMask(dib), GetGreenMask(dib), GetBlueMask(dib), true);
			// Unlock the bitmap
			result.UnlockBits(data);
			// Apply the bitmaps resolution
			result.SetResolution(GetResolutionX(dib), GetResolutionY(dib));
			// Check whether the bitmap has a palette
			if (GetPalette(dib) != IntPtr.Zero)
			{
				// Get the bitmaps palette to apply changes
				ColorPalette palette = result.Palette;
				// Get the orgininal palette
				Color[] colorPalette = new Palette(dib).ColorData;
				// Copy each value
				if (palette.Entries.Length == colorPalette.Length)
				{
					for (int i = 0; i < colorPalette.Length; i++)
					{
						palette.Entries[i] = colorPalette[i];
					}
					// Set the bitmaps palette
					result.Palette = palette;
				}
			}
			// Copy metadata
			if (copyMetadata)
			{
				try
				{
					List<PropertyItem> list = new List<PropertyItem>();
					// Get a list of all types
					FITAG tag;
					FIMETADATA mData;
					foreach (FREE_IMAGE_MDMODEL model in FREE_IMAGE_MDMODELS)
					{
						// Get a unique search handle
						mData = FindFirstMetadata(model, dib, out tag);
						// Check if metadata exists for this type
						if (mData.IsNull) continue;
						do
						{
							PropertyItem propItem = CreatePropertyItem();
							propItem.Len = (int)GetTagLength(tag);
							propItem.Id = (int)GetTagID(tag);
							propItem.Type = (short)GetTagType(tag);
							byte[] buffer = new byte[propItem.Len];

							unsafe
							{
								byte* src = (byte*)GetTagValue(tag);
								fixed (byte* dst = buffer)
								{
									MoveMemory(dst, src, (uint)propItem.Len);
								}
							}

							propItem.Value = buffer;
							list.Add(propItem);
						}
						while (FindNextMetadata(mData, out tag));
						FindCloseMetadata(mData);
					}
					foreach (PropertyItem propItem in list)
					{
						result.SetPropertyItem(propItem);
					}
				}
				catch
				{
				}
			}
			return result;
		}

		/// <summary>
		/// Converts an .NET <see cref="System.Drawing.Bitmap"/> into a FreeImage bitmap.
		/// </summary>
		/// <param name="bitmap">The <see cref="System.Drawing.Bitmap"/> to convert.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <remarks>Copying metadata has been disabled until a proper way
		/// of reading and storing metadata in a .NET bitmap is found.</remarks>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="bitmap"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// The bitmaps pixelformat is invalid.</exception>
		public static FIBITMAP CreateFromBitmap(Bitmap bitmap)
		{
			return CreateFromBitmap(bitmap, false);
		}

		/// <summary>
		/// Converts an .NET <see cref="System.Drawing.Bitmap"/> into a FreeImage bitmap.
		/// </summary>
		/// <param name="bitmap">The <see cref="System.Drawing.Bitmap"/> to convert.</param>
		/// <param name="copyMetadata">When true existing metadata will be copied.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <remarks>Copying metadata has been disabled until a proper way
		/// of reading and storing metadata in a .NET bitmap is found.</remarks>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="bitmap"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// The bitmaps pixelformat is invalid.</exception>
		internal static FIBITMAP CreateFromBitmap(Bitmap bitmap, bool copyMetadata)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			uint bpp, red_mask, green_mask, blue_mask;
			FREE_IMAGE_TYPE type;
			if (!GetFormatParameters(bitmap.PixelFormat, out type, out bpp, out red_mask, out green_mask, out blue_mask))
			{
				throw new ArgumentException("The bitmaps pixelformat is invalid.");
			}

			// Locking the complete bitmap in readonly mode
			BitmapData data = bitmap.LockBits(
				new Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadOnly, bitmap.PixelFormat);
			// Copying the bitmap data directly from the .NET bitmap
			FIBITMAP result =
				ConvertFromRawBits(
					data.Scan0,
					type,
					data.Width,
					data.Height,
					data.Stride,
					bpp,
					red_mask,
					green_mask,
					blue_mask,
					true);
			bitmap.UnlockBits(data);
			// Handle palette
			if (GetPalette(result) != IntPtr.Zero)
			{
				Palette palette = new Palette(result);
				if (palette.Length == bitmap.Palette.Entries.Length)
				{
					for (int i = 0; i < palette.Length; i++)
					{
						palette[i] = (RGBQUAD)bitmap.Palette.Entries[i];
					}
				}
			}
			// Handle meta data
			// Disabled
			//if (copyMetadata)
			//{
			//    foreach (PropertyItem propItem in bitmap.PropertyItems)
			//    {
			//        FITAG tag = CreateTag();
			//        SetTagLength(tag, (uint)propItem.Len);
			//        SetTagID(tag, (ushort)propItem.Id);
			//        SetTagType(tag, (FREE_IMAGE_MDTYPE)propItem.Type);
			//        SetTagValue(tag, propItem.Value);
			//        SetMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, result, "", tag);
			//    }
			//}
			return result;
		}

		/// <summary>
		/// Converts a raw bitmap somewhere in memory to a FreeImage bitmap.
		/// The parameters in this function are used to describe the raw bitmap.
		/// </summary>
		/// <param name="bits">Pointer to start of the raw bits.</param>
		/// <param name="type">Type of the bitmap.</param>
		/// <param name="width">Width of the bitmap.</param>
		/// <param name="height">Height of the bitmap.</param>
		/// <param name="pitch">Defines the total width of a scanline in the source bitmap,
		/// including padding bytes that may be applied.</param>
		/// <param name="bpp">The bit depth of the bitmap.</param>
		/// <param name="red_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="green_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="blue_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="topdown">Stores the bitmap top-left pixel first when it is true
		/// or bottom-left pixel first when it is false</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		public static unsafe FIBITMAP ConvertFromRawBits(
			IntPtr bits,
			FREE_IMAGE_TYPE type,
			int width,
			int height,
			int pitch,
			uint bpp,
			uint red_mask,
			uint green_mask,
			uint blue_mask,
			bool topdown)
		{
			byte* addr = (byte*)bits;
			if ((addr == null) || (width <= 0) || (height <= 0))
			{
				return FIBITMAP.Zero;
			}

			FIBITMAP dib = AllocateT(type, width, height, (int)bpp, red_mask, green_mask, blue_mask);
			if (dib != FIBITMAP.Zero)
			{
				if (topdown)
				{
					for (int i = height - 1; i >= 0; --i)
					{
						CopyMemory((byte*)GetScanLine(dib, i), addr, (int)GetLine(dib));
						addr += pitch;
					}
				}
				else
				{
					for (int i = 0; i < height; ++i)
					{
						CopyMemory((byte*)GetScanLine(dib, i), addr, (int)GetLine(dib));
						addr += pitch;
					}
				}
			}
			return dib;
		}

		/// <summary>
		/// Saves a .NET <see cref="System.Drawing.Bitmap"/> to a file.
		/// </summary>
		/// <param name="bitmap">The .NET <see cref="System.Drawing.Bitmap"/> to save.</param>
		/// <param name="filename">Name of the file to save to.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="bitmap"/> or <paramref name="filename"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// The bitmaps pixelformat is invalid.</exception>
		public static bool SaveBitmap(
			Bitmap bitmap,
			string filename)
		{
			return SaveBitmap(
				bitmap,
				filename,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Saves a .NET <see cref="System.Drawing.Bitmap"/> to a file.
		/// </summary>
		/// <param name="bitmap">The .NET <see cref="System.Drawing.Bitmap"/> to save.</param>
		/// <param name="filename">Name of the file to save to.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="bitmap"/> or <paramref name="filename"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// The bitmaps pixelformat is invalid.</exception>
		public static bool SaveBitmap(
			Bitmap bitmap,
			string filename,
			FREE_IMAGE_SAVE_FLAGS flags)
		{
			return SaveBitmap(
				bitmap,
				filename,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				flags);
		}

		/// <summary>
		/// Saves a .NET <see cref="System.Drawing.Bitmap"/> to a file.
		/// </summary>
		/// <param name="bitmap">The .NET <see cref="System.Drawing.Bitmap"/> to save.</param>
		/// <param name="filename">Name of the file to save to.</param>
		/// <param name="format">Format of the bitmap. If the format should be taken from the
		/// filename use <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="bitmap"/> or <paramref name="filename"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// The bitmaps pixelformat is invalid.</exception>
		public static bool SaveBitmap(
			Bitmap bitmap,
			string filename,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_SAVE_FLAGS flags)
		{
			FIBITMAP dib = CreateFromBitmap(bitmap);
			bool result = SaveEx(dib, filename, format, flags);
			Unload(dib);
			return result;
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// The file will be loaded with default loading flags.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="filename"/> does not exists.</exception>
		public static FIBITMAP LoadEx(string filename)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return LoadEx(filename, FREE_IMAGE_LOAD_FLAGS.DEFAULT, ref format);
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// Load flags can be provided by the flags parameter.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="filename"/> does not exists.</exception>
		public static FIBITMAP LoadEx(string filename, FREE_IMAGE_LOAD_FLAGS flags)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return LoadEx(filename, flags, ref format);
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// In case the loading format is <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/> the files
		/// real format is being analysed. If no plugin can read the file, format remains
		/// <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/> and 0 is returned.
		/// The file will be loaded with default loading flags.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="format">Format of the image. If the format is unknown use
		/// <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>.
		/// In case a suitable format was found by LoadEx it will be returned in format.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="filename"/> does not exists.</exception>
		public static FIBITMAP LoadEx(string filename, ref FREE_IMAGE_FORMAT format)
		{
			return LoadEx(filename, FREE_IMAGE_LOAD_FLAGS.DEFAULT, ref format);
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// In case the loading format is <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/> the files
		/// real format is being analysed. If no plugin can read the file, format remains
		/// <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/> and 0 is returned.
		/// Load flags can be provided by the flags parameter.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="format">Format of the image. If the format is unknown use
		/// <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>.
		/// In case a suitable format was found by LoadEx it will be returned in format.
		/// </param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="filename"/> does not exists.</exception>
		public static FIBITMAP LoadEx(string filename, FREE_IMAGE_LOAD_FLAGS flags, ref FREE_IMAGE_FORMAT format)
		{
			// check if file exists
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException(filename + " could not be found.");
			}
			FIBITMAP dib = new FIBITMAP();
			if (format == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
			{
				// query all plugins to see if one can read the file
				format = GetFileType(filename, 0);
			}
			// check if the plugin is capable of loading files
			if (FIFSupportsReading(format))
			{
				dib = Load(format, filename, flags);
			}
			return dib;
		}

		/// <summary>
		/// Loads a .NET <see cref="System.Drawing.Bitmap"/> from a file.
		/// </summary>
		/// <param name="filename">Name of the file to be loaded.</param>
		/// <param name="format">Format of the image. If the format should be taken from the
		/// filename use <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>The loaded .NET <see cref="System.Drawing.Bitmap"/>.</returns>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="filename"/> does not exists.</exception>
		/// <exception cref="ArgumentException">
		/// The image type of the image is not <see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/>.</exception>
		public static Bitmap LoadBitmap(string filename, FREE_IMAGE_LOAD_FLAGS flags, ref FREE_IMAGE_FORMAT format)
		{
			FIBITMAP dib = LoadEx(filename, flags, ref format);
			Bitmap result = GetBitmap(dib, true);
			Unload(dib);
			return result;
		}

		/// <summary>
		/// Deletes a previously loaded FreeImage bitmap from memory and resets the handle to 0.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public static void UnloadEx(ref FIBITMAP dib)
		{
			if (!dib.IsNull)
			{
				Unload(dib);
				dib.SetNull();
			}
		}

		/// <summary>
		/// Saves a previously loaded FreeImage bitmap to a file.
		/// The format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			FIBITMAP dib,
			string filename)
		{
			return SaveEx(
				ref dib,
				filename,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				false);
		}

		/// <summary>
		/// Saves a previously loaded FreeImage bitmap to a file.
		/// In case the loading format is <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>
		/// the format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified.</param>
		/// <param name="format">Format of the image. If the format should be taken from the
		/// filename use <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			FIBITMAP dib,
			string filename,
			FREE_IMAGE_FORMAT format)
		{
			return SaveEx(
				ref dib,
				filename,
				format,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				false);
		}

		/// <summary>
		/// Saves a previously loaded FreeImage bitmap to a file.
		/// The format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.
		/// If the function failed and returned false, the bitmap was not unloaded.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			ref FIBITMAP dib,
			string filename,
			bool unloadSource)
		{
			return SaveEx(
				ref dib,
				filename,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				unloadSource);
		}

		/// <summary>
		/// Saves a previously loaded FreeImage bitmap to a file.
		/// The format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// Save flags can be provided by the flags parameter.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			FIBITMAP dib,
			string filename,
			FREE_IMAGE_SAVE_FLAGS flags)
		{
			return SaveEx(
				ref dib,
				filename,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				flags,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				false);
		}

		/// <summary>
		/// Saves a previously loaded FreeImage bitmap to a file.
		/// The format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// Save flags can be provided by the flags parameter.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.
		/// If the function failed and returned false, the bitmap was not unloaded.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			ref FIBITMAP dib,
			string filename,
			FREE_IMAGE_SAVE_FLAGS flags,
			bool unloadSource)
		{
			return SaveEx(
				ref dib,
				filename,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				flags,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				unloadSource);
		}

		/// <summary>
		/// Saves a previously loaded FreeImage bitmap to a file.
		/// In case the loading format is <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>
		/// the format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified.</param>
		/// <param name="format">Format of the image. If the format should be taken from the
		/// filename use <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.
		/// If the function failed and returned false, the bitmap was not unloaded.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			ref FIBITMAP dib,
			string filename,
			FREE_IMAGE_FORMAT format,
			bool unloadSource)
		{
			return SaveEx(
				ref dib,
				filename,
				format,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				unloadSource);
		}

		/// <summary>
		/// Saves a previously loaded FreeImage bitmap to a file.
		/// In case the loading format is <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>
		/// the format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// Save flags can be provided by the flags parameter.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified.</param>
		/// <param name="format">Format of the image. If the format should be taken from the
		/// filename use <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			FIBITMAP dib,
			string filename,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_SAVE_FLAGS flags)
		{
			return SaveEx(
				ref dib,
				filename,
				format,
				flags,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				false);
		}

		/// <summary>
		/// Saves a previously loaded FreeImage bitmap to a file.
		/// In case the loading format is <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>
		/// the format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// Save flags can be provided by the flags parameter.
		/// The bitmaps color depth can be set by 'colorDepth'.
		/// If set to <see cref="FREE_IMAGE_COLOR_DEPTH.FICD_AUTO"/> a suitable color depth
		/// will be taken if available.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified.</param>
		/// <param name="format">Format of the image. If the format should be taken from the
		/// filename use <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="colorDepth">The new color depth of the bitmap.
		/// Set to <see cref="FREE_IMAGE_COLOR_DEPTH.FICD_AUTO"/> if Save should take the
		/// best suitable color depth.
		/// If a color depth is selected that the provided format cannot write an
		/// error-message will be thrown.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.
		/// If the function failed and returned false, the bitmap was not unloaded.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentException">
		/// A direct color conversion failed.</exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			ref FIBITMAP dib,
			string filename,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_SAVE_FLAGS flags,
			FREE_IMAGE_COLOR_DEPTH colorDepth,
			bool unloadSource)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			bool result = false;
			// Gets format from filename if the format is unknown
			if (format == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
			{
				format = GetFIFFromFilename(filename);
			}
			if (format != FREE_IMAGE_FORMAT.FIF_UNKNOWN)
			{
				// Checks writing support
				if (FIFSupportsWriting(format) && FIFSupportsExportType(format, GetImageType(dib)))
				{
					// Check valid filename and correct it if needed
					if (!IsFilenameValidForFIF(format, filename))
					{
						int index = filename.LastIndexOf('.');
						string extension = GetPrimaryExtensionFromFIF(format);

						if (index == -1)
						{
							// We have no '.' (dot) so just add the extension
							filename += "." + extension;
						}
						else
						{
							// Overwrite the old extension
							filename = filename.Substring(0, filename.LastIndexOf('.')) + extension;
						}
					}

					FIBITMAP dibToSave = PrepareBitmapColorDepth(dib, format, colorDepth);
					result = Save(format, dibToSave, filename, flags);

					// Always unload a temporary created bitmap.
					if (dibToSave != dib)
					{
						UnloadEx(ref dibToSave);
					}
					// On success unload the bitmap
					if (result && unloadSource)
					{
						UnloadEx(ref dib);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// The stream must be set to the correct position before calling LoadFromStream.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="stream"/> is not capable of reading.</exception>
		public static FIBITMAP LoadFromStream(
			Stream stream)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return LoadFromStream(stream, FREE_IMAGE_LOAD_FLAGS.DEFAULT, ref format);
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// The stream must be set to the correct position before calling LoadFromStream.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="stream"/> is not capable of reading.</exception>
		public static FIBITMAP LoadFromStream(
			Stream stream,
			FREE_IMAGE_LOAD_FLAGS flags)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return LoadFromStream(stream, flags, ref format);
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// In case the loading format is <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/> the
		/// bitmaps real format is being analysed.
		/// The stream must be set to the correct position before calling LoadFromStream.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="format">Format of the image. If the format is unknown use
		/// <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>.
		/// In case a suitable format was found by LoadFromStream it will be returned in format.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="stream"/> is not capable of reading.</exception>
		public static FIBITMAP LoadFromStream(
			Stream stream,
			ref FREE_IMAGE_FORMAT format)
		{
			return LoadFromStream(stream, FREE_IMAGE_LOAD_FLAGS.DEFAULT, ref format);
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// In case the loading format is <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>
		/// the bitmaps real format is being analysed.
		/// The stream must be set to the correct position before calling LoadFromStream.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="format">Format of the image. If the format is unknown use
		/// <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>.
		/// In case a suitable format was found by LoadFromStream it will be returned in format.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="stream"/> is not capable of reading.</exception>
		public static FIBITMAP LoadFromStream(
			Stream stream,
			FREE_IMAGE_LOAD_FLAGS flags,
			ref FREE_IMAGE_FORMAT format)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("stream is not capable of reading.");
			}
			// Wrap the source stream if it is unable to seek (which is required by FreeImage)
			stream = (stream.CanSeek) ? stream : new StreamWrapper(stream, true);
			// Save the streams position
			if (format == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
			{
				long position = stream.Position;
				// Get the format of the bitmap
				format = GetFileTypeFromStream(stream);
				// Restore the streams position
				stream.Position = position;
			}
			if (!FIFSupportsReading(format))
			{
				return FIBITMAP.Zero;
			}
			// Create a 'FreeImageIO' structure for calling 'LoadFromHandle'
			// using the internal structure 'FreeImageStreamIO'.
			FreeImageIO io = FreeImageStreamIO.io;
			using (fi_handle handle = new fi_handle(stream))
			{
				return LoadFromHandle(format, ref io, handle, flags);
			}
		}

		/// <summary>
		/// Saves a previously loaded FreeImage bitmap to a stream.
		/// The stream must be set to the correct position before calling SaveToStream.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="format">Format of the image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="stream"/> cannot write.</exception>
		public static bool SaveToStream(
			FIBITMAP dib,
			Stream stream,
			FREE_IMAGE_FORMAT format)
		{
			return SaveToStream(
				ref dib,
				stream,
				format,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				false);
		}

		/// <summary>
		/// Saves a previously loaded FreeImage bitmap to a stream.
		/// The stream must be set to the correct position before calling SaveToStream.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="stream"/> cannot write.</exception>
		public static bool SaveToStream(
			ref FIBITMAP dib,
			Stream stream,
			FREE_IMAGE_FORMAT format,
			bool unloadSource)
		{
			return SaveToStream(
				ref dib,
				stream,
				format,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				unloadSource);
		}

		/// <summary>
		/// Saves a previously loaded FreeImage bitmap to a stream.
		/// The stream must be set to the correct position before calling SaveToStream.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="stream"/> cannot write.</exception>
		public static bool SaveToStream(
			FIBITMAP dib,
			Stream stream,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_SAVE_FLAGS flags)
		{
			return SaveToStream(
				ref dib,
				stream,
				format,
				flags,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				false);
		}

		/// <summary>
		/// Saves a previously loaded FreeImage bitmap to a stream.
		/// The stream must be set to the correct position before calling SaveToStream.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="stream"/> cannot write.</exception>
		public static bool SaveToStream(
			ref FIBITMAP dib,
			Stream stream,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_SAVE_FLAGS flags,
			bool unloadSource)
		{
			return SaveToStream(
				ref dib, stream,
				format,
				flags,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				unloadSource);
		}

		/// <summary>
		/// Saves a previously loaded FreeImage bitmap to a stream.
		/// The stream must be set to the correct position before calling SaveToStream.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="colorDepth">The new color depth of the bitmap.
		/// Set to <see cref="FREE_IMAGE_COLOR_DEPTH.FICD_AUTO"/> if SaveToStream should
		/// take the best suitable color depth.
		/// If a color depth is selected that the provided format cannot write an
		/// error-message will be thrown.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="stream"/> cannot write.</exception>
		public static bool SaveToStream(
			FIBITMAP dib,
			Stream stream,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_SAVE_FLAGS flags,
			FREE_IMAGE_COLOR_DEPTH colorDepth)
		{
			return SaveToStream(
				ref dib,
				stream,
				format,
				flags,
				colorDepth,
				false);
		}

		/// <summary>
		/// Saves a previously loaded FreeImage bitmap to a stream.
		/// The stream must be set to the correct position before calling SaveToStream.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="colorDepth">The new color depth of the bitmap.
		/// Set to <see cref="FREE_IMAGE_COLOR_DEPTH.FICD_AUTO"/> if SaveToStream should
		/// take the best suitable color depth.
		/// If a color depth is selected that the provided format cannot write an
		/// error-message will be thrown.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="stream"/> cannot write.</exception>
		public static bool SaveToStream(
			ref FIBITMAP dib,
			Stream stream,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_SAVE_FLAGS flags,
			FREE_IMAGE_COLOR_DEPTH colorDepth,
			bool unloadSource)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException("stream is not capable of writing.");
			}
			if ((!FIFSupportsWriting(format)) || (!FIFSupportsExportType(format, FREE_IMAGE_TYPE.FIT_BITMAP)))
			{
				return false;
			}

			FIBITMAP dibToSave = PrepareBitmapColorDepth(dib, format, colorDepth);
			bool result = false;

			try
			{
				// Create a 'FreeImageIO' structure for calling 'SaveToHandle'
				FreeImageIO io = FreeImageStreamIO.io;

				using (fi_handle handle = new fi_handle(stream))
				{
					result = SaveToHandle(format, dibToSave, ref io, handle, flags);
				}
			}
			finally
			{
				// Always unload a temporary created bitmap.
				if (dibToSave != dib)
				{
					UnloadEx(ref dibToSave);
				}
				// On success unload the bitmap
				if (result && unloadSource)
				{
					UnloadEx(ref dib);
				}
			}

			return result;
		}

		#endregion

		#region Plugin functions

		/// <summary>
		/// Checks if an extension is valid for a certain format.
		/// </summary>
		/// <param name="fif">The desired format.</param>
		/// <param name="extension">The desired extension.</param>
		/// <returns>True if the extension is valid for the given format, false otherwise.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="extension"/> is null.</exception>
		public static bool IsExtensionValidForFIF(FREE_IMAGE_FORMAT fif, string extension)
		{
			return IsExtensionValidForFIF(fif, extension, StringComparison.CurrentCulture);
		}

		/// <summary>
		/// Checks if an extension is valid for a certain format.
		/// </summary>
		/// <param name="fif">The desired format.</param>
		/// <param name="extension">The desired extension.</param>
		/// <param name="comparisonType">The string comparison type.</param>
		/// <returns>True if the extension is valid for the given format, false otherwise.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="extension"/> is null.</exception>
		public static bool IsExtensionValidForFIF(FREE_IMAGE_FORMAT fif, string extension, StringComparison comparisonType)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			bool result = false;
			// Split up the string and compare each with the given extension
			string tempList = GetFIFExtensionList(fif);
			if (tempList != null)
			{
				string[] extensionList = tempList.Split(',');
				foreach (string ext in extensionList)
				{
					if (extension.Equals(ext, comparisonType))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Checks if a filename is valid for a certain format.
		/// </summary>
		/// <param name="fif">The desired format.</param>
		/// <param name="filename">The desired filename.</param>
		/// <returns>True if the filename is valid for the given format, false otherwise.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="filename"/> is null.</exception>
		public static bool IsFilenameValidForFIF(FREE_IMAGE_FORMAT fif, string filename)
		{
			return IsFilenameValidForFIF(fif, filename, StringComparison.CurrentCulture);
		}

		/// <summary>
		/// Checks if a filename is valid for a certain format.
		/// </summary>
		/// <param name="fif">The desired format.</param>
		/// <param name="filename">The desired filename.</param>
		/// <param name="comparisonType">The string comparison type.</param>
		/// <returns>True if the filename is valid for the given format, false otherwise.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="filename"/> is null.</exception>
		public static bool IsFilenameValidForFIF(FREE_IMAGE_FORMAT fif, string filename, StringComparison comparisonType)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			bool result = false;
			// Extract the filenames extension if it exists
			int position = filename.LastIndexOf('.');
			if (position >= 0)
			{
				result = IsExtensionValidForFIF(fif, filename.Substring(position + 1), comparisonType);
			}
			return result;
		}

		/// <summary>
		/// This function returns the primary (main or most commonly used?) extension of a certain
		/// image format (fif). This is done by returning the first of all possible extensions
		/// returned by GetFIFExtensionList().
		/// That assumes, that the plugin returns the extensions in ordered form.</summary>
		/// <param name="fif">The image format to obtain the primary extension for.</param>
		/// <returns>The primary extension of the specified image format.</returns>
		public static string GetPrimaryExtensionFromFIF(FREE_IMAGE_FORMAT fif)
		{
			string result = null;
			string extensions = GetFIFExtensionList(fif);
			if (extensions != null)
			{
				int position = extensions.IndexOf(',');
				if (position < 0)
				{
					result = extensions;
				}
				else
				{
					result = extensions.Substring(0, position);
				}
			}
			return result;
		}

		#endregion

		#region Multipage functions

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="filename"/> does not exists while opening.</exception>
		public static FIMULTIBITMAP OpenMultiBitmapEx(
			string filename)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return OpenMultiBitmapEx(
				filename,
				ref format,
				FREE_IMAGE_LOAD_FLAGS.DEFAULT,
				false,
				false,
				false);
		}

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="keep_cache_in_memory">When true performance is increased at the cost of memory.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="filename"/> does not exists while opening.</exception>
		public static FIMULTIBITMAP OpenMultiBitmapEx(
			string filename,
			bool keep_cache_in_memory)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return OpenMultiBitmapEx(
				filename,
				ref format,
				FREE_IMAGE_LOAD_FLAGS.DEFAULT,
				false,
				false,
				keep_cache_in_memory);
		}

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="read_only">When true the bitmap will be loaded read only.</param>
		/// <param name="keep_cache_in_memory">When true performance is increased at the cost of memory.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="filename"/> does not exists while opening.</exception>
		public static FIMULTIBITMAP OpenMultiBitmapEx(
			string filename,
			bool read_only,
			bool keep_cache_in_memory)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return OpenMultiBitmapEx(
				filename,
				ref format,
				FREE_IMAGE_LOAD_FLAGS.DEFAULT,
				false,
				read_only,
				keep_cache_in_memory);
		}

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="create_new">When true a new bitmap is created.</param>
		/// <param name="read_only">When true the bitmap will be loaded read only.</param>
		/// <param name="keep_cache_in_memory">When true performance is increased at the cost of memory.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="filename"/> does not exists while opening.</exception>
		public static FIMULTIBITMAP OpenMultiBitmapEx(
			string filename,
			bool create_new,
			bool read_only,
			bool keep_cache_in_memory)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return OpenMultiBitmapEx(
				filename,
				ref format,
				FREE_IMAGE_LOAD_FLAGS.DEFAULT,
				create_new,
				read_only,
				keep_cache_in_memory);
		}

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap.
		/// In case the loading format is <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/> the files real
		/// format is being analysed. If no plugin can read the file, format remains
		/// <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/> and 0 is returned.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="format">Format of the image. If the format is unknown use
		/// <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>.
		/// In case a suitable format was found by LoadEx it will be returned in format.</param>
		/// <param name="create_new">When true a new bitmap is created.</param>
		/// <param name="read_only">When true the bitmap will be loaded read only.</param>
		/// <param name="keep_cache_in_memory">When true performance is increased at the cost of memory.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="filename"/> does not exists while opening.</exception>
		public static FIMULTIBITMAP OpenMultiBitmapEx(
			string filename,
			ref FREE_IMAGE_FORMAT format,
			bool create_new,
			bool read_only,
			bool keep_cache_in_memory)
		{
			return OpenMultiBitmapEx(
				filename,
				ref format,
				FREE_IMAGE_LOAD_FLAGS.DEFAULT,
				create_new,
				read_only,
				keep_cache_in_memory);
		}

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap.
		/// In case the loading format is <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/> the files
		/// real format is being analysed. If no plugin can read the file, format remains
		/// <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/> and 0 is returned.
		/// Load flags can be provided by the flags parameter.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="format">Format of the image. If the format is unknown use 
		/// <see cref="FREE_IMAGE_FORMAT.FIF_UNKNOWN"/>.
		/// In case a suitable format was found by LoadEx it will be returned in format.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="create_new">When true a new bitmap is created.</param>
		/// <param name="read_only">When true the bitmap will be loaded read only.</param>
		/// <param name="keep_cache_in_memory">When true performance is increased at the cost of memory.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="filename"/> does not exists while opening.</exception>
		public static FIMULTIBITMAP OpenMultiBitmapEx(
			string filename,
			ref FREE_IMAGE_FORMAT format,
			FREE_IMAGE_LOAD_FLAGS flags,
			bool create_new,
			bool read_only,
			bool keep_cache_in_memory)
		{
			if (!File.Exists(filename) && !create_new)
			{
				throw new FileNotFoundException(filename + " could not be found.");
			}
			if (format == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
			{
				// Check if a plugin can read the data
				format = GetFileType(filename, 0);
			}
			FIMULTIBITMAP dib = new FIMULTIBITMAP();
			if (FIFSupportsReading(format))
			{
				dib = OpenMultiBitmap(format, filename, create_new, read_only, keep_cache_in_memory, flags);
			}
			return dib;
		}

		/// <summary>
		/// Closes a previously opened multi-page bitmap and, when the bitmap was not opened read-only,
		/// applies any changes made to it.
		/// On success the handle will be reset to null.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage multi-paged bitmap.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool CloseMultiBitmapEx(ref FIMULTIBITMAP dib)
		{
			return CloseMultiBitmapEx(ref dib, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Closes a previously opened multi-page bitmap and, when the bitmap was not opened read-only,
		/// applies any changes made to it.
		/// On success the handle will be reset to null.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool CloseMultiBitmapEx(ref FIMULTIBITMAP dib, FREE_IMAGE_SAVE_FLAGS flags)
		{
			bool result = false;
			if (!dib.IsNull)
			{
				if (CloseMultiBitmap(dib, flags))
				{
					dib.SetNull();
					result = true;
				}
			}
			return result;
		}

		/// <summary>
		/// Retrieves the number of pages that are locked in a multi-paged bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage multi-paged bitmap.</param>
		/// <returns>Number of locked pages.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static int GetLockedPageCount(FIMULTIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			int result = 0;
			GetLockedPageNumbers(dib, null, ref result);
			return result;
		}

		/// <summary>
		/// Retrieves a list locked pages of a multi-paged bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage multi-paged bitmap.</param>
		/// <returns>List containing the indexes of the locked pages.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static int[] GetLockedPages(FIMULTIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			// Get the number of pages and create an array to save the information
			int count = 0;
			int[] result = null;
			// Get count
			if (GetLockedPageNumbers(dib, result, ref count))
			{
				result = new int[count];
				// Fill array
				if (!GetLockedPageNumbers(dib, result, ref count))
				{
					result = null;
				}
			}
			return result;
		}

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap from a stream and returns the
		/// FreeImage memory stream used as temporary buffer.
		/// The bitmap can not be modified by calling
		/// <see cref="FreeImage.AppendPage(FIMULTIBITMAP,FIBITMAP)"/>,
		/// <see cref="FreeImage.InsertPage(FIMULTIBITMAP,Int32,FIBITMAP)"/>,
		/// <see cref="FreeImage.MovePage(FIMULTIBITMAP,Int32,Int32)"/> or
		/// <see cref="FreeImage.DeletePage(FIMULTIBITMAP,Int32)"/>.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="memory">The temporary memory buffer used to load the bitmap.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="stream"/> can not read.</exception>
		public static FIMULTIBITMAP LoadMultiBitmapFromStream(
			Stream stream,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_LOAD_FLAGS flags,
			out FIMEMORY memory)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("stream");
			}
			const int blockSize = 1024;
			int bytesRead;
			byte[] buffer = new byte[blockSize];

			stream = stream.CanSeek ? stream : new StreamWrapper(stream, true);
			memory = OpenMemory(IntPtr.Zero, 0);

			do
			{
				bytesRead = stream.Read(buffer, 0, blockSize);
				WriteMemory(buffer, (uint)blockSize, (uint)1, memory);
			}
			while (bytesRead == blockSize);

			return LoadMultiBitmapFromMemory(format, memory, flags);
		}

		#endregion

		#region Filetype functions

		/// <summary>
		/// Orders FreeImage to analyze the bitmap signature.
		/// In case the stream is not seekable, the stream will have been used
		/// and must be recreated for loading.
		/// </summary>
		/// <param name="stream">Name of the stream to analyze.</param>
		/// <returns>Type of the bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="stream"/> can not read.</exception>
		public static FREE_IMAGE_FORMAT GetFileTypeFromStream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("stream is not capable of reading.");
			}
			// Wrap the stream if it cannot seek
			stream = (stream.CanSeek) ? stream : new StreamWrapper(stream, true);
			// Create a 'FreeImageIO' structure for the stream
			FreeImageIO io = FreeImageStreamIO.io;
			using (fi_handle handle = new fi_handle(stream))
			{
				return GetFileTypeFromHandle(ref io, handle, 0);
			}
		}

		#endregion

		#region Pixel access functions

		/// <summary>
		/// Retrieves an hBitmap for a FreeImage bitmap.
		/// Call FreeHbitmap(IntPtr) to free the handle.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="hdc">A reference device context.
		/// Use IntPtr.Zero if no reference is available.</param>
		/// <param name="unload">When true dib will be unloaded if the function succeeded.</param>
		/// <returns>The hBitmap for the FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static unsafe IntPtr GetHbitmap(FIBITMAP dib, IntPtr hdc, bool unload)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			IntPtr hBitmap = IntPtr.Zero;
			bool release = false;
			IntPtr ppvBits = IntPtr.Zero;
			// Check if we have destination
			if (release = (hdc == IntPtr.Zero))
			{
				// We don't so request dc
				hdc = GetDC(IntPtr.Zero);
			}
			if (hdc != IntPtr.Zero)
			{
				// Get pointer to the infoheader of the bitmap
				IntPtr info = GetInfo(dib);
				// Create a bitmap in the dc
				hBitmap = CreateDIBSection(hdc, info, DIB_RGB_COLORS, out ppvBits, IntPtr.Zero, 0);
				if (hBitmap != IntPtr.Zero && ppvBits != IntPtr.Zero)
				{
					// Copy the data into the dc
					CopyMemory(
						ppvBits,
						GetBits(dib),
						(GetHeight(dib) * GetPitch(dib)));
					// Success: we unload the bitmap
					if (unload)
					{
						Unload(dib);
					}
				}
				// We have to release the dc
				if (release)
				{
					ReleaseDC(IntPtr.Zero, hdc);
				}
			}
			return hBitmap;
		}

		/// <summary>
		/// Returns an HBITMAP created by the <c>CreateDIBitmap()</c> function which in turn
		/// has always the same color depth as the reference DC, which may be provided
		/// through <paramref name="hdc"/>. The desktop DC will be used,
		/// if <c>IntPtr.Zero</c> DC is specified.
		/// Call <see cref="FreeImage.FreeHbitmap(IntPtr)"/> to free the handle.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="hdc">Handle to a device context.</param>
		/// <param name="unload">When true the structure will be unloaded on success.
		/// If the function failed and returned false, the bitmap was not unloaded.</param>
		/// <returns>If the function succeeds, the return value is a handle to the
		/// compatible bitmap. If the function fails, the return value is <see cref="IntPtr.Zero"/>.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static IntPtr GetBitmapForDevice(FIBITMAP dib, IntPtr hdc, bool unload)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			IntPtr hbitmap = IntPtr.Zero;
			bool release = false;
			if (release = (hdc == IntPtr.Zero))
			{
				hdc = GetDC(IntPtr.Zero);
			}
			if (hdc != IntPtr.Zero)
			{
				hbitmap = CreateDIBitmap(
					hdc,
					GetInfoHeader(dib),
					CBM_INIT,
					GetBits(dib),
					GetInfo(dib),
					DIB_RGB_COLORS);
				if (unload)
				{
					Unload(dib);
				}
				if (release)
				{
					ReleaseDC(IntPtr.Zero, hdc);
				}
			}
			return hbitmap;
		}

		/// <summary>
		/// Creates a FreeImage DIB from a Device Context/Compatible Bitmap.
		/// </summary>
		/// <param name="hbitmap">Handle to the bitmap.</param>
		/// <param name="hdc">Handle to a device context.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="hbitmap"/> is null.</exception>
		public unsafe static FIBITMAP CreateFromHbitmap(IntPtr hbitmap, IntPtr hdc)
		{
			if (hbitmap == IntPtr.Zero)
			{
				throw new ArgumentNullException("hbitmap");
			}

			FIBITMAP dib = new FIBITMAP();
			BITMAP bm;
			uint colors;
			bool release;

			if (GetObject(hbitmap, sizeof(BITMAP), (IntPtr)(&bm)) != 0)
			{
				dib = Allocate(bm.bmWidth, bm.bmHeight, bm.bmBitsPixel, 0, 0, 0);
				if (!dib.IsNull)
				{
					colors = GetColorsUsed(dib);
					if (release = (hdc == IntPtr.Zero))
					{
						hdc = GetDC(IntPtr.Zero);
					}
					if (GetDIBits(
							hdc,
							hbitmap,
							0,
							(uint)bm.bmHeight,
							GetBits(dib),
							GetInfo(dib),
							DIB_RGB_COLORS) != 0)
					{
						if (colors != 0)
						{
							BITMAPINFOHEADER* bmih = (BITMAPINFOHEADER*)GetInfo(dib);
							bmih[0].biClrImportant = bmih[0].biClrUsed = colors;
						}
					}
					else
					{
						UnloadEx(ref dib);
					}
					if (release)
					{
						ReleaseDC(IntPtr.Zero, hdc);
					}
				}
			}

			return dib;
		}

		/// <summary>
		/// Frees a bitmap handle.
		/// </summary>
		/// <param name="hbitmap">Handle to a bitmap.</param>
		/// <returns>True on success, false on failure.</returns>
		public static bool FreeHbitmap(IntPtr hbitmap)
		{
			return DeleteObject(hbitmap);
		}

		#endregion

		#region Bitmap information functions

		/// <summary>
		/// Retrieves a DIB's resolution in X-direction measured in 'dots per inch' (DPI) and not in
		/// 'dots per meter'.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The resolution in 'dots per inch'.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static uint GetResolutionX(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			return (uint)(0.5d + 0.0254d * GetDotsPerMeterX(dib));
		}

		/// <summary>
		/// Retrieves a DIB's resolution in Y-direction measured in 'dots per inch' (DPI) and not in
		/// 'dots per meter'.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The resolution in 'dots per inch'.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static uint GetResolutionY(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			return (uint)(0.5d + 0.0254d * GetDotsPerMeterY(dib));
		}

		/// <summary>
		/// Sets a DIB's resolution in X-direction measured in 'dots per inch' (DPI) and not in
		/// 'dots per meter'.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="res">The new resolution in 'dots per inch'.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static void SetResolutionX(FIBITMAP dib, uint res)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			SetDotsPerMeterX(dib, (uint)((double)res / 0.0254d + 0.5d));
		}

		/// <summary>
		/// Sets a DIB's resolution in Y-direction measured in 'dots per inch' (DPI) and not in
		/// 'dots per meter'.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="res">The new resolution in 'dots per inch'.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static void SetResolutionY(FIBITMAP dib, uint res)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			SetDotsPerMeterY(dib, (uint)((double)res / 0.0254d + 0.5d));
		}

		/// <summary>
		/// Returns whether the image is a greyscale image or not.
		/// The function scans all colors in the bitmaps palette for entries where
		/// red, green and blue are not all the same (not a grey color).
		/// Supports 1-, 4- and 8-bit bitmaps.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>True if the image is a greyscale image, else false.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static unsafe bool IsGreyscaleImage(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			bool result = true;
			uint bpp = GetBPP(dib);
			switch (bpp)
			{
				case 1:
				case 4:
				case 8:
					RGBQUAD* palette = (RGBQUAD*)GetPalette(dib);
					uint paletteLength = GetColorsUsed(dib);
					for (int i = 0; i < paletteLength; i++)
					{
						if (palette[i].rgbRed != palette[i].rgbGreen ||
							palette[i].rgbRed != palette[i].rgbBlue)
						{
							result = false;
							break;
						}
					}
					break;
				default:
					result = false;
					break;
			}
			return result;
		}

		/// <summary>
		/// Returns a structure that represents the palette of a FreeImage bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>A structure representing the bitmaps palette.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static Palette GetPaletteEx(FIBITMAP dib)
		{
			return new Palette(dib);
		}

		/// <summary>
		/// Returns the <see cref="BITMAPINFOHEADER"/> structure of a FreeImage bitmap.
		/// The structure is a copy, so changes will have no effect on
		/// the bitmap itself.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns><see cref="BITMAPINFOHEADER"/> structure of the bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static unsafe BITMAPINFOHEADER GetInfoHeaderEx(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			return *(BITMAPINFOHEADER*)GetInfoHeader(dib);
		}

		/// <summary>
		/// Returns the <see cref="BITMAPINFO"/> structure of a FreeImage bitmap.
		/// The structure is a copy, so changes will have no effect on
		/// the bitmap itself.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns><see cref="BITMAPINFO"/> structure of the bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static BITMAPINFO GetInfoEx(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			BITMAPINFO result = new BITMAPINFO();
			result.bmiHeader = GetInfoHeaderEx(dib);
			IntPtr ptr = GetPalette(dib);
			if (ptr == IntPtr.Zero)
			{
				result.bmiColors = new RGBQUAD[0];
			}
			else
			{
				result.bmiColors = new MemoryArray<RGBQUAD>(ptr, (int)result.bmiHeader.biClrUsed).Data;
			}
			return result;
		}

		/// <summary>
		/// Returns the pixelformat of the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns><see cref="System.Drawing.Imaging.PixelFormat"/> of the bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static PixelFormat GetPixelFormat(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}

			PixelFormat result = PixelFormat.Undefined;

			if (GetImageType(dib) == FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				switch (GetBPP(dib))
				{
					case 1:
						result = PixelFormat.Format1bppIndexed;
						break;
					case 4:
						result = PixelFormat.Format4bppIndexed;
						break;
					case 8:
						result = PixelFormat.Format8bppIndexed;
						break;
					case 16:
						if ((GetBlueMask(dib) == FI16_565_BLUE_MASK) &&
							(GetGreenMask(dib) == FI16_565_GREEN_MASK) &&
							(GetRedMask(dib) == FI16_565_RED_MASK))
						{
							result = PixelFormat.Format16bppRgb565;
						}
						if ((GetBlueMask(dib) == FI16_555_BLUE_MASK) &&
							(GetGreenMask(dib) == FI16_555_GREEN_MASK) &&
							(GetRedMask(dib) == FI16_555_RED_MASK))
						{
							result = PixelFormat.Format16bppRgb555;
						}
						break;
					case 24:
						result = PixelFormat.Format24bppRgb;
						break;
					case 32:
						result = PixelFormat.Format32bppArgb;
						break;
				}
			}
			return result;
		}

		/// <summary>
		/// Retrieves all parameters needed to create a new FreeImage bitmap from
		/// the format of a .NET <see cref="System.Drawing.Image"/>.
		/// </summary>
		/// <param name="format">The <see cref="System.Drawing.Imaging.PixelFormat"/>
		/// of the .NET <see cref="System.Drawing.Image"/>.</param>
		/// <param name="type">Returns the type used for the new bitmap.</param>
		/// <param name="bpp">Returns the color depth for the new bitmap.</param>
		/// <param name="red_mask">Returns the red_mask for the new bitmap.</param>
		/// <param name="green_mask">Returns the green_mask for the new bitmap.</param>
		/// <param name="blue_mask">Returns the blue_mask for the new bitmap.</param>
		/// <returns>True in case a matching conversion exists; else false.
		/// </returns>
		public static bool GetFormatParameters(
			PixelFormat format,
			out FREE_IMAGE_TYPE type,
			out uint bpp,
			out uint red_mask,
			out uint green_mask,
			out uint blue_mask)
		{
			bool result = false;
			type = FREE_IMAGE_TYPE.FIT_UNKNOWN;
			bpp = 0;
			red_mask = 0;
			green_mask = 0;
			blue_mask = 0;
			switch (format)
			{
				case PixelFormat.Format1bppIndexed:
					type = FREE_IMAGE_TYPE.FIT_BITMAP;
					bpp = 1;
					result = true;
					break;
				case PixelFormat.Format4bppIndexed:
					type = FREE_IMAGE_TYPE.FIT_BITMAP;
					bpp = 4;
					result = true;
					break;
				case PixelFormat.Format8bppIndexed:
					type = FREE_IMAGE_TYPE.FIT_BITMAP;
					bpp = 8;
					result = true;
					break;
				case PixelFormat.Format16bppRgb565:
					type = FREE_IMAGE_TYPE.FIT_BITMAP;
					bpp = 16;
					red_mask = FI16_565_RED_MASK;
					green_mask = FI16_565_GREEN_MASK;
					blue_mask = FI16_565_BLUE_MASK;
					result = true;
					break;
				case PixelFormat.Format16bppRgb555:
				case PixelFormat.Format16bppArgb1555:
					type = FREE_IMAGE_TYPE.FIT_BITMAP;
					bpp = 16;
					red_mask = FI16_555_RED_MASK;
					green_mask = FI16_555_GREEN_MASK;
					blue_mask = FI16_555_BLUE_MASK;
					result = true;
					break;
				case PixelFormat.Format24bppRgb:
					type = FREE_IMAGE_TYPE.FIT_BITMAP;
					bpp = 24;
					red_mask = FI_RGBA_RED_MASK;
					green_mask = FI_RGBA_GREEN_MASK;
					blue_mask = FI_RGBA_BLUE_MASK;
					result = true;
					break;
				case PixelFormat.Format32bppRgb:
				case PixelFormat.Format32bppArgb:
				case PixelFormat.Format32bppPArgb:
					type = FREE_IMAGE_TYPE.FIT_BITMAP;
					bpp = 32;
					red_mask = FI_RGBA_RED_MASK;
					green_mask = FI_RGBA_GREEN_MASK;
					blue_mask = FI_RGBA_BLUE_MASK;
					result = true;
					break;
				case PixelFormat.Format16bppGrayScale:
					type = FREE_IMAGE_TYPE.FIT_UINT16;
					bpp = 16;
					result = true;
					break;
				case PixelFormat.Format48bppRgb:
					type = FREE_IMAGE_TYPE.FIT_RGB16;
					bpp = 48;
					result = true;
					break;
				case PixelFormat.Format64bppArgb:
				case PixelFormat.Format64bppPArgb:
					type = FREE_IMAGE_TYPE.FIT_RGBA16;
					bpp = 64;
					result = true;
					break;
			}
			return result;
		}

		/// <summary>
		/// Retrieves all parameters needed to create a new FreeImage bitmap from
		/// raw bits <see cref="System.Drawing.Image"/>.
		/// </summary>
		/// <param name="type">The <see cref="FREE_IMAGE_TYPE"/>
		/// of the data in memory.</param>
		/// <param name="bpp">The color depth for the data.</param>
		/// <param name="red_mask">Returns the red_mask for the data.</param>
		/// <param name="green_mask">Returns the green_mask for the data.</param>
		/// <param name="blue_mask">Returns the blue_mask for the data.</param>
		/// <returns>True in case a matching conversion exists; else false.
		/// </returns>
		public static bool GetTypeParameters(
			FREE_IMAGE_TYPE type,
			int bpp,
			out uint red_mask,
			out uint green_mask,
			out uint blue_mask)
		{
			bool result = false;
			red_mask = 0;
			green_mask = 0;
			blue_mask = 0;
			switch (type)
			{
				case FREE_IMAGE_TYPE.FIT_BITMAP:
					switch (bpp)
					{
						case 1:
						case 4:
						case 8:
							result = true;
							break;
						case 16:
							result = true;
							red_mask = FI16_555_RED_MASK;
							green_mask = FI16_555_GREEN_MASK;
							blue_mask = FI16_555_BLUE_MASK;
							break;
						case 24:
						case 32:
							result = true;
							red_mask = FI_RGBA_RED_MASK;
							green_mask = FI_RGBA_GREEN_MASK;
							blue_mask = FI_RGBA_BLUE_MASK;
							break;
					}
					break;
				case FREE_IMAGE_TYPE.FIT_UNKNOWN:
					break;
				default:
					result = true;
					break;
			}
			return result;
		}

		/// <summary>
		/// Compares two FreeImage bitmaps.
		/// </summary>
		/// <param name="dib1">The first bitmap to compare.</param>
		/// <param name="dib2">The second bitmap to compare.</param>
		/// <param name="flags">Determines which components of the bitmaps will be compared.</param>
		/// <returns>True in case both bitmaps match the compare conditions, false otherwise.</returns>
		public static bool Compare(FIBITMAP dib1, FIBITMAP dib2, FREE_IMAGE_COMPARE_FLAGS flags)
		{
			// Check whether one bitmap is null
			if (dib1.IsNull ^ dib2.IsNull)
			{
				return false;
			}
			// Check whether both pointers are the same
			if (dib1 == dib2)
			{
				return true;
			}
			if (((flags & FREE_IMAGE_COMPARE_FLAGS.HEADER) > 0) && (!CompareHeader(dib1, dib2)))
			{
				return false;
			}
			if (((flags & FREE_IMAGE_COMPARE_FLAGS.PALETTE) > 0) && (!ComparePalette(dib1, dib2)))
			{
				return false;
			}
			if (((flags & FREE_IMAGE_COMPARE_FLAGS.DATA) > 0) && (!CompareData(dib1, dib2)))
			{
				return false;
			}
			if (((flags & FREE_IMAGE_COMPARE_FLAGS.METADATA) > 0) && (!CompareMetadata(dib1, dib2)))
			{
				return false;
			}
			return true;
		}

		private static unsafe bool CompareHeader(FIBITMAP dib1, FIBITMAP dib2)
		{
			IntPtr i1 = GetInfoHeader(dib1);
			IntPtr i2 = GetInfoHeader(dib2);
			return CompareMemory((void*)i1, (void*)i2, sizeof(BITMAPINFOHEADER));
		}

		private static unsafe bool ComparePalette(FIBITMAP dib1, FIBITMAP dib2)
		{
			IntPtr pal1 = GetPalette(dib1), pal2 = GetPalette(dib2);
			bool hasPalette1 = pal1 != IntPtr.Zero;
			bool hasPalette2 = pal2 != IntPtr.Zero;
			if (hasPalette1 ^ hasPalette2)
			{
				return false;
			}
			if (!hasPalette1)
			{
				return true;
			}
			uint colors = GetColorsUsed(dib1);
			if (colors != GetColorsUsed(dib2))
			{
				return false;
			}
			return CompareMemory((void*)pal1, (void*)pal2, sizeof(RGBQUAD) * colors);
		}

		private static unsafe bool CompareData(FIBITMAP dib1, FIBITMAP dib2)
		{
			uint width = GetWidth(dib1);
			if (width != GetWidth(dib2))
			{
				return false;
			}
			uint height = GetHeight(dib1);
			if (height != GetHeight(dib2))
			{
				return false;
			}
			uint bpp = GetBPP(dib1);
			if (bpp != GetBPP(dib2))
			{
				return false;
			}
			if (GetColorType(dib1) != GetColorType(dib2))
			{
				return false;
			}
			FREE_IMAGE_TYPE type = GetImageType(dib1);
			if (type != GetImageType(dib2))
			{
				return false;
			}

			byte* ptr1, ptr2;
			int fullBytes;
			int shift;
			uint line = GetLine(dib1);

			if (type == FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				switch (bpp)
				{
					case 32:
						for (int i = 0; i < height; i++)
						{
							ptr1 = (byte*)GetScanLine(dib1, i);
							ptr2 = (byte*)GetScanLine(dib2, i);
							if (!CompareMemory(ptr1, ptr2, line))
							{
								return false;
							}
						}
						break;
					case 24:
						for (int i = 0; i < height; i++)
						{
							ptr1 = (byte*)GetScanLine(dib1, i);
							ptr2 = (byte*)GetScanLine(dib2, i);
							if (!CompareMemory(ptr1, ptr2, line))
							{
								return false;
							}
						}
						break;
					case 16:
						short* sPtr1, sPtr2;
						for (int i = 0; i < height; i++)
						{
							sPtr1 = (short*)GetScanLine(dib1, i);
							sPtr2 = (short*)GetScanLine(dib2, i);
							for (int x = 0; x < width; x++)
							{
								if ((sPtr1[x] << 1) != (sPtr2[x] << 1))
								{
									return false;
								}
							}
						}
						break;
					case 8:
						for (int i = 0; i < height; i++)
						{
							ptr1 = (byte*)GetScanLine(dib1, i);
							ptr2 = (byte*)GetScanLine(dib2, i);
							if (!CompareMemory(ptr1, ptr2, line))
							{
								return false;
							}
						}
						break;
					case 4:
						fullBytes = (int)width / 2;
						shift = (width % 2) == 0 ? 8 : 4;
						for (int i = 0; i < height; i++)
						{
							ptr1 = (byte*)GetScanLine(dib1, i);
							ptr2 = (byte*)GetScanLine(dib2, i);
							if (fullBytes != 0)
							{
								if (!CompareMemory(ptr1, ptr2, fullBytes))
								{
									return false;
								}
								ptr1 += fullBytes;
								ptr2 += fullBytes;
							}
							if (shift != 8)
							{
								if ((ptr1[0] >> shift) != (ptr2[0] >> shift))
								{
									return false;
								}
							}
						}
						break;
					case 1:
						fullBytes = (int)width / 8;
						shift = 8 - ((int)width % 8);
						for (int i = 0; i < height; i++)
						{
							ptr1 = (byte*)GetScanLine(dib1, i);
							ptr2 = (byte*)GetScanLine(dib2, i);
							if (fullBytes != 0)
							{
								if (!CompareMemory(ptr1, ptr2, fullBytes))
								{
									return false;
								}
								ptr1 += fullBytes;
								ptr2 += fullBytes;
							}
							if (shift != 8)
							{
								if ((ptr1[0] >> shift) != (ptr2[0] >> shift))
								{
									return false;
								}
							}
						}
						break;
					default:
						throw new NotSupportedException();
				}
			}
			else
			{
				for (int i = 0; i < height; i++)
				{
					ptr1 = (byte*)GetScanLine(dib1, i);
					ptr2 = (byte*)GetScanLine(dib2, i);
					if (!CompareMemory(ptr1, ptr2, line))
					{
						return false;
					}
				}
			}
			return true;
		}

		private static bool CompareMetadata(FIBITMAP dib1, FIBITMAP dib2)
		{
			MetadataTag tag1, tag2;

			foreach (FREE_IMAGE_MDMODEL metadataModel in FREE_IMAGE_MDMODELS)
			{
				if (GetMetadataCount(metadataModel, dib1) !=
					GetMetadataCount(metadataModel, dib2))
				{
					return false;
				}
				if (GetMetadataCount(metadataModel, dib1) == 0)
				{
					continue;
				}

				FIMETADATA mdHandle = FindFirstMetadata(metadataModel, dib1, out tag1);
				if (mdHandle.IsNull)
				{
					continue;
				}
				do
				{
					if ((!GetMetadata(metadataModel, dib2, tag1.Key, out tag2)) ||
						(tag1 != tag2))
					{
						FindCloseMetadata(mdHandle);
						return false;
					}
				}
				while (FindNextMetadata(mdHandle, out tag1));
				FindCloseMetadata(mdHandle);
			}

			return true;
		}

		/// <summary>
		/// Returns the FreeImage bitmap's transparency table.
		/// The array is empty in case the bitmap has no transparency table.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The FreeImage bitmap's transparency table.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static unsafe byte[] GetTransparencyTableEx(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			uint count = GetTransparencyCount(dib);
			byte[] result = new byte[count];
			byte* ptr = (byte*)GetTransparencyTable(dib);
			fixed (byte* dst = result)
			{
				MoveMemory(dst, ptr, count);
			}
			return result;
		}

		/// <summary>
		/// Set the FreeImage bitmap's transparency table. Only affects palletised bitmaps.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="table">The FreeImage bitmap's new transparency table.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> or <paramref name="table"/> is null.</exception>
		public static void SetTransparencyTable(FIBITMAP dib, byte[] table)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			SetTransparencyTable_(dib, table, table.Length);
		}

		/// <summary>
		/// This function returns the number of unique colors actually used by the
		/// specified 1-, 4-, 8-, 16-, 24- or 32-bit image. This might be different from
		/// what function FreeImage_GetColorsUsed() returns, which actually returns the
		/// palette size for palletised images. Works for
		/// <see cref="FREE_IMAGE_TYPE.FIT_BITMAP"/> type images only.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Returns the number of unique colors used by the image specified or
		/// zero, if the image type cannot be handled.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static unsafe int GetUniqueColors(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}

			int result = 0;

			if (GetImageType(dib) == FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				BitArray bitArray;
				int uniquePalEnts;
				int hashcode;
				byte[] lut;
				int width = (int)GetWidth(dib);
				int height = (int)GetHeight(dib);

				switch (GetBPP(dib))
				{
					case 1:

						result = 1;
						lut = CreateShrunkenPaletteLUT(dib, out uniquePalEnts);
						if (uniquePalEnts == 1)
						{
							break;
						}

						if ((*(byte*)GetScanLine(dib, 0) & 0x80) == 0)
						{
							for (int y = 0; y < height; y++)
							{
								byte* scanline = (byte*)GetScanLine(dib, y);
								int mask = 0x80;
								for (int x = 0; x < width; x++)
								{
									if ((scanline[x / 8] & mask) > 0)
									{
										return 2;
									}
									mask = (mask == 0x1) ? 0x80 : (mask >> 1);
								}
							}
						}
						else
						{
							for (int y = 0; y < height; y++)
							{
								byte* scanline = (byte*)GetScanLine(dib, y);
								int mask = 0x80;
								for (int x = 0; x < width; x++)
								{
									if ((scanline[x / 8] & mask) == 0)
									{
										return 2;
									}
									mask = (mask == 0x1) ? 0x80 : (mask >> 1);
								}
							}
						}
						break;

					case 4:

						bitArray = new BitArray(0x10);
						lut = CreateShrunkenPaletteLUT(dib, out uniquePalEnts);
						if (uniquePalEnts == 1)
						{
							result = 1;
							break;
						}

						for (int y = 0; (y < height) && (result < uniquePalEnts); y++)
						{
							byte* scanline = (byte*)GetScanLine(dib, y);
							bool top = true;
							for (int x = 0; (x < width) && (result < uniquePalEnts); x++)
							{
								if (top)
								{
									hashcode = lut[scanline[x / 2] >> 4];
								}
								else
								{
									hashcode = lut[scanline[x / 2] & 0xF];
								}
								top = !top;
								if (!bitArray[hashcode])
								{
									bitArray[hashcode] = true;
									result++;
								}
							}
						}
						break;

					case 8:

						bitArray = new BitArray(0x100);
						lut = CreateShrunkenPaletteLUT(dib, out uniquePalEnts);
						if (uniquePalEnts == 1)
						{
							result = 1;
							break;
						}

						for (int y = 0; (y < height) && (result < uniquePalEnts); y++)
						{
							byte* scanline = (byte*)GetScanLine(dib, y);
							for (int x = 0; (x < width) && (result < uniquePalEnts); x++)
							{
								hashcode = lut[scanline[x]];
								if (!bitArray[hashcode])
								{
									bitArray[hashcode] = true;
									result++;
								}
							}
						}
						break;

					case 16:

						bitArray = new BitArray(0x10000);

						for (int y = 0; y < height; y++)
						{
							short* scanline = (short*)GetScanLine(dib, y);
							for (int x = 0; x < width; x++, scanline++)
							{
								hashcode = *scanline;
								if (!bitArray[hashcode])
								{
									bitArray[hashcode] = true;
									result++;
								}
							}
						}
						break;

					case 24:

						bitArray = new BitArray(0x1000000);

						for (int y = 0; y < height; y++)
						{
							byte* scanline = (byte*)GetScanLine(dib, y);
							for (int x = 0; x < width; x++, scanline += 3)
							{
								hashcode = *((int*)scanline) & 0x00FFFFFF;
								if (!bitArray[hashcode])
								{
									bitArray[hashcode] = true;
									result++;
								}
							}
						}
						break;

					case 32:

						bitArray = new BitArray(0x1000000);

						for (int y = 0; y < height; y++)
						{
							int* scanline = (int*)GetScanLine(dib, y);
							for (int x = 0; x < width; x++, scanline++)
							{
								hashcode = *scanline & 0x00FFFFFF;
								if (!bitArray[hashcode])
								{
									bitArray[hashcode] = true;
									result++;
								}
							}
						}
						break;
				}
			}
			return result;
		}

		/// <summary>
		/// Verifies whether the FreeImage bitmap is 16bit 555.
		/// </summary>
		/// <param name="dib">The FreeImage bitmap to verify.</param>
		/// <returns><b>true</b> if the bitmap is RGB16-555; otherwise <b>false</b>.</returns>
		public static bool IsRGB555(FIBITMAP dib)
		{
			return ((GetRedMask(dib) == FI16_555_RED_MASK) &&
					(GetGreenMask(dib) == FI16_555_GREEN_MASK) &&
					(GetBlueMask(dib) == FI16_555_BLUE_MASK));
		}

		/// <summary>
		/// Verifies whether the FreeImage bitmap is 16bit 565.
		/// </summary>
		/// <param name="dib">The FreeImage bitmap to verify.</param>
		/// <returns><b>true</b> if the bitmap is RGB16-565; otherwise <b>false</b>.</returns>
		public static bool IsRGB565(FIBITMAP dib)
		{
			return ((GetRedMask(dib) == FI16_565_RED_MASK) &&
					(GetGreenMask(dib) == FI16_565_GREEN_MASK) &&
					(GetBlueMask(dib) == FI16_565_BLUE_MASK));
		}

		#endregion

		#region ICC profile functions

		/// <summary>
		/// Creates a new ICC-Profile for a FreeImage bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="data">The data of the new ICC-Profile.</param>
		/// <returns>The new ICC-Profile of the bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static FIICCPROFILE CreateICCProfileEx(FIBITMAP dib, byte[] data)
		{
			return new FIICCPROFILE(dib, data);
		}

		/// <summary>
		/// Creates a new ICC-Profile for a FreeImage bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="data">The data of the new ICC-Profile.</param>
		/// <param name="size">The number of bytes of <paramref name="data"/> to use.</param>
		/// <returns>The new ICC-Profile of the FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static FIICCPROFILE CreateICCProfileEx(FIBITMAP dib, byte[] data, int size)
		{
			return new FIICCPROFILE(dib, data, size);
		}

		#endregion

		#region Conversion functions

		/// <summary>
		/// Converts a FreeImage bitmap from one color depth to another.
		/// If the conversion fails the original FreeImage bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				128,
				FREE_IMAGE_DITHER.FID_FS,
				FREE_IMAGE_QUANTIZE.FIQ_WUQUANT,
				false);
		}

		/// <summary>
		/// Converts a FreeImage bitmap from one color depth to another.
		/// If the conversion fails the original FreeImage bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			bool unloadSource)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				128,
				FREE_IMAGE_DITHER.FID_FS,
				FREE_IMAGE_QUANTIZE.FIQ_WUQUANT,
				unloadSource);
		}

		/// <summary>
		/// Converts a FreeImage bitmap from one color depth to another.
		/// If the conversion fails the original FreeImage bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="threshold">Threshold value when converting with
		/// <see cref="FREE_IMAGE_COLOR_DEPTH.FICD_01_BPP_THRESHOLD"/>.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			byte threshold)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				threshold,
				FREE_IMAGE_DITHER.FID_FS,
				FREE_IMAGE_QUANTIZE.FIQ_WUQUANT,
				false);
		}

		/// <summary>
		/// Converts a FreeImage bitmap from one color depth to another.
		/// If the conversion fails the original FreeImage bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="ditherMethod">Dither algorithm when converting 
		/// with <see cref="FREE_IMAGE_COLOR_DEPTH.FICD_01_BPP_DITHER"/>.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			FREE_IMAGE_DITHER ditherMethod)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				128,
				ditherMethod,
				FREE_IMAGE_QUANTIZE.FIQ_WUQUANT,
				false);
		}


		/// <summary>
		/// Converts a FreeImage bitmap from one color depth to another.
		/// If the conversion fails the original FreeImage bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="quantizationMethod">The quantization algorithm for conversion to 8-bit color depth.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			FREE_IMAGE_QUANTIZE quantizationMethod)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				128,
				FREE_IMAGE_DITHER.FID_FS,
				quantizationMethod,
				false);
		}

		/// <summary>
		/// Converts a FreeImage bitmap from one color depth to another.
		/// If the conversion fails the original FreeImage bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="threshold">Threshold value when converting with
		/// <see cref="FREE_IMAGE_COLOR_DEPTH.FICD_01_BPP_THRESHOLD"/>.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			byte threshold,
			bool unloadSource)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				threshold,
				FREE_IMAGE_DITHER.FID_FS,
				FREE_IMAGE_QUANTIZE.FIQ_WUQUANT,
				unloadSource);
		}

		/// <summary>
		/// Converts a FreeImage bitmap from one color depth to another.
		/// If the conversion fails the original FreeImage bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="ditherMethod">Dither algorithm when converting with
		/// <see cref="FREE_IMAGE_COLOR_DEPTH.FICD_01_BPP_DITHER"/>.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			FREE_IMAGE_DITHER ditherMethod,
			bool unloadSource)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				128,
				ditherMethod,
				FREE_IMAGE_QUANTIZE.FIQ_WUQUANT,
				unloadSource);
		}


		/// <summary>
		/// Converts a FreeImage bitmap from one color depth to another.
		/// If the conversion fails the original FreeImage bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="quantizationMethod">The quantization algorithm for conversion to 8-bit color depth.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			FREE_IMAGE_QUANTIZE quantizationMethod,
			bool unloadSource)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				128,
				FREE_IMAGE_DITHER.FID_FS,
				quantizationMethod,
				unloadSource);
		}

		/// <summary>
		/// Converts a FreeImage bitmap from one color depth to another.
		/// If the conversion fails the original FreeImage bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="threshold">Threshold value when converting with
		/// <see cref="FREE_IMAGE_COLOR_DEPTH.FICD_01_BPP_THRESHOLD"/>.</param>
		/// <param name="ditherMethod">Dither algorithm when converting with
		/// <see cref="FREE_IMAGE_COLOR_DEPTH.FICD_01_BPP_DITHER"/>.</param>
		/// <param name="quantizationMethod">The quantization algorithm for conversion to 8-bit color depth.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		internal static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			byte threshold,
			FREE_IMAGE_DITHER ditherMethod,
			FREE_IMAGE_QUANTIZE quantizationMethod,
			bool unloadSource)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}

			FIBITMAP result = new FIBITMAP();
			FIBITMAP dibTemp = new FIBITMAP();
			uint bpp = GetBPP(dib);
			bool reorderPalette = ((conversion & FREE_IMAGE_COLOR_DEPTH.FICD_REORDER_PALETTE) > 0);
			bool forceGreyscale = ((conversion & FREE_IMAGE_COLOR_DEPTH.FICD_FORCE_GREYSCALE) > 0);

			switch (conversion & (FREE_IMAGE_COLOR_DEPTH)0xFF)
			{
				case FREE_IMAGE_COLOR_DEPTH.FICD_01_BPP_THRESHOLD:

					if (bpp != 1)
					{
						result = Threshold(dib, threshold);
					}
					else
					{
						bool isGreyscale = IsGreyscaleImage(dib);
						if ((forceGreyscale && (!isGreyscale)) ||
						(reorderPalette && isGreyscale))
						{
							result = Threshold(dib, threshold);
						}
					}
					break;

				case FREE_IMAGE_COLOR_DEPTH.FICD_01_BPP_DITHER:

					if (bpp != 1)
					{
						result = Dither(dib, ditherMethod);
					}
					else
					{
						bool isGreyscale = IsGreyscaleImage(dib);
						if ((forceGreyscale && (!isGreyscale)) ||
						(reorderPalette && isGreyscale))
						{
							result = Dither(dib, ditherMethod);
						}
					}
					break;

				case FREE_IMAGE_COLOR_DEPTH.FICD_04_BPP:

					if (bpp != 4)
					{
						// Special case when 1bpp and FIC_PALETTE
						if (forceGreyscale && (bpp == 1) && (GetColorType(dib) == FREE_IMAGE_COLOR_TYPE.FIC_PALETTE))
						{
							dibTemp = ConvertToGreyscale(dib);
							result = ConvertTo4Bits(dibTemp);
							Unload(dibTemp);
						}
						// All other cases are converted directly
						else
						{
							result = ConvertTo4Bits(dib);
						}
					}
					else
					{
						bool isGreyscale = IsGreyscaleImage(dib);
						if ((forceGreyscale && (!isGreyscale)) ||
							(reorderPalette && isGreyscale))
						{
							dibTemp = ConvertToGreyscale(dib);
							result = ConvertTo4Bits(dibTemp);
							Unload(dibTemp);
						}
					}

					break;

				case FREE_IMAGE_COLOR_DEPTH.FICD_08_BPP:

					if (bpp != 8)
					{
						if (forceGreyscale)
						{
							result = ConvertToGreyscale(dib);
						}
						else
						{
							dibTemp = ConvertTo24Bits(dib);
							result = ColorQuantize(dibTemp, quantizationMethod);
							Unload(dibTemp);
						}
					}
					else
					{
						bool isGreyscale = IsGreyscaleImage(dib);
						if ((forceGreyscale && (!isGreyscale)) || (reorderPalette && isGreyscale))
						{
							result = ConvertToGreyscale(dib);
						}
					}
					break;

				case FREE_IMAGE_COLOR_DEPTH.FICD_16_BPP_555:

					if (forceGreyscale)
					{
						dibTemp = ConvertToGreyscale(dib);
						result = ConvertTo16Bits555(dibTemp);
						Unload(dibTemp);
					}
					else if (bpp != 16 || GetRedMask(dib) != FI16_555_RED_MASK || GetGreenMask(dib) != FI16_555_GREEN_MASK || GetBlueMask(dib) != FI16_555_BLUE_MASK)
					{
						result = ConvertTo16Bits555(dib);
					}
					break;

				case FREE_IMAGE_COLOR_DEPTH.FICD_16_BPP:

					if (forceGreyscale)
					{
						dibTemp = ConvertToGreyscale(dib);
						result = ConvertTo16Bits565(dibTemp);
						Unload(dibTemp);
					}
					else if (bpp != 16 || GetRedMask(dib) != FI16_565_RED_MASK || GetGreenMask(dib) != FI16_565_GREEN_MASK || GetBlueMask(dib) != FI16_565_BLUE_MASK)
					{
						result = ConvertTo16Bits565(dib);
					}
					break;

				case FREE_IMAGE_COLOR_DEPTH.FICD_24_BPP:

					if (forceGreyscale)
					{
						dibTemp = ConvertToGreyscale(dib);
						result = ConvertTo24Bits(dibTemp);
						Unload(dibTemp);
					}
					else if (bpp != 24)
					{
						result = ConvertTo24Bits(dib);
					}
					break;

				case FREE_IMAGE_COLOR_DEPTH.FICD_32_BPP:

					if (forceGreyscale)
					{
						dibTemp = ConvertToGreyscale(dib);
						result = ConvertTo32Bits(dibTemp);
						Unload(dibTemp);
					}
					else if (bpp != 32)
					{
						result = ConvertTo32Bits(dib);
					}
					break;
			}

			if (result.IsNull)
			{
				return dib;
			}
			if (unloadSource)
			{
				Unload(dib);
			}

			return result;
		}

		#endregion

		#region Metadata

		/// <summary>
		/// Copies metadata from one FreeImage bitmap to another.
		/// </summary>
		/// <param name="src">Source FreeImage bitmap containing the metadata.</param>
		/// <param name="dst">FreeImage bitmap to copy the metadata to.</param>
		/// <param name="flags">Flags to switch different copy modes.</param>
		/// <returns>Returns -1 on failure else the number of copied tags.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="src"/> or <paramref name="dst"/> is null.</exception>
		public static int CloneMetadataEx(FIBITMAP src, FIBITMAP dst, FREE_IMAGE_METADATA_COPY flags)
		{
			if (src.IsNull)
			{
				throw new ArgumentNullException("src");
			}
			if (dst.IsNull)
			{
				throw new ArgumentNullException("dst");
			}

			FITAG tag = new FITAG(), tag2 = new FITAG();
			int copied = 0;

			// Clear all existing metadata
			if ((flags & FREE_IMAGE_METADATA_COPY.CLEAR_EXISTING) > 0)
			{
				foreach (FREE_IMAGE_MDMODEL model in FREE_IMAGE_MDMODELS)
				{
					if (!SetMetadata(model, dst, null, tag))
					{
						return -1;
					}
				}
			}

			bool keep = !((flags & FREE_IMAGE_METADATA_COPY.REPLACE_EXISTING) > 0);

			foreach (FREE_IMAGE_MDMODEL model in FREE_IMAGE_MDMODELS)
			{
				FIMETADATA mData = FindFirstMetadata(model, src, out tag);
				if (mData.IsNull) continue;
				do
				{
					string key = GetTagKey(tag);
					if (!(keep && GetMetadata(model, dst, key, out tag2)))
					{
						if (SetMetadata(model, dst, key, tag))
						{
							copied++;
						}
					}
				}
				while (FindNextMetadata(mData, out tag));
				FindCloseMetadata(mData);
			}

			return copied;
		}

		/// <summary>
		/// Returns the comment of a JPEG, PNG or GIF image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Comment of the FreeImage bitmp, or null in case no comment exists.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static string GetImageComment(FIBITMAP dib)
		{
			string result = null;
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			FITAG tag;
			if (GetMetadata(FREE_IMAGE_MDMODEL.FIMD_COMMENTS, dib, "Comment", out tag))
			{
				MetadataTag metadataTag = new MetadataTag(tag, FREE_IMAGE_MDMODEL.FIMD_COMMENTS);
				result = metadataTag.Value as string;
			}
			return result;
		}

		/// <summary>
		/// Sets the comment of a JPEG, PNG or GIF image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="comment">New comment of the FreeImage bitmap.
		/// Use null to remove the comment.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static bool SetImageComment(FIBITMAP dib, string comment)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			bool result;
			if (comment != null)
			{
				FITAG tag = CreateTag();
				MetadataTag metadataTag = new MetadataTag(tag, FREE_IMAGE_MDMODEL.FIMD_COMMENTS);
				metadataTag.Value = comment;
				result = SetMetadata(FREE_IMAGE_MDMODEL.FIMD_COMMENTS, dib, "Comment", tag);
				DeleteTag(tag);
			}
			else
			{
				result = SetMetadata(FREE_IMAGE_MDMODEL.FIMD_COMMENTS, dib, "Comment", FITAG.Zero);
			}
			return result;
		}

		/// <summary>
		/// Retrieve a metadata attached to a FreeImage bitmap.
		/// </summary>
		/// <param name="model">The metadata model to look for.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="key">The metadata field name.</param>
		/// <param name="tag">A <see cref="MetadataTag"/> structure returned by the function.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static bool GetMetadata(
			FREE_IMAGE_MDMODEL model,
			FIBITMAP dib,
			string key,
			out MetadataTag tag)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}

			FITAG _tag;
			bool result;
			if (GetMetadata(model, dib, key, out _tag))
			{
				tag = new MetadataTag(_tag, model);
				result = true;
			}
			else
			{
				tag = null;
				result = false;
			}
			return result;
		}

		/// <summary>
		/// Attach a new metadata tag to a FreeImage bitmap.
		/// </summary>
		/// <param name="model">The metadata model used to store the tag.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="key">The tag field name.</param>
		/// <param name="tag">The <see cref="MetadataTag"/> to be attached.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static bool SetMetadata(
			FREE_IMAGE_MDMODEL model,
			FIBITMAP dib,
			string key,
			MetadataTag tag)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			return SetMetadata(model, dib, key, tag.tag);
		}

		/// <summary>
		/// Provides information about the first instance of a tag that matches the metadata model.
		/// </summary>
		/// <param name="model">The model to match.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="tag">Tag that matches the metadata model.</param>
		/// <returns>Unique search handle that can be used to call FindNextMetadata or FindCloseMetadata.
		/// Null if the metadata model does not exist.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static FIMETADATA FindFirstMetadata(
			FREE_IMAGE_MDMODEL model,
			FIBITMAP dib,
			out MetadataTag tag)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			FITAG _tag;
			FIMETADATA result = FindFirstMetadata(model, dib, out _tag);
			if (result.IsNull)
			{
				tag = null;
				return result;
			}
			tag = new MetadataTag(_tag, model);
			if (metaDataSearchHandler.ContainsKey(result))
			{
				metaDataSearchHandler[result] = model;
			}
			else
			{
				metaDataSearchHandler.Add(result, model);
			}
			return result;
		}

		/// <summary>
		/// Find the next tag, if any, that matches the metadata model argument in a previous call
		/// to FindFirstMetadata, and then alters the tag object contents accordingly.
		/// </summary>
		/// <param name="mdhandle">Unique search handle provided by FindFirstMetadata.</param>
		/// <param name="tag">Tag that matches the metadata model.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool FindNextMetadata(FIMETADATA mdhandle, out MetadataTag tag)
		{
			FITAG _tag;
			bool result;
			if (FindNextMetadata(mdhandle, out _tag))
			{
				tag = new MetadataTag(_tag, metaDataSearchHandler[mdhandle]);
				result = true;
			}
			else
			{
				tag = null;
				result = false;
			}
			return result;
		}

		/// <summary>
		/// Closes the specified metadata search handle and releases associated resources.
		/// </summary>
		/// <param name="mdhandle">The handle to close.</param>
		public static void FindCloseMetadata(FIMETADATA mdhandle)
		{
			if (metaDataSearchHandler.ContainsKey(mdhandle))
			{
				metaDataSearchHandler.Remove(mdhandle);
			}
			FindCloseMetadata_(mdhandle);
		}

		/// <summary>
		/// This dictionary links FIMETADATA handles and FREE_IMAGE_MDMODEL models.
		/// </summary>
		private static Dictionary<FIMETADATA, FREE_IMAGE_MDMODEL> metaDataSearchHandler
			= new Dictionary<FIMETADATA, FREE_IMAGE_MDMODEL>(1);

		#endregion

		#region Rotation and flipping

		/// <summary>
		/// Rotates a 4-bit color FreeImage bitmap.
		/// Allowed values for <paramref name="angle"/> are 90, 180 and 270.
		/// In case <paramref name="angle"/> is 0 or 360 a clone is returned.
		/// 0 is returned for other values or in case the rotation fails.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="angle">The angle of rotation.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <remarks>
		/// This function is kind of temporary due to FreeImage's lack of
		/// rotating 4-bit images. It's particularly used by <see cref="FreeImageBitmap"/>'s
		/// method RotateFlip. This function will be removed as soon as FreeImage
		/// supports rotating 4-bit images.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="dib"/> is null.</exception>
		public static unsafe FIBITMAP Rotate4bit(FIBITMAP dib, double angle)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}

			FIBITMAP result = new FIBITMAP();
			int ang = (int)angle;

			if ((GetImageType(dib) == FREE_IMAGE_TYPE.FIT_BITMAP) &&
				(GetBPP(dib) == 4) &&
				((ang % 90) == 0))
			{
				int width, height, xOrg, yOrg;
				Scanline<FI4BIT>[] src, dst;
				width = (int)GetWidth(dib);
				height = (int)GetHeight(dib);
				byte index = 0;
				switch (ang)
				{
					case 90:
						result = Allocate(height, width, 4, 0, 0, 0);
						if (result.IsNull)
						{
							break;
						}
						CopyPalette(dib, result);
						src = Get04BitScanlines(dib);
						dst = Get04BitScanlines(result);
						for (int y = 0; y < width; y++)
						{
							yOrg = height - 1;
							for (int x = 0; x < height; x++, yOrg--)
							{
								index = src[yOrg][y];
								dst[y][x] = index;
							}
						}
						break;
					case 180:
						result = Allocate(width, height, 4, 0, 0, 0);
						if (result.IsNull)
						{
							break;
						}
						CopyPalette(dib, result);
						src = Get04BitScanlines(dib);
						dst = Get04BitScanlines(result);

						yOrg = height - 1;
						for (int y = 0; y < height; y++, yOrg--)
						{
							xOrg = width - 1;
							for (int x = 0; x < width; x++, xOrg--)
							{
								index = src[yOrg][xOrg];
								dst[y][x] = index;
							}
						}
						break;
					case 270:
						result = Allocate(height, width, 4, 0, 0, 0);
						if (result.IsNull)
						{
							break;
						}
						CopyPalette(dib, result);
						src = Get04BitScanlines(dib);
						dst = Get04BitScanlines(result);
						xOrg = width - 1;
						for (int y = 0; y < width; y++, xOrg--)
						{
							for (int x = 0; x < height; x++)
							{
								index = src[x][xOrg];
								dst[y][x] = index;
							}
						}
						break;
					case 0:
					case 360:
						result = Clone(dib);
						break;
				}
			}
			return result;
		}

		#endregion

		#region Wrapper functions

		/// <summary>
		/// Returns the next higher possible color depth.
		/// </summary>
		/// <param name="bpp">Color depth to increase.</param>
		/// <returns>The next higher color depth or 0 if there is no valid color depth.</returns>
		internal static int GetNextColorDepth(int bpp)
		{
			int result = 0;
			switch (bpp)
			{
				case 1:
					result = 4;
					break;
				case 4:
					result = 8;
					break;
				case 8:
					result = 16;
					break;
				case 16:
					result = 24;
					break;
				case 24:
					result = 32;
					break;
			}
			return result;
		}

		/// <summary>
		/// Returns the next lower possible color depth.
		/// </summary>
		/// <param name="bpp">Color depth to decrease.</param>
		/// <returns>The next lower color depth or 0 if there is no valid color depth.</returns>
		internal static int GetPrevousColorDepth(int bpp)
		{
			int result = 0;
			switch (bpp)
			{
				case 32:
					result = 24;
					break;
				case 24:
					result = 16;
					break;
				case 16:
					result = 8;
					break;
				case 8:
					result = 4;
					break;
				case 4:
					result = 1;
					break;
			}
			return result;
		}

		/// <summary>
		/// Reads a null-terminated c-string.
		/// </summary>
		/// <param name="ptr">Pointer to the first char of the string.</param>
		/// <returns>The converted string.</returns>
		internal static unsafe string PtrToStr(byte* ptr)
		{
			string result = null;
			if (ptr != null)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();

				while (*ptr != 0)
				{
					sb.Append((char)(*(ptr++)));
				}
				result = sb.ToString();
			}
			return result;
		}

		internal static unsafe byte[] CreateShrunkenPaletteLUT(FIBITMAP dib, out int uniqueColors)
		{
			byte[] result = null;
			uniqueColors = 0;

			if ((!dib.IsNull) && (GetImageType(dib) == FREE_IMAGE_TYPE.FIT_BITMAP) && (GetBPP(dib) <= 8))
			{
				int size = (int)GetColorsUsed(dib);
				List<RGBQUAD> newPalette = new List<RGBQUAD>(size);
				List<byte> lut = new List<byte>(size);
				RGBQUAD* palette = (RGBQUAD*)GetPalette(dib);
				RGBQUAD color;
				int index;

				for (int i = 0; i < size; i++)
				{
					color = palette[i];
					color.rgbReserved = 255; // ignore alpha

					index = newPalette.IndexOf(color);
					if (index < 0)
					{
						newPalette.Add(color);
						lut.Add((byte)(newPalette.Count - 1));
					}
					else
					{
						lut.Add((byte)index);
					}
				}
				result = lut.ToArray();
				uniqueColors = newPalette.Count;
			}
			return result;
		}

		internal static PropertyItem CreatePropertyItem()
		{
			PropertyItem result = null;
			try
			{
				result = (PropertyItem)PropertyItemConstructor.Invoke(null);
			}
			catch
			{
			}
			return result;
		}

		private static unsafe void CopyPalette(FIBITMAP src, FIBITMAP dst)
		{
			RGBQUAD* orgPal = (RGBQUAD*)GetPalette(src);
			RGBQUAD* newPal = (RGBQUAD*)GetPalette(dst);
			uint size = (uint)(sizeof(RGBQUAD) * GetColorsUsed(src));
			MoveMemory(newPal, orgPal, size);
		}

		private static unsafe Scanline<FI4BIT>[] Get04BitScanlines(FIBITMAP dib)
		{
			int height = (int)GetHeight(dib);
			Scanline<FI4BIT>[] array = new Scanline<FI4BIT>[height];
			for (int i = 0; i < height; i++)
			{
				array[i] = new Scanline<FI4BIT>(dib, i);
			}
			return array;
		}

		/// <summary>
		/// Changes a bitmaps color depth.
		/// Used by SaveEx and SaveToStream
		/// </summary>
		private static FIBITMAP PrepareBitmapColorDepth(FIBITMAP dibToSave, FREE_IMAGE_FORMAT format, FREE_IMAGE_COLOR_DEPTH colorDepth)
		{
			int bpp = (int)GetBPP(dibToSave);
			int targetBpp = (int)(colorDepth & FREE_IMAGE_COLOR_DEPTH.FICD_COLOR_MASK);

			if (colorDepth != FREE_IMAGE_COLOR_DEPTH.FICD_AUTO)
			{
				// A fix colordepth was chosen
				if (FIFSupportsExportBPP(format, targetBpp))
				{
					dibToSave = ConvertColorDepth(dibToSave, colorDepth, false);
				}
				else
				{
					throw new ArgumentException("FreeImage\n\nFreeImage Library plugin " +
						GetFormatFromFIF(format) + " is unable to write images with a color depth of " +
						targetBpp + " bpp.");
				}
			}
			else
			{
				// Auto selection was chosen
				if (!FIFSupportsExportBPP(format, bpp))
				{
					// The color depth is not supported
					int bppUpper = bpp;
					int bppLower = bpp;
					// Check from the bitmaps current color depth in both directions
					do
					{
						bppUpper = GetNextColorDepth(bppUpper);
						if (FIFSupportsExportBPP(format, bppUpper))
						{
							dibToSave = ConvertColorDepth(dibToSave, (FREE_IMAGE_COLOR_DEPTH)bppUpper, false);
							break;
						}
						bppLower = GetPrevousColorDepth(bppLower);
						if (FIFSupportsExportBPP(format, bppLower))
						{
							dibToSave = ConvertColorDepth(dibToSave, (FREE_IMAGE_COLOR_DEPTH)bppLower, false);
							break;
						}
					} while (!((bppLower == 0) && (bppUpper == 0)));
				}
			}
			return dibToSave;
		}

		/// <summary>
		/// Compares blocks of memory.
		/// </summary>
		/// <param name="buf1">Pointer to a block of memory to compare.</param>
		/// <param name="buf2">Pointer to a block of memory to compare.</param>
		/// <param name="length">Specifies the number of bytes to be compared.</param>
		/// <returns>If all bytes compare as equal, true is returned.</returns>
		public static unsafe bool CompareMemory(void* buf1, void* buf2, uint length)
		{
			return (length == RtlCompareMemory(buf1, buf2, length));
		}

		/// <summary>
		/// Compares blocks of memory.
		/// </summary>
		/// <param name="buf1">Pointer to a block of memory to compare.</param>
		/// <param name="buf2">Pointer to a block of memory to compare.</param>
		/// <param name="length">Specifies the number of bytes to be compared.</param>
		/// <returns>If all bytes compare as equal, true is returned.</returns>
		public static unsafe bool CompareMemory(void* buf1, void* buf2, long length)
		{
			return (length == RtlCompareMemory(buf1, buf2, checked((uint)length)));
		}

		/// <summary>
		/// Compares blocks of memory.
		/// </summary>
		/// <param name="buf1">Pointer to a block of memory to compare.</param>
		/// <param name="buf2">Pointer to a block of memory to compare.</param>
		/// <param name="length">Specifies the number of bytes to be compared.</param>
		/// <returns>If all bytes compare as equal, true is returned.</returns>
		public static unsafe bool CompareMemory(IntPtr buf1, IntPtr buf2, uint length)
		{
			return (length == RtlCompareMemory(buf1.ToPointer(), buf2.ToPointer(), length));
		}

		/// <summary>
		/// Compares blocks of memory.
		/// </summary>
		/// <param name="buf1">Pointer to a block of memory to compare.</param>
		/// <param name="buf2">Pointer to a block of memory to compare.</param>
		/// <param name="length">Specifies the number of bytes to be compared.</param>
		/// <returns>If all bytes compare as equal, true is returned.</returns>
		public static unsafe bool CompareMemory(IntPtr buf1, IntPtr buf2, long length)
		{
			return (length == RtlCompareMemory(buf1.ToPointer(), buf2.ToPointer(), checked((uint)length)));
		}

		/// <summary>
		/// Moves a block of memory from one location to another.
		/// </summary>
		/// <param name="dst">Pointer to the starting address of the move destination.</param>
		/// <param name="src">Pointer to the starting address of the block of memory to be moved.</param>
		/// <param name="size">Size of the block of memory to move, in bytes.</param>
		public static unsafe void MoveMemory(void* dst, void* src, long size)
		{
			MoveMemory(dst, src, checked((uint)size));
		}

		/// <summary>
		/// Moves a block of memory from one location to another.
		/// </summary>
		/// <param name="dst">Pointer to the starting address of the move destination.</param>
		/// <param name="src">Pointer to the starting address of the block of memory to be moved.</param>
		/// <param name="size">Size of the block of memory to move, in bytes.</param>
		public static unsafe void MoveMemory(IntPtr dst, IntPtr src, uint size)
		{
			MoveMemory(dst.ToPointer(), src.ToPointer(), size);
		}

		/// <summary>
		/// Moves a block of memory from one location to another.
		/// </summary>
		/// <param name="dst">Pointer to the starting address of the move destination.</param>
		/// <param name="src">Pointer to the starting address of the block of memory to be moved.</param>
		/// <param name="size">Size of the block of memory to move, in bytes.</param>
		public static unsafe void MoveMemory(IntPtr dst, IntPtr src, long size)
		{
			MoveMemory(dst.ToPointer(), src.ToPointer(), checked((uint)size));
		}

		/// <summary>
		/// Copies a block of memory from one location to another.
		/// </summary>
		/// <param name="dest">Pointer to the starting address of the copy destination.</param>
		/// <param name="src">Pointer to the starting address of the block of memory to be copied.</param>
		/// <param name="len">Size of the block of memory to copy, in bytes.</param>
		/// <remarks>
		/// <b>CopyMemory</b> runs faster than <see cref="MoveMemory(void*, void*, uint)"/>.
		/// However, if both blocks overlap the result is undefined.
		/// </remarks>
		public static unsafe void CopyMemory(byte* dest, byte* src, int len)
		{
			if (len >= 0x10)
			{
				do
				{
					*((int*)dest) = *((int*)src);
					*((int*)(dest + 4)) = *((int*)(src + 4));
					*((int*)(dest + 8)) = *((int*)(src + 8));
					*((int*)(dest + 12)) = *((int*)(src + 12));
					dest += 0x10;
					src += 0x10;
				}
				while ((len -= 0x10) >= 0x10);
			}
			if (len > 0)
			{
				if ((len & 8) != 0)
				{
					*((int*)dest) = *((int*)src);
					*((int*)(dest + 4)) = *((int*)(src + 4));
					dest += 8;
					src += 8;
				}
				if ((len & 4) != 0)
				{
					*((int*)dest) = *((int*)src);
					dest += 4;
					src += 4;
				}
				if ((len & 2) != 0)
				{
					*((short*)dest) = *((short*)src);
					dest += 2;
					src += 2;
				}
				if ((len & 1) != 0)
				{
					*dest = *src;
				}
			}
		}

		/// <summary>
		/// Copies a block of memory from one location to another.
		/// </summary>
		/// <param name="dest">Pointer to the starting address of the copy destination.</param>
		/// <param name="src">Pointer to the starting address of the block of memory to be copied.</param>
		/// <param name="size">Size of the block of memory to copy, in bytes.</param>
		/// <remarks>
		/// <b>CopyMemory</b> runs faster than <see cref="MoveMemory(void*, void*, long)"/>.
		/// However, if both blocks overlap the result is undefined.
		/// </remarks>
		public static unsafe void CopyMemory(void* dest, void* src, long size)
		{
			CopyMemory((byte*)dest, (byte*)src, checked((int)size));
		}

		/// <summary>
		/// Copies a block of memory from one location to another.
		/// </summary>
		/// <param name="dest">Pointer to the starting address of the copy destination.</param>
		/// <param name="src">Pointer to the starting address of the block of memory to be copied.</param>
		/// <param name="size">Size of the block of memory to copy, in bytes.</param>
		/// <remarks>
		/// <b>CopyMemory</b> runs faster than <see cref="MoveMemory(void*, void*, long)"/>.
		/// However, if both blocks overlap the result is undefined.
		/// </remarks>
		public static unsafe void CopyMemory(void* dest, void* src, int size)
		{
			CopyMemory((byte*)dest, (byte*)src, size);
		}

		/// <summary>
		/// Copies a block of memory from one location to another.
		/// </summary>
		/// <param name="dest">Pointer to the starting address of the copy destination.</param>
		/// <param name="src">Pointer to the starting address of the block of memory to be copied.</param>
		/// <param name="size">Size of the block of memory to copy, in bytes.</param>
		/// <remarks>
		/// <b>CopyMemory</b> runs faster than <see cref="MoveMemory(IntPtr, IntPtr, uint)"/>.
		/// However, if both blocks overlap the result is undefined.
		/// </remarks>
		public static unsafe void CopyMemory(IntPtr dest, IntPtr src, int size)
		{
			CopyMemory((byte*)dest, (byte*)src, size);
		}

		/// <summary>
		/// Copies a block of memory from one location to another.
		/// </summary>
		/// <param name="dest">Pointer to the starting address of the copy destination.</param>
		/// <param name="src">Pointer to the starting address of the block of memory to be copied.</param>
		/// <param name="size">Size of the block of memory to copy, in bytes.</param>
		/// <remarks>
		/// <b>CopyMemory</b> runs faster than <see cref="MoveMemory(IntPtr, IntPtr, long)"/>.
		/// However, if both blocks overlap the result is undefined.
		/// </remarks>
		public static unsafe void CopyMemory(IntPtr dest, IntPtr src, long size)
		{
			CopyMemory((byte*)dest, (byte*)src, checked((int)size));
		}

		internal static string ColorToString(Color color)
		{
			return string.Format(
				System.Globalization.CultureInfo.CurrentCulture,
				"{{Name={0}, ARGB=({1}, {2}, {3}, {4})}}",
				new object[] { color.Name, color.A, color.R, color.G, color.B });
		}

		#endregion

		#region Dll-Imports

		/// <summary>
		/// Retrieves a handle to a display device context (DC) for the client area of a specified window
		/// or for the entire screen. You can use the returned handle in subsequent GDI functions to draw in the DC.
		/// </summary>
		/// <param name="hWnd">Handle to the window whose DC is to be retrieved.
		/// If this value is IntPtr.Zero, GetDC retrieves the DC for the entire screen. </param>
		/// <returns>If the function succeeds, the return value is a handle to the DC for the specified window's client area.
		/// If the function fails, the return value is NULL.</returns>
		[DllImport("user32.dll")]
		private static extern IntPtr GetDC(IntPtr hWnd);

		/// <summary>
		/// Releases a device context (DC), freeing it for use by other applications.
		/// The effect of the ReleaseDC function depends on the type of DC. It frees only common and window DCs.
		/// It has no effect on class or private DCs.
		/// </summary>
		/// <param name="hWnd">Handle to the window whose DC is to be released.</param>
		/// <param name="hDC">Handle to the DC to be released.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport("user32.dll")]
		private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

		/// <summary>
		/// Creates a DIB that applications can write to directly.
		/// The function gives you a pointer to the location of the bitmap bit values.
		/// You can supply a handle to a file-mapping object that the function will use to create the bitmap,
		/// or you can let the system allocate the memory for the bitmap.
		/// </summary>
		/// <param name="hdc">Handle to a device context.</param>
		/// <param name="pbmi">Pointer to a BITMAPINFO structure that specifies various attributes of the DIB,
		/// including the bitmap dimensions and colors.</param>
		/// <param name="iUsage">Specifies the type of data contained in the bmiColors array member of the BITMAPINFO structure
		/// pointed to by pbmi (either logical palette indexes or literal RGB values).</param>
		/// <param name="ppvBits">Pointer to a variable that receives a pointer to the location of the DIB bit values.</param>
		/// <param name="hSection">Handle to a file-mapping object that the function will use to create the DIB.
		/// This parameter can be NULL.</param>
		/// <param name="dwOffset">Specifies the offset from the beginning of the file-mapping object referenced by hSection
		/// where storage for the bitmap bit values is to begin. This value is ignored if hSection is NULL.</param>
		/// <returns>If the function succeeds, the return value is a handle to the newly created DIB,
		/// and *ppvBits points to the bitmap bit values. If the function fails, the return value is NULL, and *ppvBits is NULL.</returns>
		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateDIBSection(
			IntPtr hdc,
			[In] IntPtr pbmi,
			uint iUsage,
			out IntPtr ppvBits,
			IntPtr hSection,
			uint dwOffset);

		/// <summary>
		/// Deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object.
		/// After the object is deleted, the specified handle is no longer valid.
		/// </summary>
		/// <param name="hObject">Handle to a logical pen, brush, font, bitmap, region, or palette.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		/// <summary>
		/// Creates a compatible bitmap (DDB) from a DIB and, optionally, sets the bitmap bits.
		/// </summary>
		/// <param name="hdc">Handle to a device context.</param>
		/// <param name="lpbmih">Pointer to a bitmap information header structure.</param>
		/// <param name="fdwInit">Specifies how the system initializes the bitmap bits - (use 4).</param>
		/// <param name="lpbInit">Pointer to an array of bytes containing the initial bitmap data.</param>
		/// <param name="lpbmi">Pointer to a BITMAPINFO structure that describes the dimensions
		/// and color format of the array pointed to by the lpbInit parameter.</param>
		/// <param name="fuUsage">Specifies whether the bmiColors member of the BITMAPINFO structure
		/// was initialized - (use 0).</param>
		/// <returns>Handle to a DIB or null on failure.</returns>
		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateDIBitmap(
			IntPtr hdc,
			IntPtr lpbmih,
			uint fdwInit,
			IntPtr lpbInit,
			IntPtr lpbmi,
			uint fuUsage);

		/// <summary>
		/// Retrieves information for the specified graphics object.
		/// </summary>
		/// <param name="hgdiobj">Handle to the graphics object of interest.</param>
		/// <param name="cbBuffer">Specifies the number of bytes of information to
		/// be written to the buffer.</param>
		/// <param name="lpvObject">Pointer to a buffer that receives the information
		/// about the specified graphics object.</param>
		/// <returns>0 on failure.</returns>
		[DllImport("gdi32.dll")]
		private static extern int GetObject(IntPtr hgdiobj, int cbBuffer, IntPtr lpvObject);

		/// <summary>
		/// Retrieves the bits of the specified compatible bitmap and copies them into a buffer
		/// as a DIB using the specified format.
		/// </summary>
		/// <param name="hdc">Handle to the device context.</param>
		/// <param name="hbmp">Handle to the bitmap. This must be a compatible bitmap (DDB).</param>
		/// <param name="uStartScan">Specifies the first scan line to retrieve.</param>
		/// <param name="cScanLines">Specifies the number of scan lines to retrieve.</param>
		/// <param name="lpvBits">Pointer to a buffer to receive the bitmap data.</param>
		/// <param name="lpbmi">Pointer to a BITMAPINFO structure that specifies the desired
		/// format for the DIB data.</param>
		/// <param name="uUsage">Specifies the format of the bmiColors member of the
		/// BITMAPINFO structure - (use 0).</param>
		/// <returns>0 on failure.</returns>
		[DllImport("gdi32.dll")]
		private static extern unsafe int GetDIBits(
			IntPtr hdc,
			IntPtr hbmp,
			uint uStartScan,
			uint cScanLines,
			IntPtr lpvBits,
			IntPtr lpbmi,
			uint uUsage);

		/// <summary>
		/// Moves a block of memory from one location to another.
		/// </summary>
		/// <param name="dst">Pointer to the starting address of the move destination.</param>
		/// <param name="src">Pointer to the starting address of the block of memory to be moved.</param>
		/// <param name="size">Size of the block of memory to move, in bytes.</param>
		[DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
		public static unsafe extern void MoveMemory(void* dst, void* src, uint size);

		/// <summary>
		/// The RtlCompareMemory routine compares blocks of memory
		/// and returns the number of bytes that are equivalent.
		/// </summary>
		/// <param name="buf1">Pointer to a block of memory to compare.</param>
		/// <param name="buf2">Pointer to a block of memory to compare.</param>
		/// <param name="count">Specifies the number of bytes to be compared.</param>
		/// <returns>RtlCompareMemory returns the number of bytes that compare as equal.
		/// If all bytes compare as equal, the input Length is returned.</returns>
		[DllImport("ntdll.dll", EntryPoint = "RtlCompareMemory", SetLastError = false)]
		internal static unsafe extern uint RtlCompareMemory(void* buf1, void* buf2, uint count);

		#endregion
	}
}
