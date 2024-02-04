using LS_Shop_App.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LS_Shop_App.ViewModels
{
    internal class AddEditSignDefinitionsViewModel : INotifyPropertyChanged
    {
        private string _imagePath;

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged();
                }
            }
        }

        public AddEditSignDefinitionsViewModel() { }

        public AddEditSignDefinitionsViewModel(string imagePath)
        {
            this._imagePath = imagePath;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex.ToString());
            }
        }
    }
}
