namespace BlackjackSim.Objects;

public class Player
{
    public int Money { get; private set; }

    public Player(int money)
    {
        Money = money;
    }
    
    public bool CanBet(int amount)
    {
        return amount > 0 && amount <= Money;
    }

    public void Bet(int amount)
    {
        Money -= amount;
    }

    public void Win(int amount)
    {
        Money += amount;
    }
}
