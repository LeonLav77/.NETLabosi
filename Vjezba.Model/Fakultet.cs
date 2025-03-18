using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vjezba.Model
{
    public class Fakultet
    {
        public List<Osoba> Osobe { get; set; }

        public Fakultet()
        {
            Osobe = new List<Osoba>();
        }

        public void DodajOsobu(Osoba osoba)
        {
            Osobe.Add(osoba);
        }

        public int KolikoProfesora()
        {
            return Osobe.Count(o => o is Profesor);
        }

        public int KolikoStudenata()
        {
            return Osobe.Count(o => o is Student);
        }

        public Osoba DohvatiStudenta(string jmbag)
        {
            return Osobe.FirstOrDefault(o => o is Student && ((Student)o).JMBAG == jmbag);
        }

        public IEnumerable<Profesor> DohvatiProfesore()
        {
            return Osobe.Where(o => o is Profesor).OrderBy(o => ((Profesor)o).DatumIzbora).Cast<Profesor>();
        }

        public IEnumerable<Student> DohvatiStudente91()
        {
            return Osobe.Where(o => o is Student && ((Student)o).DatumRodjenja.Year > 1991).Cast<Student>();
        }

        public IEnumerable<Student> DohvatiStudente91NoLinq()
        {
            List<Student> studenti = new List<Student>();

            foreach (Osoba osoba in Osobe)
            {
                if (osoba is Student)
                {
                    Student student = (Student)osoba;
                    if (student.DatumRodjenja.Year > 1991)
                    {
                        studenti.Add(student);
                    }
                }
            }
            return studenti;
        }

        public IEnumerable<Student> StudentiNeTvzD()
        {
            return Osobe.Where(o => o is Student && !((Student)o).JMBAG.StartsWith("0246") && ((Student)o).Prezime.StartsWith('D')).Cast<Student>();
        }

        public List<Student> DohvatiStudente91List()
        {
            return Osobe.OfType<Student>().Where(s => s.DatumRodjenja.Year > 1991).ToList();
        }

        public Student NajboljiProsjek(int god_rodenja)
        {
            return Osobe.OfType<Student>().Where(s => s.DatumRodjenja.Year == god_rodenja).OrderByDescending(s => s.Prosjek).FirstOrDefault();
        }

        public List<Student> StudentiGodinaOrdered(int god)
        {
            return Osobe.OfType<Student>().Where(s => s.DatumRodjenja.Year == god).OrderByDescending(s => s.Prosjek).ToList();
        }


        public List<Profesor> SviProfesori(bool asc)
        {
            return asc
                ? Osobe.OfType<Profesor>().OrderBy(p => p.Prezime).ThenBy(p => p.Ime).ToList()
                : Osobe.OfType<Profesor>().OrderByDescending(p => p.Prezime).ThenByDescending(p => p.Ime).ToList();
        }

        public int  KolikoProfesoraUZvanju(Zvanje zvanje) 
        {
            return Osobe.OfType<Profesor>().Count(p => p.Zvanje == zvanje);
        }

        public List<Profesor> NeaktivniProfesori(int x)
        {
            return Osobe.OfType<Profesor>().Where(p => (p.Zvanje == Zvanje.Predavac || p.Zvanje == Zvanje.VisiPredavac) && p.Predmeti.Count < x).ToList();
        }

        public IEnumerable<Profesor> AktivniAsistenti(int x, int minEcts)
        {
            return Osobe.OfType<Profesor>().Where(p => p.Zvanje == Zvanje.Asistent && p.getAmountOfSubjectsWithMoreThan(minEcts) > x);
        }

        public void IzmjeniProfesore(Action<Profesor> action)
        {
            Osobe.OfType<Profesor>().ToList().ForEach(action);
        }

    }
}