using Slackers.Repository;

namespace Slackers.SecurityQuestions.ConsoleScreen.Models;

public class User : IEntity
{
    public User()
    {
        Answers = new Dictionary<string, string>();
    }
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string DisplayName { get; set; } = default!;

    public Dictionary<string, string> Answers { get; set; }
}