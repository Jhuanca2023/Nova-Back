namespace Domain.Entities;

public class CardInfo
{
    public string CardHolder { get; set; }
    public string CardNumber { get; set; }
    public int ExpMonth { get; set; }
    public int ExpYear { get; set; }
    public string CVV { get; set; }
}
