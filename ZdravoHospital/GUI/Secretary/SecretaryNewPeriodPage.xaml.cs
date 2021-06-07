﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Model;
using ZdravoHospital.GUI.Secretary.DTOs;
using ZdravoHospital.GUI.Secretary.Service;
using ZdravoHospital.GUI.Secretary.ViewModels;

namespace ZdravoHospital.GUI.Secretary
{
    /// <summary>
    /// Interaction logic for SecretaryNewPeriodPage.xaml
    /// </summary>
    public partial class SecretaryNewPeriodPage : Page
    {
        public PeriodsService PeriodsService { get; set; }
        public PeriodDTO PeriodDTO { get; set; }
        public ObservableCollection<Doctor> Doctors { get; set; }
        public ObservableCollection<Patient> Patients { get; set; }
        public ObservableCollection<Room> Rooms { get; set; }
        public Period PeriodDEMO { get; set; }

        


        public SecretaryNewPeriodPage(bool isDemoMode = false)
        {
            InitializeComponent();
            this.DataContext = this;
            PeriodsService = new PeriodsService();
            PeriodDTO = new PeriodDTO();
            initializeListsForBinding();
            setSearchFilters();
            PeriodDEMO = new Period(DateTime.Today, 30, "saki", "pantela", false);
            if (isDemoMode)
                ExecuteDemo();
        }

        

        private void initializeListsForBinding()
        {
            Doctors = new ObservableCollection<Doctor>(PeriodsService.GetDoctors());
            Patients = new ObservableCollection<Patient>(PeriodsService.GetPatients());
            Rooms = new ObservableCollection<Room>(PeriodsService.GetRooms());
        }

        private void setDoctorFilter()
        {
            ICollectionView viewDoctors = (ICollectionView)CollectionViewSource.GetDefaultView(Doctors);
            viewDoctors.Filter = DoctorsFilter;
        }
        private void setPatientFilter()
        {
            ICollectionView viewPatients = (ICollectionView)CollectionViewSource.GetDefaultView(Patients);
            viewPatients.Filter = PatientsFilter;
        }
        private void setRoomFilter()
        {
            ICollectionView viewRooms = (ICollectionView)CollectionViewSource.GetDefaultView(Rooms);
            viewRooms.Filter = RoomsFilter;
        }
        private void setSearchFilters()
        {
            setDoctorFilter();
            setPatientFilter();
            setRoomFilter();
        }

        private bool DoctorsFilter(object item)
        {
            if (String.IsNullOrEmpty(DoctorTextBox.Text))
                return true;
            else
                return ((item.ToString()).IndexOf(DoctorTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool PatientsFilter(object item)
        {
            if (String.IsNullOrEmpty(PatientTextBox.Text))
                return true;
            else
                return ((item.ToString()).IndexOf(PatientTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool RoomsFilter(object item)
        {
            if (String.IsNullOrEmpty(RoomTextBox.Text))
                return true;
            else
                return ((item.ToString()).IndexOf(RoomTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void RoomsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(CollectionViewSource.GetDefaultView(RoomsListBox.ItemsSource) != null)
            {
                CollectionViewSource.GetDefaultView(RoomsListBox.ItemsSource).Refresh();
            }
        }

        private void DoctorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CollectionViewSource.GetDefaultView(DoctorsListBox.ItemsSource) != null)
            {
                CollectionViewSource.GetDefaultView(DoctorsListBox.ItemsSource).Refresh();
            }

        }

        private void PatientsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(CollectionViewSource.GetDefaultView(PatientsListBox.ItemsSource) != null)
            {
                CollectionViewSource.GetDefaultView(PatientsListBox.ItemsSource).Refresh();
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(PeriodDTO.Doctor == null || PeriodDTO.Patient == null || PeriodDTO.Room == null)
            {
                SecretaryWindowVM.CustomMessageBox = new CustomMessageBox("Invalid request", "Please select preferred entities from lists above.");
                SecretaryWindowVM.CustomMessageBox.Owner = SecretaryWindowVM.SecretaryWindow;
                SecretaryWindowVM.CustomMessageBox.Show();
            }
            else
            {
                bool success = PeriodsService.ProcessPeriodCreation(PeriodDTO);
                if (success)
                    NavigationService.Navigate(new SecretaryPeriodsPage());
            }
            
        }

        public void ExecuteDemo()
        {
            Thread thread = new Thread(CallDemoMethods);
            thread.Start();
        }

        public void CallDemoMethods()
        {
            while (true)
            {
                toggleStopVisibility();
                clearFields();
                disableComponents();
                textBoxDemo(DoctorTextBox, "Marko");
                listBoxDemo(DoctorsListBox);
                textBoxDemo(PatientTextBox, "Satara");
                listBoxDemo(PatientsListBox);
                textBoxDemo(RoomTextBox, "appointment");
                listBoxDemo(RoomsListBox);
                comboboxDemo(PeriodTypeComboBox);
                datepickerDemo();
                textBoxDemo(TimeTextBox, "21:00");
                textBoxDemo(DurationTextBox, "45");
                buttonDemo();
                executeCountdown();
            }
        }

        private void clearFields()
        {
            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    PatientTextBox.Text = "";
                    DoctorTextBox.Text = "";
                    RoomTextBox.Text = "";
                    PeriodTypeComboBox.SelectedIndex = -1;
                    DateDatePicker.SelectedDate = null;
                    TimeTextBox.Text = "";
                    DurationTextBox.Text = "";

                }));
            }
            catch (Exception ex) { }
            
        }

        private void toggleStopVisibility()
        {
            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    StopDemoButton.Visibility = Visibility.Visible;
                    SecondsLeftTextBlock.Visibility = Visibility.Visible;
                }));
            }catch(Exception ex) { }
            
        }

        private void executeCountdown()
        {
            try
            {
                for (int i = 5; i >= 0; --i)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        SecondsLeftTextBlock.Text = i.ToString();
                    }));
                    Thread.Sleep(1000);
                }
            }catch(Exception ex) { }
        }

        private void textBoxDemo(TextBox textBox, string value)
        {
            try
            {
                for (int i = 1; i <= value.Length; i++)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        textBox.Text = value.Substring(0, i);
                    }));
                    Thread.Sleep(250);
                }
            }catch(Exception ex) { }
            
        }

        private void datepickerDemo()
        {
            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    DateDatePicker.IsEnabled = true;
                    DateDatePicker.IsDropDownOpen = true;
                    DateDatePicker.Text = PeriodDEMO.StartTime.Date.ToString();
                }));
                Thread.Sleep(600);
                this.Dispatcher.Invoke((Action)(() =>
                {
                    DateDatePicker.IsDropDownOpen = false;
                    DateDatePicker.IsEnabled = false;
                }));
            }catch(Exception ex) { }
        }
        private void comboboxDemo(ComboBox comboBox)
        {
            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    comboBox.IsDropDownOpen = true;
                }));
                Thread.Sleep(600);
                this.Dispatcher.Invoke((Action)(() =>
                {
                    comboBox.SelectedIndex = 0;
                    comboBox.IsDropDownOpen = false;
                }));
            }catch(Exception ex) { }
        }

        private void listBoxDemo(ListBox listBox)
        {
            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    listBox.SelectedIndex = 0;
                }));
            }catch(Exception ex) { }
        }

        private void buttonDemo()
        {
            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    SaveButton.IsEnabled = true;
                    SaveButton.Background = Brushes.GreenYellow;
                }));
                Thread.Sleep(450);
                this.Dispatcher.Invoke((Action)(() =>
                {
                    BrushConverter bc = new BrushConverter();
                    SaveButton.Background = (Brush)bc.ConvertFrom("#4267B2");
                    SaveButton.IsEnabled = false;
                }));
            }catch(Exception ex) { }
            
        }

        

        private void disableComponents()
        {
            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    PatientTextBox.IsReadOnly = true;
                    DoctorTextBox.IsReadOnly = true;
                    RoomTextBox.IsReadOnly = true;
                    PeriodTypeComboBox.IsEnabled = false;
                    DateDatePicker.IsEnabled = false;
                    TimeTextBox.IsReadOnly = true;
                    DurationTextBox.IsReadOnly = true;
                    SaveButton.IsEnabled = false;
                }));
            }catch(Exception ex) { }
            
        }

        private void StopDemoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DemoPage());
        }
    }
}
