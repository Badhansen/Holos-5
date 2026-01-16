using H.Core.Factories.Fields;
using H.Core.Factories.Rotations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.LandManagement.Rotation;

namespace H.Core.Services.LandManagement.Fields;

public interface IRotationComponentService
{
    void InitializeComponent(Farm farm, RotationComponent rotationComponent);
    IRotationComponentDto TransferToRotationComponentDto(RotationComponent template);
}