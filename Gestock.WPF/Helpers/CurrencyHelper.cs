namespace SuperMarcheApp.Helpers
{
    public static class CurrencyHelper
    {
        public static string Label(string text)
        {
            var symbol = App.Settings.CurrencySymbol;
            return $"{text} ({symbol})";
        }
    }
}