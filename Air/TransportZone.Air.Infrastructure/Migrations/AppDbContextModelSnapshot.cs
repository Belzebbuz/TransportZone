﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TransportZone.Air.Domain.Bookings;
using TransportZone.Air.Infrastructure.Persistence;

#nullable disable

namespace TransportZone.Air.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "postgis");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FlightTicket", b =>
                {
                    b.Property<int>("FlightsId")
                        .HasColumnType("integer");

                    b.Property<int>("TicketsId")
                        .HasColumnType("integer");

                    b.HasKey("FlightsId", "TicketsId");

                    b.HasIndex("TicketsId");

                    b.ToTable("FlightTicket");
                });

            modelBuilder.Entity("TransportZone.Air.Domain.Aircrafts.Aircraft", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Range")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Aircrafts");
                });

            modelBuilder.Entity("TransportZone.Air.Domain.Aircrafts.Seat", b =>
                {
                    b.Property<string>("AircraftId")
                        .HasColumnType("character varying(3)");

                    b.Property<string>("Id")
                        .HasMaxLength(4)
                        .HasColumnType("character varying(4)");

                    b.Property<int>("FareCondition")
                        .HasColumnType("integer");

                    b.HasKey("AircraftId", "Id");

                    b.ToTable("Seats");
                });

            modelBuilder.Entity("TransportZone.Air.Domain.Airports.Airport", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Point>("Coordinates")
                        .IsRequired()
                        .HasColumnType("geography (point)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Timezone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Airports");
                });

            modelBuilder.Entity("TransportZone.Air.Domain.BoardingPasses.BoardingPass", b =>
                {
                    b.Property<int>("FlightId")
                        .HasColumnType("integer");

                    b.Property<int>("TicketId")
                        .HasColumnType("integer");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.HasKey("FlightId", "TicketId");

                    b.HasIndex("TicketId");

                    b.HasIndex("FlightId", "Number")
                        .IsUnique();

                    b.ToTable("BoardingPass");
                });

            modelBuilder.Entity("TransportZone.Air.Domain.Bookings.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("BookDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("BookingState")
                        .HasColumnType("integer");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("TransportZone.Air.Domain.Bookings.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<int>("BookingId")
                        .HasColumnType("integer");

                    b.Property<ContactData>("ContactData")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("SeatAircraftId")
                        .IsRequired()
                        .HasColumnType("character varying(3)");

                    b.Property<string>("SeatId")
                        .IsRequired()
                        .HasColumnType("character varying(4)");

                    b.ComplexProperty<Dictionary<string, object>>("Passenger", "TransportZone.Air.Domain.Bookings.Ticket.Passenger#Passenger", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Id")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");
                        });

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("SeatAircraftId", "SeatId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("TransportZone.Air.Domain.Flights.Flight", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("ActualArrival")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ActualDeparture")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("AircraftId")
                        .IsRequired()
                        .HasColumnType("character varying(3)");

                    b.Property<string>("ArrivalAirportId")
                        .IsRequired()
                        .HasColumnType("character varying(3)");

                    b.Property<string>("DepartureAirportId")
                        .IsRequired()
                        .HasColumnType("character varying(3)");

                    b.Property<DateTime>("ScheduledArrival")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ScheduledDeparture")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AircraftId");

                    b.HasIndex("ArrivalAirportId");

                    b.HasIndex("DepartureAirportId");

                    b.ToTable("Flights");
                });

            modelBuilder.Entity("FlightTicket", b =>
                {
                    b.HasOne("TransportZone.Air.Domain.Flights.Flight", null)
                        .WithMany()
                        .HasForeignKey("FlightsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TransportZone.Air.Domain.Bookings.Ticket", null)
                        .WithMany()
                        .HasForeignKey("TicketsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TransportZone.Air.Domain.Aircrafts.Seat", b =>
                {
                    b.HasOne("TransportZone.Air.Domain.Aircrafts.Aircraft", "Aircraft")
                        .WithMany("Seats")
                        .HasForeignKey("AircraftId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Aircraft");
                });

            modelBuilder.Entity("TransportZone.Air.Domain.BoardingPasses.BoardingPass", b =>
                {
                    b.HasOne("TransportZone.Air.Domain.Flights.Flight", "Flight")
                        .WithMany()
                        .HasForeignKey("FlightId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TransportZone.Air.Domain.Bookings.Ticket", "Ticket")
                        .WithMany()
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Flight");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("TransportZone.Air.Domain.Bookings.Ticket", b =>
                {
                    b.HasOne("TransportZone.Air.Domain.Bookings.Booking", "Booking")
                        .WithMany("Tickets")
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TransportZone.Air.Domain.Aircrafts.Seat", "Seat")
                        .WithMany()
                        .HasForeignKey("SeatAircraftId", "SeatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");

                    b.Navigation("Seat");
                });

            modelBuilder.Entity("TransportZone.Air.Domain.Flights.Flight", b =>
                {
                    b.HasOne("TransportZone.Air.Domain.Aircrafts.Aircraft", "Aircraft")
                        .WithMany()
                        .HasForeignKey("AircraftId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TransportZone.Air.Domain.Airports.Airport", "ArrivalAirport")
                        .WithMany()
                        .HasForeignKey("ArrivalAirportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TransportZone.Air.Domain.Airports.Airport", "DepartureAirport")
                        .WithMany()
                        .HasForeignKey("DepartureAirportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Aircraft");

                    b.Navigation("ArrivalAirport");

                    b.Navigation("DepartureAirport");
                });

            modelBuilder.Entity("TransportZone.Air.Domain.Aircrafts.Aircraft", b =>
                {
                    b.Navigation("Seats");
                });

            modelBuilder.Entity("TransportZone.Air.Domain.Bookings.Booking", b =>
                {
                    b.Navigation("Tickets");
                });
#pragma warning restore 612, 618
        }
    }
}
