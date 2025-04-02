﻿// <auto-generated />
using System;
using CoriCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CoriCore.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250401100031_PRMRequiredFieldsFix")]
    partial class PRMRequiredFieldsFix
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CoriCore.Models.Admin", b =>
                {
                    b.Property<int>("AdminId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AdminId"));

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("AdminId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("CoriCore.Models.Employee", b =>
                {
                    b.Property<int>("EmployeeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EmployeeId"));

                    b.Property<DateOnly>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("EmployDate")
                        .HasColumnType("date");

                    b.Property<int>("EmployType")
                        .HasColumnType("integer");

                    b.Property<int>("Gender")
                        .HasColumnType("integer");

                    b.Property<bool>("IsSuspended")
                        .HasColumnType("boolean");

                    b.Property<string>("JobTitle")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly?>("LastPaidDate")
                        .HasColumnType("date");

                    b.Property<int>("PayCycle")
                        .HasColumnType("integer");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("SalaryAmount")
                        .HasColumnType("numeric");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("EmployeeId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("CoriCore.Models.Equipment", b =>
                {
                    b.Property<int>("EquipmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EquipmentId"));

                    b.Property<DateTime>("AssignedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Condition")
                        .HasColumnType("integer");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer");

                    b.Property<int>("EquipmentCatId")
                        .HasColumnType("integer");

                    b.HasKey("EquipmentId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("EquipmentCatId");

                    b.ToTable("Equipments");
                });

            modelBuilder.Entity("CoriCore.Models.EquipmentCategory", b =>
                {
                    b.Property<int>("EquipmentCatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EquipmentCatId"));

                    b.Property<string>("EquipmentCatName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("EquipmentCatId");

                    b.ToTable("EquipmentCategories");
                });

            modelBuilder.Entity("CoriCore.Models.LeaveBalance", b =>
                {
                    b.Property<int>("LeaveBalanceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("LeaveBalanceId"));

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer");

                    b.Property<int>("LeaveTypeId")
                        .HasColumnType("integer");

                    b.Property<int>("RemainingDays")
                        .HasColumnType("integer");

                    b.HasKey("LeaveBalanceId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("LeaveTypeId");

                    b.ToTable("LeaveBalances");
                });

            modelBuilder.Entity("CoriCore.Models.LeaveRequest", b =>
                {
                    b.Property<int>("LeaveRequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("LeaveRequestId"));

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<int>("LeaveTypeId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("LeaveRequestId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("LeaveTypeId");

                    b.ToTable("LeaveRequests");
                });

            modelBuilder.Entity("CoriCore.Models.LeaveType", b =>
                {
                    b.Property<int>("LeaveTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("LeaveTypeId"));

                    b.Property<int>("DefaultDays")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("LeaveTypeName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LeaveTypeId");

                    b.ToTable("LeaveTypes");
                });

            modelBuilder.Entity("CoriCore.Models.PerformanceReview", b =>
                {
                    b.Property<int>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ReviewId"));

                    b.Property<int>("AdminId")
                        .HasColumnType("integer");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<string>("DocUrl")
                        .HasColumnType("text");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("boolean");

                    b.Property<string>("MeetLink")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("MeetLocation")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int?>("Rating")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.HasKey("ReviewId");

                    b.HasIndex("AdminId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("PerformanceReviews");
                });

            modelBuilder.Entity("CoriCore.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("GoogleId")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CoriCore.Models.Admin", b =>
                {
                    b.HasOne("CoriCore.Models.User", "User")
                        .WithOne("Admin")
                        .HasForeignKey("CoriCore.Models.Admin", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CoriCore.Models.Employee", b =>
                {
                    b.HasOne("CoriCore.Models.User", "User")
                        .WithOne("Employee")
                        .HasForeignKey("CoriCore.Models.Employee", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CoriCore.Models.Equipment", b =>
                {
                    b.HasOne("CoriCore.Models.Employee", "Employee")
                        .WithMany("Equipment")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CoriCore.Models.EquipmentCategory", "EquipmentCategory")
                        .WithMany("Equipment")
                        .HasForeignKey("EquipmentCatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("EquipmentCategory");
                });

            modelBuilder.Entity("CoriCore.Models.LeaveBalance", b =>
                {
                    b.HasOne("CoriCore.Models.Employee", "Employee")
                        .WithMany("LeaveBalances")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CoriCore.Models.LeaveType", "LeaveType")
                        .WithMany("LeaveBalances")
                        .HasForeignKey("LeaveTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("LeaveType");
                });

            modelBuilder.Entity("CoriCore.Models.LeaveRequest", b =>
                {
                    b.HasOne("CoriCore.Models.Employee", "Employee")
                        .WithMany("LeaveRequests")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CoriCore.Models.LeaveType", "LeaveType")
                        .WithMany("LeaveRequests")
                        .HasForeignKey("LeaveTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("LeaveType");
                });

            modelBuilder.Entity("CoriCore.Models.PerformanceReview", b =>
                {
                    b.HasOne("CoriCore.Models.Admin", "Admin")
                        .WithMany("PerformanceReviews")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CoriCore.Models.Employee", "Employee")
                        .WithMany("PerformanceReviews")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Admin");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("CoriCore.Models.Admin", b =>
                {
                    b.Navigation("PerformanceReviews");
                });

            modelBuilder.Entity("CoriCore.Models.Employee", b =>
                {
                    b.Navigation("Equipment");

                    b.Navigation("LeaveBalances");

                    b.Navigation("LeaveRequests");

                    b.Navigation("PerformanceReviews");
                });

            modelBuilder.Entity("CoriCore.Models.EquipmentCategory", b =>
                {
                    b.Navigation("Equipment");
                });

            modelBuilder.Entity("CoriCore.Models.LeaveType", b =>
                {
                    b.Navigation("LeaveBalances");

                    b.Navigation("LeaveRequests");
                });

            modelBuilder.Entity("CoriCore.Models.User", b =>
                {
                    b.Navigation("Admin");

                    b.Navigation("Employee");
                });
#pragma warning restore 612, 618
        }
    }
}
