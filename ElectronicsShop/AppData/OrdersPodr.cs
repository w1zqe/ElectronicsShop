//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ElectronicsShop.AppData
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrdersPodr
    {
        public int ID_OrdersPodr { get; set; }
        public int ID_Orders { get; set; }
        public int ID_Product { get; set; }
        public int Quantity { get; set; }
    
        public virtual Orders Orders { get; set; }
        public virtual Product Product { get; set; }
    }
}
