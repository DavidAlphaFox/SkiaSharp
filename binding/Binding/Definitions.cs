﻿//
// Skia Definitions, enumerations and interop structures
//
// Author:
//    Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2016 Xamarin Inc
//
// TODO: 
//   Add more ToString, operators, convenience methods to various structures here (point, rect, etc)
//   Sadly, the Rectangles are not binary compatible with the System.Drawing ones.
//
// SkMatrix could benefit from bringing some of the operators defined in C++
//
// Augmented primitives come from Mono:
// Author:
//   Mike Kestner (mkestner@speakeasy.net)
//
// Copyright (C) 2001 Mike Kestner
// Copyright (C) 2004,2006 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Runtime.InteropServices;
using System.Globalization;
 	
namespace SkiaSharp
{
	public enum SKCodecResult {
		Success,
		IncompleteInput,
		InvalidConversion,
		InvalidScale,
		InvalidParameters,
		InvalidInput,
		CouldNotRewind,
		Unimplemented,
	}

	public enum SKCodecOrigin {
		TopLeft = 1,
		TopRight = 2,
		BottomRight = 3,
		BottomLeft = 4,
		LeftTop = 5,
		RightTop = 6,
		RightBottom = 7,
		LeftBottom = 8,
	}

	public enum SKEncodedFormat {
		Unknown,
		Bmp,
		Gif,
		Ico,
		Jpeg,
		Png,
		Wbmp,
		Webp,
		Pkm,
		Ktx,
		Astc,
		Dng,
	}

	public partial struct SKColor {
		public static readonly SKColor Empty;

		private uint color;

		internal SKColor (uint value)
		{
			color = value;
		}

		public SKColor (byte red, byte green, byte blue, byte alpha)
		{
			color = (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);
		}

		public SKColor (byte red, byte green, byte blue)
		{
			color = (uint)(0xff000000u | (red << 16) | (green << 8) | blue);
		}

		public SKColor WithAlpha (byte alpha)
		{
			return new SKColor (Red, Green, Blue, alpha);
		}

		public byte Alpha => (byte)((color >> 24) & 0xff);
		public byte Red => (byte)((color >> 16) & 0xff);
		public byte Green => (byte)((color >> 8) & 0xff);
		public byte Blue => (byte)((color) & 0xff);

		public float GetBrightness ()
		{
			int r = Red;
			int g = Green;
			int b = Blue;
			byte minval = (byte) Math.Min (r, Math.Min (g, b));
			byte maxval = (byte) Math.Max (r, Math.Max (g, b));
	
			return (float)(maxval + minval) / 510;
		}

		public float GetSaturation ()
		{
			int r = Red;
			int g = Green;
			int b = Blue;
			byte minval = (byte) Math.Min (r, Math.Min (g, b));
			byte maxval = (byte) Math.Max (r, Math.Max (g, b));
			
			if (maxval == minval)
					return 0.0f;

			int sum = maxval + minval;
			if (sum > 255)
				sum = 510 - sum;

			return (float)(maxval - minval) / sum;
		}

		public float GetHue ()
		{
			int r = Red;
			int g = Green;
			int b = Blue;
			byte minval = (byte) Math.Min (r, Math.Min (g, b));
			byte maxval = (byte) Math.Max (r, Math.Max (g, b));
			
			if (maxval == minval)
					return 0.0f;
			
			float diff = (float)(maxval - minval);
			float rnorm = (maxval - r) / diff;
			float gnorm = (maxval - g) / diff;
			float bnorm = (maxval - b) / diff;
	
			float hue = 0.0f;
			if (r == maxval) 
				hue = 60.0f * (6.0f + bnorm - gnorm);
			if (g == maxval) 
				hue = 60.0f * (2.0f + rnorm - bnorm);
			if (b  == maxval) 
				hue = 60.0f * (4.0f + gnorm - rnorm);
			if (hue > 360.0f) 
				hue = hue - 360.0f;

			return hue;
		}
		
		public override string ToString ()
		{
			return string.Format (CultureInfo.InvariantCulture, "#{0:x2}{1:x2}{2:x2}{3:x2}",  Alpha, Red, Green, Blue);
		}

		public override bool Equals (object other)
		{
			if (!(other is SKColor))
				return false;

			var c = (SKColor) other;
			return c.color == this.color;
		}

		public override int GetHashCode ()
		{
			return (int) color;
		}

		public static implicit operator SKColor (uint color)
		{
			return new SKColor (color);
		}

		public static explicit operator uint (SKColor color)
		{
			return color.color;
		}

		public static bool operator == (SKColor left, SKColor right)
		{
			return left.color == right.color;
		}

		public static bool operator != (SKColor left, SKColor right)
		{
			return !(left == right);
		}
	}

	[Flags]
	public enum SKTypefaceStyle {
		Normal     = 0,
		Bold       = 0x01,
		Italic     = 0x02,
		BoldItalic = 0x03
	}

	public enum SKFontStyleWeight {
		Thin        = 100,
		ExtraLight  = 200,
		Light       = 300,
		Normal      = 400,
		Medium      = 500,
		SemiBold    = 600,
		Bold        = 700,
		ExtraBold   = 800,
		Black       = 900
	};

	public enum SKFontStyleWidth {
		UltraCondensed   = 1,
		ExtraCondensed   = 2,
		Condensed        = 3,
		SemiCondensed    = 4,
		Normal           = 5,
		SemiExpanded     = 6,
		Expanded         = 7,
		ExtraExpanded    = 8,
		UltaExpanded     = 9
	};

	public enum SKFontStyleSlant {
		Upright = 0,
		Italic  = 1,
		Oblique = 2,
	};

	public enum SKPointMode {
		Points, Lines, Polygon
	}

	public enum SKPathDirection {
		Clockwise,
		CounterClockwise
	}

	public enum SKPathArcSize {
		Small,
		Large
	}

	public enum SKPathFillType
	{
		Winding,
		EvenOdd,
		InverseWinding,
		InverseEvenOdd
	}

	public enum SKColorType {
		Unknown,
		Alpha8,
		Rgb565,
		Argb4444,
		Rgba8888,
		Bgra8888,
		Index8,
		Gray8,
		RgbaF16
	}

	public enum SKColorProfileType {
		Linear,
		SRGB
	}

	public enum SKAlphaType {
		Unknown,
		Opaque,
		Premul,
		Unpremul
	}

	public enum SKShaderTileMode {
		Clamp, Repeat, Mirror
	}

	public enum SKBlurStyle {
		Normal, Solid, Outer, Inner
	}

	public enum SKXferMode {
		Clear,
		Src,
		Dst,
		SrcOver,
		DstOver,
		SrcIn,
		DstIn,
		SrcOut,
		DstOut,
		SrcATop,
		DstATop,
		Xor,
		Plus,
		Modulate,
		Screen,
		Overlay,
		Darken,
		Lighten,
		ColorDodge,
		ColorBurn,
		HardLight,
		SoftLight,
		Difference,
		Exclusion,
		Multiply,
		Hue,
		Saturation,
		Color,
		Luminosity,
	}

	public enum SKClipType {
		Intersect, Difference 
	}

	public enum SKPixelGeometry {
		Unknown,
		RgbHorizontal,
		BgrHorizontal,
		RgbVertical,
		BgrVertical
	}

	public enum SKEncoding {
		Utf8, Utf16, Utf32
	}

	public static class SkiaExtensions {
		public static bool IsBgr (this SKPixelGeometry pg)
		{
			return pg == SKPixelGeometry.BgrHorizontal || pg == SKPixelGeometry.BgrVertical;
		}

		public static bool IsRgb (this SKPixelGeometry pg)
		{
			return pg == SKPixelGeometry.RgbHorizontal || pg == SKPixelGeometry.RgbVertical;
		}

		public static bool IsVertical (this SKPixelGeometry pg)
		{
			return pg == SKPixelGeometry.BgrVertical || pg == SKPixelGeometry.RgbVertical;
		}

		public static bool IsHorizontal (this SKPixelGeometry pg)
		{
			return pg == SKPixelGeometry.BgrHorizontal || pg == SKPixelGeometry.RgbHorizontal;
		}
	}

	public enum SKStrokeCap {
		Butt, Round, Square
	}

	public enum SKStrokeJoin {
		Mitter, Round, Bevel
	}

	public enum SKTextAlign {
		Left, Center, Right
	}

	public enum SKTextEncoding {
		Utf8, Utf16, Utf32, GlyphId
	}

	public enum SKFilterQuality
	{
		None,
		Low,
		Medium,
		High
	}

	[Flags]
	public enum SKCropRectFlags
	{
		HasLeft = 0x01,
		HasTop = 0x02,
		HasWidth = 0x04,
		HasHeight = 0x08,
		HasAll = 0x0F,
	}

	public enum SKDropShadowImageFilterShadowMode
	{
		DrawShadowAndForeground,
		DrawShadowOnly,
	}

	public enum SKDisplacementMapEffectChannelSelectorType
	{
		Unknown,
		R,
		G,
		B,
		A,
	}

	public enum SKMatrixConvolutionTileMode
	{
		Clamp,
		Repeat,
		ClampToBlack,
	}

	public enum SKPaintStyle
	{
		Fill,
		Stroke,
		StrokeAndFill,
	}

	public enum SKPaintHinting
	{
		NoHinting = 0,
		Slight = 1,
		Normal = 2,
		Full = 3
	}

	public enum SKRegionOperation
	{
		Difference,
		Intersect,
		Union,
		XOR,
		ReverseDifference,
		Replace,
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SKImageInfo {
		public static SKImageInfo Empty;
		public static SKColorType PlatformColorType;

		private int width;
		private int height;
		private SKColorType colorType;
		private SKAlphaType alphaType;

		static SKImageInfo ()
		{
#if WINDOWS_UWP
			var isUnix = false;
#else
			var isUnix = Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix;
#endif
			if (isUnix) {
				// Unix depends on the CPU endianess, but we use RGBA
				PlatformColorType = SKColorType.Rgba8888;
			} else {
				// Windows is always BGRA
				PlatformColorType = SKColorType.Bgra8888;
			}
		}

		public int Width {
			get { return width; }
			set { width = value; }
		}

		public int Height {
			get { return height; }
			set { height = value; }
		}

		public SKColorType ColorType {
			get { return colorType; }
			set { colorType = value; }
		}

		public SKAlphaType AlphaType {
			get { return alphaType; }
			set { alphaType = value; }
		}

		public SKImageInfo (int width, int height)
		{
			this.width = width;
			this.height = height;
			this.colorType = PlatformColorType;
			this.alphaType = SKAlphaType.Premul;
		}

		public SKImageInfo (int width, int height, SKColorType colorType)
		{
			this.width = width;
			this.height = height;
			this.colorType = colorType;
			this.alphaType = SKAlphaType.Premul;
		}

		public SKImageInfo (int width, int height, SKColorType colorType, SKAlphaType alphaType)
		{
			this.width = width;
			this.height = height;
			this.colorType = colorType;
			this.alphaType = alphaType;
		}

		public int BytesPerPixel {
			get {
				switch (ColorType) {
				case SKColorType.Unknown:
					return 0;
				case SKColorType.Alpha8:
				case SKColorType.Index8:
				case SKColorType.Gray8:
					return 1;
				case SKColorType.Rgb565:
				case SKColorType.Argb4444:
					return 2;
				case SKColorType.Bgra8888:
				case SKColorType.Rgba8888:
					return 4;
				case SKColorType.RgbaF16:
					return 8;
				}
				throw new ArgumentOutOfRangeException ("ColorType");
			}
		}

		public int BytesSize {
			get { return Width * Height * BytesPerPixel; }
		}

		public int RowBytes {
			get { return Width * BytesPerPixel; }
		}

		public bool IsEmpty {
			get { return Width <= 0 || Height <= 0; }
		}

		public bool IsOpaque {
			get { return AlphaType == SKAlphaType.Opaque; }
		}

		public SKPointI Size {
			get { return new SKPointI (Width, Height); }
		}

		public SKRectI Rect {
			get { return SKRectI.Create (Width, Height); }
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SKSurfaceProps {
		private SKPixelGeometry pixelGeometry;

		public SKPixelGeometry PixelGeometry {
			get { return pixelGeometry; }
			set { pixelGeometry = value; }
		}
	}

	public enum SKZeroInitialized {
		Yes,
		No,
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SKCodecOptions {
		public static readonly SKCodecOptions Default;

		private SKZeroInitialized zeroInitialized;
		private SKRectI subset;
		private bool hasSubset;

		static SKCodecOptions ()
		{
			Default = new SKCodecOptions (SKZeroInitialized.No);
		}
		public SKCodecOptions (SKZeroInitialized zeroInitialized) {
			this.zeroInitialized = zeroInitialized;
			this.subset = SKRectI.Empty;
			this.hasSubset = false;
		}
		public SKCodecOptions (SKZeroInitialized zeroInitialized, SKRectI subset) {
			this.zeroInitialized = zeroInitialized;
			this.subset = subset;
			this.hasSubset = true;
		}
		public SKZeroInitialized ZeroInitialized{
			get { return zeroInitialized; }
			set { zeroInitialized = value; }
		}

		public SKRectI Subset{
			get { return subset; }
			set { subset = value; }
		}

		public bool HasSubset{
			get { return hasSubset; }
			set { hasSubset = value; }
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SKPoint {
		private float x, y;

		public static readonly SKPoint Empty;

		public static SKPoint operator + (SKPoint pt, SKSizeI sz)
		{
			return new SKPoint (pt.X + sz.Width, pt.Y + sz.Height);
		}
		public static SKPoint operator + (SKPoint pt, SKSize sz)
		{
			return new SKPoint (pt.X + sz.Width, pt.Y + sz.Height);
		}
		
		public static SKPoint operator + (SKPoint pt, SKPointI sz)
		{
			return new SKPoint (pt.X + sz.X, pt.Y + sz.Y);
		}
		public static SKPoint operator + (SKPoint pt, SKPoint sz)
		{
			return new SKPoint (pt.X + sz.X, pt.Y + sz.Y);
		}
		
		public static bool operator == (SKPoint left, SKPoint right)
		{
			return ((left.X == right.X) && (left.Y == right.Y));
		}
		
		public static bool operator != (SKPoint left, SKPoint right)
		{
			return !(left == right);
		}
		
		public static SKPoint operator - (SKPoint pt, SKSizeI sz)
		{
			return new SKPoint (pt.X - sz.Width, pt.Y - sz.Height);
		}
		public static SKPoint operator - (SKPoint pt, SKSize sz)
		{
			return new SKPoint (pt.X - sz.Width, pt.Y - sz.Height);
		}
		
		public static SKPoint operator - (SKPoint pt, SKPointI sz)
		{
			return new SKPoint (pt.X - sz.X, pt.Y - sz.Y);
		}
		public static SKPoint operator - (SKPoint pt, SKPoint sz)
		{
			return new SKPoint (pt.X - sz.X, pt.Y - sz.Y);
		}
		
		public SKPoint (float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public bool IsEmpty {
			get {
				return ((x == 0.0) && (y == 0.0));
			}
		}

		public float X {
			get {
				return x;
			}
			set {
				x = value;
			}
		}

		public float Y {
			get {
				return y;
			}
			set {
				y = value;
			}
		}

		public override bool Equals (object obj)
		{
			if (!(obj is SKPoint))
				return false;

			return (this == (SKPoint) obj);
		}

		public override int GetHashCode ()
		{
			return (int) x ^ (int) y;
		}

		public override string ToString ()
		{
			return String.Format (
				"{{X={0}, Y={1}}}", 
				x.ToString (CultureInfo.CurrentCulture),
				y.ToString (CultureInfo.CurrentCulture));
		}

		public static SKPoint Add (SKPoint pt, SKSizeI sz) => pt + sz;
		public static SKPoint Add (SKPoint pt, SKSize sz) => pt + sz;
		public static SKPoint Add (SKPoint pt, SKPointI sz) => pt + sz;
		public static SKPoint Add (SKPoint pt, SKPoint sz) => pt + sz;

		public static SKPoint Subtract (SKPoint pt, SKSizeI sz) => pt - sz;
		public static SKPoint Subtract (SKPoint pt, SKSize sz) => pt - sz;
		public static SKPoint Subtract (SKPoint pt, SKPointI sz) => pt - sz;
		public static SKPoint Subtract (SKPoint pt, SKPoint sz) => pt - sz;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SKPointI {
		private int x, y;

		public static readonly SKPointI Empty;

		public static SKPointI Ceiling (SKPoint value)
		{
			int x, y;
			checked {
				x = (int) Math.Ceiling (value.X);
				y = (int) Math.Ceiling (value.Y);
			}

			return new SKPointI (x, y);
		}

		public static SKPointI Round (SKPoint value)
		{
			int x, y;
			checked {
				x = (int) Math.Round (value.X);
				y = (int) Math.Round (value.Y);
			}

			return new SKPointI (x, y);
		}

		public static SKPointI Truncate (SKPoint value)
		{
			int x, y;
			checked {
				x = (int) value.X;
				y = (int) value.Y;
			}

			return new SKPointI (x, y);
		}

		public static SKPointI operator + (SKPointI pt, SKSizeI sz)
		{
			return new SKPointI (pt.X + sz.Width, pt.Y + sz.Height);
		}
		
		public static SKPointI operator + (SKPointI pt, SKPointI sz)
		{
			return new SKPointI (pt.X + sz.X, pt.Y + sz.Y);
		}
		
		public static bool operator == (SKPointI left, SKPointI right)
		{
			return ((left.X == right.X) && (left.Y == right.Y));
		}
		
		public static bool operator != (SKPointI left, SKPointI right)
		{
			return !(left == right);
		}
		
		public static SKPointI operator - (SKPointI pt, SKSizeI sz)
		{
			return new SKPointI (pt.X - sz.Width, pt.Y - sz.Height);
		}
		
		public static SKPointI operator - (SKPointI pt, SKPointI sz)
		{
			return new SKPointI (pt.X - sz.X, pt.Y - sz.Y);
		}
		
		public static explicit operator SKSizeI (SKPointI p)
		{
			return new SKSizeI (p.X, p.Y);
		}

		public static implicit operator SKPoint (SKPointI p)
		{
			return new SKPoint (p.X, p.Y);
		}

		public SKPointI (SKSizeI sz)
		{
			x = sz.Width;
			y = sz.Height;
		}

		public SKPointI (int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public bool IsEmpty {
			get {
				return ((x == 0) && (y == 0));
			}
		}

		public int X {
			get {
				return x;
			}
			set {
				x = value;
			}
		}

		public int Y {
			get {
				return y;
			}
			set {
				y = value;
			}
		}

		public override bool Equals (object obj)
		{
			if (!(obj is SKPointI))
				return false;

			return (this == (SKPointI) obj);
		}

		public override int GetHashCode ()
		{
			return x^y;
		}

		public void Offset (int dx, int dy)
		{
			x += dx;
			y += dy;
		}

		public override string ToString ()
		{
			return string.Format (
				"{{X={0},Y={1}}}", 
				x.ToString (CultureInfo.InvariantCulture), 
				y.ToString (CultureInfo.InvariantCulture));
		}

		public void Offset (SKPointI p)
		{
			Offset (p.X, p.Y);
		}

		public static SKPointI Add (SKPointI pt, SKSizeI sz) => pt + sz;
		public static SKPointI Add (SKPointI pt, SKPointI sz) => pt + sz;

		public static SKPointI Subtract (SKPointI pt, SKSizeI sz) => pt - sz;
		public static SKPointI Subtract (SKPointI pt, SKPointI sz) => pt - sz;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SKPoint3
	{
		public static readonly SKPoint3 Empty;
		private float x, y, z;

		public float X {
			get {
				return x;
			}
			set {
				x = value;
			}
		}

		public float Y {
			get {
				return y;
			}
			set {
				y = value;
			}
		}

		public float Z {
			get {
				return z;
			}
			set {
				z = value;
			}
		}

		public SKPoint3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static SKPoint3 operator + (SKPoint3 pt, SKPoint3 sz)
		{
			return new SKPoint3 (pt.X + sz.X, pt.Y + sz.Y, pt.Z + sz.Z);
		}
		
		public static SKPoint3 operator - (SKPoint3 pt, SKPoint3 sz)
		{
			return new SKPoint3 (pt.X - sz.X, pt.Y - sz.Y, pt.Z - sz.Z);
		}
		
		public static bool operator == (SKPoint3 left, SKPoint3 right)
		{
			return ((left.x == right.x) && (left.y == right.y) && (left.z == right.z));
		}
		
		public static bool operator != (SKPoint3 left, SKPoint3 right)
		{
			return !(left == right);
		}

		public bool IsEmpty {
			get {
				return ((x == 0.0) && (y == 0.0) && (z==0.0));
			}
		}

		public override bool Equals (object obj)
		{
			if (!(obj is SKPoint3))
				return false;

			return (this == (SKPoint3) obj);
		}

		public override int GetHashCode ()
		{
			return (int) x ^ (int) y ^ (int) z;
		}

		public override string ToString ()
		{
			return String.Format ("{{X={0}, Y={1}, Z={2}}}",
					      x.ToString (CultureInfo.CurrentCulture),
					      y.ToString (CultureInfo.CurrentCulture),
					      z.ToString (CultureInfo.CurrentCulture)
				);
		}

		public static SKPoint3 Add (SKPoint3 pt, SKPoint3 sz) => pt + sz;
		
		public static SKPoint3 Subtract (SKPoint3 pt, SKPoint3 sz) => pt - sz;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SKSize
	{
		private float width, height;

		public static readonly SKSize Empty;

		public SKSize (float width, float height)
		{
			this.width = width;
			this.height = height;
		}
		
		public SKSize (SKPoint pt)
		{
			this.width = pt.X;
			this.height = pt.Y;
		}

		public bool IsEmpty => (width == 0) && (height == 0);

		public float Width {
			get { return width; }
			set { width = value; }
		}

		public float Height {
			get { return height; }
			set { height = value; }
		}

		public SKPoint ToPoint ()
		{
			return new SKPoint (width, height);
		}

		public SKSizeI ToSizeI ()
		{
			int w, h;
			checked {
				w = (int) width;
				h = (int) height;
			}

			return new SKSizeI (w, h);
		}

		public static SKSize operator + (SKSize sz1, SKSize sz2)
		{
			return new SKSize (sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		}

		public static SKSize operator - (SKSize sz1, SKSize sz2)
		{
			return new SKSize (sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}

		public static bool operator == (SKSize sz1, SKSize sz2)
		{
			return ((sz1.Width == sz2.Width) && (sz1.Height == sz2.Height));
		}

		public static bool operator != (SKSize sz1, SKSize sz2)
		{
			return !(sz1 == sz2);
		}

		public static explicit operator SKPoint (SKSize size)
		{
			return new SKPoint (size.Width, size.Height);
		}

		public static implicit operator SKSize (SKSizeI size)
		{
			return new SKSize (size.Width, size.Height);
		}

		public override bool Equals (object obj)
		{
			if (!(obj is SKSize))
				return false;

			return (this == (SKSize) obj);
		}

		public override int GetHashCode ()
		{
			return (int) width ^ (int) height;
		}

		public override string ToString ()
		{
			return string.Format (
				"{{Width={0}, Height={1}}}", 
				width.ToString (CultureInfo.CurrentCulture),
				height.ToString (CultureInfo.CurrentCulture));
		}

		public static SKSize Add (SKSize sz1, SKSize sz2) => sz1 + sz2;
		
		public static SKSize Subtract (SKSize sz1, SKSize sz2) => sz1 - sz2;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SKSizeI
	{
		private int width, height;

		public static readonly SKSizeI Empty;

		public SKSizeI (int width, int height)
		{
			this.width = width;
			this.height = height;
		}
		
		public SKSizeI (SKPointI pt)
		{
			this.width = pt.X;
			this.height = pt.Y;
		}

		public bool IsEmpty => (width == 0) && (height == 0);

		public int Width {
			get { return width; }
			set { width = value; }
		}

		public int Height {
			get { return height; }
			set { height = value; }
		}

		public SKPointI ToPointI ()
		{
			return new SKPointI (width, height);
		}

		public static SKSizeI operator + (SKSizeI sz1, SKSizeI sz2)
		{
			return new SKSizeI (sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		}

		public static SKSizeI operator - (SKSizeI sz1, SKSizeI sz2)
		{
			return new SKSizeI (sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}

		public static bool operator == (SKSizeI sz1, SKSizeI sz2)
		{
			return ((sz1.Width == sz2.Width) && (sz1.Height == sz2.Height));
		}

		public static bool operator != (SKSizeI sz1, SKSizeI sz2)
		{
			return !(sz1 == sz2);
		}

		public static explicit operator SKPointI (SKSizeI size)
		{
			return new SKPointI (size.Width, size.Height);
		}

		public override bool Equals (object obj)
		{
			if (!(obj is SKSizeI))
				return false;

			return (this == (SKSizeI) obj);
		}

		public override int GetHashCode ()
		{
			return (int) width ^ (int) height;
		}

		public override string ToString ()
		{
			return string.Format (
				"{{Width={0}, Height={1}}}", 
				width.ToString (CultureInfo.CurrentCulture),
				height.ToString (CultureInfo.CurrentCulture));
		}

		public static SKSizeI Add (SKSizeI sz1, SKSizeI sz2) => sz1 + sz2;
		
		public static SKSizeI Subtract (SKSizeI sz1, SKSizeI sz2) => sz1 - sz2;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SKRectI {
		public static readonly SKRectI Empty;
		
		private int left, top, right, bottom;
		
		public SKRectI (int left, int top, int right, int bottom)
		{
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}

		public int Left {
			get { return left; }
			set { left = value; }
		}

		public int Top {
			get { return top; }
			set { top = value; }
		}

		public int Right {
			get { return right; }
			set { right = value; }
		}

		public int Bottom {
			get { return bottom; }
			set { bottom = value; }
		}

		public int Width => right - left;

		public int Height => bottom - top;

		public bool IsEmpty => this == Empty;

		public SKSizeI Size {
			get { return new SKSizeI (Width, Height); }
			set {
				right = left + value.Width;
				bottom = top + value.Height; 
			}
		}

		public SKPointI Location {
			get { return new SKPointI (left, top); }
			set {
				left = value.X;
				top = value.Y; 
			}
		}

		public static SKRectI Ceiling (SKRect value)
		{
			int x, y, r, b;
			checked {
				x = (int) Math.Ceiling (value.Left);
				y = (int) Math.Ceiling (value.Top);
				r = (int) Math.Ceiling (value.Right);
				b = (int) Math.Ceiling (value.Bottom);
			}

			return new SKRectI (x, y, r, b);
		}

		public static SKRectI Inflate (SKRectI rect, int x, int y)
		{
			SKRectI r = new SKRectI (rect.left, rect.top, rect.right, rect.bottom);
			r.Inflate (x, y);
			return r;
		}

		public void Inflate (SKSizeI size)
		{
			Inflate (size.Width, size.Height);
		}

		public void Inflate (int width, int height)
		{
			left -= width;
			top -= height;
			right += width;
			bottom += height;
		}

		public static SKRectI Intersect (SKRectI a, SKRectI b)
		{
			if (!a.IntersectsWithInclusive (b))
				return Empty;

			return new SKRectI (
				Math.Max (a.left, b.left),
				Math.Max (a.top, b.top),
				Math.Min (a.right, b.right),
				Math.Min (a.bottom, b.bottom));
		}

		public void Intersect (SKRectI rect)
		{
			this = SKRectI.Intersect (this, rect);
		}

		public static SKRectI Round (SKRect value)
		{
			int x, y, r, b;
			checked {
				x = (int) Math.Round (value.Left);
				y = (int) Math.Round (value.Top);
				r = (int) Math.Round (value.Right);
				b = (int) Math.Round (value.Bottom);
			}

			return new SKRectI (x, y, r, b);
		}

		public static SKRectI Truncate (SKRect value)
		{
			int x, y, r, b;
			checked {
				x = (int) value.Left;
				y = (int) value.Top;
				r = (int) value.Right;
				b = (int) value.Bottom;
			}

			return new SKRectI (x, y, r, b);
		}

		public static SKRectI Union (SKRectI a, SKRectI b)
		{
			return new SKRectI (
				Math.Min (a.Left, b.Left),
				Math.Min (a.Top, b.Top),
				Math.Max (a.Right, b.Right),
				Math.Max (a.Bottom, b.Bottom));
		}

		public void Union (SKRectI rect)
		{
			this = SKRectI.Union (this, rect);
		}

		public bool Contains (int x, int y)
		{
			return 
				(x >= left) && (x < right) && 
				(y >= top) && (y < bottom);
		}

		public bool Contains (SKPointI pt)
		{
			return Contains (pt.X, pt.Y);
		}

		public bool Contains (SKRectI rect)
		{
			return
				(left <= rect.left) && (right >= rect.right) && 
				(top <= rect.top) && (bottom >= rect.bottom);
		}

		public override bool Equals (object obj)
		{
			if (!(obj is SKRectI))
				return false;

			return (this == (SKRectI) obj);
		}
		
		public override int GetHashCode ()
		{
			return unchecked((int)(
				(((UInt32)Left)) ^ 
				(((UInt32)Top << 13) | ((UInt32)Top >> 19)) ^
				(((UInt32)Width << 26) | ((UInt32)Width >>  6)) ^
				(((UInt32)Height <<  7) | ((UInt32)Height >> 25))));
		}

		public bool IntersectsWith (SKRectI rect)
		{
			return
				!((left >= rect.right) || (right <= rect.left) ||
				  (top >= rect.bottom) || (bottom <= rect.top));
		}

		private bool IntersectsWithInclusive (SKRectI r)
		{
			return
				!((left > r.right) || (right < r.left) ||
				  (top > r.bottom) || (bottom < r.top));
		}
		
		public void Offset (int x, int y)
		{
			left += x;
			top += y;
			right += x;
			bottom += y;
		}

		public void Offset (SKPointI pos)
		{
			Offset (pos.X, pos.Y);
		}

		public override string ToString ()
		{
			return String.Format (
				"{{Left={0},Top={1},Width={2},Height={3}}}",
				Left.ToString (CultureInfo.CurrentCulture),
				Top.ToString (CultureInfo.CurrentCulture),
				Width.ToString (CultureInfo.CurrentCulture),
				Height.ToString (CultureInfo.CurrentCulture));
		}

		public static bool operator == (SKRectI left, SKRectI right)
		{
			return
				(left.left == right.left) && (left.top == right.top) &&
				(left.right == right.right) && (left.bottom == right.bottom);
		}

		public static bool operator != (SKRectI left, SKRectI right)
		{
			return !(left == right);
		}

		public static SKRectI Create (SKSizeI size)
		{
			return SKRectI.Create (SKPointI.Empty.X, SKPointI.Empty.Y, size.Width, size.Height);
		}

		public static SKRectI Create (SKPointI location, SKSizeI size)
		{
			return SKRectI.Create (location.X, location.Y, size.Width, size.Height);
		}

		public static SKRectI Create (int width, int height)
		{
			return new SKRectI (SKPointI.Empty.X, SKPointI.Empty.X, width, height);
		}

		public static SKRectI Create (int x, int y, int width, int height)
		{
			return new SKRectI (x, y, x + width, y + height);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SKRect {
		public static readonly SKRect Empty;

		private float left, top, right, bottom;

		public SKRect (float left, float top, float right, float bottom)
		{
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}

		public float Left {
			get { return left; }
			set { left = value; }
		}

		public float Top {
			get { return top; }
			set { top = value; }
		}

		public float Right {
			get { return right; }
			set { right = value; }
		}

		public float Bottom {
			get { return bottom; }
			set { bottom = value; }
		}

		public float Width => right - left;

		public float Height => bottom - top;

		public bool IsEmpty => this == Empty;

		public SKSize Size {
			get { return new SKSize (Width, Height); }
			set {
				right = left + value.Width;
				bottom = top + value.Height; 
			}
		}

		public SKPoint Location {
			get { return new SKPoint (left, top); }
			set {
				left = value.X;
				top = value.Y; 
			}
		}

		public static SKRect Inflate (SKRect rect, float x, float y)
		{
			var r = new SKRect (rect.left, rect.top, rect.right, rect.bottom);
			r.Inflate (x, y);
			return r;
		}

		public void Inflate (SKSize size)
		{
			Inflate (size.Width, size.Height);
		}

		public void Inflate (float x, float y)
		{
			left -= x;
			top -= y;
			right += x;
			bottom += y;
		}

		public static SKRect Intersect (SKRect a, SKRect b)
		{
			if (!a.IntersectsWithInclusive (b)) {
				return Empty;
			}
			return new SKRect (
				Math.Max (a.left, b.left),
				Math.Max (a.top, b.top),
				Math.Min (a.right, b.right),
				Math.Min (a.bottom, b.bottom));
		}

		public void Intersect (SKRect rect)
		{
			this = SKRect.Intersect (this, rect);
		}

		public static SKRect Union (SKRect a, SKRect b)
		{
			return new SKRect (
				Math.Min (a.left, b.left),
				Math.Min (a.top, b.top),
				Math.Max (a.right, b.right),
				Math.Max (a.bottom, b.bottom));
		}

		public void Union (SKRect rect)
		{
			this = SKRect.Union (this, rect);
		}

		public static bool operator == (SKRect left, SKRect right)
		{
			return
				(left.left == right.left) && (left.top == right.top) &&
				(left.right == right.right) && (left.bottom == right.bottom);
		}

		public static bool operator != (SKRect left, SKRect right)
		{
			return !(left == right);
		}

		public static implicit operator SKRect (SKRectI r)
		{
			return new SKRect (r.Left, r.Top, r.Right, r.Bottom);
		}

		public bool Contains (float x, float y)
		{
			return (x >= left) && (x < right) && (y >= top) && (y < bottom);
		}

		public bool Contains (SKPoint pt)
		{
			return Contains (pt.X, pt.Y);
		}

		public bool Contains (SKRect rect)
		{
			return 
				(left <= rect.left) && (right >= rect.right) && 
				(top <= rect.top) && (bottom >= rect.bottom);
		}
		
		public override bool Equals (object obj)
		{
			if (!(obj is SKRect))
				return false;

			return this == (SKRect) obj;
		}

		public override int GetHashCode ()
		{
			return unchecked((int)(
				(((UInt32)Left)) ^ 
				(((UInt32)Top << 13) | ((UInt32)Top >> 19)) ^
				(((UInt32)Width << 26) | ((UInt32)Width >>  6)) ^
				(((UInt32)Height <<  7) | ((UInt32)Height >> 25))));
		}

		public bool IntersectsWith (SKRect rect)
		{
			return !((left >= rect.right) || (right <= rect.left) ||
					(top >= rect.bottom) || (bottom <= rect.top));
		}

		public bool IntersectsWithInclusive (SKRect rect)
		{
			return !((left > rect.right) || (right < rect.left) ||
					 (top > rect.bottom) || (bottom < rect.top));
		}
		
		public void Offset (float x, float y)
		{
			left += x;
			top += y;
			right += x;
			bottom += y;
		}

		public void Offset (SKPoint pos)
		{
			Offset (pos.X, pos.Y);
		}

		public override string ToString ()
		{
			return String.Format (
				"{{Left={0},Top={1},Width={2},Height={3}}}",
				Left.ToString (CultureInfo.CurrentCulture),
				Top.ToString (CultureInfo.CurrentCulture),
				Width.ToString (CultureInfo.CurrentCulture),
				Height.ToString (CultureInfo.CurrentCulture));
		}

		public static SKRect Create (SKPoint location, SKSize size)
		{
			return SKRect.Create (location.X, location.Y, size.Width, size.Height);
		}

		public static SKRect Create (SKSize size)
		{
			return SKRect.Create (SKPoint.Empty, size);
		}

		public static SKRect Create (float width, float height)
		{
			return new SKRect (SKPoint.Empty.X, SKPoint.Empty.Y, width, height);
		}

		public static SKRect Create (float x, float y, float width, float height)
		{
			return new SKRect (x, y, x + width, y + height);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SKMatrix {
		public float ScaleX, SkewX, TransX;
		public float SkewY, ScaleY, TransY;
		public float Persp0, Persp1, Persp2;

#if OPTIMIZED_SKMATRIX

		//
		// If we manage to get an sk_matrix_t that contains the extra
		// the fTypeMask flag, we could accelerate various operations
		// as well, as this caches state of what is needed to be done.
		//
	
		[Flags]
		enum Mask : uint {
			Identity = 0,
			Translate = 1,
			Scale = 2,
			Affine = 4,
			Perspective = 8,
			RectStaysRect = 0x10,
			OnlyPerspectiveValid = 0x40,
			Unknown = 0x80,
			OrableMasks = Translate | Scale | Affine | Perspective,
			AllMasks = OrableMasks | RectStaysRect
		}
		Mask typeMask;

		Mask GetMask ()
		{
			if (typeMask.HasFlag (Mask.Unknown))
				typeMask = (Mask) SkiaApi.sk_matrix_get_type (ref this);

		        // only return the public masks
			return (Mask) ((uint)typeMask & 0xf);
		}
#endif

		static float sdot (float a, float b, float c, float d) => a * b + c * d;
		static float scross(float a, float b, float c, float d) => a * b - c * d;

		public static SKMatrix MakeIdentity ()
		{
			return new SKMatrix () { ScaleX = 1, ScaleY = 1, Persp2 = 1
#if OPTIMIZED_SKMATRIX
					, typeMask = Mask.Identity | Mask.RectStaysRect
#endif
                        };
		}

		public void SetScaleTranslate (float sx, float sy, float tx, float ty)
		{
			ScaleX = sx;
			SkewX = 0;
			TransX = tx;

			SkewY = 0;
			ScaleY = sy;
			TransY = ty;

			Persp0 = 0;
			Persp1 = 0;
			Persp2 = 1;

#if OPTIMIZED_SKMATRIX
			typeMask = Mask.RectStaysRect | 
				((sx != 1 || sy != 1) ? Mask.Scale : 0) |
				((tx != 0 || ty != 0) ? Mask.Translate : 0);
#endif
		}

		public static SKMatrix MakeScale (float sx, float sy)
		{
			if (sx == 1 && sy == 1)
				return MakeIdentity ();
			return new SKMatrix () { ScaleX = sx, ScaleY = sy, Persp2 = 1, 
#if OPTIMIZED_SKMATRIX
typeMask = Mask.Scale | Mask.RectStaysRect
#endif
			};
				
		}

		/// <summary>
		/// Set the matrix to scale by sx and sy, with a pivot point at (px, py).
		/// The pivot point is the coordinate that should remain unchanged by the
		/// specified transformation.
		public static SKMatrix MakeScale (float sx, float sy, float pivotX, float pivotY)
		{
			if (sx == 1 && sy == 1)
				return MakeIdentity ();
			float tx = pivotX - sx * pivotX;
			float ty = pivotY - sy * pivotY;

#if OPTIMIZED_SKMATRIX
			Mask mask = Mask.RectStaysRect | 
				((sx != 1 || sy != 1) ? Mask.Scale : 0) |
				((tx != 0 || ty != 0) ? Mask.Translate : 0);
#endif
			return new SKMatrix () { 
				ScaleX = sx, ScaleY = sy, 
				TransX = tx,
				TransY = ty,
				Persp2 = 1,
#if OPTIMIZED_SKMATRIX
				typeMask = mask
#endif
			};
		}

		public static SKMatrix MakeTranslation (float dx, float dy)
		{
			if (dx == 0 && dy == 0)
				return MakeIdentity ();
			
			return new SKMatrix () { 
				ScaleX = 1, ScaleY = 1,
				TransX = dx, TransY = dy,
				Persp2 = 1,
#if OPTIMIZED_SKMATRIX
				typeMask = Mask.Translate | Mask.RectStaysRect
#endif
			};
		}

		public static SKMatrix MakeRotation (float radians)
		{
			var sin = (float) Math.Sin (radians);
			var cos = (float) Math.Cos (radians);

			var matrix = new SKMatrix ();
			SetSinCos (ref matrix, sin, cos);
			return matrix;
		}

		public static SKMatrix MakeRotation (float radians, float pivotx, float pivoty)
		{
			var sin = (float) Math.Sin (radians);
			var cos = (float) Math.Cos (radians);

			var matrix = new SKMatrix ();
			SetSinCos (ref matrix, sin, cos, pivotx, pivoty);
			return matrix;
		}

		const float degToRad = (float)System.Math.PI / 180.0f;
		
		public static SKMatrix MakeRotationDegrees (float degrees)
		{
			return MakeRotation (degrees * degToRad);
		}

		public static SKMatrix MakeRotationDegrees (float degrees, float pivotx, float pivoty)
		{
			return MakeRotation (degrees * degToRad, pivotx, pivoty);
		}

		static void SetSinCos (ref SKMatrix matrix, float sin, float cos)
		{
			matrix.ScaleX = cos;
			matrix.SkewX = -sin;
			matrix.TransX = 0;
			matrix.SkewY = sin;
			matrix.ScaleY = cos;
			matrix.TransY = 0;
			matrix.Persp0 = 0;
			matrix.Persp1 = 0;
			matrix.Persp2 = 1;
#if OPTIMIZED_SKMATRIX
			matrix.typeMask = Mask.Unknown | Mask.OnlyPerspectiveValid;
#endif
		}

		static void SetSinCos (ref SKMatrix matrix, float sin, float cos, float pivotx, float pivoty)
		{
			float oneMinusCos = 1-cos;
			
			matrix.ScaleX = cos;
			matrix.SkewX = -sin;
			matrix.TransX = sdot(sin, pivoty, oneMinusCos, pivotx);
			matrix.SkewY = sin;
			matrix.ScaleY = cos;
			matrix.TransY = sdot(-sin, pivotx, oneMinusCos, pivoty);
			matrix.Persp0 = 0;
			matrix.Persp1 = 0;
			matrix.Persp2 = 1;
#if OPTIMIZED_SKMATRIX
			matrix.typeMask = Mask.Unknown | Mask.OnlyPerspectiveValid;
#endif
		}
		
		public static void Rotate (ref SKMatrix matrix, float radians, float pivotx, float pivoty)
		{
			var sin = (float) Math.Sin (radians);
			var cos = (float) Math.Cos (radians);
			SetSinCos (ref matrix, sin, cos, pivotx, pivoty);
		}

		public static void RotateDegrees (ref SKMatrix matrix, float degrees, float pivotx, float pivoty)
		{
			var sin = (float) Math.Sin (degrees * degToRad);
			var cos = (float) Math.Cos (degrees * degToRad);
			SetSinCos (ref matrix, sin, cos, pivotx, pivoty);
		}

		public static void Rotate (ref SKMatrix matrix, float radians)
		{
			var sin = (float) Math.Sin (radians);
			var cos = (float) Math.Cos (radians);
			SetSinCos (ref matrix, sin, cos);
		}

		public static void RotateDegrees (ref SKMatrix matrix, float degrees)
		{
			var sin = (float) Math.Sin (degrees * degToRad);
			var cos = (float) Math.Cos (degrees * degToRad);
			SetSinCos (ref matrix, sin, cos);
		}

		public static SKMatrix MakeSkew (float sx, float sy)
		{
			return new SKMatrix () {
				ScaleX = 1,
				SkewX = sx,
				TransX = 0,
				SkewY = sy,
				ScaleY = 1,
				TransY = 0,
				Persp0 = 0,
				Persp1 = 0,
				Persp2 = 1,
#if OPTIMIZED_SKMATRIX
				typeMask = Mask.Unknown | Mask.OnlyPerspectiveValid
#endif
			};
		}

		public bool TryInvert (out SKMatrix inverse)
		{
			return SkiaApi.sk_matrix_try_invert (ref this, out inverse) != 0;
		}

		public static void Concat (ref SKMatrix target, ref SKMatrix first, ref SKMatrix second)
		{
			SkiaApi.sk_matrix_concat (ref target, ref first, ref second);
		}

		public static void PreConcat (ref SKMatrix target, ref SKMatrix matrix)
		{
			SkiaApi.sk_matrix_pre_concat (ref target, ref matrix);
		}

		public static void PostConcat (ref SKMatrix target, ref SKMatrix matrix)
		{
			SkiaApi.sk_matrix_post_concat (ref target, ref matrix);
		}

		public void MapRect (ref SKMatrix matrix, out SKRect dest, ref SKRect source)
		{
			SkiaApi.sk_matrix_map_rect (ref matrix, out dest, ref source);
		}

		public SKRect MapRect (SKRect source)
		{
			SKRect result;
			MapRect (ref this, out result, ref source);
			return result;
		}

		public void MapPoints (SKPoint [] result, SKPoint [] points)
		{
			if (result == null)
				throw new ArgumentNullException (nameof (result));
			if (points == null)
				throw new ArgumentNullException (nameof (points));
			int dl = result.Length;
			if (dl != points.Length)
				throw new ArgumentException ("buffers must be the same size");
			unsafe {
				fixed (SKPoint *rp = &result[0]){
					fixed (SKPoint *pp = &points[0]){
						SkiaApi.sk_matrix_map_points (ref this, (IntPtr) rp, (IntPtr) pp, dl);
					}
				}
			}
		}

		public SKPoint [] MapPoints (SKPoint [] points)
		{
			if (points == null)
				throw new ArgumentNullException (nameof (points));
			var res = new SKPoint [points.Length];
			MapPoints (res, points);
			return res;
		}

		public void MapVectors (SKPoint [] result, SKPoint [] vectors)
		{
			if (result == null)
				throw new ArgumentNullException (nameof (result));
			if (vectors == null)
				throw new ArgumentNullException (nameof (vectors));
			int dl = result.Length;
			if (dl != vectors.Length)
				throw new ArgumentException ("buffers must be the same size");
			unsafe {
				fixed (SKPoint *rp = &result[0]){
					fixed (SKPoint *pp = &vectors[0]){
						SkiaApi.sk_matrix_map_vectors (ref this, (IntPtr) rp, (IntPtr) pp, dl);
					}
				}
			}
		}

		public SKPoint [] MapVectors (SKPoint [] vectors)
		{
			if (vectors == null)
				throw new ArgumentNullException (nameof (vectors));
			var res = new SKPoint [vectors.Length];
			MapVectors (res, vectors);
			return res;
		}

		public SKPoint MapXY (float x, float y)
		{
			SKPoint result;
			SkiaApi.sk_matrix_map_xy (ref this, x, y, out result);
			return result;
		}

		public SKPoint MapVector (float x, float y)
		{
			SKPoint result;
			SkiaApi.sk_matrix_map_vector(ref this, x, y, out result);
			return result;
		}

		public float MapRadius (float radius)
		{
			return SkiaApi.sk_matrix_map_radius (ref this, radius);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SKFontMetrics
	{
		uint flags;                     // Bit field to identify which values are unknown
		float top;                      // The greatest distance above the baseline for any glyph (will be <= 0)
		float ascent;                   // The recommended distance above the baseline (will be <= 0)
		float descent;                  // The recommended distance below the baseline (will be >= 0)
		float bottom;                   // The greatest distance below the baseline for any glyph (will be >= 0)
		float leading;                  // The recommended distance to add between lines of text (will be >= 0)
		float avgCharWidth;             // the average character width (>= 0)
		float maxCharWidth;             // the max character width (>= 0)
		float xMin;                     // The minimum bounding box x value for all glyphs
		float xMax;                     // The maximum bounding box x value for all glyphs
		float xHeight;                  // The height of an 'x' in px, or 0 if no 'x' in face
		float capHeight;                // The cap height (> 0), or 0 if cannot be determined.
		float underlineThickness;       // underline thickness, or 0 if cannot be determined
		float underlinePosition;        // underline position, or 0 if cannot be determined

		const uint flagsUnderlineThicknessIsValid = (1U << 0);
		const uint flagsUnderlinePositionIsValid = (1U << 1);

		public float Top
		{
			get { return top; }
		}

		public float Ascent
		{
			get { return ascent; }
		}

		public float Descent
		{
			get { return descent; }
		}

		public float Bottom
		{
			get { return bottom; }
		}

		public float Leading
		{
			get { return leading; }
		}

		public float AverageCharacterWidth
		{
			get { return avgCharWidth; }
		}

		public float MaxCharacterWidth
		{
			get { return maxCharWidth; }
		}

		public float XMin
		{
			get { return xMin; }
		}

		public float XMax
		{
			get { return xMax; }
		}

		public float XHeight
		{
			get { return xHeight; }
		}

		public float CapHeight
		{
			get { return capHeight; }
		}

		public float? UnderlineThickness
		{
			get {
				if ((flags & flagsUnderlineThicknessIsValid) != 0)
					return underlineThickness;
				else
					return null;
			}
		}

		public float? UnderlinePosition
		{
			get {
				if ((flags & flagsUnderlinePositionIsValid) != 0)
					return underlinePosition;
				else
					return null;
			}
		}
	}
}

