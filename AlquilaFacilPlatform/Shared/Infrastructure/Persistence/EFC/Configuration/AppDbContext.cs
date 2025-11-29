using AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Booking.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.ImageManagement.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using AlquilaFacilPlatform.Subscriptions.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Subscriptions.Domain.Model.Entities;
using AlquilaFacilPlatform.IAM.Domain.Model.Aggregates;
using AlquilaFacilPlatform.IAM.Domain.Model.Entities;
using RefreshToken = AlquilaFacilPlatform.IAM.Domain.Model.Entities.RefreshToken;
using AlquilaFacilPlatform.Locals.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Locals.Domain.Model.Entities;
using AlquilaFacilPlatform.Management.Domain.Model.Entities;
using AlquilaFacilPlatform.Notifications.Domain.Models.Aggregates;
using AlquilaFacilPlatform.Profiles.Domain.Model.Aggregates;
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        base.OnConfiguring(builder);
        // Enable Audit Fields Interceptors
        builder.AddCreatedUpdatedInterceptor();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Place here your entities configuration
        
        builder.Entity<Plan>().HasKey(p => p.Id);
        builder.Entity<Plan>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Plan>().Property(p => p.Name).IsRequired();
        builder.Entity<Plan>().Property(p => p.Service).IsRequired();
        builder.Entity<Plan>().Property(p => p.Price).IsRequired();

        builder.Entity<Plan>().HasMany<Subscription>().WithOne().HasForeignKey(s => s.PlanId);
        
        builder.Entity<Subscription>().HasKey(s => s.Id);
        builder.Entity<Subscription>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Subscription>().Property(s => s.VoucherImageUrl).IsRequired();
        builder.Entity<Subscription>().HasOne<SubscriptionStatus>().WithMany()
            .HasForeignKey(s => s.SubscriptionStatusId);
        builder.Entity<Subscription>()
            .HasOne<Plan>().WithMany().HasForeignKey(s => s.PlanId);

        builder.Entity<SubscriptionStatus>().HasKey(s => s.Id);
        builder.Entity<SubscriptionStatus>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SubscriptionStatus>().Property(s => s.Status);

        builder.Entity<Invoice>().HasKey(i => i.Id);
        builder.Entity<Invoice>().Property(i => i.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Invoice>().Property(i => i.SubscriptionId).IsRequired();
        builder.Entity<Invoice>().Property(i => i.Amount).IsRequired();
        builder.Entity<Invoice>().Property(i => i.Date).IsRequired();
        
        builder.Entity<Subscription>().HasOne<Invoice>().WithOne().HasForeignKey<Invoice>(i => i.SubscriptionId);

        //Local Context

        builder.Entity<LocalCategory>().HasKey(c => c.Id);
        builder.Entity<LocalCategory>().Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<LocalCategory>().Property(c => c.Name).IsRequired().HasMaxLength(30);
        builder.Entity<LocalCategory>().Property(c => c.PhotoUrl).IsRequired();


        builder.Entity<LocalCategory>().HasMany<Local>()
            .WithOne()
            .HasForeignKey(t => t.LocalCategoryId);
        
        builder.Entity<Local>().HasKey(p => p.Id);
        builder.Entity<Local>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Local>().Property(p => p.Features).IsRequired();
        builder.Entity<Local>().Property(p => p.Capacity).IsRequired();
        builder.Entity<Local>().OwnsOne(p => p.PricePerHour,
            n =>
            {
                n.WithOwner().HasForeignKey("Id");
                n.Property(p => p.Value).HasColumnName("PricePerHour");
            });
        builder.Entity<Local>().OwnsOne(p => p.Name,
            e =>
            {
                e.WithOwner().HasForeignKey("Id");
                e.Property(a => a.Value).HasColumnName("LocalName");
            });
        builder.Entity<Local>().OwnsOne(p => p.Description,
            h =>
            {
                h.WithOwner().HasForeignKey("Id");
                h.Property(g => g.Value).HasColumnName("Description");

            });
        builder.Entity<Local>().OwnsOne(p => p.Country,
            a =>
            {
                a.WithOwner().HasForeignKey("Id");
                a.Property(c => c.Value).HasColumnName("Country");

            });
        builder.Entity<Local>().OwnsOne(p => p.City,
            a =>
            {
                a.WithOwner().HasForeignKey("Id");
                a.Property(c => c.Value).HasColumnName("City");

            });
        builder.Entity<Local>().OwnsOne(p => p.District,
            a =>
            {
                a.WithOwner().HasForeignKey("Id");
                a.Property(d => d.Value).HasColumnName("District");

            });
        builder.Entity<Local>().OwnsOne(p => p.Street,
            a =>
            {
                a.WithOwner().HasForeignKey("Id");
                a.Property(d => d.Value).HasColumnName("Street");

            });
        builder.Entity<Local>().HasOne<LocalCategory>().WithMany().HasForeignKey(l => l.LocalCategoryId);
        builder.Entity<Local>().HasOne<User>().WithMany().HasForeignKey(l => l.UserId);
        builder.Entity<Local>()
            .HasMany(l => l.LocalPhotos)
            .WithOne(p => p.Local)
            .HasForeignKey(p => p.LocalId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<LocalPhoto>().HasKey(p => p.Id);
        builder.Entity<LocalPhoto>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<LocalPhoto>().Property(p => p.Url).IsRequired();

        builder.Entity<Comment>().HasKey(c => c.Id);
        builder.Entity<Comment>().Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();

        builder.Entity<Comment>().OwnsOne(c => c.Text,
            n =>
            {
                n.WithOwner().HasForeignKey("Id");
                n.Property(g => g.Value).HasColumnName("Text");
            });
        
        builder.Entity<Comment>().OwnsOne(c => c.Rating,
            n =>
            {
                n.WithOwner().HasForeignKey("Id");
                n.Property(g => g.Value).HasColumnName("Rating");
            });
        

        builder.Entity<Comment>().HasOne<User>().WithMany().HasForeignKey(u => u.UserId);
        builder.Entity<Comment>().HasOne<Local>().WithMany().HasForeignKey(l => l.LocalId);
        
        builder.Entity<Report>().HasKey(report => report.Id);
        builder.Entity<Report>().Property(report => report.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Report>().Property(report => report.Description).IsRequired();
        builder.Entity<Report>().Property(report => report.Title).IsRequired();
        builder.Entity<Report>().Property(report => report.CreatedAt).IsRequired();
        builder.Entity<Report>().HasOne<User>().WithMany().HasForeignKey(r => r.UserId);
        builder.Entity<Report>().HasOne<Local>().WithMany().HasForeignKey(r => r.LocalId);
        
        
        // Profile Context
        
        builder.Entity<Profile>().HasKey(p => p.Id);
        builder.Entity<Profile>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Profile>().OwnsOne(p => p.Name,
            n =>
            {
                n.WithOwner().HasForeignKey("Id");
                n.Property(p => p.Name).HasColumnName("FirstName");
                n.Property(p => p.FatherName).HasColumnName("FatherName");
                n.Property(p => p.MotherName).HasColumnName("MotherName");
            });
        builder.Entity<Profile>().OwnsOne(p => p.PhoneN,
            e =>
            {
                e.WithOwner().HasForeignKey("Id");
                e.Property(a => a.PhoneNumber).HasColumnName("PhoneNumber");
            });
        builder.Entity<Profile>().OwnsOne(p => p.DocumentN,
            e =>
            {
                e.WithOwner().HasForeignKey("Id");
                e.Property(a => a.NumberDocument).HasColumnName("NumberDocument");
            });
        builder.Entity<Profile>().OwnsOne(p => p.Birth,
            e =>
            {
                e.WithOwner().HasForeignKey("Id");
                e.Property(a => a.BirthDate).HasColumnName("BirthDate");
            });
        builder.Entity<Profile>().Property(p => p.AvatarUrl).HasMaxLength(500);

        builder.Entity<Profile>().HasOne<User>().WithOne().HasForeignKey<Profile>(p => p.UserId);
        
        
        //IAM Context

        builder.Entity<User>().HasKey(u => u.Id);
        builder.Entity<User>().Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<User>().Property(u => u.Username).IsRequired();
        builder.Entity<User>().Property(u => u.PasswordHash).IsRequired();
        builder.Entity<User>().Property(u => u.Email).IsRequired();
        builder.Entity<UserRole>().HasMany<User>().WithOne().HasForeignKey(u => u.RoleId);

        builder.Entity<UserRole>().HasKey(ur => ur.Id);
        builder.Entity<UserRole>().Property(ur => ur.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<UserRole>().Property(ur => ur.Role).IsRequired();

        // RefreshToken entity configuration
        builder.Entity<RefreshToken>().HasKey(rt => rt.Id);
        builder.Entity<RefreshToken>().Property(rt => rt.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<RefreshToken>().Property(rt => rt.Token).IsRequired().HasMaxLength(256);
        builder.Entity<RefreshToken>().Property(rt => rt.UserId).IsRequired();
        builder.Entity<RefreshToken>().Property(rt => rt.ExpiresAt).IsRequired();
        builder.Entity<RefreshToken>().Property(rt => rt.CreatedAt).IsRequired();
        builder.Entity<RefreshToken>().Property(rt => rt.IsRevoked).IsRequired();
        builder.Entity<RefreshToken>().HasOne<User>().WithMany().HasForeignKey(rt => rt.UserId);


        builder.Entity<Reservation>().HasKey(r => r.Id);
        builder.Entity<Reservation>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Reservation>().Property(r => r.StartDate).IsRequired();
        builder.Entity<Reservation>().Property(r => r.EndDate).IsRequired();
        builder.Entity<Reservation>().Property(r => r.VoucherImageUrl).IsRequired();
        builder.Entity<User>().HasMany<Reservation>().WithOne().HasForeignKey(r => r.UserId);
        builder.Entity<Local>().HasMany<Reservation>().WithOne().HasForeignKey(r => r.LocalId);

        // Notification Context

        builder.Entity<Notification>().HasKey(n => n.Id);
        builder.Entity<Notification>().Property(n => n.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Notification>().Property(n => n.Title).IsRequired();
        builder.Entity<Notification>().Property(n => n.Description).IsRequired();
        
        builder.Entity<User>().HasMany<Notification>().WithOne().HasForeignKey(n => n.UserId);

        // Management Context
        
        builder.Entity<SensorType>().HasKey(s => s.Id);
        builder.Entity<SensorType>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SensorType>().Property(s => s.Type).IsRequired().HasMaxLength(30);
        
        builder.Entity<Reading>().HasKey(reading => reading.Id);
        builder.Entity<Reading>().Property(reading => reading.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Reading>().Property(reading => reading.LocalId).IsRequired();
        builder.Entity<Reading>().Property(reading => reading.SensorTypeId).IsRequired();
        builder.Entity<Reading>().Property(reading => reading.Message).IsRequired().HasMaxLength(500);
        builder.Entity<Reading>().Property(reading => reading.Timestamp).IsRequired();
        
        builder.Entity<Reading>().HasOne<Local>().WithMany().HasForeignKey(r => r.LocalId);
        builder.Entity<Reading>().HasOne<SensorType>().WithMany().HasForeignKey(r => r.SensorTypeId);
        
        builder.Entity<LocalEdgeNode>().HasKey(l => l.Id);
        builder.Entity<LocalEdgeNode>().Property(l => l.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<LocalEdgeNode>().Property(l => l.LocalId).IsRequired();
        builder.Entity<LocalEdgeNode>().Property(l => l.EdgeNodeUrl).IsRequired().HasMaxLength(500);
        builder.Entity<LocalEdgeNode>().HasOne<Local>().WithMany().HasForeignKey(l => l.LocalId);

        // Chat Context
        builder.Entity<Conversation>().HasKey(c => c.Id);
        builder.Entity<Conversation>().Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Conversation>().Property(c => c.ParticipantOneId).IsRequired();
        builder.Entity<Conversation>().Property(c => c.ParticipantTwoId).IsRequired();
        builder.Entity<Conversation>().Property(c => c.CreatedAt).IsRequired();
        builder.Entity<Conversation>().HasOne<User>().WithMany().HasForeignKey(c => c.ParticipantOneId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Conversation>().HasOne<User>().WithMany().HasForeignKey(c => c.ParticipantTwoId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Conversation>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Message>().HasKey(m => m.Id);
        builder.Entity<Message>().Property(m => m.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Message>().Property(m => m.ConversationId).IsRequired();
        builder.Entity<Message>().Property(m => m.SenderId).IsRequired();
        builder.Entity<Message>().Property(m => m.Content).HasMaxLength(2000);
        builder.Entity<Message>().Property(m => m.SentAt).IsRequired();
        builder.Entity<Message>().Property(m => m.IsRead).IsRequired();
        builder.Entity<Message>().Property(m => m.AttachmentUrl).HasMaxLength(500);
        builder.Entity<Message>().Property(m => m.AttachmentType).HasMaxLength(50);
        builder.Entity<Message>().Property(m => m.AttachmentFileName).HasMaxLength(255);
        builder.Entity<Message>().Property(m => m.Reactions).HasMaxLength(2000);
        builder.Entity<Message>().HasOne<User>().WithMany().HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ChatResponseMetric>().HasKey(m => m.Id);
        builder.Entity<ChatResponseMetric>().Property(m => m.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<ChatResponseMetric>().Property(m => m.UserId).IsRequired();
        builder.Entity<ChatResponseMetric>().Property(m => m.ConversationId).IsRequired();
        builder.Entity<ChatResponseMetric>().Property(m => m.FirstMessageAt).IsRequired();
        builder.Entity<ChatResponseMetric>().Property(m => m.LastCalculatedAt).IsRequired();
        builder.Entity<ChatResponseMetric>().HasOne<User>().WithMany().HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ChatResponseMetric>().HasOne<Conversation>().WithMany().HasForeignKey(m => m.ConversationId).OnDelete(DeleteBehavior.Cascade);

        // Contracts Context
        builder.Entity<ContractTemplate>().HasKey(ct => ct.Id);
        builder.Entity<ContractTemplate>().Property(ct => ct.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<ContractTemplate>().Property(ct => ct.Title).IsRequired().HasMaxLength(200);
        builder.Entity<ContractTemplate>().Property(ct => ct.Content).IsRequired();
        builder.Entity<ContractTemplate>().Property(ct => ct.UserId).IsRequired();
        builder.Entity<ContractTemplate>().Property(ct => ct.CreatedAt).IsRequired();
        builder.Entity<ContractTemplate>().HasOne<User>().WithMany().HasForeignKey(ct => ct.UserId);

        builder.Entity<ContractInstance>().HasKey(ci => ci.Id);
        builder.Entity<ContractInstance>().Property(ci => ci.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<ContractInstance>().Property(ci => ci.ContractTemplateId).IsRequired();
        builder.Entity<ContractInstance>().Property(ci => ci.LocalId).IsRequired();
        builder.Entity<ContractInstance>().Property(ci => ci.LandlordUserId).IsRequired();
        builder.Entity<ContractInstance>().Property(ci => ci.TenantUserId).IsRequired();
        builder.Entity<ContractInstance>().Property(ci => ci.ReservationId).IsRequired();
        builder.Entity<ContractInstance>().Property(ci => ci.StartDate).IsRequired();
        builder.Entity<ContractInstance>().Property(ci => ci.EndDate).IsRequired();
        builder.Entity<ContractInstance>().Property(ci => ci.Status).IsRequired();
        builder.Entity<ContractInstance>().Property(ci => ci.GeneratedContent).IsRequired();
        builder.Entity<ContractInstance>().Property(ci => ci.CreatedAt).IsRequired();
        builder.Entity<ContractInstance>().Property(ci => ci.LandlordSignature).HasMaxLength(2000);
        builder.Entity<ContractInstance>().Property(ci => ci.TenantSignature).HasMaxLength(2000);
        builder.Entity<ContractInstance>().Property(ci => ci.Version).IsRequired();
        builder.Entity<ContractInstance>()
            .HasOne(ci => ci.Template)
            .WithMany()
            .HasForeignKey(ci => ci.ContractTemplateId);
        builder.Entity<ContractInstance>().HasOne<Local>().WithMany().HasForeignKey(ci => ci.LocalId);
        builder.Entity<ContractInstance>().HasOne<User>().WithMany().HasForeignKey(ci => ci.LandlordUserId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ContractInstance>().HasOne<User>().WithMany().HasForeignKey(ci => ci.TenantUserId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ContractInstance>().HasOne<Reservation>().WithMany().HasForeignKey(ci => ci.ReservationId);

        // ImageManagement Context
        builder.Entity<Image>().HasKey(i => i.Id);
        builder.Entity<Image>().Property(i => i.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Image>().Property(i => i.Url).IsRequired().HasMaxLength(500);
        builder.Entity<Image>().Property(i => i.FileName).IsRequired().HasMaxLength(255);
        builder.Entity<Image>().Property(i => i.ContentType).IsRequired().HasMaxLength(100);
        builder.Entity<Image>().Property(i => i.FileSizeBytes).IsRequired();
        builder.Entity<Image>().Property(i => i.StoragePath).IsRequired().HasMaxLength(500);
        builder.Entity<Image>().Property(i => i.EntityType).IsRequired().HasMaxLength(50);
        builder.Entity<Image>().Property(i => i.EntityId).IsRequired();
        builder.Entity<Image>().Property(i => i.UploadedAt).IsRequired();
        builder.Entity<Image>().Property(i => i.UploadedBy).IsRequired();
        builder.Entity<Image>().Property(i => i.IsDeleted).IsRequired();
        builder.Entity<Image>().HasOne<User>().WithMany().HasForeignKey(i => i.UploadedBy);

        // Availability Context
        builder.Entity<AvailabilityCalendar>().HasKey(ac => ac.Id);
        builder.Entity<AvailabilityCalendar>().Property(ac => ac.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<AvailabilityCalendar>().Property(ac => ac.LocalId).IsRequired();
        builder.Entity<AvailabilityCalendar>().Property(ac => ac.StartDate).IsRequired();
        builder.Entity<AvailabilityCalendar>().Property(ac => ac.EndDate).IsRequired();
        builder.Entity<AvailabilityCalendar>().Property(ac => ac.IsAvailable).IsRequired();
        builder.Entity<AvailabilityCalendar>().Property(ac => ac.Reason).HasMaxLength(500);
        builder.Entity<AvailabilityCalendar>().Property(ac => ac.CreatedAt).IsRequired();
        builder.Entity<AvailabilityCalendar>().Property(ac => ac.CreatedBy).IsRequired();
        builder.Entity<AvailabilityCalendar>().HasOne<Local>().WithMany().HasForeignKey(ac => ac.LocalId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<AvailabilityCalendar>().HasOne<User>().WithMany().HasForeignKey(ac => ac.CreatedBy).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BlockedDate>().HasKey(bd => bd.Id);
        builder.Entity<BlockedDate>().Property(bd => bd.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<BlockedDate>().Property(bd => bd.LocalId).IsRequired();
        builder.Entity<BlockedDate>().Property(bd => bd.Date).IsRequired();
        builder.Entity<BlockedDate>().Property(bd => bd.Reason).IsRequired().HasMaxLength(500);
        builder.Entity<BlockedDate>().Property(bd => bd.IsRecurring).IsRequired();
        builder.Entity<BlockedDate>().Property(bd => bd.CreatedAt).IsRequired();
        builder.Entity<BlockedDate>().Property(bd => bd.CreatedBy).IsRequired();
        builder.Entity<BlockedDate>().HasOne<Local>().WithMany().HasForeignKey(bd => bd.LocalId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<BlockedDate>().HasOne<User>().WithMany().HasForeignKey(bd => bd.CreatedBy).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AvailabilityRule>().HasKey(ar => ar.Id);
        builder.Entity<AvailabilityRule>().Property(ar => ar.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<AvailabilityRule>().Property(ar => ar.LocalId).IsRequired();
        builder.Entity<AvailabilityRule>().Property(ar => ar.DayOfWeek).IsRequired();
        builder.Entity<AvailabilityRule>().Property(ar => ar.StartTime).IsRequired();
        builder.Entity<AvailabilityRule>().Property(ar => ar.EndTime).IsRequired();
        builder.Entity<AvailabilityRule>().Property(ar => ar.IsAvailable).IsRequired();
        builder.Entity<AvailabilityRule>().Property(ar => ar.CreatedAt).IsRequired();
        builder.Entity<AvailabilityRule>().Property(ar => ar.CreatedBy).IsRequired();
        builder.Entity<AvailabilityRule>().HasOne<Local>().WithMany().HasForeignKey(ar => ar.LocalId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<AvailabilityRule>().HasOne<User>().WithMany().HasForeignKey(ar => ar.CreatedBy).OnDelete(DeleteBehavior.Restrict);

        // Apply SnakeCase Naming Convention
        builder.UseSnakeCaseWithPluralizedTableNamingConvention();

    }
}
