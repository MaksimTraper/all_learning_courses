using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TransportCabinet.Models;

public partial class Transport
{
    [Key]
    public string pk_car_num { get; set; } = null!;

    public int fk_id_vehicle { get; set; }
}
