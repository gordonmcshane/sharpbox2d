/*******************************************************************************
 * Copyright (c) 2013, Daniel Murphy
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 * 	* Redistributions of source code must retain the above copyright notice,
 * 	  this list of conditions and the following disclaimer.
 * 	* Redistributions in binary form must reproduce the above copyright notice,
 * 	  this list of conditions and the following disclaimer in the documentation
 * 	  and/or other materials provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 ******************************************************************************/
/**
 * Created at 4:35:29 AM Jul 15, 2010
 */

using System;
using SharpBox2D.Common;
using SharpBox2D.Particle;

namespace SharpBox2D.Callbacks
{

/**
 * Implement this abstract class to allow JBox2d to automatically draw your physics for debugging
 * purposes. Not intended to replace your own custom rendering routines!
 * 
 * @author Daniel Murphy
 */
    [Flags]
    public enum DebugDrawFlags
    {
        None = 0,
        Shapes = 1 << 1,
        Joints = 1 << 2,
        AABB = 1 << 3,
        Pairs = 1 << 4,
        CenterOfMass = 1 << 5,
        DynamicTree = 1 << 6,
        Wireframe = 1 << 7
    }   

    public abstract class DebugDraw
    {
        protected DebugDrawFlags m_drawFlags;
        protected IViewportTransform viewportTransform;

        protected DebugDraw() : this(null)
        {
        }

        protected DebugDraw(IViewportTransform viewport)
        {
            m_drawFlags = DebugDrawFlags.None;
            viewportTransform = viewport;
        }

        public void setViewportTransform(IViewportTransform viewportTransform)
        {
            this.viewportTransform = viewportTransform;
        }

        public void setFlags(DebugDrawFlags flags)
        {
            m_drawFlags = flags;
        }

        public DebugDrawFlags getFlags()
        {
            return m_drawFlags;
        }

        public void appendFlags(DebugDrawFlags flags)
        {
            m_drawFlags |= flags;
        }

        public void clearFlags(DebugDrawFlags flags)
        {
            m_drawFlags &= ~flags;
        }

        /**
   * Draw a closed polygon provided in CCW order. This implementation uses
   * {@link #drawSegment(Vec2, Vec2, Color4f)} to draw each side of the polygon.
   * 
   * @param vertices
   * @param vertexCount
   * @param color
   */

        public void drawPolygon(Vec2[] vertices, int vertexCount, Color4f color)
        {
            if (vertexCount == 1)
            {
                drawSegment(vertices[0], vertices[0], color);
                return;
            }

            for (int i = 0; i < vertexCount - 1; i += 1)
            {
                drawSegment(vertices[i], vertices[i + 1], color);
            }

            if (vertexCount > 2)
            {
                drawSegment(vertices[vertexCount - 1], vertices[0], color);
            }
        }

        public abstract void drawPoint(Vec2 argPoint, float argRadiusOnScreen, Color4f argColor);

        /**
   * Draw a solid closed polygon provided in CCW order.
   * 
   * @param vertices
   * @param vertexCount
   * @param color
   */
        public abstract void drawSolidPolygon(Vec2[] vertices, int vertexCount, Color4f color);

        /**
   * Draw a circle.
   * 
   * @param center
   * @param radius
   * @param color
   */
        public abstract void drawCircle(Vec2 center, float radius, Color4f color);

        /** Draws a circle with an axis */

        public void drawCircle(Vec2 center, float radius, Vec2 axis, Color4f color)
        {
            drawCircle(center, radius, color);
        }

        /**
   * Draw a solid circle.
   * 
   * @param center
   * @param radius
   * @param axis
   * @param color
   */
        public abstract void drawSolidCircle(Vec2 center, float radius, Vec2 axis, Color4f color);

        /**
   * Draw a line segment.
   * 
   * @param p1
   * @param p2
   * @param color
   */
        public abstract void drawSegment(Vec2 p1, Vec2 p2, Color4f color);

        /**
   * Draw a transform. Choose your own length scale
   * 
   * @param xf
   */
        public abstract void drawTransform(Transform xf);

        /**
   * Draw a string.
   * 
   * @param x
   * @param y
   * @param s
   * @param color
   */
        public abstract void drawString(float x, float y, string s, Color4f color);

        /**
   * Draw a particle array
   * 
   * @param colors can be null
   */
        public abstract void drawParticles(Vec2[] centers, float radius, ParticleColor[] colors, int count);

        /**
   * Draw a particle array
   * 
   * @param colors can be null
   */

        public abstract void drawParticlesWireframe(Vec2[] centers, float radius, ParticleColor[] colors,
            int count);

        /** Called at the end of drawing a world */

        public void flush()
        {
        }

        public void drawString(Vec2 pos, string s, Color4f color)
        {
            drawString(pos.x, pos.y, s, color);
        }

        public IViewportTransform getViewportTranform()
        {
            return viewportTransform;
        }

        /**
   * @param x
   * @param y
   * @param scale
   * @deprecated use the viewport transform in {@link #getViewportTranform()}
   */

        public void setCamera(float x, float y, float scale)
        {
            viewportTransform.setCamera(x, y, scale);
        }


        /**
   * @param argScreen
   * @param argWorld
   */

        public void getScreenToWorldToOut(Vec2 argScreen, Vec2 argWorld)
        {
            viewportTransform.getScreenToWorld(argScreen, argWorld);
        }

        /**
   * @param argWorld
   * @param argScreen
   */

        public void getWorldToScreenToOut(Vec2 argWorld, Vec2 argScreen)
        {
            viewportTransform.getWorldToScreen(argWorld, argScreen);
        }

        /**
   * Takes the world coordinates and puts the corresponding screen coordinates in argScreen.
   * 
   * @param worldX
   * @param worldY
   * @param argScreen
   */

        public void getWorldToScreenToOut(float worldX, float worldY, Vec2 argScreen)
        {
            argScreen.set(worldX, worldY);
            viewportTransform.getWorldToScreen(argScreen, argScreen);
        }

        /**
   * takes the world coordinate (argWorld) and returns the screen coordinates.
   * 
   * @param argWorld
   */

        public Vec2 getWorldToScreen(Vec2 argWorld)
        {
            Vec2 screen = new Vec2();
            viewportTransform.getWorldToScreen(argWorld, screen);
            return screen;
        }

        /**
   * Takes the world coordinates and returns the screen coordinates.
   * 
   * @param worldX
   * @param worldY
   */

        public Vec2 getWorldToScreen(float worldX, float worldY)
        {
            Vec2 argScreen = new Vec2(worldX, worldY);
            viewportTransform.getWorldToScreen(argScreen, argScreen);
            return argScreen;
        }

        /**
   * takes the screen coordinates and puts the corresponding world coordinates in argWorld.
   * 
   * @param screenX
   * @param screenY
   * @param argWorld
   */

        public void getScreenToWorldToOut(float screenX, float screenY, Vec2 argWorld)
        {
            argWorld.set(screenX, screenY);
            viewportTransform.getScreenToWorld(argWorld, argWorld);
        }

        /**
   * takes the screen coordinates (argScreen) and returns the world coordinates
   * 
   * @param argScreen
   */

        public Vec2 getScreenToWorld(Vec2 argScreen)
        {
            Vec2 world = new Vec2();
            viewportTransform.getScreenToWorld(argScreen, world);
            return world;
        }

        /**
   * takes the screen coordinates and returns the world coordinates.
   * 
   * @param screenX
   * @param screenY
   */

        public Vec2 getScreenToWorld(float screenX, float screenY)
        {
            Vec2 screen = new Vec2(screenX, screenY);
            viewportTransform.getScreenToWorld(screen, screen);
            return screen;
        }
    }
}
