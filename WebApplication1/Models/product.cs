using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class product
{
    public int product_id { get; set; }

    public string product_code { get; set; } = null!;

    public string product_name { get; set; } = null!;

    public decimal product_price { get; set; }

    public DateTime product_update_at { get; set; }

    public virtual ICollection<stock> stocks { get; set; } = new List<stock>();
}
