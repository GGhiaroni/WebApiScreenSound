using System.ComponentModel.DataAnnotations;

namespace ScreenSound.API.Requests;
public record MusicaRequest(
    [Required] string nome,
    [Required] int artistaId,
    [Required] int anoLancamento,
    ICollection<GeneroRequest> generos = null);
public record MusicaRequestEdit(int id, string nome, int artistaId, int anoLancamento) : MusicaRequest(nome, artistaId, anoLancamento);

