﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

// made by Dschahannam.
namespace Sibaui.Database.Entities
{
    public partial class Team
    {
        public Team()
        {
            Gangwars = new HashSet<Gangwar>();
            Garages = new HashSet<Garage>();
            PlayerTeamInfos = new HashSet<PlayerTeamInfo>();
            Vehicles = new HashSet<Vehicle>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Type { get; set; }
        public int ColorR { get; set; }
        public int ColorG { get; set; }
        public int ColorB { get; set; }
        public sbyte ColorBlip { get; set; }
        public float SpawnPositionX { get; set; }
        public float SpawnPositionY { get; set; }
        public float SpawnPositionZ { get; set; }
        public float StoragePositionX { get; set; }
        public float StoragePositionY { get; set; }
        public float StoragePositionZ { get; set; }
        public float ManagePositionX { get; set; }
        public float ManagePositionY { get; set; }
        public float ManagePositionZ { get; set; }

        public virtual TeamInfo TeamInfo { get; set; }
        public virtual ICollection<Gangwar> Gangwars { get; set; }
        public virtual ICollection<Garage> Garages { get; set; }
        public virtual ICollection<PlayerTeamInfo> PlayerTeamInfos { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}