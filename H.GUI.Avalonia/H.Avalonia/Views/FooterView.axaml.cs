using Avalonia.Controls;
using H.Core.Enumerations;
using H.Core.Services;

namespace H.Avalonia.Views
{
    public partial class FooterView : UserControl
    {
        private string _footerImageTitleCa = "CanadaLogo";
        private string _footerImageTitleIE = "DefaultEULogo";

        private CountrySettings _countrySettrings = new CountrySettings();
        public FooterView()
        {
            InitializeComponent();
            InitializeFooterImage();

        }
        public void InitializeFooterImage()
        {   
            if (_countrySettrings.Version == CountryVersion.Ireland)
            {
                // found = false | footerImage = null
                var found = this.TryGetResource(_footerImageTitleIE, this.ActualThemeVariant, out var footerImage);
                this.DynamicImage.Source = (global::Avalonia.Media.IImage?)footerImage;
            }
            else
            {
                // found = false | footerImage = null
                var found = this.TryGetResource(_footerImageTitleCa, this.ActualThemeVariant, out var footerImage);
                this.DynamicImage.Source = (global::Avalonia.Media.IImage?)footerImage;
            }
        }
    }
}