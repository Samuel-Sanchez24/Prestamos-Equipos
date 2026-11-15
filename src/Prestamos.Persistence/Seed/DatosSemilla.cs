using Microsoft.EntityFrameworkCore;
using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;

namespace Prestamos.Persistence.Seed;

public static class DatosSemilla
{
    private const string HashClaveDemo =
        "AQAAAAIAAYagAAAAENfOGNBdii5MF2zYkWtL+Ki0wurmwOKt1e1s7NAZmNQNFmptK0HDNcVT7JP1ztvZMw==";

    private static readonly DateTime FechaBase = new(2026, 11, 1, 8, 0, 0);

    public static void Sembrar(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().HasData(Usuarios());
        modelBuilder.Entity<Categoria>().HasData(Categorias());
        modelBuilder.Entity<Recurso>().HasData(Recursos());
        modelBuilder.Entity<Solicitud>().HasData(Solicitudes());
        modelBuilder.Entity<DetalleSolicitud>().HasData(Detalles());
    }

    private static Usuario[] Usuarios() =>
    [
        new() { Id = 1, Nombre = "Administrador del Sistema", Documento = "900000001", Correo = "admin@prestamos.test",
                PasswordHash = HashClaveDemo, Rol = Rol.Administrador, Activo = true, FechaRegistro = FechaBase },
        new() { Id = 2, Nombre = "Carlos Ruiz", Documento = "900000002", Correo = "tecnico@prestamos.test",
                PasswordHash = HashClaveDemo, Rol = Rol.Tecnico, Activo = true, FechaRegistro = FechaBase },
        new() { Id = 3, Nombre = "Laura Gómez", Documento = "900000003", Correo = "coordinacion@prestamos.test",
                PasswordHash = HashClaveDemo, Rol = Rol.Coordinacion, Activo = true, FechaRegistro = FechaBase },
        new() { Id = 4, Nombre = "Ana Pérez", Documento = "1000000001", Correo = "ana.perez@prestamos.test",
                PasswordHash = HashClaveDemo, Rol = Rol.Solicitante, TipoSolicitante = TipoSolicitante.Estudiante,
                Activo = true, FechaRegistro = FechaBase },
        new() { Id = 5, Nombre = "Juan Martínez", Documento = "1000000002", Correo = "juan.martinez@prestamos.test",
                PasswordHash = HashClaveDemo, Rol = Rol.Solicitante, TipoSolicitante = TipoSolicitante.Docente,
                Activo = true, FechaRegistro = FechaBase }
    ];

    private static Categoria[] Categorias() =>
    [
        new() { Id = 1, Nombre = "Portátiles" },
        new() { Id = 2, Nombre = "Cámaras" },
        new() { Id = 3, Nombre = "Instrumentos" },
        new() { Id = 4, Nombre = "Audiovisuales" }
    ];
    private static Recurso[] Recursos() =>
    [
        R(1,  "PT-001", "Portátil HP ProBook 450",       1, "Bloque P, sala 201"),
        R(2,  "PT-002", "Portátil Lenovo ThinkPad E14",  1, "Bloque P, sala 201"),
        R(3,  "PT-003", "Portátil Dell Latitude 3440",   1, "Bloque P, sala 201"),
        R(4,  "CM-001", "Cámara Canon EOS R50",          2, "Bloque L, almacén audiovisual"),
        R(5,  "CM-002", "Cámara Sony Alpha a6400",       2, "Bloque L, almacén audiovisual", EstadoRecurso.Danado),
        R(6,  "CM-003", "Cámara Nikon Z50",              2, "Bloque L, almacén audiovisual"),
        R(7,  "IN-001", "Multímetro digital Fluke 115",  3, "Laboratorio de electrónica"),
        R(8,  "IN-002", "Osciloscopio Rigol DS1054Z",    3, "Laboratorio de electrónica"),
        R(9,  "IN-003", "Kit Arduino Uno",               3, "Laboratorio de electrónica"),
        R(10, "AV-001", "Videoproyector Epson PowerLite", 4, "Bloque L, almacén audiovisual"),
        R(11, "AV-002", "Micrófono inalámbrico Shure",   4, "Bloque L, almacén audiovisual"),
        R(12, "AV-003", "Trípode Manfrotto",             4, "Bloque L, almacén audiovisual")
    ];

    private static Recurso R(int id, string codigo, string nombre, int categoriaId, string ubicacion,
                             EstadoRecurso estado = EstadoRecurso.Disponible) =>
        new()
        {
            Id = id,
            Codigo = codigo,
            Nombre = nombre,
            CategoriaId = categoriaId,
            Ubicacion = ubicacion,
            EstadoOperativo = estado,
            FechaRegistro = FechaBase
        };

    private static Solicitud[] Solicitudes() =>
    [
        // 1. Finalizada: ciclo completo
        new() { Id = 1, SolicitanteId = 4, FechaInicio = new(2026, 11, 3), FechaFin = new(2026, 11, 6),
                Motivo = "Desarrollo del proyecto de Aplicaciones y Servicios Web", Estado = EstadoSolicitud.Finalizada,
                FechaRegistro = new(2026, 11, 2, 10, 0, 0),
                EvaluadaPorId = 2, FechaEvaluacion = new(2026, 11, 2, 14, 0, 0),
                EntregadaPorId = 2, FechaEntrega = new(2026, 11, 3, 9, 0, 0) },

        // 2. Entregada y vencida (fecha fin superada sin devolución)
        new() { Id = 2, SolicitanteId = 4, FechaInicio = new(2026, 11, 10), FechaFin = new(2026, 11, 14),
                Motivo = "Grabación del video institucional del semillero", Estado = EstadoSolicitud.Entregada,
                FechaRegistro = new(2026, 11, 8, 11, 0, 0),
                EvaluadaPorId = 2, FechaEvaluacion = new(2026, 11, 9, 8, 30, 0),
                EntregadaPorId = 2, FechaEntrega = new(2026, 11, 10, 8, 0, 0) },

        // 3. Entregada y activa (docente, dos recursos)
        new() { Id = 3, SolicitanteId = 5, FechaInicio = new(2026, 11, 12), FechaFin = new(2026, 12, 12),
                Motivo = "Clases del curso de Redes de Computadores", Estado = EstadoSolicitud.Entregada,
                FechaRegistro = new(2026, 11, 10, 9, 0, 0),
                EvaluadaPorId = 2, FechaEvaluacion = new(2026, 11, 10, 15, 0, 0),
                EntregadaPorId = 2, FechaEntrega = new(2026, 11, 12, 7, 30, 0) },

        // 4. Aprobada, pendiente de entrega
        new() { Id = 4, SolicitanteId = 4, FechaInicio = new(2026, 11, 25), FechaFin = new(2026, 11, 28),
                Motivo = "Práctica de laboratorio de Electrónica Digital", Estado = EstadoSolicitud.Aprobada,
                FechaRegistro = new(2026, 11, 13, 10, 0, 0),
                EvaluadaPorId = 2, FechaEvaluacion = new(2026, 11, 14, 9, 0, 0) },

        // 5. Pendiente de evaluación (docente, dos recursos)
        new() { Id = 5, SolicitanteId = 5, FechaInicio = new(2026, 12, 1), FechaFin = new(2026, 12, 5),
                Motivo = "Registro fotográfico de la feria de proyectos", Estado = EstadoSolicitud.Pendiente,
                FechaRegistro = new(2026, 11, 15, 8, 0, 0) },

        // 6. Rechazada: pedía un recurso dañado
        new() { Id = 6, SolicitanteId = 4, FechaInicio = new(2026, 11, 20), FechaFin = new(2026, 11, 22),
                Motivo = "Grabación de entrevistas para el curso de Comunicación", Estado = EstadoSolicitud.Rechazada,
                FechaRegistro = new(2026, 11, 9, 16, 0, 0),
                EvaluadaPorId = 2, FechaEvaluacion = new(2026, 11, 10, 8, 0, 0),
                MotivoRechazo = "El recurso CM-002 fue reportado como dañado." },

        // 7. Cancelada por el solicitante
        new() { Id = 7, SolicitanteId = 5, FechaInicio = new(2026, 11, 18), FechaFin = new(2026, 11, 20),
                Motivo = "Demostración en el curso de Instrumentación", Estado = EstadoSolicitud.Cancelada,
                FechaRegistro = new(2026, 11, 11, 10, 0, 0) },

        // 8. No recogida
        new() { Id = 8, SolicitanteId = 4, FechaInicio = new(2026, 11, 11), FechaFin = new(2026, 11, 12),
                Motivo = "Exposición final de Inglés 4", Estado = EstadoSolicitud.NoRecogida,
                FechaRegistro = new(2026, 11, 7, 12, 0, 0),
                EvaluadaPorId = 2, FechaEvaluacion = new(2026, 11, 8, 9, 0, 0) }
    ];

    private static DetalleSolicitud[] Detalles() =>
    [
        new() { Id = 1,  SolicitudId = 1, RecursoId = 1,
                FechaDevolucion = new(2026, 11, 6, 16, 0, 0), RecibidoPorId = 2, DevueltoConDano = false },
        new() { Id = 2,  SolicitudId = 2, RecursoId = 4 },
        new() { Id = 3,  SolicitudId = 3, RecursoId = 2 },
        new() { Id = 4,  SolicitudId = 3, RecursoId = 10 },
        new() { Id = 5,  SolicitudId = 4, RecursoId = 7 },
        new() { Id = 6,  SolicitudId = 5, RecursoId = 6 },
        new() { Id = 7,  SolicitudId = 5, RecursoId = 12 },
        new() { Id = 8,  SolicitudId = 6, RecursoId = 5 },
        new() { Id = 9,  SolicitudId = 7, RecursoId = 8 },
        new() { Id = 10, SolicitudId = 8, RecursoId = 11 }
    ];
}