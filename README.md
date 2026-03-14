# Spider Defense
## Autorzy
Mikołaj Gosztyła, Magdalena Pabisz
## Opis projektu
Projekt polega na stworzeniu gry typu **Tower Defense** z wykorzystaniem silnika **Unity**. W grze zadaniem gracza jest obrona swojej bazy przed nadchodzącymi falami przeciwników poprzez strategiczne rozmieszczanie wieżyczek obronnych na mapie.
### Cel gry
Celem gry jest obrona bazy przed rosnącymi falami wrogów poprzez wykorzystanie dostępnych typów wieżyczek oraz podejmowanie decyzji dotyczących rozbudowy planszy. Gra kończy się zwycięstwem po przetrwaniu określonej liczby rund.

### Mechanika gry
Rozgrywka składa się z rund, w których pojawiają się kolejne fale przeciwników o rosnącym poziomie trudności. Gracz może budować nowe wieżyczki, ulepszać istniejące oraz podejmować decyzje wpływające na rozwój planszy.

### Przeciwnicy
Głównymi przeciwnikami w grze są **pająki**, których modele zostaną stworzone z wykorzystaniem zasad **dynamiki odwrotnej (inverse kinematics)**. W grze występuje kilka rodzajów pająków, różniących się m.in. szybkością i wytrzymałością, co wymusza na graczu stosowanie różnych strategii obronnych.

### Mapa rozgrywki
Mapa rozgrywki składa się z trzech rodzajów pól:
- pola tworzące ścieżkę, po której poruszają się przeciwnicy,
- pola przeznaczone pod budowę wieżyczek,
- pola niedostępne, pełniące funkcję wizualną.

\
Po zakończeniu każdej rundy gracz może zdecydować o rozbudowie planszy, wybierając jedną z dwóch opcji:
- wydłużenie ścieżki przeciwników o jeden segment, co sprawia, że wrogowie pokonują dłuższą drogę do celu,
- dodanie dwóch nowych pól przeznaczonych pod budowę wieżyczek, co zwiększa możliwości obronne gracza w kolejnych rundach.

### Wieżyczki i system nagród

Gracz dysponuje różnymi typami wieżyczek, które różnią się zasięgiem, siłą ataku i specjalnymi efektami. Po każdej rundzie gracz otrzymuje określoną liczbę zasobów (monet), które może wykorzystać na:
- budowę nowych wieżyczek,
- ulepszanie istniejących.

System ten zmusza gracza do podejmowania decyzji strategicznych – czy bardziej opłaca się wzmocnić już postawione wieżyczki, czy zwiększyć liczbę punktów obrony na mapie.

### Elementy gry
Projekt obejmuje m.in.:
- **Interfejs użytkownika (UI)** – pokazujący informacje o liczbie zasobów, aktualnej rundzie, stanie zdrowia bazy oraz dostępnych opcjach budowy wieżyczek.
- **System fal przeciwników** – fale stopniowo zwiększają poziom trudności.
- **Wieżyczki obronne** – różniące się zasięgiem, szybkością ataku i siłą rażenia.
- **Mapy** – poziomy o różnych początkowych układach terenu i ścieżek przeciwników.


## Propozycja modeli do wykorzystania
Modele wieżyczek i pól planszy: [Tower Defense Kit](https://kenney-assets.itch.io/tower-defense-kit)
