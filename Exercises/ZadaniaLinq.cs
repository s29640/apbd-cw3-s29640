using LinqConsoleLab.PL.Data;

namespace LinqConsoleLab.PL.Exercises;

public sealed class ZadaniaLinq
{
    /// <summary>
    /// Zadanie:
    /// Wyszukaj wszystkich studentów mieszkających w Warsaw.
    /// Zwróć numer indeksu, pełne imię i nazwisko oraz miasto.
    ///
    /// SQL:
    /// SELECT NumerIndeksu, Imie, Nazwisko, Miasto
    /// FROM Studenci
    /// WHERE Miasto = 'Warsaw';
    /// </summary>
    public IEnumerable<string> Zadanie01_StudenciZWarszawy()
    {
        return DaneUczelni.Studenci
            .Where(s => s.Miasto == "Warsaw")
            .Select(s => $"{s.NumerIndeksu} | {s.Imie} | {s.Nazwisko} | {s.Miasto}");
    }

    /// <summary>
    /// Zadanie:
    /// Przygotuj listÄ™ adresĂłw e-mail wszystkich studentĂłw.
    /// UĹĽyj projekcji, tak aby w wyniku nie zwracaÄ‡ caĹ‚ych obiektĂłw.
    ///
    /// SQL:
    /// SELECT Email
    /// FROM Studenci;
    /// </summary>
    public IEnumerable<string> Zadanie02_AdresyEmailStudentow()
    {
        return DaneUczelni.Studenci
            .Select(s => s.Email);
    }

    /// <summary>
    /// Zadanie:
    /// Posortuj studentĂłw alfabetycznie po nazwisku, a nastÄ™pnie po imieniu.
    /// ZwrĂłÄ‡ numer indeksu i peĹ‚ne imiÄ™ i nazwisko.
    ///
    /// SQL:
    /// SELECT NumerIndeksu, Imie, Nazwisko
    /// FROM Studenci
    /// ORDER BY Nazwisko, Imie;
    /// </summary>
    public IEnumerable<string> Zadanie03_StudenciPosortowani()
    {
        return DaneUczelni.Studenci
            .OrderBy(s => s.Nazwisko)
            .ThenBy(s => s.Imie)
            .Select(s => $"{s.NumerIndeksu} | {s.Imie} | {s.Nazwisko}");
    }

    /// <summary>
    /// Zadanie:
    /// ZnajdĹş pierwszy przedmiot z kategorii Analytics.
    /// JeĹĽeli taki przedmiot nie istnieje, zwrĂłÄ‡ komunikat tekstowy.
    ///
    /// SQL:
    /// SELECT TOP 1 Nazwa, DataStartu
    /// FROM Przedmioty
    /// WHERE Kategoria = 'Analytics';
    /// </summary>
    public IEnumerable<string> Zadanie04_PierwszyPrzedmiotAnalityczny()
    {
        var p = DaneUczelni.Przedmioty
            .FirstOrDefault(p => p.Kategoria == "Analytics");

        return  p != null
                ? [$"{p.Nazwa} | {p.DataStartu:yyyy-MM-dd}"]
                : ["Brak danych"];
    }

    /// <summary>
    /// Zadanie:
    /// SprawdĹş, czy w danych istnieje przynajmniej jeden nieaktywny zapis.
    /// ZwrĂłÄ‡ jedno zdanie z odpowiedziÄ… True/False albo Tak/Nie.
    ///
    /// SQL:
    /// SELECT CASE WHEN EXISTS (
    ///     SELECT 1
    ///     FROM Zapisy
    ///     WHERE CzyAktywny = 0
    /// ) THEN 1 ELSE 0 END;
    /// </summary>
    public IEnumerable<string> Zadanie05_CzyIstniejeNieaktywneZapisanie()
    {
        var exists = DaneUczelni.Zapisy.Any(z => !z.CzyAktywny);
        return [$"{exists}"];
    }

    /// <summary>
    /// Zadanie:
    /// SprawdĹş, czy kaĹĽdy prowadzÄ…cy ma uzupeĹ‚nionÄ… nazwÄ™ katedry.
    /// Warto uĹĽyÄ‡ metody, ktĂłra weryfikuje warunek dla caĹ‚ej kolekcji.
    ///
    /// SQL:
    /// SELECT CASE WHEN COUNT(*) = COUNT(Katedra)
    /// THEN 1 ELSE 0 END
    /// FROM Prowadzacy;
    /// </summary>
    public IEnumerable<string> Zadanie06_CzyWszyscyProwadzacyMajaKatedre()
    {
        var all = DaneUczelni.Prowadzacy.All(p => !string.IsNullOrWhiteSpace(p.Katedra));
        return [$"{all}"];
    }

    /// <summary>
    /// Zadanie:
    /// Policz, ile aktywnych zapisĂłw znajduje siÄ™ w systemie.
    ///
    /// SQL:
    /// SELECT COUNT(*)
    /// FROM Zapisy
    /// WHERE CzyAktywny = 1;
    /// </summary>
    public IEnumerable<string> Zadanie07_LiczbaAktywnychZapisow()
    {
        return [$"{DaneUczelni.Zapisy.Count(z => z.CzyAktywny)}"];
    }

    /// <summary>
    /// Zadanie:
    /// Pobierz listÄ™ unikalnych miast studentĂłw i posortuj jÄ… rosnÄ…co.
    ///
    /// SQL:
    /// SELECT DISTINCT Miasto
    /// FROM Studenci
    /// ORDER BY Miasto;
    /// </summary>
    public IEnumerable<string> Zadanie08_UnikalneMiastaStudentow()
    {
        return DaneUczelni.Studenci
            .Select(s => s.Miasto)
            .Distinct();
    }

    /// <summary>
    /// Zadanie:
    /// ZwrĂłÄ‡ trzy najnowsze zapisy na przedmioty.
    /// W wyniku pokaĹĽ datÄ™ zapisu, identyfikator studenta i identyfikator przedmiotu.
    ///
    /// SQL:
    /// SELECT TOP 3 DataZapisu, StudentId, PrzedmiotId
    /// FROM Zapisy
    /// ORDER BY DataZapisu DESC;
    /// </summary>
    public IEnumerable<string> Zadanie09_TrzyNajnowszeZapisy()
    {
        return DaneUczelni.Zapisy
            .OrderByDescending(z => z.DataZapisu)
            .Take(3)
            .Select(z => $"{z.StudentId} | {z.PrzedmiotId} | {z.DataZapisu:yyyy-MM-dd}");
    }

    /// <summary>
    /// Zadanie:
    /// Zaimplementuj prostÄ… paginacjÄ™ dla listy przedmiotĂłw.
    /// ZaĹ‚ĂłĹĽ stronÄ™ o rozmiarze 2 i zwrĂłÄ‡ drugÄ… stronÄ™ danych.
    ///
    /// SQL:
    /// SELECT Nazwa, Kategoria
    /// FROM Przedmioty
    /// ORDER BY Nazwa
    /// OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY;
    /// </summary>
    public IEnumerable<string> Zadanie10_DrugaStronaPrzedmiotow()
    {
        return DaneUczelni.Przedmioty
            .OrderBy(p => p.Nazwa)
            .Skip(2)
            .Take(2)
            .Select(p => $"{p.Nazwa} | {p.Kategoria}");
    }

    /// <summary>
    /// Zadanie:
    /// PoĹ‚Ä…cz studentĂłw z zapisami po StudentId.
    /// ZwrĂłÄ‡ peĹ‚ne imiÄ™ i nazwisko studenta oraz datÄ™ zapisu.
    ///
    /// SQL:
    /// SELECT s.Imie, s.Nazwisko, z.DataZapisu
    /// FROM Studenci s
    /// JOIN Zapisy z ON s.Id = z.StudentId;
    /// </summary>
    public IEnumerable<string> Zadanie11_PolaczStudentowIZapisy()
    {
        return DaneUczelni.Studenci
            .Join(
                DaneUczelni.Zapisy,
                s => s.Id,
                z => z.StudentId,
                (s, z) => $"{s.Imie} | {s.Nazwisko} | {z.DataZapisu:yyyy-MM-dd}"
            );
    }

    /// <summary>
    /// Zadanie:
    /// Przygotuj wszystkie pary student-przedmiot na podstawie zapisĂłw.
    /// UĹĽyj podejĹ›cia, ktĂłre pozwoli spĹ‚aszczyÄ‡ dane do jednej sekwencji wynikĂłw.
    ///
    /// SQL:
    /// SELECT s.Imie, s.Nazwisko, p.Nazwa
    /// FROM Zapisy z
    /// JOIN Studenci s ON s.Id = z.StudentId
    /// JOIN Przedmioty p ON p.Id = z.PrzedmiotId;
    /// </summary>
    public IEnumerable<string> Zadanie12_ParyStudentPrzedmiot()
    {
        return DaneUczelni.Zapisy
            .Join(DaneUczelni.Studenci,
                z => z.StudentId,
                s => s.Id,
                (z, s) => new { z, s })
            .Join(DaneUczelni.Przedmioty,
                zs => zs.z.PrzedmiotId,
                p => p.Id,
                (zs, p) => $"{zs.s.Imie} | {zs.s.Nazwisko} | {p.Nazwa}");
    }

    /// <summary>
    /// Zadanie:
    /// Pogrupuj zapisy wedĹ‚ug przedmiotu i zwrĂłÄ‡ nazwÄ™ przedmiotu oraz liczbÄ™ zapisĂłw.
    ///
    /// SQL:
    /// SELECT p.Nazwa, COUNT(*)
    /// FROM Zapisy z
    /// JOIN Przedmioty p ON p.Id = z.PrzedmiotId
    /// GROUP BY p.Nazwa;
    /// </summary>
    public IEnumerable<string> Zadanie13_GrupowanieZapisowWedlugPrzedmiotu()
    {
        return DaneUczelni.Zapisy
            .Join(DaneUczelni.Przedmioty,
                z => z.PrzedmiotId,
                p => p.Id,
                (z, p) => p.Nazwa)
            .GroupBy(n => n)
            .Select(g => $"{g.Key} | {g.Count()}");
    }

    /// <summary>
    /// Zadanie:
    /// Oblicz Ĺ›redniÄ… ocenÄ™ koĹ„cowÄ… dla kaĹĽdego przedmiotu.
    /// PomiĹ„ rekordy, w ktĂłrych ocena koĹ„cowa ma wartoĹ›Ä‡ null.
    ///
    /// SQL:
    /// SELECT p.Nazwa, AVG(z.OcenaKoncowa)
    /// FROM Zapisy z
    /// JOIN Przedmioty p ON p.Id = z.PrzedmiotId
    /// WHERE z.OcenaKoncowa IS NOT NULL
    /// GROUP BY p.Nazwa;
    /// </summary>
    public IEnumerable<string> Zadanie14_SredniaOcenaNaPrzedmiot()
    {
        return DaneUczelni.Zapisy
                .Where(z => z.OcenaKoncowa != null)
                .Join(DaneUczelni.Przedmioty,
                    z => z.PrzedmiotId,
                    p => p.Id,
                    (z, p) => new { p.Nazwa, z.OcenaKoncowa })
                .GroupBy(x => x.Nazwa)
                .Select(g => $"{g.Key} | {g.Average(x => x.OcenaKoncowa)}");
    }

    /// <summary>
    /// Zadanie:
    /// Dla kaĹĽdego prowadzÄ…cego policz liczbÄ™ przypisanych przedmiotĂłw.
    /// W wyniku zwrĂłÄ‡ peĹ‚ne imiÄ™ i nazwisko oraz liczbÄ™ przedmiotĂłw.
    ///
    /// SQL:
    /// SELECT pr.Imie, pr.Nazwisko, COUNT(p.Id)
    /// FROM Prowadzacy pr
    /// LEFT JOIN Przedmioty p ON p.ProwadzacyId = pr.Id
    /// GROUP BY pr.Imie, pr.Nazwisko;
    /// </summary>
    public IEnumerable<string> Zadanie15_ProwadzacyILiczbaPrzedmiotow()
    {
        return DaneUczelni.Prowadzacy
        .Select(p => $"{p.Imie} | {p.Nazwisko} | {DaneUczelni.Przedmioty.Count(pr => pr.ProwadzacyId == p.Id)}");
    }

    /// <summary>
    /// Zadanie:
    /// Dla kaĹĽdego studenta znajdĹş jego najwyĹĽszÄ… ocenÄ™ koĹ„cowÄ….
    /// PomiĹ„ studentĂłw, ktĂłrzy nie majÄ… jeszcze ĹĽadnej oceny.
    ///
    /// SQL:
    /// SELECT s.Imie, s.Nazwisko, MAX(z.OcenaKoncowa)
    /// FROM Studenci s
    /// JOIN Zapisy z ON s.Id = z.StudentId
    /// WHERE z.OcenaKoncowa IS NOT NULL
    /// GROUP BY s.Imie, s.Nazwisko;
    /// </summary>
    public IEnumerable<string> Zadanie16_NajwyzszaOcenaKazdegoStudenta()
    {
        return DaneUczelni.Zapisy
            .Where(z => z.OcenaKoncowa != null)
            .Join(DaneUczelni.Studenci,
                z => z.StudentId,
                s => s.Id,
                (z, s) => new { s.Imie, s.Nazwisko, z.OcenaKoncowa })
            .GroupBy(x => new { x.Imie, x.Nazwisko })
            .Select(g => $"{g.Key.Imie} | {g.Key.Nazwisko} | {g.Max(x => x.OcenaKoncowa)}");
    }

    /// <summary>
    /// Wyzwanie:
    /// ZnajdĹş studentĂłw, ktĂłrzy majÄ… wiÄ™cej niĹĽ jeden aktywny zapis.
    /// ZwrĂłÄ‡ peĹ‚ne imiÄ™ i nazwisko oraz liczbÄ™ aktywnych przedmiotĂłw.
    ///
    /// SQL:
    /// SELECT s.Imie, s.Nazwisko, COUNT(*)
    /// FROM Studenci s
    /// JOIN Zapisy z ON s.Id = z.StudentId
    /// WHERE z.CzyAktywny = 1
    /// GROUP BY s.Imie, s.Nazwisko
    /// HAVING COUNT(*) > 1;
    /// </summary>
    public IEnumerable<string> Wyzwanie01_StudenciZWiecejNizJednymAktywnymPrzedmiotem()
    {
        return DaneUczelni.Zapisy
            .Where(z => z.CzyAktywny)
            .GroupBy(z => z.StudentId)
            .Where(g => g.Count() > 1)
            .Join(DaneUczelni.Studenci,
                g => g.Key,
                s => s.Id,
                (g, s) => $"{s.Imie} | {s.Nazwisko} | {g.Count()}");
    }

    /// <summary>
    /// Wyzwanie:
    /// Wypisz przedmioty startujÄ…ce w kwietniu 2026, dla ktĂłrych ĹĽaden zapis nie ma jeszcze oceny koĹ„cowej.
    ///
    /// SQL:
    /// SELECT p.Nazwa
    /// FROM Przedmioty p
    /// JOIN Zapisy z ON p.Id = z.PrzedmiotId
    /// WHERE MONTH(p.DataStartu) = 4 AND YEAR(p.DataStartu) = 2026
    /// GROUP BY p.Nazwa
    /// HAVING SUM(CASE WHEN z.OcenaKoncowa IS NOT NULL THEN 1 ELSE 0 END) = 0;
    /// </summary>
    public IEnumerable<string> Wyzwanie02_PrzedmiotyStartujaceWKwietniuBezOcenKoncowych()
    {
        return DaneUczelni.Przedmioty
            .Where(p => p.DataStartu.Month == 4 && p.DataStartu.Year == 2026)
            .Where(p => !DaneUczelni.Zapisy
                .Any(z => z.PrzedmiotId == p.Id && z.OcenaKoncowa != null))
            .Select(p => p.Nazwa);
    }

    /// <summary>
    /// Wyzwanie:
    /// Oblicz Ĺ›redniÄ… ocen koĹ„cowych dla kaĹĽdego prowadzÄ…cego na podstawie wszystkich jego przedmiotĂłw.
    /// PomiĹ„ brakujÄ…ce oceny, ale pozostaw samych prowadzÄ…cych w wyniku.
    ///
    /// SQL:
    /// SELECT pr.Imie, pr.Nazwisko, AVG(z.OcenaKoncowa)
    /// FROM Prowadzacy pr
    /// LEFT JOIN Przedmioty p ON p.ProwadzacyId = pr.Id
    /// LEFT JOIN Zapisy z ON z.PrzedmiotId = p.Id
    /// WHERE z.OcenaKoncowa IS NOT NULL
    /// GROUP BY pr.Imie, pr.Nazwisko;
    /// </summary>
    public IEnumerable<string> Wyzwanie03_ProwadzacyISredniaOcenNaIchPrzedmiotach()
    {
        return DaneUczelni.Prowadzacy
            .Select(p =>
            {
                var avg = DaneUczelni.Przedmioty
                    .Where(pr => pr.ProwadzacyId == p.Id)
                    .Join(DaneUczelni.Zapisy.Where(z => z.OcenaKoncowa != null),
                        pr => pr.Id,
                        z => z.PrzedmiotId,
                        (pr, z) => z.OcenaKoncowa.Value);

                return $"{p.Imie} | {p.Nazwisko} | {(avg.Any() ? avg.Average() : 0)}";
            });
    }

    /// <summary>
    /// Wyzwanie:
    /// PokaĹĽ miasta studentĂłw oraz liczbÄ™ aktywnych zapisĂłw wykonanych przez studentĂłw z danego miasta.
    /// Posortuj wynik malejÄ…co po liczbie aktywnych zapisĂłw.
    ///
    /// SQL:
    /// SELECT s.Miasto, COUNT(*)
    /// FROM Studenci s
    /// JOIN Zapisy z ON s.Id = z.StudentId
    /// WHERE z.CzyAktywny = 1
    /// GROUP BY s.Miasto
    /// ORDER BY COUNT(*) DESC;
    /// </summary>
    public IEnumerable<string> Wyzwanie04_MiastaILiczbaAktywnychZapisow()
    {
        return DaneUczelni.Zapisy
            .Where(z => z.CzyAktywny)
            .Join(DaneUczelni.Studenci,
                z => z.StudentId,
                s => s.Id,
                (z, s) => s.Miasto)
            .GroupBy(m => m)
            .Select(g => $"{g.Key} | {g.Count()}");
    }

    private static NotImplementedException Niezaimplementowano(string nazwaMetody)
    {
        return new NotImplementedException(
            $"UzupeĹ‚nij metodÄ™ {nazwaMetody} w pliku Exercises/ZadaniaLinq.cs i uruchom polecenie ponownie.");
    }
}
