using System.Diagnostics.CodeAnalysis;

namespace Compentio.SourceConfig.App.Dto
{
    [ExcludeFromCodeCoverage]
    public record NoteDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
