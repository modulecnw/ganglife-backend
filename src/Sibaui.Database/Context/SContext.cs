﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sibaui.Database.Entities;

#nullable disable

// made by Dschahannam.
namespace Sibaui.Database.Context
{
    public partial class SContext : DbContext
    {
        public SContext()
        {
        }

        public SContext(DbContextOptions<SContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Gangwar> Gangwars { get; set; }
        public virtual DbSet<Garage> Garages { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<InventoryItem> InventoryItems { get; set; }
        public virtual DbSet<InventoryType> InventoryTypes { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<PlayerCharacter> PlayerCharacters { get; set; }
        public virtual DbSet<PlayerInventory> PlayerInventories { get; set; }
        public virtual DbSet<PlayerTeamInfo> PlayerTeamInfos { get; set; }
        public virtual DbSet<PlayerWeapon> PlayerWeapons { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<TeamHideoutWeapon> TeamHideoutWeapons { get; set; }
        public virtual DbSet<TeamInfo> TeamInfos { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<Weapon> Weapons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            modelBuilder.Entity<Gangwar>(entity =>
            {
                entity.ToTable("gangwars");

                entity.HasIndex(e => e.TeamId, "team_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.AttackPositionX).HasColumnName("attack_position_x");

                entity.Property(e => e.AttackPositionY).HasColumnName("attack_position_y");

                entity.Property(e => e.AttackPositionZ).HasColumnName("attack_position_z");

                entity.Property(e => e.Flag1PositionX).HasColumnName("flag1_position_x");

                entity.Property(e => e.Flag1PositionY).HasColumnName("flag1_position_y");

                entity.Property(e => e.Flag1PositionZ).HasColumnName("flag1_position_z");

                entity.Property(e => e.Flag2PositionX).HasColumnName("flag2_position_x");

                entity.Property(e => e.Flag2PositionY).HasColumnName("flag2_position_y");

                entity.Property(e => e.Flag2PositionZ).HasColumnName("flag2_position_z");

                entity.Property(e => e.Flag3PositionX).HasColumnName("flag3_position_x");

                entity.Property(e => e.Flag3PositionY).HasColumnName("flag3_position_y");

                entity.Property(e => e.Flag3PositionZ).HasColumnName("flag3_position_z");

                entity.Property(e => e.LastAttack)
                    .HasColumnType("datetime")
                    .HasColumnName("last_attack");

                entity.Property(e => e.MiddlePositionX).HasColumnName("middle_position_x");

                entity.Property(e => e.MiddlePositionY).HasColumnName("middle_position_y");

                entity.Property(e => e.MiddlePositionZ).HasColumnName("middle_position_z");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.RewardPositionX).HasColumnName("reward_position_x");

                entity.Property(e => e.RewardPositionY).HasColumnName("reward_position_y");

                entity.Property(e => e.RewardPositionZ).HasColumnName("reward_position_z");

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.TeamId)
                    .HasColumnType("int(11)")
                    .HasColumnName("team_id");

                entity.Property(e => e.Type)
                    .HasColumnType("int(11)")
                    .HasColumnName("type");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Gangwars)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gangwars_ibfk_1");
            });

            modelBuilder.Entity<Garage>(entity =>
            {
                entity.ToTable("garages");

                entity.HasIndex(e => e.TeamId, "team_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.PositionX).HasColumnName("position_x");

                entity.Property(e => e.PositionY).HasColumnName("position_y");

                entity.Property(e => e.PositionZ).HasColumnName("position_z");

                entity.Property(e => e.SpawnPositionHeading).HasColumnName("spawn_position_heading");

                entity.Property(e => e.SpawnPositionX).HasColumnName("spawn_position_x");

                entity.Property(e => e.SpawnPositionY).HasColumnName("spawn_position_y");

                entity.Property(e => e.SpawnPositionZ).HasColumnName("spawn_position_z");

                entity.Property(e => e.TeamId)
                    .HasColumnType("int(11)")
                    .HasColumnName("team_id")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Garages)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("garages_ibfk_1");
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.ToTable("inventories");

                entity.HasIndex(e => e.InventoryTypeId, "inventory_type_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.InventoryTypeId)
                    .HasColumnType("int(11)")
                    .HasColumnName("inventory_type_id");

                entity.Property(e => e.TargetId)
                    .HasColumnType("int(11)")
                    .HasColumnName("target_id");

                entity.HasOne(d => d.InventoryType)
                    .WithMany(p => p.Inventories)
                    .HasForeignKey(d => d.InventoryTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("inventories_ibfk_1");
            });

            modelBuilder.Entity<InventoryItem>(entity =>
            {
                entity.ToTable("inventory_items");

                entity.HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.HasIndex(e => e.InventoryId, "inventory_id");

                entity.HasIndex(e => e.ItemId, "item_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnType("int(11)")
                    .HasColumnName("amount");

                entity.Property(e => e.InventoryId)
                    .HasColumnType("int(11)")
                    .HasColumnName("inventory_id");

                entity.Property(e => e.ItemId)
                    .HasColumnType("int(11)")
                    .HasColumnName("item_id");

                entity.Property(e => e.Slot)
                    .HasColumnType("int(11)")
                    .HasColumnName("slot");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.InventoryItems)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("inventory_items_ibfk_1");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.InventoryItems)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("inventory_items_ibfk_2");
            });

            modelBuilder.Entity<InventoryType>(entity =>
            {
                entity.ToTable("inventory_types");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("display_name")
                    .HasDefaultValueSql("'Inventar'");

                entity.Property(e => e.MaxSlots)
                    .HasColumnType("int(11)")
                    .HasColumnName("max_slots");

                entity.Property(e => e.MaxWeight)
                    .HasColumnType("int(11)")
                    .HasColumnName("max_weight");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("items");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CustomDataText)
                    .HasMaxLength(44)
                    .HasColumnName("customDataText");

                entity.Property(e => e.MaxStack)
                    .HasColumnType("int(11)")
                    .HasColumnName("maxStack");

                entity.Property(e => e.Name)
                    .HasMaxLength(42)
                    .HasColumnName("name");

                entity.Property(e => e.Weight)
                    .HasColumnType("int(11)")
                    .HasColumnName("weight");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("players");

                entity.HasIndex(e => e.CharacterName, "character_name")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CharacterName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("character_name");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Injured).HasColumnName("injured");

                entity.Property(e => e.Money)
                    .HasColumnType("int(11)")
                    .HasColumnName("money")
                    .HasDefaultValueSql("'15000'");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.PositionX).HasColumnName("position_x");

                entity.Property(e => e.PositionY).HasColumnName("position_y");

                entity.Property(e => e.PositionZ).HasColumnName("position_z");

                entity.Property(e => e.Serial)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("serial");

                entity.Property(e => e.SocialclubName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("socialclub_name");
            });

            modelBuilder.Entity<PlayerCharacter>(entity =>
            {
                entity.ToTable("player_characters");

                entity.HasIndex(e => e.PlayerId, "player_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Customization)
                    .IsRequired()
                    .HasMaxLength(1200)
                    .HasColumnName("customization")
                    .HasComment("JSON a la Phill");

                entity.Property(e => e.PlayerId)
                    .HasColumnType("int(11)")
                    .HasColumnName("player_id");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerCharacters)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_characters_ibfk_1");
            });

            modelBuilder.Entity<PlayerInventory>(entity =>
            {
                entity.ToTable("player_inventories");

                entity.HasIndex(e => e.InventoryId, "inventory_id");

                entity.HasIndex(e => e.PlayerId, "player_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.InventoryId)
                    .HasColumnType("int(11)")
                    .HasColumnName("inventory_id");

                entity.Property(e => e.PlayerId)
                    .HasColumnType("int(11)")
                    .HasColumnName("player_id");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.PlayerInventories)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_inventories_ibfk_1");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerInventories)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_inventories_ibfk_2");
            });

            modelBuilder.Entity<PlayerTeamInfo>(entity =>
            {
                entity.ToTable("player_team_infos");

                entity.HasIndex(e => e.PlayerId, "player_id");

                entity.HasIndex(e => e.TeamId, "team_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.PlayerId)
                    .HasColumnType("int(11)")
                    .HasColumnName("player_id");

                entity.Property(e => e.Rank)
                    .HasColumnType("int(11)")
                    .HasColumnName("rank");

                entity.Property(e => e.Salary)
                    .HasColumnType("int(11)")
                    .HasColumnName("salary");

                entity.Property(e => e.TeamId)
                    .HasColumnType("int(11)")
                    .HasColumnName("team_id");

                entity.Property(e => e.WeaponPackets)
                    .HasColumnType("int(11)")
                    .HasColumnName("weapon_packets");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerTeamInfos)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_team_infos_ibfk_1");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.PlayerTeamInfos)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_team_infos_ibfk_2");
            });

            modelBuilder.Entity<PlayerWeapon>(entity =>
            {
                entity.ToTable("player_weapons");

                entity.HasIndex(e => e.PlayerId, "player_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Ammo)
                    .HasColumnType("int(11)")
                    .HasColumnName("ammo")
                    .HasDefaultValueSql("'9999'");

                entity.Property(e => e.PlayerId)
                    .HasColumnType("int(11)")
                    .HasColumnName("player_id");

                entity.Property(e => e.WeaponHash)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("weapon_hash");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerWeapons)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_weapons_ibfk_1");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("teams");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.ColorB)
                    .HasColumnType("int(11)")
                    .HasColumnName("color_b");

                entity.Property(e => e.ColorBlip)
                    .HasColumnType("tinyint(2)")
                    .HasColumnName("color_blip");

                entity.Property(e => e.ColorG)
                    .HasColumnType("int(11)")
                    .HasColumnName("color_g");

                entity.Property(e => e.ColorR)
                    .HasColumnType("int(11)")
                    .HasColumnName("color_r");

                entity.Property(e => e.ManagePositionX).HasColumnName("manage_position_x");

                entity.Property(e => e.ManagePositionY).HasColumnName("manage_position_y");

                entity.Property(e => e.ManagePositionZ).HasColumnName("manage_position_z");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasMaxLength(6)
                    .HasColumnName("short_name");

                entity.Property(e => e.SpawnPositionX).HasColumnName("spawn_position_x");

                entity.Property(e => e.SpawnPositionY).HasColumnName("spawn_position_y");

                entity.Property(e => e.SpawnPositionZ).HasColumnName("spawn_position_z");

                entity.Property(e => e.StoragePositionX).HasColumnName("storage_position_x");

                entity.Property(e => e.StoragePositionY).HasColumnName("storage_position_y");

                entity.Property(e => e.StoragePositionZ).HasColumnName("storage_position_z");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("enum('BAD','NEUTRAL','DEPARTMENT')")
                    .HasColumnName("type");
            });

            modelBuilder.Entity<TeamHideoutWeapon>(entity =>
            {
                entity.ToTable("team_hideout_weapons");

                entity.HasIndex(e => e.WeaponId, "weapon_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Packets)
                    .HasColumnType("int(11)")
                    .HasColumnName("packets");

                entity.Property(e => e.Price)
                    .HasColumnType("int(11)")
                    .HasColumnName("price");

                entity.Property(e => e.WeaponId)
                    .HasColumnType("int(11)")
                    .HasColumnName("weapon_id");

                entity.HasOne(d => d.Weapon)
                    .WithMany(p => p.TeamHideoutWeapons)
                    .HasForeignKey(d => d.WeaponId)
                    .HasConstraintName("team_hideout_weapons_ibfk_1");
            });

            modelBuilder.Entity<TeamInfo>(entity =>
            {
                entity.HasKey(e => e.TeamId)
                    .HasName("PRIMARY");

                entity.ToTable("team_infos");

                entity.HasIndex(e => e.Id, "id")
                    .IsUnique();

                entity.Property(e => e.TeamId)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever()
                    .HasColumnName("team_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.WeaponPackets)
                    .HasColumnType("int(11)")
                    .HasColumnName("weapon_packets");

                entity.HasOne(d => d.Team)
                    .WithOne(p => p.TeamInfo)
                    .HasForeignKey<TeamInfo>(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("team_infos_ibfk_1");
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.ToTable("vehicles");

                entity.HasIndex(e => e.GarageId, "garage_id");

                entity.HasIndex(e => e.OwnerId, "owner_id");

                entity.HasIndex(e => e.TeamId, "team_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.GarageId)
                    .HasColumnType("int(11)")
                    .HasColumnName("garage_id");

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("model");

                entity.Property(e => e.OwnerId)
                    .HasColumnType("int(11)")
                    .HasColumnName("owner_id");

                entity.Property(e => e.Parked).HasColumnName("parked");

                entity.Property(e => e.TeamId)
                    .HasColumnType("int(11)")
                    .HasColumnName("team_id")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.Garage)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.GarageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("vehicles_ibfk_1");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("vehicles_ibfk_2");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("vehicles_ibfk_3");
            });

            modelBuilder.Entity<Weapon>(entity =>
            {
                entity.ToTable("weapons");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.MaxAmmo)
                    .HasColumnType("int(11)")
                    .HasColumnName("max_ammo")
                    .HasDefaultValueSql("'9999'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.WeaponHash)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("weapon_hash");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}