using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TransportCabinet.Models;

public partial class Route
{
    [Key]
    public int pk_id_route { get; set; }

    public string? start_point { get; set; }

    public string? end_point { get; set; }
}
