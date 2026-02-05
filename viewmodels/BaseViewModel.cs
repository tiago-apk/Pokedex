using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Pokedex.viewmodels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Este método avisa a View que um valor mudou e ela deve se atualizar
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}