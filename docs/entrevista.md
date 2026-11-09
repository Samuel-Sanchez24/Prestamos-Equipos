# Registro de entrevista — Levantamiento de requisitos

| Campo | Valor |
|---|---|
| Fecha | 06/11/2026 |
| Entrevistado | Docente (rol: representante de la institución) |
| Entrevistadores | Samuel Andrés Sánchez Orrego · Dani Moreno Ciro |
| Método | Entrevista en lenguaje del negocio |

**Estado de cada respuesta:** **Acuerdo** (respuesta clara) 

**Pendiente** (queda por confirmar) 

**Supuesto** (no se definió; lo decide el equipo).

---

## 1. Los recursos

| # | Pregunta | Respuesta | Estado |
|---|---|---|---|
| P01 | ¿Qué tipos de equipos y recursos presta la institución? | Computadores portátiles, cámaras, instrumentos y otros recursos para fines académicos. | Acuerdo |
| P02 | Cuando alguien pide un recurso, ¿pide uno en particular o un tipo de recurso y ustedes eligen cuál entregar? | No es una decisión del cliente; es una decisión de software. | Supuesto (S-01) |
| P03 | ¿Hay recursos que en algún momento no se pueden prestar (dañados o en revisión)? | Es posible. | Acuerdo |

## 2. La solicitud

| # | Pregunta | Respuesta | Estado |
|---|---|---|---|
| P04 | ¿Los recursos se piden para uso inmediato o se pueden apartar con anticipación? | Se pueden pedir con anticipación. | Acuerdo |
| P05 | ¿Una persona puede pedir varios recursos en una misma solicitud? | Sí. | Acuerdo |
| P06 | ¿Qué información debe dar la persona al pedir un recurso? | Las que se describieron (para qué lo necesita, desde cuándo, hasta cuándo). | Acuerdo |
| P07 | ¿Cuánto tiempo máximo puede alguien tener un recurso? ¿Igual para docentes y estudiantes? | Estudiantes: un semestre. Docentes: dos semestres. | Acuerdo (duración en días: S-03) |
| P08 | ¿Hay límite de recursos que una persona pueda tener al mismo tiempo? | No. | Acuerdo |
| P09 | ¿La persona puede cancelar lo que pidió? ¿Hasta cuándo? | Hasta antes de llevarse el recurso. | Acuerdo |

## 3. Aprobación y entrega

| # | Pregunta | Respuesta | Estado |
|---|---|---|---|
| P10 | ¿Quién decide si se presta un recurso? ¿Es la misma persona que lo entrega? | Un técnico. | Acuerdo (aprueba y entrega el mismo rol: S-04) |
| P11 | ¿Por qué razones se le negaría un préstamo a alguien? | Por ninguna, solo las del sistema. | Acuerdo |
| P12 | Si dos personas piden el mismo recurso para el mismo momento, ¿a quién se le da? | Al primero que lo pida. | Acuerdo |
| P13 | Si a alguien le aprueban pero no lo recoge, ¿qué pasa? | El técnico debe reportarlo y cancelar la reserva del recurso. | Acuerdo |
| P14 | Si una persona no devolvió a tiempo, ¿puede pedir otro? ¿Qué medidas se toman? | Puede pedir prórroga. | Acuerdo (quién aprueba: S-10) |

## 4. Devolución

| # | Pregunta | Respuesta | Estado |
|---|---|---|---|
| P15 | Cuando alguien devuelve un recurso, ¿qué se revisa o se anota? | Que fue entregado y una observación opcional. | Acuerdo |
| P16 | Si un recurso vuelve dañado, ¿qué se hace con él? | Se reporta como dañado y se saca de los recursos reservables mientras se arregla o se reemplaza. | Acuerdo |
| P17 | ¿Quién debe poder consultar los préstamos pasados? | Cada persona los suyos; el personal responsable, todos. | Acuerdo |

## 5. Coordinación

| # | Pregunta | Respuesta | Estado |
|---|---|---|---|
| P18 | ¿Qué información necesita ver la coordinación? | Préstamos, recursos utilizados y el historial. | Acuerdo (solo consulta: S-07) |

## 6. Usuarios

| # | Pregunta | Respuesta | Estado |
|---|---|---|---|
| P19 | ¿Quiénes usan el sistema y cómo se les da acceso? | Los usuarios se registran solos; al personal lo registra el administrador. | Acuerdo |

---

## Resumen al cierre

### Acuerdos
- Se prestan portátiles, cámaras, instrumentos y otros recursos académicos.
- Los recursos se reservan con anticipación, por rango de fechas, y una solicitud puede incluir varios recursos.
- Duración máxima: un semestre para estudiantes y dos para docentes. No hay límite de recursos simultáneos.
- El solicitante puede cancelar hasta antes de recibir el recurso.
- Un **técnico** evalúa las solicitudes; solo se niegan por reglas del sistema.
- Ante solicitudes simultáneas, gana **la primera registrada**.
- Si el solicitante no recoge el recurso, el técnico lo reporta y cancela la reserva.
- Se puede pedir **prórroga**.
- La devolución registra la entrega y una observación opcional. Un recurso dañado deja de ser reservable hasta que se repare o reemplace.
- Cada solicitante ve su historial; el personal ve todo.
- Coordinación consulta préstamos, recursos utilizados e historial.
- Los solicitantes se autorregistran; el personal lo crea un **administrador**.

### Pendientes (confirmar al aprobar los requisitos)
- Duración de un semestre en días (S-03).
- Quién aprueba la prórroga (S-10).

### Supuestos del equipo (no confirmados)
- Ver sección 13 de [`requisitos.md`](requisitos.md).
