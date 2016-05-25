[Plik pomocy po angielsku | English README](README.md)

BCDD - BCalc Double Dummy
=========================

Analiza "w widne" rozkładów zapisanych w formacie PBN przy użyciu BCalc.

Opis ogólny
===========

Program pełni funkcję alternatywy dla sugerowanej przez BigDeal analizy rozdań przy użyciu Deep Finesse dla rozdań, w których BigDeal stwierdził brak wyników takiej analizy w drukowanym pliku PBN.

Jego główne funkcjonalności, i przewaga nad rozwiązaniem z BigDeal, to:

 * lepsza wydajność i stabilność
 * możliwość wsadowej analizy wielu plików
 * obsługa niestandardowych JFR-owych tagów PBN dla wyników analizy oraz [standardowych tagów PBN](http://www.tistis.nl/pbn/pbn_v21.txt)
 * dodatkowa obsługa importu danych z niestandardowego tagu PBN programu Double Dummy Solver (`OptimumResult`)
 * automatyczna konwersja danych w powyższych formatach

Analiza "w widne" używa biblioteki będącej częścią projektu [BCalc](http://bcalc.w8.pl) - `libbcalcdds` (biblioteka ta jest wymagana do uruchomienia programu).

Instalacja
==========

 1. Pobierz program.
 2. Rozpakuj go do folderu docelowego.
 3. [Pobierz bibliotekę `libbcalcdds.dll`](http://bcalc.w8.pl/download/API_C/) odpowiednią dla używanej architektury procesora (aplikacja działa zarówno z wersją win32, jak i win64) i zapisz ją w katalogu programu lub w dowolnym innym katalogu systemowej ścieżki ładowania bibliotek.
 4. Powinno styknąć.

BCDD jest aplikacją .NET 3.5 i wymaga [odpowiedniego środowiska](https://www.microsoft.com/download/details.aspx?id=21) do prawidłowego funkcjonowania.

Użycie
======

Ścieżkę(i) pliku(ów) PBN może podać programowi jako jego argument(y) linii poleceń:

    BCDD.exe [FILE_1 [FILE_2...]]

W przypadku braku argumentów (lub podania samych nieprawidłowych ścieżek), aplikacja pyta o pliki standardowym, systemowym oknem otwarcia plików.

Wszystkie pliki nadpisywane są w miejscu (ale za pośrednictwem pliku tymczasowego, więc w przypadku katastrofalnego błędu, oryginał nie zostanie nadpisany).

Uwaga: Jeśli pojedyncze rozdania pliku wejściowego okażą się błędne, zostaną one pominięte w pliku wyjściowym.

Szczegóły implementacji
=======================

Rezultaty analizy "w widne" podlegają kilku umownym kwestiom. W przypadku jakichkolwiek niejednoznaczności, głównym priorytetem aplikacji było zachowanie zgodności z wynikami generowanymi przez BigDeal.

Konwencje przyjęte w programie:

 * w rozdaniach z tym samym optymalnym kontraktem dla obu stron jako minimaks przyjmuje się kontrakt dla strony rozdającej (w przeciwieństwie do koncepcji [Richarda Pavlicka](http://www.rpbridge.net/7a23.htm) przypisującej 4 pasy jako minimaks w tej sytuacji)
 * ze wszystkich równych kontraktów (tj. kontraktów wartych tyle samo i nieposiadających opłacalnej obrony, lub kontraktów będących równie opłacalnymi obronami), *najwyższy* z nich jest traktowany jako minimaks
 * w przypadku tego samego minimaksa z obu rąk tej samej strony, konkretny rozgrywający jest nieokreślony i wybrany dowolnie (i może różnić się od tego w wynikach BigDeal)

Istniejące w wejściowym pliku PBN tagi traktowano są jako źródło tabeli liczby lew oraz minimaksa teoretycznego w następującej kolejności:

 * niestandardowe tagi JFR: `Ability` i `Minimax`
 * niestandardowy tag programu Double Dummy Solver `OptimumResult`, oraz standardowy tag PBN `OptimumScore` (dla minimaksa)
 * standardowa sekcja PBN v2.1 `OptimumResultTable` (dla tabeli liczby lew)
 * jedynie w przypadku braku tagów `Ability` i `OptimumResultTable`, uruchamiana jest analiza "w widne"

Niestandardowy tag DDS używany jest tylko jako źródło danych, nie jest on eksportowany do pliku wynikowego w przypadku jego nieobecności.

Uwagi o autorach
================

 * `libbcalcdds.dll` jest częścią [projektu BCalc](http://bcalc.w8.pl) autorstwa Piotra Belinga
 * ikona aplikacji jest zmodyfikowaną wersją ikony autorstwa [Freepik](http://www.freepik.com), dostępnej na [www.flaticon.com](http://www.flaticon.com/free-icon/playing-cards_82783) na licencji [Creative Commons BY 3.0](http://creativecommons.org/licenses/by/3.0/)

Autorem oprogramowania jest [Michał Klichowicz](https://emkael.info).

Jeśli go używasz (oprogramowania, nie autora), powinieneś wiedzieć, gdzie mnie szukać.

Jeśli nie (wiesz, gdzie mnie szukać), moja strona domowa jest niezłym początkiem.

Licencja
========

Oprogramowanie udostępniane jest na [Uproszczonej Licencji BSD](https://opensource.org/licenses/BSD-2-Clause), bo czemu nie.

Oto jej tekst:

```
Copyright (C) 2016, Michał Klichowicz
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
   2. Redistributions in binary form must reproduce the above copyright notice,
      this list of conditions and the following disclaimer in the documentation
         and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies,
either expressed or implied, of the FreeBSD Project.
```

---

`Then make them bring the curtain down.`
