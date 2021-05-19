﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Model;
using Model.Repository;
using Newtonsoft.Json;
using ZdravoHospital.GUI.ManagerUI.Commands;
using ZdravoHospital.GUI.ManagerUI.Logics;
using ZdravoHospital.GUI.ManagerUI.ValidationRules;
using ZdravoHospital.GUI.ManagerUI.View;

namespace ZdravoHospital.GUI.ManagerUI.ViewModel
{
    class ManagerWindowViewModel : ViewModel
    {
        /*Singleton*/
        private static ManagerWindowViewModel dashboard;

        public static ManagerWindowViewModel GetDashboard()
        {
            return dashboard;
        }

        #region Mutex

        private static Mutex _roomMutex;
        private static Mutex _inventoryMutex;
        private static Mutex _transferMutex;

        #endregion

        #region Fields

        private string activeManager;
        private Visibility _roomTableVisibility;
        private Visibility _inventoryTableVisibility;
        private Visibility _medicineTableVisibility;

        private Room _selectedRoom;
        private Inventory _selectedInventory;
        private Medicine _selectedMedicine;

        private InventoryManagementDialogViewModel _inventoryManagementDialogViewModel;

        private Window dialog;

        private FilteringEventArgs _passedArgs;

        private RoomRepository _roomRepository;

        #endregion

        #region Observable collections
        public ObservableCollection<Room> Rooms { get; set; }
        public ObservableCollection<Inventory> Inventory { get; set; }
        public ObservableCollection<Medicine> Medicines { get; set; }

        #endregion

        #region Functions and services

        private Logics.TransferRequestsFunctions transferRequestFunctions;
        private Logics.RoomScheduleFunctions roomScheduleFunctions;

        #endregion

        #region Table visibility properties

        public Visibility RoomTableVisibility
        {
            get => _roomTableVisibility;
            set
            {
                _roomTableVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility InventoryTableVisibility
        {
            get => _inventoryTableVisibility;
            set
            {
                _inventoryTableVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility MedicineTableVisibility
        {
            get => _medicineTableVisibility;
            set
            {
                _medicineTableVisibility = value;
                OnPropertyChanged();
            }
        }

        #endregion
        
        #region Properties

        public string ActiveManager
        {
            get => activeManager;
            set
            {
                activeManager = value;
                OnPropertyChanged();
            }
        }

        public Room SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                OnPropertyChanged();
            }
        }

        public Inventory SelectedInventory
        {
            get => _selectedInventory;
            set
            {
                _selectedInventory = value;
                OnPropertyChanged();
            }
        }

        public Medicine SelectedMedicine
        {
            get => _selectedMedicine;
            set
            {
                _selectedMedicine = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public MyICommand ShowRoomCommand { get; set; }
        public MyICommand ShowInventoryCommand { get; set; }
        public MyICommand ShowMedicineCommand { get; set; }
        public MyICommand AddRoomCommand { get; set; }
        public MyICommand AddInventoryCommand { get; set; }
        public MyICommand AddMedicineCommand { get; set; }
        public MyICommand ManageInventoryCommand { get; set; }
        public MyICommand PlanRenovationCommand { get; set; }

        #endregion


        public ManagerWindowViewModel(string activeUser)
        {
            dashboard = this;
            //TODO: namesti da se ispisuje ko se loguje
            var currManager = "";
            ActiveManager = "Welcome, " + currManager;

            OpenDataBase();
            SetObservables();
            TurnOffTables();

            ShowRoomCommand = new MyICommand(OnShowRooms);
            ShowInventoryCommand = new MyICommand(OnShowInventory);
            ShowMedicineCommand = new MyICommand(OnShowMedicine);
            AddRoomCommand = new MyICommand(OnAddRoom);
            AddInventoryCommand = new MyICommand(OnAddInventory);
            AddMedicineCommand = new MyICommand(OnAddMedicine);
            ManageInventoryCommand = new MyICommand(OnManageInventory);
            PlanRenovationCommand = new MyICommand(OnPlanRenovation);

            _roomMutex = new Mutex();
            _inventoryMutex = new Mutex();
            _transferMutex = new Mutex();

            _inventoryManagementDialogViewModel = new InventoryManagementDialogViewModel();

            RunAllTasks();
        }

        #region Private functions

        private void OpenDataBase()
        {
            _roomRepository = new RoomRepository();

            var listInventory = JsonConvert.DeserializeObject<List<Inventory>>(File.ReadAllText(@"..\..\..\Resources\inventory.json"));
            Resources.inventory = new Dictionary<string, Inventory>();
            foreach (var inv in listInventory)
            {
                Resources.inventory[inv.Id] = inv;
            }
            
            Resources.OpenMedicines();
            Resources.OpenRoomSchedule();
            Resources.OpenTransferRequests();
            Resources.OpenPeriods();
            Resources.OpenMedicineRecensions();

            var doctorList = JsonConvert.DeserializeObject<List<Doctor>>(File.ReadAllText(@"..\..\..\Resources\doctors.json"));
            Resources.doctors = new Dictionary<string, Doctor>();
            foreach (var doc in doctorList)
            {
                Resources.doctors[doc.Username] = doc;
            }
        }

        private void SetObservables()
        {
            Rooms = new ObservableCollection<Room>(_roomRepository.GetValues());
            Inventory = new ObservableCollection<Inventory>(Resources.inventory.Values);
            Medicines = new ObservableCollection<Medicine>(Resources.medicines);
        }

        private void TurnOffTables()
        {
            RoomTableVisibility = Visibility.Hidden;
            InventoryTableVisibility = Visibility.Hidden;
            MedicineTableVisibility = Visibility.Hidden;
        }
        
        private void RunAllTasks()
        {
            var transferFunctions = new TransferRequestsFunctions();
            transferFunctions.RunOrExecute();

            var roomScheduleFunctions = new RoomScheduleFunctions();
            roomScheduleFunctions.RunOrExecute();
        }

        private bool InventoryFilter(object item)
        {
            var inventory = item as Inventory;

            if (inventory.Id.Contains(_passedArgs.Id) &&
                inventory.Name.Contains(_passedArgs.InventoryName) &&
                inventory.Supplier.Contains(_passedArgs.Supplier) &&
                inventory.Quantity <= _passedArgs.Quantity &&
                ((_passedArgs.Type.Equals("STATIC") && inventory.InventoryType == InventoryType.STATIC_INVENTORY) ||
                 (_passedArgs.Type.Equals("DYNAMIC") && inventory.InventoryType == InventoryType.DYNAMIC_INVENTORY) ||
                 (_passedArgs.Type.Equals("BOTH"))))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        #endregion

        #region Public functions

        public string FindVisibleDataGrid()
        {
            if (RoomTableVisibility == Visibility.Visible)
                return "roomTable";
            else if (InventoryTableVisibility == Visibility.Visible)
                return "inventoryTable";
            else if (MedicineTableVisibility == Visibility.Visible)
                return "medicineTable";
            else
                return "initialTable";
        }

        #endregion

        #region Button Functions

        //Show rooms button
        private void OnShowRooms()
        {
            TurnOffTables();
            RoomTableVisibility = Visibility.Visible;
        }

        //Show inventory button
        private void OnShowInventory()
        {
            TurnOffTables();
            InventoryTableVisibility = Visibility.Visible;
        }

        //Show medicine button
        private void OnShowMedicine()
        {
            TurnOffTables();
            MedicineTableVisibility = Visibility.Visible;
        }

        //Add room button
        private void OnAddRoom()
        {
            dialog = new AddOrEditRoomDialog(null);
            dialog.ShowDialog();
        }

        //Add inventory button
        private void OnAddInventory()
        {
            dialog = new AddOrEditInventoryDialog(null);
            dialog.ShowDialog();
        }

        //Add medicine button
        private void OnAddMedicine()
        {
            dialog = new AddOrEditMedicineDialog(null);
            dialog.ShowDialog();
        }

        //Manage inventory button
        private void OnManageInventory()
        {
            _inventoryManagementDialogViewModel.SenderRooms = new ObservableCollection<Room>(_roomRepository.GetValues());
            _inventoryManagementDialogViewModel.SenderRoom = null;
            _inventoryManagementDialogViewModel.ReceiverRoom = null;
            dialog = new InventoryManagementDialog(_inventoryManagementDialogViewModel);
            dialog.ShowDialog();
        }

        //Plan renovation button
        private void OnPlanRenovation()
        {
            dialog = new RenovationPlaningDialog();
            dialog.ShowDialog();
        }

        #endregion

        #region Complex Key Handling

        public void HandleEnterClick()
        {
            if (RoomTableVisibility == Visibility.Visible)
            {
                if (SelectedRoom != null)
                {
                    var room = _roomRepository.GetById(SelectedRoom.Id);
                    dialog = new AddOrEditRoomDialog(SelectedRoom);
                }
            }
            else if (InventoryTableVisibility == Visibility.Visible)
            {
                if (SelectedInventory != null)
                    dialog = new AddOrEditInventoryDialog(SelectedInventory);
            }
            else if (MedicineTableVisibility == Visibility.Visible && 
                     (SelectedMedicine.Status != MedicineStatus.PENDING &&
                      SelectedMedicine.Status != MedicineStatus.APPROVED))
            {
                if (SelectedMedicine != null)
                    dialog = new AddOrEditMedicineDialog(SelectedMedicine);
            }

            if (dialog != null)
                dialog.ShowDialog();
        }

        public void HandleDeleteClick()
        {
            if (RoomTableVisibility == Visibility.Visible)
            {
                if (SelectedRoom != null)
                {
                    var room = _roomRepository.GetById(SelectedRoom.Id);
                    dialog = new WarningDialog(SelectedRoom);
                }
            }
            else if (InventoryTableVisibility == Visibility.Visible)
            {
                if (SelectedInventory != null)
                    dialog = new WarningDialog(SelectedInventory);
                
            }
            else if (MedicineTableVisibility == Visibility.Visible)
            {
                if (SelectedMedicine != null)
                    dialog = new WarningDialog(SelectedMedicine);
            }

            if (dialog != null)
                dialog.ShowDialog();
        }

        public void HandleFClick()
        {
            if (InventoryTableVisibility == Visibility.Visible)
            {
                dialog = new InventoryFilteringDialog();
                dialog.ShowDialog();
            }
        }

        public void HandleAddClick()
        {
            if (InventoryTableVisibility == Visibility.Visible)
            {
                dialog = new InventoryAdderSubtractor(SelectedInventory);
                dialog.ShowDialog();
            }
        }

        public void HandleSClick()
        {
            if (MedicineTableVisibility == Visibility.Visible &&
                (SelectedMedicine.Status != MedicineStatus.PENDING &&
                 SelectedMedicine.Status != MedicineStatus.APPROVED))
            {
                dialog = new ValidationRequestDialog(SelectedMedicine);
                dialog.ShowDialog();
            }
        }

        public void HandleRClick()
        {
            if (MedicineTableVisibility == Visibility.Visible && SelectedMedicine.Status == MedicineStatus.REJECTED)
            {
                dialog = new RejectionNoteDialog(SelectedMedicine);
                dialog.ShowDialog();
            }
        }

        #endregion

        #region Events

        public void OnRoomsChanged(object sender, EventArgs e)
        {
            _roomMutex.WaitOne();

            Rooms = new ObservableCollection<Room>(_roomRepository.GetValues());
            OnPropertyChanged("Rooms");

            _roomMutex.ReleaseMutex();

            OnRefreshTransferDialog(sender, e);
        }

        public void OnInventoryChanged(object sender, EventArgs e)
        {
            _inventoryMutex.WaitOne();

            Inventory = new ObservableCollection<Inventory>(Resources.inventory.Values);
            OnPropertyChanged("Inventory");

            _inventoryMutex.ReleaseMutex();
        }

        public void OnMedicineChanged(object sender, EventArgs e)
        {
            Medicines = new ObservableCollection<Medicine>(Resources.medicines);
            OnPropertyChanged("Medicines");
        }

        public void OnRefreshTransferDialog(object sender, EventArgs e)
        {
            _transferMutex.WaitOne();
            _inventoryManagementDialogViewModel.OnShoudRefresh();
            _transferMutex.ReleaseMutex();
        }

        public void OnFilteringRequested(object sender, FilteringEventArgs e)
        {
            _inventoryMutex.WaitOne();
            var itemsVisual = CollectionViewSource.GetDefaultView(Inventory);

            _passedArgs = e;

            itemsVisual.Filter = InventoryFilter;
            _inventoryMutex.ReleaseMutex();
        }
        #endregion
    }
}
