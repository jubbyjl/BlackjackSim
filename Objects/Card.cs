namespace BlackjackSim.Objects;

public class Card
{
    public Value Value { get; }
    public Suit Suit { get; }
    
    public Card(Value value, Suit suit)
    {
        Value = value;
        Suit = suit;
    }
}
