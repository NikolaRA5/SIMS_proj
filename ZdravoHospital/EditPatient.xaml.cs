﻿using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZdravoHospital
{
    /// <summary>
    /// Interaction logic for EditPatient.xaml
    /// </summary>
    public partial class EditPatient : Window, INotifyPropertyChanged
    {
        private string _name;
        private string _surname;
        private string _username;
        private string _password;
        private string _telephone;
        private string _email;
        private string _streetName;
        private string _streetNum;
        private DateTime _dateOfBirth;
        private string _personID;
        private string _country;
        private string _city;
        private int _postalCode;

        private string _healthCardNumber;   // sve osim ovoga u klasi person
        private string _parentsName;
        private MaritalStatus _maritalStatus;
        private Gender gender;
        private Credentials _credentials;

        private Patient _selectedPatient;
        private string _oldPassword;
        private PatientsView _parentPage;

        public static ObservableCollection<Patient> Patients { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public Patient SelectedPatient
        {
            get { return _selectedPatient; }
            set 
            { 
                _selectedPatient = value;
                OnPropertyChanged("SelectedPatient");
            }
        }

        public string PName
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("PName");
            }
        }
        public string Surname
        {
            get { return _surname; }
            set
            {
                _surname = value;
                OnPropertyChanged("Surname");
            }
        }
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged("Username");
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }
        public string Telephone
        {
            get => _telephone;
            set
            {
                _telephone = value;
                OnPropertyChanged("Telephone");
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }
        public string StreetName
        {
            get => _streetName;
            set
            {
                _streetName = value;
                OnPropertyChanged("StreetName");
            }
        }
        public string StreetNum
        {
            get => _streetNum;
            set
            {
                _streetNum = value;
                OnPropertyChanged("StreetNum");
            }
        }
        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged("DateOfBirth");
            }
        }
        public string PersonID
        {
            get => _personID;
            set
            {
                _personID = value;
                OnPropertyChanged("PersonID");
            }
        }
        public string Country
        {
            get => _country;
            set
            {
                _country = value;
                OnPropertyChanged("Country");
            }
        }
        public string City
        {
            get => _city;
            set
            {
                _city = value;
                OnPropertyChanged("City");
            }
        }
        public int PostalCode
        {
            get => _postalCode;
            set
            {
                _postalCode = value;
                OnPropertyChanged("PostalCode");
            }
        }

        public string HealthCardNumber
        {
            get => _healthCardNumber;
            set
            {
                _healthCardNumber = value;
                OnPropertyChanged("HealthCardNumber");
            }
        }
        public string ParentsName
        {
            get => _parentsName;
            set
            {
                _parentsName = value;
                OnPropertyChanged("ParentsName");
            }
        }
        public MaritalStatus PMaritalStatus
        {
            get => _maritalStatus;
            set
            {
                _maritalStatus = value;
                OnPropertyChanged("PMaritalStatus");
            }
        }
        public Gender PGender
        {
            get => gender;
            set
            {
                gender = value;
                OnPropertyChanged("PGender");
            }
        }

        public void initializeBindingFields()
        {
            PName = SelectedPatient.PName;
            Surname = SelectedPatient.Surname;
            Username = SelectedPatient.Username;
            Password = "";
            Dictionary<string, Credentials> accounts = new Dictionary<string, Credentials>();
            try
            {
                accounts = JsonConvert.DeserializeObject<Dictionary<string, Credentials>>(File.ReadAllText(@"..\..\..\Resources\accounts.json"));
                foreach (KeyValuePair<string, Credentials> item in accounts)
                {
                    if (item.Key.Equals(Username))
                    {
                        Password = item.Value.Password;
                        OldPassword = item.Value.Password;
                        break;
                    }
                }
            }
            catch(Exception e)
            {

            }
            Telephone = SelectedPatient.PhoneNumber;
            Email = SelectedPatient.Email;
            if(SelectedPatient.Address != null){
                StreetName = SelectedPatient.Address.StreetName;
                StreetNum = SelectedPatient.Address.StreetNum;
                if(SelectedPatient.Address.City != null)
                {
                    City = SelectedPatient.Address.City.PName;
                    PostalCode = SelectedPatient.Address.City.PostalCode;
                    if(SelectedPatient.Address.City.Country != null)
                    {
                        Country = SelectedPatient.Address.City.Country.PName;
                    }
                }
            }
            if(SelectedPatient.DateOfBirth != null)
                DateOfBirth = SelectedPatient.DateOfBirth;
            PersonID = SelectedPatient.PersonID;
            HealthCardNumber = SelectedPatient.HealthCardNumber;
            ParentsName = SelectedPatient.ParentsName;
            PMaritalStatus = SelectedPatient.MaritalStatus;
            PGender = SelectedPatient.Gender;
            cbGender.SelectedIndex = (int)SelectedPatient.Gender;
            cbMaritalStatus.SelectedIndex = (int)SelectedPatient.MaritalStatus;
    }

        public Credentials Credentials { get => _credentials; set => _credentials = value; }
        public string OldPassword { get => _oldPassword; set => _oldPassword = value; }
        public PatientsView ParentPage { get => _parentPage; set => _parentPage = value; }

        public EditPatient(Patient selectedPatient, PatientsView patientsView)
        {
            InitializeComponent();
            SelectedPatient = selectedPatient;
            initializeBindingFields();
            ParentPage = patientsView;
            this.DataContext = this;
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            Patient patient = new Patient(HealthCardNumber, PName, Surname, Email, DateOfBirth, Telephone, Username, ParentsName, (MaritalStatus)cbMaritalStatus.SelectedIndex, (Gender)cbGender.SelectedIndex, PersonID);

            patient.Address = new Adress(StreetName, StreetNum,
                new Model.City(PostalCode, this.City, new Model.Country(this.Country)));

            if (Password.Equals(""))
            {
                MessageBox.Show("Password is a required field.");
            }
            else
            {
                ////////////////////////EDITING THE ACCOUNT CREDENTIALS//////////////////////////////////
                if (!OldPassword.Equals(Password))
                {
                    Credentials = new Credentials(Username, Password, RoleType.PATIENT);
                    Dictionary<string, Credentials> accounts = new Dictionary<string, Credentials>();
                    accounts = JsonConvert.DeserializeObject<Dictionary<string, Credentials>>(File.ReadAllText(@"..\..\..\Resources\accounts.json"));
                    foreach (KeyValuePair<string, Credentials> item in accounts)
                    {
                        if (item.Key.Equals(Username))
                        {
                            item.Value.Password = Password;
                            break;
                        }
                    }
                    string accountsJson = JsonConvert.SerializeObject(accounts);
                    File.WriteAllText(@"..\..\..\Resources\accounts.json", accountsJson);
                }
               

                ////////////////////////EDITING THE PATIENT INFO////////////////////////////////////
                Dictionary<string, Patient> patientsForSerialization = new Dictionary<string, Patient>();

                if (File.Exists(@"..\..\..\Resources\patients.json"))
                {
                    patientsForSerialization = JsonConvert.DeserializeObject<Dictionary<string, Patient>>(File.ReadAllText(@"..\..\..\Resources\patients.json"));
                    foreach (KeyValuePair<string, Patient> item in patientsForSerialization)
                    {
                        if (item.Key.Equals(Username))
                        {
                            patientsForSerialization[item.Key] = patient;
                            break;
                        }
                    }
                    string patientsJson = JsonConvert.SerializeObject(patientsForSerialization);
                    File.WriteAllText(@"..\..\..\Resources\patients.json", patientsJson);
                    ParentPage.patientsDataGrid.ItemsSource = ParentPage.dictionaryToList(patientsForSerialization);
                    ParentPage.PatientsForTable = ParentPage.dictionaryToList(patientsForSerialization);
                    MessageBox.Show("Successfuly changed.");
                    this.Close();
                }
            }
        }
    }
}
