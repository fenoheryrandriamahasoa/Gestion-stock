using System.Collections.ObjectModel;
using SuperMarcheApp.Models;

namespace SuperMarcheApp.ViewModels
{
    public class VendorsViewModel
    {
        public ObservableCollection<Vendor> Vendors { get; set; }

        public VendorsViewModel()
        {
            Vendors = new ObservableCollection<Vendor>
            {
                new Vendor
                {
                    Nom = "Rakoto",
                    Pseudo = "rakoto01",
                    Telephone = "0341234567"
                },
                new Vendor
                {
                    Nom = "Rabe",
                    Pseudo = "rabe02",
                    Telephone = "0329876543"
                }
            };
        }
    }
}
