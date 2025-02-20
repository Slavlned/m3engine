# M3

Движок для матч-3 игр под юнити с открытым
исходным кодом. ⭐

Этот движок используется My Cozy Nook (ещё не релизнулась).

### feautures
- Продвинутая система для красивого осыпания фишек;
- Анимации появления и исчезнавения фишек;
- Удобная система для добавление визуальных и звуковых эффектов;
- Легкая расширяемость, легко добавлять свои элементы в игру;

### plans
- Планируется добавление фишек с размером на больше чем 1 клетка;
- Планируется добавление заднего слоя на тайлах, например для аналога 
мёда из GardenScapes. ✅

### engine
Unity Engine 6.0.0

### server-side
Здесь нет серверной части для аналитики и т.д.
Так что писать серверную часть и ServerService придется
в ручную.

### main entities
- M3Board - current level board with tiles (MonoBeh)
- M3Fx - effects for m3 (MonoBeh)
- M3Tile - point on a board (MonoBeh)
- M3Object - element on a tile (MonoBeh)
- M3Match - match of elements
- M3Pos - position on the field
- M3Predications - predications of a possible matchs for shuffling
- M3Level - info of a board, can be saved as json
