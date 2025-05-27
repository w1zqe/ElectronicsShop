using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicsShop.AppData
{
    internal class AppConnect
    {
        public static ElectronicsShopEntities model0db; // Замените YourDbContext на ваш класс контекста
        static AppConnect()
        {
            model0db = new ElectronicsShopEntities(); // Инициализация при первом обращении
        }
    }
}
