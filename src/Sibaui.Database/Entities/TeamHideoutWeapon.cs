﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

// made by Dschahannam.
namespace Sibaui.Database.Entities
{
    public partial class TeamHideoutWeapon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? WeaponId { get; set; }
        public int Packets { get; set; }
        public int Price { get; set; }

        public virtual Weapon Weapon { get; set; }
    }
}