using Compentio.SourceConfig.App.Dto;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Compentio.SourceConfig.App.Services
{
    public interface INotesService
    {
        NoteDto GetNote(long noteId);
    }

    [ExcludeFromCodeCoverage]
    public class NotesService : INotesService
    {
        private readonly IConfiguration _configuration;

        public NotesService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public NoteDto GetNote(long noteId)
        {
            var appSettings = _configuration.Get<AppSettings>();

            return new NoteDto
            {
                Id = noteId,
                Description = appSettings.DefaultNote.Description,
                Title = appSettings.DefaultNote.Title
            };
        }      
    }
}
