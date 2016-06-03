Problemy z najniższym spośród równych kontraktów minimaksowych w  MiniMaxDOS
============================================================================

*Uwaga: poniższe problemy dotyczą komponentu BigDeal o nazwie MiniMaxDOS, piszącego bezpośrednio do PBN dane analizy w widne w formatach JFR. Implementacja minimaksa w samym BigDeal, używana do wyświetlania/drukowania danych analizy z tabelami rozkładów, jest niezależna od poniższych.*

MiniMaxDOS zazwyczaj przyjmuje, że ze wszystkich równie opłacalnych kontraktów, minimaksem jest najniższy.

Zasada ta przestaje jednak działać, jeśli liczba lew do wzięcia różni się między rozgrywką z obu rąk tej samej strony. Poniżej znajdują się przykłady z zestawu testowego.

Przykład 1
----------

<table>
  <tr>
    <td>W/nikt</td>
    <td>
      ♠ K7<br />
      ♥ AK87<br />
      ♦ A87<br />
      ♣ AJ52
    </td>
    <td></td>
    <td></td>
  </tr>
  <tr>
    <td>
      ♠ 9843<br />
      ♥ 53<br />
      ♦ QT4<br />
      ♣ Q983
    </td>
    <td></td>
    <td>
      ♠ AJ5<br />
      ♥ QT9<br />
      ♦ 9532<br />
      ♣ T76
    </td>
    <td></td>
  </tr>
  <tr>
    <td></td>
    <td>
      ♠ QT62<br />
      ♥ J642<br />
      ♦ KJ6<br />
      ♣ K4
    </td>
    <td></td>
    <td>
      <table>
        <tr>
          <td></td><td>NT</td><td>♠</td><td>♥</td><td>♦</td><td>♣</td>
        </tr>
        <tr>
          <td>N</td><td>10</td><td>10</td><td>11</td><td>10</td><td>10</td>
        </tr>
        <tr>
          <td>S</td><td>11</td><td>10</td><td>11</td><td>10</td><td>10</td>
        </tr>
        <tr>
          <td>W</td><td>2</td><td>3</td><td>2</td><td>3</td><td>3</td>
        </tr>
        <tr>
          <td>E</td><td>2</td><td>3</td><td>2</td><td>3</td><td>3</td>
        </tr>
      </table>
    </td>
  </tr>
</table>

N realizuje 4NT, S realizuje 5NT. E i W nie mają opłacalnej obrony dla 3NT z ręki N/S.

`MiniMaxDOS` ustala `4NT S, +460` jako minimaks, zamiast `3NT N/S, +460`.

Przykład 2
----------

<table>
  <tr>
    <td>N/EW</td>
    <td>
      ♠ Q52<br />
      ♥ KQ974<br />
      ♦ AQ<br />
      ♣ AKT
    </td>
    <td></td>
    <td></td>
  </tr>
  <tr>
    <td>
      ♠ T73<br />
      ♥ 32<br />
      ♦ JT5<br />
      ♣ Q8764
    </td>
    <td></td>
    <td>
      ♠ AKJ94<br />
      ♥ AT6<br />
      ♦ K8642<br />
      ♣ ==
    </td>
    <td></td>
  </tr>
  <tr>
    <td></td>
    <td>
      ♠ 86<br />
      ♥ J85<br />
      ♦ 973<br />
      ♣ J9532
    </td>
    <td></td>
    <td>
      <table>
        <tr>
          <td></td><td>NT</td><td>♠</td><td>♥</td><td>♦</td><td>♣</td>
        </tr>
        <tr>
          <td>N</td><td>8</td><td>3</td><td>9</td><td>4</td><td>7</td>
        </tr>
        <tr>
          <td>S</td><td>3</td><td>3</td><td>8</td><td>3</td><td>7</td>
        </tr>
        <tr>
          <td>W</td><td>4</td><td>10</td><td>4</td><td>9</td><td>6</td>
        </tr>
        <tr>
          <td>E</td><td>4</td><td>10</td><td>4</td><td>9</td><td>6</td>
        </tr>
      </table>
    </td>
  </tr>
</table>

E/W ugrywają 4 pik. 4 NT i 5 kier są bez 2 z ręki N, lecz nie z ręki S.

`MiniMaxDOS` ustala `5Hx N, -300` jako minimaks, zamiast `4NTx N, -300`.

Przykład 3
----------

<table>
  <tr>
    <td>W/EW</td>
    <td>
      ♠ Q<br />
      ♥ KT97<br />
      ♦ J953<br />
      ♣ 9754
    </td>
    <td></td>
    <td></td>
  </tr>
  <tr>
    <td>
      ♠ 94<br />
      ♥ AQ642<br />
      ♦ Q72<br />
      ♣ T83
    </td>
    <td></td>
    <td>
      ♠ AKJT3<br />
      ♥ J3<br />
      ♦ T64<br />
      ♣ AQ2
    </td>
    <td></td>
  </tr>
  <tr>
    <td></td>
    <td>
      ♠ 87652<br />
      ♥ 85<br />
      ♦ AK8<br />
      ♣ KJ6
    </td>
    <td></td>
    <td>
      <table>
        <tr>
          <td></td><td>NT</td><td>♠</td><td>♥</td><td>♦</td><td>♣</td>
        </tr>
        <tr>
          <td>N</td><td>4</td><td>4</td><td>5</td><td>6</td><td>7</td>
        </tr>
        <tr>
          <td>S</td><td>4</td><td>4</td><td>5</td><td>7</td><td>6</td>
        </tr>
        <tr>
          <td>W</td><td>8</td><td>8</td><td>7</td><td>5</td><td>6</td>
        </tr>
        <tr>
          <td>E</td><td>9</td><td>8</td><td>7</td><td>6</td><td>6</td>
        </tr>
      </table>
    </td>
  </tr>
</table>

E ugrywa 3NT. N ma opłacalną obronę w 4 trefl (ale nie 4 karo), a S ma opłacalną obronę w 4 karo (ale nie 4 trefl), obie bez trzech.

`MiniMaxDOS` ustala `4Dx S, -500` jako minimaks, zamiast `4Cx S, -500`.

Przykład 4
----------

<table>
  <tr>
    <td>W/nikt</td>
    <td>
      ♠ J5<br />
      ♥ T872<br />
      ♦ KT76<br />
      ♣ K32
    </td>
    <td></td>
    <td></td>
  </tr>
  <tr>
    <td>
      ♠ KT93<br />
      ♥ K54<br />
      ♦ A<br />
      ♣ AT986
    </td>
    <td></td>
    <td>
      ♠ Q87642<br />
      ♥ 9<br />
      ♦ 84<br />
      ♣ Q754
    </td>
    <td></td>
  </tr>
  <tr>
    <td></td>
    <td>
      ♠ A<br />
      ♥ AQJ63<br />
      ♦ QJ9532<br />
      ♣ J
    </td>
    <td></td>
    <td>
      <table>
        <tr>
          <td></td><td>NT</td><td>♠</td><td>♥</td><td>♦</td><td>♣</td>
        </tr>
        <tr>
          <td>N</td><td>6</td><td>3</td><td>10</td><td>9</td><td>3</td>
        </tr>
        <tr>
          <td>S</td><td>6</td><td>3</td><td>10</td><td>10</td><td>3</td>
        </tr>
        <tr>
          <td>W</td><td>6</td><td>10</td><td>3</td><td>3</td><td>10</td>
        </tr>
        <tr>
          <td>E</td><td>6</td><td>10</td><td>3</td><td>3</td><td>10</td>
        </tr>
      </table>
    </td>
  </tr>
</table>

E/W ugrywają 4 pik. Zarówno N, jak i S mogą bronić 5 kierami, bez jednej, lecz S może bronić również 5 karami, bez jednej.

`MiniMaxDOS` ustala `5Hx S, -100` jako minimaks, zamiast `5Dx S, -100`.

Przykład 5
----------

<table>
  <tr>
    <td>E/EW</td>
    <td>
      ♠ 9865<br />
      ♥ Q98<br />
      ♦ AJT4<br />
      ♣ 84
    </td>
    <td></td>
    <td></td>
  </tr>
  <tr>
    <td>
      ♠ KQ732<br />
      ♥ AJ2<br />
      ♦ K53<br />
      ♣ AK
    </td>
    <td></td>
    <td>
      ♠ J4<br />
      ♥ KT76<br />
      ♦ Q98<br />
      ♣ J762
    </td>
    <td></td>
  </tr>
  <tr>
    <td></td>
    <td>
      ♠ AT<br />
      ♥ 543<br />
      ♦ 762<br />
      ♣ QT953
    </td>
    <td></td>
    <td>
      <table>
        <tr>
          <td></td><td>NT</td><td>♠</td><td>♥</td><td>♦</td><td>♣</td>
        </tr>
        <tr>
          <td>N</td><td>3</td><td>3</td><td>3</td><td>3</td><td>5</td>
        </tr>
        <tr>
          <td>S</td><td>3</td><td>3</td><td>3</td><td>3</td><td>5</td>
        </tr>
        <tr>
          <td>W</td><td>10</td><td>10</td><td>10</td><td>9</td><td>8</td>
        </tr>
        <tr>
          <td>E</td><td>9</td><td>9</td><td>10</td><td>8</td><td>8</td>
        </tr>
      </table>
    </td>
  </tr>
</table>

W ugrywa 4NT, E ugrywa 3NT. N/S nie mają opłacalnej obrony na poziomie 4.

`MiniMaxDOS` ustala `4NT W, -630` jako minimaks, zamiast `3NT W, -630`.

Podsumowanie
------------

Przykład 5 jest wytłumaczalny. Wyszczególnianie 4NT jako minimaksa może być użyteczne do podkreślenia różnicy liczby lew branych z obu rąk.

Niestety, nie tłumaczy to Przykładu 1, gdzie raportowany jest konktrakt o jeden poziom niższy od najwyższego wychodzącego, a kontrakt o dwa poziomy niższy - już nie.

Zachowanie to jest niespójne z Przykładem 4, gdzie kontrakt, dla którego zachodzą różnice liczby lew nie jest wyszczególniony jako minimaks, a Przykłady 2-3 pokazują, jak problematyczne może być takie podejście, gdy więcej niż jeden kontrakt (z punktu widzenia poziomu i miana) różni się liczbą lew z obu rąk.

Wskazuje to w związku z tym nie na zgrabną funkcjonalność, lecz na niewielkie odstępstwa od przyjętych konwencji.

Double Dummy Solver rozwiązuje Przykłady 1 i 5 poprawnie (spójnie), a Przykłady 2-4 go nie dotyczą, z racji przyjmowania najwyższej opłacalnej obrony jako minimaks.

Dane PBN dla przykładów
-----------------------

```
[Board "Example 1"]
[Dealer "W"]
[Vulnerable "None"]
[Deal "N:K7.AK87.A87.AJ52 AJ5.QT9.9532.T76 QT62.J642.KJ6.K4 9843.53.QT4.Q983"]

[Board "Example 2"]
[Dealer "N"]
[Vulnerable "EW"]
[Deal "N:Q52.KQ974.AQ.AKT AKJ94.AT6.K8642. 86.J85.973.J9532 T73.32.JT5.Q8764"]

[Board "Example 3"]
[Dealer "W"]
[Vulnerable "EW"]
[Deal "N:Q.KT97.J953.9754 AKJT3.J3.T64.AQ2 87652.85.AK8.KJ6 94.AQ642.Q72.T83"]

[Board "Example 4"]
[Dealer "W"]
[Vulnerable "None"]
[Deal "N:J5.T872.KT76.K32 Q87642.9.84.Q754 A.AQJ63.QJ9532.J KT93.K54.A.AT986"]

[Board "Example 5"]
[Dealer "E"]
[Vulnerable "EW"]
[Deal "N:9865.Q98.AJT4.84 J4.KT76.Q98.J762 AT.543.762.QT953 KQ732.AJ2.K53.AK"]
```
