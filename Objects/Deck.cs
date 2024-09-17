namespace BlackjackSim.Objects;

public class Deck
{
    private Card[] cards = new Card[52];
    public int Count { get; private set; }

    public Deck()
    {
        for (int i = 1; i <= 13; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                cards[Count++] = new Card((Value)i, (Suit)j);
            }
        }
    }
    
    public void Shuffle()
    {
        Random.Shared.Shuffle(cards);
        Count = 52;
    }

    public void Shuffle(int test)
    {
        Shuffle();
        if (test == 1) // player blackjack
        {
            cards[51] = new Card(Value.Ace, Suit.Clubs);
            cards[49] = new Card(Value.Ten, Suit.Clubs);
        }
        else if (test == 2) // dealer blackjack
        {
            cards[50] = new Card(Value.Ace, Suit.Clubs);
            cards[48] = new Card(Value.Ten, Suit.Clubs);
        }
        else if (test == 3) // blackjack tie
        {
            cards[51] = new Card(Value.Ace, Suit.Clubs);
            cards[49] = new Card(Value.Ten, Suit.Clubs);
            cards[50] = new Card(Value.Ace, Suit.Diamonds);
            cards[48] = new Card(Value.Ten, Suit.Diamonds);
        }
        else if (test == 4) // player split or double down
        {
            cards[51] = new Card(Value.Five, Suit.Clubs);
            cards[49] = new Card(Value.Five, Suit.Diamonds);
        }
    }
    
    public Card Draw() 
    {
        Count--;
        return cards[Count];
    }
}
