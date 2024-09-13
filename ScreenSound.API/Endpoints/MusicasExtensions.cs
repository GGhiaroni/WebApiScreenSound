using Microsoft.AspNetCore.Mvc;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.API.Requests;
using ScreenSound.API.Responses;
using ScreenSound.Shared.Modelos.Modelos;

namespace ScreenSound.API.Endpoints
{
    public static class MusicasExtensions
    {
        public static void AddEndpointsMusicas(this WebApplication app)
        {
            app.MapGet("/Musicas", ([FromServices] DAL<Musica> dal) =>
            {
                var musicas = dal.Listar();
                var response = EntityListToResponseList(musicas);
                return Results.Ok(response);
            });

            app.MapGet("/Musicas/{nome}", ([FromServices] DAL<Musica> dal, string nome) =>
            {
                var musica = dal.RecuperarPor(
                        m => m.Nome.ToUpper().Contains(nome.ToUpper())
                    );
                if (musica == null) return Results.NotFound();
                var response = EntityToResponse(musica);
                return Results.Ok(response);
            });

            app.MapPost("/CadastroMusica", (
                [FromServices] DAL<Musica> dalMusica,
                [FromServices] DAL<Artista> dalArtista,
                [FromServices] DAL<Genero> dalGenero,
                [FromBody] MusicaRequest musicaRequest) =>
            {
                Artista artista = dalArtista.RecuperarPor(a => a.Id == musicaRequest.artistaId);

                if (artista == null)
                {
                    return Results.NotFound("Artista não encontrado.");
                }

                Musica musica = new Musica(musicaRequest.nome)
                {
                    AnoLancamento = musicaRequest.anoLancamento,
                    Artista = artista,
                    ArtistaId = artista.Id,
                    Generos = musicaRequest.generos is not null ? GeneroRequestConverter(musicaRequest.generos, dalGenero)
                              : new List<Genero>()
                };

                dalMusica.Adicionar(musica);
                return Results.Ok(musica);
            });

            app.MapDelete("/Musicas/{id}", ([FromServices] DAL<Musica> dal, int id) =>
            {
                var musicaEncontrada = dal.RecuperarPor(m => m.Id.Equals(id));
                if (musicaEncontrada == null) return Results.NotFound();
                dal.Deletar(musicaEncontrada);
                return Results.NoContent();
            });

            app.MapPut("/Musicas", ([FromServices] DAL<Musica> dal, [FromBody] MusicaRequestEdit musicaRequestEdit) =>
            {
                var musicaAAtualizar = dal.RecuperarPor(m => m.Id.Equals(musicaRequestEdit.id));
                if (musicaAAtualizar == null) return Results.NotFound();
                musicaAAtualizar.Nome = musicaRequestEdit.nome;
                musicaAAtualizar.AnoLancamento = musicaRequestEdit.anoLancamento;

                dal.Atualizar(musicaAAtualizar);
                return Results.Ok();
            });
        }

        private static ICollection<Genero> GeneroRequestConverter(ICollection<GeneroRequest> generos, DAL<Genero> dalGenero)
        {
            List<Genero> listaDeGeneros = new List<Genero>();

            foreach (GeneroRequest genero in generos)
            {
                var generoBuscado = dalGenero.RecuperarPor(g => g.Nome.ToUpper().Equals(genero.Nome.ToUpper()));
                if (generoBuscado is not null)
                {
                    listaDeGeneros.Add(generoBuscado);
                }
                else
                {
                    var entidade = RequestToEntity(genero);
                    listaDeGeneros.Add(entidade);
                }
            }

            return listaDeGeneros;
        }

        private static Genero RequestToEntity(GeneroRequest genero)
        {
            return new Genero() { Nome = genero.Nome, Descricao = genero.Descricao };
        }

        private static ICollection<MusicaResponse> EntityListToResponseList(IEnumerable<Musica> musicaList)
        {
            return musicaList.Select(a => EntityToResponse(a)).ToList();
        }

        private static MusicaResponse EntityToResponse(Musica musica)
        {
            // Verifique se a música tem um artista associado.
            if (musica.Artista == null)
            {
                // Você pode optar por ignorar essas músicas ou retornar uma resposta padrão.
                // Aqui estamos retornando um valor padrão ou indicando que o artista está faltando.
                return new MusicaResponse(musica.Id, musica.Nome!, 0, "Artista não disponível", musica.AnoLancamento);
            }

            return new MusicaResponse(musica.Id, musica.Nome!, musica.Artista.Id, musica.Artista.Nome, musica.AnoLancamento);
        }



    }
}

