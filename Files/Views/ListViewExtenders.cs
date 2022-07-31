using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Files.Views
{
    internal class ListViewExtenders : DependencyObject
    {
        #region AutoScrollTHorizontalSelectedProperty

        public static readonly DependencyProperty AutoScrollToHorizontalSelectedProperty =
            DependencyProperty.RegisterAttached("AutoScrollToHorizontalSelected", typeof(double), typeof(ListViewExtenders),
                new FrameworkPropertyMetadata(null, new CoerceValueCallback((s, v) =>
                {
                    var scroller = GetFirstVisualChild<ScrollViewer>((ListView)s);
                    if (scroller != null)
                        scroller.ScrollToHorizontalOffset((double)v);
                    return v;
                })));

        /// <summary>
        /// Sets the value of the AutoScrollToTopProperty
        /// </summary>
        /// <param name="obj">The dependency-object whichs value should be set</param>
        /// <param name="value">The value which should be assigned to the AutoScrollToTopProperty</param>
        public static void SetAutoScrollToHorizontalSelected(DependencyObject obj, double value)
        {
            obj.SetValue(AutoScrollToHorizontalSelectedProperty, value);
        }

        #endregion

        #region AutoScrollToVerticalSelectedProperty

        public static readonly DependencyProperty AutoScrollToVerticalSelectedProperty =
            DependencyProperty.RegisterAttached("AutoScrollToVerticalSelected", typeof(double), typeof(ListViewExtenders),
                new FrameworkPropertyMetadata(null, new CoerceValueCallback((s, v) =>
                {
                    var scroller = GetFirstVisualChild<ScrollViewer>((ListView)s);
                    if (scroller != null)
                        scroller.ScrollToVerticalOffset((double)v);
                    return v;
                })));

        /// <summary>
        /// Sets the value of the AutoScrollToTopProperty
        /// </summary>
        /// <param name="obj">The dependency-object whichs value should be set</param>
        /// <param name="value">The value which should be assigned to the AutoScrollToTopProperty</param>
        public static void SetAutoScrollToVerticalSelected(DependencyObject obj, double value)
        {
            obj.SetValue(AutoScrollToVerticalSelectedProperty, value);
        }

        #endregion

        #region AutoScrollToEndProperty

        public static readonly DependencyProperty AutoScrollToEndProperty =
            DependencyProperty.RegisterAttached("AutoScrollToEnd", typeof(bool), typeof(ListViewExtenders), 
                new UIPropertyMetadata(default(bool), OnAutoScrollToEndChanged));

        /// <summary>
        /// Returns the value of the AutoScrollToEndProperty
        /// </summary>
        /// <param name="obj">The dependency-object whichs value should be returned</param>
        /// <returns>The value of the given property</returns>
        public static bool GetAutoScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToEndProperty);
        }

        /// <summary>
        /// Sets the value of the AutoScrollToEndProperty
        /// </summary>
        /// <param name="obj">The dependency-object whichs value should be set</param>
        /// <param name="value">The value which should be assigned to the AutoScrollToEndProperty</param>
        public static void SetAutoScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        /// <summary>
        /// This method will be called when the AutoScrollToEnd
        /// property was changed
        /// </summary>
        /// <param name="s">The sender (the ListView)</param>
        /// <param name="e">Some additional information</param>
        public static void OnAutoScrollToEndChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var listView = s as ListView;
            if (listView != null)
            {
                var scrollViewer = GetFirstVisualChild<ScrollViewer>(listView);
                var listViewItems = listView.Items;
                var data = listViewItems.SourceCollection as INotifyCollectionChanged;

                var scrollToEndHandler = new NotifyCollectionChangedEventHandler((s1, e1) =>
                {
                    var scrollViewer = GetFirstVisualChild<ScrollViewer>(listView);
                    if (scrollViewer != null)
                        scrollViewer.ScrollToRightEnd();
                });

                if ((bool)e.NewValue)
                    data.CollectionChanged += scrollToEndHandler;
                else
                    data.CollectionChanged -= scrollToEndHandler;
            }
        }

        #endregion

        #region ScrollOnDragDropProperty

        public static readonly DependencyProperty ScrollOnDragDropProperty =
            DependencyProperty.RegisterAttached("ScrollOnDragDrop",
                typeof(bool),
                typeof(ListViewExtenders),
                new UIPropertyMetadata(default(bool), HandleScrollOnDragDropChanged));
 
        public static bool GetScrollOnDragDrop(DependencyObject element)
        {
            return (bool)element.GetValue(ScrollOnDragDropProperty);
        }

        public static void SetScrollOnDragDrop(DependencyObject element, bool value)
        {
            element.SetValue(ScrollOnDragDropProperty, value);
        }

        private static void HandleScrollOnDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement container = d as FrameworkElement;

            Unsubscribe(container);

            if (true.Equals(e.NewValue))
            {
                Subscribe(container);
            }
        }

        private static void Subscribe(FrameworkElement container)
        {
            container.PreviewDragOver += OnContainerPreviewDragOver;
        }

        private static void OnContainerPreviewDragOver(object sender, DragEventArgs e)
        {
            ScrollViewer scrollViewer = GetFirstVisualChild<ScrollViewer>(sender as FrameworkElement);

            double tolerance = 12;
            double horisontalPos = e.GetPosition(scrollViewer).X;
            double verticalPos = e.GetPosition(scrollViewer).Y;
            double offset = 1;

            if (horisontalPos > scrollViewer.ActualWidth - tolerance && verticalPos < tolerance) // Top RepeatButton? 
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset); //Scroll up. 
            }
            else if (horisontalPos > scrollViewer.ActualWidth - tolerance && verticalPos > scrollViewer.ActualHeight - tolerance) //Bottom RepeatButton? 
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset); //Scroll down.     
            }
        }

        private static void Unsubscribe(FrameworkElement container)
        {
            container.PreviewDragOver -= OnContainerPreviewDragOver;
        }

        #endregion

        public static T GetFirstVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = GetFirstVisualChild<T>(child);
                    if (childItem != null)
                    {
                        return childItem;
                    }
                }
            }
            return null;
        }
    }
}
