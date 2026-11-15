using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Prestamos.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Documento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoSolicitante = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Ubicacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EstadoOperativo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recursos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recursos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Movimientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Entidad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EntidadId = table.Column<int>(type: "int", nullable: false),
                    EstadoAnterior = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EstadoNuevo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movimientos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Solicitudes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitanteId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EvaluadaPorId = table.Column<int>(type: "int", nullable: true),
                    FechaEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    EntregadaPorId = table.Column<int>(type: "int", nullable: true),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitudes", x => x.Id);
                    table.CheckConstraint("CK_Solicitudes_Fechas", "[FechaFin] > [FechaInicio]");
                    table.ForeignKey(
                        name: "FK_Solicitudes_Usuarios_EntregadaPorId",
                        column: x => x.EntregadaPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Solicitudes_Usuarios_EvaluadaPorId",
                        column: x => x.EvaluadaPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Solicitudes_Usuarios_SolicitanteId",
                        column: x => x.SolicitanteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesSolicitud",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudId = table.Column<int>(type: "int", nullable: false),
                    RecursoId = table.Column<int>(type: "int", nullable: false),
                    FechaDevolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecibidoPorId = table.Column<int>(type: "int", nullable: true),
                    DevueltoConDano = table.Column<bool>(type: "bit", nullable: false),
                    ObservacionDevolucion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesSolicitud", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesSolicitud_Recursos_RecursoId",
                        column: x => x.RecursoId,
                        principalTable: "Recursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesSolicitud_Solicitudes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "Solicitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesSolicitud_Usuarios_RecibidoPorId",
                        column: x => x.RecibidoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prorrogas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudId = table.Column<int>(type: "int", nullable: false),
                    FechaFinAnterior = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFinSolicitada = table.Column<DateOnly>(type: "date", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EvaluadaPorId = table.Column<int>(type: "int", nullable: true),
                    FechaEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prorrogas", x => x.Id);
                    table.CheckConstraint("CK_Prorrogas_Fechas", "[FechaFinSolicitada] > [FechaFinAnterior]");
                    table.ForeignKey(
                        name: "FK_Prorrogas_Solicitudes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "Solicitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prorrogas_Usuarios_EvaluadaPorId",
                        column: x => x.EvaluadaPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Portátiles" },
                    { 2, "Cámaras" },
                    { 3, "Instrumentos" },
                    { 4, "Audiovisuales" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Correo", "Documento", "FechaRegistro", "Nombre", "PasswordHash", "Rol", "TipoSolicitante" },
                values: new object[,]
                {
                    { 1, true, "admin@prestamos.test", "900000001", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Administrador del Sistema", "AQAAAAIAAYagAAAAENfOGNBdii5MF2zYkWtL+Ki0wurmwOKt1e1s7NAZmNQNFmptK0HDNcVT7JP1ztvZMw==", "Administrador", null },
                    { 2, true, "tecnico@prestamos.test", "900000002", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Carlos Ruiz", "AQAAAAIAAYagAAAAENfOGNBdii5MF2zYkWtL+Ki0wurmwOKt1e1s7NAZmNQNFmptK0HDNcVT7JP1ztvZMw==", "Tecnico", null },
                    { 3, true, "coordinacion@prestamos.test", "900000003", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Laura Gómez", "AQAAAAIAAYagAAAAENfOGNBdii5MF2zYkWtL+Ki0wurmwOKt1e1s7NAZmNQNFmptK0HDNcVT7JP1ztvZMw==", "Coordinacion", null },
                    { 4, true, "ana.perez@prestamos.test", "1000000001", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Ana Pérez", "AQAAAAIAAYagAAAAENfOGNBdii5MF2zYkWtL+Ki0wurmwOKt1e1s7NAZmNQNFmptK0HDNcVT7JP1ztvZMw==", "Solicitante", "Estudiante" },
                    { 5, true, "juan.martinez@prestamos.test", "1000000002", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Juan Martínez", "AQAAAAIAAYagAAAAENfOGNBdii5MF2zYkWtL+Ki0wurmwOKt1e1s7NAZmNQNFmptK0HDNcVT7JP1ztvZMw==", "Solicitante", "Docente" }
                });

            migrationBuilder.InsertData(
                table: "Recursos",
                columns: new[] { "Id", "CategoriaId", "Codigo", "Descripcion", "EstadoOperativo", "FechaRegistro", "Nombre", "Ubicacion" },
                values: new object[,]
                {
                    { 1, 1, "PT-001", null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Portátil HP ProBook 450", "Bloque P, sala 201" },
                    { 2, 1, "PT-002", null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Portátil Lenovo ThinkPad E14", "Bloque P, sala 201" },
                    { 3, 1, "PT-003", null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Portátil Dell Latitude 3440", "Bloque P, sala 201" },
                    { 4, 2, "CM-001", null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Cámara Canon EOS R50", "Bloque L, almacén audiovisual" },
                    { 5, 2, "CM-002", null, "Danado", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Cámara Sony Alpha a6400", "Bloque L, almacén audiovisual" },
                    { 6, 2, "CM-003", null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Cámara Nikon Z50", "Bloque L, almacén audiovisual" },
                    { 7, 3, "IN-001", null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Multímetro digital Fluke 115", "Laboratorio de electrónica" },
                    { 8, 3, "IN-002", null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Osciloscopio Rigol DS1054Z", "Laboratorio de electrónica" },
                    { 9, 3, "IN-003", null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Kit Arduino Uno", "Laboratorio de electrónica" },
                    { 10, 4, "AV-001", null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Videoproyector Epson PowerLite", "Bloque L, almacén audiovisual" },
                    { 11, 4, "AV-002", null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Micrófono inalámbrico Shure", "Bloque L, almacén audiovisual" },
                    { 12, 4, "AV-003", null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Trípode Manfrotto", "Bloque L, almacén audiovisual" }
                });

            migrationBuilder.InsertData(
                table: "Solicitudes",
                columns: new[] { "Id", "EntregadaPorId", "Estado", "EvaluadaPorId", "FechaEntrega", "FechaEvaluacion", "FechaFin", "FechaInicio", "FechaRegistro", "Motivo", "MotivoRechazo", "SolicitanteId" },
                values: new object[,]
                {
                    { 1, 2, "Finalizada", 2, new DateTime(2026, 11, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 11, 2, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateOnly(2026, 11, 6), new DateOnly(2026, 11, 3), new DateTime(2026, 11, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), "Desarrollo del proyecto de Aplicaciones y Servicios Web", null, 4 },
                    { 2, 2, "Entregada", 2, new DateTime(2026, 11, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 11, 9, 8, 30, 0, 0, DateTimeKind.Unspecified), new DateOnly(2026, 11, 14), new DateOnly(2026, 11, 10), new DateTime(2026, 11, 8, 11, 0, 0, 0, DateTimeKind.Unspecified), "Grabación del video institucional del semillero", null, 4 },
                    { 3, 2, "Entregada", 2, new DateTime(2026, 11, 12, 7, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 11, 10, 15, 0, 0, 0, DateTimeKind.Unspecified), new DateOnly(2026, 12, 12), new DateOnly(2026, 11, 12), new DateTime(2026, 11, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), "Clases del curso de Redes de Computadores", null, 5 },
                    { 4, null, "Aprobada", 2, null, new DateTime(2026, 11, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateOnly(2026, 11, 28), new DateOnly(2026, 11, 25), new DateTime(2026, 11, 13, 10, 0, 0, 0, DateTimeKind.Unspecified), "Práctica de laboratorio de Electrónica Digital", null, 4 },
                    { 5, null, "Pendiente", null, null, null, new DateOnly(2026, 12, 5), new DateOnly(2026, 12, 1), new DateTime(2026, 11, 15, 8, 0, 0, 0, DateTimeKind.Unspecified), "Registro fotográfico de la feria de proyectos", null, 5 },
                    { 6, null, "Rechazada", 2, null, new DateTime(2026, 11, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateOnly(2026, 11, 22), new DateOnly(2026, 11, 20), new DateTime(2026, 11, 9, 16, 0, 0, 0, DateTimeKind.Unspecified), "Grabación de entrevistas para el curso de Comunicación", "El recurso CM-002 fue reportado como dañado.", 4 },
                    { 7, null, "Cancelada", null, null, null, new DateOnly(2026, 11, 20), new DateOnly(2026, 11, 18), new DateTime(2026, 11, 11, 10, 0, 0, 0, DateTimeKind.Unspecified), "Demostración en el curso de Instrumentación", null, 5 },
                    { 8, null, "NoRecogida", 2, null, new DateTime(2026, 11, 8, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateOnly(2026, 11, 12), new DateOnly(2026, 11, 11), new DateTime(2026, 11, 7, 12, 0, 0, 0, DateTimeKind.Unspecified), "Exposición final de Inglés 4", null, 4 }
                });

            migrationBuilder.InsertData(
                table: "DetallesSolicitud",
                columns: new[] { "Id", "DevueltoConDano", "FechaDevolucion", "ObservacionDevolucion", "RecibidoPorId", "RecursoId", "SolicitudId" },
                values: new object[,]
                {
                    { 1, false, new DateTime(2026, 11, 6, 16, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 1, 1 },
                    { 2, false, null, null, null, 4, 2 },
                    { 3, false, null, null, null, 2, 3 },
                    { 4, false, null, null, null, 10, 3 },
                    { 5, false, null, null, null, 7, 4 },
                    { 6, false, null, null, null, 6, 5 },
                    { 7, false, null, null, null, 12, 5 },
                    { 8, false, null, null, null, 5, 6 },
                    { 9, false, null, null, null, 8, 7 },
                    { 10, false, null, null, null, 11, 8 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nombre",
                table: "Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesSolicitud_RecibidoPorId",
                table: "DetallesSolicitud",
                column: "RecibidoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesSolicitud_RecursoId",
                table: "DetallesSolicitud",
                column: "RecursoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesSolicitud_SolicitudId_RecursoId",
                table: "DetallesSolicitud",
                columns: new[] { "SolicitudId", "RecursoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_Entidad_EntidadId",
                table: "Movimientos",
                columns: new[] { "Entidad", "EntidadId" });

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_UsuarioId",
                table: "Movimientos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Prorrogas_EvaluadaPorId",
                table: "Prorrogas",
                column: "EvaluadaPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Prorrogas_SolicitudId",
                table: "Prorrogas",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_Recursos_CategoriaId",
                table: "Recursos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Recursos_Codigo",
                table: "Recursos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_EntregadaPorId",
                table: "Solicitudes",
                column: "EntregadaPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_Estado_FechaInicio_FechaFin",
                table: "Solicitudes",
                columns: new[] { "Estado", "FechaInicio", "FechaFin" });

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_EvaluadaPorId",
                table: "Solicitudes",
                column: "EvaluadaPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_SolicitanteId",
                table: "Solicitudes",
                column: "SolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Documento",
                table: "Usuarios",
                column: "Documento",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesSolicitud");

            migrationBuilder.DropTable(
                name: "Movimientos");

            migrationBuilder.DropTable(
                name: "Prorrogas");

            migrationBuilder.DropTable(
                name: "Recursos");

            migrationBuilder.DropTable(
                name: "Solicitudes");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
