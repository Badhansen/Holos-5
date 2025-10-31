using System.ComponentModel;
using H.Core.Factories;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.Initialization;

public interface ICropInitializationService
{
    void InitializeFuelEnergy(Farm farm);
    void InitializeFuelEnergy(Farm farm, CropViewItem viewItem);
    void Initialize(Farm farm);
    void Initialize(CropViewItem viewItem, Farm farm);
}