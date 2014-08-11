using SharpBox2D.Pooling.Normal;

namespace SharpBox2D.Particle
{
    internal class VoronoiDiagramTaskPool : MutableStack<VoronoiDiagramTask>
    {
        public VoronoiDiagramTaskPool(int argInitSize) : base(argInitSize, () => new VoronoiDiagramTask()) { }

        protected override VoronoiDiagramTask newInstance() {
           return FactoryMethod();
        }
        
        protected override VoronoiDiagramTask[] newArray(int size) {
            return new VoronoiDiagramTask[size];
        }    
    }
}