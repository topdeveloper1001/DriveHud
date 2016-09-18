using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using DriveHUD.Application.ViewModels;

namespace DriveHUD.Application
{
    public class PaletteManager
    {
        internal static Palette _Palette;

        public static void Initialize()
        {
            _Palette = new Palette() {Source = new Uri("Steel.xaml", UriKind.RelativeOrAbsolute)};
        }

        static PaletteManager()
        {
            Initialize();
            AllPalettes = new List<Palette>
            {
                new Palette(new Uri("Steel.xaml", UriKind.RelativeOrAbsolute)) {Name = "steel"},
            };
        }

        public static Palette Palette
        {
            get { return PaletteManager._Palette; }
        }

        public static IEnumerable<Palette> AllPalettes { get; set; }

        public static IEnumerable<Palette> CustomPalettes
        {
            get
            {
                return new List<Palette>
                {
                    new Palette(new Uri("/SalesDashboard;component/Palettes/Miliotis.xaml", UriKind.RelativeOrAbsolute))
                    {
                        Name = "miliotis"
                    },
                    new Palette(new Uri("/SalesDashboard;component/Palettes/Princess.xaml", UriKind.RelativeOrAbsolute))
                    {
                        Name = "princess"
                    }
                };
            }
        }
    }

    public class Palette : DependencyObject
    {
        public Palette()
        {
            //intentionally empty
        }

        public Palette(Uri paletteUri)
        {
            this.Source = paletteUri;
        }

        public string Name { get; set; }

        public Color backgroundcolor1
        {
            get { return (Color) GetValue(backgroundcolor1Property); }
            set { SetValue(backgroundcolor1Property, value); }
        }

        // Using a DependencyProperty as the backing store for backgroundcolor1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty backgroundcolor1Property =
            DependencyProperty.Register("backgroundcolor1", typeof (Color), typeof (Palette), null);

        public Color backgroundcolor2
        {
            get { return (Color) GetValue(backgroundcolor2Property); }
            set { SetValue(backgroundcolor2Property, value); }
        }

        // Using a DependencyProperty as the backing store for backgroundcolor2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty backgroundcolor2Property =
            DependencyProperty.Register("backgroundcolor2", typeof (Color), typeof (Palette), null);

        public Color dashboardcolor1
        {
            get { return (Color) GetValue(dashboardcolor1Property); }
            set { SetValue(dashboardcolor1Property, value); }
        }

        // Using a DependencyProperty as the backing store for dashboardcolor1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty dashboardcolor1Property =
            DependencyProperty.Register("dashboardcolor1", typeof (Color), typeof (Palette), null);

        public Color dashboardcolor2
        {
            get { return (Color) GetValue(dashboardcolor2Property); }
            set { SetValue(dashboardcolor2Property, value); }
        }

        // Using a DependencyProperty as the backing store for dashboardcolor2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty dashboardcolor2Property =
            DependencyProperty.Register("dashboardcolor2", typeof (Color), typeof (Palette), null);

        public Color textcolor
        {
            get { return (Color) GetValue(textcolorProperty); }
            set { SetValue(textcolorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for textcolor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty textcolorProperty =
            DependencyProperty.Register("textcolor", typeof (Color), typeof (Palette), null);

        public Color actualcolor
        {
            get { return (Color) GetValue(actualcolorProperty); }
            set { SetValue(actualcolorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for actualcolor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty actualcolorProperty =
            DependencyProperty.Register("actualcolor", typeof (Color), typeof (Palette), null);

        public Color budgetedcolor
        {
            get { return (Color) GetValue(budgetedcolorProperty); }
            set { SetValue(budgetedcolorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for budgetedcolor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty budgetedcolorProperty =
            DependencyProperty.Register("budgetedcolor", typeof (Color), typeof (Palette), null);

        public Color overcolor
        {
            get { return (Color) GetValue(overcolorProperty); }
            set { SetValue(overcolorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for overcolor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty overcolorProperty =
            DependencyProperty.Register("overcolor", typeof (Color), typeof (Palette), null);

        public Color undercolor
        {
            get { return (Color) GetValue(undercolorProperty); }
            set { SetValue(undercolorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for undercolor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty undercolorProperty =
            DependencyProperty.Register("undercolor", typeof (Color), typeof (Palette), null);

        public Color bikescolor
        {
            get { return (Color) GetValue(bikescolorProperty); }
            set { SetValue(bikescolorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for bikescolor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty bikescolorProperty =
            DependencyProperty.Register("bikescolor", typeof (Color), typeof (Palette), null);

        public Color componenetscolor
        {
            get { return (Color) GetValue(componenetscolorProperty); }
            set { SetValue(componenetscolorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for componenetscolor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty componenetscolorProperty =
            DependencyProperty.Register("componenetscolor", typeof (Color), typeof (Palette), null);

        public Color clothingcolor
        {
            get { return (Color) GetValue(clothingcolorProperty); }
            set { SetValue(clothingcolorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for clothingcolor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty clothingcolorProperty =
            DependencyProperty.Register("clothingcolor", typeof (Color), typeof (Palette), null);

        public Color accessoriescolor
        {
            get { return (Color) GetValue(accessoriescolorProperty); }
            set { SetValue(accessoriescolorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for accessoriescolor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty accessoriescolorProperty =
            DependencyProperty.Register("accessoriescolor", typeof (Color), typeof (Palette), null);

        private Storyboard colorAnimation;

        private Uri _Source;

        public Uri Source
        {
            get { return this._Source; }
            set
            {
                if (this._Source != value)
                {
                    this._Source = value;

                    // Get colors here
                    if (this.Source != null)
                    {
                        object o;

                        ResourceDictionary dict = new ResourceDictionary();
                        if (this.Source.OriginalString.Contains("xaml"))
                        {
                            dict = System.Windows.Application.LoadComponent(this.Source) as ResourceDictionary;
                        }
                        else
                        {
                            string uri = string.Format("/SalesDashboard;component/Palettes/{0}.xaml", this.Source);
                            dict =
                                System.Windows.Application.LoadComponent(new Uri(uri, UriKind.RelativeOrAbsolute)) as
                                    ResourceDictionary;
                        }

                        Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
                        Storyboard story = new Storyboard() {Duration = duration};
                        ColorAnimation animation;
                        IEasingFunction easing = new CircleEase() {EasingMode = EasingMode.EaseOut};

                        o = dict["backgroundcolor1"];
                        if (o is Color)
                        {
                            // this.backgroundcolor1 = (Color)o;
                            animation = new ColorAnimation()
                            {
                                EasingFunction = easing,
                                From = this.backgroundcolor1,
                                To = (Color) o,
                                BeginTime = TimeSpan.Zero,
                                Duration = duration
                            };
                            Storyboard.SetTarget(animation, this);
                            Storyboard.SetTargetProperty(animation, new PropertyPath(Palette.backgroundcolor1Property));
                            story.Children.Add(animation);
                        }

                        o = dict["backgroundcolor2"];
                        if (o is Color)
                        {
                            // this.backgroundcolor2 = (Color)o;
                            animation = new ColorAnimation()
                            {
                                EasingFunction = easing,
                                From = this.backgroundcolor2,
                                To = (Color) o,
                                BeginTime = TimeSpan.Zero,
                                Duration = duration
                            };
                            Storyboard.SetTarget(animation, this);
                            Storyboard.SetTargetProperty(animation, new PropertyPath(Palette.backgroundcolor2Property));
                            story.Children.Add(animation);
                        }

                        o = dict["dashboardcolor1"];
                        if (o is Color)
                        {
                            //this.dashboardcolor1 = (Color)o;
                            animation = new ColorAnimation()
                            {
                                EasingFunction = easing,
                                From = this.dashboardcolor1,
                                To = (Color) o,
                                BeginTime = TimeSpan.Zero,
                                Duration = duration
                            };
                            Storyboard.SetTarget(animation, this);
                            Storyboard.SetTargetProperty(animation, new PropertyPath(Palette.dashboardcolor1Property));
                            story.Children.Add(animation);
                        }

                        o = dict["dashboardcolor2"];
                        if (o is Color)
                        {
                            //this.dashboardcolor2 = (Color)o;
                            animation = new ColorAnimation()
                            {
                                EasingFunction = easing,
                                From = this.dashboardcolor2,
                                To = (Color) o,
                                BeginTime = TimeSpan.Zero,
                                Duration = duration
                            };
                            Storyboard.SetTarget(animation, this);
                            Storyboard.SetTargetProperty(animation, new PropertyPath(Palette.dashboardcolor2Property));
                            story.Children.Add(animation);
                        }

                        o = dict["textcolor"];
                        if (o is Color)
                        {
                            //this.textcolor = (Color)o;
                            animation = new ColorAnimation()
                            {
                                EasingFunction = easing,
                                From = this.textcolor,
                                To = (Color) o,
                                BeginTime = TimeSpan.Zero,
                                Duration = duration
                            };
                            Storyboard.SetTarget(animation, this);
                            Storyboard.SetTargetProperty(animation, new PropertyPath(Palette.textcolorProperty));
                            story.Children.Add(animation);
                        }

                        o = dict["actualcolor"];
                        if (o is Color)
                        {
                            //this.actualcolor = (Color)o;
                            animation = new ColorAnimation()
                            {
                                EasingFunction = easing,
                                From = this.actualcolor,
                                To = (Color) o,
                                BeginTime = TimeSpan.Zero,
                                Duration = duration
                            };
                            Storyboard.SetTarget(animation, this);
                            Storyboard.SetTargetProperty(animation, new PropertyPath(Palette.actualcolorProperty));
                            story.Children.Add(animation);
                        }

                        o = dict["budgetedcolor"];
                        if (o is Color)
                        {
                            //this.budgetedcolor = (Color)o;
                            animation = new ColorAnimation()
                            {
                                EasingFunction = easing,
                                From = this.budgetedcolor,
                                To = (Color) o,
                                BeginTime = TimeSpan.Zero,
                                Duration = duration
                            };
                            Storyboard.SetTarget(animation, this);
                            Storyboard.SetTargetProperty(animation, new PropertyPath(Palette.budgetedcolorProperty));
                            story.Children.Add(animation);
                        }

                        o = dict["overcolor"];
                        if (o is Color)
                        {
                            //this.overcolor = (Color)o;
                            animation = new ColorAnimation()
                            {
                                EasingFunction = easing,
                                From = this.overcolor,
                                To = (Color) o,
                                BeginTime = TimeSpan.Zero,
                                Duration = duration
                            };
                            Storyboard.SetTarget(animation, this);
                            Storyboard.SetTargetProperty(animation, new PropertyPath(Palette.overcolorProperty));
                            story.Children.Add(animation);
                        }

                        o = dict["undercolor"];
                        if (o is Color)
                        {
                            //this.undercolor = (Color)o;
                            animation = new ColorAnimation()
                            {
                                EasingFunction = easing,
                                From = this.undercolor,
                                To = (Color) o,
                                BeginTime = TimeSpan.Zero,
                                Duration = duration
                            };
                            Storyboard.SetTarget(animation, this);
                            Storyboard.SetTargetProperty(animation, new PropertyPath(Palette.undercolorProperty));
                            story.Children.Add(animation);
                        }

                        o = dict["bikescolor"];
                        if (o is Color)
                        {
                            //this.bikescolor = (Color)o;
                            animation = new ColorAnimation()
                            {
                                EasingFunction = easing,
                                From = this.bikescolor,
                                To = (Color) o,
                                BeginTime = TimeSpan.Zero,
                                Duration = duration
                            };
                            Storyboard.SetTarget(animation, this);
                            Storyboard.SetTargetProperty(animation, new PropertyPath(Palette.bikescolorProperty));
                            story.Children.Add(animation);
                        }

                        o = dict["componenetscolor"];
                        if (o is Color)
                        {
                            //this.componenetscolor = (Color)o;
                            animation = new ColorAnimation()
                            {
                                EasingFunction = easing,
                                From = this.componenetscolor,
                                To = (Color) o,
                                BeginTime = TimeSpan.Zero,
                                Duration = duration
                            };
                            Storyboard.SetTarget(animation, this);
                            Storyboard.SetTargetProperty(animation, new PropertyPath(Palette.componenetscolorProperty));
                            story.Children.Add(animation);
                        }

                        o = dict["clothingcolor"];
                        if (o is Color)
                        {
                            //this.clothingcolor = (Color)o;
                            animation = new ColorAnimation()
                            {
                                EasingFunction = easing,
                                From = this.clothingcolor,
                                To = (Color) o,
                                BeginTime = TimeSpan.Zero,
                                Duration = duration
                            };
                            Storyboard.SetTarget(animation, this);
                            Storyboard.SetTargetProperty(animation, new PropertyPath(Palette.clothingcolorProperty));
                            story.Children.Add(animation);
                        }

                        o = dict["accessoriescolor"];
                        if (o is Color)
                        {
                            //this.accessoriescolor = (Color)o;
                            animation = new ColorAnimation()
                            {
                                EasingFunction = easing,
                                From = this.accessoriescolor,
                                To = (Color) o,
                                BeginTime = TimeSpan.Zero,
                                Duration = duration
                            };
                            Storyboard.SetTarget(animation, this);
                            Storyboard.SetTargetProperty(animation, new PropertyPath(Palette.accessoriescolorProperty));
                            story.Children.Add(animation);
                        }

#if SILVERLIGHT
                        o = dict["font"];
#else
                        o = dict["fontWPF"];
#endif
                        if (o is FontFamily)
                        {
                            this.font = (FontFamily) o;
                        }
                        this.colorAnimation = story;
                        this.colorAnimation.Begin();
                    }
                }
            }
        }

        public FontFamily font
        {
            get { return (FontFamily) GetValue(fontProperty); }
            set { SetValue(fontProperty, value); }
        }

        // Using a DependencyProperty as the backing store for font.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty fontProperty =
            DependencyProperty.Register("font", typeof (FontFamily), typeof (Palette), null);
    }
}
