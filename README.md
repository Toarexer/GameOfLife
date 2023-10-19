# Game of life - Élet játéka

Az adott szimuláció a **John Conway's Game of Life** féle szabályrendszert veszi alapul.

## Szabályrendszer

## Állat

#### Alapviselkedés:
Mindenek felett a **fajfenntartás** a legfontosabb számukra, tehát az életüket adják azért, hogy utódot generáljanak.

#### Alapértelmezett állapot:
- HP az állat HP-ja
- Életév: 0
- Halhatatlanság: 3 kör
- Szaporodási időzítő: 3 kör
- Van párja?: nincs

___
#### Mozgás:

Alapértelmezett mozgás a kereten belül: **random** pozíció keresése.
- Akkor mozog az állat, ha:
    - Tud mozogni, ha nincs ellenséges állat a szomszédos cellákba vagy még halhatatlan állapotban van.
    - Éhes

___
#### Vadászat:
- Minden alkalommal leellenőrzi, hogy van-e állat a szomszédos cellákba.
- Akkor tudja megenni az állatot, amikor a halhatatlanság időzítő elérte a **0**-t.
- Amint sikerült megennie az állatot, növeli a saját HP-ját a préda értékével, de nem lehet több, mint a Maximum HP-ja.

___
#### Párok:
- Ha talált egy párt az állat a szomszédos mezőkben, akkor egy kapcsolat alakul ki közöttük.
- Az egyik tag követni fogja a párját az új utód születéséig.
- Amint létrejött egy utód, a kapcsolat kettőjük közt megszűnik és egy időzítő indul.
- A párok csak akkor tudnak létrehozni egy utódot, ha a:
    - szaporodási időzítő lejárt
    - cella üres
    - a szomszédos cellákat is megvizsgálva csak ketten tartózkodnak a területen.

___
#### Utód:
- Amint létrejön, egy halhatatlansági és egy szaporodási időzítő indul.
- Ugyanúgy viselkedik, mint a szülő.

### Róka

#### Mozgás:
- A mozgás az alapviselkedésének felel meg.
___
#### Vadászat:
- A reálisabb szimuláció érdekében a róka bármikor megeheti a nyulat, ha éhes.
    Annyit jelent, hogy, nem veszi figyelembe, azt, hogy csak akkor egyen, amikor a róka HP-ja + a nyúl által adott érték == a róka Max HP-val.
- Mindig a legidősebb nyulat fogja megenni.
- Továbbá az alapviselkedést veszi figyelembe.
___
#### Párok:
- A párválasztás életév szerint növekvő sorrendbe történik.
- Megfelel az alapviselkedésnek.
___
#### Utód:
- Az utód viselkedése a szülő viselkedésének felel meg.
- Továbbá az alapviselkedést veszi figyelembe.

### Nyúl

#### Mozgás:
- Megvizsgálja, hogy a következő pozíció nem-e esik ki a rácsrendszerből. 
- Akkor mozdul egyet, ha a pozíció a rácsrendszeren belül van.
- Amennyiben lát **2** sugarú körzetben lát füvet és a fű állapota nem mag akkor ráugrik.
- Ha van párban vannak akkor követik egymást.
- Továbbá az alapviselkedést veszi figyelembe
___
#### Evés:
- Csak is akkor fogyasztja el a füvet, ha a HP-jából legalább annyi hiányzik, mint a fű tápértéke.
___
#### Párok:
- A párválasztás életév szerint növekvő sorrendbe történik.
- Továbbá az alapviselkedést veszi figyelembe.
___
#### Utód:
- Az utód viselkedése a szülő viselkedésének felel meg.
- Továbbá az alapviselkedést veszi figyelembe.

## Növények

### Fű

#### Alapértelmezett állapot:
- Állapotok:    
    - Seed:   Tápérték - 0 (mag),              
    - Tender: Tápérték - 1 (zsenge fű),        
    - Tuft:   Tápérték - 2 (kifejlett fűcsomó)
- Alapállapot: Seed
- Utódok száma: 0
- Minden körben az állapot egyet nő, de nem nő tovább Tuft-nál.

#### Terjedés:
- Érdekesebb szimuláció érdekében egy fűnek legfeljebb 2 utódja lehet.
- 1 egység távolságban megvizsgálja az üres cellákat és azt, hogy a cellák a kereten belül vannak-e.
- Amennyiben van legalább egy üres cella és még nincs 2 utódja, akkor random kiválaszt az utódnak egy cellát.
- Amennyiben elérte a 2 utódot, vagy nincs üres cella, nem terjed.
___
#### Utód:
- Alapértelmezett állapotba kerül a szülő által megadott pozícióra.
___
#### Elfogyasztás:
- Amennyiben el lett fogyasztva, akkor az állapota csökken pl: (Tuft - Tender), (Tender - Seed)
- Az állapot nem csökkenhet tovább, ha Seed.
