namespace BlackjackSim;
using BlackjackSim.Objects;

public class GameLoop
{
    private Player player = new Player(500);
    private Deck deck = new Deck();
    private Hand dealerHand = new Hand();
    private List<Bet> bets = [];
    private int activeHandIndex; // should encapsulate with bets

    public void Play()
    {
        while (player.Money > 0)
        {
            bets.Clear();
            dealerHand.ClearHand();

            ShowPlayerMoney();
            int originalWager = PlayerBet(); // OBO
            bets.Add(new Bet(originalWager));
            activeHandIndex = 0;

            // Initial deal
            deck.Shuffle();
            bets[activeHandIndex].Hand.AddCard(deck.Draw());
            dealerHand.AddCard(deck.Draw());
            bets[activeHandIndex].Hand.AddCard(deck.Draw());
            ShowTable();

            // Naturals
            if (HasBlackjack(bets[activeHandIndex].Hand))
            {
                DealerAimBlackjack();
                SettlePlayerBlackjack(bets[activeHandIndex]);
                continue;
            }

            //bool settled
            for (; activeHandIndex < bets.Count; activeHandIndex++)
            {
                if (activeHandIndex != 0) ShowTable();
                // Player move
                if (!PlayerMove()) // Bust
                {
                    ShowTable();
                    SettleBet(bets[activeHandIndex]);
                }
            }

            if (AllBetsSettled()) continue;
            activeHandIndex--;

            while (bets[activeHandIndex].IsSettled) activeHandIndex--;

            // Dealer move
            DealerMove();

            if (HasBlackjack(dealerHand))
            {
                SettleDealerBlackjack(originalWager);
                continue;
            }

            SettleBet(bets[activeHandIndex]);
            activeHandIndex--;
            for (; activeHandIndex >= 0; activeHandIndex--)
            {
                if (!bets[activeHandIndex].IsSettled)
                {
                    ShowTable();
                    SettleBet(bets[activeHandIndex]);
                }
            }
        }
        Console.WriteLine("Game over");
    }

    private bool HasBlackjack(Hand hand) // assumes not a split hand
    {
        if (hand.Cards.Count == 2 && hand.Total == 21) return true;
        return false;
    }

    private void SettlePlayerBlackjack(Bet bet)
    {
        Thread.Sleep(1000);
        if (!HasBlackjack(dealerHand))
        {
            int win = bet.Amount * 3 / 2;
            Console.WriteLine($"Blackjack. +${win}");
            player.Win(bet.Amount + win);
        }
        else
        {
            Console.WriteLine("Tie.");
            player.Win(bet.Amount);
        }
        Console.WriteLine();
        bet.Settle();
        Thread.Sleep(1000);
    }

    private void DealerAimBlackjack()
    {
        if (dealerHand.Cards[0].Value == Value.Ace || Math.Min((int)dealerHand.Cards[0].Value, 10) == 10)
        {
            Thread.Sleep(1000);
            dealerHand.AddCard(deck.Draw());
            ShowTable();
        }
        else return;
    }

    private void SettleDealerBlackjack(int originalWager)
    {
        Thread.Sleep(1000);
        int total = 0;
        for (int i = 0; i < bets.Count; i++)
        {
            if (!bets[i].IsSettled)
            {
                total += bets[i].Amount;
                bets[i].Settle();
            }
        }

        Console.WriteLine($"Dealer Blackjack. -${originalWager} (OBO)");
        player.Win(total - originalWager);
        Console.WriteLine();
        Thread.Sleep(1000);
    }

    private void SettleBet(Bet bet) // assumes no Blackjacks
    {
        Thread.Sleep(1000);
        if (bet.Hand.Total > 21)
        {
            Console.WriteLine($"Bust. -${bet.Amount}");
        }
        else if (dealerHand.Total > 21)
        {
            Console.WriteLine($"Dealer bust. +${bet.Amount}");
            player.Win(bet.Amount * 2);
        }
        else if (bet.Hand.Total > dealerHand.Total)
        {
            Console.WriteLine($"Player win. +${bet.Amount}");
            player.Win(bet.Amount * 2);
        }
        else if (dealerHand.Total > bet.Hand.Total)
        {
            Console.WriteLine($"Player lose. -${bet.Amount}");
        }
        else
        {
            Console.WriteLine("Tie.");
            player.Win(bet.Amount);
        }
        Console.WriteLine();
        bet.Settle();
        Thread.Sleep(1000);
    }

    private bool AllBetsSettled()
    {
        for (int i = 0; i < bets.Count; i++)
        {
            if (!bets[i].IsSettled) return false;
        }
        return true;
    }
    
    private int PlayerBet()
    {
        while (true)
        {
            Console.Write("Bet: ");
            int amount = int.Parse(Console.ReadLine()!);
            if (!player.CanBet(amount)) continue;

            player.Bet(amount);
            return amount;
        }
    }

    private bool PlayerMove()
    {
        Bet bet = bets[activeHandIndex];
        while (true)
        {
            if (bet.Hand.Cards.Count == 1) // just split
            {
                Thread.Sleep(1000);
                bet.Hand.AddCard(deck.Draw());
                ShowTable();
            }
            Console.Write("[1] Hit -- [2] Stand");
            if (player.CanBet(bet.Amount) && bet.Hand.CanDouble()) Console.Write($" -- [3] Double +{bet.Amount}");
            if (player.CanBet(bet.Amount) && bet.Hand.CanSplit()) Console.Write($" -- [4] Split +{bet.Amount}");
            Console.WriteLine();
            switch (Console.ReadLine())
            {
                case "1": // Hit
                    bet.Hand.AddCard(deck.Draw());
                    if (bet.Hand.Total > 21) return false;
                    break;
                case "2": // Stand
                    return true;
                case "3": // Double down
                    if (!player.CanBet(bet.Amount) || !bet.Hand.CanDouble()) continue;
                    player.Bet(bet.Amount);
                    bet.Double();
                    bet.Hand.AddCard(deck.Draw());
                    if (bet.Hand.Total > 21) return false;
                    return true;
                case "4": // Split
                    if (!player.CanBet(bet.Amount) || !bet.Hand.CanSplit()) continue;
                    player.Bet(bet.Amount);
                    bets.Add(new Bet(bet.Amount, bet.Hand.Split()));
                    break;
                default:
                    continue;
            }
            ShowTable();
        }
    }

    private void DealerMove()
    {
        ShowTable();
        while (dealerHand.Total < 17)
        {
            Thread.Sleep(1000);
            dealerHand.AddCard(deck.Draw());
            ShowTable();
        }
    }

    /*OUTPUT*/
    private void ShowTable()
    {
        Console.WriteLine();
        Console.WriteLine("DEALER");
        ShowHand(dealerHand);
        Console.WriteLine();
        Console.WriteLine($"PLAYER HAND ({activeHandIndex + 1}/{bets.Count}) - Wager: ${bets[activeHandIndex].Amount}");
        ShowHand(bets[activeHandIndex].Hand);
        Console.WriteLine();
    }
    private void ShowHand(Hand hand)
    {
        string totalStr = $"{hand.Total}";
        if (hand.IsSoft) totalStr = $"{hand.Total - 10}/{totalStr}";
        Console.WriteLine(totalStr);
        foreach (Card card in hand.Cards)
        {
            ShowCard(card);
        }
        Console.WriteLine();
    }
    private void ShowCard(Card card)
    {
        string valueStr = card.Value switch
        {
            Value.Ace => "A",
            Value.Jack => "J",
            Value.Queen => "Q",
            Value.King => "K",
            _ => ((int)card.Value).ToString(),
        };
        string suitStr = card.Suit switch
        {
            Suit.Clubs => "\u2663",
            Suit.Diamonds => "\u2666",
            Suit.Hearts => "\u2665",
            Suit.Spades => "\u2660",
            _ => "?",
        };
        Console.Write($"|{valueStr,-2}{suitStr}| ");
    }
    private void ShowPlayerMoney()
    {
        Console.WriteLine($"Stack: ${player.Money}");
    }
}
