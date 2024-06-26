﻿// <auto-generated />
using System;
using Auth.Api.Models.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Auth.Api.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Auth.Api.Models.Entities.Token", b =>
                {
                    b.Property<Guid>("RecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("IssuedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserCredentialId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("RecordId");

                    b.HasIndex("UserCredentialId");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("Auth.Api.Models.Entities.UserCredential", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailConfirmationCode")
                        .HasMaxLength(6)
                        .HasColumnType("nvarchar(6)");

                    b.Property<bool>("IsAccountLocked")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StoredCredentialIds")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserCredentials");
                });

            modelBuilder.Entity("Fido2NetLib.Development.StoredCredential", b =>
                {
                    b.Property<Guid>("AaGuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CredType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("DescriptorId")
                        .HasColumnType("varbinary(900)");

                    b.Property<byte[]>("PublicKey")
                        .HasColumnType("varbinary(max)");

                    b.Property<DateTime>("RegDate")
                        .HasColumnType("datetime2");

                    b.Property<long>("SignatureCounter")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("UserCredentialId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("UserHandle")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("UserId")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("AaGuid");

                    b.HasIndex("DescriptorId");

                    b.HasIndex("UserCredentialId");

                    b.ToTable("FidoCredentials");
                });

            modelBuilder.Entity("Fido2NetLib.Objects.PublicKeyCredentialDescriptor", b =>
                {
                    b.Property<byte[]>("Id")
                        .HasColumnType("varbinary(900)")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    b.Property<string>("Transports")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "transports");

                    b.Property<int?>("Type")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "type");

                    b.HasKey("Id");

                    b.ToTable("PublicKeyCredentialDescriptor");
                });

            modelBuilder.Entity("Auth.Api.Models.Entities.Token", b =>
                {
                    b.HasOne("Auth.Api.Models.Entities.UserCredential", null)
                        .WithMany("Tokens")
                        .HasForeignKey("UserCredentialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Fido2NetLib.Development.StoredCredential", b =>
                {
                    b.HasOne("Fido2NetLib.Objects.PublicKeyCredentialDescriptor", "Descriptor")
                        .WithMany()
                        .HasForeignKey("DescriptorId");

                    b.HasOne("Auth.Api.Models.Entities.UserCredential", null)
                        .WithMany("StoredCredentials")
                        .HasForeignKey("UserCredentialId");

                    b.Navigation("Descriptor");
                });

            modelBuilder.Entity("Auth.Api.Models.Entities.UserCredential", b =>
                {
                    b.Navigation("StoredCredentials");

                    b.Navigation("Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}
