using System;
using System.Collections.ObjectModel;
using Avalonia.Rendering.Composition.Animations;
using H.Core.Enumerations;
using H.Core.Factories;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Services.Animals;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Prism.Regions;

namespace H.Avalonia.ViewModels.ComponentViews.OtherAnimals
{
    public abstract class OtherAnimalsViewModelBase : AnimalComponentViewModelBase
    {
        #region Fields

        private AnimalType _otherAnimalType;
        private ObservableCollection<AnimalGroup> _animalGroups;

        #endregion

        #region Constructors

        public OtherAnimalsViewModelBase()
        {
            this.Construct();
        }

        public OtherAnimalsViewModelBase(ILogger logger, IAnimalComponentService animalComponentService, IStorageService storageService, IManagementPeriodService managementPeriodService) : base(animalComponentService, logger, storageService, managementPeriodService)
        {
this.Construct();
        }

        #endregion

        #region Properties

        /// <summary>
        ///  The <see cref="AnimalType"/> a respective component represents, used in the <see cref="Groups"/> collection / Groups data grid in the view(s), value set in child classes.
        /// </summary>
        public AnimalType OtherAnimalType
        {
            get => _otherAnimalType;
            set => SetProperty(ref _otherAnimalType, value);
        }

        /// <summary>
        /// An Observable Collection that holds <see cref="AnimalGroup"/> objects, bound to a DataGrid in the view(s).
        /// </summary>
        public ObservableCollection<AnimalGroup> Groups
        {
            get => _animalGroups;
            set => SetProperty(ref _animalGroups, value);
        }

        #endregion

        #region Public Methods

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            AddExistingManagementPeriods();
        }

        /// <summary>
        /// Adds <see cref="AnimalComponentViewModelBase.ManagementPeriodDtos"/> that correspond to existing <see cref="ManagementPeriod"/> objects associated with the current <see cref="Farm"/>
        /// </summary>
        public void AddExistingManagementPeriods()
        {
            Farm currentFarm = StorageService.GetActiveFarm();
            var existingManagementPeriods = currentFarm.GetAllManagementPeriods();
            foreach(var managementPeriod in existingManagementPeriods)
            {
                var newManagementPeriodViewModel = new ManagementPeriodDto();
                newManagementPeriodViewModel.Name = managementPeriod.GroupName;
                newManagementPeriodViewModel.Start = managementPeriod.Start;
                newManagementPeriodViewModel.End = managementPeriod.End;
                newManagementPeriodViewModel.NumberOfDays = managementPeriod.NumberOfDays;
                ManagementPeriodDtos.Add(newManagementPeriodViewModel);
            }
        }

        /// <summary>
        ///  bound to a button in the view, adds an item to the <see cref="Groups"/> collection / a row to the respective bound DataGrid. Seeded with <see cref="OtherAnimalType"/>.
        /// </summary>
        public void HandleAddGroupEvent()
        {
            Groups.Add(new AnimalGroup { GroupType = OtherAnimalType });
        }

        #endregion

        #region Private Methods

        private void Construct()
        {
            ManagementPeriodDtos = new ObservableCollection<ManagementPeriodDto>();
            Groups = new ObservableCollection<AnimalGroup>();
        }

        #endregion
    }
}