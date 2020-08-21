using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApiBack.Models
{
    public partial class ApiBckContext : DbContext
    {
        

        public ApiBckContext()
        {
        }

        public ApiBckContext(DbContextOptions<ApiBckContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CamposDesglose> CamposDesglose { get; set; }
        public virtual DbSet<DataResponse> DataResponse { get; set; }
        public virtual DbSet<GroupState> GroupState { get; set; }
        public virtual DbSet<ProcesoConciliacion> ProcesoConciliacion { get; set; }
        public virtual DbSet<ProcesoEnvioFirma> ProcesoEnvioFirma { get; set; }
        public virtual DbSet<SignatureState> SignatureState { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<ResultadoFeedback> ResultadoFeedback { get; set; }
        public virtual DbSet<DocsExtra> DocsExtra { get; set; }
      

        //public virtual DbSet<DatosConsulta> DatosConsulta { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=192.168.200.207;Initial Catalog=LogaltyFirmaDigital_Copia;User ID=nvo.backoffice;Password=Nb4ck4pr;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CamposDesglose>(entity =>
            {
                entity.HasKey(e => e.IdDetalleConciliacion)
                    .HasName("PK__CamposDe__B5EF54E67C354C5B");

                entity.Property(e => e.IdDetalleConciliacion).ValueGeneratedNever();

                entity.Property(e => e.BinaryContenGroupId)
                    .HasColumnName("binary_Conten_Group_Id")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ComentarioAutorizador)
                    .HasColumnName("comentario_Autorizador")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ComentariosFirma)
                    .HasColumnName("comentarios_Firma")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoActualGrupo)
                    .HasColumnName("estado_Actual_Grupo")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoActualTransaccion)
                    .HasColumnName("estado_Actual_Transaccion")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaFirma)
                    .HasColumnName("fecha_Firma")
                    .HasColumnType("date");

                entity.Property(e => e.GuId)
                    .HasColumnName("guId")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.IdFirma).HasColumnName("idFirma");

                entity.Property(e => e.IdProcesoConciliacion).HasColumnName("idProcesoConciliacion");

                entity.Property(e => e.ResultadoActualGrupo)
                    .HasColumnName("resultado_Actual_Grupo")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ResultadoFirma)
                    .HasColumnName("resultado_Firma")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.RstadoFirma).HasColumnName("rstadoFirma");
            });

            modelBuilder.Entity<DataResponse>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("PK__dataResp__15B69B8E94492240");

                entity.ToTable("dataResponse");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<GroupState>(entity =>
            {
                entity.HasKey(e => e.Result)
                    .HasName("PK__group_st__2A720C2D57E792CE");

                entity.ToTable("group_state");

                entity.Property(e => e.Result)
                    .HasColumnName("result")
                    .ValueGeneratedNever();

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasColumnName("comment")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnName("date")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GroupId).HasColumnName("group_id");

                entity.Property(e => e.ResultDate)
                    .IsRequired()
                    .HasColumnName("result_date")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SubstateComment)
                    .IsRequired()
                    .HasColumnName("substate_comment")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SubstateDate)
                    .IsRequired()
                    .HasColumnName("substate_date")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SubstateValue).HasColumnName("substate_value");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<ProcesoConciliacion>(entity =>
            {
                entity.HasKey(e => e.IdProcesoConciliacion)
                    .HasName("PK__ProcesoC__EDD571E493DEC546");

                entity.Property(e => e.IdProcesoConciliacion).ValueGeneratedNever();

                entity.Property(e => e.FeedbackLogalty).HasColumnType("text");

                entity.Property(e => e.Nodeid).HasMaxLength(50);
            });

            modelBuilder.Entity<ProcesoEnvioFirma>(entity =>
            {
                entity.HasKey(e => e.IdProcesoEnvioFirma)
                    .HasName("PK__ProcesoE__FEE841E71DC15942");

                entity.Property(e => e.IdProcesoEnvioFirma).ValueGeneratedNever();

                entity.Property(e => e.ApellidoMaterno)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ApellidoPaterno)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Curp)
                    .IsRequired()
                    .HasColumnName("CURP")
                    .HasMaxLength(18);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FechaEnvio).HasColumnType("date");

                entity.Property(e => e.PrimerNombre)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SegundoNombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TelefonoCelular)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SignatureState>(entity =>
            {
                entity.HasKey(e => e.Result)
                    .HasName("PK__signatur__2A720C2DE4FB1CBB");

                entity.ToTable("signature_state");

                entity.Property(e => e.Result)
                    .HasColumnName("result")
                    .ValueGeneratedNever();

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasColumnName("comment")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnName("date")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");

                entity.Property(e => e.RejectReason)
                    .IsRequired()
                    .HasColumnName("reject_reason")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ResultDate)
                    .IsRequired()
                    .HasColumnName("result_date")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RuleId).HasColumnName("rule_id");

                entity.Property(e => e.SubstateComment)
                    .IsRequired()
                    .HasColumnName("substate_comment")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SubstateDate)
                    .IsRequired()
                    .HasColumnName("substate_date")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SubstateValue).HasColumnName("substate_value");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => e.Result)
                    .HasName("PK__state__2A720C2D0C5BCF33");

                entity.ToTable("state");

                entity.Property(e => e.Result)
                    .HasColumnName("result")
                    .ValueGeneratedNever();

                entity.Property(e => e.CancelCode)
                    .IsRequired()
                    .HasColumnName("cancel_code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CancelReason)
                    .IsRequired()
                    .HasColumnName("cancel_reason")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasColumnName("comment")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnName("date")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ExternalId).HasColumnName("externalId");

                entity.Property(e => e.Guid)
                    .IsRequired()
                    .HasColumnName("guid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ResultDate)
                    .IsRequired()
                    .HasColumnName("result_date")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SubstateComment)
                    .IsRequired()
                    .HasColumnName("substate_comment")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SubstateDate)
                    .IsRequired()
                    .HasColumnName("substate_date")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SubstateValue).HasColumnName("substate_value");

                entity.Property(e => e.Value).HasColumnName("value");
            });
         




            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
