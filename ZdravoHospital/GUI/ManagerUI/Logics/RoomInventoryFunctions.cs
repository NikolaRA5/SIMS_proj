﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Model;
using ZdravoHospital.GUI.ManagerUI.DTOs;

namespace ZdravoHospital.GUI.ManagerUI.Logics
{
    public class RoomInventoryFunctions
    {
        public RoomInventoryFunctions() { }

        public RoomInventory FindRoomInventoryByRoomAndInventory(int roomId, string inventoryId)
        {
            foreach (RoomInventory ri in Model.Resources.roomInventory)
            {
                if (ri.RoomId == roomId && ri.InventoryId.Equals(inventoryId))
                    return ri;
            }

            return null;
        }

        public List<RoomInventory> FindAllRoomsWithInventory(string inventoryId)
        {
            List<RoomInventory> ret = new List<RoomInventory>();

            foreach (RoomInventory ri in Model.Resources.roomInventory)
                if (ri.InventoryId.Equals(inventoryId))
                    ret.Add(ri);

            return ret;
        }

        public List<RoomInventory> FindAllInventoryInRoom(int roomId)
        {
            List<RoomInventory> ret = new List<RoomInventory>();

            foreach (RoomInventory ri in Model.Resources.roomInventory)
                if (ri.RoomId == roomId)
                    ret.Add(ri);

            return ret;
        }

        public void DeleteByInventoryId(string iid)
        {
            Model.Resources.roomInventory.RemoveAll(ri => ri.InventoryId.Equals(iid));

            Model.Resources.SerializeRoomInventory();
        }

        public void DeleteByReference(RoomInventory ri)
        {
            Model.Resources.roomInventory.Remove(ri);
        }

        public void AddNewReference(RoomInventory ri)
        {
            Model.Resources.roomInventory.Add(ri);
        }
    }
}
