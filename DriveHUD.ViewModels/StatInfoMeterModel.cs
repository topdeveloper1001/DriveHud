using DriveHUD.Common.Annotations;
using ProtoBuf;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using DriveHUD.Common.Linq;

namespace DriveHUD.ViewModels
{
    [ProtoContract]
    public class StatInfoMeterModel : INotifyPropertyChanged
    {
        private const int ARRAY_SIZE = 10;

        private SolidColorBrush[] backgroundBrush;

        [ProtoMember(1, OverwriteList = true)]
        public SolidColorBrush[] BackgroundBrush
        {
            get
            {
                return backgroundBrush;
            }
            set
            {
                if (backgroundBrush == value)
                {
                    return;
                }

                backgroundBrush = value;

                if (backgroundBrush != null)
                {
                    backgroundBrush.ForEach(x => x.Freeze());
                }

                OnPropertyChanged();
            }
        }

        private SolidColorBrush[] borderBrush;

        [ProtoMember(2, OverwriteList = true)]
        public SolidColorBrush[] BorderBrush
        {
            get
            {
                return borderBrush;
            }
            set
            {
                if (borderBrush == value)
                {
                    return;
                }

                borderBrush = value;

                if (borderBrush != null)
                {
                    borderBrush.ForEach(x => x.Freeze());
                }

                OnPropertyChanged();
            }
        }

        public StatInfoMeterModel()
        {
            InitializeWithDefaultValues();
        }

        private void InitializeWithDefaultValues()
        {
            BackgroundBrush = new SolidColorBrush[ARRAY_SIZE]
            {
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#c92a39")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#c94939")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#c96939")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#c98d39")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#c9ac39")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#c9db39")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#bbdb39")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#a1d23c")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#73c343")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#51b848")),
            };

            BorderBrush = new SolidColorBrush[ARRAY_SIZE]
            {
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff3c52")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff6952")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff9652")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffca52")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#fff652")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffff52")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffff52")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#bde962")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#99e06e")),
                (SolidColorBrush)(new BrushConverter().ConvertFrom("#b7d87b")),
            };

            for (int i = 0; i < ARRAY_SIZE; i++)
            {
                BackgroundBrush[i].Freeze();
                BorderBrush[i].Freeze();
            }

        }

        public void UpdateBackgroundBrushes(params string[] brushes)
        {
            UpdateBackgroundBrushes(brushes.Select(x => (SolidColorBrush)(new BrushConverter().ConvertFrom(x))).ToArray());
        }

        public void UpdateBackgroundBrushes(params SolidColorBrush[] brushes)
        {
            if (brushes.Count() != ARRAY_SIZE)
            {
                throw new ArgumentException($"Array of size {ARRAY_SIZE} is required");
            }

            for (int i = 0; i < ARRAY_SIZE; i++)
            {
                BackgroundBrush[i] = new SolidColorBrush(brushes[i].Color);
                BackgroundBrush[i].Freeze();
            }
        }

        public void UpdateBorderBrushes(params string[] brushes)
        {
            UpdateBorderBrushes(brushes.Select(x => (SolidColorBrush)(new BrushConverter().ConvertFrom(x))).ToArray());
        }

        public void UpdateBorderBrushes(params SolidColorBrush[] brushes)
        {
            if (brushes.Count() != ARRAY_SIZE)
            {
                throw new ArgumentException($"Array of size {ARRAY_SIZE} is required");
            }

            for (int i = 0; i < ARRAY_SIZE; i++)
            {
                BorderBrush[i] = new SolidColorBrush(brushes[i].Color);
                BorderBrush[i].Freeze();
            }
        }

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
