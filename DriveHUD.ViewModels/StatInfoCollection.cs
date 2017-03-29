using Model;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace DriveHUD.ViewModels
{

    public class CategoryViewModel
    {
        private string _title;
        private ObservableCollection<StatInfo> _items;

        public static ObservableCollection<CategoryViewModel> Generate()
        {
            CategoryViewModel latest = new CategoryViewModel();
            latest.Caption = "Most Important";

            ObservableCollection<StatInfo> animalList = new ObservableCollection<StatInfo>();
            latest.Items.Add(new StatInfo() { Caption = "VPIP%", PropertyName = "VPIP%", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "PFR%", PropertyName = "PFR", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "3-bet%", PropertyName = "ThreeBet", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "AGG%", PropertyName = "AGG", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "C-bet%", PropertyName = "CBet", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "WTSD", PropertyName = "WTSD", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "W$SD", PropertyName = "WsSD", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "WWSF", PropertyName = "WWSF", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "TH", PropertyName = "TotalHands", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "Fold to C-Bet%", PropertyName = "FoldCBet", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "Fold to 3-bet%", PropertyName = "FoldThreeBet", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "4-bet%", PropertyName = "FourBet", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "Fold to 4-bet%", PropertyName = "FoldFourBet", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "Flop AGG%", PropertyName = "FlopAgg", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "Turn AGG%", PropertyName = "TurnAgg", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "River AGG%", PropertyName = "RiverAgg", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "Cold Call%", PropertyName = "ColdCall", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "Steal%", PropertyName = "Steal", Category = latest.Caption });
            latest.Items.Add(new StatInfo() { Caption = "Fold to Steal%", PropertyName = "FoldSteal", Category = latest.Caption });        

            ObservableCollection<CategoryViewModel> result = new ObservableCollection<CategoryViewModel>();

            result.Add(latest);      

            return result;
        }

        public CategoryViewModel()
        {
            Items = new ObservableCollection<StatInfo>();
        }

        public string Caption
        {
            get
            {
                return this._title;
            }
            set
            {
                this._title = value;
            }
        }
        public ObservableCollection<StatInfo> Items
        {
            get
            {
                return _items;
            }
            set
            {
                this._items = value;
            }
        }
    }

    //public class Animal
    //{
    //    public Animal(string name, Category category)
    //    {
    //        this.Name = name;
    //        this.Category = category;
    //    }
    //    public Animal()
    //    {

    //    }
    //    public string Name
    //    {
    //        get;
    //        set;
    //    }
    //    public Category Category
    //    {
    //        get;
    //        set;
    //    }

    //    public StatObservableCollection<StatInfo> GetAnimalList()
    //    {
    //        StatObservableCollection<StatInfo> animalList = new StatObservableCollection<StatInfo>();
    //        animalList.Add(new StatInfo("3Bet%", "ThreeBet", Category.MostPopular));
    //        animalList.Add(new StatInfo("VPIP", "VPIP", Category.MostPopular));
    //        animalList.Add(new StatInfo("PFR", "PFR", Category.MostPopular));
    //        animalList.Add(new StatInfo("3Bet%", "ThreeBet", Category.MostPopular));
    //        animalList.Add(new StatInfo("WTSD", "WTSD", Category.MostPopular));
    //        animalList.Add(new StatInfo("Agg", "Agg", Category.MostPopular));
    //        animalList.Add(new StatInfo("TH", "TotalHands", Category.MostPopular));
    //        return animalList;
    //    }
    //}
    //public enum Category
    //{
    //    MostPopular
    //}
}
