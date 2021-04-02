﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZdravoHospital.GUI.Secretary
{
    /// <summary>
    /// Interaction logic for SecretaryHomePage.xaml
    /// </summary>
    public partial class SecretaryHomePage : Page
    {
        public SecretaryHomePage()
        {
            InitializeComponent();
        }
        private void AddPatientButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientRegistrationPage());
        }

        private void SeePatientsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientsView());
        }

        private void AddGuestButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GuestAccountPage());
        }
    }
}
