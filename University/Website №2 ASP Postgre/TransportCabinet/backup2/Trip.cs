using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TransportCabinet.Models;

public partial class Trip
{

    [Key]
    public int pk_num_trip { get; set; }
    public string? fk_id_tr { get; set; }

    public int? fk_id_card { get; set; }

    public int fk_id_driver { get; set; }

    public DateTime? time_pay { get; set; }

    public int? num_route { get; set; }

    public virtual TransportCard? fk_id_card_navigation { get; set; }

    public virtual Transport? fk_id_tr_navigation { get; set; }

    public virtual Route? num_route_navigation { get; set; }

    public virtual Driver fk_id_driver_navigation { get; set; }
}
