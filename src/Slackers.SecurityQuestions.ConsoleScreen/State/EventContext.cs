namespace Slackers.SecurityQuestions.ConsoleScreen.State;

public class EventContext
{
    public Guid SessionId { get; set; }

    public string Name { get; set; } = default!;

    public bool? AddingQuestions { get; set; }

    public int QuestionsAdded { get; set; }

    public int QuestionsAnswersAttempted { get; set; }
}