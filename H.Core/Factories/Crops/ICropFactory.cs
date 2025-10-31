using H.Core.Models.LandManagement.Fields;

namespace H.Core.Factories.Crops;

public interface ICropFactory : IFactory<CropDto>
{
    #region Public Methods

    /// <summary>
    /// Create a new instance that is based on the state of an existing <see cref="CropViewItem"/>. This method is used to create a
    /// new instance of a <see cref="CropDto"/> that will be bound to a view.
    /// </summary>
    /// <param name="template">The <see cref="CropViewItem"/> that will be used to provide default values for the new <see cref="CropDto"/> instance</param>
    CropDto CreateCropDto(CropViewItem template);

    CropViewItem CreateCropViewItem(ICropDto cropDto);

    #endregion
}