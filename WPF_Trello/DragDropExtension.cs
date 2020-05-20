using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF_Trello.WpfExtensions
{
    public static class DragDropExtension
    {
        
        public static readonly DependencyProperty ScrollOnDragDropProperty =
            DependencyProperty.RegisterAttached("ScrollOnDragDrop",
                typeof(bool),
                typeof(DragDropExtension),
                new PropertyMetadata(false, HandleScrollOnDragDropChanged));
 
        public static bool GetScrollOnDragDrop(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(ScrollOnDragDropProperty);
        }

        public static void SetScrollOnDragDrop(DependencyObject element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(ScrollOnDragDropProperty, value);
        }

        private static void HandleScrollOnDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement container = d as FrameworkElement;

            if (d == null)
            {
                Debug.Fail("Invalid type!");
                return;
            }

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
            FrameworkElement container = sender as FrameworkElement;

            if (container == null)
            {
                return;
            }

            ScrollViewer verScrollViewer = GetFirstVisualChild<ScrollViewer>(container);
            ScrollViewer horScrollViewer = GetFirstVisualParent<ScrollViewer>(container);

            if (verScrollViewer == null && horScrollViewer == null)
            {
                return;
            }

            double tolerance = 60;
            double verticalPos = e.GetPosition(container).Y;
            double horizontalPos = e.GetPosition(horScrollViewer).X;
            double offset = 20;

            if (horizontalPos < tolerance)
            {
                horScrollViewer.ScrollToHorizontalOffset(horScrollViewer.HorizontalOffset - offset); //Scroll left. 
            }
            else if (horizontalPos > horScrollViewer.ActualWidth - tolerance)
            {
                horScrollViewer.ScrollToHorizontalOffset(horScrollViewer.HorizontalOffset + offset); //Scroll right.     
            }

            if (verticalPos < tolerance)
            {
                verScrollViewer.ScrollToVerticalOffset(verScrollViewer.VerticalOffset - offset); //Scroll up. 
            }
            else if (verticalPos > container.ActualHeight - tolerance)
            {
                verScrollViewer.ScrollToVerticalOffset(verScrollViewer.VerticalOffset + offset); //Scroll down.     
            }
        }

        private static void Unsubscribe(FrameworkElement container)
        {
            container.PreviewDragOver -= OnContainerPreviewDragOver;
        }

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
        public static T GetFirstVisualParent<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(depObj);
                if (parent != null && parent is T)
                {
                    return (T)parent;
                }

                T parentItem = GetFirstVisualParent<T>(parent);
                if (parentItem != null)
                {
                    return parentItem;
                }
            }

            return null;
        }
    }
}
