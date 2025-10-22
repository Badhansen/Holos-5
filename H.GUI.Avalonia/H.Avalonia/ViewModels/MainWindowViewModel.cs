using H.Avalonia.Services;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Events;
using H.Infrastructure;
using Tmds.DBus.Protocol;

namespace H.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private readonly IWindowNotificationManagerService _notificationManagerService;

        #endregion

        #region Properties

        public IWindowNotificationManagerService NotificationManagerService
        {
            get => _notificationManagerService;
        }

        #endregion

        #region Constructor

        public MainWindowViewModel(IWindowNotificationManagerService notificationManagerService) : base()
        {
            if (notificationManagerService != null)
            {
                _notificationManagerService = notificationManagerService;
            }
            else
            {
                throw new ArgumentNullException(nameof(notificationManagerService));
            }
        }

        #endregion
    }
}
