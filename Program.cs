using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AlgorytmID3
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestoweProbki testoweProbki = new TestoweProbki(); //Generowanie tablicy testowej
            TreeMaker drzewo = new TreeMaker(); //tworzenie drzewa na podstawie danych z pliku tekstowego
        }
    }
    class TestoweProbki
    {
        private List<string> LAtrybuty;
        private List<string> LWarunkiSloneczne;
        private List<string> LTrueFalse;


        private string[,] TablicaWarunkow;
        private int Lprzypadkow;

        private int ProgTemp { get; set; }
        private int ProgWilg { get; set; }

        public TestoweProbki()
        {
            LAtrybuty = new List<string>
            {
                "warunki słoneczne",
                "temperatura",
                "wilgotność",
                "wietrznie"
            };
            LWarunkiSloneczne = new List<string>
            {
                "słonecznie",
                "pochmurno",
                "deszczowo"
            };
            LTrueFalse = new List<string>
            {
                "tak",
                "nie"
            };
            ProgTemp = 10;
            ProgWilg = 75;

            BuildExample(); //buduj przykładowa tablice
            SaveToFIleExample(); //zapisz przykładową tablicę

        }
        private void BuildExample()
        {
            Console.WriteLine("Podaj ilość przypadków do rozpatrzenia: ");
            try
            {
                Lprzypadkow = Int32.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine("To nie jest liczba");
                throw;
            }
            
            TablicaWarunkow = new string[Lprzypadkow,LAtrybuty.Capacity+1];
            for (int i = 0; i < Lprzypadkow; i++)
            {
                TablicaWarunkow[i, 0] = LWarunkiSloneczne[LosujLiczbe(0,LWarunkiSloneczne.Count-1)]; //warunki słoneczne
                TablicaWarunkow[i, 1] = LosujLiczbe(1, 25).ToString(); //temperatura
                TablicaWarunkow[i, 2] = LosujLiczbe(1, 100).ToString(); //wilgotność
                TablicaWarunkow[i, 3] = CzyJestWietrznie(); //wietrznie
                TablicaWarunkow[i, 4] = ZapytanieDoPodjeciaDecyzji(i);
            }


        }
        private int LosujLiczbe(int a, int b)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int lp = random.Next(a, b + 1);
            return lp;
        }
        private int SprawdzTemp()
        {
            if (LosujLiczbe(1, 25)>=10)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        private string CzyJestWietrznie()
        {
            if (LosujLiczbe(1, 2) == 1)
            {
                return "tak";
            }
            else
            {
                return "nie";
            }
        }
        private string ZapytanieDoPodjeciaDecyzji(int i)
        {
            Console.WriteLine("Warunki gry {0} Podejmij decyzję czy grać, POGODA:", i);
            Console.WriteLine("{0}\ttemp: {1}°C\twilgotność: {2}% \twietrznie - {3}\tDECYZJA ???", TablicaWarunkow[i,0], 
                TablicaWarunkow[i, 1], TablicaWarunkow[i, 2], TablicaWarunkow[i, 3]);
            try
            {
                string decyzja = Console.ReadLine();
                if (decyzja == "t" || decyzja == "y")
                {
                    return "tak";
                }
                else { return "nie"; }
             
            }
            catch (Exception e)
            {
                Console.WriteLine("Coś poszło nie tak z odczytaniem znaku, ERROR {0}", e);
                throw;
            }
        }
        private void SaveToFIleExample()
        {
            string path = @"c:\temp\MyTest.txt";
            //Jeżeli plik nie istniej w tej ścieżce
            if (!File.Exists(path))
            {
                //utworzy nowy plik do zapisu
                using (FileStream fs = File.Create(path))
                {
                    fs.Close();
                }
            }
            using (StreamWriter file =
                    new StreamWriter(path, true))
            {
                try
                {
                    for (int i = 0; i < Lprzypadkow; i++)
                    {
                        file.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", TablicaWarunkow[i, 0], TablicaWarunkow[i, 1],
                        TablicaWarunkow[i, 2], TablicaWarunkow[i, 3], TablicaWarunkow[i, 4]);
                    }
                    

                }
                catch (Exception e)
                {
                    Console.WriteLine("Jakiś problem przy zapisie {0}", e);
                }
            }
        }
    }
    class TreeMaker
    {
        private string[,] TablicaDanychPliku;
        private Dictionary<int, string> DslownikwarunkowGornych = new Dictionary<int, string> { };
        private Dictionary<int, string> DslownikwarunkowGornychPoziomy = new Dictionary<int, string> { };
        private Dictionary<string, string> DslownikMozliwosciAtrybut = new Dictionary<string, string> { };
        private string[,] TablicaWystapienDoBudowy;
        List<string> LAtrybuty = new List<string>();
        List<string> LMozliwosci = new List<string>();
        List<int> LIloscMozliwosci = new List<int>();
        List<int> LIKorzenieIWezly = new List<int>();
        List<int> LDecyzjeTak = new List<int>();
        List<int> LDecyzjeNie = new List<int>();
        List<int> LSumaWystapien = new List<int>();
        List<Wezel> ListaWezlow = new List<Wezel>();


        public TreeMaker()
        {
            OdczytajDanezPliku();
            UstalGraniceDecyzji();
            UtworzTabliceID3();
            //TworzDrzewo();
            NoweDrzewo();
        }
        private void OdczytajDanezPliku()
        {
            string path = @"c:\temp\MyTest.txt";
            int lineCount;
            int wordCount = 0;
            lineCount = File.ReadLines(path).Count(); //sprawdzenie ilości lini
            //sprawdzenie ilosci słów w lini
            using (StreamReader file =
                new StreamReader(path, true))
            {
                string line = file.ReadLine();
                foreach (var item in line.Split('\t'))
                {
                    wordCount++;
                }
            }
            TablicaDanychPliku = new string[lineCount, wordCount]; //utworzenie tablicy do przechowywania danych wejściowych dla drzewa
            using (StreamReader file =
                new StreamReader(path, true))
            {
                //wypełnianie powyższej tablicy
                string line;
                int i = 0, j;
                while((line = file.ReadLine())!= null)
                {
                    j = 0;
                    Console.WriteLine(line);
                    foreach (var item in line.Split('\t'))
                    {
                        TablicaDanychPliku[i, j] = item;
                        j++;
                    }
                    i++;
                }
            }
        }
        private void UstalGraniceDecyzji()
        {
            string odpowiedzusera;
            string wartoscgraniczna;
            for (int i = 0; i < TablicaDanychPliku.GetLength(1)-1; i++)
            {
                Console.WriteLine("Czy {0} jest wartością ciągłą i ma swoją granicę? [Tak/Nie]",TablicaDanychPliku[0,i]);
                //odpowiedzusera = Console.ReadLine();
                if (TablicaDanychPliku[0, i] == "warunki słoneczne")
                {
                    odpowiedzusera = "n";
                }
                else if (TablicaDanychPliku[0, i] == "temperatura")
                {
                    odpowiedzusera = "t";
                }
                else if (TablicaDanychPliku[0, i] == "wilgotność")
                {
                    odpowiedzusera = "t";
                }
                else
                {
                    odpowiedzusera = "n";
                }
                if (odpowiedzusera == "T" || odpowiedzusera == "t" || odpowiedzusera=="y" || odpowiedzusera=="Y")
                {
                    //Console.WriteLine("Podaj wartość graniczną: ");
                    //wartoscgraniczna = Console.ReadLine();
                    //DslownikwarunkowGornychPoziomy.Add(i, wartoscgraniczna);
                    if (TablicaDanychPliku[0, i] == "warunki słoneczne")
                    {
                        odpowiedzusera = "n";
                    }
                    else if (TablicaDanychPliku[0, i] == "temperatura")
                    {
                        wartoscgraniczna = "10";
                        DslownikwarunkowGornychPoziomy.Add(i, wartoscgraniczna);
                        DslownikwarunkowGornych.Add(i, "gora");
                    }
                    else if (TablicaDanychPliku[0, i] == "wilgotność")
                    {
                        odpowiedzusera = "t";
                        wartoscgraniczna = "75";
                        DslownikwarunkowGornychPoziomy.Add(i, wartoscgraniczna);
                        DslownikwarunkowGornych.Add(i, "dol");
                    }
                    else
                    {
                        odpowiedzusera = "n";
                    }

                    //Console.WriteLine("Jeśli jest powyżej naciśnij 'T' jeśli poniżej naciśnij 'N'");
                    //odpowiedzusera = Console.ReadLine();
                    //if (odpowiedzusera == "T" || odpowiedzusera == "t" || odpowiedzusera == "y" || odpowiedzusera == "Y")
                    //{
                    //    DslownikwarunkowGornych.Add(i, "gora");
                    //}
                    //else { DslownikwarunkowGornych.Add(i, "dol"); }
                }

            }
        }
        private void UtworzTabliceID3()
        {
            Console.WriteLine("Teraz tworzy tablicę id3");
            TablicaWystapienDoBudowy = new string[9,6]; //do zrobienia przewidywanie wielkosci tablicy
            for (int i = 0; i < TablicaDanychPliku.GetLength(1)-1; i++)
            {
                //sprawdz jakie występują przypadki w kolumnie
                List<string> ListaElementowZnalezionych = new List<string>();
                SPrawdzWystepowaniePrzypadkowKolumny(i, ListaElementowZnalezionych);
                //sprawdz ilość wystąpień tych przypadków
                SprawdzIloscWystapien(i, ListaElementowZnalezionych);
            }
            SumaWystapien();

        }
        private void SPrawdzWystepowaniePrzypadkowKolumny(int NrKolumny, List<string> ListaElementowZnalezionych)
        {
            //List<string> ListaElementowZnalezionych = new List<string>();
            if (DslownikwarunkowGornych.ContainsKey(NrKolumny)!=true)
            {
                //iteruj po całej kolumnie dopoki nie znajdzie nowego elementu
                bool iteruj = true;
                int Ielement = 1;
                int Eelement = Ielement;
                do
                {
                    string element;
                    bool SprawdzElement = true;
                    element = TablicaDanychPliku[Ielement, NrKolumny];
                    foreach (var item in ListaElementowZnalezionych)
                    {
                        if (element == item)
                        {
                            SprawdzElement = false;
                            break;
                        }
                    }
                    if (SprawdzElement == true)
                    {
                        ListaElementowZnalezionych.Add(element);
                        SprawdzElement = false;
                    }
                    if (Ielement == TablicaDanychPliku.GetLength(0) - 1)
                    {
                        iteruj = false;
                    }
                    Ielement++;
                } while (iteruj);
                Console.WriteLine("Znalezione przypadki dla kolumny nr {0}, : ", TablicaDanychPliku[0, NrKolumny]);
                foreach (var item in ListaElementowZnalezionych)
                {
                    Console.WriteLine("{0}, ", item);
                    TablicaWystapienDoBudowy[TablicaWystapienDoBudowy.GetLength(1), 0] = TablicaDanychPliku[0, NrKolumny];
                    TablicaWystapienDoBudowy[TablicaWystapienDoBudowy.GetLength(1), 1] = item;
                    LAtrybuty.Add(TablicaDanychPliku[0, NrKolumny]);
                    LMozliwosci.Add(item);
                }
            }
            else //jeżeli element ma progi (tak/nie)
            {
                ListaElementowZnalezionych.Add("tak");
                ListaElementowZnalezionych.Add("nie");
                Console.WriteLine("Znalezione przypadki dla kolumny nr {0}, : ", TablicaDanychPliku[0, NrKolumny]);
                foreach (var item in ListaElementowZnalezionych)
                {
                    Console.WriteLine("{0}, ", item);
                    TablicaWystapienDoBudowy[TablicaWystapienDoBudowy.GetLength(1), 0] = TablicaDanychPliku[0, NrKolumny];
                    TablicaWystapienDoBudowy[TablicaWystapienDoBudowy.GetLength(1), 1] = item;
                    LAtrybuty.Add(TablicaDanychPliku[0, NrKolumny]);
                    LMozliwosci.Add(item);
                }
            }
        }
        private void SprawdzIloscWystapien(int NrKolumny, List<string> ListaElementowZnalezionych)
        {
            int WskaznikNawyrazPierwszy = 0;
            int WskaznikNawyrazOstatni = 0;
            if (DslownikwarunkowGornych.ContainsKey(NrKolumny) != true)
            {
                //1. SPrawdzenie występowania negatywnego zakończenia dla przypadku
                //bool warunekSprawdzeniaPozytywnego = true;
                //int WskaznikNawyrazPierwszy = 0;
                //int WskaznikNawyrazOstatni = 0;
                string Atrybut = TablicaDanychPliku[0, NrKolumny];
                int LPetli = LMozliwosci.Count - 1;
                for (int i = 0; i < LPetli; i++)
                {
                    if (TablicaDanychPliku[0, NrKolumny] == LAtrybuty[i]) //TablicaDanychPliku[0,NrKolumny]==TablicaWystapienDoBudowy[i,0]
                    {
                        WskaznikNawyrazPierwszy = i;
                        i++;
                        while (TablicaDanychPliku[0, NrKolumny] == LAtrybuty[i] && LAtrybuty.Count - 1 > i) //TablicaDanychPliku[0, NrKolumny] == TablicaWystapienDoBudowy[i, 0] && TablicaDanychPliku.GetLength(0)>=i
                        {
                            i++;
                        }
                        WskaznikNawyrazOstatni = i;
                        break;
                    }
                }
                for (int i = 0; i <= WskaznikNawyrazOstatni - WskaznikNawyrazPierwszy; i++)
                {
                    int licznikWystapieńPozytywnych = 0;
                    int licznikWystapieńNegatywnych = 0;
                    string WyrazDoSprawdzenia = LMozliwosci[WskaznikNawyrazPierwszy + i]; //TablicaDanychPliku[WskaznikNawyrazPierwszy,1]
                    for (int j = 1; j < TablicaDanychPliku.GetLength(0); j++) //int j = 0; j < TablicaDanychPliku.GetLength(1) - 1; j++                    int j = 0; j < LMozliwosci.Count - 1; j++
                    {
                        string sprawdzenie = TablicaDanychPliku[j, NrKolumny];
                        if (sprawdzenie == WyrazDoSprawdzenia) //TablicaDanychPliku[WskaznikNawyrazPierwszy+i,0] == WyrazDoSprawdzenia
                        {
                            if (TablicaDanychPliku[j, 4] == "tak" && WyrazDoSprawdzenia == TablicaDanychPliku[j, NrKolumny])
                            {
                                licznikWystapieńPozytywnych++;
                            }
                            else if (TablicaDanychPliku[j, 4] == "nie" && WyrazDoSprawdzenia == TablicaDanychPliku[j, NrKolumny])
                            {
                                licznikWystapieńNegatywnych++;
                            }
                        }
                    }
                    LDecyzjeNie.Add(licznikWystapieńNegatywnych);
                    LDecyzjeTak.Add(licznikWystapieńPozytywnych);

                }
            }
            else
            {
                string Atrybut = TablicaDanychPliku[0, NrKolumny];
                int poziom = Int32.Parse(DslownikwarunkowGornychPoziomy[NrKolumny]);
                string KiedyPrawda = DslownikwarunkowGornych[NrKolumny];
                int licznikWystapieńPozytywnychTak = 0;
                int licznikWystapieńNegatywnychTak = 0;
                int licznikWystapieńPozytywnychNie = 0;
                int licznikWystapieńNegatywnychNie = 0;
                if (KiedyPrawda == "gora")
                {
                    for (int j = 1; j < TablicaDanychPliku.GetLength(0); j++)
                    {
                        int sprawdzenie = Int32.Parse(TablicaDanychPliku[j, NrKolumny]);
                        if (sprawdzenie>=poziom) //tak
                        {
                            if (TablicaDanychPliku[j,4] == "tak")
                            {
                                licznikWystapieńPozytywnychTak++;
                            }
                            else
                            {
                                licznikWystapieńNegatywnychTak++;
                            }
                        }
                        else //nie
                        {
                            if (TablicaDanychPliku[j, 4] == "tak")
                            {
                                licznikWystapieńPozytywnychNie++;
                            }
                            else
                            {
                                licznikWystapieńNegatywnychNie++;
                            }
                        }
                    }
                    LDecyzjeTak.Add(licznikWystapieńPozytywnychTak);
                    LDecyzjeTak.Add(licznikWystapieńPozytywnychNie);

                    LDecyzjeNie.Add(licznikWystapieńNegatywnychTak);
                    LDecyzjeNie.Add(licznikWystapieńNegatywnychNie);
                }
                else
                {
                    for (int j = 1; j < TablicaDanychPliku.GetLength(0); j++)
                    {
                        int sprawdzenie = Int32.Parse(TablicaDanychPliku[j, NrKolumny]);
                        if (sprawdzenie <= poziom) //tak
                        {
                            if (TablicaDanychPliku[j, 4] == "tak")
                            {
                                licznikWystapieńPozytywnychTak++;
                            }
                            else
                            {
                                licznikWystapieńNegatywnychTak++;
                            }
                        }
                        else //nie
                        {
                            if (TablicaDanychPliku[j, 4] == "tak")
                            {
                                licznikWystapieńPozytywnychNie++;
                            }
                            else
                            {
                                licznikWystapieńNegatywnychNie++;
                            }
                        }
                    }
                    LDecyzjeTak.Add(licznikWystapieńPozytywnychTak);
                    LDecyzjeTak.Add(licznikWystapieńPozytywnychNie);

                    LDecyzjeNie.Add(licznikWystapieńNegatywnychTak);
                    LDecyzjeNie.Add(licznikWystapieńNegatywnychNie);
                }
            }
            if (WskaznikNawyrazPierwszy==0 && WskaznikNawyrazOstatni == 0)
            {
                LIloscMozliwosci.Add(2);
            }
            else
            {
                LIloscMozliwosci.Add(WskaznikNawyrazOstatni - WskaznikNawyrazPierwszy + 1);
            }
        }
        private void SumaWystapien()
        {
            for (int i = 0; i < LMozliwosci.Count; i++)
            {
                LSumaWystapien.Add(LDecyzjeTak[i]+LDecyzjeNie[i]);
            }

        }
        private void TworzDrzewo()
        {
            LIKorzenieIWezly = LIloscMozliwosci;
            bool SzukajPoziom = true;
            bool SzukajLiscie = true;
            int nrWezel = ZnajdzKorzen();
            ListaWezlow.Add(new Wezel() { nrWezla = 0, poziom = 0, rodzicWezel = 0, nazwaLiscia = "korzeń",nazwaWezla = TablicaDanychPliku[0,nrWezel]});
            int poziom = 0;
            WyswietlPoziom(poziom);
            poziom++;
            int Wezel = 1;
            int rodzicWezel = 0;
            while (SzukajPoziom)
            {
                nrWezel = ZnajdzKorzen();
                LIKorzenieIWezly[nrWezel] = 0;
                int liscPoczatek=0;
                for (int i = 0; i < LIloscMozliwosci[nrWezel]; i++)
                {
                    //pobierz liść
                    if (i == 0)
                    {
                        foreach (var item in LAtrybuty)
                        {
                            if (item == TablicaDanychPliku[0,nrWezel])
                            {
                                liscPoczatek = item.IndexOf(TablicaDanychPliku[0, nrWezel]);
                                break;
                            }
                        }
                    }
                    string lisc = LMozliwosci[liscPoczatek];
                    liscPoczatek++;
                }
                //sprawdz czy wezel nie konczy się na tym liściu
                //jeśli tak zakoncz ten wezel
                //jesli nie wbijaj nastepne 
                Console.ReadKey();
            }
            WyswietlPoziom(poziom);
            poziom++;

        }
        private int ZnajdzKorzen() => LIKorzenieIWezly.IndexOf(LIKorzenieIWezly.Max());
        private void WyswietlPoziom(int poziom)
        {
            if (poziom ==0)
            {
                Console.WriteLine("Przed Tobą zaczyna rysować się drzewo decyzyjne wymyślone jedynie przez komputer i jego o wielki procesor");
                Console.WriteLine("KK - korzeń\t LL - liść\t WW - węzeł\n");
                Console.WriteLine("KK{0} {1} {2} {3}",ListaWezlow[0].poziom, ListaWezlow[0].nrWezla, ListaWezlow[0].rodzicWezel, ListaWezlow[0].nazwaWezla);
            }
            else
            {
                List<object> LTymczasowaListaObiektowWezlow = new List<object>();
                string LiniaWezlow = "";
                string LiniaLisci = "";
                foreach (var item in ListaWezlow)
                {
                    if (item.poziom == poziom)
                    {
                        LTymczasowaListaObiektowWezlow.Add(item);
                        LiniaWezlow = LiniaWezlow + "\t" + String.Format("KK{0} {1} {2} {3}", item.poziom, item.nrWezla, item.rodzicWezel, item.nazwaWezla);
                        LiniaLisci = LiniaLisci + "\t" + String.Format("LL {0}",item.nazwaLiscia);
                    }
                }
                Console.WriteLine("{0}\n{1}",LiniaLisci,LiniaWezlow);
            }
            

        }
        private void ZnajdzLiscia()
        {

        }
        private void NoweDrzewo()
        {
            List<object> ListaList = new List<object>(); //zbiór wszystkich gotowych list gałęzi 
            LIKorzenieIWezly = LIloscMozliwosci;
            //1 okreslić korzeń 
            int nrWezel = ZnajdzKorzen();
            string NazwaKorzenia = TablicaDanychPliku[0, nrWezel];
            //2 utworzyć listę gałęzi w której będą dodawane listy z Obiektami liści
            List<List<ObiektLisc>> WszystkieTestowe = new List<List<ObiektLisc>>();
            WszystkieTestowe.Add(new List<ObiektLisc>() { new ObiektLisc() {atrybut = TablicaDanychPliku[0,nrWezel], warunekDoSpelnienia = "korzen", czyToKoniec = false } });
            int nrListyGalezi = 1;
            int[] TablicaGalezi = new int[10];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    List<ObiektLisc> Galezie = new List<ObiektLisc>();
                    int wskaźnikNaLiscKorzen = 0;
                    foreach (var item in LAtrybuty)
                    {
                        if (item == TablicaDanychPliku[0, nrWezel])
                        {
                            wskaźnikNaLiscKorzen = LAtrybuty.IndexOf(item);
                        }
                    }
                    //wersja dla strony tak
                    //od korzenia
                    List<string> AtrybutyDoUzycia = new List<string>() { "warunki słoneczne", "temperatura", "wilgotność", "wietrznie" };
                    List<string> TeCoZostaly = new List<string>();
                    TeCoZostaly = AtrybutyDoUzycia; //przeżucenie atrybutów do użytku
                    int dousuniecia = TeCoZostaly.IndexOf(NazwaKorzenia);
                    TeCoZostaly.RemoveAt(TeCoZostaly.IndexOf(NazwaKorzenia)); //usunięcie korzenia 
                    string ta = LAtrybuty[wskaźnikNaLiscKorzen];
                    string tl = LMozliwosci[wskaźnikNaLiscKorzen+i];
                    bool spelniony;
                    if (j == 0) { spelniony = true; }
                    else { spelniony = false; }
                    bool koniec = false;
                    WszystkieTestowe.Add(new List<ObiektLisc>());
                    WszystkieTestowe[nrListyGalezi].Add(new ObiektLisc() { atrybut = ta, warunekDoSpelnienia = tl, czySpelniony = spelniony, czyToKoniec = koniec });
                    Galezie.Add(new ObiektLisc() { atrybut = ta, warunekDoSpelnienia = tl, czySpelniony = spelniony, czyToKoniec = koniec });
                    //TeCoZostaly.Remove(ta); //usunięcie już dodanego atrybutu
                   
                    do
                    {
                        //wziecie następnego atrybutu i możliwości dla niego
                        if (TeCoZostaly.Count != 0)
                        {
                            ta = TeCoZostaly[0];
                        }
                        //wziecie następnej możliwości
                        int wzkaznikNaLisc = -1;
                        foreach (var item in LAtrybuty)
                        {
                            if (item == ta)
                            {
                                wzkaznikNaLisc = LAtrybuty.IndexOf(item);
                                break;
                            }
                        }
                        tl = LMozliwosci[wzkaznikNaLisc];
                        if (j == 0) { spelniony = true; }
                        if (TeCoZostaly.Count == 1)
                        {
                            koniec = true;
                        }
                        int szukanypoziom = -1;
                        foreach (var item in LAtrybuty)
                        {
                            foreach (var item2 in LMozliwosci)
                            {
                                if (item2 == tl && item == ta)
                                {
                                    szukanypoziom = LMozliwosci.IndexOf(item2);
                                }
                            }
                        }
                        //WszystkieTestowe.Add(new List<ObiektLisc>());
                        if (j == 0 && LDecyzjeTak[szukanypoziom] == 0) //jezeli dla warunku nie mozna grac
                        {
                            koniec = true;
                            string DecyzjaCzyGrac = "Nie";
                            Galezie.Add(new ObiektLisc() { atrybut = ta, warunekDoSpelnienia = tl, czySpelniony = spelniony, czyToKoniec = koniec, DecyzjaCzyGrac = DecyzjaCzyGrac});
                            WszystkieTestowe[nrListyGalezi].Add(new ObiektLisc() { atrybut = ta, warunekDoSpelnienia = tl, czySpelniony = spelniony, czyToKoniec = koniec, DecyzjaCzyGrac = DecyzjaCzyGrac });

                        }
                        else if (TeCoZostaly.Count == 1) // jeżeli jest to ostatni
                        {
                            string DecyzjaCzyGrac = "tak";
                            Galezie.Add(new ObiektLisc() { atrybut = ta, warunekDoSpelnienia = tl, czySpelniony = spelniony, czyToKoniec = koniec });
                            WszystkieTestowe[nrListyGalezi].Add(new ObiektLisc() { atrybut = ta, warunekDoSpelnienia = tl, czySpelniony = spelniony, czyToKoniec = koniec, DecyzjaCzyGrac = DecyzjaCzyGrac });

                        }
                        else
                        {
                            string DecyzjaCzyGrac = "";
                            Galezie.Add(new ObiektLisc() { atrybut = ta, warunekDoSpelnienia = tl, czySpelniony = spelniony, czyToKoniec = koniec });
                            WszystkieTestowe[nrListyGalezi].Add(new ObiektLisc() { atrybut = ta, warunekDoSpelnienia = tl, czySpelniony = spelniony, czyToKoniec = koniec, DecyzjaCzyGrac = DecyzjaCzyGrac });
                        }
                        //koniec = false;
                        
                        TeCoZostaly.Remove(ta); //usunięcie już dodanego atrybutu
                        //Console.ReadKey();
                                                            
                    } while (TeCoZostaly.Count != 0 || koniec != true);
                    //dodaj listę do listy galęzi
                    ListaList.Add(Galezie);
                    nrListyGalezi++;
                    //WszystkieTestowe.Add(new List<ObiektLisc>());
                }
            }
            //wyswitl zbior 
            foreach (var item in WszystkieTestowe)
            {
                Console.WriteLine("Gałąź decyzji nr:{0}", WszystkieTestowe.IndexOf(item));
                foreach (var item2 in item)
                {
                    Console.WriteLine("At: {0}\tWar: {1}\tKoniec: {2}\tGrać? {3}",item2.atrybut, item2.warunekDoSpelnienia, item2.czyToKoniec, item2.DecyzjaCzyGrac);
                }
            }
            Console.ReadKey();

        }
    }
    
    public class Wezel
    {
        public int nrWezla { get; set; }
        public int poziom { get; set; }
        public int rodzicWezel { get; set; }
        public string nazwaWezla { get; set; }
        public string nazwaLiscia { get; set; }
    }
    public class ObiektLisc
    {
        public string atrybut { get; set; }
        public string warunekDoSpelnienia { get; set; }
        public bool czySpelniony { get; set; }
        public bool czyToKoniec { get; set; }
        public string DecyzjaCzyGrac { get; set; } 
        public List<string> PozostaleAtrybuty { get => pozostaleAtrybuty; set => pozostaleAtrybuty = value; }
        private List<string> pozostaleAtrybuty = new List<string>();
    }
    public class AtrybutyWarunki
    {
        public string NazwaAtrybutu { get; set; }
        public List<string> MozliweWybory = new List<string>();
    }
    
}
