using System.Collections.ObjectModel;
using SuperMarcheApp.Models;

namespace SuperMarcheApp.ViewModels
{
    public class UsersViewModel
    {
        public ObservableCollection<User> Users { get; set; }

        public UsersViewModel()
        {
            Users = new ObservableCollection<User>
            {
                new User
                {
                    Nom = "Rakoto",
                    Pseudo = "rakoto01",
                    Telephone = "0341234567"
                },
                new User
                {
                    Nom = "Rabe",
                    Pseudo = "rabe02",
                    Telephone = "0329876543"
                }
            };
        }
    }
}
