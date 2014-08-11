using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpBox2D.Common;
using SharpBox2D.Particle;

namespace SharpBox2D.TestBed.Testbed.Framework.j2d
{
    public class MonoGameDebugDraw : Callbacks.DebugDraw
    {
        public MonoGameDebugDraw()
        {
            _stringData = new List<StringData>();
        }

        public override void drawSolidPolygon(Vec2[] vertices, int count, Color4f color)
        {
            DrawSolidPolygon(vertices, count, color, true);
        }

        private void DrawSolidPolygon(Vec2[] vertices, int count, Color4f color, bool outline)
        {
            if (count == 2)
            {
                drawPolygon(vertices, count, color);
                return;
            }

            Color colorFill = new Color(color.x, color.y, color.z) * (outline ? 0.5f : 1.0f);

            for (int i = 1; i < count - 1; i++)
            {
                _vertsFill[_fillCount * 3].Position = new Vector3(vertices[0].x, vertices[0].y, 0.0f);
                _vertsFill[_fillCount * 3].Color = colorFill;

                _vertsFill[_fillCount * 3 + 1].Position = new Vector3(vertices[i].x, vertices[i].y, 0.0f);
                _vertsFill[_fillCount * 3 + 1].Color = colorFill;

                _vertsFill[_fillCount * 3 + 2].Position = new Vector3(vertices[i+1].x, vertices[i+1].y, 0.0f);
                _vertsFill[_fillCount * 3 + 2].Color = colorFill;

                _fillCount++;
            }

            if (outline)
            {
                drawPolygon(vertices, count, color);
            }
        }

        public override void drawCircle(Vec2 center, float radius, Color4f color)
        {
            int segments = 16;
            double increment = Math.PI * 2.0 / (double)segments;
            double theta = 0.0;

            Color xnaColor = new Color(color.x, color.y, color.z, color.w);

            for (int i = 0; i < segments; i++)
            {
                Vector2 v1 = new Vector2(center.x, center.y) + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                Vector2 v2 = new Vector2(center.x, center.y) + radius * new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

                _vertsLines[_lineCount * 2].Position = new Vector3(v1, 0.0f);
                _vertsLines[_lineCount * 2].Color = xnaColor;
                _vertsLines[_lineCount * 2 + 1].Position = new Vector3(v2, 0.0f);
                _vertsLines[_lineCount * 2 + 1].Color = xnaColor;
                _lineCount++;

                theta += increment;
            }
        }
        private Vec2 zero = new Vec2(0,0);
        public override void drawSolidCircle(Vec2 center, float radius, Vec2 axis, Color4f color)
        {
            int segments = 16;
            double increment = Math.PI * 2.0 / (double)segments;
            double theta = 0.0;

            Color colorFill = new Color(color.x,color.y, color.z) * 0.5f;

            Vector2 v0 = new Vector2(center.x, center.y) + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
            theta += increment;

            for (int i = 1; i < segments - 1; i++)
            {
                Vector2 v1 = new Vector2(center.x, center.y) + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                Vector2 v2 = new Vector2(center.x, center.y) + radius * new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

                _vertsFill[_fillCount * 3].Position = new Vector3(v0, 0.0f);
                _vertsFill[_fillCount * 3].Color = colorFill;

                _vertsFill[_fillCount * 3 + 1].Position = new Vector3(v1, 0.0f);
                _vertsFill[_fillCount * 3 + 1].Color = colorFill;

                _vertsFill[_fillCount * 3 + 2].Position = new Vector3(v2, 0.0f);
                _vertsFill[_fillCount * 3 + 2].Color = colorFill;

                _fillCount++;

                theta += increment;
            }
            drawCircle(center, radius, color);

            if (axis != zero)
                drawSegment(center,  center.add(axis).mul(radius), color);
        }

        public override void drawSegment(Vec2 p1, Vec2 p2, Color4f color)
        {
             Color xnaColor = new Color(color.x, color.y, color.z);
            _vertsLines[_lineCount * 2].Position = new Vector3(p1.x, p1.y, 0.0f);
            _vertsLines[_lineCount * 2 + 1].Position = new Vector3(p2.x, p2.y, 0.0f);
            _vertsLines[_lineCount * 2].Color = _vertsLines[_lineCount * 2 + 1].Color = xnaColor;
            _lineCount++;
        }

        public override void drawTransform(Transform xf)
        {
            float k_axisScale = 0.4f;
            Vec2 p1 = new Vec2(xf.p.x, xf.p.y);

            Vec2 p2;
            p2.x = xf.p.x + k_axisScale * xf.q.c;
            p2.y = xf.p.y + k_axisScale * xf.q.s;

            drawSegment(p1, p2, new Color4f(255,0,0));

            p2.x = xf.p.x + -k_axisScale * xf.q.s;
            p2.y = xf.p.y + k_axisScale * xf.q.c;
            drawSegment(p1, p2, new Color4f(0,255,0));
        }

        public override void drawPoint(Vec2 p, float size, Color4f color)
        {
            Vec2[] verts = new Vec2[4];
            float hs = size / 2.0f;
            verts[0] = p.add(new Vec2(-hs, -hs));
            verts[1] = p.add(new Vec2(hs, -hs));
            verts[2] = p.add(new Vec2(hs, hs));
            verts[3] = p.add(new Vec2(-hs, hs));

            DrawSolidPolygon(verts, 4, color, true);
        }

        public override void drawString(float x, float y, string s, Color4f color)
        {
            _stringData.Add(new StringData(x, y, s, color));
        }

        public override void drawParticles(Vec2[] centers, float radius, ParticleColor[] colors, int count)
        {
            Color4f pcolorA = new Color4f(1f, 1f, 1f);
            for (int i = 0; i < count; i++)
            {
                Vec2 center = centers[i];
                Color4f color;
                if (colors == null)
                {
                    color = pcolorA;
                }
                else
                {
                    ParticleColor c = colors[i];
                    color = new Color4f(c.r*1f/127, c.g*1f/127, c.b*1f/127, c.a*1f/127);
                }
                drawSolidCircle(center, radius, zero, color);
            }
        }

        public override void drawParticlesWireframe(Vec2[] centers, float radius, ParticleColor[] colors, int count)
        {
            Color4f pcolorA = new Color4f(1f, 1f, 1f);
            for (int i = 0; i < count; i++)
            {
                Vec2 center = centers[i];
                Color4f color;
                if (colors == null)
                {
                    color = pcolorA;
                }
                else
                {
                    ParticleColor c = colors[i];
                    color = new Color4f(c.r * 1f / 127, c.g * 1f / 127, c.b * 1f / 127, c.a * 1f / 127);
                }
                drawCircle(center, radius, color);
            }
        }


        public void FinishDrawShapes()
        {
            _device.BlendState = BlendState.AlphaBlend;

            //_device.RenderState.CullMode = CullMode.None;
            //_device.RenderState.AlphaBlendEnable = true;

            if (_fillCount > 0)
                _device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, _vertsFill, 0, _fillCount);

            if (_lineCount > 0)
                _device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, _vertsLines, 0, _lineCount);

            _lineCount = _fillCount = 0;
        }

        public void FinishDrawString()
        {
            for (int i = 0; i < _stringData.Count; i++)
            {
                var text = _stringData[i].s;
                _batch.DrawString(_font, text, new Vector2(_stringData[i].x, _stringData[i].y), _stringData[i].color);
            }

            _stringData.Clear();
        }

        
        public static VertexPositionColor[] _vertsLines = new VertexPositionColor[100000];
        public static VertexPositionColor[] _vertsFill = new VertexPositionColor[100000];
        public static int _lineCount;
        public static int _fillCount;
        public static SpriteBatch _batch;
        public static SpriteFont _font;
        public static GraphicsDevice _device;

        private List<StringData> _stringData;
        struct StringData
        {
            public StringData(float x, float y, string s, Color4f color)
            {
                this.x = x;
                this.y = y;
                this.s = s;
                this.color = new Color(color.x, color.y, color.z, color.w);
            }

            public float x, y;
            public string s;
            public Color color;
        }
    }
}
