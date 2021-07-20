﻿// <auto-generated />
using System;
using Ext_Dynamics_API.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ext_Dynamics_API.Migrations
{
    [DbContext(typeof(ExtensibleDbContext))]
    partial class ExtensibleDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.8")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Ext_Dynamics_API.Models.ApplicationUserAccount", b =>
                {
                    b.Property<int>("AppUserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AppUserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserPassword")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AppUserId");

                    b.HasAlternateKey("AppUserName");

                    b.ToTable("App_Users");
                });

            modelBuilder.Entity("Ext_Dynamics_API.Models.CanvasPersonalAccessTokens", b =>
                {
                    b.Property<int>("PatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccessToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("AppUserId")
                        .HasColumnType("int");

                    b.Property<int?>("ApplicationUserAccountAppUserId")
                        .HasColumnType("int");

                    b.Property<string>("TokenName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PatId");

                    b.HasIndex("ApplicationUserAccountAppUserId");

                    b.ToTable("Personal_Access_Tokens");
                });

            modelBuilder.Entity("Ext_Dynamics_API.Models.CanvasPersonalAccessTokens", b =>
                {
                    b.HasOne("Ext_Dynamics_API.Models.ApplicationUserAccount", null)
                        .WithMany("AccessTokens")
                        .HasForeignKey("ApplicationUserAccountAppUserId");
                });

            modelBuilder.Entity("Ext_Dynamics_API.Models.ApplicationUserAccount", b =>
                {
                    b.Navigation("AccessTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
