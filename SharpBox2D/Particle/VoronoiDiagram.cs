 using System.Collections.Generic;
 using System.Diagnostics;
 using SharpBox2D.Common;
 using SharpBox2D.Pooling.Normal;

public interface VoronoiDiagramCallback {
    void callback(int aTag, int bTag, int cTag);
  }

 internal class VoronoiDiagramTaskPool : MutableStack<VoronoiDiagramTask>
 {
     public VoronoiDiagramTaskPool(int argInitSize) : base(argInitSize) { }

     protected override VoronoiDiagramTask newInstance() {
          return new VoronoiDiagramTask();
        }
        
        protected override VoronoiDiagramTask[] newArray(int size) {
          return new VoronoiDiagramTask[size];
        }    
 }

 public class VoronoiDiagramTask {
     internal int m_x;
     internal int m_y;
     internal int m_i;
     internal Generator m_generator;

    public VoronoiDiagramTask() {}

    public VoronoiDiagramTask(int x, int y, int i, Generator g) {
      m_x = x;
      m_y = y;
      m_i = i;
      m_generator = g;
    }

    public VoronoiDiagramTask set(int x, int y, int i, Generator g) {
      m_x = x;
      m_y = y;
      m_i = i;
      m_generator = g;
      return this;
    }
  }

  public class Generator {
    public Vec2 center = new Vec2();
    public int tag;
  }

public class VoronoiDiagram {

  private Generator[] m_generatorBuffer;
  private int m_generatorCount;
  private int m_countX, m_countY;
  // The diagram is an array of "pointers".
  private Generator[] m_diagram;

  public VoronoiDiagram(int generatorCapacity) {
    m_generatorBuffer = new Generator[generatorCapacity];
    for (int i = 0; i < generatorCapacity; i++) {
      m_generatorBuffer[i] = new Generator();
    }
    m_generatorCount = 0;
    m_countX = 0;
    m_countY = 0;
    m_diagram = null;
  }

  public void getNodes(VoronoiDiagramCallback callback) {
    for (int y = 0; y < m_countY - 1; y++) {
      for (int x = 0; x < m_countX - 1; x++) {
        int i = x + y * m_countX;
        Generator a = m_diagram[i];
        Generator b = m_diagram[i + 1];
        Generator c = m_diagram[i + m_countX];
        Generator d = m_diagram[i + 1 + m_countX];
        if (b != c) {
          if (a != b && a != c) {
            callback.callback(a.tag, b.tag, c.tag);
          }
          if (d != b && d != c) {
            callback.callback(b.tag, d.tag, c.tag);
          }
        }
      }
    }
  }

  public void addGenerator(Vec2 center, int tag) {
    Generator g = m_generatorBuffer[m_generatorCount++];
    g.center.x = center.x;
    g.center.y = center.y;
    g.tag = tag;
  }

  private Vec2 lower = new Vec2();
  private Vec2 upper = new Vec2();
  private VoronoiDiagramTaskPool taskPool = new VoronoiDiagramTaskPool(50);
     
  private Stack<VoronoiDiagramTask> queue = new Stack<VoronoiDiagramTask>();

  public void generate(float radius) {
    Debug.Assert (m_diagram == null);
    float inverseRadius = 1 / radius;
    lower.x = float.MaxValue;
    lower.y = float.MaxValue;
    upper.x = float.MinValue;
    upper.y = float.MinValue;
    for (int k = 0; k < m_generatorCount; k++) {
      Generator g = m_generatorBuffer[k];
      Vec2.minToOut(lower, g.center, ref lower);
      Vec2.maxToOut(upper, g.center, ref upper);
    }
    m_countX = 1 + (int) (inverseRadius * (upper.x - lower.x));
    m_countY = 1 + (int) (inverseRadius * (upper.y - lower.y));
    m_diagram = new Generator[m_countX * m_countY];
    queue = new Stack<VoronoiDiagramTask>(new VoronoiDiagramTask[4 * m_countX * m_countX]);
    for (int k = 0; k < m_generatorCount; k++) {
      Generator g = m_generatorBuffer[k];
      g.center.x = inverseRadius * (g.center.x - lower.x);
      g.center.y = inverseRadius * (g.center.y - lower.y);
      int x = MathUtils.max(0, MathUtils.min((int) g.center.x, m_countX - 1));
      int y = MathUtils.max(0, MathUtils.min((int) g.center.y, m_countY - 1));
      queue.Push(taskPool.pop().set(x, y, x + y * m_countX, g));
    }
    while (queue.Count != 0)
    {
      VoronoiDiagramTask front = queue.Pop();
      int x = front.m_x;
      int y = front.m_y;
      int i = front.m_i;
      Generator g = front.m_generator;
      if (m_diagram[i] == null) {
        m_diagram[i] = g;
        if (x > 0) {
          queue.Push(taskPool.pop().set(x - 1, y, i - 1, g));
        }
        if (y > 0) {
          queue.Push(taskPool.pop().set(x, y - 1, i - m_countX, g));
        }
        if (x < m_countX - 1) {
          queue.Push(taskPool.pop().set(x + 1, y, i + 1, g));
        }
        if (y < m_countY - 1) {
          queue.Push(taskPool.pop().set(x, y + 1, i + m_countX, g));
        }
      }
      taskPool.push(front);
    }
    int maxIteration = m_countX + m_countY;
    for (int iteration = 0; iteration < maxIteration; iteration++) {
      for (int y = 0; y < m_countY; y++) {
        for (int x = 0; x < m_countX - 1; x++) {
          int i = x + y * m_countX;
          Generator a = m_diagram[i];
          Generator b = m_diagram[i + 1];
          if (a != b) {
            queue.Push(taskPool.pop().set(x, y, i, b));
            queue.Push(taskPool.pop().set(x + 1, y, i + 1, a));
          }
        }
      }
      for (int y = 0; y < m_countY - 1; y++) {
        for (int x = 0; x < m_countX; x++) {
          int i = x + y * m_countX;
          Generator a = m_diagram[i];
          Generator b = m_diagram[i + m_countX];
          if (a != b) {
            queue.Push(taskPool.pop().set(x, y, i, b));
            queue.Push(taskPool.pop().set(x, y + 1, i + m_countX, a));
          }
        }
      }
      bool updated = false;
      while (queue.Count != 0) {
        VoronoiDiagramTask front = queue.Pop();
        int x = front.m_x;
        int y = front.m_y;
        int i = front.m_i;
        Generator k = front.m_generator;
        Generator a = m_diagram[i];
        Generator b = k;
        if (a != b) {
          float ax = a.center.x - x;
          float ay = a.center.y - y;
          float bx = b.center.x - x;
          float by = b.center.y - y;
          float a2 = ax * ax + ay * ay;
          float b2 = bx * bx + by * by;
          if (a2 > b2) {
            m_diagram[i] = b;
            if (x > 0) {
              queue.Push(taskPool.pop().set(x - 1, y, i - 1, b));
            }
            if (y > 0) {
              queue.Push(taskPool.pop().set(x, y - 1, i - m_countX, b));
            }
            if (x < m_countX - 1) {
              queue.Push(taskPool.pop().set(x + 1, y, i + 1, b));
            }
            if (y < m_countY - 1) {
              queue.Push(taskPool.pop().set(x, y + 1, i + m_countX, b));
            }
            updated = true;
          }
        }
        taskPool.push(front);
      }
      if (!updated) {
        break;
      }
    }
  }
}
