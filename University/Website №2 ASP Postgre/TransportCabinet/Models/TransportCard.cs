using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TransportCabinet.Models;

namespace TransportCabinet.Models;

public partial class TransportCard
{
    [Key]
    public int pk_id_card { get; set; }

    public string fk_id_owner { get; set; }

    public double balance { get; set; }

    public DateOnly data_issue { get; set; }
    public int num_days { get; set; }

    public virtual UserAccount? fk_id_owner_navigation { get; set; }

    public TransportCard(string fk_id_owner, int pk_id_card)
    {
        this.num_days = 0;
        this.balance = 0;
        data_issue = DateOnly.FromDateTime(DateTime.Today);
        this.pk_id_card = pk_id_card;
		this.fk_id_owner = fk_id_owner;
    }

    public TransportCard() { }
}
