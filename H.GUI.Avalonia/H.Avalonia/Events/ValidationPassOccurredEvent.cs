using H.Core.Models;
using Prism.Events;

namespace H.Avalonia.Events;

public class ValidationPassOccurredEvent : PubSubEvent<ErrorInformation>
{

}