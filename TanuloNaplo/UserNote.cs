namespace TanuloNaplo;

public class UserNote
{
    public int Id { get; set; }
    public string UserId { get; set; } = "AktualisDiak";
    public string CourseName { get; set; } = "";
    public string NoteContent { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}