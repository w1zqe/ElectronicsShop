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
        public static Users CurrentUser { get; set; }

        // Добавляем метод для проверки роли
        public static bool IsCurrentUserAdmin()
        {
            return CurrentUser?.Roles != null &&
                   CurrentUser.Roles.RoleName.Equals("Администратор", StringComparison.OrdinalIgnoreCase);
        }

        static AppConnect()
        {
            model0db = new ElectronicsShopEntities();
        }
    }
}
