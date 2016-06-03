Issues with lowest par contract from equally profitable in MiniMaxDOS
=====================================================================

*Note: all issues below concern the MiniMaxDOS component of BigDeal, which writes JFR-encoded double-dummy data to PBN files. BigDeal's par contract/score implementation used for printing/preview from within BigDeal is a separate subsystem, irrelevant to the following.*

MiniMaxDOS usually assumes that, from all equally profitable contracts, the lowest one is selected as par contract.

That is not the case, though, if the number of tricks to be taken from both hands of the same side for the determined par contract, differs. Below are some of the examples taken from a test set.

Example 1
---------

<table>
  <tr>
    <td>W/None</td>
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

North makes 4NT, South makes 5NT. Neither West nor East have a profitable sacrifice against 3NT from North/South.

`MiniMaxDOS` reports `4NT South, +460` as par contract, instead of `3NT North/South, +460`.

Example 2
---------

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

East/West make 4 Spades. 4 NT and 5 Hearts are both two down from North, but not from South.

`MiniMaxDOS` reports `5Hx North, -300` as par contract, instead of `4NTx North, -300`.

Example 3
---------

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

East makes 3NT. North can sacrifice in 4 Clubs (but not in 4 Diamonds) and South can sacrifice in 4 Diamonds (but not in 4 Clubs), both three down.

`MiniMaxDOS` reports `4Dx South, -500` as par contract, instead of `4Cx South, -500`.

Example 4
---------

<table>
  <tr>
    <td>W/None</td>
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

East/West make 4 Spades. Both North and South can sacrifice in 5 Hearts, down one, but South can sacrifice in 5 Diamonds, down one, too.

`MiniMaxDOS` reports `5Hx South, -100` as par contract, instead of `5Dx South, -100`.

Example 5
---------

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

West makes 4NT, East makes 3NT. North/South do not have a level 4 sacrifice.

`MiniMaxDOS` reports `4NT West, -630` as par contract, instead of `3NT West, -630`.

Summary
-------

Example 5 is explicable. Reporting 4NT as par contract may be useful to indicate the one trick difference between declaring from both hands.

Unfortunately, that does not explain Example 1, where par contract is reported a level lower than highest makeable contract and not two levels lower.

The behavior is inconsistent with Example 4, where the contract which differs for both hands is not indicated as par, and Examples 2-3 show that kind of indication to be problematic, when more than one different, in terms of level and denomination, contract differs in both hands.

This all leads to conclusion that rather than a handy feature, this behavior is just a minor deviation from accepted conventions.

Double Dummy Solver resolves Examples 1 and 5 correctly, and Examples 2-4 does not apply to it, because of preferring highest possible sacrifice from equally profitable.

Raw PBN example data
--------------------

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
