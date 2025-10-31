using System.Collections.ObjectModel;
using Prism.Regions;

namespace H.Avalonia.Test.TestHelpers;

public class TestViewsCollection : ObservableCollection<object>, IViewsCollection
{
    // IViewsCollection only adds Contains(object), which ObservableCollection already implements.
    // No additional implementation is needed.
}