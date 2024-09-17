namespace BlackjackSim.Objects;

public class Bet
{
    public Hand Hand { get; }
    public int Amount { get; private set; }
    public bool IsSettled { get; private set; }

    public Bet(int amount)
    {
        Hand = new Hand();
        Amount = amount;
    }

    public Bet(int amount, Hand hand)
    {
        Hand = hand;
        Amount = amount;
    }

    public void Settle()
    {
        Amount = 0;
        IsSettled = true;
    }

    public void Double()
    {
        Amount *= 2;
    }
}
