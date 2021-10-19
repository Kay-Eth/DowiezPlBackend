﻿// <auto-generated />
using System;
using DowiezPlBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DowiezPlBackend.Migrations
{
    [DbContext(typeof(DowiezPlDbContext))]
    partial class DowiezPlDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.9");

            modelBuilder.Entity("DowiezPlBackend.Models.AppRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.AppUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<bool>("Banned")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.City", b =>
                {
                    b.Property<Guid>("CityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CityDistrict")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("CityName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)");

                    b.HasKey("CityId");

                    b.HasIndex("CityName", "CityDistrict")
                        .IsUnique();

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Connection", b =>
                {
                    b.Property<string>("ConnectionId")
                        .HasColumnType("varchar(255)");

                    b.Property<Guid?>("AppUserId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("Connected")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserAgent")
                        .HasColumnType("longtext");

                    b.HasKey("ConnectionId");

                    b.HasIndex("AppUserId");

                    b.ToTable("Connections");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Conversation", b =>
                {
                    b.Property<Guid>("ConversationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ConversationId");

                    b.ToTable("Conversations");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Demand", b =>
                {
                    b.Property<Guid>("DemandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("varchar(2000)");

                    b.Property<Guid>("DestinationCityId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("FromCityId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("LimitedToGroupId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("RecieverId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<Guid?>("TransportId")
                        .HasColumnType("char(36)");

                    b.HasKey("DemandId");

                    b.HasIndex("CreatorId");

                    b.HasIndex("DestinationCityId");

                    b.HasIndex("FromCityId");

                    b.HasIndex("LimitedToGroupId");

                    b.HasIndex("RecieverId");

                    b.HasIndex("TransportId");

                    b.ToTable("Demands");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Group", b =>
                {
                    b.Property<Guid>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ConversationId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("varchar(2000)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.HasKey("GroupId");

                    b.HasIndex("ConversationId")
                        .IsUnique();

                    b.HasIndex("CreatorId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Member", b =>
                {
                    b.Property<Guid>("MemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("MemberId");

                    b.HasIndex("GroupId");

                    b.HasIndex("UserId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Message", b =>
                {
                    b.Property<Guid>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<Guid>("ConversationId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("SenderId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("SentDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("MessageId");

                    b.HasIndex("ConversationId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Opinion", b =>
                {
                    b.Property<Guid>("OpinionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasMaxLength(5000)
                        .HasColumnType("varchar(5000)");

                    b.Property<Guid>("IssuerId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("RatedId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.HasKey("OpinionId");

                    b.HasIndex("RatedId");

                    b.HasIndex("IssuerId", "RatedId")
                        .IsUnique();

                    b.ToTable("Opinions");

                    b.HasCheckConstraint("CK_Issuer_cannot_rate_himself", "IssuerId <> RatedId");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Participant", b =>
                {
                    b.Property<Guid>("ParticipantId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ConversationId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("ParticipantId");

                    b.HasIndex("ConversationId");

                    b.HasIndex("UserId");

                    b.ToTable("Participants");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Report", b =>
                {
                    b.Property<Guid>("ReportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("varchar(5000)");

                    b.Property<Guid?>("OperatorId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ReportedId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ReporterId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("ReportId");

                    b.HasIndex("OperatorId");

                    b.HasIndex("ReportedId");

                    b.HasIndex("ReporterId");

                    b.ToTable("Reports");

                    b.HasCheckConstraint("CK_Reporter_cannot_rate_himself", "ReporterId <> ReportedId");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Transport", b =>
                {
                    b.Property<Guid>("TransportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("varchar(2000)");

                    b.Property<Guid>("EndsInCityId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("StartsInCityId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("TransportDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("TransportId");

                    b.HasIndex("CreatorId");

                    b.HasIndex("EndsInCityId");

                    b.HasIndex("StartsInCityId");

                    b.ToTable("Transports");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("char(36)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Connection", b =>
                {
                    b.HasOne("DowiezPlBackend.Models.AppUser", null)
                        .WithMany("Connections")
                        .HasForeignKey("AppUserId");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Demand", b =>
                {
                    b.HasOne("DowiezPlBackend.Models.AppUser", "Creator")
                        .WithMany("CreatedDemands")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DowiezPlBackend.Models.City", "Destination")
                        .WithMany()
                        .HasForeignKey("DestinationCityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DowiezPlBackend.Models.City", "From")
                        .WithMany()
                        .HasForeignKey("FromCityId");

                    b.HasOne("DowiezPlBackend.Models.Group", "LimitedTo")
                        .WithMany("LimitedBy")
                        .HasForeignKey("LimitedToGroupId");

                    b.HasOne("DowiezPlBackend.Models.AppUser", "Reciever")
                        .WithMany("RecievingDemands")
                        .HasForeignKey("RecieverId");

                    b.HasOne("DowiezPlBackend.Models.Transport", "Transport")
                        .WithMany("Demands")
                        .HasForeignKey("TransportId");

                    b.Navigation("Creator");

                    b.Navigation("Destination");

                    b.Navigation("From");

                    b.Navigation("LimitedTo");

                    b.Navigation("Reciever");

                    b.Navigation("Transport");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Group", b =>
                {
                    b.HasOne("DowiezPlBackend.Models.Conversation", "GroupConversation")
                        .WithOne("OwnerGroup")
                        .HasForeignKey("DowiezPlBackend.Models.Group", "ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DowiezPlBackend.Models.AppUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId");

                    b.Navigation("Creator");

                    b.Navigation("GroupConversation");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Member", b =>
                {
                    b.HasOne("DowiezPlBackend.Models.Group", "Group")
                        .WithMany("Members")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DowiezPlBackend.Models.AppUser", "User")
                        .WithMany("Memberships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Message", b =>
                {
                    b.HasOne("DowiezPlBackend.Models.Conversation", "Conversation")
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DowiezPlBackend.Models.AppUser", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId");

                    b.Navigation("Conversation");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Opinion", b =>
                {
                    b.HasOne("DowiezPlBackend.Models.AppUser", "Issuer")
                        .WithMany("IssuedOpinions")
                        .HasForeignKey("IssuerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DowiezPlBackend.Models.AppUser", "Rated")
                        .WithMany("OpinionsRecieved")
                        .HasForeignKey("RatedId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Issuer");

                    b.Navigation("Rated");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Participant", b =>
                {
                    b.HasOne("DowiezPlBackend.Models.Conversation", "Conversation")
                        .WithMany("Participants")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DowiezPlBackend.Models.AppUser", "User")
                        .WithMany("Participations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Conversation");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Report", b =>
                {
                    b.HasOne("DowiezPlBackend.Models.AppUser", "Operator")
                        .WithMany()
                        .HasForeignKey("OperatorId");

                    b.HasOne("DowiezPlBackend.Models.AppUser", "Reported")
                        .WithMany("ReferedReports")
                        .HasForeignKey("ReportedId");

                    b.HasOne("DowiezPlBackend.Models.AppUser", "Reporter")
                        .WithMany("ReportedReports")
                        .HasForeignKey("ReporterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Operator");

                    b.Navigation("Reported");

                    b.Navigation("Reporter");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Transport", b =>
                {
                    b.HasOne("DowiezPlBackend.Models.AppUser", "Creator")
                        .WithMany("PerformedTransports")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DowiezPlBackend.Models.City", "EndsIn")
                        .WithMany()
                        .HasForeignKey("EndsInCityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DowiezPlBackend.Models.City", "StartsIn")
                        .WithMany()
                        .HasForeignKey("StartsInCityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("EndsIn");

                    b.Navigation("StartsIn");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("DowiezPlBackend.Models.AppRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("DowiezPlBackend.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("DowiezPlBackend.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("DowiezPlBackend.Models.AppRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DowiezPlBackend.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("DowiezPlBackend.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DowiezPlBackend.Models.AppUser", b =>
                {
                    b.Navigation("Connections");

                    b.Navigation("CreatedDemands");

                    b.Navigation("IssuedOpinions");

                    b.Navigation("Memberships");

                    b.Navigation("OpinionsRecieved");

                    b.Navigation("Participations");

                    b.Navigation("PerformedTransports");

                    b.Navigation("RecievingDemands");

                    b.Navigation("ReferedReports");

                    b.Navigation("ReportedReports");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Conversation", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("OwnerGroup");

                    b.Navigation("Participants");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Group", b =>
                {
                    b.Navigation("LimitedBy");

                    b.Navigation("Members");
                });

            modelBuilder.Entity("DowiezPlBackend.Models.Transport", b =>
                {
                    b.Navigation("Demands");
                });
#pragma warning restore 612, 618
        }
    }
}
