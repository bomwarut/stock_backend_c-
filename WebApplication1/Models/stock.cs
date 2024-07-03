using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class stock
{
    public int stock_id { get; set; }

    public int product_id { get; set; }

    public int product_quantity { get; set; }

    public DateTime stock_update_at { get; set; }

    public virtual product product { get; set; } = null!;

    public virtual ICollection<stock_history> stock_histories { get; set; } = new List<stock_history>();
}
