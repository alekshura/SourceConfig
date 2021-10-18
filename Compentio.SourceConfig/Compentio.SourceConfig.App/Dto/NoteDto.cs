namespace Compentio.SourceConfig.App.Dto
{
    public record NoteDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
