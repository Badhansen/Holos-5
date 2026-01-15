using H.Core.Factories;
using H.Core.Factories.Crops;
using H.Core.Models;
using H.Core.Models.LandManagement.Rotation;
using Microsoft.Extensions.Logging;

namespace H.Core.Services.LandManagement.Fields;

public class RotationComponentService : ComponentServiceBase, IRotationComponentService
{
    #region Fields

    private IFieldFactory _fieldFactory;
    private ICropFactory _cropFactory;

    #endregion

    #region Constructors

    public RotationComponentService(ILogger logger, IFieldFactory fieldFactory, ICropFactory cropFactory) : base(logger)
    {
        if (cropFactory != null)
        {
            _cropFactory = cropFactory; 
        }
        else
        {
            throw new ArgumentNullException(nameof(cropFactory));
        }

        if (fieldFactory != null)
        {
            _fieldFactory = fieldFactory; 
        }
        else
        {
            throw new ArgumentNullException(nameof(fieldFactory));
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Applies default initialization for a new <see cref="RotationComponent"/> being added to a <see cref="Farm"/>.
    /// Ensures a unique name and marks the component as initialized. No-ops if already initialized.
    /// </summary>
    /// <param name="farm">The target farm.</param>
    /// <param name="rotationComponent">The rotation component to initialize.</param>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="rotationComponent"/> is null.</exception>
    public void InitializeComponent(Farm farm, RotationComponent rotationComponent)
    {
        base.InitializeComponent(farm, rotationComponent);
    } 

    #endregion
}