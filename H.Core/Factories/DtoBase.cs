using DryIoc;
using H.Core.Helpers;
using Prism.Mvvm;

namespace H.Core.Factories;

/// <summary>
/// A base class to be used with any other classes that must validate user input. The properties in this class and subclasses are properties
/// that are bound to GUI controls and should therefore be validated before passing on the input values to the domain/business class objects.
/// </summary>
public abstract class DtoBase : ErrorValidationBase, IDto
{
    #region Fields


    #endregion

    #region Properties



    #endregion
}