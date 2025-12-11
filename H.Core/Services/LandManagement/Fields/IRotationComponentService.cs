using H.Core.Models;
using H.Core.Models.LandManagement.Rotation;

namespace H.Core.Services.LandManagement.Fields;

public interface IRotationComponentService
{
    void InitializeComponent(Farm farm, RotationComponent rotationComponent);
}