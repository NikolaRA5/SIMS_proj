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
using System.Windows.Shapes;

namespace ZdravoHospital.GUI.PatientUI
{
    /// <summary>
    /// Interaction logic for PatientWindow.xaml
    /// </summary>
    public partial class PatientWindow : Window
    {
        public PatientWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

      


        private void logOutButton_Click(object sender, RoutedEventArgs e)
        {
            LogOutDialog logOutDialog = new LogOutDialog(this);
            logOutDialog.Show();
        }

        private void addAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            myFrame.Navigate(new AddAppointmentPage());
        }

        private void appointmentsButton_Click(object sender, RoutedEventArgs e)
        {
            myFrame.Navigate(new AppointmentPage());
        }
    }
}
