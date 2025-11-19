using H.Core.Models;
using Prism.Events;

namespace H.Core.Events;

public class ValidationPassOccurredEvent : PubSubEvent<ErrorInformation>
{

}