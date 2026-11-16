# Pruebas funcionales

Las pruebas se ejecutaron manualmente con **Swagger UI** (`https://localhost:7011/swagger`) el **16/11/2026**, sobre la base de datos creada con la migración y los datos semilla.

## Cómo reproducirlas
1. Restablece la base de datos (README §5.3) para partir de los datos semilla.
2. Inicia la API y abre Swagger.
3. Inicia sesión con `POST /api/auth/login` usando el usuario indicado. La contraseña de todos es `Prestamos2026*`.
4. Pulsa **Authorize**, pega el `token` y ejecuta la petición con los datos indicados.

(Las fechas de las pruebas corresponden al 16/11/2026. Si se repiten otro día, ajusta las fechas de las solicitudes para que no queden en el pasado.)

---

## Pruebas mínimas de la guía (P-01 a P-12)

### P-01: Consulta del catálogo y disponibilidad
| Campo | Detalle |
|---|---|
| Requisito | CA-01 · RF-05 · RN-02 |
| Condiciones iniciales | Datos semilla. CM-002 está dañado; CM-001, PT-002 y AV-001 están entregados. |
| Pasos | Iniciar sesión como `ana.perez@prestamos.test` → `GET /api/recursos` → `GET /api/recursos?desde=2026-11-25&hasta=2026-11-28` |
| Datos de entrada | Parámetros `desde` y `hasta` |
| Resultado esperado | 200. Los recursos ocupados o dañados aparecen con `disponibleEnRango: false`. |
| Resultado obtenido | 200. CM-001, CM-002, PT-002 y AV-001 en `false`; además, IN-001 en `false` en el rango del 25 al 28 de noviembre. |
| Método / código | GET · 200 |
| Estado |  Aprobada |
| Evidencia | [`recursos/02`](../evidencias/recursos/02-catalogo-hoy-200.png), [`recursos/03`](../evidencias/recursos/03-disponibilidad-25-28nov-200.png), [`recursos/04`](../evidencias/recursos/04-disponibilidad-20-22dic-200.png) |

### P-02: Consulta de recurso inexistente
| Campo | Detalle |
|---|---|
| Requisito | CA-01 · RNF-05 |
| Condiciones iniciales | No existe un recurso con id 999. |
| Pasos | `GET /api/recursos/999` con un token válido |
| Resultado esperado | 404 con ProblemDetails |
| Resultado obtenido | 404 · `"No existe el recurso con id 999."` |
| Método / código | GET · 404 |
| Estado |  Aprobada |
| Evidencia | [`recursos/06`](../evidencias/recursos/06-recurso-inexistente-404.png), [`solicitudes/08`](../evidencias/solicitudes/08-recurso-inexistente-404.png) |

### P-03: Registro de solicitud válida
| Campo | Detalle |
|---|---|
| Requisito | CA-02 · RF-07 · RF-08 |
| Condiciones iniciales | PT-003 e IN-003 libres del 23 al 27 de noviembre. |
| Pasos | Iniciar sesión como Ana → `POST /api/solicitudes` → `GET /api/solicitudes/{id}` |
| Datos de entrada | `{"recursoIds":[3,9],"fechaInicio":"2026-11-23","fechaFin":"2026-11-27","motivo":"Desarrollo del prototipo para la feria de proyectos"}` |
| Resultado esperado | 201, estado `Pendiente` y la solicitud recuperable |
| Resultado obtenido | 201 (id 9), con `Location`; 200 al consultarla |
| Método / código | POST · 201 / GET · 200 |
| Estado |  Aprobada |
| Evidencia | [`solicitudes/01`](../evidencias/solicitudes/01-registrar-solicitud-201.png), [`solicitudes/02`](../evidencias/solicitudes/02-consultar-solicitud-200.png) |

### P-04: Solicitud con datos inválidos
| Campo | Detalle |
|---|---|
| Requisito | CA-02 · RN-04 · RN-05 · RNF-03 |
| Condiciones iniciales | — |
| Pasos | Iniciar sesión como Ana → `POST /api/solicitudes` con datos inválidos |
| Datos de entrada | `{"recursoIds":[],"fechaInicio":"2026-11-27","fechaFin":"2026-11-23","motivo":"Corto"}` · y un período de 200 días siendo estudiante |
| Resultado esperado | 400 con errores por campo, sin persistir nada |
| Resultado obtenido | 400. Errores en `RecursoIds`, `FechaFin` y `Motivo`; 400 por superar los 180 días. |
| Método / código | POST · 400 |
| Estado |  Aprobada |
| Evidencia | [`solicitudes/04`](../evidencias/solicitudes/04-datos-invalidos-400.png), [`solicitudes/05`](../evidencias/solicitudes/05-estudiante-excede-180-dias-400.png) |

### P-05: Evaluación de solicitud por personal autorizado
| Campo | Detalle |
|---|---|
| Requisito | CA-03 · RF-10 · RF-11 · RN-09 |
| Condiciones iniciales | Solicitudes 10 y 11 pendientes. |
| Pasos | Iniciar sesión como `tecnico@prestamos.test` → `GET /api/solicitudes?estado=Pendiente` → `POST /api/solicitudes/11/aprobar` → `POST /api/solicitudes/10/rechazar` |
| Datos de entrada | Rechazo: `{"motivo":"El portátil PT-001 está reservado para mantenimiento preventivo en diciembre."}` |
| Resultado esperado | 200, con la decisión, el técnico y la fecha registrados |
| Resultado obtenido | 200. Solicitud 11 `Aprobada` con `evaluadaPor: Carlos Ruiz`; solicitud 10 `Rechazada` con el motivo. |
| Método / código | POST · 200 |
| Estado |  Aprobada |
| Evidencia | [`gestion/01`](../evidencias/gestion/01-pendientes-orden-llegada-200.png), [`gestion/02`](../evidencias/gestion/02-aprobar-solicitud-200.png), [`gestion/06`](../evidencias/gestion/06-rechazar-con-motivo-200.png) |

### P-06: Gestión por un actor no autorizado
| Campo | Detalle |
|---|---|
| Requisito | RNF-02 · RN-17 |
| Condiciones iniciales | Solicitud 5 pendiente. |
| Pasos | Con el token de Ana → `POST /api/solicitudes/5/aprobar`. Con el token de Coordinación → la misma petición. Con el token de Ana → `GET /api/solicitudes/3` (es de Juan). |
| Resultado esperado | 403 y la operación impedida |
| Resultado obtenido | 403 en los tres casos; la solicitud 5 sigue `Pendiente`. |
| Método / código | POST / GET · 403 |
| Estado |  Aprobada |
| Evidencia | [`gestion/04`](../evidencias/gestion/04-solicitante-aprueba-403.png), [`reportes/13`](../evidencias/reportes/13-coordinacion-aprueba-403.png), [`solicitudes/11`](../evidencias/solicitudes/11-ver-solicitud-ajena-403.png), [`auth/09`](../evidencias/auth/09-solicitante-listar-usuarios-403.png) |

### P-07: Asignación de recurso disponible
| Campo | Detalle |
|---|---|
| Requisito | CA-04 · RF-12 · RN-10 |
| Condiciones iniciales | PT-001 e IN-001 libres del 16 al 20 de noviembre. |
| Pasos | Ana: `POST /api/solicitudes` (empieza hoy) → técnico: `POST /{id}/aprobar` → `POST /{id}/entregar` |
| Datos de entrada | `{"recursoIds":[1,7],"fechaInicio":"2026-11-16","fechaFin":"2026-11-20","motivo":"Práctica de mediciones para el laboratorio de Circuitos"}` |
| Resultado esperado | Préstamo registrado con el solicitante, los recursos y el técnico correctos |
| Resultado obtenido | 200. Solicitud 12 `Entregada`, con `entregadaPor: Carlos Ruiz` y `fechaEntrega`. |
| Método / código | POST · 201 / 200 / 200 |
| Estado |  Aprobada |
| Evidencia | [`gestion/09`](../evidencias/gestion/09-crear-solicitud-hoy-201.png), [`gestion/10`](../evidencias/gestion/10-aprobar-solicitud-hoy-200.png), [`gestion/11`](../evidencias/gestion/11-entregar-recursos-200.png) |

### P-08: Intento de asignación incompatible
| Campo | Detalle |
|---|---|
| Requisito | CA-04 · RN-01 · RN-02 · RN-07 · RNF-04 |
| Condiciones iniciales | PT-003 e IN-003 reservados por la solicitud 9 (23 al 27 de noviembre); CM-002 dañado. |
| Pasos | Juan: `POST /api/solicitudes` con los mismos recursos y fechas · Ana: `POST` con CM-002 · técnico: `POST /api/solicitudes/5/entregar` (pendiente) · Juan: prórroga que se cruza con otra reserva |
| Resultado esperado | 409 y la disponibilidad se conserva |
| Resultado obtenido | 409: *"Ya están reservados… PT-003, IN-003"*, *"CM-002 (Danado)"*, *"Debe estar en Aprobada"*, *"AV-001 ya está reservado"*. |
| Método / código | POST · 409 |
| Estado |  Aprobada |
| Evidencia | [`solicitudes/03`](../evidencias/solicitudes/03-recurso-ya-reservado-409.png), [`solicitudes/07`](../evidencias/solicitudes/07-recurso-danado-409.png), [`gestion/08`](../evidencias/gestion/08-entregar-no-aprobada-409.png), [`prorrogas/05`](../evidencias/prorrogas/05-ampliacion-con-cruce-409.png) |

### P-09: Consulta del préstamo activo
| Campo | Detalle |
|---|---|
| Requisito | CA-04 · CA-05 · RF-08 |
| Condiciones iniciales | Solicitud 12 entregada. |
| Pasos | Ana: `GET /api/solicitudes/12` → `GET /api/recursos?desde=2026-11-16&hasta=2026-11-20` |
| Resultado esperado | Estado, responsable y recursos coherentes; recursos no disponibles |
| Resultado obtenido | 200. `Entregada`, con `entregadaPor` y los recursos PT-001 e IN-001; ambos con `disponibleEnRango: false`. |
| Método / código | GET · 200 |
| Estado |  Aprobada |
| Evidencia | [`gestion/12`](../evidencias/gestion/12-consultar-prestamo-activo-200.png), [`gestion/13`](../evidencias/gestion/13-disponibilidad-durante-prestamo-200.png) |

### P-10: Registro de devolución y consulta histórica
| Campo | Detalle |
|---|---|
| Requisito | CA-05 · RF-16 · RF-17 · RF-19 · RN-14 · RN-15 · RN-18 |
| Condiciones iniciales | Solicitud 12 entregada. |
| Pasos | Técnico: `POST /api/solicitudes/12/devoluciones` (PT-001 sin daño) → (IN-001 con daño) → Ana: `GET /api/recursos/7` → `GET /api/solicitudes/12/movimientos` |
| Datos de entrada | `{"recursoId":1,"devueltoConDano":false,"observacion":"Sin novedad"}` · `{"recursoId":7,"devueltoConDano":true,"observacion":"Pantalla del multímetro rota"}` |
| Resultado esperado | Devolución persistida; la solicitud queda `Finalizada`; el recurso dañado deja de estar disponible; historial consultable |
| Resultado obtenido | 200. Devolución parcial y luego `Finalizada`; IN-001 en `Danado`; historial Pendiente → Aprobada → Entregada → Finalizada. |
| Método / código | POST · 200 / GET · 200 |
| Estado |  Aprobada |
| Evidencia | [`gestion/14`](../evidencias/gestion/14-devolucion-parcial-200.png), [`gestion/17`](../evidencias/gestion/17-devolucion-final-finalizada-200.png), [`gestion/18`](../evidencias/gestion/18-recurso-danado-tras-devolucion-200.png), [`gestion/20`](../evidencias/gestion/20-movimientos-ciclo-completo-200.png) |

### P-11: Información consolidada y consistencia de datos
| Campo | Detalle |
|---|---|
| Requisito | CA-06 · RF-18 · RF-19 |
| Condiciones iniciales | Base de datos con préstamos activos, vencidos y devueltos. |
| Pasos | Iniciar sesión como `coordinacion@prestamos.test` → `GET /api/reportes/resumen` → `GET /api/reportes/prestamos` → `?estado=Vencido` → `?estado=Devuelto` |
| Resultado esperado | Indicadores de múltiples recursos y operaciones, con estados y responsables; los totales coinciden con el listado |
| Resultado obtenido | 200. `prestamosVencidos` coincide con las filas `Vencido`; cada fila trae `solicitante`, `entregadoPor` y `recibidoPor`. |
| Método / código | GET · 200 |
| Estado |  Aprobada |
| Evidencia | [`reportes/03`](../evidencias/reportes/03-resumen-general-200.png), [`reportes/07`](../evidencias/reportes/07-prestamos-con-responsables-200.png), [`reportes/08`](../evidencias/reportes/08-prestamos-vencidos-200.png), [`reportes/09`](../evidencias/reportes/09-prestamos-devueltos-200.png), [`persistencia`](../evidencias/persistencia/tablas-datos-semilla.png) |

### P-12: Integración interfaz–API–base de datos
| Campo | Detalle |
|---|---|
| Requisito | Todas las CA |
| Condiciones iniciales | Frontend Blazor conectado a la API |
| Pasos | Ciclo completo desde la interfaz: solicitar → aprobar → entregar → devolver |
| Resultado esperado | Flujo completo demostrado desde la interfaz hasta la base de datos |
| Resultado obtenido | La parte API–base de datos está demostrada en P-07 a P-10. La interfaz está en desarrollo. |
| Estado |  Pendiente de la integración del frontend |
| Evidencia | Por completar |

---

## Pruebas adicionales por módulo

| Módulo | Cantidad | Qué cubre | Evidencias |
|---|---:|---|---|
| Autenticación | 9 | Registro válido y duplicado, datos inválidos, login correcto e incorrecto, 401 sin token, permisos por rol | [`evidencias/auth`](../evidencias/auth) |
| Recursos | 15 | Disponibilidad por fechas, 400/404, CRUD del administrador, daño y reparación, permisos, historial | [`evidencias/recursos`](../evidencias/recursos) |
| Solicitudes | 18 | Registro, concurrencia, duración por tipo de solicitante, cancelación, visibilidad por rol, vencidas, historial | [`evidencias/solicitudes`](../evidencias/solicitudes) |
| Gestión del técnico | 20 | Aprobación, rechazo, entrega con fechas, no recogida, devolución parcial y con daño, ciclo completo | [`evidencias/gestion`](../evidencias/gestion) |
| Prórrogas | 16 | Cruces, duración máxima, una pendiente a la vez, prórroga de un préstamo vencido, aprobación y rechazo | [`evidencias/prorrogas`](../evidencias/prorrogas) |
| Consolidado | 14 | Indicadores, filtros, préstamos con responsables, permisos de Coordinación, historial de recursos | [`evidencias/reportes`](../evidencias/reportes) |
