using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class stock_history
{
    public int stock_history_id { get; set; }

    public int stock_id { get; set; }

    /// <summary>
    /// 1 = รับสินค้าเข้าคลัง, 2=เบิกสินค้าออกคลัง
    /// </summary>
    public bool stock_type { get; set; }

    public int quantity { get; set; }

    public DateTime? stock_history_update_at { get; set; }

    public virtual stock stock { get; set; } = null!;
}
