using Microsoft.EntityFrameworkCore;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<PetPhoto> PetPhotos => Set<PetPhoto>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Faq> Faqs => Set<Faq>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationMessage> ConversationMessages => Set<ConversationMessage>();
    public DbSet<Escalation> Escalations => Set<Escalation>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.UserLevel).HasColumnName("user_level").HasMaxLength(20).HasDefaultValue("User");
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Pet configuration
        modelBuilder.Entity<Pet>(entity =>
        {
            entity.ToTable("pets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Species).HasColumnName("species").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Breed).HasColumnName("breed").HasMaxLength(100);
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.Sex).HasColumnName("sex").HasMaxLength(20);
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url").HasMaxLength(500);
            entity.Property(e => e.FosterId).HasColumnName("foster_id");
            entity.Property(e => e.AssignedAt).HasColumnName("assigned_at");
            entity.Property(e => e.AssignmentNotes).HasColumnName("assignment_notes");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Pets)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Foster)
                .WithMany()
                .HasForeignKey(e => e.FosterId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // PetPhoto configuration
        modelBuilder.Entity<PetPhoto>(entity =>
        {
            entity.ToTable("pet_photos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PetId).HasColumnName("pet_id");
            entity.Property(e => e.FileName).HasColumnName("file_name").HasMaxLength(255).IsRequired();
            entity.Property(e => e.FilePath).HasColumnName("file_path").HasMaxLength(500).IsRequired();
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary").HasDefaultValue(false);
            entity.Property(e => e.UploadedAt).HasColumnName("uploaded_at");
            entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by");

            entity.HasOne(e => e.Pet)
                .WithMany(p => p.Photos)
                .HasForeignKey(e => e.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Uploader)
                .WithMany()
                .HasForeignKey(e => e.UploadedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // MedicalRecord configuration
        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.ToTable("medical_records");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PetId).HasColumnName("pet_id");
            entity.Property(e => e.RecordType).HasColumnName("record_type").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.RecordDate).HasColumnName("record_date");
            entity.Property(e => e.NextDueDate).HasColumnName("next_due_date");
            entity.Property(e => e.Veterinarian).HasColumnName("veterinarian").HasMaxLength(100);
            entity.Property(e => e.ClinicName).HasColumnName("clinic_name").HasMaxLength(100);
            entity.Property(e => e.Cost).HasColumnName("cost").HasColumnType("decimal(10,2)");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");

            entity.HasOne(e => e.Pet)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(e => e.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Appointment configuration
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("appointments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.PetId).HasColumnName("pet_id");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.AppointmentDate).HasColumnName("appointment_date");
            entity.Property(e => e.AppointmentTime).HasColumnName("appointment_time");
            entity.Property(e => e.IsCompleted).HasColumnName("is_completed").HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Appointments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Pet)
                .WithMany(p => p.Appointments)
                .HasForeignKey(e => e.PetId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // FAQ configuration
        modelBuilder.Entity<Faq>(entity =>
        {
            entity.ToTable("faqs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Question).HasColumnName("question").IsRequired();
            entity.Property(e => e.Answer).HasColumnName("answer").IsRequired();
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order").HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        // Conversation configuration
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.ToTable("conversations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(200);
            entity.Property(e => e.OpenAiThreadId).HasColumnName("openai_thread_id").HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Conversations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ConversationMessage configuration
        modelBuilder.Entity<ConversationMessage>(entity =>
        {
            entity.ToTable("conversation_messages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Content).HasColumnName("content").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(e => e.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(e => e.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Escalation configuration
        modelBuilder.Entity<Escalation>(entity =>
        {
            entity.ToTable("escalations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.UserEmail).HasColumnName("user_email").HasMaxLength(255).IsRequired();
            entity.Property(e => e.UserName).HasColumnName("user_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.UserQuestion).HasColumnName("user_question").IsRequired();
            entity.Property(e => e.AdditionalContext).HasColumnName("additional_context");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("Pending");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.EmailSentAt).HasColumnName("email_sent_at");
            entity.Property(e => e.ResolvedAt).HasColumnName("resolved_at");
            entity.Property(e => e.StaffNotes).HasColumnName("staff_notes");

            entity.HasOne(e => e.Conversation)
                .WithMany(c => c.Escalations)
                .HasForeignKey(e => e.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Escalations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Message)
                .WithMany()
                .HasForeignKey(e => e.MessageId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // NotificationPreference configuration
        modelBuilder.Entity<NotificationPreference>(entity =>
        {
            entity.ToTable("notification_preferences");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.EmailAppointments).HasColumnName("email_appointments").HasDefaultValue(true);
            entity.Property(e => e.EmailReminders).HasColumnName("email_reminders").HasDefaultValue(true);
            entity.Property(e => e.ReminderDaysBefore).HasColumnName("reminder_days_before").HasDefaultValue(1);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.UserId).IsUnique();

            entity.HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<NotificationPreference>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
