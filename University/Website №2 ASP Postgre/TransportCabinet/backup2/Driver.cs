using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TransportCabinet.Models;

public partial class Driver
{
    [Key]
    public int pk_id_driver { get; set; }

    public string? name { get; set; }

    public string? surname { get; set; }

    public string? patronymic { get; set; }
}
