using System;
using System.Collections.Generic;

namespace TransportCabinet.Models;

public partial class Purchase
{
    public int pk_num_purchase { get; set; }

    public int fk_id_card { get; set; }

    public string name_purchase { get; set; } = null!;

    public decimal price { get; set; }

    public int amount { get; set; }

    public Purchase() { }
    public Purchase(int pk_num_purchase = 0, int fk_id_card = 0, string name_purchase = null, decimal price = 0, int amount = 0)
    {
        this.pk_num_purchase = pk_num_purchase;
        this.fk_id_card = fk_id_card;
        this.name_purchase = name_purchase;
        this.price = price;
        this.amount = amount;
    }
}
