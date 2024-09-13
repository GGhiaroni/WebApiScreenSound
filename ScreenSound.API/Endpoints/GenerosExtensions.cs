using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Responses;
using ScreenSound.Banco;
using ScreenSound.Shared.Modelos.Modelos;
using ScreenSound.Modelos;

namespace ScreenSound.API.Endpoints
{
    public static class GenerosExtensions
    {
        public static void AddEndpointsGeneros(this WebApplication app)
        {
            app.MapGet("/Generos", ([FromServices] DAL<Genero> dal) =>
            {
                return EntityListToResponseList(dal.Listar());
            });

            app.MapPost("/Generos", ([FromServices] DAL<Genero> dal, [FromBody] GeneroRequest generoRequest) =>
            {
                dal.Adicionar(RequestToEntity(generoRequest));
            });

            app.MapGet("/Generos/{nome}", ([FromServices] DAL<Genero> dal, string nome) =>
            {
                var genero = dal.RecuperarPor(a => a.Nome.ToUpper().Contains(nome.ToUpper()));
                if (genero is not null)
                {
                    var response = EntityToResponse(genero!);
                    return Results.Ok(response);
                }
                return Results.NotFound("Gênero não encontrado.");
            });

            app.MapDelete("/Generos/{id}", ([FromServices] DAL<Genero> dal, int id) =>
            {
                var genero = dal.RecuperarPor(a => a.Id == id);
                if (genero is null)
                {
                    return Results.NotFound("Gênero para exclusão não encontrado.");
                }
                dal.Deletar(genero);
                return Results.NoContent();
            });
        }
        private static Genero RequestToEntity(GeneroRequest genero)
        {
            return new Genero() { Nome = genero.Nome, Descricao = genero.Descricao };
        }

        private static ICollection<GeneroResponse> EntityListToResponseList(IEnumerable<Genero> generos)
        {
            return generos.Select(a => EntityToResponse(a)).ToList();
        }

        private static GeneroResponse EntityToResponse(Genero genero)
        {
            return new GeneroResponse(genero.Id, genero.Nome!, genero.Descricao!);
        }

    }
}
