using AppCine.dto;
using System.Collections.ObjectModel;

namespace AppCine.dto
{
    public class TaquillaViewModel
    {
        public ObservableCollection<Asiento> Asientos { get; set; }

        public TaquillaViewModel()
        {
            Asientos = new ObservableCollection<Asiento>
        {
            new Asiento { Id = 1 },
            new Asiento { Id = 2 },
            new Asiento { Id = 3 },
            new Asiento { Id = 4 },
            new Asiento { Id = 5 },
            new Asiento { Id = 6 },
            new Asiento { Id = 7 },
            new Asiento { Id = 8 },
            new Asiento { Id = 9 }
        };
        }

        public void ToggleSelection(Asiento asiento)
        {
            asiento.IsSelected = !asiento.IsSelected;
        }
    }

}
