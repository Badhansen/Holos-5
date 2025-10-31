using System.ComponentModel;

namespace H.Core.Factories;

public interface IDto : INotifyPropertyChanged
{
    string Name { get; set; }
    public Guid Guid { get; set; }
}