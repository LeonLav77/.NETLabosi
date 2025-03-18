using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vjezba.Model
{
    public class Profesor : Osoba
    {
        public string Odjel { get; set; }

        public Zvanje Zvanje { get; set; }

        public DateTime DatumIzbora { get; set; }

        private List<Predmet> _predmeti;

        public List<Predmet> Predmeti
        {
            get 
            { 
                if (_predmeti == null)
                    _predmeti = new List<Predmet>();
                return _predmeti;
            }
            set { _predmeti = value; }
        }

        public int KolikoDoReizbora()
        {
            DateTime datumReizbora = DatumIzbora.AddYears(ZvanjeHelper.getNumberOfYearsTillReelection(Zvanje));
            TimeSpan timeRemaining = datumReizbora.Subtract(DateTime.Now);
            return (int)(timeRemaining.TotalDays / 365.25); 
        }

        public int getAmountOfSubjectsWithMoreThan(int ects)
        {
            return Predmeti.Count(p => p.ECTS >= ects);
        }
    }
}