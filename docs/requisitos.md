# Requisitos — Gestión de préstamos de equipos y recursos

| Campo | Valor |
|---|---|
| Proyecto | Proyecto Final 03 (PF-03) |
| Asignatura | Aplicaciones y Servicios Web — ITM |
| Estudiantes | Samuel Andrés Sánchez Orrego · Dani Moreno Ciro |
| Versión | 0.2 (incorpora resultados de la entrevista) |
| Estado | Pendiente de aprobación del docente |

---

## 1. Contexto

Una institución educativa dispone de computadores portátiles, cámaras, instrumentos y otros recursos que docentes y estudiantes utilizan temporalmente para actividades académicas. El personal responsable de su administración y custodia gestiona los préstamos.

### Situación actual
- Las solicitudes y entregas se registran en formularios, hojas de cálculo o comunicaciones independientes.
- No es posible saber con certeza qué recursos están disponibles, cuáles están prestados y quién debe devolverlos.
- Se reciben solicitudes simultáneas sobre un mismo recurso, sin un mecanismo para resolverlas.
- Es difícil verificar las fechas y las condiciones de devolución.

## 2. Problema

La institución no cuenta con un registro único y confiable del ciclo de préstamo de sus recursos. Esto genera:

1. Falta de visibilidad sobre la disponibilidad real de cada recurso.
2. Asignaciones incompatibles: un mismo recurso comprometido con más de una persona.
3. Responsabilidad difusa: no siempre se sabe quién tiene un recurso ni cuándo debe devolverlo.
4. Ausencia de historial para supervisar el uso de los recursos.

## 3. Objetivo de la solución

Desarrollar una aplicación web que organice y dé seguimiento al préstamo temporal de recursos, desde la consulta de disponibilidad y la reserva hasta la asignación, el uso y la devolución. La aplicación debe conservar la información necesaria para identificar responsables y evitar asignaciones incompatibles.

## 4. Actores

| Actor | Descripción | Responsabilidades en el proceso |
|---|---|---|
| **Solicitante** | Estudiante o docente. Se registra por su cuenta. | Consultar disponibilidad, registrar y cancelar solicitudes, pedir prórrogas y consultar su historial. |
| **Técnico** (responsable de préstamos) | Personal encargado de la custodia de los recursos. | Evaluar solicitudes, entregar recursos, registrar no recogidas y devoluciones, gestionar prórrogas y el estado físico de los recursos. |
| **Coordinación** | Personal que supervisa el uso de los recursos. | Consultar información consolidada de préstamos, recursos utilizados e historial. |
| **Administrador** | Gestiona el acceso del personal. | Registrar técnicos y coordinadores, y administrar el catálogo de recursos. |

(El actor **Administrador** surge de la entrevista (P19): el personal no se autorregistra.)

## 5. Alcance

### 5.1 Proceso cubierto

```
Consulta de        Reserva            Evaluación          Entrega          Seguimiento        Devolución
disponibilidad ──▶ (solicitud)   ──▶  (técnico)     ──▶   (préstamo)  ──▶  y prórroga    ──▶  e historial
   (CA-01)          (CA-02)            (CA-03)             (CA-04)          (CA-05)            (CA-05)

                     Coordinación: consulta consolidada de todo el proceso (CA-06)
```

### 5.2 Condiciones de aceptación obligatorias

| ID | Condición | Demostración esperada |
|---|---|---|
| CA-01 | Los usuarios autorizados pueden consultar los recursos y conocer su disponibilidad. | Se consultan recursos almacenados, con disponibilidad coherente con las asignaciones registradas. |
| CA-02 | Un solicitante puede registrar una solicitud de préstamo. | La solicitud queda persistida y puede recuperarse desde la aplicación. |
| CA-03 | El personal responsable puede evaluar y gestionar solicitudes. | Se registra una decisión o actuación válida conforme a las reglas aprobadas. |
| CA-04 | El sistema permite registrar la asignación y entrega de un recurso, respetando su disponibilidad. | Se identifica recurso, solicitante y operación; se impiden asignaciones incompatibles. |
| CA-05 | Es posible registrar la devolución y consultar el historial de préstamos. | El cierre se conserva y el recurso actualiza su disponibilidad. |
| CA-06 | La coordinación puede consultar información consolidada sobre recursos y préstamos. | Consulta de múltiples recursos y operaciones, sus estados y responsables. |

### 5.3 Fuera del alcance

- Identificación de equipos con QR o RFID.
- Integración con inventarios institucionales externos.
- Notificaciones por correo o WhatsApp.
- Gestión de compras.
- Cálculo de sanciones económicas.
- Mantenimiento físico de los recursos (solo se registra que está dañado o reparado).

### 5.4 Restricciones técnicas

| ID | Restricción |
|---|---|
| RT-01 | La solución incluye interfaz web, backend con API REST y persistencia en base de datos. |
| RT-02 | Comunicación real entre interfaz, API y base de datos; no se aceptan datos simulados. |
| RT-03 | Las validaciones se aplican en el servidor, aunque la solicitud no provenga de la interfaz. |
| RT-04 | No se incluyen credenciales reales en el repositorio; se usan datos ficticios. |
| RT-05 | No se exige despliegue en la nube, Docker ni pruebas automatizadas. |

**Tecnologías seleccionadas por el equipo:** C# / ASP.NET Core Web API, SQL Server con Entity Framework Core y Arquitectura Limpia. (Se hara despues del que nos apruebe los requisitos)

---

## 6. Método de levantamiento

Entrevista semiestructurada con el docente en el rol de representante de la institución, realizada el 06/11/2026. Registro completo en [`entrevista.md`](entrevista.md).

## 7. Actores y permisos

| Acción | Solicitante | Técnico | Coordinación | Administrador |
|---|:-:|:-:|:-:|:-:|
| Registrarse en el sistema | **X** | — | — | — |
| Registrar personal (técnicos, coordinación) | — | — | — | **X** |
| Administrar el catálogo de recursos | — | — | — | **X** |
| Consultar recursos y disponibilidad | **X** | **X** | **X** | **X** |
| Marcar un recurso como dañado o reparado | — | **X** | — | — |
| Registrar y cancelar sus propias solicitudes | **X** | — | — | — |
| Solicitar prórroga de sus préstamos | **X** | — | — | — |
| Aprobar o rechazar solicitudes y prórrogas | — | **X** | — | — |
| Registrar entrega, no recogida y devolución | — | **X** | — | — |
| Consultar su propio historial | **X** | — | — | — |
| Consultar el historial de todos | — | **X** | **X** | **X** |
| Consultar información consolidada | — | **X** | **X** | **X** |

## 8. Reglas de negocio

### Recursos
| ID | Regla | Origen |
|---|---|---|
| RN-01 | Un recurso tiene un estado operativo: Disponible, Dañado o Dado de baja. Solo los recursos Disponibles pueden reservarse. | P03, P16 |
| RN-02 | La disponibilidad de un recurso en un rango de fechas se calcula: está disponible si su estado operativo es Disponible y no tiene reservas vigentes (Pendiente, Aprobada o Entregada) cuyas fechas se crucen con ese rango. | P04, CA-01 |

### Solicitud
| ID | Regla | Origen |
|---|---|---|
| RN-03 | Una solicitud incluye uno o varios recursos específicos, una fecha de inicio, una fecha de fin y el motivo de uso. | P05, P06, S-01 |
| RN-04 | La fecha de inicio no puede ser anterior a la fecha actual, y la fecha de fin debe ser posterior a la de inicio. | P04 |
| RN-05 | La duración máxima es de un semestre para estudiantes y de dos semestres para docentes (valor configurable). | P07, S-03 |
| RN-06 | No hay límite de recursos que una persona pueda tener al mismo tiempo. | P08 |
| RN-07 | Prioridad por orden de llegada: al registrar una solicitud, si algún recurso ya está reservado en fechas que se cruzan, la solicitud se rechaza en ese momento. Esta regla se cumple aun con solicitudes simultáneas. | P12, CA-04 |
| RN-08 | El solicitante puede cancelar su solicitud mientras esté Pendiente o Aprobada (antes de recibir el recurso). | P09 |

### Evaluación y entrega
| ID | Regla | Origen |
|---|---|---|
| RN-09 | Solo el técnico aprueba o rechaza solicitudes. Un rechazo debe indicar el motivo y solo procede por incumplimiento de reglas del sistema (por ejemplo, un recurso que quedó dañado). | P10, P11 |
| RN-10 | Solo se entregan solicitudes Aprobadas, a partir de su fecha de inicio. La entrega registra la fecha, la hora y el técnico. | P10, S-11 |
| RN-11 | Si el solicitante no recoge el recurso, el técnico marca la solicitud como No recogida, lo que libera las fechas reservadas. | P13 |

### Seguimiento y devolución
| ID | Regla | Origen |
|---|---|---|
| RN-12 | Un préstamo está*Vencido cuando pasa la fecha de fin sin haberse devuelto. Esto no impide hacer nuevas solicitudes. | P08, P14 |
| RN-13 | El solicitante puede pedir una prórroga. Se aprueba solo si la nueva fecha de fin no supera la duración máxima total y no se cruza con otra reserva del mismo recurso. | P14, S-10 |
| RN-14 | La devolución se registra por cada recurso, con fecha, hora, técnico y una observación opcional. La solicitud queda Finalizada cuando se han devuelto todos sus recursos. | P15, S-05 |
| RN-15 | Si un recurso se devuelve dañado, pasa a Dañado y deja de ser reservable hasta que el técnico lo marque como reparado. | P16 |

### Acceso y trazabilidad
| ID | Regla | Origen |
|---|---|---|
| RN-16 | Los solicitantes se autorregistran indicando si son estudiantes o docentes. Al personal lo registra únicamente el administrador. | P19, S-06 |
| RN-17 | Cada solicitante solo ve sus propias solicitudes. El técnico y Coordinación ven todas. Coordinación solo consulta. | P17, P18, S-07 |
| RN-18 | Todo cambio de estado de una solicitud, préstamo o recurso registra quién lo hizo, cuándo y el estado anterior y el nuevo. | CA-04, CA-05 |

### Ciclo de vida de la solicitud

```
                ┌──────────── Cancelada (solicitante)
                │                    ▲
Pendiente ──▶ Aprobada ──▶ Entregada ──▶ Finalizada
    │            │            │
    ▼            ▼            └─ (fecha fin superada) → se muestra como Vencida
Rechazada    No recogida
```

## 9. Requisitos funcionales

| ID | Requisito | Actor | CA |
|---|---|---|---|
| RF-01 | Registrar una cuenta de solicitante (nombre, documento, correo, tipo estudiante/docente y contraseña). | Solicitante | — |
| RF-02 | Iniciar sesión con correo y contraseña. | Todos | — |
| RF-03 | Registrar técnicos y coordinadores. | Administrador | — |
| RF-04 | Registrar, editar y dar de baja recursos (código, nombre, categoría, descripción y ubicación). | Administrador | CA-01 |
| RF-05 | Consultar el catálogo de recursos, filtrando por categoría y por disponibilidad en un rango de fechas. | Todos | CA-01 |
| RF-06 | Marcar un recurso como dañado o reparado. | Técnico | CA-01 |
| RF-07 | Registrar una solicitud con uno o varios recursos, fechas de inicio y fin, y motivo. | Solicitante | CA-02 |
| RF-08 | Consultar mis solicitudes y su estado actual. | Solicitante | CA-02 |
| RF-09 | Cancelar una solicitud propia antes de la entrega. | Solicitante | CA-02 |
| RF-10 | Listar las solicitudes pendientes en orden de llegada. | Técnico | CA-03 |
| RF-11 | Aprobar o rechazar una solicitud, indicando el motivo si es un rechazo. | Técnico | CA-03 |
| RF-12 | Registrar la entrega de una solicitud aprobada. | Técnico | CA-04 |
| RF-13 | Marcar una solicitud aprobada como no recogida. | Técnico | CA-03 |
| RF-14 | Solicitar la prórroga de un préstamo. | Solicitante | CA-05 |
| RF-15 | Aprobar o rechazar una prórroga. | Técnico | CA-05 |
| RF-16 | Registrar la devolución de cada recurso, con observación opcional e indicación de daño. | Técnico | CA-05 |
| RF-17 | Consultar el historial de préstamos: el solicitante ve el suyo; el técnico y Coordinación ven el de todos. | Todos | CA-05 |
| RF-18 | Consultar información consolidada: préstamos por estado (activos, vencidos, finalizados), recursos más utilizados e historial, con filtros por fechas, categoría y estado. | Coordinación, Técnico | CA-06 |
| RF-19 | Consultar el registro de movimientos (cambios de estado) de una solicitud o de un recurso. | Técnico, Coordinación | CA-05, CA-06 |

## 10. Requisitos no funcionales

| ID | Requisito |
|---|---|
| RNF-01 | **Seguridad:** autenticación con token (JWT) y contraseñas almacenadas con hash; nunca en texto plano. |
| RNF-02 | **Autorización:** cada servicio verifica el rol del usuario. Responde 401 si no hay sesión y 403 si no tiene permiso. |
| RNF-03 | **Validación:** todas las reglas de negocio se validan en el servidor, independientemente de la interfaz. |
| RNF-04 | **Consistencia:** dos solicitudes simultáneas nunca pueden reservar el mismo recurso en fechas que se crucen (control de concurrencia en la base de datos). |
| RNF-05 | **Errores:** la API responde con códigos HTTP coherentes (400, 401, 403, 404, 409) y mensajes comprensibles en español. |
| RNF-06 | **Usabilidad:** interfaz en español; muestra el estado de cada solicitud y los mensajes de error de forma clara. |
| RNF-07 | **Reproducibilidad:** la base de datos se crea con migraciones y datos de prueba ficticios; el README explica cómo ejecutar el proyecto. |
| RNF-08 | **Mantenibilidad:** arquitectura en capas (dominio, aplicación, infraestructura y API), con las reglas de negocio en el dominio. |
| RNF-09 | **Fechas:** las fechas y horas se manejan en la hora de Colombia (UTC-5). |
| RNF-10 | **Confidencialidad:** sin credenciales reales en el repositorio; la configuración sensible va fuera del código. |

## 11. Historias de usuario y criterios de aceptación

### HU-01 — Consultar disponibilidad (CA-01 · RF-05)
**Como** solicitante, **quiero** ver qué recursos están disponibles en unas fechas **para** saber qué puedo reservar.
- **Dado** un recurso reservado del 10 al 15, **cuando** consulto disponibilidad del 12 al 20, **entonces** aparece como no disponible.
- **Dado** un recurso Dañado, **cuando** consulto cualquier rango, **entonces** aparece como no disponible.

### HU-02 — Registrar solicitud (CA-02 · RF-07)
**Como** solicitante, **quiero** reservar uno o varios recursos para unas fechas **para** usarlos en mi actividad académica.
- **Dado** que todos los recursos están disponibles, **cuando** envío la solicitud, **entonces** queda *Pendiente* y la veo en "Mis solicitudes".
- **Dado** que un recurso ya está reservado en esas fechas, **cuando** envío la solicitud, **entonces** se rechaza indicando qué recurso no está disponible.
- **Dado** que soy estudiante, **cuando** pido un período mayor a un semestre, **entonces** el sistema no permite registrarla.
- **Dado** que la fecha de inicio ya pasó, **cuando** envío la solicitud, **entonces** el sistema no permite registrarla.

### HU-03 — Cancelar solicitud (CA-02 · RF-09)
**Como** solicitante, **quiero** cancelar una solicitud **para** liberar los recursos si ya no los necesito.
- **Dado** una solicitud *Pendiente* o *Aprobada*, **cuando** la cancelo, **entonces** queda *Cancelada* y los recursos vuelven a estar disponibles en esas fechas.
- **Dado** una solicitud *Entregada*, **cuando** intento cancelarla, **entonces** el sistema no lo permite.

### HU-04 — Evaluar solicitud (CA-03 · RF-10, RF-11)
**Como** técnico, **quiero** revisar las solicitudes pendientes en orden de llegada **para** aprobarlas o rechazarlas.
- **Dado** una solicitud Pendiente, **cuando** la apruebo, **entonces** queda Aprobada con mi usuario y la fecha.
- **Dado** que la rechazo sin motivo, **entonces** el sistema exige el motivo.
- **Dado** un solicitante, **cuando** intenta aprobar una solicitud, **entonces** recibe un error de permisos.

### HU-05 — Registrar entrega (CA-04 · RF-12)
**Como** técnico, **quiero** registrar la entrega de los recursos **para** dejar constancia de quién los tiene.
- **Dado** una solicitud Aprobada cuya fecha de inicio llegó, **cuando** registro la entrega, **entonces** queda Entregada con fecha, hora y técnico.
- **Dado** una solicitud Pendiente o Cancelada, **cuando** intento entregarla, **entonces** el sistema no lo permite.

### HU-06 — Marcar no recogida (CA-03 · RF-13)
**Como** técnico, **quiero** reportar que el solicitante no recogió los recursos **para** liberarlos.
- **Dado** una solicitud *Aprobada*, **cuando** la marco como no recogida, **entonces** cambia a *No recogida* y los recursos vuelven a estar disponibles.

### HU-07 — Solicitar y aprobar prórroga (CA-05 · RF-14, RF-15)
**Como** solicitante, **quiero** pedir más tiempo **para** no quedar con un préstamo vencido.
- **Dado** un préstamo *Entregado*, **cuando** pido una nueva fecha de fin dentro del máximo permitido y sin cruces, **entonces** queda pendiente de aprobación del técnico.
- **Dado** que la nueva fecha se cruza con otra reserva del recurso, **entonces** el sistema la rechaza.

### HU-08 — Registrar devolución (CA-05 · RF-16)
**Como** técnico, **quiero** registrar la devolución de cada recurso **para** cerrar el préstamo y actualizar la disponibilidad.
- **Dado** un préstamo Entregado, **cuando** registro la devolución de todos sus recursos, **entonces** la solicitud queda Finalizada.
- **Dado** que marco un recurso como dañado al devolverlo, **entonces** pasa a Dañado y no aparece como reservable.

### HU-09 — Consultar historial (CA-05 · RF-17)
**Como** solicitante, **quiero** ver mis préstamos anteriores **para** llevar control de lo que he usado.
- **Dado** que soy solicitante, **cuando** consulto el historial, **entonces** solo veo mis propias solicitudes.
- **Dado** que soy técnico, **cuando** consulto el historial, **entonces** veo las de todos los solicitantes.

### HU-10 — Consulta consolidada (CA-06 · RF-18)
**Como** coordinación, **quiero** ver un resumen de préstamos y recursos **para** supervisar su uso.
- **Dado** que existen préstamos en distintos estados, **cuando** consulto el resumen, **entonces** veo cuántos hay activos, vencidos y finalizados, y los recursos más utilizados.
- **Dado** que filtro por fechas o categoría, **entonces** el resumen se ajusta al filtro.
- **Dado** que soy coordinación, **cuando** intento aprobar una solicitud, **entonces** recibo un error de permisos.

### HU-11 — Registro y acceso (RF-01, RF-02, RF-03)
**Como** estudiante o docente, **quiero** crear mi cuenta **para** poder solicitar recursos.
- **Dado** un correo ya registrado, **cuando** intento registrarme, **entonces** el sistema lo indica.
- **Dado** un usuario no autenticado, **cuando** intenta usar un servicio protegido, **entonces** recibe un error 401.

### HU-12 — Gestionar el estado de los recursos (CA-01 · RF-04, RF-06)
**Como** técnico, **quiero** marcar un recurso como dañado o reparado **para** que la disponibilidad sea real.
- **Dado** un recurso Disponible, **cuando** lo marco como dañado, **entonces** deja de ser reservable.
- **Dado** un recurso Dañado, **cuando** lo marco como reparado, **entonces** vuelve a ser reservable.

## 12. Priorización (MoSCoW)

| Prioridad | Requisitos | Criterio |
|---|---|---|
| **Must** (imprescindible) | RF-01, RF-02, RF-03, RF-04, RF-05, RF-06, RF-07, RF-08, RF-09, RF-10, RF-11, RF-12, RF-13, RF-16, RF-17, RF-18 | Cubren CA-01 a CA-06 y el control de acceso. |
| **Should** (importante) | RF-14, RF-15, RF-19 | Acordadas con el docente, pero no son condición de aceptación. |
| **Could** (deseable) | Filtros adicionales y gráficos en la consulta consolidada | Solo si sobra tiempo. |

## 13. Supuestos del equipo

| ID   | Supuesto | Motivo | Confirmar |
|---|---|---|:-:|
| S-01 | El solicitante elige recursos específicos del catálogo (no una categoría genérica). | P02: el docente lo dejó como decisión de software. Simplifica el cálculo de disponibilidad. | — |
| S-02 | Datos de la solicitud: recursos, fecha de inicio, fecha de fin y motivo. | P06. | — |
| S-03 | Un semestre equivale a 180 días (estudiantes: 180; docentes: 360). El valor es configurable. | P07 no define la duración en días. | **x** |
| S-04 | El técnico aprueba y también entrega. | P10. | — |
| S-05 | La aprobación y la entrega se hacen por la solicitud completa; la devolución, por cada recurso. | Permite registrar el daño de un recurso puntual. | **x** |
| S-06 | El solicitante indica si es estudiante o docente al registrarse; el administrador puede corregirlo. | P19 no especifica cómo se verifica el tipo. | **x** |
| S-07 | Coordinación solo consulta; no aprueba ni gestiona. | La pregunta sobre sus permisos no se hizo. | — |
| S-08 | El administrador gestiona el catálogo de recursos. | No se preguntó quién registra los recursos. | — |
| S-09 | Si un recurso queda dañado y tiene reservas futuras, el técnico debe rechazarlas o cancelarlas; el sistema las señala. | P16 no cubre este caso. | — |
| S-10 | La prórroga la aprueba el técnico. | P14 no indica quién la aprueba. | **x** |
| S-11 | La entrega solo puede registrarse a partir de la fecha de inicio de la reserva. | Coherencia con la reserva anticipada (P04). | — |
| S-12 | Un préstamo vencido no impide hacer nuevas solicitudes. | P08 (sin límite) y P14. | — |


## 14. Aprobación

| Fecha | Versión | Aprobado por | Observaciones |
|---|---|---|---|
| | 0.2 | | |
