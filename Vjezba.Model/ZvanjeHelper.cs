namespace Vjezba.Model
{
    public static class ZvanjeHelper
    {
        public static int getNumberOfYearsTillReelection(Zvanje zvanje)
        {
            switch (zvanje)
            {
                case Zvanje.Asistent:
                    return 4;
                default:
                    return 5;
            }
        }
    }
}