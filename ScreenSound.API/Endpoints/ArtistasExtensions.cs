using Microsoft.AspNetCore.Mvc;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.API.Requests;
using ScreenSound.API.Responses;

namespace ScreenSound.API.Endpoints
{
    public static class ArtistasExtensions
    {
        public static void AddEndpointsArtistas(this WebApplication app)
        {
            app.MapGet("/Artistas", ([FromServices] DAL<Artista> dal) =>
            {
                var artistas = dal.Listar();
                var response = EntityToResponseList(artistas);
                return Results.Ok(response);
            });

            app.MapGet("/Artistas/{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
            {
                var artista = dal.RecuperarPor(artista => artista.Nome.ToUpper().Contains(nome.ToUpper()));

                if (artista == null)
                {
                    return Results.NotFound();
                }

                var response = EntityToResponse(artista);
                return Results.Ok(response);
            });

            app.MapPost("/CadastroArtista", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequest artistaRequest) =>
            {
                Artista artista = new Artista(artistaRequest.nome, artistaRequest.bio);
                dal.Adicionar(artista);
                return Results.Ok(artista);
            });

            app.MapDelete("/Artistas/{id}", ([FromServices] DAL<Artista> dal, int id) =>
            {
                var artista = dal.RecuperarPor(artista => artista.Id.Equals(id));

                if (artista == null)
                {
                    return Results.NotFound();
                }

                dal.Deletar(artista);
                return Results.NoContent();
            });

            app.MapPut("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequestEdit artistaRequestEdit) =>
            {
                var artistaAAtualizar = dal.RecuperarPor(artista => artista.Id == artistaRequestEdit.id);

                if (artistaAAtualizar == null)
                {
                    return Results.NotFound();
                }

                artistaAAtualizar.Nome = artistaRequestEdit.nome;
                artistaAAtualizar.Bio = artistaRequestEdit.bio;
                dal.Atualizar(artistaAAtualizar);
                return Results.Ok();
            });
        }
        private static ICollection<ArtistaResponse> EntityToResponseList(IEnumerable<Artista> listaDeArtistas)
        {
            return listaDeArtistas.Select(artista => EntityToResponse(artista)).ToList();
        }
        private static ArtistaResponse EntityToResponse(Artista artista)
        {
            return new ArtistaResponse(artista.Id, artista.Nome, artista.Bio, artista.FotoPerfil);
        }
    }
}


