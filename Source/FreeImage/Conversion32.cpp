// ==========================================================
// Bitmap conversion routines
//
// Design and implementation by
// - Floris van den Berg (flvdberg@wxs.nl)
// - Herv� Drolon (drolon@infonie.fr)
// - Jani Kajala (janik@remedy.fi)
// - Detlev Vendt (detlev.vendt@brillit.de)
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

#include "FreeImage.h"
#include "Utilities.h"

// ----------------------------------------------------------
//  internal conversions X to 32 bits
// ----------------------------------------------------------

void DLL_CALLCONV
FreeImage_ConvertLine1To32(BYTE *target, BYTE *source, int width_in_pixels, RGBQUAD *palette) {
	for (int cols = 0; cols < width_in_pixels; cols++) {
		int index = (source[cols>>3] & (0x80 >> (cols & 0x07))) != 0 ? 1 : 0;

		target[FIRGBA_BLUE] = palette[index].rgbBlue;
		target[FIRGBA_GREEN] = palette[index].rgbGreen;
		target[FIRGBA_RED] = palette[index].rgbRed;
		target[FIRGBA_ALPHA] = 0xff;
		target += 4;
	}	
}

void DLL_CALLCONV
FreeImage_ConvertLine4To32(BYTE *target, BYTE *source, int width_in_pixels, RGBQUAD *palette) {
	BOOL low_nibble = FALSE;
	int x = 0;

	for (int cols = 0 ; cols < width_in_pixels ; ++cols) {
		if (low_nibble) {
			target[FIRGBA_BLUE] = palette[LOWNIBBLE(source[x])].rgbBlue;
			target[FIRGBA_GREEN] = palette[LOWNIBBLE(source[x])].rgbGreen;
			target[FIRGBA_RED] = palette[LOWNIBBLE(source[x])].rgbRed;

			x++;
		} else {
			target[FIRGBA_BLUE] = palette[HINIBBLE(source[x]) >> 4].rgbBlue;
			target[FIRGBA_GREEN] = palette[HINIBBLE(source[x]) >> 4].rgbGreen;
			target[FIRGBA_RED] = palette[HINIBBLE(source[x]) >> 4].rgbRed;
		}

		low_nibble = !low_nibble;

		target[FIRGBA_ALPHA] = 0xff;
		target += 4;
	}
}

void DLL_CALLCONV
FreeImage_ConvertLine8To32(BYTE *target, BYTE *source, int width_in_pixels, RGBQUAD *palette) {
	for (int cols = 0; cols < width_in_pixels; cols++) {
		target[FIRGBA_BLUE] = palette[source[cols]].rgbBlue;
		target[FIRGBA_GREEN] = palette[source[cols]].rgbGreen;
		target[FIRGBA_RED] = palette[source[cols]].rgbRed;
		target[FIRGBA_ALPHA] = 0xff;
		target += 4;
	}
}

void DLL_CALLCONV
FreeImage_ConvertLine16To32_555(BYTE *target, BYTE *source, int width_in_pixels) {
	WORD *bits = (WORD *)source;

	for (int cols = 0; cols < width_in_pixels; cols++) {
		target[FIRGBA_RED] = (((bits[cols] & FI16_555_RED_MASK) >> FI16_555_RED_SHIFT) * 0xFF) / 0x1F;
		target[FIRGBA_GREEN] = (((bits[cols] & FI16_555_GREEN_MASK) >> FI16_555_GREEN_SHIFT) * 0xFF) / 0x1F;
		target[FIRGBA_BLUE] = (((bits[cols] & FI16_555_BLUE_MASK) >> FI16_555_BLUE_SHIFT) * 0xFF) / 0x1F;
		target[FIRGBA_ALPHA] = 0xff;
		target += 4;
	}
}

void DLL_CALLCONV
FreeImage_ConvertLine16To32_565(BYTE *target, BYTE *source, int width_in_pixels) {
	WORD *bits = (WORD *)source;

	for (int cols = 0; cols < width_in_pixels; cols++) {
		target[FIRGBA_RED] = (((bits[cols] & FI16_565_RED_MASK) >> FI16_565_RED_SHIFT) * 0xFF) / 0x1F;
		target[FIRGBA_GREEN] = (((bits[cols] & FI16_565_GREEN_MASK) >> FI16_565_GREEN_SHIFT) * 0xFF) / 0x3F;
		target[FIRGBA_BLUE] = (((bits[cols] & FI16_565_BLUE_MASK) >> FI16_565_BLUE_SHIFT) * 0xFF) / 0x1F;
		target[FIRGBA_ALPHA] = 0xff;
		target += 4;
	}
}

void DLL_CALLCONV
FreeImage_ConvertLine24To32(BYTE *target, BYTE *source, int width_in_pixels) {
	for (int cols = 0; cols < width_in_pixels; cols++) {
		*(DWORD *)target = (*(DWORD *) source & FIRGBA_RGB_MASK) | FIRGBA_ALPHA_MASK;
		target += 4;
		source += 3;
	}
}

// ----------------------------------------------------------

static void DLL_CALLCONV
MapTransparentTableToAlpha(RGBQUAD *target, BYTE *source, BYTE *table, int transparent_pixels, int width_in_pixels) {
	for (int cols = 0; cols < width_in_pixels; cols++) {
		target[cols].rgbReserved = (source[cols] < transparent_pixels) ? table[source[cols]] : 255;
	}
}

// ----------------------------------------------------------

FIBITMAP * DLL_CALLCONV
FreeImage_ConvertTo32Bits(FIBITMAP *dib) {
	if(!dib) return NULL;

	int bpp = FreeImage_GetBPP(dib);

	if(bpp != 32) {
		int width = FreeImage_GetWidth(dib);
		int height = FreeImage_GetHeight(dib);

		switch(bpp) {
			case 1 :
			{
				FIBITMAP *new_dib = FreeImage_Allocate(width, height, 32, FIRGBA_RED_MASK, FIRGBA_GREEN_MASK, FIRGBA_BLUE_MASK);

				if (new_dib != NULL)
					for (int rows = 0; rows < height; rows++)
						FreeImage_ConvertLine1To32(FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), width, FreeImage_GetPalette(dib));
				
				return new_dib;
			}

			case 4 :
			{
				FIBITMAP *new_dib = FreeImage_Allocate(width, height, 32, FIRGBA_RED_MASK, FIRGBA_GREEN_MASK, FIRGBA_BLUE_MASK);

				if (new_dib != NULL) {
					for (int rows = 0; rows < height; rows++) {
						FreeImage_ConvertLine4To32(FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), width, FreeImage_GetPalette(dib));
				
						if (FreeImage_IsTransparent(dib)) {
							MapTransparentTableToAlpha((RGBQUAD *)FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), FreeImage_GetTransparencyTable(dib), FreeImage_GetTransparencyCount(dib), width);
						}
					}

					return new_dib;
				}
			}
				
			case 8 :
			{
				FIBITMAP *new_dib = FreeImage_Allocate(width, height, 32, FIRGBA_RED_MASK, FIRGBA_GREEN_MASK, FIRGBA_BLUE_MASK);

				if (new_dib != NULL) {
					for (int rows = 0; rows < height; rows++) {
						FreeImage_ConvertLine8To32(FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), width, FreeImage_GetPalette(dib));

						if (FreeImage_IsTransparent(dib)) {
							MapTransparentTableToAlpha((RGBQUAD *)FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), FreeImage_GetTransparencyTable(dib), FreeImage_GetTransparencyCount(dib), width);
						}
					}

					return new_dib;
				}
			}

			case 16 :
			{
				FIBITMAP *new_dib = FreeImage_Allocate(width, height, 32, FIRGBA_RED_MASK, FIRGBA_GREEN_MASK, FIRGBA_BLUE_MASK);

				if (new_dib != NULL) {
					for (int rows = 0; rows < height; rows++) {
						if ((FreeImage_GetRedMask(dib) == FI16_565_RED_MASK) && (FreeImage_GetGreenMask(dib) == FI16_565_GREEN_MASK) && (FreeImage_GetBlueMask(dib) == FI16_565_BLUE_MASK)) {
							FreeImage_ConvertLine16To32_565(FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), width);
						} else {
							// includes case where all the masks are 0
							FreeImage_ConvertLine16To32_555(FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), width);
						}
					}
				}

				return new_dib;
			}

			case 24 :
			{
				FIBITMAP *new_dib = FreeImage_Allocate(width, height, 32, FIRGBA_RED_MASK, FIRGBA_GREEN_MASK, FIRGBA_BLUE_MASK);

				if (new_dib != NULL)
					for (int rows = 0; rows < height; rows++)
						FreeImage_ConvertLine24To32(FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), width);
				
				return new_dib;
			}
		}
	}

	return FreeImage_Clone(dib);
}
