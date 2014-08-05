using SharpBox2D.Pooling.Normal;

namespace SharpBox2D.Particle
{
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
}