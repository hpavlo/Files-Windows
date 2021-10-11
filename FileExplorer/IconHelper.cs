using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;

namespace FileExplorer
{
	/// <summary>
	/// Provides static methods to read system icons for both folders and files.
	/// </summary>
	/// <example>
	/// <code>IconReader.GetFileIcon("c:\\general.xls");</code>
	/// </example>
	public class IconReader
	{
		/// <summary>
		/// Options to specify the size of icons to return.
		/// </summary>
		public enum IconSize
		{
			/// <summary>
			/// Specify large icon - 32 pixels by 32 pixels.
			/// </summary>
			Large = 0,
			/// <summary>
			/// Specify small icon - 16 pixels by 16 pixels.
			/// </summary>
			Small = 1
		}
        
		/// <summary>
		/// Options to specify whether folders should be in the open or closed state.
		/// </summary>
		public enum FolderType
		{
			/// <summary>
			/// Specify open folder.
			/// </summary>
			Open = 0,
			/// <summary>
			/// Specify closed folder.
			/// </summary>
			Closed = 1
		}




		/*

		[Flags]
		enum SHGFI : uint
		{
			/// <summary>get icon</summary>
			Icon = 0x000000100,
			/// <summary>get display name</summary>
			DisplayName = 0x000000200,
			/// <summary>get type name</summary>
			TypeName = 0x000000400,
			/// <summary>get attributes</summary>
			Attributes = 0x000000800,
			/// <summary>get icon location</summary>
			IconLocation = 0x000001000,
			/// <summary>return exe type</summary>
			ExeType = 0x000002000,
			/// <summary>get system icon index</summary>
			SysIconIndex = 0x000004000,
			/// <summary>put a link overlay on icon</summary>
			LinkOverlay = 0x000008000,
			/// <summary>show icon in selected state</summary>
			Selected = 0x000010000,
			/// <summary>get only specified attributes</summary>
			Attr_Specified = 0x000020000,
			/// <summary>get large icon</summary>
			LargeIcon = 0x000000000,
			/// <summary>get small icon</summary>
			SmallIcon = 0x000000001,
			/// <summary>get open icon</summary>
			OpenIcon = 0x000000002,
			/// <summary>get shell size icon</summary>
			ShellIconSize = 0x000000004,
			/// <summary>pszPath is a pidl</summary>
			PIDL = 0x000000008,
			/// <summary>use passed dwFileAttribute</summary>
			UseFileAttributes = 0x000000010,
			/// <summary>apply the appropriate overlays</summary>
			AddOverlays = 0x000000020,
			/// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
			OverlayIndex = 0x000000040,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SHFILEINFO
		{
			public const int NAMESIZE = 80;
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};


		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left, top, right, bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			int x;
			int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct IMAGELISTDRAWPARAMS
		{
			public int cbSize;
			public IntPtr himl;
			public int i;
			public IntPtr hdcDst;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public int xBitmap;    // x offest from the upperleft of bitmap
			public int yBitmap;    // y offset from the upperleft of bitmap
			public int rgbBk;
			public int rgbFg;
			public int fStyle;
			public int dwRop;
			public int fState;
			public int Frame;
			public int crEffect;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct IMAGEINFO
		{
			public IntPtr hbmImage;
			public IntPtr hbmMask;
			public int Unused1;
			public int Unused2;
			public RECT rcImage;
		}
		[ComImportAttribute()]
		[GuidAttribute("46EB5926-582E-4017-9FDF-E8998DAA0950")]
		[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IImageList
		{
			[PreserveSig]
			int Add(
			IntPtr hbmImage,
			IntPtr hbmMask,
			ref int pi);

			[PreserveSig]
			int ReplaceIcon(
			int i,
			IntPtr hicon,
			ref int pi);

			[PreserveSig]
			int SetOverlayImage(
			int iImage,
			int iOverlay);

			[PreserveSig]
			int Replace(
			int i,
			IntPtr hbmImage,
			IntPtr hbmMask);

			[PreserveSig]
			int AddMasked(
			IntPtr hbmImage,
			int crMask,
			ref int pi);

			[PreserveSig]
			int Draw(
			ref IMAGELISTDRAWPARAMS pimldp);

			[PreserveSig]
			int Remove(
		int i);

			[PreserveSig]
			int GetIcon(
			int i,
			int flags,
			ref IntPtr picon);

			[PreserveSig]
			int GetImageInfo(
			int i,
			ref IMAGEINFO pImageInfo);

			[PreserveSig]
			int Copy(
			int iDst,
			IImageList punkSrc,
			int iSrc,
			int uFlags);

			[PreserveSig]
			int Merge(
			int i1,
			IImageList punk2,
			int i2,
			int dx,
			int dy,
			ref Guid riid,
			ref IntPtr ppv);

			[PreserveSig]
			int Clone(
			ref Guid riid,
			ref IntPtr ppv);

			[PreserveSig]
			int GetImageRect(
			int i,
			ref RECT prc);

			[PreserveSig]
			int GetIconSize(
			ref int cx,
			ref int cy);

			[PreserveSig]
			int SetIconSize(
			int cx,
			int cy);

			[PreserveSig]
			int GetImageCount(
		ref int pi);

			[PreserveSig]
			int SetImageCount(
			int uNewCount);

			[PreserveSig]
			int SetBkColor(
			int clrBk,
			ref int pclr);

			[PreserveSig]
			int GetBkColor(
			ref int pclr);

			[PreserveSig]
			int BeginDrag(
			int iTrack,
			int dxHotspot,
			int dyHotspot);

			[PreserveSig]
			int EndDrag();

			[PreserveSig]
			int DragEnter(
			IntPtr hwndLock,
			int x,
			int y);

			[PreserveSig]
			int DragLeave(
			IntPtr hwndLock);

			[PreserveSig]
			int DragMove(
			int x,
			int y);

			[PreserveSig]
			int SetDragCursorImage(
			ref IImageList punk,
			int iDrag,
			int dxHotspot,
			int dyHotspot);

			[PreserveSig]
			int DragShowNolock(
			int fShow);

			[PreserveSig]
			int GetDragImage(
			ref POINT ppt,
			ref POINT pptHotspot,
			ref Guid riid,
			ref IntPtr ppv);

			[PreserveSig]
			int GetItemFlags(
			int i,
			ref int dwFlags);

			[PreserveSig]
			int GetOverlayImage(
			int iOverlay,
			ref int piIndex);
		};
		[DllImport("Shell32.dll")]
		public static extern IntPtr SHGetFileInfo(
			string pszPath,
			uint dwFileAttributes,
			ref SHFILEINFO psfi,
			uint cbFileInfo,
			uint uFlags
			);
		public static int GetIconIndex(string pszFile)
		{
			SHFILEINFO sfi = new SHFILEINFO();
			SHGetFileInfo(pszFile
				, 0
				, ref sfi
				, (uint)System.Runtime.InteropServices.Marshal.SizeOf(sfi)
				, (uint)(SHGFI.SysIconIndex | SHGFI.LargeIcon | SHGFI.UseFileAttributes));
			return sfi.iIcon;
		}

		// 256*256
		public static IntPtr GetJumboIcon(int iImage)
		{
			IImageList spiml = null;
			Guid guil = new Guid(IID_IImageList2);//or IID_IImageList

			SHGetImageList(Shell322.SHIL_JUMBO, ref guil, ref spiml);
			IntPtr hIcon = IntPtr.Zero;
			spiml.GetIcon(iImage, Shell322.ILD_TRANSPARENT | Shell322.ILD_IMAGE, ref hIcon); //

			return hIcon;
		}
		[DllImport("shell32.dll", EntryPoint = "#727")]
		public extern static int SHGetImageList(int iImageList, ref Guid riid, ref IImageList ppv);
		const string IID_IImageList = "46EB5926-582E-4017-9FDF-E8998DAA0950";
		const string IID_IImageList2 = "192B9D83-50FC-457B-90A0-2B82A8B5DAE1";
		public static IntPtr GetXLIcon(int iImage)
		{
			IImageList spiml = null;
			Guid guil = new Guid(IID_IImageList2);//or IID_IImageList

			SHGetImageList(Shell322.SHIL_EXTRALARGE, ref guil, ref spiml);
			IntPtr hIcon = IntPtr.Zero;
			spiml.GetIcon(iImage, Shell322.ILD_TRANSPARENT | Shell322.ILD_IMAGE, ref hIcon); //

			return hIcon;
		}
		public static class Shell322
		{

			public const int SHIL_LARGE = 0x0;
			public const int SHIL_SMALL = 0x1;
			public const int SHIL_EXTRALARGE = 0x2;
			public const int SHIL_SYSSMALL = 0x3;
			public const int SHIL_JUMBO = 0x4;
			public const int SHIL_LAST = 0x4;

			public const int ILD_TRANSPARENT = 0x00000001;
			public const int ILD_IMAGE = 0x00000020;
		}

		*/




		/// <summary>
		/// Returns an icon for a given file - indicated by the name parameter.
		/// </summary>
		/// <param name="name">Pathname for file.</param>
		/// <param name="size">Large or small</param>
		/// <param name="linkOverlay">Whether to include the link icon</param>
		/// <returns>System.Drawing.Icon</returns>
		public static System.Drawing.Icon GetFileIcon(string name, IconSize size, bool linkOverlay)
		{
			Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
			uint flags = Shell32.SHGFI_ICON | Shell32.SHGFI_USEFILEATTRIBUTES;

			if (true == linkOverlay) flags += Shell32.SHGFI_LINKOVERLAY;

			/* Check the size specified for return. */
			if (IconSize.Small == size)
			{
				flags += Shell32.SHGFI_SMALLICON ;
			} 
			else 
			{
				flags += Shell32.SHGFI_LARGEICON;
			}

			Shell32.SHGetFileInfo(	name, 
				Shell32.FILE_ATTRIBUTE_NORMAL, 
				ref shfi, 
				(uint) System.Runtime.InteropServices.Marshal.SizeOf(shfi),
				flags);

			// Copy (clone) the returned icon to a new object, thus allowing us to clean-up properly
			System.Drawing.Icon icon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();
			User32.DestroyIcon( shfi.hIcon );		// Cleanup
			return icon;
		}

		/// <summary>
		/// Used to access system folder icons.
		/// </summary>
		/// <param name="size">Specify large or small icons.</param>
		/// <param name="folderType">Specify open or closed FolderType.</param>
		/// <returns>System.Drawing.Icon</returns>
		public static System.Drawing.Icon GetFolderIcon( IconSize size, FolderType folderType )
		{
			// Need to add size check, although errors generated at present!
			uint flags = Shell32.SHGFI_ICON | Shell32.SHGFI_USEFILEATTRIBUTES;

			if (FolderType.Open == folderType)
			{
				flags += Shell32.SHGFI_OPENICON;
			}
			
			if (IconSize.Small == size)
			{
				flags += Shell32.SHGFI_SMALLICON;
			} 
			else 
			{
				flags += Shell32.SHGFI_LARGEICON;
			}

			// Get the folder icon
			Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
			Shell32.SHGetFileInfo(	null, 
				Shell32.FILE_ATTRIBUTE_DIRECTORY, 
				ref shfi, 
				(uint) System.Runtime.InteropServices.Marshal.SizeOf(shfi), 
				flags );

			System.Drawing.Icon.FromHandle(shfi.hIcon);	// Load the icon from an HICON handle

			// Now clone the icon, so that it can be successfully stored in an ImageList
			System.Drawing.Icon icon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();

			User32.DestroyIcon( shfi.hIcon );		// Cleanup
			return icon;
		}	
	}

	/// <summary>
	/// Wraps necessary Shell32.dll structures and functions required to retrieve Icon Handles using SHGetFileInfo. Code
	/// courtesy of MSDN Cold Rooster Consulting case study.
	/// </summary>
	/// 

	// This code has been left largely untouched from that in the CRC example. The main changes have been moving
	// the icon reading code over to the IconReader type.
	public class Shell32  
	{
		
		public const int 	MAX_PATH = 256;
		[StructLayout(LayoutKind.Sequential)]
			public struct SHITEMID
		{
			public ushort cb;
			[MarshalAs(UnmanagedType.LPArray)]
			public byte[] abID;
		}

		[StructLayout(LayoutKind.Sequential)]
			public struct ITEMIDLIST
		{
			public SHITEMID mkid;
		}

		[StructLayout(LayoutKind.Sequential)]
			public struct BROWSEINFO 
		{ 
			public IntPtr		hwndOwner; 
			public IntPtr		pidlRoot; 
			public IntPtr 		pszDisplayName;
			[MarshalAs(UnmanagedType.LPTStr)] 
			public string 		lpszTitle; 
			public uint 		ulFlags; 
			public IntPtr		lpfn; 
			public int			lParam; 
			public IntPtr 		iImage; 
		} 

		// Browsing for directory.
		public const uint BIF_RETURNONLYFSDIRS   =	0x0001;
		public const uint BIF_DONTGOBELOWDOMAIN  =	0x0002;
		public const uint BIF_STATUSTEXT         =	0x0004;
		public const uint BIF_RETURNFSANCESTORS  =	0x0008;
		public const uint BIF_EDITBOX            =	0x0010;
		public const uint BIF_VALIDATE           =	0x0020;
		public const uint BIF_NEWDIALOGSTYLE     =	0x0040;
		public const uint BIF_USENEWUI           =	(BIF_NEWDIALOGSTYLE | BIF_EDITBOX);
		public const uint BIF_BROWSEINCLUDEURLS  =	0x0080;
		public const uint BIF_BROWSEFORCOMPUTER  =	0x1000;
		public const uint BIF_BROWSEFORPRINTER   =	0x2000;
		public const uint BIF_BROWSEINCLUDEFILES =	0x4000;
		public const uint BIF_SHAREABLE          =	0x8000;

		[StructLayout(LayoutKind.Sequential)]
			public struct SHFILEINFO
		{ 
			public const int NAMESIZE = 80;
			public IntPtr	hIcon; 
			public int		iIcon; 
			public uint	dwAttributes; 
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=MAX_PATH)]
			public string szDisplayName; 
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=NAMESIZE)]
			public string szTypeName; 
		};

		public const uint SHGFI_ICON				= 0x000000100;     // get icon
		public const uint SHGFI_DISPLAYNAME			= 0x000000200;     // get display name
		public const uint SHGFI_TYPENAME          	= 0x000000400;     // get type name
		public const uint SHGFI_ATTRIBUTES        	= 0x000000800;     // get attributes
		public const uint SHGFI_ICONLOCATION      	= 0x000001000;     // get icon location
		public const uint SHGFI_EXETYPE           	= 0x000002000;     // return exe type
		public const uint SHGFI_SYSICONINDEX      	= 0x000004000;     // get system icon index
		public const uint SHGFI_LINKOVERLAY       	= 0x000008000;     // put a link overlay on icon
		public const uint SHGFI_SELECTED          	= 0x000010000;     // show icon in selected state
		public const uint SHGFI_ATTR_SPECIFIED    	= 0x000020000;     // get only specified attributes
		public const uint SHGFI_LARGEICON         	= 0x000000000;     // get large icon
		public const uint SHGFI_SMALLICON         	= 0x000000001;     // get small icon
		public const uint SHGFI_OPENICON          	= 0x000000002;     // get open icon
		public const uint SHGFI_SHELLICONSIZE     	= 0x000000004;     // get shell size icon
		public const uint SHGFI_PIDL              	= 0x000000008;     // pszPath is a pidl
		public const uint SHGFI_USEFILEATTRIBUTES 	= 0x000000010;     // use passed dwFileAttribute
		public const uint SHGFI_ADDOVERLAYS       	= 0x000000020;     // apply the appropriate overlays
		public const uint SHGFI_OVERLAYINDEX      	= 0x000000040;     // Get the index of the overlay

		public const uint FILE_ATTRIBUTE_DIRECTORY  = 0x00000010;  
		public const uint FILE_ATTRIBUTE_NORMAL     = 0x00000080;

        [DllImport("Shell32.dll")]
        public static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbFileInfo,
            uint uFlags
            );
    }

	/// <summary>
	/// Wraps necessary functions imported from User32.dll. Code courtesy of MSDN Cold Rooster Consulting example.
	/// </summary>
	public class User32
	{
		/// <summary>
		/// Provides access to function required to delete handle. This method is used internally
		/// and is not required to be called separately.
		/// </summary>
		/// <param name="hIcon">Pointer to icon handle.</param>
		/// <returns>N/A</returns>
		[DllImport("User32.dll")]
		public static extern int DestroyIcon( IntPtr hIcon );
	}
}

