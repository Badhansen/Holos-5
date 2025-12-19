using Prism.Regions;

namespace H.Avalonia.ViewModels
{
    public class AboutPageViewModel : ViewModelBase
    {
        public AboutPageViewModel() { }

        public AboutPageViewModel(IRegionManager regionManager) : base(regionManager)
        {
        }
    }
}
