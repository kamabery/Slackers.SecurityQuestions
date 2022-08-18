using Slackers.Repository;

namespace Slackers.SecurityQuestions.ConsoleScreen.Models;

public class SecurityQuestion : IEntity
{
    public Guid Id { get; set; }
    public string Question { get; set; } = default!;
}

