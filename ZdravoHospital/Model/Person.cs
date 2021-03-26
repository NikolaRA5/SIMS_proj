// File:    Person.cs
// Author:  Nikola
// Created: Monday, March 22, 2021 8:32:15 AM
// Purpose: Definition of Class Person

using System;

namespace Model
{
   public class Person
   {
      public string PName { get; set; }
      public string Surname { get; set; }
      public string Email { get; set; }
      public DateTime DateOfBirth { get; set; }
      public string PhoneNumber { get; set; }
      public string Username { get; set; }
      public string ParentsName { get; set; }
      public MaritalStatus MaritalStatus { get; set; }
      public Gender Gender { get; set; }
      public Adress Address { get; set; }

        public Person(string name, string surname, string email, DateTime dateOfBirth, string phoneNumber, string username, string parentsName, MaritalStatus maritalStatus, Gender gender)
        {
            PName = name;
            Surname = surname;
            Email = email;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber;
            Username = username;
            ParentsName = parentsName;
            MaritalStatus = maritalStatus;
            Gender = gender;
        }

        public Person()
        {

        }
    }
}