# Guía de contribución

## Ramas
| Rama | Uso |
|---|---|
| `main` | Versión estable, entregable. Solo recibe merges desde `develop`. |
| `develop` | Integración del trabajo del equipo. |
| `feature/<ca>-<descripcion>` | Funcionalidad. Ej.: `feature/ca02-registrar-solicitud` |
| `docs/<tema>` | Documentación. Ej.: `docs/requisitos` |
| `fix/<descripcion>` | Corrección de errores. |

Nunca se hace commit directo a `main` ni a `develop`: todo entra por pull request.

## Commits (Conventional Commits, en español)
```
<tipo>(<alcance>): <descripción en imperativo>
```
Tipos: `feat`, `fix`, `docs`, `refactor`, `test`, `chore`.

Ejemplos:
- `feat(solicitudes): registrar solicitud de préstamo (RF-03)`
- `fix(prestamos): impedir asignación de recurso ya prestado`
- `docs(requisitos): agregar registro de entrevista`

Evitar mensajes genéricos como `cambios`, `update` o `avance`.

## Pull requests
1. Crear la rama desde `develop` actualizada.
2. Abrir el PR hacia `develop` y llenar la plantilla completa.
3. Otro integrante revisa y aprueba antes del merge.
4. Usar **Squash and merge** o **Merge commit** de forma consistente en todo el proyecto.

## Responsables
| Integrante | Usuario GitHub | Responsabilidad principal |
|---|---|---|
| Sebastián López Sánchez | @por-completar | Por definir |
| Por completar | @por-completar | Por definir |
