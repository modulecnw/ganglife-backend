﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

// made by Dschahannam.
namespace Sibaui.Database.Entities
{
    public partial class PlayerWeapon
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public uint WeaponHash { get; set; }
        public int Ammo { get; set; }

        public virtual Player Player { get; set; }
    }
}