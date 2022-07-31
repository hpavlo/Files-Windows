using Files.Resources.MultilingualResources;
using Files.Resources.Sounds;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = System.Windows.Media.Brush;
using Point = System.Windows.Point;

namespace Files.Views
{
    /// <summary>
    /// Window with custom chrome supporting theming of non-client areas
    /// </summary>
    [TemplatePart(Name = PART_IconPresenter, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_MinimizeButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_MaximizeRestoreButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]

    public class FilesWindow : Window
    {
        private const string PART_IconPresenter = "PART_IconPresenter";
        private const string PART_MinimizeButton = "PART_MinimizeButton";
        private const string PART_MaximizeRestoreButton = "PART_MaximizeRestoreButton";
        private const string PART_CloseButton = "PART_CloseButton";

        private static SoundPlayer playerAlwaysOnTopOn;
        private static SoundPlayer playerAlwaysOnTopOff;

        public FrameworkElement IconPresenter { get; protected set; }
        public Button MinimizeButton { get; protected set; }
        public Button MaximizeRestoreButton { get; protected set; }
        public Button CloseButton { get; protected set; }

        /// <summary>
        /// Gets or sets the visibility of the icon component of the window.
        /// </summary>
        public Visibility IconVisibility
        {
            get => (Visibility)GetValue(IconVisibilityProperty);
            set => SetValue(IconVisibilityProperty, value);
        }

        /// <summary>
        /// Gets or sets the window's icon as <see cref="ImageSource">ImageSource</see>.
        /// When the <see cref="Window.IconProperty">IconProperty</see> property changes, this is updated accordingly.
        /// </summary>
        protected internal ImageSource IconSource
        {
            get => (ImageSource)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        /// <summary>
        /// Gets the title bar actual height.
        /// </summary>
        public double TitleBarActualHeight
        {
            get => (double)GetValue(TitleBarActualHeightProperty);
            private set => SetValue(TitleBarActualHeightPropertyKey, value);
        }

        /// <summary>
        /// Gets or sets the content of the window's title bar
        /// between the title and the window buttons.
        /// </summary>
        public object TitleBarContent
        {
            get => GetValue(TitleBarContentProperty);
            set => SetValue(TitleBarContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground brush of the window's title bar.
        /// </summary>
        public Brush TitleBarForeground
        {
            get => (Brush)GetValue(TitleBarForegroundProperty);
            set => SetValue(TitleBarForegroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the background brush of the window's title bar.
        /// </summary>
        public Brush TitleBarBackground
        {
            get => (Brush)GetValue(TitleBarBackgroundProperty);
            set => SetValue(TitleBarBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the visibility of the title component of the window.
        /// </summary>
        public Visibility TitleVisibility
        {
            get => (Visibility)GetValue(TitleVisibilityProperty);
            set => SetValue(TitleVisibilityProperty, value);
        }

        /// <summary>
        /// Gets or sets the background brush of the minimize, maximize and restore
        /// buttons when they are hovered.
        /// </summary>
        public Brush WindowButtonHighlightBrush
        {
            get => (Brush)GetValue(WindowButtonHighlightBrushProperty);
            set => SetValue(WindowButtonHighlightBrushProperty, value);
        }

        /// <summary>
        /// Gets the size of the display overlapping area when the window is maximized.
        /// </summary>
        protected internal Thickness MaximizeBorderThickness
        {
            get => (Thickness)GetValue(MaximizeBorderThicknessProperty);
            private set => SetValue(MaximizeBorderThicknessPropertyKey, value);
        }

        /// <summary>
        /// Controls whether to shrink the title bar height a little when the window is maximized.
        /// The default is <see langword="true"/> as this is how native windows behave.
        /// </summary>
        public bool ShrinkTitleBarWhenMaximized
        {
            get => (bool)GetValue(ShrinkTitleBarWhenMaximizedProperty);
            set => SetValue(ShrinkTitleBarWhenMaximizedProperty, value);
        }

        /// <summary>
        /// Controls whether the title bar should be drawn over the window content instead of being stacked on top of it.
        /// </summary>
        public bool PlaceTitleBarOverContent
        {
            get => (bool)GetValue(PlaceTitleBarOverContentProperty);
            set => SetValue(PlaceTitleBarOverContentProperty, value);
        }

        public static readonly DependencyProperty IconVisibilityProperty = DependencyProperty.Register("IconVisibility", typeof(Visibility), typeof(FilesWindow), new PropertyMetadata(Visibility.Visible));

        protected internal static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(FilesWindow), new PropertyMetadata(null));

        protected internal static readonly DependencyPropertyKey TitleBarActualHeightPropertyKey = DependencyProperty.RegisterReadOnly("TitleBarActualHeight", typeof(double), typeof(FilesWindow), new PropertyMetadata(0.0d));

        protected internal static readonly DependencyProperty TitleBarActualHeightProperty = TitleBarActualHeightPropertyKey.DependencyProperty;

        public static readonly DependencyProperty TitleBarContentProperty = DependencyProperty.Register("TitleBarContent", typeof(object), typeof(FilesWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleBarForegroundProperty = DependencyProperty.Register("TitleBarForeground", typeof(Brush), typeof(FilesWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleBarBackgroundProperty = DependencyProperty.Register("TitleBarBackground", typeof(Brush), typeof(FilesWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleVisibilityProperty = DependencyProperty.Register("TitleVisibility", typeof(Visibility), typeof(FilesWindow), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty WindowButtonHighlightBrushProperty = DependencyProperty.Register("WindowButtonHighlightBrush", typeof(Brush), typeof(FilesWindow), new PropertyMetadata(null));

        protected internal static readonly DependencyPropertyKey MaximizeBorderThicknessPropertyKey = DependencyProperty.RegisterReadOnly("MaximizeBorderThickness", typeof(Thickness), typeof(FilesWindow), new PropertyMetadata(new Thickness()));

        protected internal static readonly DependencyProperty MaximizeBorderThicknessProperty = MaximizeBorderThicknessPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ShrinkTitleBarWhenMaximizedProperty = DependencyProperty.Register("ShrinkTitleBarWhenMaximized", typeof(bool), typeof(FilesWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty PlaceTitleBarOverContentProperty = DependencyProperty.Register("PlaceTitleBarOverContent", typeof(bool), typeof(FilesWindow), new PropertyMetadata(false));

        static FilesWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilesWindow), new FrameworkPropertyMetadata(typeof(FilesWindow)));
            IconProperty.OverrideMetadata(typeof(FilesWindow), new FrameworkPropertyMetadata(OnIconPropertyChanged));
        }

        private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FilesWindow sourceWindow))
                return;

            if (e.NewValue is ImageSource image)
            {
                sourceWindow.IconSource = image;
                return;
            }

            string newIcon = e.NewValue.ToString();

            sourceWindow.IconSource = String.IsNullOrEmpty(newIcon) ? null : new BitmapImage(new Uri(newIcon));
        }

        /// <inheritdoc/>
        public FilesWindow()
        {
            IconSource = GetApplicationIcon();
            MaximizeBorderThickness = GetSystemMaximizeBorderThickness();
        }

        private BitmapSource GetApplicationIcon()
        {
            string appFilePath = Process.GetCurrentProcess().MainModule.FileName;
            if (!File.Exists(appFilePath))
                return null;

            Icon appIcon = System.Drawing.Icon.ExtractAssociatedIcon(appFilePath);

            if (appIcon == null)
                return null;

            return Imaging.CreateBitmapSourceFromHIcon(appIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private Thickness GetSystemMaximizeBorderThickness()
        {
            Thickness frameThickness = SystemParameters.WindowNonClientFrameThickness;
            Thickness resizeBorderThickness = SystemParameters.WindowResizeBorderThickness;

            return new Thickness(
                frameThickness.Left + resizeBorderThickness.Left - 1,
                frameThickness.Top + resizeBorderThickness.Top - SystemParameters.CaptionHeight - 1,
                frameThickness.Right + resizeBorderThickness.Right - 1,
                frameThickness.Bottom + resizeBorderThickness.Bottom - 1);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            IconPresenter = GetTemplateChild(PART_IconPresenter) as FrameworkElement;
            MinimizeButton = GetTemplateChild(PART_MinimizeButton) as Button;
            MaximizeRestoreButton = GetTemplateChild(PART_MaximizeRestoreButton) as Button;
            CloseButton = GetTemplateChild(PART_CloseButton) as Button;

            if (IconPresenter != null)
                InitIconPresenter(IconPresenter);
            if (MinimizeButton != null)
                InitMinimizeButton(MinimizeButton);
            if (MaximizeRestoreButton != null)
                InitMaximizeRestoreButton(MaximizeRestoreButton);
            if (CloseButton != null)
                InitCloseButton(CloseButton);

            playerAlwaysOnTopOn = new SoundPlayer(ResourceSounds.AlwaysOnTopOn);
            playerAlwaysOnTopOff = new SoundPlayer(ResourceSounds.AlwaysOnTopOff);

            SystemInterop.AlwaysOnTopEnable = () =>
            {
                Topmost = true;
                playerAlwaysOnTopOn.Play();
            };
            SystemInterop.AlwaysOnTopDisable = () =>
            {
                Topmost = false;
                playerAlwaysOnTopOff.Play();
            };
            SystemInterop.Initialize(this);
        }

        /// <summary>
        /// Initializes functionality of the minimize button of the window's title bar.
        /// </summary>
        /// <param name="minimizeButton">The minimize button of the window</param>
        protected virtual void InitMinimizeButton(Button minimizeButton)
        {
            minimizeButton.Click += MinimizeClick;
        }

        /// <summary>
        /// Initializes functionality of the maximize/restore button of the window's title bar.
        /// </summary>
        /// <param name="maximizeRestoreButton">The maximize/restore button of the window</param>
        protected virtual void InitMaximizeRestoreButton(Button maximizeRestoreButton)
        {
            maximizeRestoreButton.Click += MaximizeRestoreClick;
        }

        /// <summary>
        /// Initializes functionality of the close button of the window's title bar.
        /// </summary>
        /// <param name="closeButton">The close button of the window</param>
        protected virtual void InitCloseButton(Button closeButton)
        {
            closeButton.Click += CloseClick;
        }

        /// <summary>
        /// Initializes functionality of the icon presenter component of the window's title bar.
        /// </summary>
        /// <param name="iconPresenter">The icon presenter component of the window</param>
        protected virtual void InitIconPresenter(FrameworkElement iconPresenter)
        {
            iconPresenter.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ClickCount == 2)
                {
                    Close();
                    return;
                }
            };

            iconPresenter.MouseLeftButtonDown += (s, e) =>
            {
                var menuPosition = IconPresenter.TranslatePoint(new Point(0, IconPresenter.ActualHeight), this);
                SystemInterop.OpenSystemContextMenu(PointToScreen(menuPosition));
            };

            iconPresenter.MouseRightButtonUp += (s, e) =>
            {
                var menuPosition = IconPresenter.TranslatePoint(new Point(0, IconPresenter.ActualHeight), this);
                SystemInterop.OpenSystemContextMenu(PointToScreen(menuPosition));
            };
        }

        /// <summary>
        /// Handles the close button's click event.
        /// </summary>
        protected virtual void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the maximize/restore button's click event.
        /// </summary>
        protected virtual void MaximizeRestoreClick(object sender, RoutedEventArgs e)
        {
            ToggleWindowState();
        }

        /// <summary>
        /// Handles the minimize button's click event.
        /// </summary>
        protected virtual void MinimizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Sets the <see cref="Window.WindowState"/> to <see cref="WindowState.Maximized"/>
        /// if it is currently at <see cref="WindowState.Normal"/> or else to <see cref="WindowState.Normal"/>.
        /// </summary>
        protected virtual void ToggleWindowState()
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }
    }

    /// <summary>
    /// Helper class for interactions with the system's native functions
    /// </summary>
    internal static class SystemInterop
    {
        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern IntPtr PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern bool InsertMenu(IntPtr hMenu, int wPosition, int wFlags, IntPtr wIDNewItem, string lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern bool ModifyMenu(IntPtr hMenu, int wPosition, int wFlags, IntPtr wIDItem, string lpItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);


        private const int MF_CHECKED = 0x8;
        private const int MF_SEPARATOR = 0x800;
        private const int MF_BYPOSITION = 0x400;

        private const uint TPM_LEFTALIGN = 0;
        private const uint TPM_RETURNCMD = 0x100;

        private const uint WM_SYSCOMMAND = 0x112;
        private const uint WM_KEYDOWN = 0x100;

        private const int VK_SHIFT = 0x10;
        private const int VK_CONTROL = 0x11;
        private const int VK_T = 0x54;

        private static IntPtr windowHandle;
        private static IntPtr systemMenu;

        public static Action AlwaysOnTopEnable;
        public static Action AlwaysOnTopDisable;

        private static string name = TranslationSource.Instance["SystemMenuAlwaysOnTop"] + "\tCtrl+Shift+T";
        private static int position = 6;
        private static IntPtr alwaysOnTopSystemMenuID = new IntPtr(1000);
        private static bool isChecked = false;

        public static void Initialize(Window window)
        {
            windowHandle = new WindowInteropHelper(window).Handle;
            systemMenu = GetSystemMenu(windowHandle, false);

            InsertMenu(systemMenu, position - 1, MF_SEPARATOR | MF_BYPOSITION, IntPtr.Zero, "");
            InsertMenu(systemMenu, position, MF_BYPOSITION, alwaysOnTopSystemMenuID, name);

            HwndSource.FromHwnd(windowHandle).AddHook(new HwndSourceHook(WndProc));
        }

        public static void OpenSystemContextMenu(Point screenCoordinate)
        {
            int track = TrackPopupMenuEx(
                systemMenu,
                TPM_LEFTALIGN | TPM_RETURNCMD,
                Convert.ToInt32(screenCoordinate.X),
                Convert.ToInt32(screenCoordinate.Y),
                windowHandle,
                IntPtr.Zero);

            if (track == 0)
                return;

            PostMessage(windowHandle, WM_SYSCOMMAND, new IntPtr(track), IntPtr.Zero);
        }

        private static void AlwaysOnTopChange()
        {
            if (isChecked)
            {
                AlwaysOnTopDisable();
                isChecked = false;
                ModifyMenu(systemMenu, position, MF_BYPOSITION, alwaysOnTopSystemMenuID, name);
            }
            else
            {
                AlwaysOnTopEnable();
                isChecked = true;
                ModifyMenu(systemMenu, position, MF_BYPOSITION | MF_CHECKED, alwaysOnTopSystemMenuID, name);
            }
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((uint)msg)
            {
                case WM_SYSCOMMAND:
                    if (wParam == alwaysOnTopSystemMenuID)
                        AlwaysOnTopChange();
                    break;

                case WM_KEYDOWN:
                    //Pressed Control+Shift+T
                    if (GetKeyState(VK_CONTROL) < 0 && GetKeyState(VK_SHIFT) < 0 && GetKeyState(VK_T) < 0)
                        AlwaysOnTopChange();
                    break;
            }

            return IntPtr.Zero;
        }
    }
}