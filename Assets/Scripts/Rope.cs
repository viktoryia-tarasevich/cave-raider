using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    //Визуализатор отображающий веревку
    LineRenderer lineRenderer;

    //Список объектов RopeSegment
    List<GameObject> ropeSegments = new List<GameObject>();

    //Шаблон RopeSegment для создания новых звеньев
    public GameObject ropeSegmentPrefab;

    public float maxRopeSegmentLength = 1.0f;

    public float defaultRopeSegmentLenght = 0.0f;

    //Скорость создания новых звеньев
    public float ropeSpeed = 4.0f;

    //Объект к которому прицепляется веревка
    public Rigidbody2D connectedObject;

    public bool IsDecreasing { get; set; }
    public bool IsIncreasing { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        //Кэшировать ссылку на визуализатор, чтобы 
        //не пришлось искать его в каждом кадре.
        lineRenderer = GetComponent<LineRenderer>();

        ResetLength();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject topSegment = ropeSegments[0];

        SpringJoint2D topSegmentJoint = topSegment.GetComponent<SpringJoint2D>();

        if (IsIncreasing)
        {
            if (topSegmentJoint.distance >= maxRopeSegmentLength)
            {
                CreateRopeSegment();
            }
            else
            {
                topSegmentJoint.distance += ropeSpeed * Time.deltaTime;
            }
        }
        if (IsDecreasing)
        {
            if (topSegmentJoint.distance <= 0.005f)
            {
                RemoveRopeSegment();
            }
            else
            {
                topSegmentJoint.distance -= ropeSpeed * Time.deltaTime;
            }
        }
        if (lineRenderer != null)
        {

            //Число вершин для визуализатора который нарисуют линию по точкам равно количество звеньев + 1 и плюс точка на нашем герое
            lineRenderer.positionCount = ropeSegments.Count + 2;

            lineRenderer.SetPosition(0, this.transform.position);

            for (int i = 0; i < ropeSegments.Count; i++)
            {
                lineRenderer.SetPosition(i + 1, ropeSegments[i].transform.position);
            }

            SpringJoint2D connectedObjectJoint = connectedObject.GetComponent<SpringJoint2D>();

            var lastPosition = connectedObject.transform.TransformPoint(connectedObjectJoint.anchor);

            lineRenderer.SetPosition(ropeSegments.Count + 1, lastPosition);
        }
    }

    //Удаляет все звенья и создает новое звено
    public void ResetLength()
    {
        foreach (GameObject segment in ropeSegments)
        {
            Destroy(segment);
        }

        ropeSegments = new List<GameObject>();

        IsDecreasing = false;
        IsIncreasing = false;

        CreateRopeSegment();
    }

    //Добавляет новое звено веревки к началу веревки
    void CreateRopeSegment()
    {
        //Создаем новое звено
        GameObject segment = (GameObject)Instantiate(ropeSegmentPrefab, this.transform.position, Quaternion.identity);

        //Сделать segment потомком this и сохранить его координаты
        segment.transform.SetParent(this.transform, true);

        //Получить твердое тело звена
        Rigidbody2D segmentBody = segment.GetComponent<Rigidbody2D>();

        if (segmentBody == null)
        {
            Debug.LogError("Rope segment body prefab has no Rigidbody2D component");
            return;
        }

        //Получить длину соеденения из звена
        SpringJoint2D segmentJoint = segment.GetComponent<SpringJoint2D>();

        if (segmentJoint == null)
        {
            Debug.LogError("Rope segment body prefab has no SpringJoint2D component");
            return;
        }

        ropeSegments.Insert(0, segment);

        if (ropeSegments.Count == 1)
        {
            //Соеденить звено с соединением несущего объекта
            SpringJoint2D connectedObjectJoint = connectedObject.GetComponent<SpringJoint2D>();

            connectedObjectJoint.connectedBody = segmentBody;

            connectedObjectJoint.distance = 0.1f;

            segmentJoint.distance = maxRopeSegmentLength;
        }
        else
        {
            GameObject nextSegment = ropeSegments[1];

            SpringJoint2D nextSegmentJoint = nextSegment.GetComponent<SpringJoint2D>();

            //соединяем 2ое звено и новое звено
            nextSegmentJoint.connectedBody = segmentBody;

            //Устанавливаем длину на 0, она увеличивается автоматически
            segmentJoint.distance = defaultRopeSegmentLenght;
        }

        //соединяем новое звено с опорой для веревки
        segmentJoint.connectedBody = this.GetComponent<Rigidbody2D>();
    }

    //Удаляем верхнее звено веревки
    void RemoveRopeSegment()
    {
        if (ropeSegments.Count < 2)
        {
            return;
        }

        GameObject topSegment = ropeSegments[0];
        GameObject nextSegment = ropeSegments[1];

        //Соединяем 2ое звено с опорой
        SpringJoint2D nextSegmentJoint = nextSegment.GetComponent<SpringJoint2D>();

        nextSegmentJoint.connectedBody = this.GetComponent<Rigidbody2D>();

        ropeSegments.RemoveAt(0);
        Destroy(topSegment);
    }
}
