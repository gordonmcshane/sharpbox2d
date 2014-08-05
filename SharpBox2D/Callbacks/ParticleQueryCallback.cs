namespace SharpBox2D.Callbacks
{

    /**
     * Callback class for AABB queries. See
     * {@link World#queryAABB(QueryCallback, org.jbox2d.collision.AABB)}.
     * 
     * @author dmurph
     * 
     */
    public interface ParticleQueryCallback
    {
        /**
         * Called for each particle found in the query AABB.
         * 
         * @return false to terminate the query.
         */
        bool reportParticle(int index);
    }
}
