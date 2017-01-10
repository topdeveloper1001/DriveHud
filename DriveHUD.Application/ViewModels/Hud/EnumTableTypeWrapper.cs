using DriveHUD.Common.Resources;
using Model.Enums;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class EnumTableTypeWrapper
    {
        public EnumTableType TableType { get; }
        public string TableTypeText { get; } 

        public EnumTableTypeWrapper(EnumTableType tableType)
        {
            TableType = tableType;
            TableTypeText = CommonResourceManager.Instance.GetEnumResource(TableType);
        }
    }
}