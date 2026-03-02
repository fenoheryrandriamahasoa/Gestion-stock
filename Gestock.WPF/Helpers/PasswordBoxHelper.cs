using System.Windows;
using System.Windows.Controls;

namespace Gestock.WPF.Helpers
{
    public static class PasswordBoxHelper
    {
        // ── Attached Property : surveille si du texte est saisi ──
        public static readonly DependencyProperty MonitorProperty =
            DependencyProperty.RegisterAttached(
                "Monitor",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnMonitorChanged));

        public static void SetMonitor(DependencyObject obj, bool value)
            => obj.SetValue(MonitorProperty, value);

        public static bool GetMonitor(DependencyObject obj)
            => (bool)obj.GetValue(MonitorProperty);

        // ── Attached Property : indique si le champ a du contenu ──
        public static readonly DependencyProperty HasPasswordProperty =
            DependencyProperty.RegisterAttached(
                "HasPassword",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false));

        public static void SetHasPassword(DependencyObject obj, bool value)
            => obj.SetValue(HasPasswordProperty, value);

        public static bool GetHasPassword(DependencyObject obj)
            => (bool)obj.GetValue(HasPasswordProperty);

        // ── Quand Monitor passe à True, on écoute PasswordChanged ──
        private static void OnMonitorChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox pb)
            {
                if ((bool)e.NewValue)
                    pb.PasswordChanged += OnPasswordChanged;
                else
                    pb.PasswordChanged -= OnPasswordChanged;
            }
        }

        private static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
                SetHasPassword(pb, pb.Password.Length > 0);
        }
    }
}