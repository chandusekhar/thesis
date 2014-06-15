# Jegyzetek

## 1. Bemutatkozás

## 2. Modelltranszformáció

* mi az a modelltranszformáció (1-2 mondatban)
* modell reprezentálása gráfként
* a modelltranformáció két lépése:
	* mintaillesztés
	* gráf újraírás
* a diplomámban csak az első lépéssel foglalkoztam (mintaillesztés)

## 3. Miért elosztott?

* túl nagyra növő modellek (pl egy nagy kódbázis refaktorálás)
* hosszan tartó feldolgozás (pl szövegelemzés, arcfelismerés, képmanipuláció)
* skálázódás
* elosztottság ezekre megoldást nyújt:
	* "végtelen" horizontális skálázódás
	* párhuzamos végrehajtás

## 4. Hogyan lesz elosztott?

* elosztottság lényege -> több kisebb darabot egyszerre dolgozunk fel
* lépések rövid (1-1 mondatos magyarázata)

## 5. Partícionálás

* miért kell? -> problématér felosztása, elosztottság alapfeltételét biztosítja
* milyen a jó partícionálás? -> egymáshoz "közel" helyezi az összetartozó csomópontokat,
	tehát minimalizálja a partíciók között futó éleket
* több szintű algoritmus
	* algo rövid magyarázata a felsorolt lépéseken keresztül
* miért nem vált be?
	* főleg implementációs gondok miatt
	* csomópontok összehúzása nem elég hatékony -> sok finomítási lépéshez vezet
	* maga a koncepció nem rossz, de a megvalósítás nem sikerült elég hatékonyra
* helyette: egyszerű, de gyors algoritmus a teszteléshez (azonos méretű partíciók létrehozása)
* bírálói kérdés

	> Az 51. oldalon azt írja, hogy végül a particionálást az implementációban darab
	azonos méretű halmazra való felbontással valósította meg. Véleménye szerint
	továbbfejleszthető-e ez a megoldás, vagy ha több ideje lenne, akkor a korábban
	bemutatott módszereken dolgozna tovább?

	Az új, "buta" módszert is lehetne tovább javítani, de a kutatásaim alapján úgy tűnik,
	hogy érdemesebb a több szintű algoritmussal tovább próbálkozni. Főleg két részen lehet továbblépni:
	hatékonyabb csomópont összevonás és egy rendes partícionáló algoritmus implementálása.

## 6. Elosztott mintaillesztés

* valójában _izomorf részgráf keresés_
* 3 megvalósított stratégia
	1. **lokális**: csak az adott partícióban keres, sosem nyúlik át. lehetne elérhetetlen csomópontok,
	ezáltal lehet, hogy nem tud megtalálni minden lehetséges találatot a modellben
	2. **proxy**: a partíciókat elfedjük és a csomópontokat transparens módon érjük el proxy-kon keresztül.
	előnye, hogy a lokális algoritmust triviálisan terjeszti ki. Hátrány: sokszor hálózati kommunikáció lassúvá teheti.
	3. **részleges találatok keresése**: az egyes partíciókban a minta csak bizonyos részeit keressük. Az egyes partíciókban
	megpróbáljuk a maximális találatot megkeresni és azzal visszatérni a hívóhoz.
	Nehézségek:
		* A keresést a minta bármely csomópontjából tudni kell folytatni.
		* Hogyan "daraboljuk fel" a mintát?

## 7. Implementáció

* architektúra: kiterjeszthetőség, pluginnek
* keretrendszer: a "futtatókörnyezet" számára a modell és a feldolgozás teljesen fekete doboz
* elosztottság: REST interfész, HTTP (főleg kényelmi okok miatt)
* önálló alkalmazás, nem integrálódik bele meglévő megoldásokba

	bírálói kérdés:

	> Az implementáció során mindvégig szem előtt tartotta, hogy az elkészült
	> alkalmazás minél jobbak kiegészíthető és rugalmas legyen. Hogyan tudna
	> ez a keretrendszer együttműködni valós modelltranszformációs
	> környezetekkel? Cél volt ez a téma kidolgozása során?

	Válasz:

	* igen, cél volt
	* csak interfészeket használ a keretrendszer, az egyes al-részek könnyen kicserélhetőek (.net platformhoz kötött)
	* egyelőre egyetlen input lehetőség: xml bemeneti fájl
	* jövőben lehetne webes API

## 8. Eredmények

* partícionálás fontossága
	* a komminkáció drága -> jobb partícionálás gyorsabb matcheléshez vezet
	* kiemelten fontos
* számításigényes matchnél éri meg