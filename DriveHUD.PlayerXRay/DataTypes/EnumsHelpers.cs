namespace DriveHUD.PlayerXRay.DataTypes
{
    public static class EnumsHelpers
    {
        public static string GetCompareOperator(CompareEnum comp)
        {
            switch (comp)
            {
                case CompareEnum.EqualTo:
                    return "=";
                case CompareEnum.GreaterThan:
                    return ">";
                case CompareEnum.LessThan:
                    return "<";
            }
            return ">";
        }
    }
}