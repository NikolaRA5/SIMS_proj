﻿using Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ZdravoHospital.GUI.ManagerUI.Logics
{
    public static class InventoryFunctions
    {
        public static bool DeleteInventory(Inventory someInventory)
        {
            RoomInventoryFunctions.DeleteByInventoryId(someInventory.Id);

            /* Visual */
            ManagerWindow.Inventory.Remove(someInventory);

            Model.Resources.inventory.Remove(someInventory.Id);
            Model.Resources.SerializeInventory();
            return true;
        }

        public static bool AddInventory(Inventory newInventory)
        {
            Room someRoom = RoomFunctions.FindRoomByPrio();

            if (someRoom == null)
            {
                return false;
            }
            /* Clean input */
            newInventory.Name = Regex.Replace(newInventory.Name, @"\s+", " ");
            newInventory.Name = newInventory.Name.Trim().ToLower();

            newInventory.Supplier = Regex.Replace(newInventory.Supplier, @"\s+", " ");
            newInventory.Supplier = newInventory.Supplier.Trim().ToLower();
            newInventory.Supplier = newInventory.Supplier.Substring(0, 1).ToUpper() + newInventory.Supplier.Substring(1);

            newInventory.Id = Regex.Replace(newInventory.Id, @"\s+", " ");
            newInventory.Id = newInventory.Id.Trim().ToUpper();

            /* Found a room to put some inventory in */
            Model.Resources.inventory[newInventory.Id] = newInventory;
            RoomInventoryFunctions.AddNewReference(new RoomInventory(newInventory.Id, someRoom.Id, newInventory.Quantity));

            Model.Resources.SerializeInventory();
            Model.Resources.SerializeRoomInventory();

            ManagerWindow.Inventory.Add(Model.Resources.inventory[newInventory.Id]);
            return true;
        }

        public static void EditInventory(Inventory oldInventory, Inventory newInventory)
        {
            int index = ManagerWindow.Inventory.IndexOf(oldInventory);
            ManagerWindow.Inventory.Remove(oldInventory);

            /* Clean input */
            newInventory.Name = Regex.Replace(newInventory.Name, @"\s+", " ");
            newInventory.Name = newInventory.Name.Trim().ToLower();

            newInventory.Supplier = Regex.Replace(newInventory.Supplier, @"\s+", " ");
            newInventory.Supplier = newInventory.Supplier.Trim().ToLower();
            newInventory.Supplier = newInventory.Supplier.Substring(0, 1).ToUpper() + newInventory.Supplier.Substring(1);

            newInventory.Id = Regex.Replace(newInventory.Id, @"\s+", " ");
            newInventory.Id = newInventory.Id.Trim().ToUpper();

            Model.Resources.inventory[oldInventory.Id] = newInventory;

            ManagerWindow.Inventory.Insert(index, newInventory);

            Model.Resources.SerializeInventory();
        }

        public static void EditInventoryAmount(Inventory inventory, int newQuantity, Room room)
        {
            int difference;

            RoomInventory roomInventory = RoomInventoryFunctions.FindRoomInventoryByRoomAndInventory(room.Id, inventory.Id);
            if (roomInventory == null)
            {
                RoomInventoryFunctions.AddNewReference(new RoomInventory(inventory.Id, room.Id, newQuantity));
                difference = newQuantity;
            }
            else
            {
                if (newQuantity == roomInventory.Quantity)
                    return;

                difference = newQuantity - roomInventory.Quantity;
                roomInventory.Quantity = newQuantity;

                if (newQuantity == 0)
                {
                    RoomInventoryFunctions.DeleteByReference(roomInventory);
                }
            }

            int index = ManagerWindow.Inventory.IndexOf(inventory);
            ManagerWindow.Inventory.Remove(inventory);

            if (difference < 0)
            {
                difference = (-1) * difference;
                Model.Resources.inventory[inventory.Id].Quantity -= difference;
                if (Model.Resources.inventory[inventory.Id].Quantity == 0)
                    Model.Resources.inventory.Remove(inventory.Id);
            }
            else
            {
                Model.Resources.inventory[inventory.Id].Quantity += difference;
            }
            
            ManagerWindow.Inventory.Insert(index, inventory);

            

            Model.Resources.SerializeRoomInventory();
            Model.Resources.SerializeInventory();
        }
    }
}
