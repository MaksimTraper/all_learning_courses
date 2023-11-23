using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TransportCabinet.Models;

public partial class TransportCard
{
    [Key]
    public int pk_id_card { get; set; }

    public string fk_id_owner { get; set; }

    public double balance { get; set; }

    public DateOnly data_issue { get; set; }

    public int num_days { get; set; }
}
