using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TransportCabinet.Models;

public partial class UserAccount
{

    [Key]
    public string passport_num { get; set; } = null!;

    public string? name { get; set; } = null;

    public string? surname { get; set; } = null;

    public string? patronymic { get; set; } = null;

    public string pk_login { get; set; } = null;

    public string? password { get; set; } = null;

    public string? email { get; set; } = null;

    public string? role { get; set; } = null;

    public DateOnly? birthday { get; set; } = null;

    public virtual ICollection<TransportCard> transport_cards { get; set; } = new List<TransportCard>();

    public TransportCard GetTransportCard(TransportCabinetContext db)
    {
        TransportCard card = null;
        card = db.TransportCards.FirstOrDefault(u => u.fk_id_owner == this.pk_login);
        return card;
    }
}
