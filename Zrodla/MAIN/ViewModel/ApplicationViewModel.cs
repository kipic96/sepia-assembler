using MAIN.Model;
using MAIN.ViewModel.Helper;
using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MAIN.ViewModel
{
    class ApplicationViewModel : BindableClass
    {
        private SepiaManager _sepiaManager;
        private ICommand _loadImage;
        private ICommand _executeEffectCommand;
        private ICommand _saveImage;
        private BitmapSource _beforeBitmapImage;
        private BitmapSource _afterBitmapImage;
        private int _elapsedTime;
        private bool[] _sepiaType;
        private int _sepiaRate; 
        private int _threadsNumber; 

        public ApplicationViewModel()
        {
            SepiaType = new bool[] { false, false };
            ThreadsNumber = System.Environment.ProcessorCount;
            SepiaRate = 20;
        }

        public BitmapSource BeforeBitmapImage
        {
            get
            {
                return _beforeBitmapImage;
            }
            private set
            {
                _beforeBitmapImage = value;
                RaisePropertyChanged(nameof(BeforeBitmapImage));
            }
        }

        public BitmapSource AfterBitmapImage
        {
            get
            {
                return _afterBitmapImage;
            }
            private set
            {
                _afterBitmapImage = value;
                RaisePropertyChanged(nameof(AfterBitmapImage));
            }
        }

        public bool[] SepiaType
        {
            get { return _sepiaType; }
            set
            {
                _sepiaType = value;
                RaisePropertyChanged(nameof(SepiaType));
            }
        }

        public int ThreadsNumber
        {
            get { return _threadsNumber; }
            set
            {
                _threadsNumber = value;
                RaisePropertyChanged(nameof(ThreadsNumber));
            }
        }

        public int SepiaRate
        {
            get { return _sepiaRate; }
            set
            {
                _sepiaRate = value;
                RaisePropertyChanged(nameof(SepiaRate));
            }
        }

        public int ElapsedTime
        {
            get
            {
                return _elapsedTime;
            }
            set
            {
                _elapsedTime = value;
                RaisePropertyChanged(nameof(ElapsedTime));
            }
        }

        public Enum.SepiaMechanismType SepiaMechanismType
        {
            get
            {
                for (int i = 0; i < SepiaType.Length; i++)
                    if (SepiaType[i])
                        return (Enum.SepiaMechanismType)(i + 1);
                return Enum.SepiaMechanismType.Undefined;
            }
        }

        public ICommand LoadImageCommand
        {
            get
            {
                if (_loadImage == null)
                {
                    _loadImage = new RelayCommand(
                        p => LoadImageFromFile(),
                        p => { return true; });
                }
                return _loadImage;
            }
        }

        public ICommand SaveImageCommand
        {
            get
            {
                if (_saveImage == null)
                {
                    _saveImage = new RelayCommand(
                        p => SaveImageToFile(),
                        p =>
                        {
                            return !(AfterBitmapImage == null);
                        });
                }
                return _saveImage;
            }
        }

        public ICommand ExecuteEffectCommand
        {
            get
            {
                if (_executeEffectCommand == null)
                {
                    _executeEffectCommand = new RelayCommand(
                        p => ExecuteEffect(),
                        p =>
                        { 
                            return (_beforeBitmapImage != null &&
                                SepiaMechanismType != Enum.SepiaMechanismType.Undefined);
                        });
                }
                return _executeEffectCommand;
            }
        }

        private void ExecuteEffect()
        {
            TimeSpan elapsedTime;
            _sepiaManager = new SepiaManager(
                _beforeBitmapImage, SepiaMechanismType,
                (float)_sepiaRate, _threadsNumber);
            AfterBitmapImage = _sepiaManager.ExecuteEffect(out elapsedTime);
            ElapsedTime = elapsedTime.Milliseconds;
        }

        private void LoadImageFromFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = 
                new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".bmp";
            dlg.Filter = "BMP Files (*.bmp)|*.bmp";
            bool? result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                BeforeBitmapImage = LoadImageToMemory(dlg.FileName);                         
            }
        }

        private void SaveImageToFile()
        {
            Microsoft.Win32.SaveFileDialog dlg = 
                new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "sepia image";
            dlg.DefaultExt = ".bmp";
            dlg.Filter = "BMP File (.bmp)|*.bmp"; 

            bool? result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                SaveImageToDisk(AfterBitmapImage, dlg.FileName);
            }
        }

        private void SaveImageToDisk(BitmapSource image, string filePath)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(fileStream);
                }
            }
            catch (Exception ex)
            {
                Log.MessageLog.Show(ex.Message);
            }
        }

        private BitmapSource LoadImageToMemory(string path)
        {
            BitmapSource newBitmap;
            try
            {
                newBitmap = new BitmapImage(new System.Uri(path));
                return newBitmap;
            }
            catch (Exception ex)
            {
                Log.MessageLog.Show(ex.Message);
            }
            return null;
        }
    }
}
