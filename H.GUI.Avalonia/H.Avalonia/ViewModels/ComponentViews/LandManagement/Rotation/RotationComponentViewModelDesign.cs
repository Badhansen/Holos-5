using H.Core.Factories.Rotations;

namespace H.Avalonia.ViewModels.ComponentViews.LandManagement.Rotation;

public class RotationComponentViewModelDesign : RotationComponentViewModel
{
    public RotationComponentViewModelDesign()
    {
        base.SelectedRotationComponentDto = new RotationComponentDto();
        base.SelectedRotationComponentDto.Name = "A Rotation";
    }
}