namespace BlackjackSim.Objects;

public class Hand
{
    public List<Card> Cards { get; } = []; // could expose as ReadOnlyCollection
    public int Total { get; private set; }
    public bool IsSoft { get; private set; }
    
    public void AddCard(Card card)
    {
        Cards.Add(card);
        if (card.Value == Value.Ace && Total + 11 <= 21)
        {
            Total += 11;
            IsSoft = true;
        }
        else
        {
            Total += Math.Min((int)card.Value, 10);
            if (Total > 21 && IsSoft)
            {
                Total -= 10;
                IsSoft = false;
            }
        }
    }

    public void ClearHand()
    {
        Cards.Clear();
        Total = 0;
        IsSoft = false;
    }

    public bool CanDouble()
    {
        if (Cards.Count != 2) return false;
        if (Total >= 9 && Total <= 11) return true;
        return false;
    }

    public bool CanSplit()
    {
        if (Cards.Count != 2) return false;
        if (Math.Min((int)Cards[0].Value, 10) == Math.Min((int)Cards[1].Value, 10)) return true;
        return false;
    }

    public Hand Split()
    {
        Card splitCard = Cards[1];

        Cards.RemoveAt(1);
        if (Cards[0].Value == Value.Ace)
        {
            Total = 11;
        }
        else
        {
            Total = Math.Min((int)Cards[0].Value, 10);
        }
        
        Hand splitHand = new Hand();
        splitHand.AddCard(splitCard);
        return splitHand;
    }
}
