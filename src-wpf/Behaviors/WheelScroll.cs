using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ValorantUnlocker.Behaviors
{
    /// <summary>
    /// Cho phép cuộn ScrollViewer bằng con lăn chuột. Cần thiết vì NavigationView của WPF-UI
    /// (và một số control con) "nuốt" sự kiện lăn chuột trước khi tới ScrollViewer — khiến chỉ
    /// kéo thanh cuộn mới cuộn được. Bắt ở PreviewMouseWheel (tunnel) để chặn trước rồi tự cuộn.
    /// Dùng: &lt;ScrollViewer b:WheelScroll.Enabled="True" /&gt;
    /// </summary>
    public static class WheelScroll
    {
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.RegisterAttached(
                "Enabled", typeof(bool), typeof(WheelScroll),
                new PropertyMetadata(false, OnEnabledChanged));

        public static bool GetEnabled(DependencyObject o) => (bool)o.GetValue(EnabledProperty);
        public static void SetEnabled(DependencyObject o, bool v) => o.SetValue(EnabledProperty, v);

        private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ScrollViewer sv) return;
            // handledEventsToo: true — BẮT BUỘC. NavigationView của WPF-UI đánh dấu PreviewMouseWheel là Handled
            // khi tunnel xuống, nên handler thường (+=) bị bỏ qua. AddHandler với true nhận cả event đã Handled.
            if ((bool)e.NewValue)
                sv.AddHandler(System.Windows.UIElement.PreviewMouseWheelEvent, new MouseWheelEventHandler(OnPreviewMouseWheel), true);
            else
                sv.RemoveHandler(System.Windows.UIElement.PreviewMouseWheelEvent, new MouseWheelEventHandler(OnPreviewMouseWheel));
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is not ScrollViewer sv) return;
            if (e.Handled || !CanScroll(sv, e.Delta)) return;

            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private static bool CanScroll(ScrollViewer sv, int delta)
        {
            if (sv.ScrollableHeight <= 0) return false;
            return delta < 0
                ? sv.VerticalOffset < sv.ScrollableHeight
                : sv.VerticalOffset > 0;
        }
    }
}
