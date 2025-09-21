namespace backend.Models;

public class Enrollment
{
    public int Id { get; set; }         // Clave primaria
    public int UserId { get; set; }     // FK hacia User
    public int CourseId { get; set; }   // FK hacia Course
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public Course? Course { get; set; }
}
