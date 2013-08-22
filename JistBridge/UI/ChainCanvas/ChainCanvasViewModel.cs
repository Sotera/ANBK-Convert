using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using JistBridge.Annotations;
using JistBridge.Data.Model;
using JistBridge.Interfaces;

namespace JistBridge.UI.ChainCanvas
{
    [Export(typeof(IChainCanvasViewModel))]
    public class ChainCanvasViewModel: IChainCanvasViewModel, INotifyPropertyChanged
    {
        private Fragment _leftFragment;
        private Fragment _centerFragment;
        private Fragment _rightFragment;

        public Fragment LeftFragment
        {
            get { return _leftFragment; }
            set
            {
                if (Equals(value, _leftFragment)) return;
                _leftFragment = value;
                OnPropertyChanged();
            }
        }

        public Fragment CenterFragment
        {
            get { return _centerFragment; }
            set
            {
                if (Equals(value, _centerFragment)) return;
                _centerFragment = value;
                OnPropertyChanged();
            }
        }

        public Fragment RightFragment
        {
            get { return _rightFragment; }
            set
            {
                if (Equals(value, _rightFragment)) return;
                _rightFragment = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
