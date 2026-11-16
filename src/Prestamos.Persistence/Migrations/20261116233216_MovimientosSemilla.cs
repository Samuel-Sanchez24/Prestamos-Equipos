using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Prestamos.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MovimientosSemilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Movimientos",
                columns: new[] { "Id", "Entidad", "EntidadId", "EstadoAnterior", "EstadoNuevo", "Fecha", "Observacion", "UsuarioId" },
                values: new object[,]
                {
                    { 1001, "Recurso", 1, null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Recurso registrado", 1 },
                    { 1002, "Recurso", 2, null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Recurso registrado", 1 },
                    { 1003, "Recurso", 3, null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Recurso registrado", 1 },
                    { 1004, "Recurso", 4, null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Recurso registrado", 1 },
                    { 1005, "Recurso", 5, null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Recurso registrado", 1 },
                    { 1006, "Recurso", 6, null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Recurso registrado", 1 },
                    { 1007, "Recurso", 7, null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Recurso registrado", 1 },
                    { 1008, "Recurso", 8, null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Recurso registrado", 1 },
                    { 1009, "Recurso", 9, null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Recurso registrado", 1 },
                    { 1010, "Recurso", 10, null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Recurso registrado", 1 },
                    { 1011, "Recurso", 11, null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Recurso registrado", 1 },
                    { 1012, "Recurso", 12, null, "Disponible", new DateTime(2026, 11, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Recurso registrado", 1 },
                    { 1013, "Recurso", 5, "Disponible", "Danado", new DateTime(2026, 11, 5, 10, 0, 0, 0, DateTimeKind.Unspecified), "Lente con hongos", 2 },
                    { 1014, "Solicitud", 1, null, "Pendiente", new DateTime(2026, 11, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), "Solicitud registrada", 4 },
                    { 1015, "Solicitud", 1, "Pendiente", "Aprobada", new DateTime(2026, 11, 2, 14, 0, 0, 0, DateTimeKind.Unspecified), "Solicitud aprobada", 2 },
                    { 1016, "Solicitud", 1, "Aprobada", "Entregada", new DateTime(2026, 11, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), "Recursos entregados", 2 },
                    { 1017, "Solicitud", 1, "Entregada", "Finalizada", new DateTime(2026, 11, 6, 16, 0, 0, 0, DateTimeKind.Unspecified), "Devuelto PT-001: Sin novedad", 2 },
                    { 1018, "Solicitud", 2, null, "Pendiente", new DateTime(2026, 11, 8, 11, 0, 0, 0, DateTimeKind.Unspecified), "Solicitud registrada", 4 },
                    { 1019, "Solicitud", 2, "Pendiente", "Aprobada", new DateTime(2026, 11, 9, 8, 30, 0, 0, DateTimeKind.Unspecified), "Solicitud aprobada", 2 },
                    { 1020, "Solicitud", 2, "Aprobada", "Entregada", new DateTime(2026, 11, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), "Recursos entregados", 2 },
                    { 1021, "Solicitud", 3, null, "Pendiente", new DateTime(2026, 11, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), "Solicitud registrada", 5 },
                    { 1022, "Solicitud", 3, "Pendiente", "Aprobada", new DateTime(2026, 11, 10, 15, 0, 0, 0, DateTimeKind.Unspecified), "Solicitud aprobada", 2 },
                    { 1023, "Solicitud", 3, "Aprobada", "Entregada", new DateTime(2026, 11, 12, 7, 30, 0, 0, DateTimeKind.Unspecified), "Recursos entregados", 2 },
                    { 1024, "Solicitud", 4, null, "Pendiente", new DateTime(2026, 11, 13, 10, 0, 0, 0, DateTimeKind.Unspecified), "Solicitud registrada", 4 },
                    { 1025, "Solicitud", 4, "Pendiente", "Aprobada", new DateTime(2026, 11, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), "Solicitud aprobada", 2 },
                    { 1026, "Solicitud", 5, null, "Pendiente", new DateTime(2026, 11, 15, 8, 0, 0, 0, DateTimeKind.Unspecified), "Solicitud registrada", 5 },
                    { 1027, "Solicitud", 6, null, "Pendiente", new DateTime(2026, 11, 9, 16, 0, 0, 0, DateTimeKind.Unspecified), "Solicitud registrada", 4 },
                    { 1028, "Solicitud", 6, "Pendiente", "Rechazada", new DateTime(2026, 11, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), "El recurso CM-002 fue reportado como dañado.", 2 },
                    { 1029, "Solicitud", 7, null, "Pendiente", new DateTime(2026, 11, 11, 10, 0, 0, 0, DateTimeKind.Unspecified), "Solicitud registrada", 5 },
                    { 1030, "Solicitud", 7, "Pendiente", "Cancelada", new DateTime(2026, 11, 12, 9, 0, 0, 0, DateTimeKind.Unspecified), "Cancelada por el solicitante", 5 },
                    { 1031, "Solicitud", 8, null, "Pendiente", new DateTime(2026, 11, 7, 12, 0, 0, 0, DateTimeKind.Unspecified), "Solicitud registrada", 4 },
                    { 1032, "Solicitud", 8, "Pendiente", "Aprobada", new DateTime(2026, 11, 8, 9, 0, 0, 0, DateTimeKind.Unspecified), "Solicitud aprobada", 2 },
                    { 1033, "Solicitud", 8, "Aprobada", "NoRecogida", new DateTime(2026, 11, 11, 17, 0, 0, 0, DateTimeKind.Unspecified), "El solicitante no recogió los recursos", 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1005);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1006);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1007);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1008);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1009);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1010);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1011);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1012);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1013);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1014);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1015);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1016);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1017);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1018);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1019);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1020);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1021);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1022);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1023);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1024);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1025);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1026);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1027);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1028);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1029);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1030);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1031);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1032);

            migrationBuilder.DeleteData(
                table: "Movimientos",
                keyColumn: "Id",
                keyValue: 1033);
        }
    }
}
