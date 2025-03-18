using System;

namespace Vjezba.Model
{
    public class Osoba
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }

        private string _JMBG;

        private string _OIB;

        public string JMBG
        {
            get { return _JMBG; }
            set
            {
                if (value.Length != 13 || value == null || !System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d+$"))
                {
                    throw new InvalidOperationException("JMBG mora imati 13 znakova");
                }
                _JMBG = value;
            }
        }

        public string OIB
        {
            get { return _OIB; }
            set
            {
                if (value.Length != 11 || value == null || !System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d+$"))
                {
                    throw new InvalidOperationException("OIB mora imati 11 znakova");
                }
                _OIB = value;
            }
        }

        public DateTime DatumRodjenja
        {
            get
            {
                int dan = int.Parse(_JMBG.Substring(0, 2));
                int mjesec = int.Parse(_JMBG.Substring(2, 2));
                int godina = int.Parse(_JMBG.Substring(4, 3));

                if (_JMBG[4] == '0')
                    godina += 2000;
                else
                    godina += 1000;

                return new DateTime(godina, mjesec, dan);
            }
        }

        public override string ToString()
        {
            return Ime + " " + Prezime + " " + JMBG;
        }
    }
}