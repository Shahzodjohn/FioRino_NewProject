using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using FioRino_NewProject.Entities;

#nullable disable

namespace FioRino_NewProject.Data
{
    public partial class FioRinoBaseContext : DbContext
    {
        public FioRinoBaseContext()
        {
        }

        public FioRinoBaseContext(DbContextOptions<FioRinoBaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DeliveryReport> DeliveryReports { get; set; }
        public virtual DbSet<DmCategory> DmCategories { get; set; }
        public virtual DbSet<DmCodesForResetPassword> DmCodesForResetPasswords { get; set; }
        public virtual DbSet<DmFileWz> DmFileWzs { get; set; }
        public virtual DbSet<DmLogEntry> DmLogEntries { get; set; }
        public virtual DbSet<DmOrder> DmOrders { get; set; }
        public virtual DbSet<DmOrderArchievum> DmOrderArchievums { get; set; }
        public virtual DbSet<DmOrderProduct> DmOrderProducts { get; set; }
        public virtual DbSet<DmOrderStatus> DmOrderStatuses { get; set; }
        public virtual DbSet<DmOrderType> DmOrderTypes { get; set; }
        public virtual DbSet<DmPosition> DmPositions { get; set; }
        public virtual DbSet<DmProduct> DmProducts { get; set; }
        public virtual DbSet<DmProductStatus> DmProductStatuses { get; set; }
        public virtual DbSet<DmProductType> DmProductTypes { get; set; }
        public virtual DbSet<DmRole> DmRoles { get; set; }
        public virtual DbSet<DmSize> DmSizes { get; set; }
        public virtual DbSet<DmSkucode> DmSkucodes { get; set; }
        public virtual DbSet<DmStorage> DmStorages { get; set; }
        public virtual DbSet<DmUniqueProduct> DmUniqueProducts { get; set; }
        public virtual DbSet<DmUser> DmUsers { get; set; }
        public virtual DbSet<DmUsersAccess> DmUsersAccesses { get; set; }
        public virtual DbSet<DmWzMagazyn> DmWzMagazyns { get; set; }
        public virtual DbSet<DomainEntity> DomainEntities { get; set; }
        public virtual DbSet<Entity> Entities { get; set; }
        public virtual DbSet<IncomingMessage> IncomingMessages { get; set; }
        public virtual DbSet<Link> Links { get; set; }
        public virtual DbSet<LinksExt> LinksExts { get; set; }
        public virtual DbSet<LinksIdent> LinksIdents { get; set; }
        public virtual DbSet<MessagesToSend> MessagesToSends { get; set; }
        public virtual DbSet<OutgoingMessage> OutgoingMessages { get; set; }
        public virtual DbSet<Structure> Structures { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=lab500.nc-one.com, 1530;Database=FioRinoBase;User ID=sa;Password=KFaerZ2dZRiM");
                //optionsBuilder.UseSqlServer("Data source = SHAHZOD;initial catalog = FioRinoBase; integrated security = true;MultipleActiveResultSets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<DeliveryReport>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.Id, e.Result }, "ClusteredIndex-20190131@check93425")
                    .IsClustered();

                entity.HasIndex(e => e.MessageId, "NonClusteredIndex-20190131@check93501");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MessageId)
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasColumnName("messageId");

                entity.Property(e => e.ProcessDate)
                    .HasColumnType("datetime")
                    .HasColumnName("processDate")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Result).HasColumnName("result");
            });

            modelBuilder.Entity<DmCategory>(entity =>
            {
                entity.ToTable("dm_Categories");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DmCodesForResetPassword>(entity =>
            {
                entity.ToTable("dm_CodesForResetPassword");

                entity.Property(e => e.RandomNumber)
                    .HasMaxLength(50)
                    .HasColumnName("randomNumber");

                entity.Property(e => e.ValidDate).HasColumnType("date");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DmCodesForResetPasswords)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__dm_CodesF__UserI__01142BA1");
            });

            modelBuilder.Entity<DmFileWz>(entity =>
            {
                entity.ToTable("dm_FileWz");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FileSize).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DmFileWzs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dm_FileWz_dm_Users");
            });

            modelBuilder.Entity<DmLogEntry>(entity =>
            {
                entity.ToTable("dm_LogEntry");

                entity.HasIndex(e => e.AuthorizedUser, "idx_dm_LogEntry_authorizedUser");

                entity.HasIndex(e => e.EntityName, "idx_dm_LogEntry_entityName");

                entity.HasIndex(e => e.MessageId, "idx_dm_LogEntry_messageId");

                entity.HasIndex(e => e.Time, "idx_dm_LogEntry_time");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActionType).HasColumnName("actionType");

                entity.Property(e => e.AuthorizedUser).HasColumnName("authorizedUser");

                entity.Property(e => e.EntityId).HasColumnName("entityId");

                entity.Property(e => e.EntityName)
                    .HasMaxLength(256)
                    .HasColumnName("entityName");

                entity.Property(e => e.Info).HasColumnName("info");

                entity.Property(e => e.MessageId)
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasColumnName("messageId");

                entity.Property(e => e.ProxyId)
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("proxyId");

                entity.Property(e => e.Success).HasColumnName("success");

                entity.Property(e => e.Time)
                    .HasColumnType("datetime")
                    .HasColumnName("time")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.TimeFinish)
                    .HasColumnType("datetime")
                    .HasColumnName("timeFinish")
                    .HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<DmOrder>(entity =>
            {
                entity.ToTable("dm_Orders");

                entity.Property(e => e.CreatedAt).HasColumnType("date");

                entity.Property(e => e.DateOfRelease).HasColumnType("date");

                entity.Property(e => e.IsBlocked).HasColumnName("isBlocked");

                entity.Property(e => e.IsInArchievum)
                    .HasColumnName("Is_InArchievum")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsInMagazyn)
                    .HasColumnName("Is_InMagazyn")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.OrderExecutor).HasMaxLength(50);

                entity.Property(e => e.SourceOfOrder).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("date");

                entity.HasOne(d => d.OrderArchievum)
                    .WithMany(p => p.DmOrders)
                    .HasForeignKey(d => d.OrderArchievumId)
                    .HasConstraintName("FK_dm_Orders_dm_OrderArchievum");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.DmOrderReceivers)
                    .HasForeignKey(d => d.ReceiverId)
                    .HasConstraintName("FK__dm_Orders__Recei__0C1BC9F9");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.DmOrderSenders)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK__dm_Orders__Sende__0D0FEE32");
            });

            modelBuilder.Entity<DmOrderArchievum>(entity =>
            {
                entity.ToTable("dm_OrderArchievum");

                entity.Property(e => e.CreateAt).HasColumnType("date");

                entity.Property(e => e.ImplementationDate).HasColumnType("date");

                entity.HasOne(d => d.FileWz)
                    .WithMany(p => p.DmOrderArchievums)
                    .HasForeignKey(d => d.FileWzId)
                    .HasConstraintName("FK_dm_OrderArchievum_WzMagazyn");

                entity.HasOne(d => d.OrderStatus)
                    .WithMany(p => p.DmOrderArchievums)
                    .HasForeignKey(d => d.OrderStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__dm_OrderArch__Order__4AB81AF0");

                entity.HasOne(d => d.Reciever)
                    .WithMany(p => p.DmOrderArchievumRecievers)
                    .HasForeignKey(d => d.RecieverId)
                    .HasConstraintName("FK__dm_OrderA__Recie__55AAAAAF");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.DmOrderArchievumSenders)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK__dm_OrderA__Sende__54B68676");
            });

            modelBuilder.Entity<DmOrderProduct>(entity =>
            {
                entity.ToTable("dm_OrderProducts");

                entity.Property(e => e.Gtin).HasMaxLength(50);

                entity.Property(e => e.SkucodeId).HasColumnName("SKUcodeId");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.DmOrderProducts)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__dm_OrderP__Categ__40C49C62");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.DmOrderProducts)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dm_OrderProducts_dm_Orders");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.DmOrderProducts)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_dm_OrderProducts_dm_Products");

                entity.HasOne(d => d.ProductStatuses)
                    .WithMany(p => p.DmOrderProducts)
                    .HasForeignKey(d => d.ProductStatusesId)
                    .HasConstraintName("FK__dm_OrderP__Produ__58671BC9");

                entity.HasOne(d => d.Size)
                    .WithMany(p => p.DmOrderProducts)
                    .HasForeignKey(d => d.SizeId)
                    .HasConstraintName("FK_dm_OrderProducts_dm_Sizes");

                entity.HasOne(d => d.Skucode)
                    .WithMany(p => p.DmOrderProducts)
                    .HasForeignKey(d => d.SkucodeId)
                    .HasConstraintName("FK__dm_OrderP__SKUco__1B9317B3");

                entity.HasOne(d => d.UniqueProduct)
                    .WithMany(p => p.DmOrderProducts)
                    .HasForeignKey(d => d.UniqueProductId)
                    .HasConstraintName("FK__dm_OrderP__Uniqu__1758727B");
            });

            modelBuilder.Entity<DmOrderStatus>(entity =>
            {
                entity.ToTable("dm_OrderStatuses");

                entity.Property(e => e.OrderStatusName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DmOrderType>(entity =>
            {
                entity.ToTable("dm_OrderTypes");

                entity.Property(e => e.OrderTypeName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DmPosition>(entity =>
            {
                entity.ToTable("dm_Positions");

                entity.Property(e => e.PositionName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DmProduct>(entity =>
            {
                entity.ToTable("dm_Products");

                entity.Property(e => e.Gtin)
                    .HasMaxLength(50)
                    .HasColumnName("GTIN");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Skunumber)
                    .HasMaxLength(50)
                    .HasColumnName("SKUnumber");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.DmProducts)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__dm_Produc__Categ__7E8CC4B1");

                entity.HasOne(d => d.Size)
                    .WithMany(p => p.DmProducts)
                    .HasForeignKey(d => d.SizeId)
                    .HasConstraintName("FK__dm_Produc__SizeI__0539C240");

                entity.HasOne(d => d.UniqueProduct)
                    .WithMany(p => p.DmProducts)
                    .HasForeignKey(d => d.UniqueProductId)
                    .HasConstraintName("FK__dm_Produc__Uniqu__15702A09");
            });

            modelBuilder.Entity<DmProductStatus>(entity =>
            {
                entity.ToTable("dm_ProductStatuses");

                entity.Property(e => e.StatusColor).HasMaxLength(50);

                entity.Property(e => e.StatusDescription).HasMaxLength(50);
            });

            modelBuilder.Entity<DmProductType>(entity =>
            {
                entity.ToTable("dm_ProductType");

                entity.Property(e => e.ProductTypeName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DmRole>(entity =>
            {
                entity.ToTable("dm_Roles");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DmSize>(entity =>
            {
                entity.ToTable("dm_Sizes");

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<DmSkucode>(entity =>
            {
                entity.ToTable("dm_SKUCodes");

                entity.Property(e => e.SkucodeName)
                    .HasMaxLength(50)
                    .HasColumnName("SKUcodeName");
            });

            modelBuilder.Entity<DmStorage>(entity =>
            {
                entity.ToTable("dm_Storage");

                entity.Property(e => e.Gtin)
                    .HasMaxLength(50)
                    .HasColumnName("GTIN");

                entity.Property(e => e.IsBlocked).HasColumnName("isBlocked");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.DmStorages)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__dm_Storag__Categ__035179CE");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.DmStorages)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__dm_Storag__Produ__0169315C");

                entity.HasOne(d => d.Size)
                    .WithMany(p => p.DmStorages)
                    .HasForeignKey(d => d.SizeId)
                    .HasConstraintName("FK__dm_Storag__SizeI__025D5595");

                entity.HasOne(d => d.UniqueProduct)
                    .WithMany(p => p.DmStorages)
                    .HasForeignKey(d => d.UniqueProductId)
                    .HasConstraintName("FK__dm_Storag__Uniqu__71D1E811");
            });

            modelBuilder.Entity<DmUniqueProduct>(entity =>
            {
                entity.ToTable("dm_UniqueProducts");

                entity.Property(e => e.ProductName).HasMaxLength(250);
            });

            modelBuilder.Entity<DmUser>(entity =>
            {
                entity.ToTable("dm_Users");

                entity.HasIndex(e => e.Email, "UQ__dm_Users__A9D10534F661AF62")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Image).HasMaxLength(250);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RoleId).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.DmUsers)
                    .HasForeignKey(d => d.PositionId)
                    .HasConstraintName("FK__dm_Users__Positi__0FA2421A");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.DmUsers)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__dm_Users__RoleId__13DCE752");
            });

            modelBuilder.Entity<DmUsersAccess>(entity =>
            {
                entity.ToTable("dm_UsersAccess");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DmUsersAccesses)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__dm_UsersA__UserI__5832119F");
            });

            modelBuilder.Entity<DmWzMagazyn>(entity =>
            {
                entity.ToTable("dm_WzMagazyn");

                entity.Property(e => e.CreatedAt).HasColumnType("date");

                entity.HasOne(d => d.OrderProduct)
                    .WithMany(p => p.DmWzMagazyns)
                    .HasForeignKey(d => d.OrderProductId)
                    .HasConstraintName("FK__dm_WzMaga__Order__257187A8");

                entity.HasOne(d => d.OrderStatuses)
                    .WithMany(p => p.DmWzMagazyns)
                    .HasForeignKey(d => d.OrderStatusesId)
                    .HasConstraintName("FK_dm_WzMagazyn_dm_OrderStatuses");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.DmWzMagazyns)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_dm_WzMagazyn_dm_Products");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.DmWzMagazynReceivers)
                    .HasForeignKey(d => d.ReceiverId)
                    .HasConstraintName("FK__dm_WzMaga__Recie__19DFD96B");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.DmWzMagazynSenders)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK__dm_WzMaga__Sende__1AD3FDA4");
            });

            modelBuilder.Entity<DomainEntity>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("DomainEntities");

                entity.Property(e => e.EntityDescription).IsRequired();

                entity.Property(e => e.EntityType).IsRequired();
            });

            modelBuilder.Entity<Entity>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Entities");

                entity.Property(e => e.Entity1).HasColumnName("Entity");
            });

            modelBuilder.Entity<IncomingMessage>(entity =>
            {
                entity.HasIndex(e => new { e.CreateDate, e.DeliveryState }, "NonClusteredIndex-20190201@check24918");

                entity.HasIndex(e => e.MessageId, "NonClusteredIndex-20190201@check24943");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Author)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasColumnName("author");

                entity.Property(e => e.Comment)
                    .HasMaxLength(1024)
                    .HasColumnName("comment");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("createDate")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DeliveryState).HasColumnName("deliveryState");

                entity.Property(e => e.MessageId)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasColumnName("messageId");

                entity.Property(e => e.Offline).HasColumnName("offline");

                entity.Property(e => e.ParentId)
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasColumnName("parentId");

                entity.Property(e => e.Payload).HasColumnName("payload");
            });

            modelBuilder.Entity<Link>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Links");
            });

            modelBuilder.Entity<LinksExt>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("LinksExt");
            });

            modelBuilder.Entity<LinksIdent>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("LinksIdents");
            });

            modelBuilder.Entity<MessagesToSend>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("MessagesToSend");

                entity.Property(e => e.Destination)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasColumnName("destination");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Payload).HasColumnName("payload");
            });

            modelBuilder.Entity<OutgoingMessage>(entity =>
            {
                entity.HasIndex(e => e.Noauto, "NonClusteredIndex-20190201@check25550");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment)
                    .HasMaxLength(1024)
                    .HasColumnName("comment");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("createDate")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Destination)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasColumnName("destination");

                entity.Property(e => e.Noauto).HasColumnName("noauto");

                entity.Property(e => e.Payload).HasColumnName("payload");
            });

            modelBuilder.Entity<Structure>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Structures");

                entity.Property(e => e.FieldName).HasMaxLength(128);

                entity.Property(e => e.FieldType).HasMaxLength(128);

                entity.Property(e => e.TableName).HasMaxLength(128);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
