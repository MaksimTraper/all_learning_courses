using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TransportCabinet.Models;

public partial class TransportVehicle
{
    [Key]
    public int pk_id_vehicle { get; set; }

    public string? brand { get; set; }

    public string? model { get; set; }

    public virtual ICollection<Transport> transports { get; set; } = new List<Transport>();
}
