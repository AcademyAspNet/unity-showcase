using System.Collections.Generic;
using UnityEngine;

// Класс "Путевая точка" (Waypoint). Это основа для построения пути врагов.
// Каждый такой скрипт добавляется на пустой объект на сцене, чтобы отметить точку.
public class Waypoint : MonoBehaviour
{
    // Цвет линий, которые мы будем рисовать в редакторе Unity.
    // Нужен только для визуальной настройки пути, в самой игре этих линий не будет.
    private static readonly Color DEBUG_LINE_COLOR = Color.yellow;

    // Это список СЛЕДУЮЩИХ путевых точек, к которым может пойти враг из этой точки.
    // [SerializeField] - это атрибут, который позволяет видеть и настраивать этот список прямо в редакторе Unity в окне Inspector.
    // Благодаря этой строке Вы сможете перетаскивать другие объекты с компонентом Waypoint в этот список.
    // Пример: у точки "А" в списке указана точка "B" и точка "C". Значит, враг из "А" может пойти на "B" ИЛИ на "C" (если нужна развилка).
    [field: SerializeField]
    public List<Waypoint> NextWaypoints { get; private set; } = new();

    // Этот метод автоматически вызывается редактором Unity, когда он рисует сцену.
    // Его единственная задача - ВИЗУАЛЬНО показать путь, который получился.
    private void OnDrawGizmos()
    {
        // Если список следующих точек пуст, нечего и рисовать. Выходим.
        if (NextWaypoints == null)
            return;

        // Говорим системе рисования: "Используй желтый цвет".
        Gizmos.color = DEBUG_LINE_COLOR;

        // Для каждой точки в нашем списке "NextWaypoints"...
        foreach (Waypoint waypoint in NextWaypoints)
        {
            // ...проверяем, не пустая ли там ссылка (на всякий случай).
            if (waypoint == null)
                continue; // Если пустая, пропускаем её и идем к следующей.

            // РИСУЕМ ЛИНИЮ от позиции ЭТОГО объекта (transform.position)
            // до позиции объекта СЛЕДУЮЩЕЙ точки (waypoint.transform.position).
            // Эта линия будет видна ТОЛЬКО в окне сцены (Scene), а не в игровом окне (Game).
            Gizmos.DrawLine(transform.position, waypoint.transform.position);
        }
    }
}
