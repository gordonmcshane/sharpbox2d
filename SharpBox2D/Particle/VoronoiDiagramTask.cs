namespace SharpBox2D.Particle
{
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
}