using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.Infrastructure.EntityConfigurations.Sqlite
{
    internal class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");

            builder.HasKey(o => o.Id);

            builder.Ignore(o => o.DomainEvents);
            builder.Ignore(o => o.OrderStatus);
            
            builder.Property(o => o.Id).ValueGeneratedOnAdd();

            builder.OwnsOne(o => o.Address);

            builder.Property<DateTime>("_orderDate")
                .HasColumnName("OrderDate")
                .IsRequired();
            
            //builder.Property<int>("OrderStatusId").IsRequired();
                        
            builder.Property<int?>("BuyerId").IsRequired(false);
            builder.Property<int?>("PaymentMethodId").IsRequired(false);
            builder.Property<string>("Description").IsRequired(false);

            var navigation = builder.Metadata.FindNavigation(nameof(Order.OrderItems));
            
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne<PaymentMethod>()
                .WithMany()
                .HasForeignKey("PaymentMethodId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Buyer>()
                .WithMany()
                .IsRequired()
                .HasForeignKey("BuyerId");

            builder.HasOne(o => o.OrderStatus).WithMany().HasForeignKey("OrderStatusId");
            //builder.HasOne<OrderStatus>().WithMany().HasForeignKey("OrderStatusId");

        }
    }

    internal class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("orderItems");

            builder.HasKey(o => o.Id);

            builder.Ignore(b => b.DomainEvents);

            builder.Property(o => o.Id).ValueGeneratedOnAdd();

            builder.Property<int>("OrderId").IsRequired();

            builder.Property<decimal>("Discount").IsRequired();

            builder.Property<int>("ProductId").IsRequired();

            builder.Property<string>("ProductName").IsRequired();

            builder.Property<decimal>("UnitPrice").IsRequired();

            builder.Property<int>("Units").IsRequired();

            builder.Property<string>("PictureUrl").IsRequired(false);
        }
    }

    internal class OrderStatusEntityTypeConfiguration : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            builder.ToTable("orderstatus");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
              .HasDefaultValue(1)
              .ValueGeneratedNever()
              .IsRequired();

            builder.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }

    internal class PaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("paymentmethods");

            builder.HasKey(b => b.Id);

            builder.Ignore(b => b.DomainEvents);

            builder.Property(o => o.Id).ValueGeneratedOnAdd();

            builder.Property<int>("BuyerId").IsRequired();

            builder.Property<string>("CardHolderName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property<string>("Alias")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property<string>("CardNumber")
                .HasMaxLength(25)
                .IsRequired();

            builder.Property<DateTime>("Expiration")
                .IsRequired();

            builder.Property<int>("CardTypeId")
                .IsRequired();

            builder.HasOne(p => p.CardType)
                .WithMany()
                .HasForeignKey("CardTypeId");
        }
    }

    internal class CardTypeEntityTypeConfiguration : IEntityTypeConfiguration<CardType>
    {
        public void Configure(EntityTypeBuilder<CardType> builder)
        {
            builder.ToTable("cardtypes");

            builder.HasKey(ct => ct.Id);

            builder.Property(ct => ct.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(ct => ct.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }

    internal class BuyerEntityTypeConfiguration : IEntityTypeConfiguration<Buyer>
    {
        public void Configure(EntityTypeBuilder<Buyer> builder)
        {
            builder.ToTable("buyers");

            builder.HasKey(b => b.Id);

            builder.Ignore(b => b.DomainEvents);

            builder.Property(o => o.Id).ValueGeneratedOnAdd();

            builder.Property(b => b.IdentityGuid)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasIndex("IdentityGuid")
              .IsUnique(true);

            builder.Property(b => b.Name);

            builder.HasMany(b => b.PaymentMethods)
               .WithOne()
               .HasForeignKey("BuyerId")
               .OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(Buyer.PaymentMethods));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
