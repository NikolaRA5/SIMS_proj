using System;

namespace Model
{
   public class Adress
   {
      public string streetName;
      public string number;
      
      public City city;
      
      
      public City City
      {
         get
         {
            return city;
         }
         set
         {
            if (this.city == null || !this.city.Equals(value))
            {
               if (this.city != null)
               {
                  City oldCity = this.city;
                  this.city = null;
                  oldCity.RemoveAdress(this);
               }
               if (value != null)
               {
                  this.city = value;
                  this.city.AddAdress(this);
               }
            }
         }
      }
   
   }
}