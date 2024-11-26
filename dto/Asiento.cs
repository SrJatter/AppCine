using System.ComponentModel;

namespace AppCine.dto
{
    public class Asiento : INotifyPropertyChanged
    {
        private bool _isSelected;

        private bool _isOcupado;

        public int Id { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        // Indicador de si el asiento está ocupado
        public bool IsOcupado
        {
            get => _isOcupado;
            set
            {
                if (_isOcupado != value)
                {
                    _isOcupado = value;
                    OnPropertyChanged(nameof(IsOcupado));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
