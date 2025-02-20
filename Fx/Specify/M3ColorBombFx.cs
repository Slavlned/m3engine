using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M3ColorBombFx : MonoBehaviour
{
    // префабы для FX
    [SerializeField]
    private GameObject fx_Prefab;
    [SerializeField]
    private GameObject sfx_Prefab;
    
    // настройки
    [SerializeField]
    private float fxTimeBetween = 0.3f;
    private WaitForSeconds fxTimeBetweenWaitForSeconds;

    // цвета
    [SerializeField]
    private List<M3ColorBombFxPair> colors;

    // рандом
    [SerializeField]
    private M3Random random;

    // создаём эффект, разрушаем на позициях объекты
    public void CreateAndDamage(M3Board board, M3Pos pos, List<M3Pos> positions)
    {
        // кординаты инстантиэйтнутого префаба
        Transform _transform = Instantiate(
            fx_Prefab,
            board.ToPoint(pos.X, pos.Y),
            Quaternion.identity
        ).transform;

        // настраиваем цвет
        Color color = colors.Find(c => c.type == board.TileAt(positions[0]).GetObject().GetM3Type()).color;
        ParticleSystem.MainModule module = _transform.GetComponent<ParticleSystem>().main;
        module.startColor = color;

        // запрещаем обновление доски
        board.SetLock(true);
        board.SetGravity(false);

        // твин
        Sequence seq = DOTween.Sequence().Pause();

        // перебераем аппендим в твин
        foreach (M3Pos _pos in positions)
        {
            seq.Append(
                _transform.DOMove(board.TileAt(_pos).transform.position, fxTimeBetween)
            ).AppendCallback(() => { 
                CreateSfx(board.ToPoint(_pos.X, _pos.Y));
            });
        }

        // запускаем твин
        seq.Play();

        // короутина выжидания
        StartCoroutine(CreateAndDamageCoroutine(_transform, board, pos, positions.Count * fxTimeBetween, positions));
    }

    // создание sfx 
    private void CreateSfx(Vector3 pos)
    {
        // создаём
        Instantiate(sfx_Prefab, pos, Quaternion.identity);
    }

    // создание и заполнение
    public void CreateAndFill(M3Board board, M3Pos pos, M3Pos sPos, M3ObjectType fillTo, List<M3Pos> positions)
    {
        // кординаты инстантиэйтнутого префаба
        Transform _transform = Instantiate(
            fx_Prefab,
            board.ToPoint(pos.X, pos.Y),
            Quaternion.identity
        ).transform;

        // настраиваем цвет
        Color color = colors.Find(c => c.type == board.TileAt(positions[0]).GetObject().GetM3Type()).color;
        ParticleSystem.MainModule module = _transform.GetComponent<ParticleSystem>().main;
        module.startColor = color;

        // запрещаем обновление доски
        board.SetLock(true);
        board.SetGravity(false);

        // первый твин
        DOTween.Sequence()
            .Append(
                _transform.DOMove(board.TileAt(positions[0]).transform.position, fxTimeBetween)
            );

        // короутина продолжения
        StartCoroutine(CreateAndFillCoroutine(_transform, board, pos, sPos, fillTo, positions));
    }

    // короутина выжидания и уничтожения выделенных объектов
    private IEnumerator CreateAndDamageCoroutine(Transform particle, M3Board board, M3Pos boosterPos, float time, List<M3Pos> positions)
    {
        // ждём время
        yield return new WaitForSeconds(time);
        // уничтожаем позиции
        foreach (M3Pos pos in positions)
        {
            // тайл
            M3Tile _tile = board.TileAt(pos);
            // если тайл не пуст
            if (!_tile.IsEmpty())
            {
                // дамажим
                _tile.GetObject().Damage(M3DestroyType.CALL);
            }
        }
        // уничтожаем бустер
        if (!board.TileAt(boosterPos).IsEmpty())
        {
            board.DestroyObject(board.TileAt(boosterPos).GetObject(), M3DestroyType.CALL);
        }
        // уничтожаем партикл
        Destroy(particle.gameObject);
        // разрешаем обновление доски
        board.SetLock(false);
        board.SetGravity(true);
    }


    // короутина выжидания и заполнениея объектов
    private IEnumerator CreateAndFillCoroutine(Transform particle, M3Board board, M3Pos boosterPos, M3Pos sBoosterPos, M3ObjectType fillType, List<M3Pos> positions)
    {
        // проверяем на сдовенные колор бомбы
        if (fillType == M3ObjectType.COLOR_BOMB) {
            // индекс
            foreach (M3Tile m3Tile in board.GetTiles())
            {
                // твин
                DOTween.Sequence()
                    .Append(
                        particle.DOMove(m3Tile.transform.position, fxTimeBetween)
                    );
                // ждём время
                yield return fxTimeBetweenWaitForSeconds;
                // дамажим
                // если тайл не пуст
                if (!m3Tile.IsEmpty())
                {
                    m3Tile.GetObject().Damage(M3DestroyType.NOCALL);
                }
            }
            // уничтожаем партикл
            Destroy(particle.gameObject);
            // разрешаем обновление доски
            board.SetLock(false);
            board.SetGravity(true);
            // брикаем
            yield break;
        }

        // создаем индекс
        int index = 0;

        // уничтожаем бустер
        board.DestroyObject(board.TileAt(boosterPos).GetObject(), M3DestroyType.CALL);
        board.DestroyObject(board.TileAt(sBoosterPos).GetObject(), M3DestroyType.CALL);

        // индекс
        while (index < positions.Count)
        {
            // позиция
            M3Pos pos = positions[index];
            // тайл
            M3Tile _tile = board.TileAt(pos);
            // твин
            DOTween.Sequence()
                .Append(
                    particle.DOMove(board.TileAt(pos).transform.position, fxTimeBetween)
                );
            // ждём время
            yield return new WaitForSeconds(fxTimeBetween);
            // дамажим
            if (_tile.IsEmpty()) { continue; }
            _tile.GetObject().Damage(M3DestroyType.CALL);
            // ждем один кадр
            yield return null;
            // реплейсим
            if (_tile.GetObject() == null)
            {
                _tile.ReplaceTo(fillType);
            }
            // добавляем к индексу еденичку
            index++;
        }
        // ждём время
        yield return new WaitForSeconds(fxTimeBetween);
        // уничтожаем партикл
        Destroy(particle.gameObject);
        // активируем
        foreach (M3Pos pos in positions)
        {
            // если пуст брикаем цикл на некст итерацию
            if (board.TileAt(pos).IsEmpty()) { continue;  }

            // активируем
            if (board.TileAt(pos).GetObject().GetComponent<M3Bomb>()) {
                board.TileAt(pos).GetObject().GetComponent<M3Bomb>().Activate();
            }
            else if (board.TileAt(pos).GetObject().GetComponent<M3Arrow>())
            {
                board.TileAt(pos).GetObject().GetComponent<M3Arrow>().Activate();
            }
        }
        // разрешаем обновление доски
        board.SetLock(false);
    }

    private void Awake()
    {
        fxTimeBetweenWaitForSeconds = new WaitForSeconds(fxTimeBetween);
    }
}