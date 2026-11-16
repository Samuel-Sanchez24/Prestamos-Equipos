using Prestamos.Domain.Entities;

namespace Prestamos.Application.Abstractions;

public interface IGeneradorToken
{
    (string Token, DateTime ExpiraEn) Generar(Usuario usuario);
}