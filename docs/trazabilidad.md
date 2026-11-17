# Matriz de trazabilidad

Relaciona cada condición de aceptación con sus requisitos, el código que la implementa, el pull request en que se integró y las pruebas que la demuestran.

Las pruebas **P-01 a P-12** son las mínimas de la guía; el detalle de cada una está en [`pruebas.md`](pruebas.md).

## Condiciones de aceptación

| Condición | Requisito aprobado | Implementación (ruta / componente) | Pull request | Prueba |
|---|---|---|---|---|
| **CA-01** Consultar recursos y disponibilidad | RF-04, RF-05, RF-06, RN-01, RN-02, RN-15 | `GET /api/recursos`, `GET /api/recursos/{id}`, `PATCH /api/recursos/{id}/estado` · `RecursoServicio` · `RecursoRepositorio.ObtenerOcupadosAsync` · `Recurso.MarcarDanado()` | #12 | P-01, P-02 · [`evidencias/recursos`](../evidencias/recursos) |
| **CA-02** Registrar solicitud | RF-07, RF-08, RF-09, RN-03 a RN-06, RN-08 | `POST /api/solicitudes`, `GET /api/solicitudes`, `GET /api/solicitudes/{id}`, `POST /api/solicitudes/{id}/cancelar` · `SolicitudServicio` · `CrearSolicitudRequestValidator` · `Solicitud.Cancelar()` | #13 | P-03, P-04 · [`evidencias/solicitudes`](../evidencias/solicitudes) |
| **CA-03** Evaluar y gestionar solicitudes | RF-10, RF-11, RF-13, RN-09, RN-11, S-09 | `POST /api/solicitudes/{id}/aprobar`, `/rechazar`, `/no-recogida` · `GestionSolicitudServicio` · `Solicitud.Aprobar()`, `Rechazar()`, `MarcarNoRecogida()` | #15 | P-05, P-06 · [`evidencias/gestion`](../evidencias/gestion) 01–08, 19 |
| **CA-04** Asignar y entregar respetando la disponibilidad | RF-07, RF-12, RN-02, RN-07, RN-10, RNF-04 | `POST /api/solicitudes`, `POST /api/solicitudes/{id}/entregar` · `RecursoRepositorio.BloquearAsync` (`UPDLOCK, HOLDLOCK`) · `UnidadDeTrabajo.EnTransaccionAsync` · `Solicitud.Entregar()` | #13, #15 | P-07, P-08 · [`evidencias/solicitudes`](../evidencias/solicitudes) 03, 07 · [`evidencias/gestion`](../evidencias/gestion) 09–13 |
| **CA-05** Devolución e historial | RF-14 a RF-17, RF-19, RN-12 a RN-15, RN-18 | `POST /api/solicitudes/{id}/devoluciones`, `GET /api/solicitudes/{id}/movimientos`, `POST /api/solicitudes/{id}/prorrogas`, `/api/prorrogas/*` · `Solicitud.RegistrarDevolucion()` · `ProrrogaServicio` · tabla `Movimientos` | #15, #16 | P-09, P-10 · [`evidencias/gestion`](../evidencias/gestion) 14–20 · [`evidencias/prorrogas`](../evidencias/prorrogas) |
| **CA-06** Información consolidada | RF-18, RF-19, RN-17, S-07 | `GET /api/reportes/resumen`, `GET /api/reportes/prestamos` · `ReporteServicio` · `ReporteRepositorio` | #17 | P-11 · [`evidencias/reportes`](../evidencias/reportes) |

## Pantallas del frontend por condición

| Condición | Pantalla (`src/Prestamos.Web`) | Prueba |
|---|---|---|
| CA-01 | `Pages/Catalogo/Catalogo.razor` (`/catalogo`) | P-12 · `integracion/02`, `06` |
| CA-02 | `Catalogo.razor` (envío) · `Pages/Solicitudes/Solicitudes.razor` · `DetalleSolicitud.razor` (cancelar) | P-12 · `integracion/03` |
| CA-03 | `Pages/Tecnico/Bandeja.razor` (panel lateral: aprobar, rechazar) · `DetalleSolicitud.razor` | P-12 · `integracion/04` |
| CA-04 | `Bandeja.razor` (panel lateral: entregar, no recogida) · `DetalleSolicitud.razor` | P-12 · `integracion/05`, `06` |
| CA-05 | `Bandeja.razor` (panel lateral: devolución por recurso) · `DetalleSolicitud.razor` (prórroga, historial) · `Pages/Tecnico/Prorrogas.razor` | P-12 · `integracion/07`, `08` |
| CA-06 | `Pages/Coordinacion/Panel.razor` (`/coordinacion`) | P-12 · `integracion/09` |

Componentes comunes: `Services/ApiCliente.cs` (comunicación con la API y errores ProblemDetails), `Services/SesionUsuario.cs` (token JWT y rol), `Components/Protegida.razor` (acceso por rol en la interfaz).

## Requisitos transversales

| Requisito | Implementación | Pull request | Prueba |
|---|---|---|---|
| RF-01 a RF-03: registro, login y personal | `AuthController`, `UsuariosController` · `AuthServicio`, `UsuarioServicio` · `GeneradorTokenJwt`, `HashContrasena` | #11 | [`evidencias/auth`](../evidencias/auth) 01–09 |
| RNF-01, RNF-02: autenticación y autorización | JWT con roles · `[Authorize(Roles = ...)]` · verificación de propiedad en los servicios (`AccesoDenegadoException`) | #11 | P-06 · [`evidencias/auth`](../evidencias/auth) 06, 09 |
| RNF-03, RNF-05: validación y errores | FluentValidation · `ManejadorExcepciones` → ProblemDetails (400/401/403/404/409) | #10, #11 | P-02, P-04 |
| RNF-07: reproducibilidad | Migraciones EF Core · `DatosSemilla` · README §5 | #9 | [`evidencias/persistencia`](../evidencias/persistencia) |
| RNF-08: arquitectura en capas | Solución Domain / Application / Contracts / Persistence / Api / Web | #8 | — |
| RNF-09: hora de Colombia | `RelojColombia` (UTC−5) | #11 | — |
| RNF-10: sin credenciales en el repositorio | Clave JWT en secretos de usuario · `.gitignore` · `.env.example` | #11 | — |
