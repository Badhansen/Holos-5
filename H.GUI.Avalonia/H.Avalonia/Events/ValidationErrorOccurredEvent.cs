using H.Core.Events;
using H.Core.Models;
using Prism.Events;

namespace H.Core.Events;

public class ValidationErrorOccurredEvent : PubSubEvent<ErrorInformation>
{

}